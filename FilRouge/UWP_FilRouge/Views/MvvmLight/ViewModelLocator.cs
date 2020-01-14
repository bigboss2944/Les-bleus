using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_FilRouge.Views.ViewModels;

namespace UWP_FilRouge.Views.MvvmLight
{
    public class ViewModelLocator
    {
        
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => 
            { 
            var navigationService = new NavigationService();
            navigationService.Configure("OrderCheckPage", typeof(OrderCheckPage)); 
            //navigationService.Configure("ListUserView", typeof(ListUserView));
            return navigationService; }); 
            SimpleIoc.Default.Register<OrderCheckPageViewModel>(); 
            //SimpleIoc.Default.Register<CreateUserViewModel>(); 
            //SimpleIoc.Default.Register<ListUserView>();
        }



        public OrderCheckPageViewModel OrderCheckPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<OrderCheckPageViewModel>(); }
        }

        //public MainPageViewModel MyProperty
        //{
        //    get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        //}

    }
}
