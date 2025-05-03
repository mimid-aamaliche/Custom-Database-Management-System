using My_DB_Management_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_DataBase_Query_Stuff
{
    public class clsRecordSet
    {


        public Dictionary<string,Column> SetColumns {  get; set; }
        public List<Dictionary<string,object>> SetRecords {  get; set; }


        public clsRecordSet()
        {
            SetColumns = new Dictionary<string,Column>();
            SetRecords = new List<Dictionary<string,object>>();
        }



        //Insert / add record to records

        public void AddRecord(Dictionary<string, object> rec)
        {

            SetRecords.Add(new Dictionary<string, object>(rec));



        }

        public void Clear()
        {
            SetColumns.Clear();
            SetRecords.Clear();
        }


        //clear Records




        //print Records













    }
}
