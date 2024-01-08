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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input.StylusPlugIns;

namespace Mypple_Music.ViewModels.Dialogs
{
    public class LoginViewModel : BindableBase, IDialogHostAware
    {
        #region Field
        public static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private Regex phoneRegex = new Regex(@"^(((13[0-9]{1})|(15[0-35-9]{1})|(17[0-9]{1})|(18[0-9]{1})|(19[0-9]{1}))+\d{8})$");
        private Regex emailRegex = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
        private readonly IEventAggregator aggregator;
        private PeriodicTimer Timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
        private DateTime autoStartingActionCountdownStart;
        private readonly ILoginService loginService;
        #endregion

        #region Property
        private bool isRemember = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsRemember"));
        public bool IsRemember
        {
            get { return isRemember; }
            set
            {
                isRemember = value;
                RaisePropertyChanged();
                config.AppSettings.Settings["IsRemember"].Value = isRemember.ToString();
                config.Save();
            }
        }

        private bool isAutoLogin = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsAutoLogin"));
        public bool IsAutoLogin
        {
            get { return isAutoLogin; }
            set
            {
                isAutoLogin = value;
                RaisePropertyChanged();
                config.AppSettings.Settings["IsAutoLogin"].Value = IsAutoLogin.ToString();
                config.Save();
            }
        }

        private bool isLoginEnable = true;

        public bool IsLoginEnable
        {
            get { return isLoginEnable; }
            set { isLoginEnable = value; RaisePropertyChanged(); }
        }

        private bool isRegisterEnable = true;

        public bool IsRegisterEnable
        {
            get { return isRegisterEnable; }
            set { isRegisterEnable = value; RaisePropertyChanged(); }
        }

        private int selectedSlider;

        public int SelectedSlider
        {
            get { return selectedSlider; }
            set { selectedSlider = value; RaisePropertyChanged(); }
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

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string phoneNum;

        public string PhoneNum
        {
            get { return phoneNum; }
            set { phoneNum = value; RaisePropertyChanged(); }
        }

        private string code;

        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged(); }
        }

        private string registerPwd;

        public string RegisterPwd
        {
            get { return registerPwd; }
            set { registerPwd = value; RaisePropertyChanged(); }
        }

        private string repeatPwd;

        public string RepeatPwd
        {
            get { return repeatPwd; }
            set { repeatPwd = value; RaisePropertyChanged(); }
        }

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand<string> ExecuteCommand { get; set; }
        public DelegateCommand<string> ChangeTransitionerSlideCommand { get; set; }
        public DelegateCommand<string> SendCodeCommand { get; set; }
        #endregion

        #region Ctor
        public LoginViewModel(ILoginService loginService, IEventAggregator aggregator)
        {
            this.loginService = loginService;
            this.aggregator = aggregator;

            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            ExecuteCommand = new DelegateCommand<string>(Execute);
            ChangeTransitionerSlideCommand = new DelegateCommand<string>(ChangeTransitionerSlide);
            SendCodeCommand = new DelegateCommand<string>(SendCode);

            if (Convert.ToBoolean(ConfigurationManager.AppSettings.Get("IsRemember")))
            {
                Account = ConfigurationManager.AppSettings.Get("Account");
                Password = ConfigurationManager.AppSettings.Get("Password");
            }
        }
        #endregion

