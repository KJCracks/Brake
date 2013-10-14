using System;
using System.IO;
using System.Windows.Forms;
using Renci.SshNet;
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
			Console.WriteLine ("Establishing ssh connection");
			var connectionInfo = new PasswordConnectionInfo ("192.168.1.208", "root", "password");
			using (var ssh = new SshClient(connectionInfo))
			{
				ssh.Connect();
				var whoami = ssh.RunCommand ("Clutch");
				Console.WriteLine ("reply: " + whoami.Result);
			}
			return;
			switch (RunningPlatform ()) {
			case Platform.Mac:
				{
					location = Environment.GetEnvironmentVariable ("HOME") + "/Music/iTunes/iTunes Media/Mobile Applications";
					break;
				}
			case Platform.Windows: {
				//windows sucks
				FolderBrowserDialog fbd = new FolderBrowserDialog ();
				DialogResult result = fbd.ShowDialog();

				string[] files = Directory.GetFiles(fbd.SelectedPath);
				System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");

				Console.WriteLine("PATH: " + files[0]);
					location = files [0];
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
