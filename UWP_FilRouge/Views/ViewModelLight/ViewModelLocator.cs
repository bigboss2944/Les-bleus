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
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<CreateBicycleViewModel>();
            SimpleIoc.Default.Register<DetailsBicycleViewModel>();
            SimpleIoc.Default.Register<UpdateBicycleViewModel>();
            SimpleIoc.Default.Register<DeleteBicycleViewModel>();
        }

        public CreateBicycleViewModel CreateBicycleInstance
        {
            get { return ServiceLocator.Current.GetInstance<CreateBicycleViewModel>(); }
        }

        public DetailsBicycleViewModel DetailsBicycleInstance
        {
            get { return ServiceLocator.Current.GetInstance<DetailsBicycleViewModel>(); }
        }

        public UpdateBicycleViewModel UpdateBicycleInstance
        {
            get { return ServiceLocator.Current.GetInstance<UpdateBicycleViewModel>(); }
        }

        public DeleteBicycleViewModel DeleteBicycleInstance
        {
            get { return ServiceLocator.Current.GetInstance<DeleteBicycleViewModel>(); }
        }
    }
}
