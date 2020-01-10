using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class BlankPageViewModel
    {
        private INavigationService navigationService;

        public User User { get; set; }
        public string ButtonContent { get; set; }
        

        public BlankPageViewModel(INavigationService navigationService)
        {
            
            this.ButtonContent = "change";
        }

    }
}
