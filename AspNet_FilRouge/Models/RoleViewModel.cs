namespace AspNet_FilRouge.Models
{
    public class RoleViewModel
    {
        public RoleViewModel() { }

        public RoleViewModel(ApplicationRole role)
        {
            Id = role.Id;
            Name = role.Name;
        }

        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
