/* Planner 2.0 by clarence yang 19/4/2019 Copyright 2020, clarence yang, All rights reserved.
 * this is the second version of my original attempt to create a planner, which failed. (i just gave up on it)
 * reasons for making this program:
 *  - im bored 
 *  - i need a way to manage by tasks and also help other people to do so
 *  - to fix a common issue ^ above
 *  Note: ignore regions, they are mainly unchanged, DO NOT DELETE 
 *  
 *  
 *  todo:
 *  - review all code and clean up: clean spaget code
 *  - fix the recent panel
 *  - fix pinned tasks
 *  - add setup release
 *  - fix all bugs for release
 *  
 *  log:
 *  - 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Xml.Serialization;


namespace Planner_2._0
{
    public partial class Planner : Form
    {
        #region version stuff, change everytime theres a new version
        // version stuff, important, change this everytime theres a new version
        public const int versionNumber = 211; // current version
        public string link = "https://drive.google.com/drive/folders/1s8---hX7PNRC-whkjVFzDYGladvnVTGH"; //link to new version, ignore contents, just placeholder
        public string tempVersionNumber; //collected version for newest version as string
        public string[] receivedOut; //output, version and link; also other commands
        string formattedNumb;
        public string command = "noValue";
        public const bool isMasterProgram = false; //admin use
        #endregion  

        //update checks
        public bool uptodate = true;



        #region recent stuff
        //recent stuff
        private string RecentRootDir = @"C:\Planner2.0\planner data";
        private bool NewProgram = false;
        BinaryFormatter formatter;
        RecentHandler RH;
        private List<string> FormListNames = new List<string>();
        private List<string> FormListDir = new List<string>();
        //private string RecentRootPath;
        Stream recentstream;

        public bool clearRecent = false;
        //
        #endregion
        public Color defaultC = Color.Black;
        //prioritise task
        public bool ispinned = false;

        private bool hasOpened = false;
        //private Panel primaryPanel = null; //the main panel which is our project
        private string ProjectTitle = "Untitled Plan";

        private bool hasExit = false;
        private bool pressedX = false;

        public bool isDarkmode = false;
        //checks if the user has opened any file yet

        #region info variables for the information class
        //info variables, determines information stuff
        public static bool Inforemove = false;
        public static bool InfoApply = false;
        public static int InfoIndex; //index of seleted task

        public static string I_title;
        public static string I_info;
        public static DateTime I_start;
        public static DateTime I_end;
        public static bool I_pinned;
        //============================================
        #endregion
        private bool saved = false; // <===== check if the plan is ever saved, (DOESNT CHECK IF ANY CHANGES ARE MADE)

        #region save stuff
        //also save stuff
        private string SavedPath;
        public static string newName;
        public static string newOpenFileName;
        DateTime savedTime;

        //save stuff
        SaveLoad SL;
        Stream stream;
        BinaryFormatter bf;
        #endregion

        #region state checks, check if changes are ever made
        // state checks <============ can be cleared using separate function (these are used to check if any changes are made when the user decides to save, etc.
        List<string> StartTitles = new List<string>();
        List<string> StartInfo = new List<string>();
        List<DateTime> StateStartTime = new List<DateTime>();
        List<DateTime> StateEndTime = new List<DateTime>();
        List<bool> StatePinned = new List<bool>();
        int totalControls = 0;
        // ==========================
        #endregion

        #region logic lists and project variables
        //function lists: for main logic and task creating
        List<int> newIndex = new List<int>(); //the indexes <--for sorting the  sort by

        //lists 
        List<Control> ProjectPanelControls = new List<Control>(); //controls
        //List<ProgressBar> ProgressBars = new List<ProgressBar>();
        List<GroupBox> groupBoxes = new List<GroupBox>();

        //list of strings for saving if the user decides to save
        List<string> TaskTitle = new List<string>();
        List<string> TaskInformation = new List<string>();

        List<DateTime> startTimerDates = new List<DateTime>(); //date calculations
        List<DateTime> endTimerDates = new List<DateTime>();

        List<bool> pinnedT = new List<bool>();

        //List<int> TimePercentage = new List<int>();
        //List<TimeSpan> remaingTimerTimes = new List<TimeSpan>();

        //all values (mainly for resetting) 
        string title = "";
        string info = "";
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;

        DateTime val1; //reset datetime values (DO NOT USE)
        DateTime val2;
        #endregion
        //panels
        Panel newProject = new Panel();
        Panel defaultProject = null;

        #region ramcleaner
        //ram cleaners
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize32Bit (IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize64Bit (IntPtr pProcess, long dwMinimumWorkingSetSize, long dwMaximumWorkingSetSize);
        #endregion

        //for saved status
        public DateTime saveddate;
        public TimeSpan savemin;
        //public bool quickopen = false;
        public string openstring = "";


            



        //}

        public Planner() //initiate
        {   
            InitializeComponent();

        }

        public void openfromFile()
        {
            QuickOpen(openstring);
        }



        private void Planner_Load(object sender, EventArgs e) //what to do on start
        {
            //TopMost = true;
            this.Activate();
            hidepanels(); //hide panels
            initiateSettings();
            //we are just getting the amounts of tasks by using the number of start timer dates
            TaskCount.Text = formatTaskText(startTimerDates.Count);
            //reset
            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(comboBox1, EventArgs.Empty);
            //reset flowlayout panel
            List<Control> C = flowLayoutPanel1.Controls.Cast<Control>().ToList();
            foreach (Control control in C)
            {
                // do i need to remove the values from groupboxes[] ***
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }

            hasExit = false;
            pressedX = false;

            saved = false;

            Inforemove = false;
            InfoApply = false;

            panel1.Show();

            //ui updates
            //label3.BackColor = Color.Transparent;
            //label15.BackColor = Color.Transparent;
            //label4.BackColor = Color.Transparent;
            //label5.BackColor = Color.Transparent;
            label20.BackColor = Color.Transparent;

            NewProgram = CheckRecent(); //exists or not exists
            applyrecent();

            //check time
            checkTime();

            label11.Text = "";
            label12.Text = "";

            label13.Text = "Unsaved";

            bf = new BinaryFormatter();
            title = "";
            info = "";
            start = DateTime.Now;
            end = DateTime.Now;

            hasOpened = false;
            //panels
            defaultProject = project;
            newProject = project;
            setTheme();
            darkmode(); //theme checks

            resetState();
            setStates();

            receivedOut = getNewsInfo(); //////////////////////////////
            
            tempVersionNumber = receivedOut[0];
            link = receivedOut[1];
            command = receivedOut[2];

            setProjectControls();





        }

        string formatTaskText(int count)
        {
            string S = "";
            if (count < 0)
            {
                S = "No Tasks";
            }
            else if (count == 1)
            {
                S = "1 Task";

            }
            else
            {
                S = count.ToString() + " Tasks";
            }

            return S;
        }

        void hidepanels()
        {
            about.Hide(); //hide the panels
            help.Hide();
            themePanel.Hide();
            Options.Hide();

            //some other initiation: should not apply on start
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false; 
        }

        void initiateSettings()//initiate values
        {
            if (Properties.Settings.Default.notif)
            {
                metroCheckBox2.Checked = true;

            }

            if (Properties.Settings.Default.autoUpdate)
            {
                metroCheckBox3.Checked = true;
            }

            if (Properties.Settings.Default.Datetimestyle)
            {
                metroComboBox1.SelectedIndex = 0;
            }
            else
            {
                metroComboBox1.SelectedIndex = 1;
            }


            
            if (Properties.Settings.Default.AutoScroll)
            {
                metroCheckBox4.Checked = true;
            }

                

            metroTextBox1.Text = Properties.Settings.Default.defPath;

            RecentRootDir = Properties.Settings.Default.defPath;

            if (Properties.Settings.Default.autoSave || Properties.Settings.Default.autoSave1)
            {
                metroCheckBox1.Checked = true;
                metroRadioButton1.Enabled = true;
                metroRadioButton2.Enabled = true;
                if (Properties.Settings.Default.autoSave)
                {
                    numericUpDown1.Enabled = true;
                    metroRadioButton1.Checked = true;
                    metroRadioButton2.Checked = false;
                }
                else if (Properties.Settings.Default.autoSave1)
                {
                    //
                    numericUpDown1.Enabled = false;
                    metroRadioButton2.Checked = true;
                    metroRadioButton1.Checked = false;
                }

            }
            else //no auto save
            {
                numericUpDown1.Enabled = false;
                metroRadioButton1.Enabled = false;
                metroRadioButton2.Enabled = false;
                metroCheckBox1.Checked = false;
            }
            numericUpDown1.Value = Properties.Settings.Default.minSave;
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Update();
            checkVersion(); //check for updates
        }

        public void checkTime()
        {
            
            DateTime n = DateTime.Now;
            //MessageBox.Show(n.Hour.ToString());
            if (n.Hour >= 0 && n.Hour < 4) //midnight to 4am
            {
                label16.Text = "Welcome back";
            }
            else if (n.Hour >= 4 && n.Hour < 12) //4am to 12pm
            {
                label16.Text = "Good morning";
            }
            else if (n.Hour >= 12 && n.Hour < 18) //12pm to 6pm
            {
                label16.Text = "Good afternoon";
            }
            else if (n.Hour >= 18 && n.Hour < 24 ) //6pm to 12am
            {
                label16.Text = "Good evening";
            }
            else
            {
                label16.Text = "Welcome back";
            }
        }
        public void setTheme()
        {
            GetTheme t = new GetTheme();
            string theme = t.setTheme();
            Color themeC = new Color();

            MetroFramework.MetroColorStyle m = MetroFramework.MetroColorStyle.Blue;

            switch (theme)
            {
                case "blue":
                    themeC = Color.FromArgb(0, 174, 219);
                    m = MetroFramework.MetroColorStyle.Blue;
                    break;
                case "orange":
                    themeC = Color.FromArgb(243, 119, 53);
                    m = MetroFramework.MetroColorStyle.Orange;

                    break;
                case "green":
                    themeC = Color.FromArgb(0, 177, 89);
                    m = MetroFramework.MetroColorStyle.Green;
                    break;
                case "grey":
                    themeC = Color.FromArgb(85, 85, 85);
                    m = MetroFramework.MetroColorStyle.Silver;
                    break;
                case "pink": //
                    themeC = Color.FromArgb(231, 113, 189);
                    m = MetroFramework.MetroColorStyle.Pink;
                    break;
                case "red":
                    themeC = Color.FromArgb(209, 17, 65);
                    m = MetroFramework.MetroColorStyle.Red;
                    break;
                case "yellow":
                    themeC = Color.FromArgb(255, 196, 37);
                    m = MetroFramework.MetroColorStyle.Yellow;
                    break;
                case "purple":
                    themeC = Color.FromArgb(124, 65, 153);
                    m = MetroFramework.MetroColorStyle.Purple;
                    break;
                default:
                    themeC = Color.FromArgb(0, 174, 219);
                    m = MetroFramework.MetroColorStyle.Blue;

                    break;

            }

            metroTabControl1.Style = m;
            Darktoggle.Style = m;
            metroToggle1.Style = m;
            metroCheckBox1.Style = m; 
            metroCheckBox2.Style = m;

            metroCheckBox4.Style = m;
            metroRadioButton1.Style = m;
            metroRadioButton2.Style = m;
            metroCheckBox3.Style = m;
            metroComboBox1.Style = m;
            #region tile styles
            metroTile5.Style = m;
            metroTile6.Style = m;
            metroTile7.Style = m;
            metroTile8.Style = m;
            #endregion


            applytheme(themeC);
        }

        public void applytheme(Color c)
        {
            GetTheme t = new GetTheme();
            string theme = t.setTheme();
            panel13.BackColor = c;
            button4.BackColor = c;
            button5.BackColor = c;
            panel8.BackColor = c;
            button2.BackColor = c;
            button9.BackColor = c;
            button10.BackColor = c;
            button16.BackColor = c;
            linkLabel3.LinkColor = c;

            //groupbox themes
            for (int i = 0; i < startTimerDates.Count; i++)
            {
                var progbar = groupBoxes[i].Controls.OfType<ProgressBar>();
                foreach (MetroFramework.Controls.MetroProgressBar item in progbar)
                {
                    switch (theme)
                    {
                        case "blue":
                            item.Style = MetroFramework.MetroColorStyle.Blue;
                            break;
                        case "orange":
                            item.Style = MetroFramework.MetroColorStyle.Orange;
                            break;
                        case "green":
                            item.Style = MetroFramework.MetroColorStyle.Green;
                            break;
                        case "grey":
                            item.Style = MetroFramework.MetroColorStyle.Silver;
                            break;
                        case "pink": //
                            item.Style = MetroFramework.MetroColorStyle.Pink;
                            break;
                        case "red":
                            item.Style = MetroFramework.MetroColorStyle.Red;
                            break;
                        case "yellow":
                            item.Style = MetroFramework.MetroColorStyle.Yellow;
                            break;
                        case "purple":
                            item.Style = MetroFramework.MetroColorStyle.Purple;
                            break;
                        default:
                            item.Style = MetroFramework.MetroColorStyle.Blue;
                            break;

                    }
                }

                if (pinnedT[i])
                {
                    groupBoxes[i].ForeColor = c;
                    var info = groupBoxes[i].Controls.OfType<RichTextBox>();
                    foreach(RichTextBox text in info)
                    {
                        text.ForeColor = c;
                    }


                }
            }
        }



        //currently unavaliable
        public void darkmode()
        {
            if (Properties.Settings.Default.darkmode)
            {
                //base
                //menuStrip1.BackColor = SystemColors.ControlDarkDark;

                //theme panel
                label2.ForeColor = SystemColors.ControlLightLight;
                themePanel.BackColor = SystemColors.ControlDarkDark;
                metroLabel5.ForeColor = SystemColors.ControlLightLight;
                metroLabel5.BackColor = SystemColors.ControlDarkDark;
                metroLabel9.BackColor = SystemColors.ControlDarkDark;
                metroLabel9.ForeColor = SystemColors.ControlLightLight;
                button8.ForeColor = SystemColors.ControlLightLight;
                //Darktoggle.Style = MetroFramework.MetroColorStyle.White;
                Darktoggle.Checked = true;


                //help panel
                help.BackColor = SystemColors.ControlDarkDark;
                label17.BackColor = SystemColors.ControlDarkDark;
                label17.ForeColor = SystemColors.ControlLightLight;
                metroLabel2.BackColor = SystemColors.ControlDarkDark;
                metroLabel2.ForeColor = SystemColors.ControlLightLight;
                metroLabel7.BackColor = SystemColors.ControlDarkDark;
                metroLabel7.ForeColor = SystemColors.ControlLightLight;
                panel10.BackColor = SystemColors.ControlDarkDark;
                button6.ForeColor = SystemColors.ControlLightLight;
                metroLabel6.BackColor = SystemColors.ControlDarkDark;
                metroLabel6.ForeColor = SystemColors.ControlLightLight;
                metroLabel1.BackColor = SystemColors.ControlDarkDark;
                metroLabel1.ForeColor = SystemColors.ControlLightLight;
                metroLabel8.BackColor = SystemColors.ControlDarkDark;
                metroLabel8.ForeColor = SystemColors.ControlLightLight;
                metroTabPage1.BackColor = SystemColors.ControlDarkDark;
                metroTabPage2.BackColor = SystemColors.ControlDarkDark;
                metroTabPage3.BackColor = SystemColors.ControlDarkDark;
                metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Dark;


     

            }
            else
            {
                //base
                //menuStrip1.BackColor = SystemColors.ControlLightLight;

                //theme
                themePanel.BackColor = SystemColors.ControlLightLight;
                label2.ForeColor = SystemColors.ActiveCaptionText;
                metroLabel5.ForeColor = SystemColors.ActiveCaptionText;
                metroLabel9.ForeColor = SystemColors.ActiveCaptionText;
                metroLabel5.BackColor = SystemColors.ControlLightLight;
                metroLabel9.BackColor = SystemColors.ControlLightLight;
                button8.ForeColor = SystemColors.ActiveCaptionText;
                Darktoggle.Style = MetroFramework.MetroColorStyle.Black;
                Darktoggle.Checked = false;


                //help panel
                help.BackColor = SystemColors.ControlLightLight;
                label17.BackColor = SystemColors.ControlLightLight;
                label17.ForeColor = SystemColors.ControlDarkDark; //
                metroLabel2.BackColor = SystemColors.ControlLightLight;
                metroLabel2.ForeColor = SystemColors.ControlDarkDark;//
                metroLabel7.BackColor = SystemColors.ControlLightLight;
                metroLabel7.ForeColor = SystemColors.ControlDarkDark;//
                panel10.BackColor = SystemColors.ControlLightLight;
                button6.ForeColor = SystemColors.ControlDarkDark;//
                metroLabel6.BackColor = SystemColors.ControlLightLight;
                metroLabel6.ForeColor = SystemColors.ControlDarkDark;//
                metroLabel1.BackColor = SystemColors.ControlLightLight;
                metroLabel1.ForeColor = SystemColors.ControlDarkDark;//
                metroLabel8.BackColor = SystemColors.ControlLightLight;
                metroLabel8.ForeColor = SystemColors.ControlDarkDark;//
                metroTabPage1.BackColor = SystemColors.ControlLightLight;
                metroTabPage2.BackColor = SystemColors.ControlLightLight;
                metroTabPage3.BackColor = SystemColors.ControlLightLight;
                metroTabControl1.Theme = MetroFramework.MetroThemeStyle.Light;

            }
        }

        // MENUSTRIP INPUT ================================================================================================================== 
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e) //about
        {
           
            about.Show();
            bringFront(about);
        }

        private void button9_Click(object sender, EventArgs e)
        {

            about.Show();
            bringFront(about);
        }
        private void plannerDocumentationToolStripMenuItem_Click(object sender, EventArgs e) //help
        {
            help.Show();
            help.BringToFront();
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            themePanel.Show();
            themePanel.BringToFront();
        }

        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e) //options
        {
            initiateSettings();
            Options.Show();
            Options.BringToFront();
        }

        


        private void newToolStripMenuItem_Click(object sender, EventArgs e) //new
        {
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            // ADD SAVE CHECK if the user has saved their work and decided to make a new project *
            hideAll();

            
            


            if (hasOpened) //if a existing nonsaved project is opened
            {
                //check for user change...





                resetValues(project); //reset all
                project.Hide();

 
                project.Controls.Clear();
                for (int i = 0; i < ProjectPanelControls.Count; i++)
                {
                    project.Controls.Add(ProjectPanelControls[i]);

                }

                //sync time
                setTime();
 
                project.Show();
                
            }
            else //if it is a new project
            {
                //works
                

                project.Show();
                hasOpened = true;
                setTime();
                //set states already called in load function
            }
            saveTimer.Enabled = false;
            timer_elapsed.Enabled = false;
            TaskCount.Text = formatTaskText(startTimerDates.Count);

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) //save*
        {
            //save everything in the flow layout panel as well as the values for starttimerdates and endtimerdates arrays
            if (hasOpened) //the plan is opened
            {
                if (saved == true)
                {
                    //the user had already saved their work and doesnt want to saveas to save it again
                    QuickSave(SavedPath); //save (not save as)
                }
                else if (saved == false)
                {
                    save(); //save as
                }
            }


        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) //save as
        {
            if (hasOpened)
            {
                save();

                
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) //open / load
        {
            //check if the user has any existing work open and would wish to save it*
            
            bool checksaved = false;
            if (hasOpened)
            {
                
                if (checkChanges() == true) //check if this is not equal to last load  *
                {

                    //notsaved = true;

                    DialogResult result = MessageBox.Show("Would you like to save your changes to " + ProjectTitle + " ?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // user clicked yes
                        if (saved == true)
                        {
                            QuickSave(SavedPath);
                        }
                        else
                        {
                            save();
                        }
                        
                        checksaved = true; //remove everything
                    }
                    else if (result == DialogResult.No)
                    {
                        // user clicked no
                        checksaved = true;
                    }
                    else
                    {
                        //cancel
                        return;
                    }
                }
                else
                {
                    checksaved = true;
                }
            }
            else
            {
                checksaved = true;
            }

            if (checksaved)
            {
                setValues();
                open();
            }
            else { return; }
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) //exit
        {
            hasExit = false;
            if (hasOpened)
            {
                if (checkChanges() == true) //check if this is not equal to last load  *
                {

                    //notsaved = true;

                    DialogResult result = MessageBox.Show("Would you like to save your changes to " + ProjectTitle + " ?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // user clicked yes

                        if (saved == true)
                        {
                            QuickSave(SavedPath);
                        }
                        else
                        {
                            save();
                        }

                    }
                    else if (result == DialogResult.No)
                    {
                        // user clicked no
                        //ignore

                    }
                    else
                    {
                        hasExit = false;
                        //cancel
                        return;
                    }
                }

            }
            hasExit = true;
            if (pressedX)
            {
                //ignore
                pressedX = false;
            }
            else
            {
                Application.Exit(); //exit
            }
            
        }
        // ================================================================================================================================== 

        // Important functions !!!!!!! ------------------------------------------------------------------------------------------------------

        void hideAll() //hide all controls/panels when switching between panels (applies mainly to the primary panel) DO NOT USE ON SECONDARY PANELS
        {
            //if (primaryPanel != null) { primaryPanel.Hide(); }
            about.Hide();
            panel1.Hide(); //start
            help.Hide();
            Options.Hide();
        }

        void setValues() //set these whenever we make a new project
        {
            dateTimePicker1.Value = DateTime.Now;
            val1 = dateTimePicker1.Value;

            dateTimePicker2.Value = DateTime.Now;
            val2 = dateTimePicker2.Value;

            label11.Text = "";
            label12.Text = "";
        }


        void setTime() //sets time for both date time pickers, call on open
        {
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker2.MinDate = DateTime.Now;
            val2 = dateTimePicker2.Value;

            dateTimePicker1.Value = DateTime.Now;
            //dateTimePicker1.MinDate = DateTime.Now;
            val1 = dateTimePicker1.Value;
        }

        public bool CheckRecent()
        {
            //checks for folder in documents and checks for file named: PlannerRecentFiles.
            //if no such folder exists, create that folder, ignore the file for now.
            if (!Directory.Exists(RecentRootDir)) //if it doesnt exist
            {
                Directory.CreateDirectory(RecentRootDir); //create new one

                return true; //doesnt exist
            }

            return false; //exists
        }

        void setRecent() //essentially saves info, is saved during the app run, not seen on home screen
        {
            if (clearRecent)
            {
                return;
            }
            
            string[] targetFile = { RecentRootDir, "PlannerRecentFiles.plData" };
            string targetPath = Path.Combine(targetFile);

            if (!File.Exists(targetPath)) //if the file doesnt exist
            {
                //MessageBox.Show(targetPath);
                //create file in that path
                formatter = new BinaryFormatter();
                //string FileName = "PlannerRecentFiles";

                //string newFileName = Path.ChangeExtension(FileName, ".plData"); //set its name

                //RecentRootPath = newFileName;
                RH = new RecentHandler(FormListNames.ToArray(), FormListDir.ToArray()); //serialize, we will get the names of all the tasks and their directories
                // Code to write the stream goes here.
               

                recentstream = File.Open(targetPath, FileMode.Create);
                formatter.Serialize(recentstream, RH);
                recentstream.Close();
            }
            else //if the file already exists
            {
                //begin to save items into the file
                RH = new RecentHandler(FormListNames.ToArray(), FormListDir.ToArray()); //serialize, we will get the names of all the tasks and their directories
                formatter = new BinaryFormatter();

                recentstream = File.Open(targetPath, FileMode.Create);
                formatter.Serialize(recentstream, RH); //apply 
                recentstream.Close();
            }
        }

        void applyrecent() //only occurs on start
        {
            string[] targetFile = { RecentRootDir, "PlannerRecentFiles.plData" };
            string targetPath = Path.Combine(targetFile);
            if (!NewProgram) //already exists
            {
                //setRecent();
                TextDirect.Visible = false;
             
                if (!File.Exists(targetPath)) //no file
                {
                    TextDirect.Visible = true;
             
                    setRecent(); //if it doesnt exist, we will check in another class
                    return;
                }
                //TextDirect.Visible = false;
                loadRecents(); //get our recent stuff
                //

                if (FormListDir.Count == 0) //no results <-- file exits but is empty
                {
                    TextDirect.Visible = true;
                  
                    return;
                }

                //if the file to get results from already exists,
                //extracts data and outputs it into the recent panel
                //make a new class
                RecentCreator RC = new RecentCreator();


                for (int i = FormListDir.Count; i --> 0;)
                {
                    try
                    {
                        GroupBox box = RC.getRecentOBJ(FormListNames[i], FormListDir[i]); //
                        flowLayoutPanel2.Controls.Add(box);


                        List<Control> controls = box.Controls.Cast<Control>().ToList();

                        foreach (Control ctrl in controls)
                        {

                            if (ctrl.Name == "recentBTN") //assigns event to this button
                            {
                                ctrl.Click += new EventHandler(openRecent);
                            }
                        }
                    }
                    catch
                    {

                    }

                }


            }
            else
            {
                TextDirect.Visible = true;

            }
        }

        void loadRecents() //loads all recents
        {
            
            string[] targetFile = { RecentRootDir, "PlannerRecentFiles.plData" };
            string targetPath = Path.Combine(targetFile);
            recentstream = File.Open(targetPath, FileMode.Open);
            formatter = new BinaryFormatter();

            RH = (RecentHandler)formatter.Deserialize(recentstream);


            FormListNames = RH.getNames().ToList<string>();
            FormListDir = RH.getdirectory().ToList<string>();
            //MessageBox.Show(RH.getNames()[0]);

            recentstream.Close();

            
        }


        void bringFront(Panel p) //brings some panels to the front incase you dont want to delete panels
        {
            p.BringToFront(); //brings in fron panels, just incase we dont want to remove the other ones them.
        }

        void setProjectControls() 
        {


            foreach  (Control ctrl in project.Controls)
            {
                ProjectPanelControls.Add(ctrl);
            }



            //works
        } // this resets the form textboxes, etc. if we want to make a 'new document' (NOTE: remove this in a future update)

        void resetValues(Panel p) //Resets the controls inside of our New project if we decide to make a new document
        {
            // add a save check incase the user hasnt yet saved their plan

            
            //bool notsaved = true;
            bool BoolResult = checkChanges();
            if (BoolResult == true) //check if this is not equal to last load  *
            {
   
                //notsaved = true;

                DialogResult result = MessageBox.Show("Would you like to save your changes to '" + ProjectTitle + "' ?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);                
                if (result == DialogResult.Yes)
                {
                    // user clicked yes
                    if (saved == true)
                    {
                       
                        QuickSave(SavedPath);
                    }
                    else
                    {
                        save();
                    }
                    
                    //notsaved = false; //remove everything
                }
                else if (result == DialogResult.No)
                {
                    // user clicked no
                    //notsaved = false;
                }
                else
                {
                    //cancel
                    return;
                }
            }

            //add more values to reset when more elements are added
            setValues();
            //labels
            ProjectTitle = "Untitled Plan";
            label4.Text = ProjectTitle; //reset
            label11.Text = "";
            label12.Text = "";

            //remove everything from the flowLayoutPanel
            List<Control> listControls = flowLayoutPanel1.Controls.Cast<Control>().ToList();

            foreach (Control control in listControls)
            {
                
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }

            groupBoxes.Clear();
            TaskTitle.Clear();
            TaskInformation.Clear();

            startTimerDates.Clear();
            endTimerDates.Clear();
            //textboxes
            textBox1.Text = "";
            textBox2.Text = "";

            //datetime
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            resetState();
            setStates();

            //SavedPath = ""; //RESET SAVED PATH **************

            label13.Text = "Unsaved";
            saved = false; //new document so the task is unsaved


        }

        
        void createTask() //creates the task with the information set
        {

            //dates set
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            startTimerDates.Add(startDate);
            endTimerDates.Add(endDate);
            pinnedT.Add(ispinned);
            

            createTask cT = new createTask();


            //box properties
            GroupBox box = cT.getTask(title, info, startDate, endDate, ispinned); //
            //box.AutoSize = true;
            groupBoxes.Add(box);

            if (ispinned)
            {
                box.Paint += PaintBorderlessGroupBox;
                GetTheme th = new GetTheme();
                string theme = th.setTheme();
                Color themeC = new Color();
                switch (theme)
                {
                    case "blue":
                        themeC = Color.FromArgb(0, 174, 219);
                        break;
                    case "orange":
                        themeC = Color.FromArgb(243, 119, 53);
                        break;
                    case "green":
                        themeC = Color.FromArgb(0, 177, 89);
                        break;
                    case "grey":
                        themeC = Color.FromArgb(85, 85, 85);
                        break;
                    case "pink": //
                        themeC = Color.FromArgb(231, 113, 189);
                        break;
                    case "red":
                        themeC = Color.FromArgb(209, 17, 65);
                        break;
                    case "yellow":
                        themeC = Color.FromArgb(255, 196, 37);
                        break;
                    case "purple":
                        themeC = Color.FromArgb(124, 65, 153);
                        break;
                    default:
                        themeC = Color.FromArgb(0, 174, 219);
                        break;

                }

                defaultC = themeC;

            }

            List<Control> controls = box.Controls.Cast<Control>().ToList();
 
            RichTextBox r;
            foreach (Control ctrl in controls)
            {
                if (ctrl.Name == "DoneBtn")
                {
                    ctrl.Click += new EventHandler(DoneButton_Click);
                }
                if (ctrl.Name == "infoBtn") //assigns event to this button
                {
                    ctrl.Click += new EventHandler(button7_Click);
                }
                if (ctrl.Name == "taskInfo")
                {
                    r = ctrl as RichTextBox;
                    r.LinkClicked += new LinkClickedEventHandler(RichboxHandling);

                    if (Properties.Settings.Default.AutoScroll)
                    {
                        r.MouseEnter += new EventHandler(richTextBox2_MouseEnter);
                        r.MouseLeave += new EventHandler(richTextBox2_MouseLeave);
                    }

                    TaskInformation.Add(ctrl.Text);
                }
                
            }



            TaskTitle.Add(box.Text);
            flowLayoutPanel1.Controls.Add(box);
            box.Refresh();
            resetVar();
            //reset all 
            //labels
            label4.Text = ProjectTitle;
            label11.Text = "";
            label12.Text = "";
            metroToggle1.Checked = false;
            //textboxes
            textBox1.Text = "";
            textBox2.Text = "";

            //datetime
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            comboBox1_SelectedIndexChanged(comboBox1, EventArgs.Empty);
            TaskCount.Text = formatTaskText(startTimerDates.Count);

            if (Properties.Settings.Default.autoSave1)
            {
                QuickSave(SavedPath);
            }


        } //this function creates a task from user input in the form

        void OpenTask(DateTime[] start, DateTime[] end, string[] titles, string[] infos, bool[] taskpinned) //create all the tasks with saved values from opened file (load tasks from file)
        {
            createTask task = new createTask();


            for (int i = 0; i < start.Length; i++)
            {
                startTimerDates.Add(start[i]); //add values
                endTimerDates.Add(end[i]);
                pinnedT.Add(taskpinned[i]);

                GroupBox b = task.getTask(titles[i], infos[i], start[i], end[i], taskpinned[i]);

                if (taskpinned[i]) //update border 
                {
                    b.Paint += PaintBorderlessGroupBox;
                    GetTheme th = new GetTheme();
                    string theme = th.setTheme();
                    Color themeC = new Color();
                    switch (theme)
                    {
                        case "blue":
                            themeC = Color.FromArgb(0, 174, 219);
                            break;
                        case "orange":
                            themeC = Color.FromArgb(243, 119, 53);
                            break;
                        case "green":
                            themeC = Color.FromArgb(0, 177, 89);
                            break;
                        case "grey":
                            themeC = Color.FromArgb(85, 85, 85);
                            break;
                        case "pink": //
                            themeC = Color.FromArgb(231, 113, 189);
                            break;
                        case "red":
                            themeC = Color.FromArgb(209, 17, 65);
                            break;
                        case "yellow":
                            themeC = Color.FromArgb(255, 196, 37);
                            break;
                        case "purple":
                            themeC = Color.FromArgb(124, 65, 153);
                            break;
                        default:
                            themeC = Color.FromArgb(0, 174, 219);
                            break;

                    }

                    defaultC = themeC;
                }

                groupBoxes.Add(b);
                b.Refresh();

                List<Control> controls = b.Controls.Cast<Control>().ToList();
                RichTextBox r;
                foreach (Control ctrl in controls)
                {
                    if (ctrl.Name == "DoneBtn")
                    {
                        ctrl.Click += new EventHandler(DoneButton_Click);
                    }
                    if (ctrl.Name == "infoBtn")
                    {
                        ctrl.Click += new EventHandler(button7_Click);
                    }
                    if (ctrl.Name == "taskInfo")
                    {
                
                        r = ctrl as RichTextBox;
                        r.LinkClicked += new LinkClickedEventHandler(RichboxHandling);
                        if (Properties.Settings.Default.AutoScroll)
                        {
                            r.MouseEnter += new EventHandler(richTextBox2_MouseEnter);
                            r.MouseLeave += new EventHandler(richTextBox2_MouseLeave);
                        }
                        TaskInformation.Add(ctrl.Text);
                    }

                }

                //
                TaskTitle.Add(b.Text.Replace("📌 ", ""));
                flowLayoutPanel1.Controls.Add(b);
                resetState();
                setStates(); //*
                saved = true;
                savedTime = DateTime.Now;

                if (Properties.Settings.Default.Datetimestyle)
                {
                    label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
                    label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
                }
                else
                {
                    saveddate = savedTime;
                    timer_elapsed.Enabled = true;
                }
                


                initiateAutosave();
                TaskCount.Text = formatTaskText(startTimerDates.Count);
                comboBox1.SelectedIndex = 0;
                comboBox1_SelectedIndexChanged(comboBox1, EventArgs.Empty);
            }
        }

        void resetVar() //resets the variables
        {
            title = "";
            info = "";
            start = DateTime.Now;
            end = DateTime.Now;
        } //this function resets the main functions

        
        void save() //this function handles saving: save as *
        {
            
            
            DateTime[] startSave = startTimerDates.ToArray();
            DateTime[] endSave = endTimerDates.ToArray();
            SaveFileDialog saveFile = new SaveFileDialog();

            //groupbox info
            string[] TaskTitleArray = TaskTitle.ToArray();
            string[] TaskInfoArray = TaskInformation.ToArray();
            bool[] pinnedarray = pinnedT.ToArray();
            //GroupBox[] Boxes = groupBoxes.ToArray();
            saveFile.FileName = ProjectTitle; //temp
            saveFile.Filter = "PLAN File | *.plan";
            saveFile.DefaultExt = ".plan";
            bf = new BinaryFormatter();

            if (saveFile.ShowDialog() == DialogResult.OK) //pressed ok
            {
                string names = System.IO.Path.GetFileName(saveFile.FileName); //get the name

                //newName = Path.ChangeExtension(saveFile.FileName, ".plan");
                newName = saveFile.FileName; //our path

                SavedPath = newName; //our path


                ProjectTitle = names.Replace(".plan", "");
                //MessageBox.Show(newName);
                if (File.Exists(newName))
                {
                    //MessageBox.Show("exists");

                    File.Delete(newName);
                }


                SL = new SaveLoad(startSave, endSave, ProjectTitle, TaskTitleArray, TaskInfoArray, pinnedarray);



                stream = File.Open(newName, FileMode.Create); //used by another stream
                bf.Serialize(stream, SL);
                stream.Close();


                //after stuff
                updateRecent(names, SavedPath);

                setRecent();
                saved = true;
                savedTime = DateTime.Now;
                if (Properties.Settings.Default.Datetimestyle)
                {
                    label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
                }
                else
                {
                    saveddate = savedTime;
                    timer_elapsed.Enabled = true;
                }
                initiateAutosave();
                reload();
                resetState();
                setStates();

            }

        }

        void updateRecent(string title, string path)
        {
            if (FormListDir.Count != FormListNames.Count)
                MessageBox.Show("An error has occurred");


            try
            {
                for (int i = 0; i < FormListDir.Count; i++)
                {
                    if (FormListNames[i] == title)
                    {
                        FormListNames.Remove(FormListNames[i]);
                        FormListDir.Remove(FormListDir[i]);
                        break;
                    }

                }

                FormListNames.Add(title); //adds this project to the list of recent forms
                FormListDir.Add(path); //adds the directory
                                       //MessageBox.Show($"{FormListNames[0]}");
            }
            catch
            {
                
            }


        }

        void QuickSave(string path) //save without saveas (ADD TO OTHER CASES E.G OPEN, NEW, ETC. IF THE USER FORGETS TO SAVE)
        {
            DateTime[] StartDatesave = startTimerDates.ToArray();
            DateTime[] EndDateSave = endTimerDates.ToArray();
            

            //groupbox info
            string[] TitleTaskArray = TaskTitle.ToArray();
            string[] InfoTaskArray = TaskInformation.ToArray();
            bool[] pinnedarray = pinnedT.ToArray();

            SL = new SaveLoad(StartDatesave, EndDateSave, ProjectTitle, TitleTaskArray, InfoTaskArray, pinnedarray); //serialize
            bf = new BinaryFormatter();

            stream = File.Open(path, FileMode.Create);
            bf.Serialize(stream, SL);
            stream.Close();

            saved = true;
            savedTime = DateTime.Now;

            if (Properties.Settings.Default.Datetimestyle)
            {
                label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
            }
            else
            {
                saveddate = savedTime;
                timer_elapsed.Enabled = true;
            }

            resetState();
            setStates(); //
        }

        bool autosaveCheck()
        {
            if (Properties.Settings.Default.autoSave || Properties.Settings.Default.autoSave1)
            {
                return true;
            }
            return false;
        }

        void open() // this function opens any file
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "PLAN File | *.plan"; //must be a .plan file extension.

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fileName = System.IO.Path.GetFileName(openFile.FileName);

                //newOpenFileName = Path.ChangeExtension(openFile.FileName, ".plan");
                newOpenFileName = openFile.FileName;

                SavedPath = newOpenFileName;

                
                try
                {
                    stream = File.Open(newOpenFileName, FileMode.Open);

                    SL = (SaveLoad)bf.Deserialize(stream); 
                    stream.Close();
                }
                catch
                {
                    MessageBox.Show("Unable to open plan! ");
                    return;
                }





                DateTime[] start = SL.getStart();
                DateTime[] end = SL.getEnd();
                //set values
                newToolStripMenuItem.PerformClick(); //new 
                ProjectTitle = SL.getTitle();
                label4.Text = ProjectTitle;

                updateRecent(SL.getTitle(), SavedPath);

                setRecent();

                string[] taskTitles = SL.getTaskTitle();
                string[] taskInfos = SL.getInfo();
                bool[] istaskpinned = SL.getpinned();

                SetStartTasks(false);
                comboBox1.SelectedIndex = 0;

                //sets the variables with saved data and add them to the tasks list
                OpenTask(start, end, taskTitles, taskInfos, istaskpinned);
                checkDates();

                saveAsToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                initiateAutosave();
                //comboBox1.SelectedIndex = 0;
                //comboBox1_SelectedIndexChanged(comboBox1, EventArgs.Empty);
            }
            else
            {
                return;
            }

        }

        void initiateAutosave()
        {
            if (Properties.Settings.Default.autoSave)
            {
                saveTimer.Interval = 60000 * Properties.Settings.Default.minSave;

                saveTimer.Enabled = true;
            }
        }

        void QuickOpen(string file)
        {
            //quickopen = false;
            try
            {
                //string fileName = FormListNames[index];

                //string Path = FormListDir[index];
                Stream s = File.Open(file, FileMode.Open);

                SaveLoad sl= new SaveLoad();
                BinaryFormatter b = new BinaryFormatter();
                sl = (SaveLoad)b.Deserialize(s);
                s.Close();

                SavedPath = file;

                saved = true;
                savedTime = DateTime.Now;
                if (Properties.Settings.Default.Datetimestyle)
                {
                    label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
                }
                else
                {
                    saveddate = savedTime;
                    timer_elapsed.Enabled = true;
                }



                DateTime[] start = sl.getStart();
                DateTime[] end = sl.getEnd();
                //set values
                newToolStripMenuItem.PerformClick(); //new 
                ProjectTitle = sl.getTitle();
                label4.Text = ProjectTitle;

                updateRecent(sl.getTitle(), file);

                setRecent();

                string[] taskTitles = sl.getTaskTitle();
                string[] taskInfos = sl.getInfo();
                bool[] pinnedtasks = sl.getpinned();

                //sets the variables with saved data and add them to the tasks list
                OpenTask(start, end, taskTitles, taskInfos, pinnedtasks);
                checkDates();
            }
            catch (Exception e)
            {
                MessageBox.Show("This file cannot be opened! " + e);
            }


        }

        void InitiateOpen(int index)
        {
            try
            {
                string fileName = FormListNames[index];

                string Path = FormListDir[index];


                stream = File.Open(Path, FileMode.Open);

                SL = (SaveLoad)bf.Deserialize(stream);
                stream.Close();

                SavedPath = Path;

                saved = true;
                savedTime = DateTime.Now;
                if (Properties.Settings.Default.Datetimestyle)
                {
                    label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved at: " + savedTime.ToString();
                }
                else
                {
                    saveddate = savedTime;
                    timer_elapsed.Enabled = true;
                }



                DateTime[] start = SL.getStart();
                DateTime[] end = SL.getEnd();
                //set values
                newToolStripMenuItem.PerformClick(); //new 
                ProjectTitle = SL.getTitle();
                label4.Text = ProjectTitle;

                updateRecent(SL.getTitle(), Path);

                setRecent();

                string[] taskTitles = SL.getTaskTitle();
                string[] taskInfos = SL.getInfo();
                bool[] pinnedtasks = SL.getpinned();

                //sets the variables with saved data and add them to the tasks list
                OpenTask(start, end, taskTitles, taskInfos, pinnedtasks);
                checkDates();
            }
            catch
            {
                DialogResult result = MessageBox.Show("This file cannot be opened, it may be replaced or removed. Remove from recent?", "File not found", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string tempstring = FormListDir[index];
                    FormListNames.Remove(FormListNames[index]);
                    FormListDir.Remove(FormListDir[index]);


                    foreach (GroupBox box in flowLayoutPanel2.Controls)
                    {
                 
                        foreach(Control C in box.Controls)
                        {
                            if (C is TextBox)
                            {
                                if (C.Text == tempstring)
                                {
                                    flowLayoutPanel2.Controls.Remove(box);
                                    break;
                                }
                            }

                        }
                    }
                    setRecent();
                    if (FormListDir.Count == 0) //no results <-- file exits but is empty
                    {
                        TextDirect.Visible = true;

                        return;
                    }

                    

                }
                else
                {
                    return;
                }
            }

            
        }

        void reload() //this function reloads the form after saving to update titles, etc. TODO: add a load function* 
        {
            //load again
            SL = null;
           

            stream = File.Open(newName, FileMode.Open);
            bf = new BinaryFormatter();

            SL = (SaveLoad)bf.Deserialize(stream);
            stream.Close();

            //reset
            label4.Text = SL.getTitle(); //fix this

            // add the saved groupboxes

        } 

        void resetState() //resets the form state (used after task create)
        {
            // resets the form state
            totalControls = 0;

            for (int i = 0; i < StartTitles.Count; i++)
            {
                StartTitles.Clear();
                StartInfo.Clear();
                StateStartTime.Clear();
                StateEndTime.Clear();
                StatePinned.Clear();

            }
        } //resets the info for the form states

        bool checkChanges() // checks for if any changes are made *
        {
            //temporary variables that hold current form info
            List<bool> tempPinned = pinnedT;
            List<string> tempTitles = TaskTitle;
            List<string> tempInfo = TaskInformation;
            List<DateTime> TempStart = startTimerDates;
            List<DateTime> TempEnd = endTimerDates;
            int TempControlTotal = flowLayoutPanel1.Controls.Count;
    

            if (TempControlTotal != totalControls) //the new amount of controls changed
            {
                //it is 100% that the user had made changes
                //MessageBox.Show("changes");
                return true; // changes are made
                
            }
            else //if the amount is the same
            {
                for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
                {
                    //if the stuff changes
                    if ((tempTitles[i] != StartTitles[i]) || (tempInfo[i] != StartInfo[i]) || (TempStart[i] != StateStartTime[i]) || (TempEnd[i] != StateEndTime[i]) || (tempPinned[i] != StatePinned[i])) //the amount of tasks are the same but the content is different
                    {
                        //MessageBox.Show("changes 2");
                        return true;
                    }
                    else //everything remained the same
                    {
                        //ignore and continue (DO NOT ADD CODE HERE)
                    }
                }

            }
            //MessageBox.Show("no changes");
            return false;
        }

        void setStates() //sets our states to the current states
        {
            totalControls = flowLayoutPanel1.Controls.Count;
            //our states
            for (int i = 0; i < TaskTitle.Count; i++)
            {
                StatePinned.Add(pinnedT[i]);
                StartTitles.Add(TaskTitle[i]);
                StartInfo.Add(TaskInformation[i]);
                StateStartTime.Add(startTimerDates[i]);
                StateEndTime.Add(endTimerDates[i]);
            }
        }

        //TIMER
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (startTimerDates.Count != 0)
            {
                TimeSpan totaltime;
                TimeSpan timeElapsed;
                TimeSpan timeLeft;

                


                TimeSpan timeUntilStart;

                int percent;
                double timePercent;

                for (int i = 0; i < startTimerDates.Count; i++)
                {
                    if(startTimerDates[i] > DateTime.Now)
                    {
                        //ignore this one
                        //change label text to "starts on + date"
                        timeUntilStart = startTimerDates[i] - DateTime.Now;
                        Label untilStart = new Label();
                        var UntilStartLabel = groupBoxes[i].Controls.OfType<Label>();
                        foreach (Label l in UntilStartLabel)
                        {
                            if (l.Name == "timeleft")
                            {
                                untilStart = l;
                            }
                        }

                        // add a starts in...
                        untilStart.Text = "Starts in: " + string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", timeUntilStart.Days, timeUntilStart.Hours, timeUntilStart.Minutes, timeUntilStart.Seconds);


                    }
                    else
                    {
                        //calculate the times required
                        totaltime = endTimerDates[i] - startTimerDates[i]; //total time range

                        timeElapsed = DateTime.Now - startTimerDates[i]; //elapsed time range
                        timeLeft = endTimerDates[i] - DateTime.Now; //remaining time 

                        //calculate percentages from our data
                        timePercent = timeElapsed.Ticks / (double)totaltime.Ticks * 100;
                        percent = (int)timePercent; //parses it into an integer value

                        MetroFramework.Controls.MetroProgressBar selectedBar = new MetroFramework.Controls.MetroProgressBar();
                        //ProgressBar selectedBar = new ProgressBar();
                        Label timelabel = new Label();

                        var progbar = groupBoxes[i].Controls.OfType<ProgressBar>();
                        var timeLabel = groupBoxes[i].Controls.OfType<Label>();


                        foreach (MetroFramework.Controls.MetroProgressBar myprogressbar in progbar)
                        {
                            selectedBar = myprogressbar;
                        }

                        foreach (Label l in timeLabel)
                        {
                            if (l.Name == "timeleft")
                            {
                                timelabel = l;
                            }
                        }

                        timelabel.Text = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);

                        if (percent >= 100 || percent < 0)
                        {
                            //if (timelabel.Text == "Time is up!")
                            //{
                                //checkedForDone = true;
                            //}
                            percent = 100;
                            selectedBar.Value = 100;
                            timelabel.Text = "Time is up!";
                            //done

                            
                        }
                        else
                        {
                            selectedBar.Value = percent;
                        }



                        //remaingTimerTimes.Add(timeLeft); //remember to add our timespan to our list for later use
                        //TimePercentage.Add(percent);

                    }



                }
                

            }
            else
            {
                
                return;
            }
            



        } //our tick function that handles timing for the progress bars

        void flushmem()
        {
            Process currentProc = Process.GetCurrentProcess();
            long memoryUsed = currentProc.PrivateMemorySize64;


            if (memoryUsed > 30000000)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {

                    SetProcessWorkingSetSize32Bit(System.Diagnostics
                       .Process.GetCurrentProcess().Handle, -1, -1);

                }
            }



        }

        private void timer2_Tick(object sender, EventArgs e) //
        {
            flushmem(); //flush memory <-------------------------- memory leak?
        }

        void checkDates() //check if any tasks are done
        {
            List<int> taskDoneIndex = new List<int>();

            for (int i = 0; i < endTimerDates.Count; i++)
            {
                if (endTimerDates[i] <= DateTime.Now)
                {
                    taskDoneIndex.Add(i);
                }
            }

            if (taskDoneIndex.Count > 0)
            {
                sendDone(taskDoneIndex);
            }
            else
            {
                return;
            }
            


        }

        void sendDone(List<int> taskIndex)
        {
            TasksDone td = new TasksDone();
            //TasksDone.getTaskInfo(length, endedTime);

            FlowLayoutPanel lb = td.Controls.Find("flowLayoutPanel1", true).FirstOrDefault() as FlowLayoutPanel;
            for (int i = 0; i < taskIndex.Count; i++)
            {
                MetroFramework.Controls.MetroLabel l = new MetroFramework.Controls.MetroLabel();
                l.AutoSize = true;
                l.Text = (TaskTitle[taskIndex[i]] + " finshed on: " + endTimerDates[taskIndex[i]]);
                lb.Controls.Add(l);
                lb.SetFlowBreak(l, true);
            }
            
            td.StartPosition = FormStartPosition.CenterParent;
            td.ShowDialog(this);
 
        }


        public string[] getNewsInfo()
        {
            string[] output = { "failed", "failed", "failed" }; //criteria 
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    //has internet
                    GetNews g = new GetNews();
                    List<string> News = new List<string>();
                    News = g.getInfo();


                    string versionTemp = News[0];
                    string first = versionTemp.Replace("OUT NOW", ""); //getting versions
                    formattedNumb = first;
                    string final = first.Replace(".", "");

                    output[2] = g.getCommands();

                    for (int i = 0; i < News.Count; i++)
                    {
                        richTextBox1.AppendText(News[i]);
                        richTextBox1.AppendText(Environment.NewLine);
                        richTextBox1.AppendText(Environment.NewLine);
                    }
                    output[0] = final;

                    string URl = News[1];
                    
                    string final2 = URl.Replace("get it here: ", "");
    
                    output[1] = final2;
                    return output;
                }
            }
            catch
            {
                //no internet
                richTextBox1.Text = "Error, Failed to load Planner news. (press HELP for troubleshooting tips.)";
                return output;
            }
        }

        public void checkVersion()
        {
            if (Properties.Settings.Default.notif)
            {
                try
                {
                    int x = Int32.Parse(tempVersionNumber);
                    if (x > versionNumber) //comparing newest versions
                    {
                        uptodate = false;
                        //do stuff
                        if (Properties.Settings.Default.autoUpdate)
                        {
                            DialogResult result = MessageBox.Show("New update! Version " + formattedNumb + " is avaliable, Confirm Auto Update?", "New update avaliable", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                //Process.Start("chrome.exe", link);
                                AutoUpdate();

                            }
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("New update! Version " + formattedNumb + " is avaliable, would you like to update?", "New update avaliable", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(link);


                            }
                        }




                    }
                    else
                    {
                        uptodate = true;
                    }
                }
                catch
                {
                    //nothing, internet probably failed
                    uptodate = true;
                }
            }
            else
            {
                if (Properties.Settings.Default.autoUpdate)
                {
                    AutoUpdate();
                }
            }


            checkCommands();
        }

        public void AutoUpdate()
        {
            if (!Directory.Exists(@"C:\Planner2.0\planner update"))
            {
                Directory.CreateDirectory(@"C:\Planner2.0\planner update");
            }

            try
            {

                GetNews g = new GetNews();
                string item = g.getFile();
                //MessageBox.Show(item);
                WebClient wb = new WebClient();
                wb.DownloadFile("https://drive.google.com/uc?export=download&id=" + item, @"C:\Planner2.0\planner update\Planner Installer.msi");
                Process.Start(@"C:\Planner2.0\planner update\Planner Installer.msi");
            }
            catch
            {
                MessageBox.Show("unable to download update!");
            }


        }

        public void checkCommands() //checks for specific commands
        {
            //make sure to save
            string error = "";

            switch (command.ToLower())
            {
                case "idle":
                    //allow user to access
                    clearSettings();
                    break;
                case "":
                    clearSettings();
                    break;
                case "mandupdate":
                    //mandatory update
                    //some function
                    bool update = MandUpdate();
                    if (!update)
                        error = "Apologies for the inconvenience, to continue with our service, this program requires you to update to the latest version, thanks for understanding.";
                    //may be add a functiuon where the user can be on a minimum version

                    break;
                case "lockdown":
                    //inablilty to access
                    setLocked();
                    error = "Apologies for the inconvenience, unfortunately this program appears to be in lockdown mode, try again later while we are resolving this issue.";
                    
                    break;
                case "failed": //default at failed if theres no internet
                    break;
                default:
                    break;
            }

            commenceStartUP(error);

        }

        void clearSettings()
        {
            //if the program is under restrictions, we clear them here
            if (Properties.Settings.Default.locked == true)
            {
                Properties.Settings.Default.locked = false;
                Properties.Settings.Default.Save();
            }

        }

        void setLocked()
        {
            if (Properties.Settings.Default.locked != true)
            {
                Properties.Settings.Default.locked = true;
                Properties.Settings.Default.Save();
            }
        }

        bool MandUpdate()
        {
            if (uptodate)
            {
                //if updated
                clearSettings();
                return true;
            }
            else
            {
                if (Properties.Settings.Default.locked != true)
                {
                    Properties.Settings.Default.locked = true; //not up to date
                    Properties.Settings.Default.Save();
                }
                return false;
            }
            
        }

        public void commenceStartUP(string error)
        {
            if (Properties.Settings.Default.locked == true && isMasterProgram == false)
            {
                //say something
                if (error != "")
                    MessageBox.Show(error);
                else
                {
                    MessageBox.Show("There has been an issue regarding your user license, reconnect now to find the issue.");
                }

                Application.Exit();
                return;
            }
            //so the master program does not need to access these
        }

        private void Planner_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (this.DialogResult == DialogResult.Cancel)
            {
                

            }
            else
            {
                pressedX = true;
                exitToolStripMenuItem.PerformClick();

                if (hasExit == true)
                {

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        // ----------------------------------------------------------------------------------------------------------------------------------



        // All buttons ======================================================================================================================

        //About panel
        private void button1_Click(object sender, EventArgs e) //hiding the about panel (back)
        {
            about.Hide();
        }



        //New panel
        private void button2_Click(object sender, EventArgs e) //create task (from new tab)
        {
            label11.Text = "";
            label12.Text = "";
            if (textBox1.Text == "" | textBox1.Text == null)
            {
                label11.Text = "Please enter a title";
                return;
            }
            else if (dateTimePicker2.Value < dateTimePicker1.Value) //the task doesnt count backwards
            {
                label12.Text = "Please enter a valid date";
                return;
            }
            else
            {
                title = textBox1.Text;
                

                info = textBox2.Text;
                start = dateTimePicker1.Value;
                end = dateTimePicker2.Value;
                createTask();
                //create task 
            }

        }

        private void DoneButton_Click(object sender, EventArgs e) //buttons for if the user has completed the task and wishes to remove it
        {
            Button b = (Button)sender;
            GroupBox box = b.Parent as GroupBox;
            int index = groupBoxes.IndexOf(box);
            string title = box.Text;

            DialogResult result = MessageBox.Show("Are you sure you will like to remove '" + title + "' ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //remove stuff
                flowLayoutPanel1.Controls.Remove(groupBoxes[index]);
                groupBoxes.Remove(box);
                startTimerDates.Remove(startTimerDates[index]);
                endTimerDates.Remove(endTimerDates[index]);
                TaskTitle.Remove(TaskTitle[index]);
                TaskInformation.Remove(TaskInformation[index]);
                pinnedT.Remove(pinnedT[index]);
                TaskCount.Text = formatTaskText(startTimerDates.Count);

            }
            else if (result == DialogResult.No)
            {
                return;
            }

            if (Properties.Settings.Default.autoSave1)
            {
                QuickSave(SavedPath);
            }

        }

        private void openRecent(object sender, EventArgs e)
        {
            int index = 0;
            Button recentB = (Button)sender;
            for (int i = 0; i < FormListNames.Count; i++)
            {
                if (FormListNames[i] == recentB.Text)
                {
                    index = i;
                }
            }

            InitiateOpen(index); //open our new file

        }

        

        private void button4_Click(object sender, EventArgs e) //start new
        {
            newToolStripMenuItem.PerformClick();
        }

        private void button5_Click(object sender, EventArgs e) //start open
        {
            openToolStripMenuItem.PerformClick();
        }



        private void button6_Click(object sender, EventArgs e) //help back
        {
            help.Hide();
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //when the combo box is changed (trigger this when new task is created)
        {
            newIndex.Clear();
            //the combo box item is set.
            //MessageBox.Show("changed");
            int selectedItem = comboBox1.SelectedIndex;
            /* index:
             * 0- Start Date (descending)
             * 1- Start Date (ascending)
             * 2- End Date (descending)
             * 3- End Date (ascending)
             * 4- pinned tasks first, 
             */
            switch (selectedItem)
            {
                case 0:
                    SetStartTasks(false);
                    break;
                case 1:
                    SetStartTasks(true);
                    break;
                case 2:
                    SetEndTasks(false);
                    break;
                case 3:
                    SetEndTasks(true);
                    break;
                case 4:
                    setPinnedTasks();
                    break;
                default:
                    SetStartTasks(false);
                    break;
            }

        }



        // combo box commands ===================================================================================================================================
        void SetStartTasks(bool A) //set the end times
        {
            int N = endTimerDates.Count;
            int[] index = Enumerable.Range(0, N).ToArray<int>();

            newIndex.Clear();


            //starttimerdates
            if (A)
            {
                Array.Sort<int>(index, (a, b) => startTimerDates[a].CompareTo(startTimerDates[b]));

            }
            else
            {
                Array.Sort<int>(index, (a, b) => startTimerDates[b].CompareTo(startTimerDates[a]));
            }
            newIndex = index.ToList();
            resetArrange();
            //label20.Text = newIndex[0].ToString() + " " + newIndex[1].ToString() + " " + newIndex[2].ToString() + " " + newIndex[3].ToString() + " ";
        }

        void SetEndTasks(bool A) //set the start times 
        {
            int N = endTimerDates.Count;
            int[] index = Enumerable.Range(0, N).ToArray<int>();


            newIndex.Clear();

            if (A)
            {
                Array.Sort<int>(index, (a, b) => endTimerDates[a].CompareTo(endTimerDates[b]));
            }
            else
            {
                Array.Sort<int>(index, (a, b) => endTimerDates[b].CompareTo(endTimerDates[a]));
            }

            newIndex = index.ToList();
            //dates are sorted
            resetArrange();

        }

        void setPinnedTasks() //adds the pinned first and arranges rest accordingly
        {
            newIndex.Clear();

            List<int> pinned = new List<int>();
            List<int> notpinned = new List<int>(); //array for not pinned, for all not pinned indexes

            for (int i = 0; i < pinnedT.Count; i++)
            {
                if (pinnedT[i])
                {
                    pinned.Add(i);
                }
                else
                {
                    notpinned.Add(i);
                }
            }

            //MessageBox.Show(pinned.Count.ToString());
            //MessageBox.Show(notpinned.Count.ToString());
            //MessageBox.Show(pinnedT.Count.ToString());
            List<int> index = pinned.Concat(notpinned).ToList();
            
            newIndex = index;

            resetArrange();//
        }

        void resetArrange() //removes all the controls and rearranges them accordingly
        {

            flowLayoutPanel1.Controls.Clear(); //clear the flow layout panel

            //temporary variables
            List<bool> tmppinned = new List<bool>();
            List<DateTime> tmpStart = new List<DateTime>();
            List<DateTime> tmpEnd = new List<DateTime>();
            List<string> tmpTitle = new List<string>();
            List<string> tmpInfo = new List<string>();
            List<GroupBox> tmpBox = new List<GroupBox>();

            for (int i = 0; i < startTimerDates.Count; i++) //update all
            {
                tmpTitle.Add(TaskTitle[i]);
                tmpInfo.Add(TaskInformation[i]);
                tmpStart.Add(startTimerDates[i]);
                tmpEnd.Add(endTimerDates[i]);
                tmpBox.Add(groupBoxes[i]);
                tmppinned.Add(pinnedT[i]); //this one will have a copy of the list
            }

            //clear the old ones
            startTimerDates.Clear();
            endTimerDates.Clear();
            TaskTitle.Clear();
            TaskInformation.Clear();
            groupBoxes.Clear();
            pinnedT.Clear();

            //MessageBox.Show(tmpTitle.Count.ToString());

            for (int i = 0; i < tmpTitle.Count; i++) //we loop though the temp ones
            {
                //MessageBox.Show(tmppinned[newIndex[i]].ToString());
                pinnedT.Add(tmppinned[newIndex[i]]);
                TaskTitle.Add(tmpTitle[newIndex[i]]); //with new index
                TaskInformation.Add(tmpInfo[newIndex[i]]);
                startTimerDates.Add(tmpStart[newIndex[i]]);
                endTimerDates.Add(tmpEnd[newIndex[i]]);
                groupBoxes.Add(tmpBox[newIndex[i]]);
                flowLayoutPanel1.Controls.Add(groupBoxes[i]);
                
            }




        }

        //forgot to name, this is the info button
        private void button7_Click(object sender, EventArgs e) //opens the info 
        {
            TaskInfo TI = new TaskInfo(); //creates a new instance

            Button b = (Button)sender;
            GroupBox box = b.Parent as GroupBox;
            int index = groupBoxes.IndexOf(box); //gets the index of the selected item

            //sets info (includes the index)
            TI.setInfo(TaskTitle[index].Replace("📌 ", ""), TaskInformation[index], startTimerDates[index], endTimerDates[index], index, pinnedT[index]);

            //specifies a start position
            TI.StartPosition = FormStartPosition.CenterParent;
            TI.ShowDialog(this); //show it


            if (Inforemove == true) //does the info class request a remove?
            {
                Inforemove = false; 
                remove(InfoIndex); //remove this item
            }
            else if(InfoApply == true) //does it request an apply?
            {
                InfoApply = false;
                apply(InfoIndex); //apply this item
            }
        }
        //
        public void apply (int ind) //apply from the TaskInfo.cs class
        {
            //need to rechange to rich text box. (optimisation for newest version)
            
            //int h;
            //changes variables to new variables
            TaskTitle[ind] = I_title; 
            TaskInformation[ind] = I_info;
            startTimerDates[ind] = I_start;
            endTimerDates[ind] = I_end;
            pinnedT[ind] = I_pinned;

            groupBoxes[ind].Text = TaskTitle[ind].ToUpper();
            groupBoxes[ind].Paint += PaintBorderlessGroupBox;

            //recreates some of the task (the groupbox is not destroyed)
            RichTextBox t = new RichTextBox(); //task information
            var info = groupBoxes[ind].Controls.OfType<RichTextBox>(); //get all textboxes in the groupbox
            foreach (RichTextBox l in info) //loop through them
            {
                if (l.Name == "taskInfo") //if a task information textbox is found
                {
                    //t = l;
                    //l.Text = TaskInformation[ind];
                    groupBoxes[ind].Controls.Remove(l); //delete it
                }
            }

            RichTextBox taskInfo = new RichTextBox(); //lets create a new taks information box
            taskInfo.Font = new Font("Century Gothic", 8, FontStyle.Regular);
            taskInfo.Name = "taskInfo";
            taskInfo.ReadOnly = true;
            taskInfo.Multiline = true;
            taskInfo.ScrollBars = RichTextBoxScrollBars.Vertical; //verticle
            taskInfo.BorderStyle = BorderStyle.None;
            taskInfo.BackColor = SystemColors.ControlLightLight;
            taskInfo.LinkClicked += new LinkClickedEventHandler(RichboxHandling);

            taskInfo.Location = new Point(6, 22);
            taskInfo.Text = TaskInformation[ind]; //set info
            taskInfo.Width = 324;
            //h = 28;
            taskInfo.Height = 28;
            taskInfo.Anchor = (AnchorStyles.Top | AnchorStyles.Left);

            if (pinnedT[ind])
            {
                GetTheme th = new GetTheme();
                string theme = th.setTheme();
                Color themeC = new Color();
                switch (theme)
                {
                    case "blue":
                        themeC = Color.FromArgb(0, 174, 219);
                        break;
                    case "orange":
                        themeC = Color.FromArgb(243, 119, 53);
                        break;
                    case "green":
                        themeC = Color.FromArgb(0, 177, 89);
                        break;
                    case "grey":
                        themeC = Color.FromArgb(85, 85, 85);
                        break;
                    case "pink": //
                        themeC = Color.FromArgb(231, 113, 189);
                        break;
                    case "red":
                        themeC = Color.FromArgb(209, 17, 65);
                        break;
                    case "yellow":
                        themeC = Color.FromArgb(255, 196, 37);
                        break;
                    case "purple":
                        themeC = Color.FromArgb(124, 65, 153);
                        break;
                    default:
                        themeC = Color.FromArgb(0, 174, 219);
                        break;

                }
                groupBoxes[ind].ForeColor = themeC;
                taskInfo.ForeColor = themeC;
                groupBoxes[ind].Font = new Font("Century Gothic", 10, FontStyle.Bold);
                taskInfo.Font = new Font("Century Gothic", 9, FontStyle.Regular);
                groupBoxes[ind].Text = "📌 " + TaskTitle[ind].ToUpper();
                defaultC = themeC;
            }
            else
            {
                groupBoxes[ind].ForeColor = Color.Black;
                defaultC = Color.Black;
                groupBoxes[ind].Font = new Font("Century Gothic", 10, FontStyle.Regular);
                taskInfo.Font = new Font("Century Gothic", 8, FontStyle.Regular);
            }
            
            
            groupBoxes[ind].Controls.Add(taskInfo);

            //t.Text = TaskInformation[ind];
            groupBoxes[ind].Refresh();

            if (Properties.Settings.Default.autoSave1)
            {
                QuickSave(SavedPath);
            }

        }
        #region
        public void PaintBorderlessGroupBox(object sender, PaintEventArgs p)
        {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, p.Graphics, Color.Red, defaultC);
        }

        private void DrawGroupBox(GroupBox box, Graphics g, Color textColor, Color borderColor)
        {
            if (box != null)
            {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                //g.Clear(this.b);

                // Draw text
                //g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width + 5), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }
        #endregion

        public void remove(int ind)
        {
            int index = ind;
            GroupBox box = groupBoxes[index];

            flowLayoutPanel1.Controls.Remove(groupBoxes[index]);
            groupBoxes.Remove(box);
            startTimerDates.Remove(startTimerDates[index]);
            endTimerDates.Remove(endTimerDates[index]);
            TaskTitle.Remove(TaskTitle[index]);
            TaskInformation.Remove(TaskInformation[index]);
            TaskCount.Text = formatTaskText(startTimerDates.Count);
            pinnedT.Remove(pinnedT[index]);

            if (Properties.Settings.Default.autoSave1)
            {
                QuickSave(SavedPath);
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        public void RichboxHandling(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://drive.google.com/drive/folders/1xyxmrBAtCskAdM5gypGmh0cBjfk4lpFy");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/G8W7Vtw"); //discord server
        }

        #region theme changes
        //theme tile changes






        #region tile hovers




        private void tilehover(object sender, EventArgs e)
        {
            MetroFramework.Controls.MetroTile tile = (MetroFramework.Controls.MetroTile)sender;
            tile.Size = new Size(160, 160);
        }

        private void tileleave(object sender, EventArgs e)
        {
            MetroFramework.Controls.MetroTile tile = (MetroFramework.Controls.MetroTile)sender;
            tile.Size = new Size(150, 150);
        }

        #endregion
        private void metroTile1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "blue";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }

        private void metroTile2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "green";
            Properties.Settings.Default.Save();
            setTheme(); //5 6 7 8
            //MessageBox.Show("Changes made");
        }


        private void metroTile3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "orange";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }

        private void metroTile4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "grey";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }
        //
        private void metroTile5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "pink";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }

        
        private void metroTile6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "red";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }
        private void metroTile7_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "yellow";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }

        private void metroTile8_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Theme = "purple";
            Properties.Settings.Default.Save();
            setTheme();
            //MessageBox.Show("Changes made");
        }


        private void button8_Click(object sender, EventArgs e)
        {
            themePanel.Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            help.Show();
            help.BringToFront();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            initiateSettings();
            Options.Show();
            Options.BringToFront();
        }

        private void Darktoggle_CheckedChanged(object sender, EventArgs e) //dark mode
        {
            if (!isDarkmode)
            {
                //set to dark mode
                isDarkmode = true;
                Properties.Settings.Default.darkmode = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                //set to light mode
                isDarkmode = false;
                Properties.Settings.Default.darkmode = false;
                Properties.Settings.Default.Save();
            }

            darkmode();
        }
        #endregion

        private void Planner_Paint(object sender, PaintEventArgs e)
        {


        }

        private void themePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroToggle1.Checked)
            {
                
                ispinned = true;
            }
            else
            {
                ispinned = false;
            }
        }

        //options

        private void button15_Click(object sender, EventArgs e) //ok or save
        {
            //save here

            if (!Directory.Exists(metroTextBox1.Text))
            {
                MessageBox.Show($"Path {metroTextBox1.Text} does not exist!");
                return;
            }

            Properties.Settings.Default.defPath = metroTextBox1.Text;
            Properties.Settings.Default.notif = metroCheckBox2.Checked;
            Properties.Settings.Default.autoUpdate = metroCheckBox3.Checked;
            Properties.Settings.Default.autoSave = metroRadioButton1.Checked;
            Properties.Settings.Default.autoSave1 = metroRadioButton2.Checked;
            
            if (metroRadioButton1.Checked)
            {
                Properties.Settings.Default.minSave = (int)numericUpDown1.Value;
            }

            if(metroComboBox1.SelectedIndex == 0)
            {
                Properties.Settings.Default.Datetimestyle = true;
            }
            else
            {
                Properties.Settings.Default.Datetimestyle = false;
            }

            




            Properties.Settings.Default.AutoScroll = metroCheckBox4.Checked;
            Properties.Settings.Default.Save(); //save
            Options.Hide();

            //notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            //notifyIcon1.BalloonTipText = "Changes made!";
            notifyIcon1.Icon = notifyIcon1.Icon;
            //notifyIcon1.BalloonTipTitle = "Changes may occur after this program restarts.";
            notifyIcon1.ShowBalloonTip(1000, "Changes made!", "Changes have been saved!", ToolTipIcon.None);
            initiateSettings();
        }


        private void button12_Click(object sender, EventArgs e) //back/cancel
        {
            Options.Hide();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //show folder browser
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                RecentRootDir = folderBrowserDialog1.SelectedPath;
            }

            metroTextBox1.Text = RecentRootDir;
       
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (metroCheckBox1.Checked == false)
            {
                metroRadioButton1.Enabled = false;
                metroRadioButton2.Enabled = false;
                numericUpDown1.Enabled = false;
                

            }
            else
            {
                metroRadioButton1.Enabled = true;
                metroRadioButton2.Enabled = true;
                //numericUpDown1.Enabled = true;
                if (metroRadioButton1.Checked)
                {
                    numericUpDown1.Enabled = true;
                }
            }
                
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to clear data in '{Properties.Settings.Default.defPath}'?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                string[] targetFile = { Properties.Settings.Default.defPath, "PlannerRecentFiles.plData" };
                string targetPath = Path.Combine(targetFile);
                if (!File.Exists(targetPath))
                {
                    //no data found
                }
                else
                {
                    File.Delete(targetPath);
                    clearRecent = true;
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void metroRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            //metroRadioButton2.Checked = false;
        }
        private void metroRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            //metroRadioButton1.Checked = false;
        }
        private void saveTimer_Tick(object sender, EventArgs e)
        {
            //save
            QuickSave(SavedPath);
        }

        

        private void richTextBox2_MouseEnter(object sender, EventArgs e) //auto scroll
        {
            //mousein = true;
            RichTextBox t = sender as RichTextBox;
            for (int i = 0; i < t.Text.Length; i++)
            {
                t.SelectionStart = i;
                t.ScrollToCaret();
                t.Refresh();
                Thread.Sleep(50);
            }
            

        }

        private void richTextBox2_MouseLeave(object sender, EventArgs e)
        {

            RichTextBox t = sender as RichTextBox;
            t.SelectionStart = 0;
            t.ScrollToCaret();
  
        }

        private void timer_elapsed_Tick(object sender, EventArgs e)
        {
            savemin = DateTime.Now - saveddate;
            int min = (int)savemin.TotalMinutes;
            if (min == 0)
            {
                label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Saved!";
            }
            else
            {
                label13.Text = "AutoSave: " + autosaveCheck().ToString() + "| Last Saved " + min.ToString() + " minute(s) ago";
            }
        }



        private void button17_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure you want to restore settings to default?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                initiateSettings();
                //...
            }
            else if (result == DialogResult.No)
            {
                //...
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/G8W7Vtw"); //discord server
        }

        private void button19_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://drive.google.com/drive/folders/1xyxmrBAtCskAdM5gypGmh0cBjfk4lpFy");
        }

        private void OnMouseEnterButton1(object sender, EventArgs e)
        {
            //Button btn = (Button)sender;
            button18.BackColor = Color.Transparent; // or Color.Red or whatever you want
            button19.BackColor = Color.Transparent; // or Color.Red or whatever you want
        }

    }
}
