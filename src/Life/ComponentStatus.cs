using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lyfe
{
    public struct ComponentStatus
    {
        static readonly IReadOnlyDictionary<string, object> _emptyDetails = new Dictionary<string, object>();
        public string Component { get; }
        public string Status { get; }
        public IReadOnlyDictionary<string, object> Details { get; }

        public ComponentStatus(string component, string status)
            : this(component, status, _emptyDetails) { }

        [JsonConstructor]
        public ComponentStatus(string component, string status, IReadOnlyDictionary<string, object> details)
        {
            Component = component;
            Status = status;
            Details = details;
        }

        public static ComponentStatus Up(string component) =>
            Up(component, _emptyDetails);
        public static ComponentStatus Up(string component, IReadOnlyDictionary<string, object> details) =>
            new ComponentStatus(component, "Up", details);
        public static ComponentStatus Down(string component) =>
            Down(component, _emptyDetails);
        public static ComponentStatus Down(string component, IReadOnlyDictionary<string, object> details) =>
            new ComponentStatus(component, "Down", details);
    }
}
