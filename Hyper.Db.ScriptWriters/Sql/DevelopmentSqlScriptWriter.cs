using System;
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


    // TODO: When implementing the identity column, see comments below.
    /*
     * Our production writer should be just like our developer version except:
     *  )   default constraints should never be dropped. Instead, if we would replace an existing default with something different, throw an exception
     *  )   column types and nullability may need some protection: perhaps according to a setting, either allow or disallow column types to be changed.
     *      -   The user just has to understand that if we allow column types to be changed, some data loss may occur, and/or some changes may throw an exception from SQL. These will have to be dealt with
     *          on a case-by-case basis
     *      -   On the other hand, if the types are not allowed to change automatically, then the system will always throw an exception when such a change might otherwise occur. This, at least, forces
     *          manual data migrations and completely prevents data from accidentally being lost due to a type change.
     *  )   identity column re-seeding should probably be allowed, since this can happen with no cost.        
     */
    public class DevelopmentSqlScriptWriter : IDbScriptWriter
    {
        public void WriteDbScript(TextWriter writer, IDbTable table)
        {
            // Check if we have multiple identity columns (this is DB-specific, so it is not part of the IDbSchema validation)
            if (table.Columns.Count(c => c.IsIdentity ?? false) > 1)
                throw new InvalidOperationException("Transact-SQL does not allow more than one identity column per table.");

            var iWriter = new IndentedTextWriter(writer, "    ");

            iWriter.WriteLine("set nocount on");
            iWriter.WriteLine();
            iWriter.WriteLine($"if {GetTableExistsCondition(table)}");
            iWriter.WriteLine("begin");
            iWriter.WriteLine("    declare @DefaultConstraintName nvarchar(128)");

            // If I need to write an identity column, check if one already exists
            var identityColumn = table.Columns.FirstOrDefault(c => c.IsIdentity ?? false);
            if (identityColumn != null)
            {
                iWriter.Indent++;
                iWriter.WriteLine("declare @IdentityColumnName nvarchar(128)");
                iWriter.WriteLine("declare @IdentityColumnType nvarchar(128)");
                iWriter.WriteLine();
                iWriter.WriteLine("select");
                iWriter.WriteLine("    @IdentityColumnName = [COLUMN_NAME],");
                iWriter.WriteLine("    @IdentityColumnType = [DATA_TYPE]");
                iWriter.WriteLine("from [INFORMATION_SCHEMA].[COLUMNS]");
                iWriter.WriteLine("where");
                iWriter.WriteLine($"        [TABLE_SCHEMA] = '{SqlDefaults.DefaultSchemaName}'");
                iWriter.WriteLine($"    and [TABLE_NAME] = '{table.Name}'");
                iWriter.WriteLine("    and columnproperty(object_id([TABLE_NAME]), [COLUMN_NAME], 'IsIdentity') = 1");
                iWriter.WriteLine();
                iWriter.WriteLine("if @IdentityColumnName is null");
                iWriter.WriteLine("begin");
                iWriter.WriteLine($"    if {GetColumnExistsCondition(table, identityColumn)}");
                iWriter.WriteLine("    begin");
                iWriter.WriteLine("        raiserror('Cannot add the identity property to an existing column.', 16, 1)");
                iWriter.WriteLine("    end");
                iWriter.WriteLine("end");
                iWriter.WriteLine("else");
                iWriter.WriteLine("begin");
                iWriter.WriteLine($"    if @IdentityColumnName <> '{identityColumn.Name}' or @IdentityColumnType <> '{identityColumn.Type}'");
                iWriter.WriteLine("    begin");
                iWriter.WriteLine("        raiserror('Cannot modify an existing identity column.', 16, 1)");
                iWriter.WriteLine("    end");
                iWriter.WriteLine("end");
                iWriter.WriteLine();
                iWriter.Indent--;
            }

            iWriter.Indent++;
            foreach (var column in table.Columns)
            {
                // If the column doesn't exist, we always want to add it.
                iWriter.WriteLine($"if not {GetColumnExistsCondition(table, column)}");
                iWriter.WriteLine("begin");
                iWriter.WriteLine($"    {GetAddColumnStatement(table, column)}");
                iWriter.WriteLine("end");

                // However, if the column does exist, we only want to alter it if it is NOT an identity column, since identity columns can't be safely changed after they've been created.
                if (!(column.IsIdentity ?? false))
                {
                    iWriter.WriteLine("else");
                    iWriter.WriteLine("begin");
                    iWriter.Indent++;
                    WriteDropDefaultScript(iWriter, table, column);
                    iWriter.Indent--;
                    iWriter.WriteLine();
                    iWriter.WriteLine($"    {GetAlterColumnStatement(table, column)}");
                    iWriter.WriteLine("end");
                }
                
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
                    table.Columns.Select(c => $"{GetColumnDefinitionString(c, true)} {GetIdentityDefinitionString(c)}")
                )
            );
            iWriter.Indent--;
            iWriter.WriteLine(")");
            iWriter.Indent--;

            iWriter.WriteLine("end");
            iWriter.WriteLine();
            iWriter.WriteLine("go");
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
            writer.WriteLine();
            writer.WriteLine("go");
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
            writer.WriteLine();
            writer.WriteLine("go");
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

            if (column.IsIdentity ?? false)
            {
                if (column.IsNullable.HasValue && column.IsNullable.Value)
                    throw new InvalidOperationException("An identity column cannot be nullable.");
                if (column.DefaultValue != null)
                    throw new InvalidOperationException("An identity column cannot have a default value.");
            }
            else
            {
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
            }

            return builder.ToString();
        }

        private static string GetIdentityDefinitionString(IDbColumn column)
        {
            return (column.IsIdentity ?? false) ? $"identity({column.Seed ?? 1},{column.Increment ?? 1})" : "";
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
            return $"alter table [{SqlDefaults.DefaultSchemaName}].[{table.Name}] add {GetColumnDefinitionString(column)} {GetIdentityDefinitionString(column)}";
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
