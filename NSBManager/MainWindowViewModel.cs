namespace NSBManager
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Messaging;
    using System.Threading.Tasks;

    public class MainWindowViewModel
    {
        private readonly ObservableCollection<MessageEntry> messages;

        private QueueEntry selectedQueueEntry;

        private AsyncMessageFetcher worker;

        public MainWindowViewModel()
        {
            this.messages = new ObservableCollection<MessageEntry>();

            this.QueueList = new AutoRefreshListCollectionView(new QueueList());

            this.SelectedQueue = new AutoRefreshListCollectionView(this.messages);
        }

        public AutoRefreshListCollectionView QueueList { get; set; }

        public QueueEntry SelectedQueueEntry 
        {
            get
            {
                return this.selectedQueueEntry;
            }

            set
            {
                if (this.selectedQueueEntry != null)
                {
                    this.selectedQueueEntry.PropertyChanged -= this.UpdateListOfMessages;
                }
                if (this.worker != null)
                {
                    this.worker.Stop();
                }

                this.messages.Clear();
                this.SetSelectedQueueMessages(value);
                this.selectedQueueEntry = value;
                this.selectedQueueEntry.PropertyChanged += this.UpdateListOfMessages;
            }
        }

        public AutoRefreshListCollectionView SelectedQueue { get; set; }

        private void UpdateListOfMessages(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.SetSelectedQueueMessages(sender as QueueEntry);
        }

        private void SetSelectedQueueMessages(QueueEntry value)
        {
            this.worker = new AsyncMessageFetcher(value.QueuePath);
            this.worker.MessagesFound += (sender, messages) =>
                {
                    if (this.worker != sender)
                    {
                        return;
                    }

                    this.worker = null;
                    this.messages.Except(messages).ToList().ForEach(m => this.messages.Remove(m));
                    messages.Except(this.messages).ToList().ForEach(m => this.messages.Add(m));
                };
        }
    }
}