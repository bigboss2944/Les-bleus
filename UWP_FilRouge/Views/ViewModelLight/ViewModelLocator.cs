using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_FilRouge.Views.ViewModelLight
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
                navigationService.Configure("ElementOrder", typeof(ElementOrder));
                navigationService.Configure("OrderView", typeof(OrderView));
                return navigationService;
            });
            SimpleIoc.Default.Register<BlankPageViewModel>();
            SimpleIoc.Default.Register<OrderViewModel>();
        }

        public BlankPageViewModel BlankPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<BlankPageViewModel>(); }
        }

        public OrderViewModel OtherPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<OrderViewModel>(); }
        }
    }
}
