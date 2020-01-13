using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP_FilRouge.Views.ViewModelLight
{
    public class UserViewModel 
    {
        public UserViewModel()
        {
            
            //this.TextFirstName = "";
            //this.TextLastName = "";

            
        }

        

        


        



        public void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("Enter Creation Page");
        }
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("Leave Creation Page");
        }
    }
}
