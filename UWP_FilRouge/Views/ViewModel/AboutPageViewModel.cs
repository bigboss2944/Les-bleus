using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModel
{
    public class AboutPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToAboutPage { get; private set; }
        public RelayCommand MoveToContactPage { get; private set; }
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

        public AboutPageViewModel(INavigationService navigationService)
        {
            navigationService = navigationService;
            Title = "About Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            //MoveToOrderPage = new RelayCommand(ToOrderPage);
            //MoveToSellerPage = new RelayCommand(ToSellerPage);
            //MoveToCustomerPage = new RelayCommand(ToCustomerPage);
        }

        private void ToHomePage()
        {
            // Do Something
            _navigationService.NavigateTo("Home Page");
        }

        private void ToContactPage()
        {
            // Do Something
            _navigationService.NavigateTo("Contact Page");
        }

        private void ToRegisterPage()
        {
            // Do Something
            _navigationService.NavigateTo("Register Page");
        }

        private void ToLoginPage()
        {
            // Do Something
            _navigationService.NavigateTo("Login Page");
        }

        private void ToSellerPage()
        {
            // Do Something
            _navigationService.NavigateTo("Seller Main Page");
        }

        private void ToOrderPage()
        {
            // Do Something
            _navigationService.NavigateTo("Order Main Page");
        }

    }
}
