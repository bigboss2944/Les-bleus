using System.Globalization;
using Maui_FilRouge.Models;
using Microsoft.Data.Sqlite;

namespace Maui_FilRouge.Services
{
    /// <inheritdoc cref="ILocalCacheService"/>
    public class LocalCacheService : ILocalCacheService
    {
        private readonly string _dbPath;
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private bool _initialized;

        public LocalCacheService()
        {
            _dbPath = Path.Combine(FileSystem.Current.AppDataDirectory, "filrouge_local_cache.db");
        }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            await _writeLock.WaitAsync();
            try
            {
                if (_initialized) return;

                using var db = OpenConnection();
                ExecuteNonQuery(db, "PRAGMA journal_mode=WAL");
                ExecuteNonQuery(db, "PRAGMA synchronous=NORMAL");

                ExecuteNonQuery(db, @"
                    CREATE TABLE IF NOT EXISTS Orders (
                        IdOrder INTEGER PRIMARY KEY,
                        Date TEXT NOT NULL,
                        IsValidated INTEGER NOT NULL,
                        Discount REAL NOT NULL,
                        Total REAL NOT NULL,
                        BicycleCount INTEGER NOT NULL,
                        SellerId TEXT NULL,
                        SellerFirstName TEXT NULL,
                        SellerLastName TEXT NULL,
                        CustomerId TEXT NULL,
                        CustomerFirstName TEXT NULL,
                        CustomerLastName TEXT NULL,
                        CustomerEmail TEXT NULL
                    )");

                ExecuteNonQuery(db, @"
                    CREATE TABLE IF NOT EXISTS Bicycles (
                        Id INTEGER PRIMARY KEY,
                        TypeOfBike TEXT NULL,
                        Category TEXT NULL,
                        Reference TEXT NULL,
                        FreeTaxPrice REAL NOT NULL,
                        Tax REAL NOT NULL,
                        Quantity INTEGER NOT NULL,
                        Exchangeable INTEGER NOT NULL,
                        Insurance INTEGER NOT NULL,
                        Deliverable INTEGER NOT NULL
                    )");

                ExecuteNonQuery(db, @"
                    CREATE TABLE IF NOT EXISTS Sellers (
                        Id TEXT PRIMARY KEY,
                        FirstName TEXT NULL,
                        LastName TEXT NULL
                    )");

                ExecuteNonQuery(db, @"
                    CREATE TABLE IF NOT EXISTS Customers (
                        Id TEXT PRIMARY KEY,
                        FirstName TEXT NULL,
                        LastName TEXT NULL,
                        Email TEXT NULL
                    )");

                ExecuteNonQuery(db, @"
                    CREATE TABLE IF NOT EXISTS SyncState (
                        Id INTEGER PRIMARY KEY CHECK (Id = 1),
                        LastSyncedAtUtc TEXT NULL
                    )");
                ExecuteNonQuery(db, "INSERT OR IGNORE INTO SyncState (Id, LastSyncedAtUtc) VALUES (1, NULL)");

                _initialized = true;
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task ReplaceOrdersAsync(IEnumerable<OrderSummary> orders)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                ExecuteNonQuery(db, "DELETE FROM Orders", tx);
                foreach (var order in orders)
                {
                    using var cmd = new SqliteCommand(@"
                        INSERT INTO Orders (IdOrder, Date, IsValidated, Discount, Total, BicycleCount,
                                            SellerId, SellerFirstName, SellerLastName,
                                            CustomerId, CustomerFirstName, CustomerLastName, CustomerEmail)
                        VALUES (@IdOrder, @Date, @IsValidated, @Discount, @Total, @BicycleCount,
                                @SellerId, @SellerFirstName, @SellerLastName,
                                @CustomerId, @CustomerFirstName, @CustomerLastName, @CustomerEmail)", db, tx);

                    cmd.Parameters.AddWithValue("@IdOrder", order.IdOrder);
                    cmd.Parameters.AddWithValue("@Date", order.Date.ToString("o"));
                    cmd.Parameters.AddWithValue("@IsValidated", order.IsValidated ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Discount", order.Discount);
                    cmd.Parameters.AddWithValue("@Total", order.Total);
                    cmd.Parameters.AddWithValue("@BicycleCount", order.BicycleCount);
                    cmd.Parameters.AddWithValue("@SellerId", (object?)order.Seller?.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SellerFirstName", (object?)order.Seller?.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SellerLastName", (object?)order.Seller?.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerId", (object?)order.Customer?.Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerFirstName", (object?)order.Customer?.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerLastName", (object?)order.Customer?.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerEmail", (object?)order.Customer?.Email ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                ExecuteNonQuery(db, "UPDATE SyncState SET LastSyncedAtUtc = @Now WHERE Id = 1", tx,
                    ("@Now", DateTime.UtcNow.ToString("o")));

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public Task<List<OrderSummary>> GetOrdersAsync()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(
                "SELECT IdOrder, Date, IsValidated, Discount, Total, BicycleCount, SellerId, SellerFirstName, SellerLastName, CustomerId, CustomerFirstName, CustomerLastName, CustomerEmail FROM Orders ORDER BY Date DESC", db);
            using var reader = cmd.ExecuteReader();

            var results = new List<OrderSummary>();
            while (reader.Read())
            {
                var sellerId = reader.IsDBNull(6) ? null : reader.GetString(6);
                var customerId = reader.IsDBNull(9) ? null : reader.GetString(9);

                results.Add(new OrderSummary
                {
                    IdOrder = reader.GetInt64(0),
                    Date = DateTime.Parse(reader.GetString(1), null, DateTimeStyles.RoundtripKind),
                    IsValidated = reader.GetInt32(2) != 0,
                    Discount = (float)reader.GetDouble(3),
                    Total = (float)reader.GetDouble(4),
                    BicycleCount = reader.GetInt32(5),
                    Seller = sellerId == null ? null : new SellerSummary
                    {
                        Id = sellerId,
                        FirstName = reader.IsDBNull(7) ? null : reader.GetString(7),
                        LastName = reader.IsDBNull(8) ? null : reader.GetString(8)
                    },
                    Customer = customerId == null ? null : new CustomerSummary
                    {
                        Id = customerId,
                        FirstName = reader.IsDBNull(10) ? null : reader.GetString(10),
                        LastName = reader.IsDBNull(11) ? null : reader.GetString(11),
                        Email = reader.IsDBNull(12) ? null : reader.GetString(12)
                    }
                });
            }
            return Task.FromResult(results);
        }

        public async Task ReplaceBicyclesAsync(IEnumerable<BicycleSummary> bicycles)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                ExecuteNonQuery(db, "DELETE FROM Bicycles", tx);
                foreach (var b in bicycles)
                {
                    using var cmd = new SqliteCommand(@"
                        INSERT INTO Bicycles (Id, TypeOfBike, Category, Reference, FreeTaxPrice, Tax, Quantity, Exchangeable, Insurance, Deliverable)
                        VALUES (@Id, @TypeOfBike, @Category, @Reference, @FreeTaxPrice, @Tax, @Quantity, @Exchangeable, @Insurance, @Deliverable)", db, tx);

                    cmd.Parameters.AddWithValue("@Id", b.Id);
                    cmd.Parameters.AddWithValue("@TypeOfBike", (object?)b.TypeOfBike ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Category", (object?)b.Category ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference", (object?)b.Reference ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FreeTaxPrice", b.FreeTaxPrice);
                    cmd.Parameters.AddWithValue("@Tax", b.Tax);
                    cmd.Parameters.AddWithValue("@Quantity", b.Quantity);
                    cmd.Parameters.AddWithValue("@Exchangeable", b.Exchangeable ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Insurance", b.Insurance ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Deliverable", b.Deliverable ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public Task<List<BicycleSummary>> GetBicyclesAsync()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand(
                "SELECT Id, TypeOfBike, Category, Reference, FreeTaxPrice, Tax, Quantity, Exchangeable, Insurance, Deliverable FROM Bicycles", db);
            using var reader = cmd.ExecuteReader();

            var results = new List<BicycleSummary>();
            while (reader.Read())
            {
                results.Add(new BicycleSummary
                {
                    Id = reader.GetInt64(0),
                    TypeOfBike = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Category = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Reference = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FreeTaxPrice = (float)reader.GetDouble(4),
                    Tax = (float)reader.GetDouble(5),
                    Quantity = reader.GetInt32(6),
                    Exchangeable = reader.GetInt32(7) != 0,
                    Insurance = reader.GetInt32(8) != 0,
                    Deliverable = reader.GetInt32(9) != 0
                });
            }
            return Task.FromResult(results);
        }

        public async Task ReplaceSellersAsync(IEnumerable<SellerSummary> sellers)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                ExecuteNonQuery(db, "DELETE FROM Sellers", tx);
                foreach (var s in sellers)
                {
                    using var cmd = new SqliteCommand(
                        "INSERT INTO Sellers (Id, FirstName, LastName) VALUES (@Id, @FirstName, @LastName)", db, tx);
                    cmd.Parameters.AddWithValue("@Id", s.Id);
                    cmd.Parameters.AddWithValue("@FirstName", (object?)s.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object?)s.LastName ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public Task<List<SellerSummary>> GetSellersAsync()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand("SELECT Id, FirstName, LastName FROM Sellers", db);
            using var reader = cmd.ExecuteReader();

            var results = new List<SellerSummary>();
            while (reader.Read())
            {
                results.Add(new SellerSummary
                {
                    Id = reader.GetString(0),
                    FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    LastName = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return Task.FromResult(results);
        }

        public async Task ReplaceCustomersAsync(IEnumerable<CustomerSummary> customers)
        {
            await _writeLock.WaitAsync();
            try
            {
                using var db = OpenConnection();
                using var tx = db.BeginTransaction();

                ExecuteNonQuery(db, "DELETE FROM Customers", tx);
                foreach (var c in customers)
                {
                    using var cmd = new SqliteCommand(
                        "INSERT INTO Customers (Id, FirstName, LastName, Email) VALUES (@Id, @FirstName, @LastName, @Email)", db, tx);
                    cmd.Parameters.AddWithValue("@Id", c.Id);
                    cmd.Parameters.AddWithValue("@FirstName", (object?)c.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object?)c.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public Task<List<CustomerSummary>> GetCustomersAsync()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand("SELECT Id, FirstName, LastName, Email FROM Customers", db);
            using var reader = cmd.ExecuteReader();

            var results = new List<CustomerSummary>();
            while (reader.Read())
            {
                results.Add(new CustomerSummary
                {
                    Id = reader.GetString(0),
                    FirstName = reader.IsDBNull(1) ? null : reader.GetString(1),
                    LastName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return Task.FromResult(results);
        }

        public Task<DateTime?> GetLastSyncedAtUtcAsync()
        {
            using var db = OpenConnection();
            using var cmd = new SqliteCommand("SELECT LastSyncedAtUtc FROM SyncState WHERE Id = 1", db);
            var result = cmd.ExecuteScalar() as string;
            return Task.FromResult(result == null
                ? (DateTime?)null
                : DateTime.Parse(result, null, DateTimeStyles.RoundtripKind));
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private SqliteConnection OpenConnection()
        {
            var db = new SqliteConnection($"Filename={_dbPath}");
            db.Open();
            using var cmd = new SqliteCommand("PRAGMA busy_timeout=5000", db);
            cmd.ExecuteNonQuery();
            return db;
        }

        private static void ExecuteNonQuery(SqliteConnection db, string sql, SqliteTransaction? tx = null, params (string Name, object Value)[] parameters)
        {
            using var cmd = new SqliteCommand(sql, db, tx);
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value);
            cmd.ExecuteNonQuery();
        }
    }
}
