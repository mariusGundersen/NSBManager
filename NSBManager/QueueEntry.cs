namespace NSBManager
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Messaging;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    public class QueueEntry : INotifyPropertyChanged
    {
        private readonly PerformanceCounter queueCounter;

        private ICommand purgeCommand;

        public QueueEntry(MessageQueue messageQueue)
        {
            this.QueueName = messageQueue.QueueName.Split('\\').Last();
            this.QueueLength = 0;
            this.QueuePath = string.Format(@"{0}\private$\{1}", Environment.MachineName, this.QueueName);

            this.queueCounter = new PerformanceCounter(
                "MSMQ Queue",
                "Messages in Queue",
                this.QueuePath);

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += this.Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
            dispatcherTimer.Start();

            this.UpdateQueueLength();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string QueueName { get; set; }

        public int QueueLength { get; set; }

        public string QueuePath { get; set; }

        public FontWeight IsBold
        {
            get
            {
                return this.QueueLength > 0 ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        public ICommand PurgeCommand
        {
            get
            {
                return this.purgeCommand ?? (this.purgeCommand = new RelayCommand(param => this.Purge(), param => this.QueueLength > 0 ));
            }
        }

        public void Purge()
        {
            var msgQueue = new MessageQueue(this.QueuePath);
            msgQueue.Purge();
            this.UpdateQueueLength();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            this.UpdateQueueLength();
        }

        private void UpdateQueueLength()
        {
            var old = this.QueueLength;
            this.QueueLength = this.GetQueueLength();
            if (this.QueueLength == old)
            {
                return;
            }

            this.OnPropertyChanged("QueueLength");
            if (old == 0 && this.QueueLength > 0)
            {
                this.OnPropertyChanged("IsBold");
            }
            else if (old > 0 && this.QueueLength == 0)
            {
                this.OnPropertyChanged("IsBold");
            }
        }

        private int GetQueueLength()
        {
            try
            {
                return (int)this.queueCounter.NextValue();
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}