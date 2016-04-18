namespace Hyper.Db.ScriptWriters.Sql
{
    public static class SqlDefaults
    {
        // TODO: This default schema name is used everywhere, which is a problem (see comments below).
        /*
         * We need to figure out how we can give schema names to DB objects without forcing it on every single DB provider.
         * Maybe have another layer of ISqlTableScriptSource that has a Schema property? Then we could just check if the
         * object specified to the script writer implements the interface and use that schema by default, or use the default
         * if that schema is blank.
         * 
         * This extra interface layer would shield the other DB types, which would inherit directly from the higher-level
         * interfaces instead of the Sql-specific interfaces.
         */
        public static string DefaultSchemaName = "dbo";
    }
}
