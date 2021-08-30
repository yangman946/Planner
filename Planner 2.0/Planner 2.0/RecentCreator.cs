// 19/12/19 recent creator by clarence yang
/* this task creates the recent objects shown on the start page
 * based of createTask.cs class
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planner_2._0
{
    class RecentCreator
    {
        public GroupBox getRecentOBJ(string name, string directory)
        {
            GroupBox box = new GroupBox();
            box.Location = new Point(3, 3);
            box.Size = new Size(433, 72);
            box.BackColor = SystemColors.InactiveBorder;
            box.Text = ""; //

            //button
            Button recentBTN = new Button();
            recentBTN.Name = "recentBTN";
            recentBTN.Location = new Point(6, 14);
            recentBTN.Size = new Size(421, 33);
            recentBTN.Font = new Font("Century Gothic", 14, FontStyle.Regular);
            recentBTN.Text = name;
            recentBTN.TextAlign = ContentAlignment.TopLeft;
            recentBTN.FlatAppearance.BorderSize = 0;
            recentBTN.BackColor = SystemColors.InactiveBorder;
            recentBTN.FlatStyle = FlatStyle.Flat;
            recentBTN.Cursor = Cursors.Hand;
            box.Controls.Add(recentBTN);

            TextBox dirText = new TextBox();
            dirText.Location = new Point(12, 51);
            dirText.Size = new Size(415, 14);
            dirText.Font = new Font("Century Gothic", 8, FontStyle.Regular);
            dirText.Text = directory;
            dirText.ReadOnly = true;
            dirText.BorderStyle = BorderStyle.None;
            dirText.BackColor = SystemColors.InactiveBorder;
            box.Controls.Add(dirText);

            return box;
        }
    }
}
