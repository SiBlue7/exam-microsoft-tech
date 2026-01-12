using Exercice1_exam.ApiServicedotnet.Models;

namespace Exercice1_exam.ApiServicedotnet.Services;

public class StockService : IStockService
{
    private readonly List<Product> _products;
    private readonly object _lock = new();

    public StockService()
    {
        _products = new List<Product>
        {
            new Product { Id = 1, Name = "Clavier mécanique", UnitPrice = 100, Stock = 10 },
            new Product { Id = 2, Name = "Souris gaming", UnitPrice = 80, Stock = 15 },
            new Product { Id = 3, Name = "Casque audio", UnitPrice = 150, Stock = 8 },
            new Product { Id = 4, Name = "Écran 27 pouces", UnitPrice = 300, Stock = 5 },
            new Product { Id = 5, Name = "Tapis de souris", UnitPrice = 50, Stock = 30 },
            new Product { Id = 6, Name = "Manchettes", UnitPrice = 15, Stock = 12 }
        };
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _products;
    }

    public Product? GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }
    
    public bool TryReserveProducts(IEnumerable<(int productId, int quantity)> items, out List<string> errors)
    {
        errors = new List<string>();

        lock (_lock)
        {
            // 1) validations
            foreach (var (productId, quantity) in items)
            {
                var product = GetProductById(productId);
                if (product is null)
                {
                    errors.Add($"Le produit avec l'identifiant {productId} n'existe pas");
                    continue;
                }

                if (quantity <= 0)
                {
                    errors.Add($"La quantité demandée pour le produit {product.Name} est invalide");
                    continue;
                }

                if (quantity > product.Stock)
                {
                    errors.Add($"Il ne reste que {product.Stock} exemplaire pour le produit {product.Name}");
                }
            }

            if (errors.Count > 0)
                return false;

            // 2) réservation (décrément)
            foreach (var (productId, quantity) in items)
            {
                var product = GetProductById(productId)!;
                product.Stock -= quantity;
            }

            return true;
        }
    }
}