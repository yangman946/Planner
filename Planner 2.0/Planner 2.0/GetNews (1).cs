/*collects news data, this class is now repurposed for internet communications*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace Planner_2._0
{
    class GetNews
    {
        List<string> Info = new List<string>();


        //to be used:
        string command = "";

        public List<string> getInfo()
        {
            //only accepts 4 news titles at the moment
            Info.Clear();
            WebClient WC = new WebClient();
            string html = WC.DownloadString("https://sites.google.com/view/plannerprogram/home");
            MatchCollection m1 = Regex.Matches(html, "<p id=\"h.p_opiU0jDFuZIe\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            MatchCollection m2 = Regex.Matches(html, "<p id=\"h.p_n48ADLFuuc0b\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            MatchCollection m3 = Regex.Matches(html, "<p id=\"h.p_olQw9ZvDuj8R\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            MatchCollection m4 = Regex.Matches(html, "<p id=\"h.p_D9MOXkCjutb0\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);

            //advertisements [must get text element with class zfr3Q]
            MatchCollection m5 = Regex.Matches(html, "<p id=\"h.p_7cW3B_HDJTuy\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            MatchCollection m6 = Regex.Matches(html, "<p id=\"h.p_wHsj1bshKAHt\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            MatchCollection m7 = Regex.Matches(html, "<p id=\"h.p_pkxPDB65hB4P\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);

            foreach (Match m in m1)
            {
                string Item = m.Groups[1].Value;
                Info.Add(Item);
            }
            foreach(Match m in m2)
            {
                string item = m.Groups[1].Value;
                Info.Add(item);
            }
            foreach (Match m in m3)
            {
                string item = m.Groups[1].Value;
                if (item != "")
                    Info.Add(item);

            }
            foreach (Match m in m4)
            {
                string item = m.Groups[1].Value;
                Info.Add(item);
            }

            foreach (Match m in m5)
            {
                string item = m.Groups[1].Value;
                Info.Add(item);
            }
            foreach (Match m in m6)
            {
                string item = m.Groups[1].Value;
            
  
                Info.Add(item);
            }
            foreach (Match m in m7)
            {
                string item = m.Groups[1].Value;
              
            
                Info.Add(item);
            }


            return Info; 
        }

        public string getCommands()
        {

            WebClient WC = new WebClient();
            string html = WC.DownloadString("https://sites.google.com/view/plannerprogram/home");
            MatchCollection m1 = Regex.Matches(html, "<p id=\"h.p_cbjR6_OelQJA\" class=\"zfr3Q\">\\s*(.+?)\\s*</p>", RegexOptions.Singleline);
            foreach (Match m in m1)
            {
                command = m.Groups[1].Value;
                
            }
            return command;
        }
    }
}

