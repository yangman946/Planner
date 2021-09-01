using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;


namespace Planner_2._0
{
    static class Program
    {

        //[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
        //static Form splash;
        static Planner mainForm;

        static string[] argsVar;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);




            

            if (args.Length != 0)
            {


                argsVar = args;
                //Application.Run(new Planner());
                mainForm = new Planner();

                Application.Run(mainForm);
                mainForm.openstring = argsVar[0]; // set string
                mainForm.openfromFile();

            }
            else
            {
                mainForm = new Planner();
                
                Application.Run(mainForm);
            }

            



        }


    }

}
