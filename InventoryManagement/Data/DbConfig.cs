namespace InventoryManagement.Data
{
    public static class DbConfig
    {
        public const string ConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=InventoryDB;" +
            "Trusted_Connection=True;TrustServerCertificate=True;";
    }
}