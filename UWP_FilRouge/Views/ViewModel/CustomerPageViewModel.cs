
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
    public class CustomerPageViewModel : ViewModelBase
    {
        private INavigationService navigationService;
        private DatabaseService databaseService;

        public CustomerPageAccessor dataCustomer { get; set; }

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
            SetupDataCustomer();
            Title = "Customer Main Page";
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            //MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToHomePage = new RelayCommand(ToHomePage);
            MoveToContactPage = new RelayCommand(ToContactPage);
            MoveToAboutPage = new RelayCommand(ToAboutPage);
        }

        private void SetupDataCustomer()
        {
            dataCustomer = new CustomerPageAccessor();
            SetupCustomerEdit();
            SetupCustomerList();
            SetupCustomerUpdate();
        }

        private void SetupCustomerEdit()
        {
            dataCustomer.customerEdit.validateButton.Content = "Valider";
            dataCustomer.customerEdit.validateButton.Action = new RelayCommand(CustomerEditCommand);
            dataCustomer.customerEdit.cancelButton.Content = "Cancel";
            dataCustomer.customerEdit.cancelButton.Action = new RelayCommand(CustomerEditCancel); //Press on cancelButton to activate the SellerEdit function
            dataCustomer.customerEdit.customer = new Customer();
        }

        private void CustomerEditCancel()
        {
            navigationService.GoBack();
        }

        private void CustomerEditCommand()
        {
            Customer customer = new Customer();
            customer.CopyFrom(dataCustomer.customerEdit.customer);
            bool insert = true;

            try
            {
                foreach (var item in databaseService.Customers)
                {
                    Debug.WriteLine("{0}", item.Id);
                    if (customer.Id == item.Id) //check if the id is not already present in the Seller table
                    {
                        Debug.WriteLine("{0}", item.Id);
                        databaseService.SqliteConnection.Update(customer);
                        dataCustomer.customerList.customers.IndexOf(customer);
                        insert = false;
                        break;
                    }
                    else
                    {
                        continue;

                    }

                }

                if (insert)
                {
                    databaseService.SqliteConnection.Insert(customer);// Insert Seller in the database
                    dataCustomer.customerList.customers.Add(customer);
                    dataCustomer.customerUpdate.customers.Add(customer);
                }
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

        private void SetupCustomerUpdate()
        {
            dataCustomer.customerUpdate.validateButton.Content = "Valider";
            dataCustomer.customerUpdate.validateButton.Action = new RelayCommand(CustomerUpdateCommand); //Press on validateButton to activate the "SellerUpdateCommand" function
            dataCustomer.customerUpdate.cancelButton.Content = "Cancel";
            dataCustomer.customerUpdate.cancelButton.Action = new RelayCommand(CustomerEditCancel); //Press on cancelButton to activate the function "SellerEditCancel" to cancel the operation
            dataCustomer.customerUpdate.customer = new Customer();

            dataCustomer.customerUpdate.listView.SelectedItem = new Customer();
            dataCustomer.customerUpdate.listView.SellerSelected = new RelayCommand(CustomerListSelectionChanged);//The function "SellerListSellerSelected" must be activated each time a Seller item is selected
        }

        private void SetupCustomerList()
        {
            dataCustomer.customerList.customers = new ObservableCollection<Customer>();

            dataCustomer.customerList.deleteButton.Content = "Delete";
            dataCustomer.customerList.deleteButton.Action = new RelayCommand(CustomerRemoveCommand);

            dataCustomer.customerList.updateButton.Content = "Update";
            dataCustomer.customerList.updateButton.Action = new RelayCommand(CustomerUpdateCommand);

            foreach (var item in databaseService.Customers)
            {
                dataCustomer.customerList.customers.Add(item);
                dataCustomer.customerUpdate.customers.Add(item);
            }
            dataCustomer.customerList.listView.SelectedItem = new Customer();
            dataCustomer.customerList.listView.SelectionChanged = new RelayCommand(CustomerListSelectionChanged);
        }

        private void CustomerUpdateCommand()
        {
            //Updating the seller infos

            Customer customer = new Customer();
            customer.CopyFrom(dataCustomer.customerUpdate.listView.SelectedItem);//Catching the selected seller's infos
            customer.FirstName = dataCustomer.customerUpdate.customer.FirstName;//Updating his/her informations
            customer.Address = dataCustomer.customerUpdate.customer.Address;
            customer.Email = dataCustomer.customerUpdate.customer.Email;
            customer.Gender = dataCustomer.customerUpdate.customer.Gender;
            customer.Phone = dataCustomer.customerUpdate.customer.Phone;
            
            Debug.WriteLine("{0}", customer.Id);
            Debug.WriteLine("{0}", customer.FirstName);
            Debug.WriteLine("{0}", customer.Address);
            Debug.WriteLine("{0}", customer.Email);
            Debug.WriteLine("{0}", customer.Gender);
            Debug.WriteLine("{0}", customer.Phone);
            try
            {
                foreach (var item in databaseService.Customers)
                {
                    //looking for seller's id

                    Debug.WriteLine("{0}", item.Id);
                    if (customer.Id == item.Id)
                    {
                        Debug.WriteLine("{0}", item.Id);
                        databaseService.SqliteConnection.Update(customer);//Updating seller info in the database
                        CustomerUpdateList();//Updating the lists

                    }
                    //DataSeller.sellerList.sellers.Add(item);
                }


                //DataSeller.sellerList.sellers.(seller);
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

        private void CustomerUpdateList()
        {
            //Refresh the lists

            dataCustomer.customerList.customers.Clear();
            dataCustomer.customerUpdate.customers.Clear();

            foreach (var item in databaseService.Customers)
            {
                dataCustomer.customerList.customers.Add(item);
                dataCustomer.customerUpdate.customers.Add(item);
            }
        }

        private void CustomerRemoveCommand()
        {
            Customer customer = new Customer();

            customer.CopyFrom(dataCustomer.customerList.listView.SelectedItem);//Catch the information about the seller we want to delete in the database and the list

            System.Diagnostics.Debug.WriteLine("{0}", customer.Id);

            try
            {
                databaseService.SqliteConnection.Delete(customer);
                dataCustomer.customerList.customers.Remove(customer);
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

        private void CustomerListSelectionChanged()
        {
            dataCustomer.customerList.customer = new Customer();

            dataCustomer.customerList.customer.CopyFrom(dataCustomer.customerList.listView.SelectedItem);
            
            if (dataCustomer.customerList.customer != null)
            {
                dataCustomer.customerList.customer.CopyFrom(dataCustomer.customerList.customer);
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
