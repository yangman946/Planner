/*handles recent section of start panel. 2/12/19 by clarence yang
 * it should save the recently opened files in a reserved folder
 * it should also load them on start, creating them
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Planner_2._0
{
    [Serializable()]
    class RecentHandler : ISerializable
    {
        private string[] projectNames { get; set; }
        private string[] projectDir { get; set; }

        //private List<string> ListNames = new List<string>();
        //private List<string> ListDir = new List<string>();

        //string saveDir; //our directory where we waved our file
        public RecentHandler() { }

        public RecentHandler(string[] names, string[] directory) //set 
        {
            projectNames = names;
            projectDir = directory;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) //enable serializing
        {
            info.AddValue("projectnames", projectNames); //serializes each array into a variable.
            info.AddValue("projectDirectory", projectDir);
        }

        public string[] getNames()
        {
            return projectNames;

        }

        public string[] getdirectory()
        {
            return projectDir;
        }

        public RecentHandler(SerializationInfo info, StreamingContext context) //deserialize
        {
            projectNames = (string[])info.GetValue("projectnames", typeof(string[]));
            projectDir = (string[])info.GetValue("projectDirectory", typeof(string[]));
        }


    }
}
