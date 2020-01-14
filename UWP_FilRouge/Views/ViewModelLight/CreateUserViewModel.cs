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
using Windows.UI.Xaml.Navigation;
using static UWP_FilRouge.Views.ViewModelLight.ViewModelLocator;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class CreateUserViewModel : INavigationEvent
    {
        private INavigationService navigationService;

        public User user { get; set; }

        public string ButtonContent { get; set; }


        public ICommand ButtonClick
        {
            get
            {
                return new RelayCommand(() =>
                {
                    this.navigationService.NavigateTo(Pages.ListUserView.ToString(),user);
                    Debug.WriteLine(user.Firstname);
                    Debug.WriteLine(user.Lastname);
                });
            }
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Enter OtherPage");
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("Leave OtherPage");
        }


        public CreateUserViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.user = new User();
        }

        public interface INavigationEvent
        {
            void OnNavigatedTo(NavigationEventArgs e);
            void OnNavigatedFrom(NavigationEventArgs e);
        }
    }
}