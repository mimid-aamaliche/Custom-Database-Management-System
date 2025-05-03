using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_DB_Management_System
{
    public class clsGeneral
    {


        public static string Delemtre = "=>>>";

        static public  List<string> SplitLine(string Line,string Delemtre="#//#")
        {
            int pos = 0;
            List<string> Items = new List<string>();

            while (pos !=-1) {

                pos = Line.IndexOf(Delemtre);

                if ((pos==-1))
                {
                    Items.Add(Line);
                    break;
                }

                Items.Add(Line.Substring(0,pos));

                Line= Line.Substring(pos + Delemtre.Length);

            
            
            }




            return Items;   




        }


        public static int OffSetLengthInFile = 8;



    }
}
