
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP_FilRouge.Entities;
using Windows.UI.Xaml.Controls;

namespace UWP_FilRouge.Views.ViewModels
{
    public class OrderCheckPageViewModel
    {
        private INavigationService navigationService;

        public Order Order { get; set; }
        public string ButtonContent { get; set; }
        public ICommand ButtonClick => new RelayCommand(() =>
        {
            this.navigationService.NavigateTo(Views.ListOrderPage.ToString, Order);
            Debug.WriteLine(Order.ToString());
            
        });

        




    }
}
