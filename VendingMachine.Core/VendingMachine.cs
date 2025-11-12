namespace VM.Core
{
    public class VendingMachine
    {
        public VendingMachine()
        {
            DisplayMessage = DisplayProducts();
        }
        public List<Product> Products { get; } = new List<Product>()
        {
            new Product(1, "Cola", 105),
            new Product(2, "Crips", 50),
            new Product(3, "Chocalate", 65)
        };
        public List<Coins> ReturnedCoins { get; private set; }  = new List<Coins>();
        public int AmountInserted { get; private set; } = 0;
        public string DisplayMessage { get; private set; }

        private static readonly Coins[] ValidCoins = {
                Coins.TwoPounds,
                Coins.OnePound,
                Coins.FiftyPence,
                Coins.TwentyPence,
                Coins.TenPence,
                Coins.FivePence
        };
        private string DisplayProducts()
        {
            return string.Join(Environment.NewLine, Products.Select(p => p.ToString()));
        }
        private string DisplayAmount()
        {
            return AmountInserted == 0 ? "INSERT COIN" : $"Amount Inserted: {AmountInserted.ToCurrency()}";
        }
        private string DisplayReturned()
        {
            var amountReturned = ReturnedCoins.Sum(c => (int)c);
            return ReturnedCoins.Count == 0 ? "INSERT COIN" : $"Amount Returned: {amountReturned.ToCurrency()}";
        }

        /// <summary>
        /// Accepts a coin and updates the total amount inserted if the coin is valid.
        /// </summary>
        /// <remarks>If the coin is not valid, it is added to the collection of returned coins. The
        /// display message is updated to reflect the current amount inserted.</remarks>
        /// <param name="coin">The coin to be accepted, represented by the <see cref="Coins"/> enumeration.</param>
        public void AcceptCoin(Coins coin)
        {
            if (ValidCoins.Contains(coin))
                AmountInserted += (int)coin;
            else
                ReturnedCoins.Add(coin);

            DisplayMessage = DisplayAmount();
        }        

        /// <summary>
        /// Selects a product based on the specified product ID and processes the transaction.
        /// </summary>
        /// <remarks>If the product with the specified ID is not found, the display message is set to
        /// "INVALID SELECTION". If the inserted amount is sufficient to cover the product's price, the amount is
        /// deducted and the display message is set to "THANK YOU". Otherwise, the display message indicates the
        /// product's price and the additional amount required.</remarks>
        /// <param name="productId">The unique identifier of the product to be selected.</param>
        public void SelectProduct(int productId)
        {
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                DisplayMessage = "INVALID SELECTION";
                return;
            }

            if (AmountInserted >= product.Price)
            {
                AmountInserted -= product.Price;
                DisplayMessage = "THANK YOU";
            }
            else
            {
                var shortfall = product.Price - AmountInserted;
                DisplayMessage = $"PRICE: {product.Price.ToCurrency()}. Please insert additional {shortfall.ToCurrency()}";
            }
        }

        /// <summary>
        /// Dispenses change by returning the largest possible denominations of valid coins.
        /// </summary>
        /// <remarks>This method iterates over a predefined list of valid coin denominations and subtracts
        /// their values from the total amount inserted until the amount is less than the coin's value. The returned
        /// coins are added to a collection, and the total amount inserted is reset to zero.</remarks>
        public void MakeChange()
        {
            foreach (var coin in ValidCoins)
            {
                while (AmountInserted >= (int)coin)
                {
                    ReturnedCoins.Add(coin);
                    AmountInserted -= (int)coin;
                }
            }
            AmountInserted = 0;
            DisplayMessage = DisplayReturned();
        }
    }
}
