/* this class creates the tasks  
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace Planner_2._0
{
    class createTask
    {
        public Color defaultC = Color.Black;

        public static int h;

        public GroupBox currentbox;

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
                //box.Paint -= PaintBorderlessGroupBox;
                defaultC = Color.Black;
            }
        }

        public GroupBox getTask(string title, string info, DateTime start, DateTime end, bool pinned)
        {
            defaultC = Color.Black;
            GroupBox box = new GroupBox(); //create a new box
            currentbox = box;
            box.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            box.Location = new Point(3, 3);
            box.Size = new Size(343, 93);
            box.Text = title.ToUpper(); //set title


            box.AutoSize = true;

            box.Paint += PaintBorderlessGroupBox;

            //progressbar
            MetroFramework.Controls.MetroProgressBar bar = new MetroFramework.Controls.MetroProgressBar();
            //ProgressBar bar = new ProgressBar();
            bar.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            bar.Location = new Point(6, 52);
            bar.Size = new Size(324, 23);
            switch (Properties.Settings.Default.Theme)
            {
                case "blue":
                    bar.Style = MetroFramework.MetroColorStyle.Blue;
                    break;
                case "orange":
                    bar.Style = MetroFramework.MetroColorStyle.Orange;
                    break;
                case "green":
                    bar.Style = MetroFramework.MetroColorStyle.Green;
                    break;
                case "grey":
                    bar.Style = MetroFramework.MetroColorStyle.Silver;
                    break;
                default:
                    bar.Style = MetroFramework.MetroColorStyle.Blue;
                    break;

            }
            box.Controls.Add(bar);

            //textbox
            RichTextBox taskInfo = new RichTextBox();
            taskInfo.Font = new Font("Century Gothic", 8, FontStyle.Regular);
            taskInfo.Name = "taskInfo";          
            taskInfo.ReadOnly = true;
            taskInfo.Multiline = true;
            taskInfo.ScrollBars = RichTextBoxScrollBars.Vertical; //verticle
            taskInfo.BorderStyle = BorderStyle.None;
            taskInfo.BackColor = SystemColors.ControlLightLight;
      
            taskInfo.Location = new Point(6, 22);
            taskInfo.Text = info; //set info
            taskInfo.Width = 324;
            h = 28;
            taskInfo.Height = 28;
            taskInfo.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            if (pinned)
            {
                GetTheme t = new GetTheme();
                string theme = t.setTheme();
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
                    default:
                        themeC = Color.FromArgb(0, 174, 219);
                        break;

                }
                box.ForeColor = themeC;
                taskInfo.ForeColor = themeC;
                box.Text = "📌 " + title.ToUpper(); //set title
                //defaultC = themeC;
                box.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                taskInfo.Font = new Font("Century Gothic", 9, FontStyle.Regular);

            }
            box.Controls.Add(taskInfo);

            //timeleft label
            Label timeLeft = new Label();
            timeLeft.Font = new Font("Century Gothic", 8, FontStyle.Regular);
            timeLeft.Name = "timeleft";
            timeLeft.AutoSize = false;
            timeLeft.Location = new Point(7, 78);
            timeLeft.Size = new Size(323, 13);
            timeLeft.Text = "timeleft"; //set this later
            timeLeft.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            box.Controls.Add(timeLeft);
            
            //done button
            Button DoneBtn = new Button();
            DoneBtn.Text = "DONE";
            DoneBtn.Name = "DoneBtn";
            DoneBtn.Font = new Font("Century Gothic", 9, FontStyle.Regular);
            DoneBtn.FlatStyle = FlatStyle.Flat;
            DoneBtn.TextAlign = ContentAlignment.TopCenter;
            DoneBtn.BackColor = SystemColors.ControlLightLight;
            DoneBtn.AutoSize = false;
            DoneBtn.Location = new Point(255, 99);
            DoneBtn.Size = new Size(75, 25);
            DoneBtn.Anchor = (AnchorStyles.Top | AnchorStyles.Left );
            DoneBtn.Cursor = Cursors.Hand;
            box.Controls.Add(DoneBtn);

            //info button
            Button infoBtn = new Button();
            infoBtn.Text = "INFO";
            infoBtn.Name = "infoBtn";
            infoBtn.Font = new Font("Century Gothic", 9, FontStyle.Regular);
            infoBtn.FlatStyle = FlatStyle.Flat;
            infoBtn.TextAlign = ContentAlignment.TopCenter;
            infoBtn.BackColor = SystemColors.ControlLightLight;
            infoBtn.AutoSize = false;
            infoBtn.Location = new Point(6, 99);
            infoBtn.Size = new Size(75, 25);
            infoBtn.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            infoBtn.Cursor = Cursors.Hand;
            box.Controls.Add(infoBtn);

            box.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            box.Refresh();
            
            return box;

        }

        public int geth()
        {
            return h;
        }
    }
}
