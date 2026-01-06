namespace ProductRepository_Exception
{
    public class ProductRepository
    {
        private List<Product> _products = new List<Product>()
        {
            new Product(1, "Laptop"),
            new Product(2, "Smartphone"),
            new Product(3, "Tablet")
        };

        public Product GetById(int id)
        {
            if (id <= 0)
            {
                throw new KeyNotFoundException("El id del producto debe ser entero positivo");
            }

            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException("Producto no existente");
            }

            return product;
        }
    }

    public record Product(int Id, string Name);
}
