/*code that handles task modification
 * - changing values
 * - deleting or modifing task
 * - viewing task
 */

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
    public partial class TaskInfo : Form
    {
        private string title;
        private string info;
        private DateTime startDate;
        private DateTime endDate;
        private int ind; //index of selected task
        private bool pinnedtask;
        //private Planner pl;
        //public Form form1;

        public TaskInfo()
        {
            InitializeComponent();

            //toolhelp.Show("apply", ApplyBtn);
            GetTheme t = new GetTheme();
            string theme = t.setTheme();
            Color themeC = new Color();
            switch (theme)
            {
                case "blue":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Blue;
                    themeC = Color.FromArgb(0, 174, 219);
                    break;
                case "orange":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Orange;
                    themeC = Color.FromArgb(243, 119, 53);
                    break;
                case "green":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Green;
                    themeC = Color.FromArgb(0, 177, 89);
                    break;
                case "grey":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Silver;
                    themeC = Color.FromArgb(85, 85, 85);
                    break;
                case "pink": //
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Pink;
                    themeC = Color.FromArgb(231, 113, 189);
                    break;
                case "red":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Red;
                    themeC = Color.FromArgb(209, 17, 65);
                    break;
                case "yellow":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Yellow;
                    themeC = Color.FromArgb(255, 196, 37);
                    break;
                case "purple":
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Purple;
                    themeC = Color.FromArgb(124, 65, 153);
                    break;
                default:
                    metroToggle1.Style = MetroFramework.MetroColorStyle.Blue;
                    themeC = Color.FromArgb(0, 174, 219);
                    break;

            }

            panel3.BackColor = themeC;
            panel2.BackColor = themeC;
            panel6.BackColor = themeC;
            ApplyBtn.BackColor = themeC;
            RemoveBtn.BackColor = themeC;
            CancelBtn.BackColor = themeC;
            panel1.BackColor = themeC;
            
        }

        public void setInfo(string t, string i, DateTime s, DateTime e, int index, bool pinned)
        {
            title = t;
            info = i;
            startDate = s;
            endDate = e;

            ind = index;

            //set gaps
            titleBox.Text = title;
            InfoBox.Text = info;
            startPicker.Value = startDate;
            EndPicker.Value = endDate;
            L1.Text = "";
            L2.Text = "";
            pinnedtask = pinned;

            metroToggle1.Checked = pinnedtask;
   
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            if (titleBox.Text != title || InfoBox.Text != info || startPicker.Value != startDate || EndPicker.Value != endDate || metroToggle1.Checked != pinnedtask)
            {
                DialogResult result = MessageBox.Show("Would you like to save your changes to this task?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //apply
                    ApplyBtn.PerformClick();
                }
                else if (result == DialogResult.No)
                {
                    this.Close();
                }
                else
                {
                    //cancel
                    return;
                }
            }
            else
            {
                this.Close();
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan totaltime;
            TimeSpan timeElapsed;
            TimeSpan timeLeft;

            TimeSpan timeUntilStart;

            int percent;
            double timePercent;

            if (startDate > DateTime.Now)
            {
                timeUntilStart = startDate - DateTime.Now;
                Percent.Text = "0%";
                TimeLeftText.Text = "Starts in: " + string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", timeUntilStart.Days, timeUntilStart.Hours, timeUntilStart.Minutes, timeUntilStart.Seconds);
            }
            else
            {
                //calculate the times required
                totaltime = endDate - startDate; //total time range

                timeElapsed = DateTime.Now - startDate; //elapsed time range
                timeLeft = endDate - DateTime.Now; //remaining time 

                //calculate percentages from our data
                timePercent = timeElapsed.Ticks / (double)totaltime.Ticks * 100;
                percent = (int)timePercent; //parses it into an integer value

                Percent.Text = percent.ToString() + "%";
                TimeLeftText.Text = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);

                if (percent >= 100 || percent < 0)
                {
                    percent = 100;
                    Percent.Text = "100%";
                    TimeLeftText.Text = "Time is up!";
                    //done


                }
            }
        }
        private void TaskInfo_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            
        }

        private void ApplyBtn_Click(object sender, EventArgs e) //make changes
        {
            //apply 
            if (titleBox.Text == "")
            {
                L1.Text = "Please enter a title";
            }
            else if (EndPicker.Value < startPicker.Value)
            {
                L2.Text = "Please enter a valid date";
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you will like to make changes to " + title + " ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //remove stuff
                    this.Close();
                    Planner.InfoApply = true; //directly references an active class | requests an apply
                    Planner.InfoIndex = ind; //infoindex is a reserved variable which can only be changed here

                    //overrides information
                    Planner.I_title = titleBox.Text;
                    Planner.I_end = EndPicker.Value;
                    Planner.I_info = InfoBox.Text;
                    Planner.I_start = startPicker.Value;
                    Planner.I_pinned = metroToggle1.Checked;
                }
                else if (result == DialogResult.No)
                {
                    return;
                }
            }


        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            //removes

            DialogResult result = MessageBox.Show("Are you sure you will like to remove " + title + " ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //remove stuff
                this.Close();
                Planner.Inforemove = true; //requests a remove
                Planner.InfoIndex = ind;
                

            }
            else if (result == DialogResult.No)
            {
                return;
            }

        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void titleBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
