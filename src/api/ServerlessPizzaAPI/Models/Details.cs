namespace ServerlessPizzaAPI.Models
{
    using System.Collections.Generic;

    public class Details
    {
        public Details(string size, string type, int cookTime, IEnumerable<string> toppings)
        {
            Size = size;
            Type = type;
            CookTime = cookTime;
            Toppings = toppings;
        }

        public string Size { get; }
        public string Type { get; }
        public int CookTime { get; }
        public IEnumerable<string> Toppings { get; }
    }
}
