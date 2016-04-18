using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;

namespace Hyper.Db.ScriptWriters.Sql
{
    // TODO: I saw a hint on how to accomplish writing out a stored procedure (see comments below).
    /*
     * The idea is to use the metadata tables to determine if the SP exists or not, as usual. Then, if it does exist,
     * simply write ALTER along with the proc name and parameters and definition.
     * 
     * If it doesn't exist, create a stub using the proc name and no parameters (along with a GO statement afterwards),
     * then simply proceed as above. This seems much easier than with tables.
     */

    public class DevelopmentSqlScriptWriter : IDbScriptWriter
    {
        public void WriteDbScript(TextWriter writer, IDbTable table)
        {
            var iWriter = new IndentedTextWriter(writer, "    ");

            iWriter.WriteLine("set nocount on");
            iWriter.WriteLine();
            iWriter.WriteLine($"if {GetTableExistsCondition(table)}");
            iWriter.WriteLine("begin");
            iWriter.WriteLine("    declare @DefaultConstraintName nvarchar(128)");

            iWriter.Indent++;
            foreach (var column in table.Columns)
            {
                iWriter.WriteLine($"if {GetColumnExistsCondition(table, column)}");
                iWriter.WriteLine("begin");

                iWriter.Indent++;
                WriteDropDefaultScript(iWriter, table, column);
                iWriter.Indent--;

                iWriter.WriteLine();
                iWriter.WriteLine($"    {GetAlterColumnStatement(table, column)}");
                iWriter.WriteLine("end");
                iWriter.WriteLine("else");
                iWriter.WriteLine("begin");
                iWriter.WriteLine($"    {GetAddColumnStatement(table, column)}");
                iWriter.WriteLine("end");
                iWriter.WriteLine();

                // Notice that if you want the default value to be the empty string, you must explicitly indicate as much using empty single quotes (like you would in SQL)
                if (!string.IsNullOrWhiteSpace(column.DefaultValue))
                    iWriter.WriteLine($"alter table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] add default ({column.DefaultValue}) for [{column.Name}]");
            }

            iWriter.Indent--;
            iWriter.WriteLine("end");
            iWriter.WriteLine("else");
            iWriter.WriteLine("begin");

            iWriter.Indent++;
            iWriter.WriteLine($"create table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] (");
            iWriter.Indent++;
            iWriter.WriteLine(
                string.Join(
                    "," + iWriter.NewLine + "        ",
                    table.Columns.Select(c => GetColumnDefinitionString(c, true))
                )
            );
            iWriter.Indent--;
            iWriter.WriteLine(")");
            iWriter.Indent--;

            iWriter.WriteLine("end");

            // TODO: Still need to add support for identity columns and GUID columns that auto-generate new GUIDs
        }

        public void WriteDbScript(TextWriter writer, IDbPrimaryKey primaryKey)
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
                    primaryKey.KeyColumns.Zip(
                        Enumerable.Range(1, primaryKey.KeyColumns.Count()),
                        (s, i) => $"({i}, '{s}')"
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
            writer.WriteLine($"        and [tc].[CONSTRAINT_TYPE]    = '{SqlConstraintTypes.PrimaryKey}'");
            writer.WriteLine($"        and [tc].[TABLE_SCHEMA]       = '{SqlDefaults.DefaultSchemaName}'");
            writer.WriteLine($"        and [tc].[TABLE_NAME]         = '{primaryKey.TableSource.Name}'");
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
            writer.WriteLine($"if {GetNoPrimaryKeyExistsCondition()}");
            writer.WriteLine("begin");
            writer.WriteLine($"    {GetCreatePrimaryKeyScript(primaryKey)}"); // In this case, no key exists, so we can create a new one
            writer.WriteLine("end");
            writer.WriteLine("else");
            writer.WriteLine("begin");
            writer.WriteLine($"    if {GetPrimaryKeyChangeCondition(primaryKey)}");
            writer.WriteLine("    begin");
            writer.WriteLine("        begin tran"); // We want to surround the next two commands in a transaction envelope in case the add fails in which case we'll need to roll back the drop.
            writer.WriteLine($"            {GetDropPrimaryKeyScript(primaryKey)}");
            writer.WriteLine($"            {GetCreatePrimaryKeyScript(primaryKey)}");
            writer.WriteLine("        commit tran");
            writer.WriteLine("    end");
            writer.WriteLine("end");
        }

