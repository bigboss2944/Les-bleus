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
        /// 
        public enum Pages
        {
            CreateUserView,
            ListUserView,
            
            MainPage
        }

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
            SimpleIoc.Default.Register<ListUserView>();

        }                

        public UserViewModel UserViewInstance        
        {            
            get { return ServiceLocator.Current.GetInstance<UserViewModel>(); }        
        }

        public ListUserView ListUserViewInstance
        {
            
            get {

                try //Gérer exception
                {
                    return ServiceLocator.Current.GetInstance<ListUserView>();

                }
                catch(StackOverflowException e)
                {
                    Console.WriteLine(e);
                    return ServiceLocator.Current.GetInstance<ListUserView>();
                }
                
            }
        }

        public CreateUserViewModel CreateUserViewInstance
        {
            get { return ServiceLocator.Current.GetInstance<CreateUserViewModel>(); }
        }
    }

}
