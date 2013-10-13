using System;
using System.IO;
using Ionic.Zip;

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
			FileInfo[] files;
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
						if (e.FileName.Contains (".sinf")) {
							//extract it 
						}
					}
				}
			}
			return true;
		}
	}
}

