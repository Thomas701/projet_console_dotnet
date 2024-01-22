using System.IO;
using System.Xml.Serialization;

namespace Projet_Console_Serialisation_Data
{
    internal static class SerializerFactory
    {
        public static ISerializer CreateSerializer(SerializationType type)
        {
            switch (type)
            {
                case SerializationType.Xml:
                    return new XmlSerializerAdapter();
                default:
                    throw new ArgumentException("Type de sérialiseur non pris en charge.");
            }
        }
    }

    internal enum SerializationType
    {
        Xml,
    }

    internal interface ISerializer
    {
        void Serialize(object obj, string filePath);
        object Deserialize(string filePath);
    }

    internal class XmlSerializerAdapter : ISerializer
    {
        public void Serialize(object obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }

        public object Deserialize(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Dossier)); 
            using (TextReader reader = new StreamReader(filePath))
            {
                return serializer.Deserialize(reader);
            }
        }
    }
}