using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace AutomaticImageFramer
{
    public class Settings
    {
        public Color Color { get; set; }
        public string BorderWidth { get; set; }

        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                xmls.Serialize(sw, this);
            }
        }
        public static Settings Read(string filename)
        {
            using (StreamReader sw = new StreamReader(filename))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(Settings));
                return xmls.Deserialize(sw) as Settings;
            }
        }
    }
}
