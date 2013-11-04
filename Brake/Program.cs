using System;
using System.Net;
using System.IO;
using System.Windows.Forms;
using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

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

		public static Platform RunningPlatform ()
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Unix:
				// Well, there are chances MacOSX is reported as Unix instead of MacOSX.
				// Instead of platform check, we'll do a feature checks (Mac specific root folders)
				if (Directory.Exists ("/Applications")
				    & Directory.Exists ("/System")
				    & Directory.Exists ("/Users")
				    & Directory.Exists ("/Volumes"))
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
		public static void Main (string[] args)
		{
			String location;
			AppHelper appHelper = new AppHelper ();
			Console.WriteLine ("IP address of your iDevice: ");
			string host = Console.ReadLine();
			string portString = "22";
			int port;
			int.TryParse (portString, out port);
            //COMING SOON PORT VERIFICATION
            //var ping = new Ping();
            //var reply = ping.Send(host); // 1 minute time out (in ms)
            //if (reply.Status == IPStatus.Success)
            //{
            //    Console.WriteLine("IP Address Valid");
            //}
            //else
            //{
            //    Console.WriteLine("Unable to SSH to IP");
            //}
			string user = "root";
			Console.WriteLine ("Root Password: ");
			string pass = Console.ReadLine ();
            Console.WriteLine("Is this correct? Y/N");
            Console.WriteLine("Host:" + host);
            Console.WriteLine("Root Password:" + pass);
            string confirm = Console.ReadLine();
            if (confirm != "y")
            {
                return;
            }
			Console.WriteLine ("Establishing SSH connection");
			var connectionInfo = new PasswordConnectionInfo (host, port, user, pass);
			using (var sftp = new SftpClient(connectionInfo)) {
				using (var ssh = new SshClient(connectionInfo)) {
                    ssh.Connect ();
					sftp.Connect ();
					var whoami = ssh.RunCommand ("Clutch -b");
					long b;
					long.TryParse (whoami.Result, out b);
                    if (b != 13104)
                    {
                        Console.WriteLine("You're using an old version of Clutch, please update to 1.3.1");
                        //COMING SOON download Clutch to device for you
                        //Console.WriteLine("Would you like it downloading to your device?");
                        //string dlyn = Console.ReadLine();
                        //if (dlyn == "y")
                        //{
                            //WebClient webClient = new WebClient();
                        //webClient.DownloadFileAsync(new Uri("https://github.com/aaapps/Clutch/releases/download/1.3.1/Clutch"), @"c:\Clutch");
                        //sftp.UploadFile ("/usr/bin/Clutch, "c:\Clutch");
                        //}
                        //else if (dlyn == "Y")
                        //{
                            //WebClient webClient = new WebClient();
                        //webClient.DownloadFileAsync(new Uri("https://github.com/aaapps/Clutch/releases/download/1.3.1/Clutch"), @"c:\Clutch");
                        //sftp.UploadFile ("/usr/bin/Clutch, "c:\Clutch");
                        //}
                        //else
                        //{
                            //return;
                        //}
                    }
					Console.WriteLine ("reply: " + whoami.Result);
			
					//return;
					switch (RunningPlatform ()) {
					case Platform.Mac:
						{
							location = Environment.GetEnvironmentVariable ("HOME") + "/Music/iTunes/iTunes Media/Mobile Applications";
							break;
						}
					case Platform.Windows:
						{
							string location2 = Environment.GetFolderPath (Environment.SpecialFolder.MyMusic);
							location = Path.Combine (location2, "iTunes\\iTunes Media\\Mobile Applications");
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
					if (int.TryParse (Console.ReadLine (), out a)) {
						try {
							IPAInfo ipaInfo = IPAs.Items [a - 1];
							Console.WriteLine ("Cracking " + ipaInfo.AppName);
							String ipalocation = appHelper.extractIPA (ipaInfo);


							using (var file = File.OpenRead(ipalocation)) {
								Console.WriteLine ("Uploading IPA to device..");
								sftp.UploadFile (file, "Upload.ipa");

							}
						
							Console.WriteLine ("Cracking!!");
							String binaryLocation = ipaInfo.BinaryLocation.Replace ("Payload/", "");
							String TempDownloadBinary = Path.Combine (AppHelper.GetTemporaryDirectory (), "crackedBinary");
							var crack = ssh.RunCommand ("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
							Console.WriteLine ("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
							Console.WriteLine ("cracking output: " + crack.Result);
						
							using (var file = File.OpenWrite(TempDownloadBinary)) {
								Console.WriteLine("Downloading cracked binary..");
								sftp.DownloadFile ("/tmp/crackedBinary", file);
							}

							String repack = appHelper.repack (ipaInfo, TempDownloadBinary);
							Console.WriteLine ("Cracking completed, file at " + repack);
						} catch (IndexOutOfRangeException) {
							Console.WriteLine ("Invalid input, out of range");
							return;
						}
					} else {
						Console.WriteLine ("Invalid input");
						return;
					}

					AppHelper.DeleteDirectory (AppHelper.GetTemporaryDirectory ());
					sftp.Disconnect ();
				}
			}
		}
	}
}

