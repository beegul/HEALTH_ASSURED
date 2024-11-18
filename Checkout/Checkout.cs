namespace Checkout;

public class Checkout(Dictionary<string, int> prices, Dictionary<string, SpecialPrice> specialOffers) : ICheckout
{
    private readonly Dictionary<string, int> scannedItems = new();

    public void Scan(string item)
    {
        item = item.ToUpperInvariant(); 
        if (!prices.ContainsKey(item))
        {
            throw new ArgumentException($"Invalid item: {item}");
        }

        scannedItems.TryAdd(item, 0);
        scannedItems[item]++;
    }

    public int GetTotalPrice()
    {
        var totalPrice = 0;
        foreach (var item in scannedItems)
        {
            if (specialOffers.ContainsKey(item.Key))
            {
                var offers = specialOffers
                    .Where(offer => offer.Key == item.Key)
                    .OrderByDescending(offer => offer.Value.Quantity)
                    .ToList(); 
                var remainingItems = item.Value;
                
                foreach (var offer in offers) 
                {
                    var offerCount = remainingItems / offer.Value.Quantity;
                    totalPrice += offerCount * offer.Value.Price;
                    remainingItems %= offer.Value.Quantity; 
                }
                
                totalPrice += remainingItems * prices[item.Key]; 
            }
            else
            {
                totalPrice += item.Value * prices[item.Key];
            }
        }

        return totalPrice;
    }
}