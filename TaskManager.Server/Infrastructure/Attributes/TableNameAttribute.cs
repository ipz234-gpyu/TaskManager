namespace TaskManager.Server.Infrastructure.Attributes
{
    public class TableNameAttribute : Attribute
    {
        public string TableName { get; init; }

        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
