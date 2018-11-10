using System.Collections.Generic;

namespace webcrawler.database
{
    public enum ProcedureArgumentType
    {
        Decimal,
        Byte,
        Int16,
        Int32,
        Float,
        Double,
        Timestamp,
        Int64,
        Int24,
        Date,
        Time,
        DateTime,
        Year,
        Newdate,
        VarString,
        Bit,
        JSON,
        NewDecimal,
        Enum,
        Set,
        TinyBlob,
        MediumBlob,
        LongBlob,
        Blob,
        VarChar,
        String,
        Geometry,
        UInt16,
        UInt32,
        UInt64,
        UInt24,
        Binary,
        VarBinary,
        TinyText,
        MediumText,
        LongText,
        Text,
        Guid
    }

    struct SqlCommandParameter
    {
        public string parameterName;
        public ProcedureArgumentType type;
        public object value;
    }

    interface IDbConnection
    {
        void SetServerAddress(string serverAddress);
        void SetUserName(string userName);
        void SetUserPassword(string password);
        void SetDatabaseName(string database);
        bool Open();
        bool Close();
        bool Ping();

        int ExecuteNonQuery(string query, params SqlCommandParameter[] sqlCommandParameters);
        int ExecuteStoredProcedure(string procedureName, string query, Dictionary<string, (ProcedureArgumentType type, bool isOutput)> procedureParameters);
        List<object>[] ExecuteReadQuery(string query);
        Dictionary<string, object>[] ExecuteReadQuery(string query, params string[] columns);

        ISqlTransaction BeginTransaction();
    }
}
