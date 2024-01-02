using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class SearchViewModel : NavigationViewModel
    {
        public SearchViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
        }
    }
}