        public void WriteDbScript(TextWriter writer, IDbForeignKey foreignKey)
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
                    foreignKey.KeyColumns.Zip(
                        Enumerable.Range(1, foreignKey.KeyColumns.Count()),
                        (s, i) => $"({i}, '{s.ForeignKeyColumnName}', '{foreignKey.ReferencedTableName}', '{s.ReferencedColumnName}')"
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
            writer.WriteLine($"        and [tc].[CONSTRAINT_TYPE]        = '{SqlConstraintTypes.ForeignKey}'");
            writer.WriteLine($"        and [tc].[TABLE_SCHEMA]           = '{SqlDefaults.DefaultSchemaName}'");
            writer.WriteLine($"        and [tc].[TABLE_NAME]             = '{foreignKey.TableSource.Name}'");
            writer.WriteLine($"        and [tc].[CONSTRAINT_NAME]        = '{foreignKey.Name}'");
            writer.WriteLine("    left join @XmlFKColumns [xfk] on");
            writer.WriteLine("            [xfk].[ColumnName]            = [kcu].[COLUMN_NAME]");
            writer.WriteLine("        and [xfk].[OrdinalPosition]       = [kcu].[ORDINAL_POSITION]");
            writer.WriteLine("        and [xfk].[ReferencedTableName]   = [kcuRef].[TABLE_NAME]");
            writer.WriteLine("        and [xfk].[ReferencedColumnName]  = [kcuRef].[COLUMN_NAME]");
            writer.WriteLine();
            writer.WriteLine("declare @ExistingFKColumnCount [int]");
            writer.WriteLine("select @ExistingFKColumnCount = COUNT(*) from @DbFKColumnResults");
            writer.WriteLine();
            writer.WriteLine($"if {GetNoMatchingForeignKeyExistsCondition()}");
            writer.WriteLine("begin");
            writer.WriteLine($"    {GetCreateForeignKeyScript(foreignKey)}"); // In this case, no key exists, so we can create a new one
            writer.WriteLine("end");
            writer.WriteLine("else");
            writer.WriteLine("begin");
            writer.WriteLine($"    if {GetForeignKeyChangeCondition(foreignKey)}");
            writer.WriteLine("    begin");
            writer.WriteLine("        begin tran"); // We want to surround the next two commands in a transaction envelope in case the add fails in which case we'll need to roll back the drop.
            writer.WriteLine($"            {GetDropForeignKeyScript(foreignKey)}");
            writer.WriteLine($"            {GetCreateForeignKeyScript(foreignKey)}");
            writer.WriteLine("        commit tran");
            writer.WriteLine("    end");
            writer.WriteLine("end");
        }

        #region Table

        private static string GetColumnDefinitionString(IDbColumn column)
        {
            return GetColumnDefinitionString(column, false);
        }

        private static string GetColumnDefinitionString(IDbColumn column, bool isTableCreation)
        {
            var builder = new StringBuilder();

            builder.Append($"[{column.Name}] [{column.Type}]");

            if (column.MaxLength.HasValue)
            {
                builder.Append($" ({column.MaxLength.Value}");
                if (column.Decimals.HasValue)
                    builder.Append($",{column.Decimals.Value}");
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
                builder.Append($" default ({column.DefaultValue})");

            return builder.ToString();
        }

        private static string GetTableExistsCondition(IDbTable table)
        {
            return $"exists(select * from [INFORMATION_SCHEMA].[TABLES] where [TABLE_SCHEMA] = '{SqlDefaults.DefaultSchemaName}' and [TABLE_NAME] = '{table.Name}')";
        }

        private static string GetColumnExistsCondition(IDbTable table, IDbColumn column)
        {
            return $"exists(select * from [INFORMATION_SCHEMA].[COLUMNS] where [TABLE_SCHEMA] = '{SqlDefaults.DefaultSchemaName}' and [TABLE_NAME] = '{table.Name}' and [COLUMN_NAME] = '{column.Name}')";
        }

        private static string GetAddColumnStatement(IDbTable table, IDbColumn column)
        {
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] add {GetColumnDefinitionString(column)}";
        }

        private static string GetAlterColumnStatement(IDbTable table, IDbColumn column)
        {
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] alter column {GetColumnDefinitionString(column)}";
        }

