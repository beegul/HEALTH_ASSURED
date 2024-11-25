﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xunit;

namespace Checkout.Tests;

[TestSubject(typeof(Checkout))]
public class CheckoutTests
{
    private readonly Dictionary<string, int> _prices = new()
    {
        { "A", 50 },
        { "B", 30 },
        { "C", 20 },
        { "D", 15 }
    };
    private readonly Dictionary<string, List<SpecialPrice>> _specialPrices = new()
    {
        { "A", [new SpecialPrice(3, 130)] },
        { "B", [new SpecialPrice(2, 45)] }
    };

    [Fact]
    public void EmptyBasketReturnsZero()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        Assert.Equal(0, checkout.GetTotalPrice());
    }

    [Fact]
    public void SingleItemReturnsCorrectPrice()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("A");
        Assert.Equal(50, checkout.GetTotalPrice());
    }

    [Fact]
    public void TwoDifferentItemsReturnCorrectPrice()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("A");
        checkout.Scan("B");
        Assert.Equal(80, checkout.GetTotalPrice());
    }

    [Fact]
    public void MultipleItemsWithSpecialPriceReturnCorrectPrice()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        Assert.Equal(130, checkout.GetTotalPrice());
    }

    [Fact]
    public void MultipleItemsWithAndWithoutSpecialPriceReturnCorrectPrice()
    {
        var checkout = new Checkout(_prices, _specialPrices);
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
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("B");
        checkout.Scan("A");
        checkout.Scan("B");
        Assert.Equal(95, checkout.GetTotalPrice());
    }
    
    [Fact]
    public void InvalidItemThrowsException()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        Assert.Throws<InvalidItemException>(() => checkout.Scan("E"));
    }
    
    [Fact]
    public void CaseInsensitiveItemScan()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("a");
        Assert.Equal(50, checkout.GetTotalPrice());
    }
    
    [Fact]
    public void MultipleSpecialOffersForSameItem()
    {
        _specialPrices["A"] = new List<SpecialPrice>
        {
            new(3, 130),
            new(6, 250)
        };
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        
        Assert.Equal(250, checkout.GetTotalPrice());
    }
    
    [Fact]
    public void SpecialOfferWithZeroQuantityThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SpecialPrice(0, 100));
    }

    [Fact]
    public void SpecialOfferWithNegativeQuantityThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SpecialPrice(-1, 100));
    }

    [Fact]
    public void SpecialOfferWithZeroPriceThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SpecialPrice(2, 0));
    }

    [Fact]
    public void SpecialOfferWithNegativePriceThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new SpecialPrice(2, -10));
    }

    [Fact]
    public void LargeQuantityOfItemsWithSpecialOffer()
    {
        var checkout = new Checkout(_prices, _specialPrices);
        for (var i = 0; i < 100; i++)
        {
            checkout.Scan("A"); 
        }
        Assert.Equal(4340, checkout.GetTotalPrice());
    }

    [Fact]
    public void SameQuantitySpecialOffersForSameItem()
    {
        _specialPrices["A"].Add(new SpecialPrice(3, 120)); 
        _specialPrices["A"].Add(new SpecialPrice(3, 130));
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A");

        Assert.Equal(170, checkout.GetTotalPrice());
    }

    [Fact]
    public void SpecialOfferSlightlyBetterThanUnitPrice()
    {
        _specialPrices["B"].Add(new SpecialPrice(2, 39));
        var checkout = new Checkout(_prices, _specialPrices);
        checkout.Scan("B");
        checkout.Scan("B");
        checkout.Scan("B");
        checkout.Scan("B");

        Assert.Equal(78, checkout.GetTotalPrice());
    }
}