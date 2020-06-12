using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;

namespace UWP_FilRouge.Views.ViewModel
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToContactPage { get; private set; }
        public RelayCommand MoveToAboutPage { get; private set; }
        public RelayCommand ValidateButton { get; private set; }
        public RelayCommand CancelButton { get; private set; }

        

        private String firstName;
        private String password;

        private String Admin = "Admin";
        private String passwordAdmin = "Admin";

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

        public string FirstName { get => firstName; set => firstName = value; }
        public string Password { get => password; set => password = value; }

        public LoginPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            Title = "Login Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
            ValidateButton = new RelayCommand(Validate);
            CancelButton = new RelayCommand(Cancel);
        }

        private void ToRegisterPage()
        {
            // Do Something
            navigationService.NavigateTo("Register Page");
        }

        private void ToHomePage()
        {
            // Do Something
            navigationService.NavigateTo("Home Page");
        }

        private void ToAboutPage()
        {
            // Do Something
            navigationService.NavigateTo("About Page");
        }

        private void ToContactPage()
        {
            // Do Something
            navigationService.NavigateTo("Contact Page");
        }

        private void Validate()
        {
            // Do Something
            if ((firstName == Admin) && (password == passwordAdmin))
            {
                navigationService.NavigateTo("Admin Page");//Mode admin
            }
            else
            {

            }

        }

        private void Cancel()
        {
            navigationService.GoBack();//Mode admin
            
            
        }
    }
}

