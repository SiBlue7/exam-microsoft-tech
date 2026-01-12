namespace Exercice1_exam.ApiServicedotnet.DTO;

public record OrderResponseDto(
    List<OrderLineDto> products,
    List<DiscountDto> discounts,
    decimal total
);

public record OrderLineDto(
    int id,
    string name,
    int quantity,
    decimal price_per_unit,
    decimal total
);

public record DiscountDto(string type, decimal value);

public record ErrorResponseDto(List<string> errors);