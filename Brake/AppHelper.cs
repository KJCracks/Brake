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
		public Container getIPAs(String location) {
			DirectoryInfo di = new DirectoryInfo(location);
			Container container = new Container ();
			FileInfo[] files;
			//byte[] data;

			if (!di.Exists) {
				Console.WriteLine ("Error: Directory not found?");
				return null;
			}
			try
			{
				files = di.GetFiles("*.ipa", SearchOption.TopDirectoryOnly);  
			}
			catch (Exception e)
			{
				Console.WriteLine ("Error: Exception occured, perhaps no permissions for directory?");
				Console.WriteLine (e.ToString ());
				return null;
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

							Dictionary<string, object> plist = (Dictionary<string,object>)Plist.readPlist (plistLocation);
							Debug ("plist data: " + plist.ToString ());
							Debug ("Bundle Name: " + plist ["CFBundleExecutable"]);
							IPAInfo info = new IPAInfo ();
							info.AppBundle = (string) plist ["CFBundleExecutable"];
                            if (plist.ContainsKey("CFBundleDisplayName"))
                            {
                                info.AppName = (string) plist ["CFBundleDisplayName"];
                            }
                            else
                            {
                                info.AppName = (string) plist ["CFBundleName"];
                            }
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
			DeleteDirectory (GetTemporaryDirectory ());
			return container;
		}
			
		private static string tempDir;
		public static string GetTemporaryDirectory()
		{
			if (tempDir == null) {
				tempDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
				Directory.CreateDirectory (tempDir);
			}
			return tempDir;
		}
		public static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);
		}
	}
}

