using System.IO;
using System.Xml.Serialization;

namespace DimenshipBase
{
    public static class SerializationExtension
    {
        public static string ToXmlString(this object o)
        {
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            using (StringWriter w = new StringWriter())
            {
                serializer.Serialize(w, o);
                return w.ToString();
            }
        }
    }
}