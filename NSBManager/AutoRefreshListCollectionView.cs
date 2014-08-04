namespace NSBManager
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;

    public class AutoRefreshListCollectionView : ListCollectionView
    {
        public AutoRefreshListCollectionView(IList list)
            : base(list)
        {
            this.SubscribeSourceEvents(list, false);
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var refresh = this.SortDescriptions.Any(sort => sort.PropertyName == e.PropertyName)
                       || (this.GroupDescriptions != null && this.GroupDescriptions.OfType<PropertyGroupDescription>().Any(propertyGroup => propertyGroup.PropertyName == e.PropertyName));

            if (!refresh)
            {
                return;
            }

            if (!this.Dispatcher.CheckAccess())
            {
                // Invoke handler in the target dispatcher's thread
                this.Dispatcher.Invoke(this.Refresh);
            }
            else
            {
                // Execute handler as is
                this.Refresh();
            }
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.SubscribeItemsEvents(e.NewItems, false);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.SubscribeItemsEvents(e.OldItems, true);
                    break;
            }
        }

        private void SubscribeItemEvents(object item, bool remove)
        {
            var notify = item as INotifyPropertyChanged;

            if (notify == null)
            {
                return;
            }

            if (remove)
            {
                notify.PropertyChanged -= this.ItemPropertyChanged;
            }
            else
            {
                notify.PropertyChanged += this.ItemPropertyChanged;
            }
        }

        private void SubscribeItemsEvents(IEnumerable items, bool remove)
        {
            foreach (var item in items)
            {
                this.SubscribeItemEvents(item, remove);
            }
        }

        private void SubscribeSourceEvents(object source, bool remove)
        {
            var notify = source as INotifyCollectionChanged;

            if (notify != null)
            {
                if (remove)
                {
                    notify.CollectionChanged -= this.SourceCollectionChanged;
                }
                else
                {
                    notify.CollectionChanged += this.SourceCollectionChanged;
                }
            }

            this.SubscribeItemsEvents((IEnumerable)source, remove);
        }
    }
}