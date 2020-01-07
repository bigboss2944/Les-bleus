using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_FilRouge
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //this.Loaded += MainPage_Loaded();
        }

        private void MainPage_Loaded()
        {
            Button btn = new Button();            //UIElement            
            btn.Visibility = Visibility.Visible;            
            btn.Opacity = 0.3;           
            btn.Rotation = 10;            
            btn.CanDrag = false;            //FrameworkElement            
            btn.Width = 100; btn.Height = 100;            
            btn.MaxWidth = 200; btn.MaxHeight = 200;            
            btn.MinWidth = 5; btn.MinHeight = 5;            
            btn.Name = "myButton";            
            btn.VerticalAlignment = VerticalAlignment.Center;            
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;            
            btn.Margin = new Thickness(5, 10, 15, 20);            // Button            
            btn.Content = "click me";            
            stack.Children.Add(btn) ;
        }
    }
}
