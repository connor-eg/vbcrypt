using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vbcrypt
{
    internal class Argument
    {
        private List<string> Values;

        public Argument() { 
            Values = new List<string>();
        }

        public int Count() { return Values.Count; }

        public string GetValue() { return Values[0]; }

        public string[] GetValues() { return Values.ToArray(); }

        public Argument Add(string value) { Values.Add(value); return this; }

    }
}
