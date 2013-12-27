using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Brake
{
	public class IPAInfo
	{
		public string AppName;
		public string AppVersion;
		public string AppBundle;
		public string Location;
		public string BinaryLocation;

	}
	public class Configuration {
		public string host;
		public string Password;
		public int port;
	}
	public class Container
	{
		private static Container container;

		public static Container getContainer() {
			if (container == null) {
				try {
					XmlSerializer serializer = new XmlSerializer (typeof(Container));
					StreamReader reader = new StreamReader ("Brake.xml");
					container = (Container)serializer.Deserialize (reader);
					reader.Close ();	
				} catch (System.IO.FileNotFoundException) {
					Console.WriteLine ("==============================");
					Console.WriteLine ("Looks like you're new, please fill in the details of your device");
					container = new Container ();
				}
			}
			return container;
		}


		public List<IPAInfo> IPAItems;
		public Configuration Config;
		public Container()
		{
			IPAItems = new List<IPAInfo>();
			Config = new Configuration ();
		}

		public void SaveXML() {
			using (StreamWriter writer = new StreamWriter("Brake.xml")) {
				XmlSerializer serializer = new XmlSerializer (typeof(Container));
				serializer.Serialize (writer, this); 
				Console.WriteLine("serializing data");			
			}
		}

	}

}

