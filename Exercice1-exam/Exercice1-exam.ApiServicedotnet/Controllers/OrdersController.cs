using Exercice1_exam.ApiServicedotnet.DTO;
using Exercice1_exam.ApiServicedotnet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exercice1_exam.ApiServicedotnet.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IStockService _stock;

    public OrdersController(IStockService stock)
    {
        _stock = stock;
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var errors = new List<string>();

        var productsRequest = request.Products ?? new List<OrderProductRequestDto>();

        if (productsRequest.Count == 0)
            errors.Add("La commande doit contenir au moins un produit");

        if (errors.Count > 0)
            return BadRequest(new ErrorResponseDto(errors));

        var grouped = productsRequest
            .GroupBy(p => p.Id)
            .Select(g => (productId: g.Key, quantity: g.Sum(x => x.Quantity)))
            .ToList();

        decimal baseTotal = 0m;

        foreach (var (productId, qty) in grouped)
        {
            var product = _stock.GetProductById(productId);

            if (product is null)
            {
                errors.Add($"Le produit avec l'identifiant {productId} n'existe pas");
                continue;
            }

            if (qty <= 0)
            {
                errors.Add($"La quantité demandée pour le produit {product.Name} est invalide");
                continue;
            }

            if (qty > product.Stock)
            {
                errors.Add($"Il ne reste que {product.Stock} exemplaire pour le produit {product.Name}");
            }

            baseTotal += product.UnitPrice * qty;
        }

        var promoCode = request.Promo_Code?.Trim();
        decimal promoPct = 0m;

        if (!string.IsNullOrEmpty(promoCode))
        {
            promoPct = promoCode switch
            {
                "DISCOUNT20" => 0.20m,
                "DISCOUNT10" => 0.10m,
                _ => -1m
            };

            if (promoPct < 0m)
            {
                errors.Add("Le code promo est invalide");
            }
            else if (baseTotal < 50m)
            {
                errors.Add("Les codes promos ne sont valables qu'a partir de 50€ d'achat");
            }
        }

        if (errors.Count > 0)
            return BadRequest(new ErrorResponseDto(errors));
        if (!_stock.TryReserveProducts(grouped.Select(x => (x.productId, x.quantity)), out var stockErrors))
        {
            return BadRequest(new ErrorResponseDto(stockErrors));
        }

        var lines = new List<OrderLineDto>();
        decimal subtotalAfterProductDiscounts = 0m;

        foreach (var (productId, qty) in grouped)
        {
            var product = _stock.GetProductById(productId)!;

            var lineBase = product.UnitPrice * qty;
            var lineTotal = qty > 5 ? lineBase * 0.90m : lineBase;

            lineTotal = Round2(lineTotal);
            subtotalAfterProductDiscounts += lineTotal;

            lines.Add(new OrderLineDto(
                id: product.Id,
                name: product.Name,
                quantity: qty,
                price_per_unit: Round2(product.UnitPrice),
                total: lineTotal
            ));
        }

        subtotalAfterProductDiscounts = Round2(subtotalAfterProductDiscounts);

        var discounts = new List<DiscountDto>();
        decimal orderPct = subtotalAfterProductDiscounts > 100m ? 0.05m : 0m;

        if (orderPct > 0m)
        {
            var orderValue = Round2(subtotalAfterProductDiscounts * orderPct);
            discounts.Add(new DiscountDto("order", orderValue));
        }

        if (promoPct > 0m)
        {
            var promoValue = Round2(subtotalAfterProductDiscounts * promoPct);
            discounts.Add(new DiscountDto("promo", promoValue));
        }

        var combinedPct = orderPct + promoPct;
        var finalTotal = Round2(subtotalAfterProductDiscounts * (1m - combinedPct));

        var response = new OrderResponseDto(
            products: lines,
            discounts: discounts,
            total: finalTotal
        );

        return Ok(response);
    }

    private static decimal Round2(decimal value)
        => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
