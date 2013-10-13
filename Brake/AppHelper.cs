using System;
using System.IO;
using Ionic.Zip;
using PlistCS;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Brake
{
	public class AppHelper
	{
		public AppHelper ()
		{

		}
		public static void Debug(String debugString) {
			if (MainClass.DEBUG == 1) {
				Console.WriteLine ("Debug: " + debugString);
			}
		}
		public bool setupDirectory(String location) {
			DirectoryInfo di = new DirectoryInfo(location);
			Container container = new Container ();
			FileInfo[] files;
			//byte[] data;

			if (!di.Exists) {
				Console.WriteLine ("Error: Directory not found?");
				return false;
			}
			try
			{
				files = di.GetFiles("*.ipa", SearchOption.TopDirectoryOnly);  
			}
			catch (Exception e)
			{
				Console.WriteLine ("Error: Exception occured, perhaps no permissions for directory?");
				Console.WriteLine (e.ToString ());
				return false;
			}
			foreach (FileInfo file in files) {
				Debug ("IPA found: " + file.Name);
				using (ZipFile ipa = ZipFile.Read(file.FullName)) {
					foreach (ZipEntry e in ipa) {
						Debug ("ZipEntry: " + e.FileName);
						if (e.FileName.Contains (".sinf") || e.FileName.Contains (".supp")) {
							//extract it 
							Debug (GetTemporaryDirectory());
							e.Extract (GetTemporaryDirectory(), ExtractExistingFileAction.OverwriteSilently);

						} else if (e.FileName.Contains (".app/Info.plist")) {
							Debug ("INFO.PLIST FOUND!!!!! " + GetTemporaryDirectory());
							e.Extract (GetTemporaryDirectory(), ExtractExistingFileAction.OverwriteSilently);
							string plistLocation = Path.Combine (GetTemporaryDirectory(), e.FileName);
							//unpackDirectory + "/" + e.FileName;

							Dictionary<string, object> plist = (Dictionary<string,object>)Plist.readPlist (unpackDirectory + "/" + e.FileName);
							Debug ("plist data: " + plist.ToString ());
							Debug ("Bundle Name: " + plist ["CFBundleExecutable"]);
							IPAInfo info = new IPAInfo ();
							info.AppBundle = (string) plist ["CFBundleExecutable"];
							info.AppName = (string) plist ["CFBundleDisplayName"];
							info.AppVersion = (string) plist ["CFBundleVersion"];
							info.Location = file.FullName;
							container.Items.Add (info);
						}
					}
				}

			}
			using (StreamWriter writer = new StreamWriter("ipas.xml")) {
				XmlSerializer serializer = new XmlSerializer (typeof(Container));
				serializer.Serialize (writer, container); 
				Debug ("serializing data");			
			}
			Directory.Delete (GetTemporaryDirectory());
			return true;
		}

		private string tempDir;
		public string GetTemporaryDirectory()
		{
			if (tempDir == null) {
				tempDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
				Directory.CreateDirectory (tempDir);
			}
			return tempDir;
		}
	}
}

