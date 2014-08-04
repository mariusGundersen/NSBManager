namespace NSBManager
{
    using System.IO;
    using System.Messaging;
    using System.Text;
    using System.Xml;

    public class MsmqHelper
    {
        public static XmlDocument ConvertMessageToXmlDoc(Message msg)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(msg.BodyStream);
                return doc;
            }
            catch (XmlException)
            {
                return new XmlDocument();
            }
        }

        public static string PrintXml(XmlDocument document)
        {
            using (var mStream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(mStream, Encoding.Unicode))
                {

                    try
                    {
                        writer.Formatting = Formatting.Indented;

                        // Write the XML into a formatting XmlTextWriter
                        document.WriteContentTo(writer);
                        writer.Flush();
                        mStream.Flush();

                        // Have to rewind the MemoryStream in order to read
                        // its contents.
                        mStream.Position = 0;

                        // Read MemoryStream contents into a StreamReader.
                        var sReader = new StreamReader(mStream);

                        // Extract the text from the StreamReader.
                        return sReader.ReadToEnd();
                    }
                    catch (XmlException)
                    {
                        return string.Empty;
                    }
                }
            }
        }
    }
}