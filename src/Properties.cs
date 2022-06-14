using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace File_Manager
{
    public class Properties // класс свойств, в него входят длина страницы и последний активный каталог
    {
        public int LengthOfPage { get; set; }
        public string LastState { get; set; }
        public string ProperetiesPath { get; set; }
        public static int PageLength { get; set; }

        public Properties()
        {
            int length = LengthOfPage;
            string lastState = LastState;
        }

        public void SetProperetiesPath()
        {
            ProperetiesPath = String.Format(@"{0}\App.config", Environment.CurrentDirectory);
        }
        public void LoadSettings()
        {
            string xmlText = File.ReadAllText(ProperetiesPath);
            StringReader stringReader = new StringReader(xmlText);
            XmlSerializer serializer = new XmlSerializer(typeof(Properties));
            Properties properties = (Properties)serializer.Deserialize(stringReader);
            LengthOfPage = properties.LengthOfPage;
            LastState = properties.LastState;
            PageLength = LengthOfPage; 
            
        } 

        public void Save(string lastState, int PageLength)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Properties));
            Properties properties = new Properties();
            properties.LastState = lastState;
            properties.LengthOfPage = PageLength;
            serializer.Serialize(stringWriter, properties);
            string xml = stringWriter.ToString();
            File.WriteAllText(ProperetiesPath, xml);
        }

    }
}
