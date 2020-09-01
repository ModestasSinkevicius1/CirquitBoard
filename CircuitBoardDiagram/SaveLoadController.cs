using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CircuitBoardDiagram
{
    class SaveLoadController
    {
        public void WriteXML(Object obj)
        {           
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ElementControl));

            //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            System.IO.FileStream file = System.IO.File.Create("SavedProperties/Serialization.xml");

            writer.Serialize(file, obj);
            file.Close();
        }

        public ElementControl ReadXML()
        {                       
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ElementControl));
            System.IO.FileStream file = System.IO.File.Open("SavedProperties/Serialization.xml",FileMode.Open);
            ElementControl ec = (ElementControl)reader.Deserialize(file);
            file.Close();

            return ec;
        }
    }
}
