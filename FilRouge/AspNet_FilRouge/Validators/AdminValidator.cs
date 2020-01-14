using AspNet_FilRouge.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNet_FilRouge.Validators
{
    

    public class AdminValidator : ValidationAttribute
    
    {
      
        public AdminValidator() : base("Only admin autorized")
        {
           
          
        }
      

        //public override bool IsValid(object value)
        //{
        //    bool result = false;
        //    Role role;
        //    if (Role.TryParse(value.ToString(), out data))
        //    {
        //        if (data >= 0 && data <= 100000)
        //        {
        //            result = true;
        //        }
        //    }
        //    return result;
        //}
    }
}