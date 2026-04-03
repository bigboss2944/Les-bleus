using System.ComponentModel.DataAnnotations;
using AspNet_FilRouge.Validators;

namespace LesBleus.Tests.Unit.Validators;

public class AdminValidatorTests
{
    [Fact]
    public void AdminValidator_IsValidationAttribute()
    {
        var validator = new AdminValidator();

        Assert.IsAssignableFrom<ValidationAttribute>(validator);
    }

    [Fact]
    public void AdminValidator_ErrorMessage_IsOnlyAdminAuthorized()
    {
        var validator = new AdminValidator();

        // The message is passed via base ctor and accessible through FormatErrorMessage
        var message = validator.FormatErrorMessage(string.Empty);
        Assert.Equal("Only admin authorized", message);
    }
}
