using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planner_2._0
{
    class GetTheme
    {
        private string colorType = "blue";
        
        
        /*
        public string getString()
        {
            return colorType;
        }
        */

        
        public string setTheme()
        {
            if (Properties.Settings.Default.Theme == "" || Properties.Settings.Default.Theme == null)
            {
                colorType = "blue";
                return colorType;
            }
            else
            {
                return Properties.Settings.Default.Theme;
            }


            /*
            switch (Properties.Settings.Default.Theme)
            {
                case "blue":
                  
                    break;
                case "orange":
           
                    break;
                case "green":
                    break;
                case "grey":
                    break;
                default:
                    break;
                 
            }
            */
        }
    }
}
