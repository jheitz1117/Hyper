namespace Hyper.Db.Xml.ScriptSources
{
    internal class HyperDbParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int? MaxLength { get; set; }
        public int? Decimals { get; set; }
        public object DefaultValue { get; set; }
    }
}
