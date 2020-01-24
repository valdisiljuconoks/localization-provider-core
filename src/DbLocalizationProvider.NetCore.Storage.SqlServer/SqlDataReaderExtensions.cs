using System.Data.SqlClient;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer
{
    public static class SqlDataReaderExtensions
    {
        public static string GetStringSafe(this SqlDataReader reader, string columnName)
        {
            var colIndex = reader.GetOrdinal(columnName);

            return !reader.IsDBNull(colIndex) ? reader.GetString(reader.GetOrdinal(columnName)) : string.Empty;
        }
    }
}
