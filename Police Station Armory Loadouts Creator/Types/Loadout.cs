namespace Police_Station_Armory_Loadouts_Creator.Types
{
    // System
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

    [Serializable]
    public class Loadout
    {
        [XmlElement]
        public List<Item> Items;
        [XmlElement]
        public string TextureFileName;
        [XmlElement]
        public string Name;

        [XmlIgnore]
        private string _description;
        [XmlIgnore]
        public string Description
        {
            get
            {
                if (String.IsNullOrEmpty(_description))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Item item in Items)
                    {
                        sb.AppendLine("-" + item.Type);
                        foreach (WeaponComponent comp in item.Components)
                        {
                            sb.AppendLine("  -" + comp);
                        }
                        sb.AppendLine();
                    }

                    return _description = sb.ToString();
                }
                return _description;
            }
            private set
            {
                _description = value;
            }
        }

        public Loadout()
        {
            Items = new List<Item>();
        }

        public static Loadout GetFromXML(string xmlFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Loadout));
            FileStream fs = new FileStream(xmlFilePath, FileMode.Open);
            Loadout loadout;
            loadout = (Loadout)serializer.Deserialize(fs);
            foreach (Item item in loadout.Items)
            {
                if (item.Components == null)
                {
                    item.Components = new WeaponComponent[] { };
                }
            }
            return loadout;
        }


        public static void WriteToXML(string xmlFilePath, Loadout loadout)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Loadout));
            TextWriter writer = new StreamWriter(xmlFilePath);
            serializer.Serialize(writer, loadout);
            writer.Close();
            writer.Dispose();
        }
    }
}
