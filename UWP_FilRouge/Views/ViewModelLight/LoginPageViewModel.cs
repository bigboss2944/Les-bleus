using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
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

        public LoginPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Title = "Login Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToSellerPage = new RelayCommand(ToSellerPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
        }

        private void ToRegisterPage()
        {
            // Do Something
            _navigationService.NavigateTo("Register Page");
        }

        private void ToSellerPage()
        {
            // Do Something
            _navigationService.NavigateTo("Seller Main Page");
        }

        private void ToCustomerPage()
        {
            // Do Something
            _navigationService.NavigateTo("Customer Main Page");
        }

        private void ToOrderPage()
        {
            // Do Something
            _navigationService.NavigateTo("Order Main Page");
        }
    }
}

