using System;
using System.Windows.Input;

namespace UWP_FilRouge.Views.ViewModel
{
    public class ButtonAccessor
    {
        public String Content { get; set; }
        public ICommand Action { get; set; }
    }
}