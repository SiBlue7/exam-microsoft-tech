namespace Exercice1_exam.ApiServicedotnet.DTO;

public class CreateOrderRequestDto
{
    public List<OrderProductRequestDto> Products { get; set; } = new();
    public string? Promo_Code { get; set; } // correspond à "promo_code"
}

public class OrderProductRequestDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}