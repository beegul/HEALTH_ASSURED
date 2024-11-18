namespace Checkout;

public class SpecialPrice
{
    public int Quantity { get; set; }
    public int Price { get; set; }

    public SpecialPrice(int quantity, int price)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }
        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.", nameof(price));
        }
        Quantity = quantity;
        Price = price;
    }
}