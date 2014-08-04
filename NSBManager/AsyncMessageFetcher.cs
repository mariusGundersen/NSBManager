namespace NSBManager
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Messaging;

    public class AsyncMessageFetcher
    {
        private readonly string queuePath;

        private readonly BackgroundWorker worker = new BackgroundWorker();

        private List<MessageEntry> result;

        public AsyncMessageFetcher(string queuePath)
        {
            this.queuePath = queuePath;
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += this.DoWork;
            this.worker.RunWorkerCompleted += this.BwCompleted;
            this.worker.RunWorkerAsync();
        }
        
        public delegate void MessageFetchingCompletedEvent(object sender, IList<MessageEntry> messages);

        public event MessageFetchingCompletedEvent MessagesFound;

        public void Stop()
        {
            this.worker.CancelAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var queue = new MessageQueue(this.queuePath);
            queue.MessageReadPropertyFilter.SentTime = true;
            this.result = queue.GetAllMessages().Select(x => new MessageEntry(x)).ToList();
        }

        private void BwCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == false && this.result != null)
            {
                this.MessagesFound(this, this.result);
            }
        }
    }
}