namespace Hyper.Db.Model
{
    public class DbColumn : IDbColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int? MaxLength { get; set; }
        public int? Decimals { get; set; }
        public bool? IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public bool? IsIdentity { get; set; }
        public long? Seed { get; set; }
        public int? Increment { get; set; }
    }
}
