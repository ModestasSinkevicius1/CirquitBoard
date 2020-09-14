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
                new System.Xml.Serialization.XmlSerializer(typeof(ListContainer));

            //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            FileStream file = System.IO.File.Create("SavedProperties/Serialization.xml");

            writer.Serialize(file, obj);
            file.Close();
        }

        public ListContainer ReadXML()
        {                       
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ListContainer));
            FileStream file = System.IO.File.Open("SavedProperties/Serialization.xml",FileMode.Open);
            ListContainer lc = (ListContainer)reader.Deserialize(file);
            file.Close();

            return lc;
        }
    }
}
