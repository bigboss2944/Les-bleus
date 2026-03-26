using Microsoft.Data.Sqlite;

namespace AspNet_FilRouge.Services
{
    /// <summary>
    /// Service de base de données locale SQLite (sans Entity Framework).
    /// Utilisé pour la synchronisation et le mode hors-ligne.
    /// </summary>
    public class LocalDbService : ILocalDbService
    {
        private readonly string _dbPath;

        /// <summary>
        /// Sérialise toutes les opérations d'écriture pour éviter les conflits
        /// de verrou SQLite entre le service de fond et les requêtes HTTP.
        /// </summary>
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public LocalDbService(IWebHostEnvironment env)
        {
            _dbPath = Path.Combine(env.ContentRootPath, "local_cache.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var db = OpenConnection();

            // WAL (Write-Ahead Logging) : permet des lectures concurrentes pendant les écritures
            // et réduit considérablement les conflits de verrou.
            ExecuteNonQuery(db, "PRAGMA journal_mode=WAL");

            // Réduire le nombre de fsync tout en restant sûr en mode WAL.
            ExecuteNonQuery(db, "PRAGMA synchronous=NORMAL");

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

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS StockRequestStatus (
                    Id INTEGER PRIMARY KEY,
                    Status TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                )");

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS StockRequests (
                    Id INTEGER PRIMARY KEY,
                    BicycleName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    RequestDate TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    RequestedById TEXT NULL,
                    Notes TEXT NULL,
                    SyncedAt TEXT NULL
                )");

            ExecuteNonQuery(db, @"
                CREATE TABLE IF NOT EXISTS PendingVendorStatusUpdates (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RequestId INTEGER NOT NULL,
                    Status TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    LastError TEXT NULL,
                    RetryCount INTEGER NOT NULL DEFAULT 0
                )");
        }

        private SqliteConnection OpenConnection()
        {
            var db = new SqliteConnection($"Filename={_dbPath}");
            db.Open();
            // Délai d'attente avant d'abandonner sur un verrou : évite les exceptions
            // immédiates "database is locked" lors de l'accès concurrent.
            using var cmd = new SqliteCommand("PRAGMA busy_timeout=5000", db);
            cmd.ExecuteNonQuery();
            return db;
        }

        private static void ExecuteNonQuery(SqliteConnection db, string sql)
        {
            using var cmd = new SqliteCommand(sql, db);
            cmd.ExecuteNonQuery();
        }

        // ── Bulk write (synchronisation complète, atomique) ────────────────────

        /// <summary>
        /// Écrit toutes les données en une seule transaction atomique.
        /// Protégé par un verrou pour éviter les conflits entre le service
        /// de fond et les requêtes HTTP simultanées.
        /// </summary>
        public async Task BulkUpsertAllAsync(
            IEnumerable<Order> orders,
            IEnumerable<Bicycle> bicycles,
            IEnumerable<Seller> sellers,
            IEnumerable<Customer> customers)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                foreach (var order in orders)
                    ExecuteUpsertOrder(order, db, tx);

                foreach (var bicycle in bicycles)
                    ExecuteUpsertBicycle(bicycle, db, tx);

                foreach (var seller in sellers)
                    ExecuteUpsertSeller(seller, db, tx);

                foreach (var customer in customers)
                    ExecuteUpsertCustomer(customer, db, tx);

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        // ── Orders ────────────────────────────────────────────────────────────

        private static void ExecuteUpsertOrder(Order order, SqliteConnection db, SqliteTransaction tx)
        {
            using var cmd = new SqliteCommand(@"
                INSERT INTO Orders (IdOrder, Date, PayMode, Discount, UseLoyaltyPoint, Tax, ShippingCost, IsValidated,
                                    SellerId, CustomerId, ShopId, SyncedAt)
                VALUES (@IdOrder, @Date, @PayMode, @Discount, @UseLoyaltyPoint, @Tax, @ShippingCost, @IsValidated,
                        @SellerId, @CustomerId, @ShopId, @SyncedAt)
                ON CONFLICT(IdOrder) DO UPDATE SET
                    Date=excluded.Date, PayMode=excluded.PayMode, Discount=excluded.Discount,
                    UseLoyaltyPoint=excluded.UseLoyaltyPoint, Tax=excluded.Tax, ShippingCost=excluded.ShippingCost,
                    IsValidated=excluded.IsValidated, SellerId=excluded.SellerId, CustomerId=excluded.CustomerId,
                    ShopId=excluded.ShopId, SyncedAt=excluded.SyncedAt", db, tx);

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

        /// <summary>
        /// Retourne toutes les commandes de la base locale.
        /// Les résultats sont matérialisés avant la fermeture de la connexion
        /// pour éviter de maintenir un verrou de lecture pendant l'itération.
        /// </summary>
        public IEnumerable<(long IdOrder, DateTime Date, string? PayMode, float Discount, bool IsValidated)> GetOrders()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand("SELECT IdOrder, Date, PayMode, Discount, IsValidated FROM Orders ORDER BY Date DESC", db);
            using var reader = cmd.ExecuteReader();
            var results = new List<(long IdOrder, DateTime Date, string? PayMode, float Discount, bool IsValidated)>();
            while (reader.Read())
            {
                results.Add((
                    reader.GetInt64(0),
                    DateTime.Parse(reader.GetString(1)),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    (float)reader.GetDouble(3),
                    reader.GetInt32(4) != 0
                ));
            }
            return results;
        }

        // ── Bicycles ──────────────────────────────────────────────────────────

        private static void ExecuteUpsertBicycle(Bicycle bicycle, SqliteConnection db, SqliteTransaction tx)
        {
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
                    SyncedAt=excluded.SyncedAt", db, tx);

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

        private static void ExecuteUpsertSeller(Seller seller, SqliteConnection db, SqliteTransaction tx)
        {
            using var cmd = new SqliteCommand(@"
                INSERT INTO Sellers (Id, LastName, FirstName, Email, SyncedAt)
                VALUES (@Id, @LastName, @FirstName, @Email, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    LastName=excluded.LastName, FirstName=excluded.FirstName,
                    Email=excluded.Email, SyncedAt=excluded.SyncedAt", db, tx);

            cmd.Parameters.AddWithValue("@Id", seller.Id);
            cmd.Parameters.AddWithValue("@LastName", (object?)seller.LastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstName", (object?)seller.FirstName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)seller.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        // ── Customers ─────────────────────────────────────────────────────────

        private static void ExecuteUpsertCustomer(Customer customer, SqliteConnection db, SqliteTransaction tx)
        {
            using var cmd = new SqliteCommand(@"
                INSERT INTO Customers (Id, Town, PostalCode, Address, LoyaltyPoints, Phone, Email, Gender, LastName, FirstName, SyncedAt)
                VALUES (@Id, @Town, @PostalCode, @Address, @LoyaltyPoints, @Phone, @Email, @Gender, @LastName, @FirstName, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    Town=excluded.Town, PostalCode=excluded.PostalCode, Address=excluded.Address,
                    LoyaltyPoints=excluded.LoyaltyPoints, Phone=excluded.Phone, Email=excluded.Email,
                    Gender=excluded.Gender, LastName=excluded.LastName, FirstName=excluded.FirstName,
                    SyncedAt=excluded.SyncedAt", db, tx);

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

        // ── Statistics ────────────────────────────────────────────────────────

        /// <summary>
        /// Retourne les statistiques de la base locale : nombre d'enregistrements
        /// par table et date de la dernière synchronisation.
        /// </summary>
        public (int Orders, int Bicycles, int Sellers, int Customers, DateTime? LastSyncedAt) GetStats()
        {
            using var db = OpenConnection();

            using var cmd = new SqliteCommand(@"
                SELECT
                    (SELECT COUNT(*) FROM Orders)   AS OrderCount,
                    (SELECT COUNT(*) FROM Bicycles) AS BicycleCount,
                    (SELECT COUNT(*) FROM Sellers)  AS SellerCount,
                    (SELECT COUNT(*) FROM Customers) AS CustomerCount,
                    (SELECT MAX(SyncedAt) FROM (
                        SELECT MAX(SyncedAt) AS SyncedAt FROM Orders
                        UNION ALL SELECT MAX(SyncedAt) FROM Bicycles
                        UNION ALL SELECT MAX(SyncedAt) FROM Sellers
                        UNION ALL SELECT MAX(SyncedAt) FROM Customers
                    )) AS LastSyncedAt", db);

            using var reader = cmd.ExecuteReader();
            reader.Read();

            int orders    = reader.GetInt32(0);
            int bicycles  = reader.GetInt32(1);
            int sellers   = reader.GetInt32(2);
            int customers = reader.GetInt32(3);

            var lastSyncStr = reader.IsDBNull(4) ? null : reader.GetString(4);
            DateTime? lastSync = null;
            if (lastSyncStr != null &&
                DateTime.TryParse(lastSyncStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
            {
                lastSync = parsed;
            }

            return (orders, bicycles, sellers, customers, lastSync);
        }

        // ── Stock Request Status ──────────────────────────────────────────────

        /// <summary>
        /// Enregistre une mise à jour de statut pour une demande de stock.
        /// Permettra au vendeur de lire ces changements lors de sa synchronisation.
        /// </summary>
        public async Task SaveStockRequestStatusAsync(int stockRequestId, string newStatus)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var cmd = new SqliteCommand(@"
                    INSERT INTO StockRequestStatus (Id, Status, UpdatedAt)
                    VALUES (@Id, @Status, @UpdatedAt)
                    ON CONFLICT(Id) DO UPDATE SET
                        Status=excluded.Status,
                        UpdatedAt=excluded.UpdatedAt", db);

                cmd.Parameters.AddWithValue("@Id", stockRequestId);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow.ToString("o"));
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// Retourne un dictionnaire Id → Statut pour toutes les demandes de stock
        /// dont le statut a été mis à jour par l'admin.
        /// </summary>
        public Dictionary<int, string> GetStockRequestStatuses()
        {
            var result = new Dictionary<int, string>();
            using var db = OpenConnection();

            using var checkCmd = new SqliteCommand(
                "SELECT name FROM sqlite_master WHERE type='table' AND name='StockRequestStatus';", db);
            if (checkCmd.ExecuteScalar() == null) return result;

            using var cmd = new SqliteCommand("SELECT Id, Status FROM StockRequestStatus", db);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result[reader.GetInt32(0)] = reader.GetString(1);
            }
            return result;
        }

        /// <summary>
        /// Écrit toutes les demandes de stock dans la base locale.
        /// </summary>
        public async Task BulkUpsertStockRequestsAsync(IEnumerable<StockRequest> requests)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                foreach (var request in requests)
                    ExecuteUpsertStockRequest(request, db, tx);

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private static void ExecuteUpsertStockRequest(StockRequest request, SqliteConnection db, SqliteTransaction tx)
        {
            using var cmd = new SqliteCommand(@"
                INSERT INTO StockRequests (Id, BicycleName, Quantity, RequestDate, Status, RequestedById, Notes, SyncedAt)
                VALUES (@Id, @BicycleName, @Quantity, @RequestDate, @Status, @RequestedById, @Notes, @SyncedAt)
                ON CONFLICT(Id) DO UPDATE SET
                    BicycleName=excluded.BicycleName, Quantity=excluded.Quantity,
                    RequestDate=excluded.RequestDate, Status=excluded.Status,
                    RequestedById=excluded.RequestedById, Notes=excluded.Notes,
                    SyncedAt=excluded.SyncedAt", db, tx);

            cmd.Parameters.AddWithValue("@Id", request.Id);
            cmd.Parameters.AddWithValue("@BicycleName", request.BicycleName ?? throw new InvalidOperationException($"BicycleName ne peut pas être null pour la demande Id={request.Id}."));
            cmd.Parameters.AddWithValue("@Quantity", request.Quantity);
            cmd.Parameters.AddWithValue("@RequestDate", request.RequestDate.ToString("o"));
            cmd.Parameters.AddWithValue("@Status", request.Status);
            cmd.Parameters.AddWithValue("@RequestedById", (object?)request.RequestedById ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object?)request.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SyncedAt", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        // ── Pending Vendor Sync Queue ───────────────────────────────────────

        public async Task EnqueuePendingVendorStatusUpdateAsync(int requestId, string status, string? lastError)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var cmd = new SqliteCommand(@"
                    INSERT INTO PendingVendorStatusUpdates (RequestId, Status, CreatedAt, LastError, RetryCount)
                    VALUES (@RequestId, @Status, @CreatedAt, @LastError, 0)", db);

                cmd.Parameters.AddWithValue("@RequestId", requestId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow.ToString("o"));
                cmd.Parameters.AddWithValue("@LastError", (object?)lastError ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public List<PendingVendorStatusUpdate> GetPendingVendorStatusUpdates(int limit)
        {
            var items = new List<PendingVendorStatusUpdate>();
            using var db = OpenConnection();

            using var cmd = new SqliteCommand(@"
                SELECT Id, RequestId, Status, CreatedAt, RetryCount, LastError
                FROM PendingVendorStatusUpdates
                ORDER BY Id ASC
                LIMIT @Limit", db);
            cmd.Parameters.AddWithValue("@Limit", limit);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var createdAtRaw = reader.GetString(3);
                DateTime.TryParse(createdAtRaw, null,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out var createdAt);

                items.Add(new PendingVendorStatusUpdate(
                    Id: reader.GetInt64(0),
                    RequestId: reader.GetInt32(1),
                    Status: reader.GetString(2),
                    CreatedAt: createdAt,
                    RetryCount: reader.GetInt32(4),
                    LastError: reader.IsDBNull(5) ? null : reader.GetString(5)));
            }

            return items;
        }

        public async Task DeletePendingVendorStatusUpdateAsync(long id)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var cmd = new SqliteCommand("DELETE FROM PendingVendorStatusUpdates WHERE Id=@Id", db);
                cmd.Parameters.AddWithValue("@Id", id);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task MarkPendingVendorStatusUpdateRetryAsync(long id, string? lastError)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var cmd = new SqliteCommand(@"
                    UPDATE PendingVendorStatusUpdates
                    SET RetryCount = RetryCount + 1,
                        LastError = @LastError
                    WHERE Id=@Id", db);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@LastError", (object?)lastError ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}
