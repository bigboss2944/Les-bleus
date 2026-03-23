using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AspNet_FilRouge.Models
{
    public class BicycleOrdersViewModel
    {
        public List<Bicycle> Bicycles { get; set; } = new List<Bicycle>();
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

    public class CreateSellerViewModel
    {
        [Required]
        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Adresse")]
        public string? Address { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Téléphone")]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Le {0} doit comporter au moins {2} caractères.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string? Password { get; set; }
    }
}
