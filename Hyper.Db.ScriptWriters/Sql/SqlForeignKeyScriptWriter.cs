using System.IO;
using System.Linq;
using System.Text;

namespace Hyper.Db.ScriptWriters.Sql
{
    public class SqlForeignKeyScriptWriter : SqlScriptWriter, IDbForeignKeyScriptWriter
    {
        public IDbForeignKeyScriptSource Source { get; set; }

        public override void WriteDbScript(TextWriter writer)
        {
            writer.WriteLine("set nocount on");
            writer.WriteLine();
            writer.WriteLine("declare @XmlFKColumns table (");
            writer.WriteLine("    [OrdinalPosition]      [int]           null,");
            writer.WriteLine("    [ColumnName]           [nvarchar](128) null,");
            writer.WriteLine("    [ReferencedTableName]  [nvarchar](128) null,");
            writer.WriteLine("    [ReferencedColumnName] [nvarchar](128) null");
            writer.WriteLine(")");
            writer.WriteLine("insert into @XmlFKColumns values");
            writer.WriteLine(
                string.Join(
                    "," + writer.NewLine,
                    this.Source.KeyColumns.Zip(
                        Enumerable.Range(1, this.Source.KeyColumns.Count()),
                        (s, i) => string.Format("({0}, '{1}', '{2}', '{3}')",
                            i,
                            s.ForeignKeyColumnName,
                            this.Source.ReferencedTableName,
                            s.ReferencedColumnName)
                    )
                )
            );
            writer.WriteLine();
            writer.WriteLine("declare @DbFKColumnResults table (");
            writer.WriteLine("    [OrdinalPosition]      [int]           null,");
            writer.WriteLine("    [ColumnName]           [nvarchar](128) null,");
            writer.WriteLine("    [ReferencedTableName]  [nvarchar](128) null,");
            writer.WriteLine("    [ReferencedColumnName] [nvarchar](128) null");
            writer.WriteLine(")");
            writer.WriteLine("insert into @DbFKColumnResults");
            writer.WriteLine("select");
            writer.WriteLine("    [xfk].[OrdinalPosition],");
            writer.WriteLine("    [xfk].[ColumnName],");
            writer.WriteLine("    [xfk].[ReferencedTableName],");
            writer.WriteLine("    [xfk].[ReferencedColumnName]");
            writer.WriteLine("from [INFORMATION_SCHEMA].[REFERENTIAL_CONSTRAINTS] [rc]");
            writer.WriteLine("    inner join [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] [kcu] on");
            writer.WriteLine("            [kcu].[CONSTRAINT_CATALOG]    = [rc].[CONSTRAINT_CATALOG]");
            writer.WriteLine("        and [kcu].[CONSTRAINT_SCHEMA]     = [rc].[CONSTRAINT_SCHEMA]");
            writer.WriteLine("        and [kcu].[CONSTRAINT_NAME]       = [rc].[CONSTRAINT_NAME]");
            writer.WriteLine("    inner join [INFORMATION_SCHEMA].[KEY_COLUMN_USAGE] [kcuRef] on");
            writer.WriteLine("            [kcuRef].[CONSTRAINT_CATALOG] = [rc].[CONSTRAINT_CATALOG]");
            writer.WriteLine("        and [kcuRef].[CONSTRAINT_SCHEMA]  = [rc].[CONSTRAINT_SCHEMA]");
            writer.WriteLine("        and [kcuRef].[CONSTRAINT_NAME]    = [rc].[CONSTRAINT_NAME]");
            writer.WriteLine("        and [kcuRef].[ORDINAL_POSITION]   = [kcu].[ORDINAL_POSITION]");
            writer.WriteLine("    inner join [INFORMATION_SCHEMA].[TABLE_CONSTRAINTS] [tc] on");
            writer.WriteLine("            [tc].[CONSTRAINT_CATALOG]     = [rc].[CONSTRAINT_CATALOG]");
            writer.WriteLine("        and [tc].[CONSTRAINT_SCHEMA]      = [rc].[CONSTRAINT_SCHEMA]");
            writer.WriteLine("        and [tc].[CONSTRAINT_NAME]        = [rc].[CONSTRAINT_NAME]");
            writer.WriteLine("        and [tc].[CONSTRAINT_TYPE]        = '{0}'", SqlConstraintTypes.ForeignKey);
            writer.WriteLine("        and [tc].[TABLE_SCHEMA]           = '{0}'", this.DefaultSchemaName);
            writer.WriteLine("        and [tc].[TABLE_NAME]             = '{0}'", this.Source.TableSource.TableName);
            writer.WriteLine("        and [tc].[CONSTRAINT_NAME]        = '{0}'", this.Source.ForeignKeyName);
            writer.WriteLine("    left join @XmlFKColumns [xfk] on");
            writer.WriteLine("            [xfk].[ColumnName]            = [kcu].[COLUMN_NAME]");
            writer.WriteLine("        and [xfk].[OrdinalPosition]       = [kcu].[ORDINAL_POSITION]");
            writer.WriteLine("        and [xfk].[ReferencedTableName]   = [kcuRef].[TABLE_NAME]");
            writer.WriteLine("        and [xfk].[ReferencedColumnName]  = [kcuRef].[COLUMN_NAME]");
            writer.WriteLine();
            writer.WriteLine("declare @ExistingFKColumnCount [int]");
            writer.WriteLine("select @ExistingFKColumnCount = COUNT(*) from @DbFKColumnResults");
            writer.WriteLine();
            writer.WriteLine("if {0}", GetNoMatchingForeignKeyExistsCondition());
            writer.WriteLine("begin");
            writer.WriteLine("    {0}", GetCreateForeignKeyScript()); // In this case, no key exists, so we can create a new one
            writer.WriteLine("end");
            writer.WriteLine("else");
            writer.WriteLine("begin");
            writer.WriteLine("    if {0}", GetForeignKeyChangeCondition());
            writer.WriteLine("    begin");
            writer.WriteLine("        begin tran"); // We want to surround the next two commands in a transaction envelope in case the add fails in which case we'll need to roll back the drop.
            writer.WriteLine("            {0}", GetDropForeignKeyScript());
            writer.WriteLine("            {0}", GetCreateForeignKeyScript());
            writer.WriteLine("        commit tran");
            writer.WriteLine("    end");
            writer.WriteLine("end");
        }

