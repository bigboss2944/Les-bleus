using Database;

using (var db = new EntitiesContext())
{
    db.Database.EnsureCreated();

    foreach (var item in db.Orders.ToList())
    {
        Console.WriteLine(item.ToString());
    }
    Console.ReadLine();
}
