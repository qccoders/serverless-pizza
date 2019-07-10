namespace ServerlessPizzaAPI.Models
{
    using System;
    using System.Collections.Generic;

    public class Order
    {
        public Order(string id, string name, DateTime placed, IEnumerable<Event> events, Details details)
        {
            Id = id;
            Name = name;
            Placed = placed;
            Events = events;
            Details = details;
        }

        public string Id { get; }
        public string Name { get; }
        public DateTime Placed { get; }
        public IEnumerable<Event> Events { get; }
        public Details Details { get; }
    }
}