        private static void WriteDropDefaultScript(TextWriter writer, IDbTable table, IDbColumn column)
        {
            writer.WriteLine("select");
            writer.WriteLine("    @DefaultConstraintName = [d].[name]");
            writer.WriteLine("from [sys].[all_columns] [c]");
            writer.WriteLine("    inner join [sys].[tables]              [t] on [t].[object_id]         = [c].[object_id]");
            writer.WriteLine("    inner join [sys].[schemas]             [s] on [t].[schema_id]         = [s].[schema_id]");
            writer.WriteLine("    inner join [sys].[default_constraints] [d] on [c].[default_object_id] = [d].[object_id]");
            writer.WriteLine("where");
            writer.WriteLine($"        [s].[name] = '{SqlDefaults.DefaultSchemaName}'");
            writer.WriteLine($"    and [t].[name] = '{table.Name}'");
            writer.WriteLine($"    and [c].[name] = '{column.Name}'");
            writer.WriteLine();
            writer.WriteLine("if @DefaultConstraintName is not null");
            writer.WriteLine("begin");
            writer.WriteLine($"    exec('alter table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] drop constraint [' + @DefaultConstraintName + ']')");
            writer.WriteLine("end");
        }

        #endregion Table

        #region Primary Key

        private static string GetPrimaryKeyDefinition(IDbPrimaryKey source)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(source.Name))
                builder.Append($"constraint [{source.Name}] ");

            builder.Append("primary key (");
            builder.Append(
                string.Join(
                    ",",
                    source.KeyColumns.Select(k => $"[{k}]")
                )
            );
            builder.Append(")");

            return builder.ToString();
        }

        private static string GetCreatePrimaryKeyScript(IDbPrimaryKey source)
        {
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{source.TableSource.Name}] add {GetPrimaryKeyDefinition(source)}";
        }

        private static string GetDropPrimaryKeyScript(IDbPrimaryKey source)
        {
            // Have to use exec because we're dynamically grabbing the name of the existing primary key
            return $"exec('alter table [{SqlDefaults.DefaultSchemaName}].[{source.TableSource.Name}] drop constraint [' + @ExistingPKName + ']')";
        }

        private static string GetNoPrimaryKeyExistsCondition()
        {
            return "@ExistingPKColumnCount is null";
        }

        private static string GetPrimaryKeyChangeCondition(IDbPrimaryKey source)
        {
            return GetColumnCountChangeCondition(source) + " or " + GetPkColumnNameChangeCondition();
        }

        private static string GetColumnCountChangeCondition(IDbPrimaryKey source)
        {
            return $"(@ExistingPKColumnCount <> {source.KeyColumns.Count()})";
        }

        private static string GetPkColumnNameChangeCondition()
        {
            return "exists(select * from @DbPKColumnResults where [OrdinalPosition] is null or [ColumnName] is null)";
        }

        #endregion Primary Key

        #region Foreign Key

        private static string GetForeignKeyDefinition(IDbForeignKey source)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(source.Name))
                builder.Append($"constraint [{source.Name}] ");

            builder.Append("foreign key (");
            builder.Append(
                string.Join(
                    ",",
                    source.KeyColumns.Select(k =>
                        $"[{k.ForeignKeyColumnName}]"
                    )
                )
            );
            builder.Append(")");
            builder.Append($" references [{SqlDefaults.DefaultSchemaName}].[{source.ReferencedTableName}](");
            builder.Append(
                string.Join(
                    ",",
                    source.KeyColumns.Select(k =>
                        $"[{k.ReferencedColumnName}]"
                    )
                )
            );
            builder.Append(")");

            return builder.ToString();
        }

        private static string GetCreateForeignKeyScript(IDbForeignKey source)
        {
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{source.TableSource.Name}] add {GetForeignKeyDefinition(source)}";
        }

        private static string GetDropForeignKeyScript(IDbForeignKey source)
        {
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{source.TableSource.Name}] drop constraint [{source.Name}]";
        }

        private static string GetNoMatchingForeignKeyExistsCondition()
        {
            return "@ExistingFKColumnCount = 0";
        }

        private static string GetForeignKeyChangeCondition(IDbForeignKey source)
        {
            return GetColumnCountChangeCondition(source) + " or " + GetFkColumnNameChangeCondition();
        }

        private static string GetColumnCountChangeCondition(IDbForeignKey source)
        {
            return $"(@ExistingFKColumnCount <> {source.KeyColumns.Count()})";
        }

        private static string GetFkColumnNameChangeCondition()
        {
            return "exists(select * from @DbFKColumnResults where [OrdinalPosition] is null or [ColumnName] is null or [ReferencedTableName] is null or [ReferencedColumnName] is null)";
        }

        #endregion Foreign Key
    }
}
