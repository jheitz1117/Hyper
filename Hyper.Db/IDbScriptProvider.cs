using System;

namespace Hyper.Db
{
    public interface IDbScriptProvider
    {
        void ExecuteAllScripts(string dbSchemaName, Action<string> executeDelegate);
    }
}
