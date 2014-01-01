using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Brake
{
    public partial class CrackProcess : Form
    {
        private static Container xml;
        private IPAInfo ipaInfo;

        delegate void LogCallback(string text);

        private void log(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (logBox.InvokeRequired)
            {
                LogCallback d = new LogCallback(log);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                logBox.AppendText(text + Environment.NewLine);
                Console.WriteLine("log: " + text);
            }
        }

        delegate void PercentStatusCallback(string text, int percent);

        private void PercentStatus(string text, int percent)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (logBox.InvokeRequired)
            {
                PercentStatusCallback d = new PercentStatusCallback(PercentStatus);
                this.Invoke(d, new object[] { text, percent });
            }
            else
            {
                statusLabel.Text = "status: " + text + " (" + percent + "%)";
                progressBar1.Value = percent;
            }
        }


        public void beginCracking()
        {
            log("beginning cracking process..");
            var connectionInfo = new PasswordConnectionInfo (xml.Config.host, xml.Config.port, "root", xml.Config.Password);
            using (var sftp = new SftpClient(connectionInfo))
            {
                using (var ssh = new SshClient(connectionInfo))
                {
                    PercentStatus("Establishing SSH connection", 5);
                    ssh.Connect();
                    PercentStatus("Establishing SFTP connection", 10);
                    sftp.Connect();
                    
                    log("Cracking " + ipaInfo.AppName);
                    PercentStatus("Preparing IPA", 25);
                    String ipalocation = AppHelper.extractIPA(ipaInfo);
                    using (var file = File.OpenRead(ipalocation))
                    {
                        log("Uploading IPA to device..");
                        PercentStatus("Uploading IPA", 40);
                        sftp.UploadFile(file, "Upload.ipa");

                    }
                    log("Cracking! (This might take a while)");
                    PercentStatus("Cracking", 50);
                    String binaryLocation = ipaInfo.BinaryLocation.Replace("Payload/", "");
                    String TempDownloadBinary = Path.Combine(AppHelper.GetTemporaryDirectory(), "crackedBinary");
                    var crack = ssh.RunCommand("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
                    log("Clutch -i 'Upload.ipa' " + binaryLocation + " /tmp/crackedBinary");
                    log("cracking output: " + crack.Result);

                    using (var file = File.OpenWrite(TempDownloadBinary))
                    {
                        log("Downloading cracked binary..");
                        PercentStatus("Downloading cracked binary", 80);
                        try
                        {
                            sftp.DownloadFile("/tmp/crackedBinary", file);
                        }
                        catch (SftpPathNotFoundException e)
                        {
                            log("Could not find file, help!!!!!");
                            return;
                        }
                    }

                    PercentStatus("Repacking IPA", 90);
                    String repack = AppHelper.repack(ipaInfo, TempDownloadBinary);
                    PercentStatus("Done!", 100);

                    log("Cracking completed, file at " + repack);	
                }
            }
        }
        
        public CrackProcess(IPAInfo ipaInfo)
        {
            xml = Brake.Container.getContainer();
            this.ipaInfo = ipaInfo;
            InitializeComponent();
        }
    }
}
