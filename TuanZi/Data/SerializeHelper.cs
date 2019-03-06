
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using TuanZi.Extensions;

namespace TuanZi.Data
{
    public static class SerializeHelper
    {
        #region Binary Serialization

        public static byte[] ToBytes(object value)
        {
            byte[] bytes = new byte[Marshal.SizeOf(value)];
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            Marshal.StructureToPtr(value, ptr, true);
            return bytes;
        }

        public static T FromBytes<T>(byte[] bytes)
        {
            Type type = typeof(T);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
            object obj = Marshal.PtrToStructure(ptr, type);
            return (T)obj;
        }


        public static byte[] ToBinary(object data)
        {
            data.CheckNotNull("data");
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);
                ms.Seek(0, 0);
                return ms.ToArray();
            }
        }

        public static T FromBinary<T>(byte[] bytes)
        {
            bytes.CheckNotNullOrEmpty("bytes");
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        public static void ToBinaryFile(string fileName, object data)
        {
            fileName.CheckNotNull("fileName");
            data.CheckNotNull("data");
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, data);
            }
        }

        public static T FromBinaryFile<T>(string fileName)
        {
            fileName.CheckFileExists("fileName");
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }

        #endregion

        #region XML Serialization

        public static string ToXml(object data)
        {
            data.CheckNotNull("data");
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(ms, data);
                ms.Seek(0, 0);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static T FromXml<T>(string xml)
        {
            xml.CheckNotNull("xml");
            byte[] bytes = Encoding.Default.GetBytes(xml);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        public static void ToXmlFile(string fileName, object data)
        {
            fileName.CheckNotNull("fileName" );
            data.CheckNotNull("data");
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(fs, data);
            }
        }

        public static T FromXmlFile<T>(string fileName)
        {
            fileName.CheckFileExists("fileName");
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }

        #endregion
    }
}