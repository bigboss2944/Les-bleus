
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToContactPage { get; private set; }
        public RelayCommand MoveToAboutPage { get; private set; }
        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");

            }
        }
        private string _title;
        public string Title
        {

            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        public RegisterPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Title = "Register Page";
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            //MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
        }

        private void ToLoginPage()
        {
            //Do Something
            _navigationService.NavigateTo("Login Page");

            // or use the GoBack if you like to implement a back button functionality

            //_navigationService.GoBack(); 
        }

        private void ToHomePage()
        {
            // Do Something
            _navigationService.NavigateTo("Home Page");
        }

        private void ToAboutPage()
        {
            // Do Something
            _navigationService.NavigateTo("About Page");
        }

        private void ToContactPage()
        {
            // Do Something
            _navigationService.NavigateTo("Contact Page");
        }

        

        
    }
}
