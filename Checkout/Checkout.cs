namespace Checkout;

public class Checkout : ICheckout
{
    private readonly Dictionary<string, int> prices;
    private readonly Dictionary<string, SpecialPrice> specialOffers;
    private readonly Dictionary<string, int> scannedItems;

    public Checkout(Dictionary<string, int> prices, Dictionary<string, SpecialPrice> specialOffers)
    {
        this.prices = prices;
        this.specialOffers = specialOffers;
        scannedItems = new Dictionary<string, int>();
    }
    
    public void Scan(string item)
    {
        scannedItems.TryAdd(item, 0);
        scannedItems[item]++;
    }

    public int GetTotalPrice()
    {
        var totalPrice = 0;

        foreach (var item in scannedItems)
        {
            if (specialOffers.TryGetValue(item.Key, out var offer))
            {
                var offerCount = item.Value / offer.Quantity;
                
                totalPrice += offerCount * offer.Price;
                totalPrice += (item.Value % offer.Quantity) * prices[item.Key];
            }
            else
            {
                totalPrice += item.Value * prices[item.Key];
            }
        }

        return totalPrice;
    }
}