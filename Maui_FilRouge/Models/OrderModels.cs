namespace Maui_FilRouge.Models
{
    /// <summary>Miroir de MobileBicycleSummary côté API.</summary>
    public class BicycleSummary
    {
        public long Id { get; set; }
        public string? TypeOfBike { get; set; }
        public string? Category { get; set; }
        public string? Reference { get; set; }
        public float FreeTaxPrice { get; set; }
        public float Tax { get; set; }
        public int Quantity { get; set; }
        public bool Exchangeable { get; set; }
        public bool Insurance { get; set; }
        public bool Deliverable { get; set; }

        public string DisplayName => $"{TypeOfBike} {Category} — {Reference}".Trim();
    }

    /// <summary>Miroir de MobileSellerSummary côté API.</summary>
    public class SellerSummary
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string DisplayName => $"{FirstName} {LastName}".Trim();
    }

    /// <summary>Miroir de MobileCustomerSummary côté API.</summary>
    public class CustomerSummary
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public string DisplayName => $"{FirstName} {LastName}".Trim();
    }

    /// <summary>Miroir de MobileOrderSummary côté API — ligne de la liste des commandes (tous vendeurs).</summary>
    public class OrderSummary
    {
        public long IdOrder { get; set; }
        public DateTime Date { get; set; }
        public bool IsValidated { get; set; }
        public float Discount { get; set; }
        public float Total { get; set; }
        public SellerSummary? Seller { get; set; }
        public CustomerSummary? Customer { get; set; }
        public int BicycleCount { get; set; }

        public string StatusLabel => IsValidated ? "Validée" : "En cours";
    }

    /// <summary>Page de résultats renvoyée par l'API mobile.</summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
    }

    /// <summary>Miroir de MobileOrderDetail côté API — commande en cours de composition.</summary>
    public class OrderDetail
    {
        public long IdOrder { get; set; }
        public DateTime Date { get; set; }
        public bool IsValidated { get; set; }
        public float Discount { get; set; }
        public float Tax { get; set; }
        public float ShippingCost { get; set; }
        public float Total { get; set; }
        public SellerSummary? Seller { get; set; }
        public CustomerSummary? Customer { get; set; }
        public List<BicycleSummary> Bicycles { get; set; } = new();
    }

    /// <summary>Corps de la requête de création de commande envoyée à POST api/mobile/orders.</summary>
    public class CreateOrderRequest
    {
        public string? SellerId { get; set; }
        public string? CustomerId { get; set; }
        public float Discount { get; set; }
        public string? PayMode { get; set; }
    }
}
