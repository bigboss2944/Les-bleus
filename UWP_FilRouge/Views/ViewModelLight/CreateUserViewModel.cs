using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class CreateUserViewModel
    {
        private INavigationService navigationService;

        //public User User { get; set; }
        public string ButtonContent { get; set; }
        public ICommand ButtonClick => new RelayCommand(() =>
        {
            this.navigationService.NavigateTo("OtherPage");
            
        });

        public CreateUserViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            
            //this.ButtonContent = "change";
        }
    }
}