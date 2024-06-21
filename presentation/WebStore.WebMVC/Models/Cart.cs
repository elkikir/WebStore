using Microsoft.CodeAnalysis.CSharp;

namespace WebStore.WebMVC.Models
{
    public class Cart
    {
        public IDictionary<int, int> Items { get; set; } = new Dictionary<int, int>();
        public int Count()
        {
            int temp = 0;

            foreach(var item in Items)
            {
                temp += item.Value;
            }

            return temp;
        }
        public decimal Amount { get; set; }
    }
}