        private string GetForeignKeyDefinition()
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(this.Source.ForeignKeyName))
                builder.AppendFormat("constraint [{0}] ", this.Source.ForeignKeyName);

            builder.Append("foreign key (");
            builder.Append(
                string.Join(
                    ",",
                    this.Source.KeyColumns.Select(k =>
                        string.Format("[{0}]", k.ForeignKeyColumnName)
                    )
                )
            );
            builder.Append(")");
            builder.AppendFormat(" references [{0}].[{1}](", this.DefaultSchemaName, this.Source.ReferencedTableName);
            builder.Append(
                string.Join(
                    ",",
                    this.Source.KeyColumns.Select(k =>
                        string.Format("[{0}]", k.ReferencedColumnName)
                    )
                )
            );
            builder.Append(")");

            return builder.ToString();
        }

        private string GetCreateForeignKeyScript()
        {
            return string.Format("alter table [{0}].[{1}] add {2}",
                this.DefaultSchemaName,
                this.Source.TableSource.TableName,
                GetForeignKeyDefinition()
            );
        }

        private string GetDropForeignKeyScript()
        {
            return string.Format("alter table [{0}].[{1}] drop constraint [{2}]",
                this.DefaultSchemaName,
                this.Source.TableSource.TableName,
                this.Source.ForeignKeyName
            );
        }

        private static string GetNoMatchingForeignKeyExistsCondition()
        {
            return "@ExistingFKColumnCount = 0";
        }

        private string GetForeignKeyChangeCondition()
        {
            return GetColumnCountChangeCondition() + " or " + GetColumnNameChangeCondition();
        }

        private string GetColumnCountChangeCondition()
        {
            return string.Format("(@ExistingFKColumnCount <> {0})", this.Source.KeyColumns.Count());
        }

        private static string GetColumnNameChangeCondition()
        {
            return "exists(select * from @DbFKColumnResults where [OrdinalPosition] is null or [ColumnName] is null or [ReferencedTableName] is null or [ReferencedColumnName] is null)";
        }
    }
}
