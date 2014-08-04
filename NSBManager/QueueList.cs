namespace NSBManager
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Messaging;

    public class QueueList : ObservableCollection<QueueEntry>
    {
        public QueueList()
        {
            MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName)
                .Select(x => new QueueEntry(x)).OrderBy(x => x.QueueName).ToList().ForEach(
                    entry =>
                        {
                            Add(entry);
                            entry.PropertyChanged += EntryOnPropertyChanged;
                        });
        }

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
