using Exercice1_exam.ApiServicedotnet.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Exercice1_exam.Tests;

public class OrdersControllerTests
{
    [Fact]
    // Commande valide sans promo ni remise
    public void PostOrders_Valid_NoPromo_NoDiscounts_ReturnsOkAndReservesStock()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 5, Quantity = 1 }
            }
        };

        var result = controller.CreateOrder(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<OrderResponseDto>(ok.Value);

        Assert.Equal(50m, body.total);
        Assert.Empty(body.discounts);
        Assert.Equal(29, stock.GetProductById(5)!.Stock);
    }

    [Fact]
    // Remise de 10% appliquée sur un produit avec quantité > 5
    public void PostOrders_QuantityGreaterThan5_Applies10PercentOnLineOnly()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 1, Quantity = 6 }
            }
        };

        var result = controller.CreateOrder(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<OrderResponseDto>(ok.Value);

        Assert.Equal(513.00m, body.total);
        Assert.Single(body.discounts);
        Assert.Equal("order", body.discounts[0].type);
        Assert.Equal(27.00m, body.discounts[0].value);
        Assert.Equal(4, stock.GetProductById(1)!.Stock);
    }

    [Fact]
    // Remise de 5% appliquée sur une commande supérieure à 100€
    public void PostOrders_TotalOver100_AppliesOrderDiscount5Percent()
    {
        var (controller, _) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 2, Quantity = 2 }
            }
        };

        var result = controller.CreateOrder(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<OrderResponseDto>(ok.Value);

        Assert.Contains(body.discounts, d => d.type == "order" && d.value == 8.00m);
        Assert.Equal(152.00m, body.total);
    }

    [Fact]
    // Code promo valide appliqué de manière additive avec la remise commande
    public void PostOrders_ValidPromo_AddsPromoDiscount_AdditiveWithOrder()
    {
        var (controller, _) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 1, Quantity = 2 }
            },
            Promo_Code = "DISCOUNT10"
        };

        var result = controller.CreateOrder(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<OrderResponseDto>(ok.Value);

        Assert.Contains(body.discounts, d => d.type == "order" && d.value == 10.00m);
        Assert.Contains(body.discounts, d => d.type == "promo" && d.value == 20.00m);
        Assert.Equal(170.00m, body.total);
    }

    [Fact]
    // Code promo invalide rejeté avec erreur
    public void PostOrders_InvalidPromo_ReturnsBadRequestWithError()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 1, Quantity = 1 }
            },
            Promo_Code = "BAD"
        };

        var result = controller.CreateOrder(request);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var body = Assert.IsType<ErrorResponseDto>(bad.Value);

        Assert.Contains("Le code promo est invalide", body.errors);
        Assert.Equal(10, stock.GetProductById(1)!.Stock);
    }

    [Fact]
    // Stock insuffisant empêche la validation de la commande
    public void PostOrders_StockInsufficient_ReturnsError_AndDoesNotReserve()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 4, Quantity = 999 }
            }
        };

        var result = controller.CreateOrder(request);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var body = Assert.IsType<ErrorResponseDto>(bad.Value);

        Assert.Contains("Il ne reste que 5 exemplaire pour le produit Écran 27 pouces", body.errors);
        Assert.Equal(5, stock.GetProductById(4)!.Stock);
    }

    [Fact]
    // Plusieurs erreurs détectées et retournées dans une seule réponse
    public void PostOrders_MultipleErrors_AreAllReturned()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 999, Quantity = 1 },
                new OrderProductRequestDto { Id = 4, Quantity = 999 },
                new OrderProductRequestDto { Id = 1, Quantity = 0 }
            },
            Promo_Code = "BAD"
        };

        var result = controller.CreateOrder(request);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var body = Assert.IsType<ErrorResponseDto>(bad.Value);

        Assert.Contains("Le produit avec l'identifiant 999 n'existe pas", body.errors);
        Assert.Contains("Il ne reste que 5 exemplaire pour le produit Écran 27 pouces", body.errors);
        Assert.Contains("La quantité demandée pour le produit Clavier mécanique est invalide", body.errors);
        Assert.Contains("Le code promo est invalide", body.errors);

        Assert.Equal(10, stock.GetProductById(1)!.Stock);
        Assert.Equal(5, stock.GetProductById(4)!.Stock);
    }

    [Fact]
    // Code promo refusé si le total avant remise est inférieur à 50€
    public void PostOrders_PromoCode_WhenTotalBeforeDiscountsIsBelow50_ReturnsError()
    {
        var (controller, stock) = TestHelpers.CreateSut();

        var request = new CreateOrderRequestDto
        {
            Products = new()
            {
                new OrderProductRequestDto { Id = 6, Quantity = 3 }
            },
            Promo_Code = "DISCOUNT10"
        };

        var result = controller.CreateOrder(request);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var body = Assert.IsType<ErrorResponseDto>(bad.Value);

        Assert.Contains(
            "Les codes promos ne sont valables qu'a partir de 50€ d'achat",
            body.errors
        );

        Assert.Equal(12, stock.GetProductById(6)!.Stock);
    }
}
