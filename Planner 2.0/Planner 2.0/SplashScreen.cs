using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planner_2._0
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            this.TopMost = true;
            InitializeComponent();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

            switch (Properties.Settings.Default.Theme)
            {
                case "blue":
                    metroProgressSpinner1.BackColor = Color.FromArgb(0, 174, 219);
                    this.BackColor = Color.FromArgb(0, 174, 219);
                    break;
                case "orange":
                    metroProgressSpinner1.BackColor = Color.FromArgb(243, 119, 53);
                    this.BackColor = Color.FromArgb(243, 119, 53);
                    break;
                case "green":
                    metroProgressSpinner1.BackColor = Color.FromArgb(0, 177, 89);
                    this.BackColor = Color.FromArgb(0, 177, 89);
                    break;
                case "grey":
                    metroProgressSpinner1.BackColor = Color.FromArgb(85, 85, 85);
                    this.BackColor = Color.FromArgb(85, 85, 85);
                    break;
                case "pink": //
                    metroProgressSpinner1.BackColor = Color.FromArgb(231, 113, 189);
                    this.BackColor = Color.FromArgb(231, 113, 189);
                    break;
                case "red":
                    metroProgressSpinner1.BackColor = Color.FromArgb(209, 17, 65);
                    this.BackColor = Color.FromArgb(209, 17, 65);
                    break;
                case "yellow":
                    metroProgressSpinner1.BackColor = Color.FromArgb(255, 196, 37);
                    this.BackColor = Color.FromArgb(255, 196, 37);
                    break;
                case "purple":
                    metroProgressSpinner1.BackColor = Color.FromArgb(124, 65, 153);
                    this.BackColor = Color.FromArgb(124, 65, 153);
                    break;
                default:
                    metroProgressSpinner1.BackColor = Color.FromArgb(0, 174, 219);
                    this.BackColor = Color.FromArgb(0, 174, 219);
                    break;

            }
        }
    }
}
