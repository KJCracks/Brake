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

		public static void Debug (String debugString)
		{
			if (MainClass.DEBUG == 1) {
				Console.WriteLine ("Debug: " + debugString);
			}
		}

		public Container getIPAs (String location)
		{
			DirectoryInfo di = new DirectoryInfo (location);
			Container container = new Container ();
			FileInfo[] files;
			//byte[] data;

			if (!di.Exists) {
				Console.WriteLine ("Error: Directory not found?");
				return null;
			}
			try {
				files = di.GetFiles ("*.ipa", SearchOption.TopDirectoryOnly);  
			} catch (Exception e) {
				Console.WriteLine ("Error: Exception occured, perhaps no permissions for directory?");
				Console.WriteLine (e.ToString ());
				return null;
			}
			foreach (FileInfo file in files) {
				Debug ("IPA found: " + file.Name);
				using (ZipFile ipa = ZipFile.Read(file.FullName)) {
					foreach (ZipEntry e in ipa) {
						//Debug ("ZipEntry: " + e.FileName);
						if (e.FileName.Contains (".app/Info.plist")) {
							Debug ("INFO.PLIST FOUND!!!!! " + GetTemporaryDirectory ());
							e.Extract (GetTemporaryDirectory (), ExtractExistingFileAction.OverwriteSilently);
							string plistLocation = Path.Combine (GetTemporaryDirectory (), e.FileName);
							//unpackDirectory + "/" + e.FileName;

							Dictionary<string, object> plist = (Dictionary<string,object>)Plist.readPlist (plistLocation);
							Debug ("plist data: " + plist.ToString ());
							Debug ("Bundle Name: " + plist ["CFBundleExecutable"]);
							IPAInfo info = new IPAInfo ();
							info.AppBundle = (string)plist ["CFBundleExecutable"];
							if (plist.ContainsKey ("CFBundleDisplayName")) {
								info.AppName = (string)plist ["CFBundleDisplayName"];
							} else {
								info.AppName = (string)plist ["CFBundleName"];
							}
							info.AppVersion = (string)plist ["CFBundleVersion"];                 
							info.Location = file.FullName;
							info.BinaryLocation = e.FileName.Replace ("Info.plist", "") + info.AppBundle;
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

		public String extractIPA (IPAInfo info)
		{
			using (ZipFile ipa = ZipFile.Read(info.Location)) {
				Debug ("IPALocation: " + info.Location);
				Debug ("Payload location: " + info.BinaryLocation);
				foreach (ZipEntry e in ipa) {
					//Debug ("file:" + e.FileName);
					if (e.FileName.Contains (".sinf") || e.FileName.Contains (".supp")) {
						//extract it 
						Debug (GetTemporaryDirectory ());
						e.Extract (GetTemporaryDirectory (), ExtractExistingFileAction.OverwriteSilently);
					} else if (e.FileName == info.BinaryLocation) {
						Debug ("found binary!!");
						e.Extract (GetTemporaryDirectory (), ExtractExistingFileAction.OverwriteSilently);
					}
				}
			}
			//zip!
			String AppDirectory = Path.Combine (GetTemporaryDirectory (), "Payload");
			Debug ("AppDirectory: " + AppDirectory);
			//return null;
			String ZipPath = Path.Combine (GetTemporaryDirectory (), "upload.ipa");
			using (ZipFile zip = new ZipFile()) {
				zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
				zip.AddDirectory (AppDirectory);
				zip.Save (ZipPath);
			}
			return ZipPath;
		}

		public String repack (IPAInfo info, String binaryLocation)
		{
			String finalLocation = info.AppName + " " + info.AppVersion + ".ipa";
			File.Copy (info.Location, finalLocation, true);
			using (ZipFile ipa = ZipFile.Read(finalLocation)) {
				Debug ("found binary!!");
				try { 

					var binary = ipa.AddFile (binaryLocation).FileName = info.BinaryLocation;
				}
				catch (System.ArgumentException) {
					//remove the existing binary and insert again :/
					foreach (ZipEntry e in ipa) {
						if (e.FileName == info.BinaryLocation) {
							//Console.WriteLine ("binarylocation: " +);
							Debug ("found binary!!!!");
							ipa.RemoveEntry (e);
							var binary = ipa.AddFile (binaryLocation).FileName = info.BinaryLocation;
							break;
						}
					}
				}

				Debug ("binarylocation: " + binaryLocation + " " + info.BinaryLocation);
				//binary.FileName = info.BinaryLocation;
				ipa.Save ();
			}
			return finalLocation;
		}

		private static string tempDir;

		public static string GetTemporaryDirectory ()
		{
			if (tempDir == null) {
				tempDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
				Directory.CreateDirectory (tempDir);
			}
			return tempDir;
		}

		public static void DeleteDirectory (string target_dir)
		{
			string[] files = Directory.GetFiles (target_dir);
			string[] dirs = Directory.GetDirectories (target_dir);

			foreach (string file in files) {
				File.SetAttributes (file, FileAttributes.Normal);
				File.Delete (file);
			}

			foreach (string dir in dirs) {
				DeleteDirectory (dir);
			}

			Directory.Delete (target_dir, false);
		}
	}
}

