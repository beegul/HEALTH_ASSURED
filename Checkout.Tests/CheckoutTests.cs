using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;

namespace Checkout.Tests;

[TestSubject(typeof(Checkout))]
public class CheckoutTests
{
    private readonly Dictionary<string, int> prices;
    private readonly Dictionary<string, SpecialPrice> specialPrices;

    public CheckoutTests()
    {
        prices = new Dictionary<string, int>()
        {
            { "A", 50 },
            { "B", 30 },
            { "C", 20 },
            { "D", 15 }
        };

        specialPrices = new Dictionary<string, SpecialPrice>()
        {
            { "A", new SpecialPrice(3, 130) },
            { "B", new SpecialPrice(2, 45) }
        };
    }

    [Fact]
    public void EmptyBasketReturnsZero()
    {
        var checkout = new Checkout(prices, specialPrices);
        Assert.Equal(0, checkout.GetTotalPrice());
    }

    [Fact]
    public void SingleItemReturnsCorrectPrice()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("A");
        Assert.Equal(50, checkout.GetTotalPrice());
    }

    [Fact]
    public void TwoDifferentItemsReturnCorrectPrice()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("A");
        checkout.Scan("B");
        Assert.Equal(80, checkout.GetTotalPrice());
    }

    [Fact]
    public void MultipleItemsWithSpecialPriceReturnCorrectPrice()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        Assert.Equal(130, checkout.GetTotalPrice());
    }

    [Fact]
    public void MultipleItemsWithAndWithoutSpecialPriceReturnCorrectPrice()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("B");
        checkout.Scan("B");
        Assert.Equal(175, checkout.GetTotalPrice());
    }

    [Fact]
    public void ItemsInAnyOrderWithSpecialPriceReturnCorrectPrice()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("B");
        checkout.Scan("A");
        checkout.Scan("B");
        Assert.Equal(95, checkout.GetTotalPrice());
    }
    
    [Fact]
    public void InvalidItemThrowsException()
    {
        var checkout = new Checkout(prices, specialPrices);
        Assert.Throws<ArgumentException>(() => checkout.Scan("E"));
    }
    
    [Fact]
    public void CaseInsensitiveItemScan()
    {
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("a");
        Assert.Equal(50, checkout.GetTotalPrice());
    }
    
    [Fact]
    public void MultipleSpecialOffersForSameItem()
    {
        specialPrices.Add("A", new SpecialPrice(5, 200));
        var checkout = new Checkout(prices, specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        
        Assert.Equal(250, checkout.GetTotalPrice());
    }
}