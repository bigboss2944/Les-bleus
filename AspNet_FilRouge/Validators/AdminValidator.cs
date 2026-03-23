using System.ComponentModel.DataAnnotations;

namespace AspNet_FilRouge.Validators
{
    public class AdminValidator : ValidationAttribute
    {
        public AdminValidator() : base("Only admin authorized")
        {
        }
    }
}
