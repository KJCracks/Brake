using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brake
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Load the XML
            Container xml = Container.getContainer();
			Console.WriteLine ("hello!");
            if (xml.Config.host == null)
            {
                //load the configuration form
                Application.Run(new ConfigForm());
            }
            else
            {
                Application.Run(new AppList());
            } 
        }
    }
}
