
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UWP_FilRouge.Database;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModel
{
    public class MainMenuPageViewModel : ViewModelBase
    {
        private INavigationService navigationService;
        

        

        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToContactPage { get; private set; }
        public RelayCommand MoveToAboutPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        public RelayCommand MoveToBicyclePage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }

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

        public MainMenuPageViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
           
            
            Title = "Customer Main Page";
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            //MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
            MoveToBicyclePage = new RelayCommand(ToBicyclePage);
        }

        private void ToLoginPage()
        {
            //Do Something
            navigationService.NavigateTo("Login Page");

            // or use the GoBack if you like to implement a back button functionality

            //_navigationService.GoBack(); 
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

        private void ToOrderPage()
        {
            // Do Something
            navigationService.NavigateTo("Order Main Page");
        }

        private void ToCustomerPage()
        {
            // Do Something
            navigationService.NavigateTo("Customer Main Page");
        }

        private void ToBicyclePage()
        {
            navigationService.NavigateTo("Bicycle Main Page");
        }


    }
}
