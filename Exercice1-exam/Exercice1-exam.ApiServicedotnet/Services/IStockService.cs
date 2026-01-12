using Exercice1_exam.ApiServicedotnet.Models;

namespace Exercice1_exam.ApiServicedotnet.Services;

public interface IStockService
{
    IEnumerable<Product> GetAllProducts();
    Product? GetProductById(int id);
    bool TryReserveProducts(IEnumerable<(int productId, int quantity)> items, out List<string> errors);
}