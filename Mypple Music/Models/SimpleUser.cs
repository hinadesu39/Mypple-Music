using Mypple_Music.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models
{
    public class SimpleUser : BindableBase
    {
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string? gender;

        public string? Gender
        {
            get { return gender; }
            set { gender = value; RaisePropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private Uri? userAvatar;
        public Uri? UserAvatar
        {
            get { return userAvatar; }
            set
            {
                userAvatar = value;
                CreateLocalPicAsync(userAvatar);
                RaisePropertyChanged();
            }
        }

        private DateTime? birthDay;

        public DateTime? BirthDay
        {
            get { return birthDay; }
            set { birthDay = value; RaisePropertyChanged(); }
        }


        private string localPicUrl;
        public string LocalPicUrl
        {
            get { return localPicUrl; }
            set
            {
                localPicUrl = value;
                RaisePropertyChanged();
            }
        }

        private string? email;

        public string? Email
        {
            get { return email; }
            set { email = value; RaisePropertyChanged(); }
        }

        private string? phoneNumber;

        public string? PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; RaisePropertyChanged(); }
        }

        async void CreateLocalPicAsync(Uri picUrl)
        {
            LocalPicUrl = await DownloadHelper.GetImageAsync(picUrl);
        }
    }
}
