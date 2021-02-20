using System;
using UnityEngine.Scripting;

namespace AbcConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AbcCommandAttribute : PreserveAttribute
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
