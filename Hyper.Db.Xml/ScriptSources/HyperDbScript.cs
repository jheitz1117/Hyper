using System.IO;

namespace Hyper.Db.Xml.ScriptSources
{
    internal class HyperDbScript : IDbScriptWriter
    {
        public string Name { get; set; }
        public string Body { get; set; }

        public void WriteDbScript(TextWriter writer)
        {
            writer.WriteLine(this.Body);
        }
    }
}
