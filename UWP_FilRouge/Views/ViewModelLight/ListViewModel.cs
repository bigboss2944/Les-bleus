using Entities;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class ListViewModel : INavigationEvent
    {

        private INavigationService navigationService;

        public User User { get; set; }

        public void OnNavigatedTo(NavigationEventArgs e) 
        { 
            Debug.WriteLine("Enter OtherPage"); 
        }

        public void OnNavigatedFrom(NavigationEventArgs e) 
        { 
            Debug.WriteLine("Leave OtherPage"); 
        }

        public ListViewModel()
        {
            this.navigationService = navigationService;
            this.User = new User();
        }

    }

    public interface INavigationEvent
    {
        void OnNavigatedTo(NavigationEventArgs e); 
        void OnNavigatedFrom(NavigationEventArgs e);
    }
}
