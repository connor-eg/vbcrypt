namespace vbcrypt
{
    internal class Argument
    {
        private readonly List<string> Values;
        public int Count { get { return Values.Count; } }

        public Argument() { 
            Values = new List<string>();
        }

        public string GetValue() { return Values[0]; }

        public string[] GetValues() { return Values.ToArray(); }

        public Argument Add(string value) { Values.Add(value); return this; }

    }
}
