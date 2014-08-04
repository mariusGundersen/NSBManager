namespace NSBManager
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            this.QueueList = new AutoRefreshListCollectionView(new QueueList());
        }

        public AutoRefreshListCollectionView QueueList { get; set; } 
    }
}