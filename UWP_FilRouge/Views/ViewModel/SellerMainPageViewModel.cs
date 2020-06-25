using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            //MoveToRegisterPage = new RelayCommand(ToRegisterPage);
            MoveToLoginPage = new RelayCommand(ToLoginPage);
            MoveToOrderPage = new RelayCommand(ToOrderPage);
            //MoveToSellerPage = new RelayCommand(ToSellerPage);
            MoveToCustomerPage = new RelayCommand(ToCustomerPage);
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
            DataSeller.sellerEdit.validateButton.Action = new RelayCommand(SellerEditCommand); //Press on validateButton to activate the SellerEdit function
            DataSeller.sellerEdit.cancelButton.Content = "Cancel";
            DataSeller.sellerEdit.cancelButton.Action = new RelayCommand(SellerEditCancel); //Press on cancelButton to activate the SellerEdit function
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
            bool insert = true;

            try
            {
                foreach (var item in databaseService.Sellers)
                {
                    Debug.WriteLine("{0}", item.Id);
                    if (seller.Id == item.Id) //check if the id is not already present in the Seller table
                    {
                        Debug.WriteLine("{0}", item.Id);
                        databaseService.SqliteConnection.Update(seller);
                        DataSeller.sellerList.sellers.IndexOf(seller);
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
                    databaseService.SqliteConnection.Insert(seller);// Insert Seller in the database
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

        private void SellerRemoveCommand()
        {
            Seller seller = new Seller();
            
            seller.CopyFrom(DataSeller.sellerList.listView.SelectedItem);//Catch the information about the seller we want to delete in the database and the list

            System.Diagnostics.Debug.WriteLine("{0}", seller.Id);

            try
            {
                databaseService.SqliteConnection.Delete(seller);
                DataSeller.sellerList.sellers.Remove(seller);
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
            DataSeller.sellerUpdate.validateButton.Action = new RelayCommand(SellerUpdateCommand); //Press on validateButton to activate the "SellerUpdateCommand" function
            DataSeller.sellerUpdate.cancelButton.Content = "Cancel";
            DataSeller.sellerUpdate.cancelButton.Action = new RelayCommand(SellerEditCancel); //Press on cancelButton to activate the function "SellerEditCancel" to cancel the operation
            DataSeller.sellerUpdate.seller = new Seller();

            DataSeller.sellerUpdate.listView.SelectedItem = new Seller();
            DataSeller.sellerUpdate.listView.SellerSelected = new RelayCommand(SellerListSellerSelected);//The function "SellerListSellerSelected" must be activated each time a Seller item is selected
        }

        private void SetupSellerList()
        {
            /*
             * The role of this function is to show the seller list to the user,to allow him to refresh the list showed
             * and to delete a seller
             * 
             */

            DataSeller.sellerList.sellers = new ObservableCollection<Seller>();

            DataSeller.sellerList.deleteButton.Content = "Delete";
            DataSeller.sellerList.deleteButton.Action = new RelayCommand(SellerRemoveCommand);

            DataSeller.sellerList.updateButton.Content = "Update";
            DataSeller.sellerList.updateButton.Action = new RelayCommand(SellerUpdateList);

            //DataSeller.sellerList.updateButton.Content = "Nimp";
            //DataSeller.sellerList.updateButton.Action = new RelayCommand(SellerUpdateCommand);
            DataSeller.sellerList.cancelButton.Content = "portequoi";
            DataSeller.sellerList.cancelButton.Action = new RelayCommand(SellerEditCancel);//back

            foreach (var item in databaseService.Sellers)
            {
                //Updating the lists in sellerList and sellerUpdate

                DataSeller.sellerList.sellers.Add(item);
                DataSeller.sellerUpdate.sellers.Add(item);
            }
            DataSeller.sellerList.listView.SelectedItem = new Seller();
            DataSeller.sellerList.listView.SelectionChanged = new RelayCommand(SellerListSelectionChanged);

        }

        private void SellerUpdateList()
        {
            //Refresh the lists

            DataSeller.sellerList.sellers.Clear();
            DataSeller.sellerUpdate.sellers.Clear();

            foreach (var item in databaseService.Sellers)
            {
                DataSeller.sellerList.sellers.Add(item);
                DataSeller.sellerUpdate.sellers.Add(item);
            }
        }

        private void SellerUpdateCommand()
        {
            //Updating the seller infos

            Seller seller = new Seller();
            seller.CopyFrom(DataSeller.sellerUpdate.listView.SelectedItem);//Catching the selected seller's infos
            seller.FirstName = DataSeller.sellerUpdate.seller.FirstName;//Updating his/her informations
            seller.Password = DataSeller.sellerUpdate.seller.Password;
            Debug.WriteLine("{0}", seller.Id);
            Debug.WriteLine("{0}", seller.FirstName);
            Debug.WriteLine("{0}", seller.Password);
            try
            {
                foreach (var item in databaseService.Sellers)
                {
                    //looking for seller's id

                    Debug.WriteLine("{0}", item.Id);
                    if (seller.Id == item.Id){
                        Debug.WriteLine("{0}", item.Id);
                        databaseService.SqliteConnection.Update(seller);//Updating seller info in the database
                        SellerUpdateList();//Updating the lists
                        
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

        private void SellerListSelectionChanged()
        {
            //Catching the seller selected

            DataSeller.sellerList.seller = new Seller();

            DataSeller.sellerList.seller.CopyFrom(DataSeller.sellerList.listView.SelectedItem);
            //DataSeller.sellerList.sellers.Add(seller);
            if (DataSeller.sellerList.seller != null)
            {
                //DataSeller.sellerList.seller = new Seller();
                DataSeller.sellerList.seller.CopyFrom(DataSeller.sellerList.seller);
            }

            
        }

        private void SellerListSellerSelected()
        {

            //Catching the seller selected

            DataSeller.sellerList.seller = new Seller();

            DataSeller.sellerList.seller.CopyFrom(DataSeller.sellerList.listView.SelectedItem);
            //DataSeller.sellerList.sellers.Add(seller);
            if (DataSeller.sellerList.seller != null)
            {
                //DataSeller.sellerList.seller = new Seller();
                DataSeller.sellerList.seller.CopyFrom(DataSeller.sellerList.seller);
            }


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
            navigationService.NavigateTo("Order Main Page");
        }
    }
}
