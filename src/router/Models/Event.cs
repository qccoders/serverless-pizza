namespace ServerlessPizzaAPI.Models
{
    using System;

    public class Event
    {
        public Event(EventType type, DateTime start, DateTime? end)
        {
            Type = type;
            Start = start;
            End = end;
        }

        public EventType Type { get; }
        public DateTime Start { get; }
        public DateTime? End { get; }
    }
}
