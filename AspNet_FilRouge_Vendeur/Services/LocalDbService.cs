using Microsoft.Data.Sqlite;

namespace AspNet_FilRouge_Vendeur.Services
{
    /// <summary>
    /// Service de base de données locale SQLite (sans Entity Framework).
    /// Utilisé pour la synchronisation et le mode hors-ligne.
    /// </summary>
    public class LocalDbService
    {
        private readonly string _dbPath;

        public LocalDbService(IWebHostEnvironment env)
        {
            _dbPath = Path.Combine(env.ContentRootPath, "local_cache.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var db = OpenConnection();

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS Orders (
                    IdOrder INTEGER PRIMARY KEY,
                    Date TEXT NULL,
                    PayMode TEXT NULL,
                    Discount REAL NOT NULL DEFAULT 0,
                    UseLoyaltyPoint INTEGER NOT NULL DEFAULT 0,
                    Tax REAL NOT NULL DEFAULT 0,
                    ShippingCost REAL NOT NULL DEFAULT 0,
                    IsValidated INTEGER NOT NULL DEFAULT 0,
                    SellerId TEXT NULL,
                    CustomerId TEXT NULL,
                    ShopId INTEGER NULL,
                    SyncedAt TEXT NULL
                )");

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS Bicycles (
                    Id INTEGER PRIMARY KEY,
                    TypeOfBike TEXT NULL,
                    Category TEXT NULL,
                    Reference TEXT NULL,
                    FreeTaxPrice REAL NOT NULL DEFAULT 0,
                    Exchangeable INTEGER NOT NULL DEFAULT 0,
                    Insurance INTEGER NOT NULL DEFAULT 0,
                    Deliverable INTEGER NOT NULL DEFAULT 0,
                    Size REAL NOT NULL DEFAULT 0,
                    Weight REAL NOT NULL DEFAULT 0,
                    Color TEXT NULL,
                    WheelSize REAL NOT NULL DEFAULT 0,
                    Electric INTEGER NOT NULL DEFAULT 0,
                    State TEXT NULL,
                    Brand TEXT NULL,
                    Confort TEXT NULL,
                    Order_IdOrder INTEGER NULL,
                    Shop_ShopId INTEGER NULL,
                    SyncedAt TEXT NULL
                )");

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS Sellers (
                    Id TEXT PRIMARY KEY,
                    LastName TEXT NULL,
                    FirstName TEXT NULL,
                    Email TEXT NULL,
                    SyncedAt TEXT NULL
                )");

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id TEXT PRIMARY KEY,
                    Town TEXT NULL,
                    PostalCode INTEGER NOT NULL DEFAULT 0,
                    Address TEXT NULL,
                    LoyaltyPoints INTEGER NOT NULL DEFAULT 0,
                    Phone TEXT NULL,
                    Email TEXT NULL,
                    Gender TEXT NULL,
                    LastName TEXT NULL,
                    FirstName TEXT NULL,
                    SyncedAt TEXT NULL
                )");
        }

        private SqliteConnection OpenConnection()
        {
            var db = new SqliteConnection($"Filename={_dbPath}");
            db.Open();
            return db;
        }

        private static void ExecuteNonQuery(SqliteConnection db, string sql)
        {
            using var cmd = new SqliteCommand(sql, db);
            cmd.ExecuteNonQuery();
        }

        // ── Orders ────────────────────────────────────────────────────────────

