using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using UWP_FilRouge.Views.ViewModelLight.Entities;
using Order = UWP_FilRouge.Views.ViewModelLight.Entities.Order;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class OrderViewModel: ViewModelBase
    {
        private INavigationService navigationService;

        public Order Order { get; set; }
        public string ButtonContent { get; set; }
        public ICommand ButtonClick => new RelayCommand(() =>
        {
            this.navigationService.NavigateTo("ElementOrder");
            //this.navigationService.GoBack();
            //this.User.Firstname = "test";
            //this.User.Lastname = "1";
        });

        public OrderViewModel (INavigationService navigationService)
        {
            this.navigationService = navigationService;
            //this.User = new User() { Quantity = "50", Seller = "Seller" };
            this.ButtonContent = "change";
            MessengerInstance.Register<NotificationMessage<User>>(this, "BlankPageUserSender", BlankPageUserRetriever);
        }

        private void BlankPageUserRetriever(NotificationMessage<Order> obj)
        {
            this.Order.CopyFrom(obj.Content as Order);
        }
    }
}
