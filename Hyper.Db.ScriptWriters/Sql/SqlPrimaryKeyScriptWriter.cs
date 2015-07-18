using System.IO;
using System.Linq;
using System.Text;

namespace Hyper.Db.ScriptWriters.Sql
{
    public class SqlPrimaryKeyScriptWriter : SqlScriptWriter, IDbPrimaryKeyScriptWriter
    {
        public IDbPrimaryKeyScriptSource Source { get; set; }

        public override void WriteDbScript(TextWriter writer)
        {
            writer.WriteLine("set nocount on");
            writer.WriteLine();
            writer.WriteLine("declare @XmlPKColumns table (");
            writer.WriteLine("    [OrdinalPosition] [int]           null,");
            writer.WriteLine("    [ColumnName]      [nvarchar](128) null");
            writer.WriteLine(")");
            writer.WriteLine("insert into @XmlPKColumns values");
            writer.WriteLine(
                string.Join(
                    "," + writer.NewLine,
                    this.Source.KeyColumns.Zip(
                        Enumerable.Range(1, this.Source.KeyColumns.Count()),
                        (s, i) => string.Format("({0}, '{1}')", i, s)
                    )
                )
            );
            writer.WriteLine();
            writer.WriteLine("declare @DbPKColumnResults table (");
            writer.WriteLine("    [OrdinalPosition] [int]           null,");
            writer.WriteLine("    [ColumnName]      [nvarchar](128) null,");
            writer.WriteLine("    [PrimaryKeyName]  [nvarchar](128) null");
            writer.WriteLine(")");
            writer.WriteLine("insert into @DbPKColumnResults");
            writer.WriteLine("select");
            writer.WriteLine("    [xpk].[OrdinalPosition],");
            writer.WriteLine("    [xpk].[ColumnName],");
            writer.WriteLine("    [kcu].[CONSTRAINT_NAME]");
            writer.WriteLine("from [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] [kcu]");
            writer.WriteLine("    inner join [INFORMATION_SCHEMA].[TABLE_CONSTRAINTS] [tc] on");
            writer.WriteLine("            [tc].[CONSTRAINT_CATALOG] = [kcu].[CONSTRAINT_CATALOG]");
            writer.WriteLine("        and [tc].[CONSTRAINT_SCHEMA]  = [kcu].[CONSTRAINT_SCHEMA]");
            writer.WriteLine("        and [tc].[CONSTRAINT_NAME]    = [kcu].[CONSTRAINT_NAME]");
            writer.WriteLine("        and [tc].[CONSTRAINT_TYPE]    = '{0}'", SqlConstraintTypes.PrimaryKey);
            writer.WriteLine("        and [tc].[TABLE_SCHEMA]       = '{0}'", this.DefaultSchemaName);
            writer.WriteLine("        and [tc].[TABLE_NAME]         = '{0}'", this.Source.TableSource.TableName);
            writer.WriteLine("    left join @XmlPKColumns [xpk] on");
            writer.WriteLine("            [xpk].[ColumnName]        = [kcu].[COLUMN_NAME]");
            writer.WriteLine("        and [xpk].[OrdinalPosition]   = [kcu].[ORDINAL_POSITION]");
            writer.WriteLine();
            writer.WriteLine("declare @ExistingPKColumnCount [int]");
            writer.WriteLine("declare @ExistingPKName        [nvarchar](128)");
            writer.WriteLine("select");
            writer.WriteLine("    @ExistingPKColumnCount = COUNT(*),");
            writer.WriteLine("    @ExistingPKName        = [PrimaryKeyName]");
            writer.WriteLine("from @DbPKColumnResults");
            writer.WriteLine("group by [PrimaryKeyName]");
            writer.WriteLine();
            writer.WriteLine("if {0}", GetNoPrimaryKeyExistsCondition());
            writer.WriteLine("begin");
            writer.WriteLine("    {0}", GetCreatePrimaryKeyScript()); // In this case, no key exists, so we can create a new one
            writer.WriteLine("end");
            writer.WriteLine("else");
            writer.WriteLine("begin");
            writer.WriteLine("    if {0}", GetPrimaryKeyChangeCondition());
            writer.WriteLine("    begin");
            writer.WriteLine("        begin tran"); // We want to surround the next two commands in a transaction envelope in case the add fails in which case we'll need to roll back the drop.
            writer.WriteLine("            {0}", GetDropPrimaryKeyScript());
            writer.WriteLine("            {0}", GetCreatePrimaryKeyScript());
            writer.WriteLine("        commit tran");
            writer.WriteLine("    end");
            writer.WriteLine("end");
        }

        private string GetPrimaryKeyDefinition()
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(this.Source.PrimaryKeyName))
                builder.AppendFormat("constraint [{0}] ", this.Source.PrimaryKeyName);

            builder.Append("primary key (");
            builder.Append(
                string.Join(
                    ",",
                    this.Source.KeyColumns.Select(k =>
                        string.Format("[{0}]", k)
                    )
                )
            );
            builder.Append(")");

            return builder.ToString();
        }

        private string GetCreatePrimaryKeyScript()
        {
            return string.Format("alter table [{0}].[{1}] add {2}",
                this.DefaultSchemaName,
                this.Source.TableSource.TableName,
                GetPrimaryKeyDefinition()
            );
        }

        private string GetDropPrimaryKeyScript()
        {
            // Have to use exec because we're dynamically grabbing the name of the existing primary key
            return string.Format("exec('alter table [{0}].[{1}] drop constraint [' + @ExistingPKName + ']')",
                this.DefaultSchemaName,
                this.Source.TableSource.TableName
            );
        }

        private static string GetNoPrimaryKeyExistsCondition()
        {
            return "@ExistingPKColumnCount is null";
        }

        private string GetPrimaryKeyChangeCondition()
        {
            return GetColumnCountChangeCondition() + " or " + GetColumnNameChangeCondition();
        }

        private string GetColumnCountChangeCondition()
        {
            return string.Format("(@ExistingPKColumnCount <> {0})", this.Source.KeyColumns.Count());
        }

        private static string GetColumnNameChangeCondition()
        {
            return "exists(select * from @DbPKColumnResults where [OrdinalPosition] is null or [ColumnName] is null)";
        }
    }
}
