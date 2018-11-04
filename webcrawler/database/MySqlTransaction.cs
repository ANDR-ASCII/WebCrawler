namespace webcrawler.database
{
    class MySqlTransaction : ISqlTransaction
    {
        MySql.Data.MySqlClient.MySqlTransaction m_nativeTransaction;

        public MySqlTransaction(MySql.Data.MySqlClient.MySqlTransaction nativeTransaction)
        {
            m_nativeTransaction = nativeTransaction;
        }

        public void Commit()
        {
            m_nativeTransaction.Commit();
        }

        public void Rollback()
        {
            m_nativeTransaction.Rollback();
        }
    }
}
