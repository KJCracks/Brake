using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Brake
{

	public class MainClass
	{
		public const int DEBUG = 1;
		public enum Platform
		{
			Windows,
			Linux,
			Mac
		}

		public static Platform RunningPlatform()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Unix:
				// Well, there are chances MacOSX is reported as Unix instead of MacOSX.
				// Instead of platform check, we'll do a feature checks (Mac specific root folders)
				if (Directory.Exists("/Applications")
				    & Directory.Exists("/System")
				    & Directory.Exists("/Users")
				    & Directory.Exists("/Volumes"))
					return Platform.Mac;
				else
					return Platform.Linux;

				case PlatformID.MacOSX:
				return Platform.Mac;

				default:
				return Platform.Windows;
			}
		}
		[STAThread]
		public static void Main (string[] args) {
			String location;
			AppHelper appHelper = new AppHelper ();
			Console.WriteLine ("Loading");
			switch (RunningPlatform ()) {
			case Platform.Mac:
				{
					location = Environment.GetEnvironmentVariable ("HOME") + "/Music/iTunes/iTunes Media/Mobile Applications";
					break;
				}
                    //windows suc
            case Platform.Windows:
                {
                    string location2 = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    location = Path.Combine(location2, "iTunes\\iTunes Media\\Mobile Applications");
                    break;
                }
			default:
				{
					Console.WriteLine ("Unknown operating system!");
					return;
				}
			}
			Container IPAs = appHelper.getIPAs (location);
			int i = 1;
			int a;
			foreach (IPAInfo ipaInfo in IPAs.Items) {
				Console.WriteLine (i + ". >> " + ipaInfo.AppName + " (" + ipaInfo.AppVersion + ")");
				i++;
			} 
			Console.WriteLine ("");
			Console.Write ("Please enter your selection:  ");
			if (int.TryParse(Console.ReadLine(), out a)) {
				try {
					IPAInfo ipaInfo = IPAs.Items [a - 1];
					Console.WriteLine ("Cracking " + ipaInfo.AppName);
				}
				catch (IndexOutOfRangeException) {
					Console.WriteLine ("Invalid input, out of range");
				}
			} else {
				Console.WriteLine ("Invalid input");
			}

		}
	}
}
