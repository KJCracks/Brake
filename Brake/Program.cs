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
			Console.WriteLine ("Welcome to Brake!");
			Console.WriteLine ("Current version: Brake-0.0.4");
			Container xml = Container.getContainer ();
			if (xml.Config.host == null) {
				xml.Config = new Configuration ();
				Console.Write ("IP address of your iDevice: ");
				xml.Config.host = Console.ReadLine ();
				Console.Write ("SSH Port: ");
				string portString = "22";
				portString = Console.ReadLine ();
				int.TryParse (portString, out xml.Config.port);
				Console.Write ("Root Password: ");
				xml.Config.Password = Console.ReadLine ();
				Console.WriteLine ("== Is this correct? Y/N ==");
				Console.WriteLine ("Host: " + xml.Config.host);
				Console.WriteLine ("Root Password: " + xml.Config.Password);
				string confirm = Console.ReadLine ();
				if (confirm != "y") {
					return;
				}
				xml.SaveXML ();
			}
			AppHelper appHelper = new AppHelper ();

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
	
			Console.WriteLine ("Establishing SSH connection");
			var connectionInfo = new PasswordConnectionInfo (xml.Config.host, xml.Config.port, "root", xml.Config.Password);
			using (var sftp = new SftpClient(connectionInfo)) {
				using (var ssh = new SshClient(connectionInfo)) {
					ssh.Connect ();
					sftp.Connect ();
					var whoami = ssh.RunCommand ("Clutch -b");
					long b;
					long.TryParse (whoami.Result, out b);
					/*if (b < 13104)
                    {
                        Console.WriteLine("You're using an old version of Clutch, please update to 1.3.1!");
                        //COMING SOON download Clutch to device for you
                        //Console.WriteLine("Would you like to download the latest version to your iDevice?");
                        //string dlyn = Console.ReadLine();
                        //if (dlyn == "y")
                        //{
                            //ssh.RunCommand("apt-get install wget");
                            //ssh.RunCommand("wget --no-check-certificate -O Clutch https://github.com/CrackEngine/Clutch/releases/download/1.3.1/Clutch");
                            //ssh.RunCommand("mv Clutch /usr/bin/Clutch");
                            //ssh.RunCommand("chown root:wheel /usr/bin/Clutch");
                            //ssh.RunCommand("chmod 755 /usr/bin/Clutch");
                        //}
                        //else if (dlyn == "Y")
                        //{
                            //ssh.RunCommand("apt-get install wget");
                            //ssh.RunCommand("wget --no-check-certificate -O Clutch https://github.com/CrackEngine/Clutch/releases/download/1.3.1/Clutch");
                            //ssh.RunCommand("mv Clutch /usr/bin/Clutch");
                            //ssh.RunCommand("chown root:wheel /usr/bin/Clutch");
                            //ssh.RunCommand("chmod 755 /usr/bin/Clutch");
                        //}
                        //else
                        //{
                            return;
                        //}
                    }*/
					Console.WriteLine ("reply: " + whoami.Result);
			
					//return;
					string location;
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
					appHelper.getIPAs (location);
					int i = 1;
					int a;
					while (true) {
						foreach (IPAInfo ipaInfo in xml.IPAItems) {
							Console.WriteLine (i + ". >> " + ipaInfo.AppName + " (" + ipaInfo.AppVersion + ")");
							i++;
						} 
						Console.WriteLine ("");
						Console.Write ("Please enter your selection:  ");
						if (int.TryParse (Console.ReadLine (), out a)) {
							try {
								IPAInfo ipaInfo = xml.IPAItems [a - 1];
								Console.WriteLine ("Cracking " + ipaInfo.AppName);
								String ipalocation = appHelper.extractIPA (ipaInfo);


								using (var file = File.OpenRead(ipalocation)) {
									Console.WriteLine ("Uploading IPA to device..");
									sftp.UploadFile (file, "Upload.ipa");

								}
						
								Console.WriteLine ("Cracking! (This might take a while)");
								String binaryLocation = ipaInfo.BinaryLocation.Replace ("Payload/", "");
								String TempDownloadBinary = Path.Combine (AppHelper.GetTemporaryDirectory (), "crackedBinary");
								var crack = ssh.RunCommand ("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
								Console.WriteLine ("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
								Console.WriteLine ("cracking output: " + crack.Result);
						
								using (var file = File.OpenWrite(TempDownloadBinary)) {
									Console.WriteLine ("Downloading cracked binary..");
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
}

