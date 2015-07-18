namespace Hyper.Db.Xml.ScriptSources
{
    internal class HyperDbColumn : IDbColumnScriptSource
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int? MaxLength { get; set; }
        public int? Decimals { get; set; }
        public virtual bool? IsNullable { get; set; }
        public string DefaultValue { get; set; }
    }
}