        public void UpsertOrder(Models.Order order)
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(@"
                INSERT INTO Orders (IdOrder, Date, PayMode, Discount, UseLoyaltyPoint, Tax, ShippingCost, IsValidated,
                                    SellerId, CustomerId, ShopId, SyncedAt)
                VALUES (@IdOrder, @Date, @PayMode, @Discount, @UseLoyaltyPoint, @Tax, @ShippingCost, @IsValidated,
                        @SellerId, @CustomerId, @ShopId, @SyncedAt)
                ON CONFLICT(IdOrder) DO UPDATE SET
                    Date=excluded.Date, PayMode=excluded.PayMode, Discount=excluded.Discount,
                    UseLoyaltyPoint=excluded.UseLoyaltyPoint, Tax=excluded.Tax, ShippingCost=excluded.ShippingCost,
                    IsValidated=excluded.IsValidated, SellerId=excluded.SellerId, CustomerId=excluded.CustomerId,
                    ShopId=excluded.ShopId, SyncedAt=excluded.SyncedAt", db);

            cmd.Parameters.AddWithValue("@IdOrder", order.IdOrder);
            cmd.Parameters.AddWithValue("@Date", order.Date.ToString("o"));
            cmd.Parameters.AddWithValue("@PayMode", (object?)order.PayMode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Discount", order.Discount);
            cmd.Parameters.AddWithValue("@UseLoyaltyPoint", order.UseLoyaltyPoint ? 1 : 0);
            cmd.Parameters.AddWithValue("@Tax", order.Tax);
            cmd.Parameters.AddWithValue("@ShippingCost", order.ShippingCost);
            cmd.Parameters.AddWithValue("@IsValidated", order.IsValidated ? 1 : 0);
            cmd.Parameters.AddWithValue("@SellerId", (object?)order.Seller?.Id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CustomerId", (object?)order.Customer?.Id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ShopId", (object?)order.Shop?.ShopId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<(long IdOrder, DateTime Date, string? PayMode, float Discount, bool IsValidated)> GetOrders()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand("SELECT IdOrder, Date, PayMode, Discount, IsValidated FROM Orders ORDER BY Date DESC", db);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return (
                    reader.GetInt64(0),
                    DateTime.Parse(reader.GetString(1)),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    (float)reader.GetDouble(3),
                    reader.GetInt32(4) != 0
                );
            }
        }

        // ── Bicycles ──────────────────────────────────────────────────────────

        public void UpsertBicycle(Models.Bicycle bicycle)
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(@"
                INSERT INTO Bicycles (Id, TypeOfBike, Category, Reference, FreeTaxPrice, Exchangeable, Insurance,
                                      Deliverable, Size, Weight, Color, WheelSize, Electric, State, Brand, Confort,
                                      Order_IdOrder, Shop_ShopId, SyncedAt)
                VALUES (@Id, @TypeOfBike, @Category, @Reference, @FreeTaxPrice, @Exchangeable, @Insurance,
                        @Deliverable, @Size, @Weight, @Color, @WheelSize, @Electric, @State, @Brand, @Confort,
                        @Order_IdOrder, @Shop_ShopId, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    TypeOfBike=excluded.TypeOfBike, Category=excluded.Category, Reference=excluded.Reference,
                    FreeTaxPrice=excluded.FreeTaxPrice, Exchangeable=excluded.Exchangeable,
                    Insurance=excluded.Insurance, Deliverable=excluded.Deliverable,
                    Size=excluded.Size, Weight=excluded.Weight, Color=excluded.Color,
                    WheelSize=excluded.WheelSize, Electric=excluded.Electric, State=excluded.State,
                    Brand=excluded.Brand, Confort=excluded.Confort,
                    Order_IdOrder=excluded.Order_IdOrder, Shop_ShopId=excluded.Shop_ShopId,
                    SyncedAt=excluded.SyncedAt", db);

            cmd.Parameters.AddWithValue("@Id", bicycle.Id);
            cmd.Parameters.AddWithValue("@TypeOfBike", (object?)bicycle.TypeOfBike ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object?)bicycle.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Reference", (object?)bicycle.Reference ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FreeTaxPrice", bicycle.FreeTaxPrice);
            cmd.Parameters.AddWithValue("@Exchangeable", bicycle.Exchangeable ? 1 : 0);
            cmd.Parameters.AddWithValue("@Insurance", bicycle.Insurance ? 1 : 0);
            cmd.Parameters.AddWithValue("@Deliverable", bicycle.Deliverable ? 1 : 0);
            cmd.Parameters.AddWithValue("@Size", bicycle.Size);
            cmd.Parameters.AddWithValue("@Weight", bicycle.Weight);
            cmd.Parameters.AddWithValue("@Color", (object?)bicycle.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WheelSize", bicycle.WheelSize);
            cmd.Parameters.AddWithValue("@Electric", bicycle.Electric ? 1 : 0);
            cmd.Parameters.AddWithValue("@State", (object?)bicycle.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Brand", (object?)bicycle.Brand ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Confort", (object?)bicycle.Confort ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Order_IdOrder", (object?)bicycle.Order?.IdOrder ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Shop_ShopId", (object?)bicycle.Shop?.ShopId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        // ── Sellers ───────────────────────────────────────────────────────────

        public void UpsertSeller(Models.Seller seller)
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(@"
                INSERT INTO Sellers (Id, LastName, FirstName, Email, SyncedAt)
                VALUES (@Id, @LastName, @FirstName, @Email, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    LastName=excluded.LastName, FirstName=excluded.FirstName,
                    Email=excluded.Email, SyncedAt=excluded.SyncedAt", db);

            cmd.Parameters.AddWithValue("@Id", seller.Id);
            cmd.Parameters.AddWithValue("@LastName", (object?)seller.LastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstName", (object?)seller.FirstName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)seller.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        // ── Customers ─────────────────────────────────────────────────────────

        public void UpsertCustomer(Models.Customer customer)
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(@"
                INSERT INTO Customers (Id, Town, PostalCode, Address, LoyaltyPoints, Phone, Email, Gender, LastName, FirstName, SyncedAt)
                VALUES (@Id, @Town, @PostalCode, @Address, @LoyaltyPoints, @Phone, @Email, @Gender, @LastName, @FirstName, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    Town=excluded.Town, PostalCode=excluded.PostalCode, Address=excluded.Address,
                    LoyaltyPoints=excluded.LoyaltyPoints, Phone=excluded.Phone, Email=excluded.Email,
                    Gender=excluded.Gender, LastName=excluded.LastName, FirstName=excluded.FirstName,
                    SyncedAt=excluded.SyncedAt", db);

            cmd.Parameters.AddWithValue("@Id", customer.Id);
            cmd.Parameters.AddWithValue("@Town", (object?)customer.Town ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PostalCode", customer.PostalCode);
            cmd.Parameters.AddWithValue("@Address", (object?)customer.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LoyaltyPoints", customer.LoyaltyPoints);
            cmd.Parameters.AddWithValue("@Phone", (object?)customer.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Gender", (object?)customer.Gender ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", (object?)customer.LastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstName", (object?)customer.FirstName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
    }
}
