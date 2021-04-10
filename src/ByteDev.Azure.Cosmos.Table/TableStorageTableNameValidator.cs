namespace ByteDev.Azure.Cosmos.Table
{
    internal class TableStorageTableNameValidator
    {
        public bool IsValid(string tableName)
        {
            /*
            Table Storage table name:
            - must be unique within an account
            - may contain only [A-Za-z0-9] char
            - cannot begin with a numeric char
            - are case-insensitive
            - must be from 3 to 63 characters long
            - some table names are reserved, including "tables"

            "^[A-Za-z][A-Za-z0-9]{2,62}$"
            */

            return true;
        }
    }
}