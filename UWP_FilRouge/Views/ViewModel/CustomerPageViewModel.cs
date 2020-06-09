
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.ObjectModel;
using UWP_FilRouge.Database;
using UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModel
{
    public class CustomerPageViewModel : ViewModelBase
    {
        private INavigationService navigationService;
        private DatabaseService databaseService;

        public CustomerPageAccessor Datas { get; set; }

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

        public CustomerPageViewModel(INavigationService navigationService, DatabaseService databaseService)
        {
            this.navigationService = navigationService;
            this.databaseService = databaseService;
            SetupCustomerDatas();
            Title = "Customer Page";
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            //MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
        }

        private void SetupCustomerDatas()
        {
            Datas = new CustomerPageAccessor();
            SetupCustomerEdit();
            SetupCustomerList();
            SetupCustomerShow();
        }

        private void SetupCustomerEdit()
        {
            Datas.customerEdit.button.Content = "Valider";
            Datas.customerEdit.button.Action = new RelayCommand(CustomerEditCommand);
            Datas.customerEdit.customer = new Customer();
        }

        private void CustomerEditCommand()
        {
            Customer customer = new Customer();
            customer.CopyFrom(Datas.customerEdit.customer);

            try
            {
                databaseService.SqliteConnection.Insert(customer);
                Datas.customerList.customers.Add(customer);
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

        private void SetupCustomerShow()
        {
            Datas.customerShow.customer = new Customer();
        }

        private void SetupCustomerList()
        {
            Datas.customerList.customers = new ObservableCollection<Customer>();
            foreach (var item in databaseService.Customers)
            {
                Datas.customerList.customers.Add(item);
            }
            Datas.customerList.listView.SelectedItem = new Customer();
            Datas.customerList.listView.SelectionChanged = new RelayCommand(CustomerListSelectionChanged);
        }

        private void CustomerListSelectionChanged()
        {
            Customer customer = Datas.customerList.listView.SelectedItem;
            if (customer != null)
            {
                Datas.customerShow.customer.CopyFrom(customer);
            }
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
   
    }
}
