using VM.Core;

namespace VM.UnitTest
{
    public class AdditionalVendingMachineTests
    {
        [Fact]
        public void Constructor_SetsDisplayMessage_ToProductList()
        {
            var vm = new VendingMachine();
            var expected = string.Join(Environment.NewLine, vm.Products.Select(p => p.ToString()));
            Assert.Equal(expected, vm.DisplayMessage);
        }

        [Fact]
        public void Purchase_WithExactChange_LeavesAmountZero_AndNoReturnedCoins()
        {
            var vm = new VendingMachine();
            vm.AcceptCoin(Coins.FiftyPence); // Crips = 50
            vm.SelectProduct(2);
            Assert.Equal(0, vm.AmountInserted);
            Assert.Empty(vm.ReturnedCoins);
            Assert.Equal("THANK YOU", vm.DisplayMessage);
        }

        [Fact]
        public void InvalidSelection_DoesNotModifyAmountInserted()
        {
            var vm = new VendingMachine();
            vm.AcceptCoin(Coins.TwentyPence);
            var before = vm.AmountInserted;
            vm.SelectProduct(-1);
            Assert.Equal(before, vm.AmountInserted);
            Assert.Equal("INVALID SELECTION", vm.DisplayMessage);
        }

        [Fact]
        public void MakeChange_ReturnsExpectedDenominations_InOrder()
        {
            var vm = new VendingMachine();
            vm.AcceptCoin(Coins.TwoPounds); // 200
            vm.SelectProduct(1); // Cola = 105 => remaining 95
            vm.MakeChange();

            var expected = new[] { Coins.FiftyPence, Coins.TwentyPence, Coins.TwentyPence, Coins.FivePence };
            Assert.Equal(expected, vm.ReturnedCoins);
            Assert.Equal($"Amount Returned: {vm.ReturnedCoins.Sum(c => (int)c).ToCurrency()}", vm.DisplayMessage);
            Assert.Equal(0, vm.AmountInserted);
        }

        [Fact]
        public void RejectedCoins_ArePreserved_WhenMakingChange()
        {
            var vm = new VendingMachine();
            vm.AcceptCoin(Coins.OnePenny);   // rejected => returnedCoins contains OnePenny
            vm.AcceptCoin(Coins.TwoPounds);  // inserted 200
            vm.SelectProduct(1);             // buy Cola 105 => remaining 95
            vm.MakeChange();

            var expected = new[] { Coins.OnePenny, Coins.FiftyPence, Coins.TwentyPence, Coins.TwentyPence, Coins.FivePence };
            Assert.Equal(expected, vm.ReturnedCoins);
        }
    }
}