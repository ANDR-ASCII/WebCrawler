namespace webcrawler.database
{
    class MySqlTransaction : ISqlTransaction
    {
        MySql.Data.MySqlClient.MySqlTransaction _nativeTransaction;

        public MySqlTransaction(MySql.Data.MySqlClient.MySqlTransaction nativeTransaction)
        {
            _nativeTransaction = nativeTransaction;
        }

        public void Commit()
        {
            _nativeTransaction.Commit();
        }

        public void Rollback()
        {
            _nativeTransaction.Rollback();
        }
    }
}
