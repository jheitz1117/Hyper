namespace Hyper.Db.Xml.ScriptSources
{
    // TODO: When implementing the identity column, need to make sure to only account for it during initial table creation. All subsequent updates should ignore these types of columns.
    internal class HyperDbIdentityColumn : HyperDbColumn
    {
        public int Seed { get; set; }
        public int Increment { get; set; }
        public override bool? IsNullable
        {
            get { return false; }
            set { /* Identity Column can't be null, so override setter to do nothing */ }
        }
    }
}
