using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UWP_FilRouge.Views.ViewModelLight.Views.ViewModel.UcAccessors.Commons
{
    public class ButtonAccessor
    {
        public String Content { get; set; }
        public ICommand Action { get; set; }
    }
}
