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
    class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //Register your services used here
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<ShopPageViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
        }

        public ShopPageViewModel ShopPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<ShopPageViewModel>(); }
        }

        public MainPageViewModel MyProperty
        {
            get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        }

    }
}
