namespace UWP_FilRouge.Views.ViewModels
{
    public class UserPageViewModel
    {
        private string firstname;
        private string lastname;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserPageViewModel(string firstname, string lastname)
        {
            this.firstname = firstname;
            this.lastname = lastname;
        }
    }

    
}