using Exercice1_exam.ApiServicedotnet.Controllers;
using Exercice1_exam.ApiServicedotnet.Services;

namespace Exercice1_exam.Tests;

public static class TestHelpers
{
    public static (OrdersController controller, StockService stock) CreateSut()
    {
        var stock = new StockService();
        var controller = new OrdersController(stock);
        return (controller, stock);
    }
}