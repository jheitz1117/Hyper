namespace Hyper.Db.Model
{
    public class DbColumn : IDbColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int? MaxLength { get; set; }
        public int? Decimals { get; set; }
        public virtual bool? IsNullable { get; set; }
        public string DefaultValue { get; set; }
    }
}
