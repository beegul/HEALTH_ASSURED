namespace Checkout;

public class SpecialPrice
{
    public int Quantity { get; set; }
    public int Price { get; set; }

    public SpecialPrice(int quantity, int price)
    {
        Quantity = quantity;
        Price = price;
    }
}