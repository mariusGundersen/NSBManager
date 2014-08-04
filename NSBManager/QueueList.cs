namespace NSBManager
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Messaging;

    public class QueueList : ObservableCollection<QueueEntry>
    {
        public QueueList()
        {
            MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName)
                .Select(x => new QueueEntry(x)).OrderBy(x => x.QueueName).ToList().ForEach(this.Add);
        }
    }
}