        #region Command
        public async Task OnDialogOpendAsync(IDialogParameters parameters)
        {
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

        private async void SendCode(string obj)
        {
            if (string.IsNullOrEmpty(obj))
                return;
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

        private void ChangeTransitionerSlide(string obj)
        {
            if (obj == "注册账号")
            {
                SelectedSlider = 1;
            }
            else if (obj == "忘记密码")
            {
                SelectedSlider = 2;
            }
            else if (obj == "返回")
            {
                SelectedSlider = 0;
            }
            else if (obj == "其它方式登录")
            {
                SelectedSlider = 3;
            }
        }

        private async void Execute(string obj)
        {
            if (obj == "注册")
            {
                if (RegisterPwd != RepeatPwd)
                {
                    aggregator.SendMessage("密码不一致", "Login");
                    return;
                }
                if (UserName == null || PhoneNum == null || Code == null || RegisterPwd == null || RepeatPwd == null)
                {
                    aggregator.SendMessage("字段不能为空", "Login");
                    return;
                }

                IsRegisterEnable = false;
                var res = await loginService.CreateUserWithPhoneNumAndCode(new CreateUserWithPhoneNumAndCodeRequest(UserName, PhoneNum, RegisterPwd, Code));
                if (res.Status == true)
                {
                    Account = res.Result!.UserName;
                    SelectedSlider = 0;
                }
                else
                {
                    aggregator.SendMessage(res.Message, "Login");
                }
                IsRegisterEnable = true;
            }
            else if (obj == "其它方式登录")
            {
                IsLoginEnable = false;
                ApiResponse<string?> res = null;
                if (phoneRegex.IsMatch(Account))
                {
                    res = await loginService.LoginByPhoneAndCode(new LoginByPhoneAndCodeRequest(Account, Code));
                }
                else if (emailRegex.IsMatch(Account))
                {
                    res = await loginService.LoginByEmailAndCode(new LoginByEmailAndCodeRequest(Account, Code));
                }
                else
                    return;

                if (res != null && res.Status == true)
                {
                    AppSession.JWTToken = res.Result;
                    var user = await loginService.GetUserInfo();
                    if (user != null)
                    {
                        DialogParameters parameters = new DialogParameters();
                        parameters.Add("User", user);
                        DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
                    }
                }
                else
                {
                    aggregator.SendMessage(res.Message, "Login");
                }
                IsLoginEnable = true;
            }
            else if (obj == "修改密码")
            {
                IsLoginEnable = false;
                if (Account == null || Code == null || Password == null)
                {
                    aggregator.SendMessage("字段不能为空！", "Login");
                    IsLoginEnable = true;
                    return;
                }
                var res = await loginService.ChangePasswordWithCode(new ChangePasswordWithCodeRequest(Account, Code, Password, Password));
                if (res.Status)
                {
                    SelectedSlider = 0;
                    aggregator.SendMessage("修改成功", "Login");
                }
                else
                {
                    aggregator.SendMessage(res.Message, "Login");
                }
                IsLoginEnable = true;
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
            Timer.Dispose();
            if (IsRemember)
            {
                config.AppSettings.Settings["Account"].Value = Account ?? "".ToString();
                config.AppSettings.Settings["Password"].Value = Password ?? "".ToString();
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Account))
            {
                aggregator.SendMessage("账号和密码不能为空！", "Login");
                return;
            }
            IsLoginEnable = false;
            ApiResponse<string?> res = null;
            if (emailRegex.IsMatch(Account))
            {
                res = await loginService.LoginByEmailAndPwd(new LoginByEmailAndPwdRequest(Account, Password));
            }
            else if (phoneRegex.IsMatch(Account))
            {
                res = await loginService.LoginByPhoneAndPwd(new LoginByPhoneAndPwdRequest(Account, Password));
            }
            else
            {
                res = await loginService.LoginByUserNameAndPwd(new LoginByUserNameAndPwdRequest(Account, Password));
            }
            if (res.Status == true)
            {
                AppSession.JWTToken = res.Result;
                var user = await loginService.GetUserInfo();
                if (user != null)
                {
                    DialogParameters parameters = new DialogParameters();
                    parameters.Add("User", user);
                    DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
                }
            }
            else
            {
                aggregator.SendMessage(res.Message, "Login");
                AppSession.JWTToken = "";
            }

            IsLoginEnable = true;
            if (IsRemember)
            {
                config.AppSettings.Settings["Account"].Value = Account.ToString();
                config.AppSettings.Settings["Password"].Value = Password.ToString();
                config.AppSettings.Settings["JWTToken"].Value = AppSession.JWTToken;
                config.Save();
            }
        }
        #endregion
    }
}
