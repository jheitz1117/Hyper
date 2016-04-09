using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;

namespace Hyper.Db.ScriptWriters.Sql
{
    public class SqlTableScriptWriter : SqlScriptWriter, IDbTableScriptWriter
    {
        public IDbTableScriptSource Source { get; set; }

        public override void WriteDbScript(TextWriter writer)
        {
            var iWriter = new IndentedTextWriter(writer, "    ");

            iWriter.WriteLine("set nocount on");
            iWriter.WriteLine();
            iWriter.WriteLine("if {0}", GetTableExistsCondition());
            iWriter.WriteLine("begin");
            iWriter.WriteLine("    declare @DefaultConstraintName nvarchar(128)");

            iWriter.Indent++;
            foreach (var column in Source.Columns)
            {
                iWriter.WriteLine("if {0}", GetColumnExistsCondition(column));
                iWriter.WriteLine("begin");

                iWriter.Indent++;
                GetDropDefaultScript(column, iWriter);
                iWriter.Indent--;

                iWriter.WriteLine();
                iWriter.WriteLine("    {0}", GetAlterColumnStatement(column));
                iWriter.WriteLine("end");
                iWriter.WriteLine("else");
                iWriter.WriteLine("begin");
                iWriter.WriteLine("    {0}", GetAddColumnStatement(column));
                iWriter.WriteLine("end");
                iWriter.WriteLine();

                // Deliberately NOT checking for string.IsNullOrWhiteSpace() because:
                //     if the attribute is absent, the DefaultValue property will be null
                //     if the attribute is present and empty, the DefaultValue property will be the empty string, which is different than null. In this case, the default is simply
                //         the empty string
                //     Otherwise, if the attribute is present and populated, just use the specified value for the default
                if (column.DefaultValue != null)
                    iWriter.WriteLine("alter table [{0}].[{1}] add default ('{2}') for [{3}]", DefaultSchemaName, Source.TableName, column.DefaultValue, column.Name);
            }

            iWriter.Indent--;
            iWriter.WriteLine("end");
            iWriter.WriteLine("else");
            iWriter.WriteLine("begin");

            iWriter.Indent++;
            iWriter.WriteLine("create table [{0}].[{1}] (", DefaultSchemaName, Source.TableName);
            iWriter.Indent++;
            iWriter.WriteLine(
                string.Join(
                    "," + iWriter.NewLine + "        ",
                    Source.Columns.Select(c => GetColumnDefinitionString(c, true))
                )
            );
            iWriter.Indent--;
            iWriter.WriteLine(")");
            iWriter.Indent--;

            iWriter.WriteLine("end");

            // TODO: Still need to add support for identity columns
        }

        private static string GetColumnDefinitionString(IDbColumnScriptSource column)
        {
            return GetColumnDefinitionString(column, false);
        }

        private static string GetColumnDefinitionString(IDbColumnScriptSource column, bool isTableCreation)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("[{0}] [{1}]", column.Name, column.Type);

            if (column.MaxLength.HasValue)
            {
                builder.AppendFormat(" ({0}", column.MaxLength.Value);
                if (column.Decimals.HasValue)
                    builder.AppendFormat(",{0}", column.Decimals.Value);
                builder.Append(")");
            }

            if (column.IsNullable.HasValue)
            {
                if (!column.IsNullable.Value)
                    builder.Append(" not");

                builder.Append(" null");
            }

            // Deliberately NOT checking for string.IsNullOrWhiteSpace() because:
            //     if the attribute is absent, the DefaultValue property will be null
            //     if the attribute is present and empty, the DefaultValue property will be the empty string, which is different than null. In this case, the default is simply
            //         the empty string
            //     Otherwise, if the attribute is present and populated, just use the specified value for the default
            if (column.DefaultValue != null && isTableCreation)
                builder.AppendFormat(" default ('{0}')", column.DefaultValue);

            return builder.ToString();
        }

        private string GetTableExistsCondition()
        {
            return $"exists(select * from [INFORMATION_SCHEMA].[TABLES] where [TABLE_SCHEMA] = '{DefaultSchemaName}' and [TABLE_NAME] = '{Source.TableName}')";
        }

        private string GetColumnExistsCondition(IDbColumnScriptSource column)
        {
            return $"exists(select * from [INFORMATION_SCHEMA].[COLUMNS] where [TABLE_SCHEMA] = '{DefaultSchemaName}' and [TABLE_NAME] = '{Source.TableName}' and [COLUMN_NAME] = '{column.Name}')";
        }

        private string GetAddColumnStatement(IDbColumnScriptSource column)
        {
            return $"alter table [{DefaultSchemaName}].[{Source.TableName}] add {GetColumnDefinitionString(column)}";
        }

        private string GetAlterColumnStatement(IDbColumnScriptSource column)
        {
            return $"alter table [{DefaultSchemaName}].[{Source.TableName}] alter column {GetColumnDefinitionString(column)}";
        }

        private void GetDropDefaultScript(IDbColumnScriptSource column, TextWriter writer)
        {
            writer.WriteLine("select");
            writer.WriteLine("    @DefaultConstraintName = [d].[name]");
            writer.WriteLine("from [sys].[all_columns] [c]");
            writer.WriteLine("    inner join [sys].[tables]              [t] on [t].[object_id]         = [c].[object_id]");
            writer.WriteLine("    inner join [sys].[schemas]             [s] on [t].[schema_id]         = [s].[schema_id]");
            writer.WriteLine("    inner join [sys].[default_constraints] [d] on [c].[default_object_id] = [d].[object_id]");
            writer.WriteLine("where");
            writer.WriteLine("        [s].[name] = '{0}'", DefaultSchemaName);
            writer.WriteLine("    and [t].[name] = '{0}'", Source.TableName);
            writer.WriteLine("    and [c].[name] = '{0}'", column.Name);
            writer.WriteLine();
            writer.WriteLine("if @DefaultConstraintName is not null");
            writer.WriteLine("begin");
            writer.WriteLine("    exec('alter table [{0}].[{1}] drop constraint [' + @DefaultConstraintName + ']')", DefaultSchemaName, Source.TableName);
            writer.WriteLine("end");
        }
    }
}
