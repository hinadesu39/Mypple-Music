using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class UserCenterViewModel : NavigationViewModel
    {
        public UserCenterViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
        }
    }
}
