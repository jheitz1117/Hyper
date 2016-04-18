using System;
using System.IO;

namespace Hyper.Db.ScriptWriters.Sql
{
    public class SafeSqlScriptWriter : IDbScriptWriter
    {
        public void WriteDbScript(TextWriter writer, IDbTable table)
        {
            // TODO: If we're smart, I'm thinking we should be able to add some custom behaviors to the SafeSqlScriptWriter such that anytime we would normally throw a NotSupportedException() on account of an unsafe DB operation, we could instead execute a user-defined delegate. This would allow a certain amount of control in cases where data loss is either not a big deal, or there is a policy in place for those kinds of DB changes.
            // TODO: Write SQL to check if table exists. If not, create it. Otherwise... (see comments below)
            /*
             * If the table already exists, then we need to go column by column and:
             * 1)   If the column doesn't exist
             *      a)  If the column is (nullable with or without a default) or (non-nullable with a default), then add it TO THE END OF THE TABLE
             *      b)  Otherwise, the column is non-nullable without a default and cannot be automatically added safely.
             *          -   throw NotSupported exception with a message indicating why.
             *          -   A data migration is required for these kinds of changes
             * 2)   Otherwise, the column already exists, so:
             *      a)  If the new column and the old column differ by type or nullability, throw a NotSupported exception
             *      b)  Otherwise, the type and nullability has not changed, but we may or may not have a default constraint on the column.
             *          So we'll check if the existing column has a default value.
             *          i)  If the existing column does not have a default, then add the default constraint normally (if we have one to add, that is)
             *          ii) Otherwise, the existing column has a default.
             *              -   We check if the current default value is the same as the one we have. If it's different (either because we have a different value
             *                  or because we don't have a default defined at all), then throw a NotSupportedException, since we would have to drop the existing
             *                  default constraint before we could add the new value.
             *              -   Default values are relatively harmless, it seems, so it might be nice if we could have a setting that allows the user to specify whether or not it is okay
             *                  to drop a default constraint.
             * 3)   By this point, it's possible we might not have made any changes to the DB whatsoever. The idea is to only modify what we can without rebuilding the table or losing data.
             *      Then we simply blow out if any of the changes require a data migration.
             *      -   It's possible we may not want to react as strongly as to throw an exception. Instead, we may simply want to issue a warning indicating that a data migration
             *          is required for some specific change. That way, we can go ahead and add what we can, and then the user can add what's left after the update. Alternatively,
             *          I suppose the user can go ahead and make those changes first, then run the auto-updater. The auto-updater will simply find that everything it wouldn't have been
             *          able to update was already up to date, thereby foregoing the need to throw any exceptions.
             */
            throw new NotImplementedException();
        }

        public void WriteDbScript(TextWriter writer, IDbPrimaryKey primaryKey)
        {
            // TODO: Write SQL to check if PK exists. If not, create it. Otherwise, refuse to modify existing PK by throwing a NotSupported() exception with a message indicating why.
            throw new NotImplementedException();
        }

        public void WriteDbScript(TextWriter writer, IDbForeignKey foreignKey)
        {
            // TODO: Write SQL to check if FK exists. If not, create it. Otherwise, refuse to modify existing FK by throwing a NotSupported() exception with a message indicating why.
            throw new NotImplementedException();
        }
    }
}
