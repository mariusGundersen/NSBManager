namespace NSBManager
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            this.QueueList = new QueueList();
        }

        public ObservableCollection<QueueEntry> QueueList { get; set; } 
    }
}