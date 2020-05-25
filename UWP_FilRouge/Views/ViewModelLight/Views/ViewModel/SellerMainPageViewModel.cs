using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class SellerMainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        private bool _isLoading = false;
        private bool _addingNewSeller = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool AddingNewSeller
        {
            get => _addingNewSeller;
            set
            {
                if (_addingNewSeller != value)
                {
                    _addingNewSeller = value;
                    OnPropertyChanged();
                }
            }
        }

        

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

        public SellerMainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Title = "Seller Main Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
            //MoveToSellerPage = new RelayCommand(ToSellerPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
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

        private void ToCustomerPage()
        {
            // Do Something
            _navigationService.NavigateTo("Customer Main Page");
        }

        private void ToOrderPage()
        {
            // Do Something
            _navigationService.NavigateTo("Customer Main Page");
        }
    }
}
