using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Database;
using UWP_FilRouge.Entities;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModel
{
    public class AdminPageViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToHomePage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        public RelayCommand MoveToProductPage { get; private set; }
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

        private bool _addingNewSeller = false;

        public SellerPageAccessor DataSeller { get; set; }
        private DatabaseService databaseService;

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

        public AdminPageViewModel(INavigationService navigationService, DatabaseService databaseService)
        {
            this.navigationService = navigationService;
            this.databaseService = databaseService;
            SetupSellerDatas();

            Title = "Admin Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
            MoveToSellerPage = new RelayCommand(ToSellerPage);
            MoveToProductPage = new RelayCommand(ToProductPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
        }

        

        private void SetupSellerDatas()
        {
            DataSeller = new SellerPageAccessor();
            SetupSellerEdit();
            SetupSellerList();
            SetupSellerUpdate();
        }

        private void SetupSellerEdit()
        {
            DataSeller.sellerEdit.validateButton.Content = "Valider";
            DataSeller.sellerEdit.validateButton.Action = new RelayCommand(SellerEditCommand);
            DataSeller.sellerEdit.cancelButton.Content = "Cancel";
            DataSeller.sellerEdit.cancelButton.Action = new RelayCommand(SellerEditCancel);
            DataSeller.sellerEdit.seller = new Seller();
        }

        private void SellerEditCancel()
        {
            navigationService.GoBack();
        }

        private void SellerEditCommand()
        {
            Seller seller = new Seller();
            seller.CopyFrom(DataSeller.sellerEdit.seller);

            try
            {
                databaseService.SqliteConnection.Insert(seller);
                DataSeller.sellerList.sellers.Add(seller);
            }
            catch (Exception e)
            {
                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "Error";
                contentDialog.Content = e.Message;
                contentDialog.IsSecondaryButtonEnabled = false;
                contentDialog.PrimaryButtonText = "ok";
                contentDialog.ShowAsync();
            }
        }

        private void SetupSellerUpdate()
        {
            DataSeller.sellerUpdate.validateButton.Content = "Valider";
            DataSeller.sellerUpdate.validateButton.Action = new RelayCommand(SellerEditCommand);
            DataSeller.sellerUpdate.cancelButton.Content = "Cancel";
            DataSeller.sellerUpdate.cancelButton.Action = new RelayCommand(SellerEditCancel);
            DataSeller.sellerUpdate.seller = new Seller();
        }

        private void SetupSellerList()
        {
            DataSeller.sellerList.sellers = new ObservableCollection<Seller>();
            foreach (var item in databaseService.Sellers)
            {
                DataSeller.sellerList.sellers.Add(item);
            }
            DataSeller.sellerList.listView.SelectedItem = new Seller();
            DataSeller.sellerList.listView.SelectionChanged = new RelayCommand(SellerListSelectionChanged);
        }

        private void SellerListSelectionChanged()
        {
            Seller seller = DataSeller.sellerList.listView.SelectedItem;
            if (seller != null)
            {
                DataSeller.sellerUpdate.seller.CopyFrom(seller);
            }
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

        private void ToSellerPage()
        {
            navigationService.NavigateTo("Seller Main Page");
        }

        private void ToCustomerPage()
        {
            navigationService.NavigateTo("Customer Main Page");
        }

        private void ToProductPage()
        {
            navigationService.NavigateTo("Product Page");
        }

        private void ToOrderPage()
        {
            navigationService.NavigateTo("Order Main Page");
        }
    }
}

