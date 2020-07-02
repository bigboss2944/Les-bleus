using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using UWP_FilRouge.Database;
using UWP_FilRouge.Views.ViewModel;

namespace UWP_FilRouge.Views.MVVMLight
{
    public class ViewModelLocator
    {   /// <summary>        
        /// Initializes a new instance of the ViewModelLocator class.        
        /// /// </summary>
        /// 
        

        public ViewModelLocator()        
        {            
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);            //Register your services used here            
            var navigationService = new NavigationService();
            navigationService.Configure("Login Page", typeof(LoginPage));
            navigationService.Configure("Admin Page", typeof(AdminPage));
            navigationService.Configure("Register Page", typeof(RegisterPage));
            navigationService.Configure("Seller Main Page", typeof(SellerMainPage));
            navigationService.Configure("Customer Main Page", typeof(CustomerMainPage));
            navigationService.Configure("Order Main Page", typeof(OrderMainPage));
            //navigationService.Configure("Product Main Page", typeof(OrderMainPage));
            navigationService.Configure("Main Menu Page", typeof(MainMenu));
            navigationService.Configure("Home Page", typeof(HomePage));
            navigationService.Configure("Contact Page", typeof(ContactPage));
            navigationService.Configure("About Page", typeof(AboutPage));
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<LoginPageViewModel>();

            SimpleIoc.Default.Register<AdminPageViewModel>();

            SimpleIoc.Default.Register<CustomerPageViewModel>();

            SimpleIoc.Default.Register<SellerMainPageViewModel>();

            SimpleIoc.Default.Register<CustomerPageViewModel>();

            SimpleIoc.Default.Register<OrderMainPageViewModel>();

            SimpleIoc.Default.Register<HomePageViewModel>();

            SimpleIoc.Default.Register<ContactPageViewModel>();

            SimpleIoc.Default.Register<AboutPageViewModel>();

            SimpleIoc.Default.Register<MainMenuPageViewModel>();

            SimpleIoc.Default.Register<DatabaseService>(() =>
            {
                return new DatabaseService();
            }, true);

        }

        public MainMenuPageViewModel MainMenuPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<MainMenuPageViewModel>(); }
        }

        public AdminPageViewModel AdminPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<AdminPageViewModel>(); }
        }

        public HomePageViewModel HomePageInstance
        {
            get { return ServiceLocator.Current.GetInstance<HomePageViewModel>(); }
        }

        public ContactPageViewModel ContactPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<ContactPageViewModel>(); }
        }

        public AboutPageViewModel AboutPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<AboutPageViewModel>(); }
        }

        public LoginPageViewModel LoginPageInstance        
        {            
            get { return ServiceLocator.Current.GetInstance<LoginPageViewModel>(); }        
        }

        public CustomerPageViewModel CustomerMainPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<CustomerPageViewModel>(); }
        }

        public SellerMainPageViewModel SellerMainPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<SellerMainPageViewModel>(); }
        }

        public OrderMainPageViewModel OrderMainPageInstance
        {
            get { return ServiceLocator.Current.GetInstance<OrderMainPageViewModel>(); }
        }


    }

}
