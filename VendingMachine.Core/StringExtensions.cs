namespace VM.Core
{
    public static class StringExtensions
    {
        public static string ToCurrency(this int pennies)
        {
            return String.Format("{0:c}", pennies / 100m);
        }
    }
}
