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
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModel
{
    public class SellerMainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INavigationService navigationService;
        public RelayCommand MoveToRegisterPage { get; private set; }
        public RelayCommand MoveToLoginPage { get; private set; }
        public RelayCommand MoveToSellerPage { get; private set; }
        public RelayCommand MoveToCustomerPage { get; private set; }
        public RelayCommand MoveToOrderPage { get; private set; }
        private bool _isLoading = false;
        private bool _addingNewSeller = false;

        public SellerPageAccessor Datas { get; set; }
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

        public SellerMainPageViewModel(INavigationService navigationService, DatabaseService databaseService)
        {
            this.navigationService = navigationService;
            this.databaseService = databaseService;
            SetupSellerDatas();
            Title = "Seller Main Page";
            MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
            //MoveToSellerPage = new RelayCommand(ToSellerPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
        }

        private void SetupSellerDatas()
        {
            Datas = new SellerPageAccessor();
            SetupSellerEdit();
            SetupSellerList();
            SetupSellerShow();
        }

        private void SetupSellerEdit()
        {
            Datas.sellerEdit.button.Content = "Valider";
            Datas.sellerEdit.button.Action = new RelayCommand(SellerEditCommand);
            Datas.sellerEdit.seller = new Seller();
        }

        private void SellerEditCommand()
        {
            Seller seller = new Seller();
            seller.CopyFrom(Datas.sellerEdit.seller);

            try
            {
                databaseService.SqliteConnection.Insert(seller);
                Datas.sellerList.sellers.Add(seller);
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

        private void SetupSellerShow()
        {
            Datas.sellerShow.seller = new Seller();
        }

        private void SetupSellerList()
        {
            Datas.sellerList.sellers = new ObservableCollection<Seller>();
            foreach (var item in databaseService.Sellers)
            {
                Datas.sellerList.sellers.Add(item);
            }
            Datas.sellerList.listView.SelectedItem = new Seller();
            Datas.sellerList.listView.SelectionChanged = new RelayCommand(SellerListSelectionChanged);
        }

        private void SellerListSelectionChanged()
        {
            Seller seller = Datas.sellerList.listView.SelectedItem;
            if (seller != null)
            {
                Datas.sellerShow.seller.CopyFrom(seller);
            }
        }


        private void ToRegisterPage()
        {
            // Do Something
            navigationService.NavigateTo("Register Page");
        }

        private void ToLoginPage()
        {
            // Do Something
            navigationService.NavigateTo("Login Page");
        }

        private void ToCustomerPage()
        {
            // Do Something
            navigationService.NavigateTo("Customer Main Page");
        }

        private void ToOrderPage()
        {
            // Do Something
            navigationService.NavigateTo("Customer Main Page");
        }
    }
}
