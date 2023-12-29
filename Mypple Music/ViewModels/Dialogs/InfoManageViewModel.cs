using IdentityService.WebAPI.Login.Request;
using MaterialDesignThemes.Wpf;
using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Mypple_Music.Models;
using Mypple_Music.Models.Request;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels.Dialogs
{
    public class InfoManageViewModel : BindableBase, IDialogHostAware
    {
        #region Field
        public string DialogHostName { get; set; } = "InfoManageView";
        private PeriodicTimer Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        private DateTime autoStartingActionCountdownStart;
        private readonly IEventAggregator aggregator;
        private Regex phoneRegex = new Regex(@"^(((13[0-9]{1})|(15[0-35-9]{1})|(17[0-9]{1})|(18[0-9]{1}))+\d{8})$");
        private Regex emailRegex = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        private readonly ILoginService loginService;
        #endregion

        #region Poperty
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand<string> SendCodeCommand { get; set; }
        public DelegateCommand<string> ConfirmCommand { get; set; }
        public DelegateCommand<string> ExecuteCommand { get; set; }

        private SimpleUser userDTO;

        public SimpleUser UserDTO
        {
            get { return userDTO; }
            set { userDTO = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }

        private bool showSendButton = true;

        public bool ShowSendButton
        {
            get { return showSendButton; }
            set { showSendButton = value; RaisePropertyChanged(); }
        }

        private string restartCountdownText;

        public string RestartCountdownText
        {
            get { return restartCountdownText; }
            set { restartCountdownText = value; RaisePropertyChanged(); }
        }

        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(); }
        }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private string passwordConfirm;

        public string PasswordConfirm
        {
            get { return passwordConfirm; }
            set { passwordConfirm = value; RaisePropertyChanged(); }
        }

        private bool isButtonEnable = true;

        public bool IsButtonEnable
        {
            get { return isButtonEnable; }
            set { isButtonEnable = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Ctor
        public InfoManageViewModel(ILoginService loginService, IEventAggregator aggregator)
        {
            this.loginService = loginService;
            this.aggregator = aggregator;
            CancelCommand = new DelegateCommand(Cancel);
            SendCodeCommand = new DelegateCommand<string>(SendCode);
            ExecuteCommand = new DelegateCommand<string>(Execute);
            ConfirmCommand = new DelegateCommand<string>(Confirm);
        }
        #endregion

        #region Command
        private async void Confirm(string obj)
        {
            if (obj == string.Empty)
                return;
            autoStartingActionCountdownStart = DateTime.Now;
            ShowSendButton = false;
            if (phoneRegex.IsMatch(obj))
            {
                var res = await loginService.ConfirmPhone(obj);
                if (!res.Status)
                {
                    aggregator.SendMessage(res.Message, "InfoManage");
                }
            }
            else if (emailRegex.IsMatch(obj))
            {
                await loginService.SendCodeByEmail(new SendCodeRequest(obj));
            }
            else
                return;
        }

        private async void Execute(string obj)
        {
            if (obj == "修改密码")
            {
                IsButtonEnable = false;
                if (Account == null || Code == null || Password == null)
                {
                    aggregator.SendMessage("字段不能为空！", "InfoManage");
                    IsButtonEnable = true;
                    return;
                }
                if (Password != PasswordConfirm)
                {
                    aggregator.SendMessage("两次密码不一致！", "InfoManage");
                    IsButtonEnable = true;
                    return;
                }
                var res = await loginService.ChangePasswordWithCode(new ChangePasswordWithCodeRequest(Account, Code, Password, PasswordConfirm));
                if (res.Status)
                {
                    aggregator.SendMessage("修改成功", "InfoManage");
                }
                else
                {
                    aggregator.SendMessage(res.Message, "InfoManage");
                }
                IsButtonEnable = true;
            }
            else if (obj == "验证")
            {
                ShowSendButton = true;
                if (Code.Trim() == string.Empty)
                {
                    aggregator.SendMessage("验证码不能为空！", "InfoManage");
                }
                if (SelectedIndex == 3)
                {
                    var res = await loginService.LoginByPhoneAndCode(new LoginByPhoneAndCodeRequest(UserDTO.PhoneNumber, Code));
                    if (res.Status)
                    {
                        SelectedIndex = 1;
                        aggregator.SendMessage("验证成功！", "InfoManage");
                        Code = "";
                        return;
                    }
                }
                else
                {
                    var res = await loginService.LoginByEmailAndCode(new LoginByEmailAndCodeRequest(UserDTO.Email, Code));
                    if (res.Status)
                    {
                        SelectedIndex = 0;
                        aggregator.SendMessage("验证成功！", "InfoManage");
                        Code = "";
                        return;
                    }
                }
                aggregator.SendMessage("验证失败！", "InfoManage");
            }
            else if (obj == "绑定")
            {
                ShowSendButton = true;
                if (Account.Trim() == string.Empty || Code.Trim() == string.Empty)
                {
                    aggregator.SendMessage("字段不能为空", "InfoManage");
                }
                ApiResponse<string?> res = null;
                if (SelectedIndex == 0)
                {
                    res = await loginService.ChangeEmail(new ChangePhoneOrEmailRequest(Account, Code));
                    if (res.Status)
                    {
                        UserDTO.Email = Account;
                    }
                }
                else
                {
                    res = await loginService.ChangePhoneNum(new ChangePhoneOrEmailRequest(Account, Code));
                    if (res.Status)
                    {
                        UserDTO.PhoneNumber = Account;
                    }
                }

                aggregator.SendMessage(res.Message, "InfoManage");

            }
        }

        private async void SendCode(string obj)
        {
            if (obj.Trim() == string.Empty)
            {
                aggregator.SendMessage("字段不可为空！", "InfoView");
                return;
            }
                
            autoStartingActionCountdownStart = DateTime.Now;
            ShowSendButton = false;

            if (phoneRegex.IsMatch(obj))
            {
                await loginService.SendCodeByPhone(new SendCodeRequest(obj));
            }
            else if (emailRegex.IsMatch(obj))
            {
                await loginService.SendCodeByEmail(new SendCodeRequest(obj));
            }
            else
                return;

        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
            Timer.Dispose();
        }

        public async Task OnDialogOpendAsync(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("SelectedIndex"))
            {
                SelectedIndex = parameters.GetValue<int>("SelectedIndex");
            }
            if (parameters.ContainsKey("User"))
            {
                UserDTO = parameters.GetValue<SimpleUser>("User");
            }
            while (await Timer.WaitForNextTickAsync())
            {
                if (!ShowSendButton)
                {
                    var totalDuration = autoStartingActionCountdownStart.AddSeconds(60);
                    var span = totalDuration - DateTime.Now;
                    RestartCountdownText = $"请{span.Seconds}秒后重试";
                    if (span.Seconds == 0)
                        ShowSendButton = true;
                    Debug.WriteLine(span.Seconds);
                }
            }
        }
        #endregion
    }
}
