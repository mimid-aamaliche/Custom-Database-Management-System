using My_DB_Management_System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace My_DataBase_Query_Stuff
{
    public class clsCommand
    {
        private  string _DataBaseName { get; set; }
        public string ExecutionResultMessage { get; set; }
        private string _DataBaseDirectoryPath { get; set; }
        public TablesGraph _DataBaseStructure { get; set; }
        public clsRecordSet Records { get;private set; }









        //Constructure
        // string database path , database name
        public clsCommand(string DataBasePath,string DataBaseName) {
            
            _DataBaseDirectoryPath = DataBasePath;
            
            _DataBaseName = DataBaseName;


            _DataBaseStructure = new TablesGraph(_DataBaseDirectoryPath, _DataBaseName);

            Records = new clsRecordSet();



        }








        //Add(Insert) Record
        //Record 

        //Add A record In dictionnary
        public void Add(string TableName, Dictionary<string, object> Record)
        {
            var OurTable = _DataBaseStructure._vertexDictionary.Keys.Where(t => t.Name == TableName).First();

            //join the Dictionary element(Values) ordered by the Column Index
            var OrderedValuesAccordingToColumnIndex = Record.OrderBy(kvp => (OurTable.Columns.TryGetValue(kvp.Key, out Column CurrentColumn) ? CurrentColumn.ColumnIndex : 0)).Select(kvp => kvp.Value).ToList();



            string Line = string.Join(clsGeneral.Delemtre, OrderedValuesAccordingToColumnIndex);

            //Get the PKs Values Conbined for the sorting
            StringBuilder ValueOfPrimaryKeys = new StringBuilder();
            foreach (var pk in OurTable.PrimaryKeys)
            {
                ValueOfPrimaryKeys.Append(OrderedValuesAccordingToColumnIndex[pk.index]);
            }

            AddRecord(OurTable, Line, ValueOfPrimaryKeys.ToString());

        }

        //Add a List of string(Columns)
        //you should pay attention to the order of elements in the list
        public void Add(string TableName, List<string> Record)
        {
            var OurTable = _DataBaseStructure._vertexDictionary.Keys.Where(t => t.Name == TableName).First();

            string Line = string.Join(clsGeneral.Delemtre, Record);

            //Get the PKs Values Conbined for the sorting
            StringBuilder ValueOfPrimaryKeys = new StringBuilder();
            foreach (var pk in OurTable.PrimaryKeys)
            {
                ValueOfPrimaryKeys.Append(Record[pk.index]);
            }

            AddRecord(OurTable, Line, ValueOfPrimaryKeys.ToString());

        }








        //****************************************Add Record Related :
        
        private void AddRecord(Table OurTable, string Line,string pkValuesCombined)
        {


            //Insert the Record
            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, OurTable.Name); 


            //Find the correct position to insert the line and keep the list sorted
            int RecordPosition=_FindPositionAndSortIndexFileByPrimaryKeys(TableFilePath, pkValuesCombined, OurTable);

            //insert the line at the end and it offset at the end too
            InsertRecord(TableFilePath, OurTable.Name, Line);

            //Sort the index File 
            _SortIndexFile(TableFilePath, OurTable.Name, RecordPosition);

            //if all is good incriment the number of columns
            OurTable.NumberOfRecords++;

        }
        private void InsertRecord(string TablePath,string TableName,string Line)
        {
            //First Insert The Line At the end of the file and it s Offset in the index file
            
            string OffsetInStringFormat = "";

            using (StreamWriter sr = new StreamWriter(TablePath +"\\"+ TableName + "_Records.txt",true, new UTF8Encoding(false))) {
            
                sr.Flush();
                OffsetInStringFormat = sr.BaseStream.Position.ToString();
                sr.WriteLine(Line);
            
                sr.Close();
            }

            using (StreamWriter sr = new StreamWriter(TablePath + "\\" + "index.txt",true,new UTF8Encoding(false)))
            {

                //to keep a fixed offset length
                //so accessing the Offset is easy
               
                string OffSetFormated = OffsetInStringFormat.PadLeft(clsGeneral.OffSetLengthInFile, '0');
                sr.Write(OffSetFormated);

                sr.Close();
            }


            




        }
        private void _SortIndexFile(string TableFilePath, string TableName,int RecordIndex)
        {

            int characterAtCurrentPosition;
            int characterAtLastPosition;

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, "index.txt"), FileMode.Open,FileAccess.ReadWrite))
            {
                long StreamLength = fs.Length;
                

                for (int pos = RecordIndex; pos * clsGeneral.OffSetLengthInFile < StreamLength; pos++)
                {

                    //for each OffSet(word that starts at pos and end at pos+ clsGeneral.OffSetLengthInFile
                    //we will swap the values between the first offset and the last offset in file
                    for (int i=0;i< clsGeneral.OffSetLengthInFile; i++)
                    {

                        //Get the CurrentByte
                        fs.Seek(pos * clsGeneral.OffSetLengthInFile+i, SeekOrigin.Begin);
                        characterAtCurrentPosition = fs.ReadByte();

                        //get the LastWord[i]
                        fs.Seek(fs.Length - (clsGeneral.OffSetLengthInFile-i), SeekOrigin.Begin);
                        characterAtLastPosition = fs.ReadByte();

                        //Save the current Value at the corresponding index of the last OffSet
                        fs.Seek(fs.Length - (clsGeneral.OffSetLengthInFile - i), SeekOrigin.Begin);
                        fs.WriteByte((byte)characterAtCurrentPosition);

                        //Go to the Current Position and Save The Value
                        fs.Seek(pos * clsGeneral.OffSetLengthInFile + i, SeekOrigin.Begin);
                        fs.WriteByte((byte)characterAtLastPosition);

                        fs.Flush();
                    }





                



                }

                fs.Close();
            }


        }
        private int _FindPositionAndSortIndexFileByPrimaryKeys(string TableFilePath, string PrimaryKeysValuesCombined, Table OurTable)
        {


            //what this method will do is 
            //after an insertion
            // we inserted the new record at last and it s offset too 
            // new we should iterat over the records and compare them to our record 
            // note we compare PK
            //and when we find the position of our record 
            //we swap the of set values till the end of the file

         
            List<string> RecordItems;
            string PKCombined = "";
            int RecordIndex = 0;
         
             

            ////2 method: Binary Search Using Offsets in the index file

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, "index.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(TableFilePath + "\\" + OurTable.Name + "_Records.txt"))
            {
                if (OurTable.NumberOfRecords == 0) { return 0; }

                // Binary Search Implimentation on files
                int Inf = 0;
                int Sup = OurTable.NumberOfRecords - 1;//NumberOfRecords: i still need to start saving and loading the records count in structure file
                RecordIndex = Inf + ((Sup - Inf) / 2);

                byte[] OffSetByteFormat = new byte[clsGeneral.OffSetLengthInFile];

                while (Inf <= Sup)
                {


                    //1) seek to the med * OffSetLength in index file

                    fs.Seek(RecordIndex * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                    fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);
                    if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat).Trim(), out int offset))
                    {

                        //now using that offset we need to read the line
                        sr.BaseStream.Position = offset;
                        sr.DiscardBufferedData(); // Make sure StreamReader reads from the new position
                        //either this above or just use seek with discardbuffereddata  method
                        



                        RecordItems = clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);


                        PKCombined = string.Empty;
                        //here we can get the primary keys of the table and there index in file records
                        foreach (var pk in OurTable.PrimaryKeys)
                        {
                            PKCombined += RecordItems[pk.index];
                        }

                        //Or like this using Link 
                        // PKCombined = string.Concat(OurTable.PrimaryKeys.Select(kvp => RecordItems[kvp.index]).ToList());
                        if (PKCombined.Length > PrimaryKeysValuesCombined.Length)
                            PrimaryKeysValuesCombined=PrimaryKeysValuesCombined.PadLeft(PKCombined.Length, '0');
                        else
                            PKCombined=PKCombined.PadLeft(PrimaryKeysValuesCombined.Length, '0');



                        int comparasionResult= string.CompareOrdinal(PKCombined, PrimaryKeysValuesCombined);
                        // int comparasionResult = PKCombined.CompareTo(PrimaryKeysValuesCombined.PadLeft(PKCombined.Length, '0'));
                        if (comparasionResult > 0)
                        {
                            

                            Sup = RecordIndex - 1;



                        }
                        else if (comparasionResult < 0)
                        {
                            

                            Inf = RecordIndex + 1;



                        }
                        else if (comparasionResult == 0)
                        {
                           
                            throw new Exception($"Primary Key already exist in the table {OurTable.Name} ");

                         



                        }



                        RecordIndex = Inf + ((Sup - Inf) / 2);

                    }

                    //2) use that Offset to get the Line Record compare the Records
                    // How to read the line
                    //        1)seek to the Offset
                    //        3)Start Reding byte in a while loop (while(utf.getstring( Currentbyte)!="\n")
                    //         then cast the byte array you have to a string 
                    //         split the line
                    //         Cmpare values


                    //3) if our record is greater : inf=med ; med=(Sup-Inf)/2


                    // else if its smaller : Sup=med ; med=(Sup-Inf)/2



                    sr.BaseStream.Flush();



                }

                //at the end if sup is our position 







                fs.Close();
                sr.Close();

                return RecordIndex;
            }





            return RecordIndex;


         



        }











        //******************************************Read Opearions 

        //Get / Select(table name , condition)
        public void ClearResultSet()
        {
            Records.Clear();
        }

        //here the user can have condition on any field of the table
        public void Select(string TableName,List<string> Columns, Func<Dictionary<string, object>, bool> predicate)
        {


            if(Columns==null || Columns.Count == 0)
            {
                //Return nothing
                return ;
            }

            var ourTable = _DataBaseStructure._vertexDictionary.Keys.Where(kvp => kvp.Name == TableName).ToList().First();
            

            if (Columns.Contains("*"))
            {
                //Return All Columns

                Records.SetColumns = new Dictionary<string, Column>(ourTable.Columns);
                   
               

            }
            else
            {
                foreach(var item in Columns)
                {
                    if(ourTable.Columns.TryGetValue(item,out Column v))
                    {
                        Records.SetColumns.Add(item, v);

                    }
                    else
                    {
                        throw new Exception($"Invalide Column Name (Column with the name {item} in table {ourTable.Name} was not found");
                    }

                }


            }



            //Add The Column We Want Returned Ro The Records.SetColumns
            


            Select(TableName, predicate,ourTable);

        }

       


        //this is the normal select
        private void Select(string TableName,Func<Dictionary<string,object>,bool> predicate,Table ourTable)
        {
            Records.SetRecords.Clear();

            //Note the Records In the Records.SetColumns
            Dictionary<string, object> Record = new Dictionary<string, object>();

            
            //


            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, TableName);
            byte[] OffSetByteFormat = new byte[clsGeneral.OffSetLengthInFile];

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, "index.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(TableFilePath + "\\" + TableName + "_Records.txt"))
            {


                for (int i = 0; i < fs.Length / clsGeneral.OffSetLengthInFile; i++)
                {

                    fs.Seek(i * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                    fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);
                    if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat), out int offset))
                    {

                        //now using that offset we need to read the line
                        sr.BaseStream.Position = offset;
                        sr.DiscardBufferedData();



                        //Get Column Values In Line
                        var elements= clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);
                       
                        //Get The Values Of The Columns We Already Chose
                        foreach(var Column in ourTable.Columns.Values)
                        {
                            if(Records.SetColumns.ContainsKey(Column.Name))
                                Record.Add(Column.Name, elements[Column.ColumnIndex]);

                        }



                        //If The Record Meets Our Condition We Add it To The Result Set
                        if (predicate(Record))
                        {

                          
                            Records.AddRecord(Record);
                            
            

                        }

                        





                    }




                    Record.Clear();

                }










                fs.Close();
                sr.Close();

            }






        }


        // select that uses binary search( gonna be used if the user is quering data by primary keys or an indexed Column )











        public List<Dictionary<string, object>> Select(string TableName,string IndexFileName, List<string> ResultSetColumns,(string cName, string cValue) IndexedColumnValue)
        {
            //Insert the Record
            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, TableName);


            if (ResultSetColumns == null || ResultSetColumns.Count == 0)
            {
                //Return nothing
                return null;
            }

            var ourTable = _DataBaseStructure._vertexDictionary.Keys.Where(kvp => kvp.Name == TableName).ToList().First();


            if (ResultSetColumns.Contains("*"))
            {
                //Return All Columns

                Records.SetColumns = ourTable.Columns;


            }
            else
            {
                foreach (var item in ResultSetColumns)
                {
                    if (ourTable.Columns.TryGetValue(item, out Column v))
                    {
                        Records.SetColumns.Add(item, v);

                    }
                    else
                    {
                        throw new Exception($"Invalide Column Name (Column with the name {item} in table {ourTable.Name} was not found");
                    }

                }


            }




            return SelectByOneColumn(TableFilePath, ourTable,IndexedColumnValue.cName, IndexFileName,  kvp =>
            {
                //gets the String of pkValues Combined and Soerted according to the Column Index
                

                if (kvp.cValue.Length > IndexedColumnValue.cValue.Length)
                    IndexedColumnValue.cValue = IndexedColumnValue.cValue.PadLeft(kvp.cValue.Length, '0');
                else
                    kvp.cValue = kvp.cValue.PadLeft(IndexedColumnValue.cValue.Length, '0');


                return string.CompareOrdinal(kvp.cValue, IndexedColumnValue.cValue); ;






            });






        }
        private List<Dictionary<string, object>> SelectByOneColumn(string TableFilePath, Table OurTable,string IndexedColumnName,string IndexFileName, Func<(string cName, string cValue), int> predicate)
        {

            List<string> RecordItems;
            int RecordIndex = 0;
            Dictionary<string, object> Record = new Dictionary<string, object>();

            //Here we gonna store the records that meet our condition
            List< Dictionary<string, object>> ResultSet= new List< Dictionary<string, object>>();


            ////2 method: Binary Search Using Offsets in the index file

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, IndexFileName), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(TableFilePath + "\\" + OurTable.Name + "_Records.txt"))
            {
                if (OurTable.NumberOfRecords == 0) { return null; }

                // Binary Search Implimentation on files
                int Inf = 0;
                int Sup = OurTable.NumberOfRecords - 1;//NumberOfRecords: i still need to start saving and loading the records count in structure file
                RecordIndex = Inf + ((Sup - Inf) / 2);

                byte[] OffSetByteFormat = new byte[clsGeneral.OffSetLengthInFile];

                while (Inf <= Sup)
                {
                    RecordIndex = Inf + ((Sup - Inf) / 2);


                    //1) seek to the med * OffSetLength in index file

                    fs.Seek(RecordIndex * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                    fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);
                    if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat).Trim(), out int offset))
                    {

                        //now using that offset we need to read the line
                        sr.BaseStream.Position = offset;
                        sr.DiscardBufferedData(); // Make sure StreamReader reads from the new position
                                                  //either this above or just use seek with discardbuffereddata  method




                        RecordItems = clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);


                        //Get The Values Of The Columns We Already Chose
                        foreach (var Column in OurTable.Columns.Values)
                        {
                            if (Records.SetColumns.ContainsKey(Column.Name))
                                Record.Add(Column.Name, RecordItems[Column.ColumnIndex]);

                        }
                        int ComparaisonResult = predicate(Record.Where(kvp=> string.CompareOrdinal(kvp.Key, IndexedColumnName)==0).Select(KeyValuePaire=>(KeyValuePaire.Key, KeyValuePaire.Value.ToString())).First());

                        if (ComparaisonResult == 0)
                        {


                            //here if we found a record that satisfies the condition we start fetching data from this OffSet
                            //Fetching the Records That Come befor this current record and Satisfies the condition

                            //the current record OffSet is in the ofset variable {offset}
                            //But We Will Not Be Working With It WillBe Working With The Record Index 

                            //First Store The Record That we Found
                            ResultSet.Add(new Dictionary<string,object>(Record));

                            Record.Clear();


                            int j =RecordIndex-1;

                            while (j >= 0)
                            {

                                fs.Seek(j * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                                fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);

                                if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat).Trim(), out offset))
                                {

                                    sr.BaseStream.Position = offset;
                                    sr.DiscardBufferedData();

                                    RecordItems = clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);


                                    //Get The Values Of The Columns We Already Chose
                                    foreach (var Column in OurTable.Columns.Values)
                                    {
                                        if (Records.SetColumns.ContainsKey(Column.Name))
                                            Record.Add(Column.Name, RecordItems[Column.ColumnIndex]);

                                    }
                                     if(predicate(Record.Where(kvp => string.CompareOrdinal(kvp.Key, IndexedColumnName) == 0).Select(KeyValuePaire => (KeyValuePaire.Key, KeyValuePaire.Value.ToString())).First()) == 0)
                                    {
                                        //if the record meets the condition we added and move on to the other
                                        ResultSet.Add(new Dictionary<string, object>(Record));
                                        j--;

                                    }
                                    else
                                    {
                                        break;
                                    }



                                    Record.Clear();

                                }
                                else
                                {
                                    throw new Exception("Casting went wrong ,in the SelectByColumn Method in Command Class");
                                }

                                




                            }


                            Record.Clear();


                            //Fetching the Data that Comes after the record and satisfies the condition
                            //reassigne the j to the record right after our First Record we found
                            j = RecordIndex + 1 ;

                            while (j < OurTable.NumberOfRecords)
                            {

                                fs.Seek(j * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                                fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);

                                if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat).Trim(), out offset))
                                {

                                    sr.BaseStream.Position = offset;
                                    sr.DiscardBufferedData();

                                    RecordItems = clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);


                                    //Get The Values Of The Columns We Already Chose
                                    foreach (var Column in OurTable.Columns.Values)
                                    {
                                        if (Records.SetColumns.ContainsKey(Column.Name))
                                            Record.Add(Column.Name, RecordItems[Column.ColumnIndex]);

                                    }
                                    if (predicate(Record.Where(kvp => string.CompareOrdinal(kvp.Key, IndexedColumnName) == 0).Select(KeyValuePaire => (KeyValuePaire.Key, KeyValuePaire.Value.ToString())).First()) == 0)
                                    {
                                        //if the record meets the condition we added and move on to the other
                                        ResultSet.Add(new Dictionary<string, object>(Record));
                                        //move to the next record index
                                        j++;

                                    }
                                    else
                                    {
                                        break;
                                    }


                                    Record.Clear();


                                }
                                else
                                {
                                    throw new Exception("Casting went wrong ,in the SelectByColumn Method in Command Class");
                                }






                            }





                            return ResultSet;

                        }
                        else if (ComparaisonResult > 0)
                        {
                            //this means that this record comes after our record


                            Sup = RecordIndex - 1;


                        }
                        else
                        {


                            Inf = RecordIndex + 1;

                        }




                        Record.Clear();

                    }


                    sr.BaseStream.Flush();



                }







                fs.Close();
                sr.Close();

                return null;
            }








        }




        public Dictionary<string, object> Find( string TableName, List<string> Columns,List<(string cName,string cValue)> PKValues)
        {
            //Insert the Record
            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, TableName);


            if (Columns == null || Columns.Count == 0)
            {
                //Return nothing
                return null;
            }

            var ourTable = _DataBaseStructure._vertexDictionary.Keys.Where(kvp => kvp.Name == TableName).ToList().First();


            if (Columns.Contains("*"))
            {
                //Return All Columns

                Records.SetColumns = ourTable.Columns;


            }
            else
            {
                foreach (var item in Columns)
                {
                    if (ourTable.Columns.TryGetValue(item, out Column v))
                    {
                        Records.SetColumns.Add(item, v);

                    }
                    else
                    {
                        throw new Exception($"Invalide Column Name (Column with the name {item} in table {ourTable.Name} was not found");
                    }

                }


            }




            return Find(TableFilePath, ourTable, kvp =>
            {
                //gets the String of pkValues Combined and Soerted according to the Column Index
                string PKs = string.Concat(kvp.Where(KeyValuePair => ourTable.PrimaryKeys.Select(TupleCouple => TupleCouple.name).Contains(KeyValuePair.Key)).Select(pk_v=>pk_v.Value));

                string MyRecordPKs = string.Concat(PKValues.OrderBy(RecordInfo => ourTable.Columns.TryGetValue(RecordInfo.cName, out Column c) ? c.ColumnIndex : 1).Select(r=>r.cValue));


                if (PKs.Length > MyRecordPKs.Length)
                    MyRecordPKs= MyRecordPKs.PadLeft(PKs.Length, '0');
                else
                    PKs=PKs.PadLeft(MyRecordPKs.Length, '0');


                return string.CompareOrdinal(PKs, MyRecordPKs); ;






            });






        }
        private Dictionary<string, object> Find(string TableFilePath,Table OurTable, Func<Dictionary<string, object>, int> predicate)
        {

            List<string> RecordItems;
            int RecordIndex = 0;
            Dictionary<string, object> Record = new Dictionary<string, object>();



            ////2 method: Binary Search Using Offsets in the index file

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, "index.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(TableFilePath + "\\" + OurTable.Name + "_Records.txt"))
            {
                if (OurTable.NumberOfRecords == 0) { return null; }

                // Binary Search Implimentation on files
                int Inf = 0;
                int Sup = OurTable.NumberOfRecords - 1;//NumberOfRecords: i still need to start saving and loading the records count in structure file
                RecordIndex = Inf + ((Sup - Inf) / 2);

                byte[] OffSetByteFormat = new byte[clsGeneral.OffSetLengthInFile];

                while (Inf <= Sup)
                {
                    RecordIndex = Inf + ((Sup - Inf) / 2);


                    //1) seek to the med * OffSetLength in index file

                    fs.Seek(RecordIndex * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                    fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);
                    if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat).Trim(), out int offset))
                    {

                        //now using that offset we need to read the line
                        sr.BaseStream.Position = offset;
                        sr.DiscardBufferedData(); // Make sure StreamReader reads from the new position
                                                  //either this above or just use seek with discardbuffereddata  method




                        RecordItems = clsGeneral.SplitLine(sr.ReadLine(), clsGeneral.Delemtre);


                        //Get The Values Of The Columns We Already Chose
                        foreach (var Column in OurTable.Columns.Values)
                        {
                            if (Records.SetColumns.ContainsKey(Column.Name))
                                Record.Add(Column.Name, RecordItems[Column.ColumnIndex]);

                        }

                        int ComparaisonResult = predicate(Record);

                        if (ComparaisonResult == 0)
                        {

                            return Record;

                        }
                        else if(ComparaisonResult > 0)
                        {
                            //this means that this record comes after our record


                            Sup = RecordIndex - 1;


                        }
                        else
                        {


                            Inf = RecordIndex + 1;

                        }




                        Record.Clear();

                    }

                   
                    sr.BaseStream.Flush();



                }

               





                fs.Close();
                sr.Close();

                return null;
            }








        }




































        private void RenameFile(string path,string oldName,string newFileName)
        {
            if (!File.Exists(Path.Combine(path,oldName)))
            {
                throw new IOException($"Invalid Path(Not Found) ({Path.Combine(path,oldName)})");
            }
            if (File.Exists(Path.Combine(path, newFileName)))
            {
                throw new IOException("Invalid new File Name , there is already a file with that name (in The rename File Method In class Commands)");
            }

            File.Move(Path.Combine(path, oldName), Path.Combine(path, newFileName));

        }

       





        //update  (Table Name,with bool delegation)²
        public void Update(string TableName,Dictionary<string,object> NewValues, Func<Dictionary<string, object>, bool> predicate)
        {
            //How would we UpDate 
            //method 1 second file method
            //method 2 iterate over the file and whene an update is needed just write above the old data,but this risky because we are working with offsets 
            //and there is nothing that garanti that will always have data tha same length 



            //Variable will be needing
            var OurTable = _DataBaseStructure._vertexDictionary.Keys.Where(t => t.Name == TableName).First();
            Dictionary<string, object> ColumnValuePaires = new Dictionary<string, object>();
            string TableFilePath =Path.Combine(_DataBaseDirectoryPath,_DataBaseName,TableName);
           
            
            //Very Important to initialize number of records in the table to 0
            //because we are using the add method wich uses the NumberOfRecords Value
            OurTable.NumberOfRecords = 0;
            
            //we will use method 2

            //rename the records file from record to records_v2
            RenameFile(TableFilePath, TableName + "_Records.txt", TableName+"_Records_v2.txt");

            
            //creat the the records file that will conatin the new data
            File.Create(Path.Combine(TableFilePath, TableName+ "_Records.txt")).Close();



            //Delete The Data At The Index File
            File.Create(Path.Combine(TableFilePath, "index.txt")).Close();

            


            //Copy Data and when we find data that meets the conditions at the predicate we enter the new data not the old 
            List<string> record;
            using (StreamReader sr = new StreamReader(Path.Combine(TableFilePath, TableName + "_Records_v2.txt")))
            {
                while (!sr.EndOfStream)
                {
                    //prepare the predicate input
                    string Line=sr.ReadLine();
                    record = clsGeneral.SplitLine(Line,clsGeneral.Delemtre);

                    foreach(var c in OurTable.Columns)
                    {
                        ColumnValuePaires.Add(c.Key, record[c.Value.ColumnIndex]);

                    }


                    if (predicate(ColumnValuePaires))
                    {
                        //here we make the changes and then we right the record
                        foreach(var kvp in NewValues)
                        {
                            if(ColumnValuePaires.ContainsKey(kvp.Key))
                            {
                                ColumnValuePaires[kvp.Key] = kvp.Value;

                            }
                            else
                            {
                                throw new Exception($"Invalide Column Name , there is no column with the name {kvp.Key} in table {TableName}");
                            }
                        }
                        
                        this.Add(TableName,ColumnValuePaires);

                    }
                    else
                    {
                        this.Add(TableName,record);
                    }


                    ColumnValuePaires.Clear();
                    record.Clear();

                }
               








                sr.Close();
            }







            //Delete the Old File with old data


            File.Delete(Path.Combine(TableFilePath, TableName + "_Records_v2.txt"));


            //Done.






        }

















        //delete  (Table Name,with bool delegation)
        public void Delete(string TableName,Func<Dictionary<string,object>,bool> predicate)
        {

         

            //How would we UpDate 
            //method 1 second file method
            //method 2 iterate over the file and whene an update is needed just write above the old data,but this risky because we are working with offsets 
            //and there is nothing that garanti that will always have data tha same length 



            //Variable will be needing
            var OurTable = _DataBaseStructure._vertexDictionary.Keys.Where(t => t.Name == TableName).First();
            Dictionary<string, object> ColumnValuePaires = new Dictionary<string, object>();
            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, TableName);


            //Very Important to initialize number of records in the table to 0
            //because we are using the add method wich uses the NumberOfRecords Value
            OurTable.NumberOfRecords = 0;


            //we will use method 2

            //rename the records file from record to records_v2
            RenameFile(TableFilePath, TableName + "_Records.txt", TableName + "_Records_v2.txt");

            //creat the the records file that will conatin the new data
            File.Create(Path.Combine(TableFilePath, TableName + "_Records.txt")).Close();


            //Delete The Data At The Index File
            File.Create(Path.Combine(TableFilePath, "index.txt")).Close();



            //Copy Data that does not meet the predicate conditions 
            List<string> record;
            using (StreamReader sr = new StreamReader(Path.Combine(TableFilePath, TableName + "_Records_v2.txt")))
            {
                while (!sr.EndOfStream)
                {
                    //prepare the predicate input
                    string Line = sr.ReadLine();
                    record = clsGeneral.SplitLine(Line, clsGeneral.Delemtre);

                    foreach (var c in OurTable.Columns)
                    {
                        ColumnValuePaires.Add(c.Key, record[c.Value.ColumnIndex]);

                    }


                    if (predicate(ColumnValuePaires))
                    {
                        //if the record meets the conditions in the predicate we do nothing
                        

                    }
                    else
                    {
                        //if not we add the record to our file
                        this.Add(TableName, record);
                    }


                    ColumnValuePaires.Clear();
                    record.Clear();

                }









                sr.Close();
            }







            //Delete the Old File with old data


            File.Delete(Path.Combine(TableFilePath, TableName + "_Records_v2.txt"));


            //Done.




        }




























        //Print the Content Of Tbale s File
        public void PrintTableRecordSorted(string TableName)
        {

            string TableFilePath = Path.Combine(_DataBaseDirectoryPath, _DataBaseName, TableName);

            byte[] OffSetByteFormat=new byte[clsGeneral.OffSetLengthInFile];

            using (FileStream fs = new FileStream(Path.Combine(TableFilePath, "index.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(TableFilePath + "\\" + TableName + "_Records.txt"))
            {


                for(int i = 0;i< fs.Length/clsGeneral.OffSetLengthInFile; i++)
                {
                    //1) seek to the med * OffSetLength in index file

                    fs.Seek(i * clsGeneral.OffSetLengthInFile, SeekOrigin.Begin);
                    fs.Read(OffSetByteFormat, 0, OffSetByteFormat.Length);
                    if (int.TryParse(Encoding.UTF8.GetString(OffSetByteFormat), out int offset))
                    {

                        //now using that offset we need to read the line
                        sr.BaseStream.Position = offset;
                        sr.DiscardBufferedData();

                        Console.WriteLine(sr.ReadLine());
                        //Console.WriteLine(sr.ReadLine());


                        



                    }






                }
              



               





                fs.Close();
                sr.Close();

            }



        }














    }
}
