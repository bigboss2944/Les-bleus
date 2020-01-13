using Entities;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static UWP_FilRouge.Views.ViewModelLight.ViewModelLocator;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class CreateUserViewModel
    {
        private INavigationService navigationService;

        public User User { get; set; }

        public string ButtonContent { get; set; }


        public ICommand ButtonClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    this.navigationService.NavigateTo(Pages.ListUserView.ToString());
                    Debug.WriteLine(User.Firstname);
                    Debug.WriteLine(User.Lastname);
                });
            }
        }

        

        public CreateUserViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.User = new User();
        }
    }
}