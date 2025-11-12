using VM.Core;

namespace VM.UnitTest
{
    public class VendingMachineTests
    {
        private readonly VendingMachine _vm = new VendingMachine();

        [Fact]
        public void InitialState()
        {
            Assert.Equal(0, _vm.AmountInserted);
            Assert.Empty(_vm.ReturnedCoins);
            Assert.NotEqual("INSERT COIN", _vm.DisplayMessage);
        }

        [Theory]
        [InlineData(Coins.OnePenny)]
        [InlineData(Coins.TwoPence)]
        public void RejectCoin(Coins coin)
        {
            _vm.AcceptCoin(coin);
            
            Assert.Equal(0, _vm.AmountInserted);
            Assert.Contains(coin, _vm.ReturnedCoins);
            Assert.Equal("INSERT COIN", _vm.DisplayMessage);
        }

        [Theory]
        [InlineData(Coins.FivePence)]
        [InlineData(Coins.TenPence)]
        [InlineData(Coins.TwentyPence)]
        [InlineData(Coins.FiftyPence)]
        [InlineData(Coins.OnePound)]
        [InlineData(Coins.TwoPounds)]
        public void AcceptCoin(Coins coin)
        {
            _vm.AcceptCoin(coin);

            Assert.Equal((int)coin, _vm.AmountInserted);
            Assert.Empty(_vm.ReturnedCoins);
            Assert.Equal($"Amount Inserted: {_vm.AmountInserted.ToCurrency()}", _vm.DisplayMessage);

        }

        [Theory]
        [InlineData(new Coins[] { Coins.FivePence, Coins.TenPence, Coins.TwentyPence, Coins.FiftyPence })]
        [InlineData(new Coins[] { Coins.TwentyPence, Coins.FiftyPence, Coins.OnePound, Coins.TwoPounds })]
        public void AcceptedCoinTotal(Coins[] coins)
        {
            var total = 0;
            foreach (var coin in coins)
            {
                _vm.AcceptCoin(coin);
                total += (int)coin;
            }

            Assert.Equal(total, _vm.AmountInserted);
            Assert.Empty(_vm.ReturnedCoins);
            Assert.Equal($"Amount Inserted: {total.ToCurrency()}", _vm.DisplayMessage);

        }

        [Theory]
        [InlineData(new Coins[] { Coins.FivePence, Coins.TenPence, Coins.TwentyPence, Coins.FiftyPence }, Coins.OnePenny)]
        [InlineData(new Coins[] { Coins.TwentyPence, Coins.FiftyPence, Coins.OnePound, Coins.TwoPounds }, Coins.TwoPence)]
        public void AcceptAndRejectCoinTotal(Coins[] coins, Coins notValidCoin)
        {
            var total = 0;
            foreach (var coin in coins)
            {
                _vm.AcceptCoin(coin);
                total += (int)coin;
            }
            _vm.AcceptCoin(notValidCoin);
            Assert.Equal(total, _vm.AmountInserted);
            Assert.Contains(notValidCoin, _vm.ReturnedCoins);
            Assert.Equal($"Amount Inserted: {total.ToCurrency()}", _vm.DisplayMessage);

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5)]
        public void InvalidProduct(int productId)
        {
            _vm.SelectProduct(productId);
            Assert.Equal("INVALID SELECTION", _vm.DisplayMessage);
        }

        [Theory]
        [InlineData(new Coins[] { }, -1, "INVALID SELECTION")]
        [InlineData(new Coins[] { }, 0, "INVALID SELECTION")]
        [InlineData(new Coins[] { }, 5, "INVALID SELECTION")]
        [InlineData(new Coins[] { Coins.OnePound, Coins.FivePence }, 1, "THANK YOU")]
        [InlineData(new Coins[] { Coins.FiftyPence }, 2, "THANK YOU")]
        [InlineData(new Coins[] { Coins.FiftyPence, Coins.TenPence, Coins.FivePence }, 3, "THANK YOU")]
        [InlineData(new Coins[] { Coins.OnePound }, 1, "PRICE: £1.05. Please insert additional £0.05")]
        [InlineData(new Coins[] { Coins.TwentyPence, Coins.TenPence }, 2, "PRICE: £0.50. Please insert additional £0.20")]
        [InlineData(new Coins[] { Coins.TenPence, Coins.FivePence }, 3, "PRICE: £0.65. Please insert additional £0.50")]
        public void ProductSelectionResults(Coins[] coins, int productId, string expectedMessage)
        {
            foreach (var coin in coins)
            {
                _vm.AcceptCoin(coin);
            }
            _vm.SelectProduct(productId);
            Assert.Equal(expectedMessage, _vm.DisplayMessage);
        }

        [Theory]
        [InlineData(new Coins[] { }, "INSERT COIN",0)]   
        [InlineData(new Coins[] { Coins.OnePound, Coins.FivePence }, "Amount Returned: £1.05",2)]
        [InlineData(new Coins[] { Coins.TwentyPence, Coins.TenPence }, "Amount Returned: £0.30",2)]
        [InlineData(new Coins[] { Coins.TenPence, Coins.FivePence }, "Amount Returned: £0.15",2)]
        public void MakeChangeWithoutProductResults(Coins[] coins, string expectedMessage, int coinsReturnedCount)
        {
            foreach (var coin in coins)
            {
                _vm.AcceptCoin(coin);
            }
            _vm.MakeChange();
            Assert.Equal(expectedMessage, _vm.DisplayMessage);
            Assert.Equal(coinsReturnedCount, _vm.ReturnedCoins.Count);
        }

        [Theory]
        [InlineData(new Coins[] { Coins.OnePound, Coins.FivePence }, 1, "INSERT COIN",0)]
        [InlineData(new Coins[] { Coins.FiftyPence }, 2, "INSERT COIN",0)]
        [InlineData(new Coins[] { Coins.FiftyPence, Coins.TenPence, Coins.FivePence }, 3, "INSERT COIN",0)]
        [InlineData(new Coins[] { Coins.OnePound, Coins.FivePence, Coins.TwoPence, Coins.OnePenny }, 1, "Amount Returned: £0.03", 2)]
        [InlineData(new Coins[] { Coins.OnePound, Coins.TenPence }, 1, "Amount Returned: £0.05",1)]
        [InlineData(new Coins[] { Coins.TwentyPence, Coins.FiftyPence }, 2, "Amount Returned: £0.20",1)]
        [InlineData(new Coins[] { Coins.FiftyPence, Coins.FiftyPence }, 3, "Amount Returned: £0.35",3)]
        public void PurchaseProductMakeChangeResults(Coins[] coins, int productId, string expectedMessage, int coinsReturnedCount)
        {
            foreach (var coin in coins)
            {
                _vm.AcceptCoin(coin);
            }
            _vm.SelectProduct(productId);
            _vm.MakeChange();
            Assert.Equal(expectedMessage, _vm.DisplayMessage);
            Assert.Equal(coinsReturnedCount, _vm.ReturnedCoins.Count);
        }
    }
}
