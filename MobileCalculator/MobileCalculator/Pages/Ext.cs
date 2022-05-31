using System.Linq;

namespace MobileCalculator.Pages
{
    public static class Ext
    {
        public static bool StartsWithArray(this string toCheck, char[] chars)
        {
            return toCheck != null
                       && toCheck.Length > 0
                       && chars.Any(c => c == toCheck[0]);
        }
    }
}