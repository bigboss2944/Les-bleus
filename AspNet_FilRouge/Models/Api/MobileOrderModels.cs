namespace AspNet_FilRouge.Models.Api
{
    /// <summary>Résumé d'un vélo, pour l'affichage catalogue et le détail de commande côté mobile.</summary>
    public class MobileBicycleSummary
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
    }

    /// <summary>Résumé d'un vendeur.</summary>
    public class MobileSellerSummary
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    /// <summary>Résumé d'un client.</summary>
    public class MobileCustomerSummary
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }

    /// <summary>Ligne de la liste des commandes (toutes vendeurs confondus), avec pagination.</summary>
    public class MobileOrderSummary
    {
        public long IdOrder { get; set; }
        public DateTime Date { get; set; }
        public bool IsValidated { get; set; }
        public float Discount { get; set; }
        public float Total { get; set; }
        public MobileSellerSummary? Seller { get; set; }
        public MobileCustomerSummary? Customer { get; set; }
        public int BicycleCount { get; set; }
    }

    /// <summary>Page de résultats génériques pour les listes mobiles.</summary>
    public class MobilePagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
    }

    /// <summary>Détail complet d'une commande, y compris ses vélos, pour l'écran de composition.</summary>
    public class MobileOrderDetail
    {
        public long IdOrder { get; set; }
        public DateTime Date { get; set; }
        public bool IsValidated { get; set; }
        public float Discount { get; set; }
        public float Tax { get; set; }
        public float ShippingCost { get; set; }
        public float Total { get; set; }
        public MobileSellerSummary? Seller { get; set; }
        public MobileCustomerSummary? Customer { get; set; }
        public List<MobileBicycleSummary> Bicycles { get; set; } = new();
    }

    /// <summary>Corps de la requête de création de commande depuis l'application MAUI.</summary>
    public class MobileCreateOrderRequest
    {
        /// <summary>Vendeur créateur de la commande. Si non renseigné, le vendeur connecté (issu du JWT) est utilisé.</summary>
        public string? SellerId { get; set; }

        public string? CustomerId { get; set; }

        /// <summary>Remise commerciale en pourcentage (0-100).</summary>
        public float Discount { get; set; }

        public string? PayMode { get; set; }
    }
}
