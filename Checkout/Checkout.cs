namespace Checkout;

public class Checkout(Dictionary<string, int> prices, Dictionary<string, List<SpecialPrice>> specialPrices) : ICheckout
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
            if (specialPrices.TryGetValue(item.Key, out var price))
            {
                var offers = price
                    .OrderByDescending(offer => offer.Quantity)
                    .ThenBy(offer => offer.Price)
                    .ToList(); 
                var remainingItems = item.Value;
                
                foreach (var offer in offers)
                {
                    var offerCount = remainingItems / offer.Quantity;
                    totalPrice += offerCount * offer.Price;
                    remainingItems %= offer.Quantity; 
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