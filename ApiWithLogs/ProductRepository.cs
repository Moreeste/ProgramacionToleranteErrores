namespace ApiWithLogs
{
    public static class ProductRepository
    {
        public static readonly List<Product> Products = new()
        {
            new Product(1, "Laptop", 1200.00m),
            new Product(2, "Smartphone", 800.00m),
            new Product(3, "Tablet", 400.00m),
            new Product(4, "Monitor", 300.00m),
            new Product(5, "Keyboard", 50.00m)
        };
    }
}
