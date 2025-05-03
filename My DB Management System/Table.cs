using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace My_DB_Management_System
{
    public class Table
    {

        //Table name
        public string Name { get; set; }

        public int NumberOfRecords { get; set; }

        //a set of Key value pairs string is the name of the column and the value is the object column that has all info about  it
        public Dictionary<string,Column> Columns { get; set; }

        public List<(string name,int index)> PrimaryKeys { get; set; }

        //each record is a set of key value pairs (Key is the name of column and object is the value)
        public List<Dictionary<string,object>> Records { get; private set; }

        //Constructure Used when we are creating a new database
        public Table(string name)
        {
            Name = name;
            Records = new List<Dictionary<string,object>>();
            Columns = new Dictionary<string,Column>();
        }


        //Constructor Used when we are Editing an existing DataBase Or some Data Manupilation

        public Table(string TableName,string DataBasePath) {

            this.Name=TableName;

            Records = new List<Dictionary<string, object>>();
            Columns = new Dictionary<string, Column>();
            PrimaryKeys = new List<(string name, int index)>();
            _LoadTableStructure(TableName, DataBasePath, clsGeneral.Delemtre);


        }



        //methods related to data manupilation
        //plan of action:
        //              1) this name space gonne be for just implimenting the structure of the database
        //              2) make another command/query.dll that uses this dll
        //              3) the command.dll need to have : insert update select delete where
        //              4) first will make this methods and then will make a tokenizer to process long query
        //              

        public void AddRecord(List<(string ColumnName,object Value)> record)
        {
            //Input Validation First






            //Insertion
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
            foreach (var column in record) {

                keyValuePairs.Add(column.ColumnName, column.Value);
                
            
            
            }
            Records.Add(keyValuePairs);

            // we still have to do some sorting so the search is fast 

           



        }

       
        







        //Methods That effect the structure of the table
        public bool AddColumn(Column column)
        {
            if (Columns.ContainsKey(column.Name))
                return false;

            //column info validation
            //not inplimented yet



            //insertion
            column.ColumnIndex =Columns.Count;
            Columns.Add(column.Name, column);
            return true;



        }

        public bool RemoveColumn(string columnName) {
        

            return Columns.Remove(columnName);
        
        
        }

        public bool EditColumn(string columnName,Column NewValues)
        {
            if(!Columns.ContainsKey(columnName)) return false;

            //Validate NewValues First
            //if some field is invalide return false here



            Columns.Remove(columnName);

            AddColumn(NewValues);


            return true;

        }

        public void CreatDir_Files(string Path,string delemtre)
        {
            string FullDirectoryPath = (new StringBuilder(Path).Append("\\" + Name)).ToString();

            try
            {

                if (Directory.Exists(FullDirectoryPath))
                {
                    Console.WriteLine("Directory with the same name was already created");
                }

                //make the Directory of the table
                DirectoryInfo directoryInfo = Directory.CreateDirectory(FullDirectoryPath);

                //make the Records File

                using (StreamWriter sw = File.CreateText((new StringBuilder(FullDirectoryPath).Append("\\" + Name + "_Records.txt")).ToString()))
                { 
                    sw.Close();
                }

                File.CreateText((new StringBuilder(FullDirectoryPath).Append("\\" + "index.txt")).ToString()).Close();


                //making the table structure file
                using (StreamWriter sw = File.CreateText((new StringBuilder(FullDirectoryPath).Append("\\" + Name +"_Structure.txt")).ToString())) {

                    
                    foreach (var c in Columns)
                    {
                        sw.WriteLine(c.Value.ToString());
                    }

                    

                    sw.Close();
                }






            }
            catch (Exception ex)
            {
                Console.WriteLine("Somthing went wrong in Table class"+ex.Message);
            }




        }


        
        private void _LoadTableStructure(string TableName,string DataBasePath,string delemtre)
        {

            string FullDirectoryPath = (new StringBuilder(DataBasePath).Append("\\" + Name)).ToString();

            //Full Directory is the file of the Table which contains(Records file , Table structure file

            if (!Directory.Exists(FullDirectoryPath))
            {
                throw new DirectoryNotFoundException($"Table({TableName}) File Does Not exist");
            }

            //loading the Table columns from the structure fill

            //note to self : change the concatenation methods and use string builder Later

            using (StreamReader sr = new StreamReader(FullDirectoryPath+"\\"+ TableName+ "_Structure.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string Line = sr.ReadLine();

                    var ColumnInfo = clsGeneral.SplitLine(Line, delemtre);

                    // Note : the name of the column is the second element in the list

                    if(ColumnInfo[2] == "True")
                    {
                        PrimaryKeys.Add((ColumnInfo[1], int.TryParse(ColumnInfo[8], out int ColumIndex) ? ColumIndex : -1));
                    }



                    this.Columns.Add(ColumnInfo[1],

                        new Column()
                        {
                            //the first elemnt is the type of the column
                            type = Type.GetType(ColumnInfo[0]),

                            Name = ColumnInfo[1],

                            IsPrimaryKey = ColumnInfo[2] == "True" ? true : false,

                            IsForeignKey = ColumnInfo[3] == "True" ? true : true,

                            IsReadOnly = ColumnInfo[4] == "True" ? true : false,

                            AutoIncriment = ColumnInfo[5] == "True" ? true : true,

                            Seed = int.TryParse(ColumnInfo[6], out int Seedvalue) ? Seedvalue : 0,

                            Start = int.TryParse(ColumnInfo[7], out int Startvalue) ? Startvalue : 0,

                            ColumnIndex = int.TryParse(ColumnInfo[8], out int CIndex) ? CIndex : -1

                        }

                        );


                }

                sr.Close();
            }

            //Get The Number Of Records At The Table
            using (FileStream fs = new FileStream(Path.Combine(FullDirectoryPath, "index.txt"), FileMode.Open, FileAccess.Read))
            { 
                NumberOfRecords = (int)(fs.Length/clsGeneral.OffSetLengthInFile);
                fs.Close();
            }




            }

        /*
            what is left to do here:
                    *method Construct DataBase that calls
                            *construct Table file (a directory with the same name as the table)
                                *inside that file gonna have tow txt files:
                                    *FileName.txt for records
                                    *FileName_Structure for metadata about Table
                                        *like columns ...
                                    *in future:
                                        *we gonna add indexer to improve search time
                               
                    remarque:
                        *inside Database file gonna be a file called DatabaseStructure
                            *inside that gonna be a txt file with the relationships between Tables
         
         
         
         
         
         */



    }
}
