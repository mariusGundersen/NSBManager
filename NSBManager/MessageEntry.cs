namespace NSBManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Messaging;
    using System.Xml;

    public class MessageEntry : AutoNotifyPropertyChanged
    {
        public MessageEntry(Message message)
        {
            var document = MsmqHelper.ConvertMessageToXmlDoc(message);
            this.Id = message.Id;
            this.Body = MsmqHelper.PrintXml(document);
            this.Type = GetMessageType(document);
            this.Sender = message.ResponseQueue.QueueName.Split('\\').Last();
            this.SentAt = message.SentTime;
        }

        public string Id { get; set; }

        public string Body { get; set; }

        public DateTime SentAt { get; set; }

        public string Type { get; set; }

        public string Sender { get; set; }

        public bool Equals(MessageEntry x, MessageEntry y)
        {
            return EqualityComparer<string>.Default.Equals(x.Id, y.Id);
        }

        public int GetHashCode(MessageEntry obj)
        {
            return EqualityComparer<string>.Default.GetHashCode(obj.Id);
        }

        private static string GetMessageType(XmlDocument document)
        {
            return document.DocumentElement != null ? (document.DocumentElement.FirstChild != null ? document.DocumentElement.FirstChild.Name : "no child") : "-";
        }
    }
}