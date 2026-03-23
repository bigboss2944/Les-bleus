namespace AspNet_FilRouge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bicycles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TypeOfBike = c.String(),
                        Category = c.String(),
                        Reference = c.String(),
                        FreeTaxPrice = c.Single(nullable: false),
                        Exchangeable = c.Boolean(nullable: false),
                        Insurance = c.Boolean(nullable: false),
                        Deliverable = c.Boolean(nullable: false),
                        Size = c.Single(nullable: false),
                        Weight = c.Single(nullable: false),
                        Color = c.String(),
                        WheelSize = c.Single(nullable: false),
                        Electric = c.Boolean(nullable: false),
                        State = c.String(),
                        Brand = c.String(),
                        Confort = c.String(),
                        Order_IdOrder = c.Long(),
                        Shop_ShopId = c.Long(),
                        Customer_IdCustomer = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_IdOrder)
                .ForeignKey("dbo.Shops", t => t.Shop_ShopId)
                .ForeignKey("dbo.Customers", t => t.Customer_IdCustomer)
                .Index(t => t.Order_IdOrder)
                .Index(t => t.Shop_ShopId)
                .Index(t => t.Customer_IdCustomer);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        IdCustomer = c.Long(nullable: false, identity: true),
                        Town = c.String(),
                        PostalCode = c.Int(nullable: false),
                        Address = c.String(),
                        LoyaltyPoints = c.Int(nullable: false),
                        Phone = c.String(),
                        Email = c.String(),
                        Gender = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        Shop_ShopId = c.Long(),
                    })
                .PrimaryKey(t => t.IdCustomer)
                .ForeignKey("dbo.Shops", t => t.Shop_ShopId)
                .Index(t => t.Shop_ShopId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        IdOrder = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        PayMode = c.String(),
                        Discount = c.Single(nullable: false),
                        UseLoyaltyPoint = c.Boolean(nullable: false),
                        Tax = c.Single(nullable: false),
                        ShippingCost = c.Single(nullable: false),
                        Customer_IdCustomer = c.Long(),
                        Shop_ShopId = c.Long(),
                        Seller_IdSeller = c.Long(),
                    })
                .PrimaryKey(t => t.IdOrder)
                .ForeignKey("dbo.Customers", t => t.Customer_IdCustomer)
                .ForeignKey("dbo.Shops", t => t.Shop_ShopId)
                .ForeignKey("dbo.Sellers", t => t.Seller_IdSeller)
                .Index(t => t.Customer_IdCustomer)
                .Index(t => t.Shop_ShopId)
                .Index(t => t.Seller_IdSeller);
            
            CreateTable(
                "dbo.Sellers",
                c => new
                    {
                        IdSeller = c.Long(nullable: false, identity: true),
                        Password = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        Role_Id = c.Long(),
                        Shop_ShopId = c.Long(),
                    })
                .PrimaryKey(t => t.IdSeller)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .ForeignKey("dbo.Shops", t => t.Shop_ShopId)
                .Index(t => t.Role_Id)
                .Index(t => t.Shop_ShopId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shops",
                c => new
                    {
                        ShopId = c.Long(nullable: false, identity: true),
                        Town = c.String(),
                        Postalcode = c.Int(nullable: false),
                        Adress = c.String(),
                        Nameshop = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Website = c.String(),
                    })
                .PrimaryKey(t => t.ShopId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoleViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Bicycles", "Customer_IdCustomer", "dbo.Customers");
            DropForeignKey("dbo.Orders", "Seller_IdSeller", "dbo.Sellers");
            DropForeignKey("dbo.Sellers", "Shop_ShopId", "dbo.Shops");
            DropForeignKey("dbo.Orders", "Shop_ShopId", "dbo.Shops");
            DropForeignKey("dbo.Customers", "Shop_ShopId", "dbo.Shops");
            DropForeignKey("dbo.Bicycles", "Shop_ShopId", "dbo.Shops");
            DropForeignKey("dbo.Sellers", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Orders", "Customer_IdCustomer", "dbo.Customers");
            DropForeignKey("dbo.Bicycles", "Order_IdOrder", "dbo.Orders");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Sellers", new[] { "Shop_ShopId" });
            DropIndex("dbo.Sellers", new[] { "Role_Id" });
            DropIndex("dbo.Orders", new[] { "Seller_IdSeller" });
            DropIndex("dbo.Orders", new[] { "Shop_ShopId" });
            DropIndex("dbo.Orders", new[] { "Customer_IdCustomer" });
            DropIndex("dbo.Customers", new[] { "Shop_ShopId" });
            DropIndex("dbo.Bicycles", new[] { "Customer_IdCustomer" });
            DropIndex("dbo.Bicycles", new[] { "Shop_ShopId" });
            DropIndex("dbo.Bicycles", new[] { "Order_IdOrder" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.RoleViewModels");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Shops");
            DropTable("dbo.Roles");
            DropTable("dbo.Sellers");
            DropTable("dbo.Orders");
            DropTable("dbo.Customers");
            DropTable("dbo.Bicycles");
        }
    }
}
