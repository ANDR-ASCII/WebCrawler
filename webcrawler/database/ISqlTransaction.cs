namespace webcrawler.database
{
    interface ISqlTransaction
    {
        void Commit();
        void Rollback();
    }
}
