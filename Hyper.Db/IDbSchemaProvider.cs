namespace Hyper.Db
{
    public interface IDbSchemaProvider
    {
        IDbSchemaCollection GetDbSchemas();
    }
}
