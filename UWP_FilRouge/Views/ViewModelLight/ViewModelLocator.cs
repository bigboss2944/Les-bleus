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
    {   /// <summary>        
        /// Initializes a new instance of the ViewModelLocator class.        
        /// /// </summary>        
        public ViewModelLocator()        
        {            
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);            //Register your services used here            
            SimpleIoc.Default.Register<INavigationService>(() =>
            {
                var navigationService = new NavigationService();
                navigationService.Configure("CreateUserView", typeof(CreateUserView));
                navigationService.Configure("ListUserView", typeof(ListUserView));
                return navigationService;
            });
            SimpleIoc.Default.Register<UserViewModel>();
            SimpleIoc.Default.Register<CreateUserViewModel>();

        }                
        public UserViewModel UserViewInstance        
        {            
            get { return ServiceLocator.Current.GetInstance<UserViewModel>(); }        
        }

        public ListViewModel ListViewInstance
        {
            get { return ServiceLocator.Current.GetInstance<ListViewModel>(); }
        }


    }

}
