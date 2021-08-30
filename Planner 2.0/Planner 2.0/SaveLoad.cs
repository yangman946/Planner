using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Planner_2._0
{
    [Serializable()]
    class SaveLoad : ISerializable
    {
        //save variables
        private DateTime[] start { get; set; }
        private DateTime[] end { get; set; }
        private string projectTitle { get; set; }

        //group box controls
        //private GroupBox[] boxes { get; set; }
        private string[] Tasktitle { get; set; }
        private string[] infor { get; set; }
        
        private bool[] ispinned { get; set; }
        

        public SaveLoad() { }

        public SaveLoad(DateTime[] Start, DateTime[] End, string project, string[] Task, string[] information, bool[] pinned) //set 
        {
            start = Start;
            end = End;
            projectTitle = project;

            //groupboxes
            Tasktitle = Task;
            infor = information;
            //boxes = b;

            ispinned = pinned;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) //enable serializing
        {
            info.AddValue("Start", start); //serializes each array into a variable.
            info.AddValue("End", end);
            info.AddValue("Title", projectTitle);


            //info.AddValue("boxes", boxes);
            info.AddValue("taskTitle", Tasktitle);
            info.AddValue("information", infor);
            info.AddValue("pinned", ispinned);
        }

        public override string ToString()
        {
            if (start.Length > 0)
            {
                return string.Format("time started: {0}, and ended: {1} ", start[0], end[0]);
            }
            else
            {
                return null;
            }
            
        }

        public string getTitle()
        {
            return projectTitle;
        }

        public string[] getTaskTitle()
        {
            return Tasktitle;

        }

        public string[] getInfo()
        {
            return infor;
        }

        public DateTime[] getStart()
        {
            return start;
        }

        public DateTime[] getEnd()
        {
            return end;
        }
        //public GroupBox[] getBoxes()
        //{
            //return boxes;
        //}
        public bool[] getpinned()
        {
            return ispinned;
        }

        public SaveLoad(SerializationInfo info, StreamingContext context) //deserialize
        {
            start = (DateTime[])info.GetValue("Start", typeof(DateTime[]));
            end = (DateTime[])info.GetValue("End", typeof(DateTime[]));
            projectTitle = (string)info.GetValue("Title", typeof(string));
            //boxes = (GroupBox[])info.GetValue("boxes", typeof(GroupBox[]));
            Tasktitle = (string[])info.GetValue("taskTitle", typeof(string[]));
            infor = (string[])info.GetValue("information", typeof(string[]));
            try
            {
                ispinned = (bool[])info.GetValue("pinned", typeof(bool[]));
            }
            catch
            {
                ispinned = new bool[start.Length];
                for (int i = 0; i < ispinned.Length; i++)
                {
                    ispinned[i] = false;
                }
            }
            
        }
    }
}
