using Hyper.Db.Model;

namespace Hyper.Db.Xml.DbObjects
{
    // TODO: When implementing the identity column, see comments below.
    /*
     * According to SQL server lore, a table can only have one identity column. An identity column may be added on table creation, or it may be added later, but once it has been
     * added, it can never be removed. In addition, no column other than the current identity column may become an identity column except by dropping and recreating the table.
     * In fact, in SMSS, when you use the UI to change the identity property from one column to another, the change script creates a new table with the new identity, copies
     * the data to the new table, drops the original table, then renames the new table.
     * 
     * As a result, our development writer should:
     *  )   Be able to add identity columns to existing tables that don't already have identity columns
     *  )   If a column is set as an identity in the XML, it must automatically be set to be non-nullable (SQL server requires this). However, if the user tried to explicitly set it to be nullable in the XML, throw an exception
     *  )   If the table already has an identity column with the same name and type as the XML, then we may want reseed the column, but SQL server does not allow you to change the increment
     *      -   Note that in this case, we should completely ignore all of the normal stuff we do for columns: we shouldn't do anything with defaults, changing the type, or anything since this is a special case.
     *      -   Checkout the following commands for managing identity columns:
     *          -   DBCC CHECKIDENT
     *          -   IDENT_SEED
     *  )   If the table already has an identity column with a different name or type, we should throw an exception
     *  )   If the table already has a column with the same name and type as the identity column specified in the XML, and if that column in the table is NOT an identity column, we should throw an exception
     *
     * and our production writer should:
     *  )   Be able to add identity columns to existing tables that don't already have identity columns
     *  )   If a column is set as an identity in the XML, it must automatically be set to be non-nullable (SQL server requires this). However, if the user tried to explicitly set it to be nullable in the XML, throw an exception
     *  )   If the table already has an identity column that doesn't match the XML we should throw an exception
     *      -   To "match", an existing ID column must have the same name and type
     *      -   I don't think the seed and increment values matter for this matching.
     *  )   If the table already has a column with the same name and type as the identity column specified in the XML, and if that column in the table is NOT an identity column, we should throw an exception
     */

    /*
     * As further notes on the identity column, I'm thinking perhaps we shouldn't have an inherited class just for identity. We should just move the seed and increment properties into the normal DbColumn class
     * class.
     * 
     * In addition, because we can't use XSD to force a single column element to define the identity attribute, we'll have to validate manually in C# that every table contains no more than just a single identity column.
     * 
     * Finally, as far as changes to our SQL writers, we'll have to add logic to the column writers to check if it's an identity before doing anything. We'll also have to modify the column definition code to add
     * the IDENTITY(1,1) as the last piece of the column definition
     */
    internal class DbIdentityColumn : DbColumn
    {
        public long Seed { get; set; }
        public long Increment { get; set; }
        public override bool? IsNullable
        {
            get { return false; }
            set { /* Identity Column can't be null, so override setter to do nothing */ }
        }
    }
}
