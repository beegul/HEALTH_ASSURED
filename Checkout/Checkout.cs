namespace Checkout;

public class Checkout(IReadOnlyDictionary<string, int> prices, IReadOnlyDictionary<string, List<SpecialPrice>> specialPrices) : ICheckout
{
    private readonly Dictionary<string, int> _scannedItems = new();

    /// <summary>
    /// Scans an item and adds it to the checkout.
    /// </summary>
    /// <param name="item">The SKU of the item to scan.</param>
    /// <exception cref="InvalidItemException">Thrown when the scanned item is not found in the price list.</exception>
    public void Scan(string item)
    {
        item = item.ToUpperInvariant(); 
        if (!prices.ContainsKey(item))
        {
            throw new InvalidItemException($"Invalid item: {item}");
        }

        //add the item to the scanned items and increment the count
        _scannedItems.TryAdd(item, 0);
        _scannedItems[item]++;
    }

    /// <summary>
    /// Calculates the total price of all scanned items, taking into account any applicable special offers.
    /// </summary>
    /// <returns>The total price.</returns>
    public int GetTotalPrice()
    {
        var totalPrice = 0;
        foreach (var item in _scannedItems)
        {
            if (specialPrices.TryGetValue(item.Key, out var specialPricesForItem))
            {
                //sort the offers by quantity and then by price
                var sortedOffers = specialPricesForItem
                    .OrderByDescending(offer => offer.Quantity)
                    .ThenBy(offer => offer.Price)
                    .ToList(); 

                var remainingItems = item.Value;

                //apply each offer in order
                foreach (var offer in sortedOffers)
                {
                    var offerCount = remainingItems / offer.Quantity;
                    totalPrice += offerCount * offer.Price;
                    remainingItems %= offer.Quantity; 
                }

                //add the price of the remaining items
                totalPrice += remainingItems * prices[item.Key]; 
            }
            else
            {
                //if there are no special prices for the item, just add the price of the item to the total
                totalPrice += item.Value * prices[item.Key];
            }
        }

        return totalPrice;
    }
}