namespace PedrosCantina
{
    // Ændr Server-navnet, hvis din SQL Server hedder noget andet.
    // Eksempler:
    //   Server=localhost;
    //   Server=.\SQLEXPRESS;
    //   Server=(localdb)\MSSQLLocalDB;
    public static class Database
    {
        public const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=PedrosCantina;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
