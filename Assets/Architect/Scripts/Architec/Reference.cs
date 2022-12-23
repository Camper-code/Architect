namespace Architect
{
    public struct Reference
    {
        public string fromModule;
        public string fromNode;
        public string toModule;
        public string toNode;

        public Reference(string fromNode, string toNode, string fromModule, string toModule)
        {
            this.fromNode = fromNode;
            this.toNode = toNode;
            this.fromModule = fromModule;
            this.toModule = toModule;
        }
    }
}
