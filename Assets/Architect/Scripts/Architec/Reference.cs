namespace Architect
{
    public struct Reference
    {
        public string fromModule;
        public string from;
        public string toModule;
        public string to;

        public Reference(string from, string to, string fromModule, string toModule)
        {
            this.from = from;
            this.to = to;
            this.fromModule = fromModule;
            this.toModule = toModule;
        }
    }
}
