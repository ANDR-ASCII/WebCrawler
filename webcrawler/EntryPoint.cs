namespace webcrawler
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            loader.CommandWatcher watcher = new loader.CommandWatcher(0, 8);
            watcher.StartWatching();
        }
    }
}
