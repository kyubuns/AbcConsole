using System;

namespace AbcConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AbcCommandAttribute : Attribute
    {
        public string Summary { get; }

        public AbcCommandAttribute()
        {
            Summary = string.Empty;
        }

        public AbcCommandAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
