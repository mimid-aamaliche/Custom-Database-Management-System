using My_DB_Management_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_DataBase_Query_Stuff
{
    public class clsQuery
    {

        clsCommand Command { get; set; }



        public clsQuery(string DataBasePath,string DataBaseName){ 
        
            Command=new clsCommand(DataBasePath, DataBaseName);
        
        }


        public clsRecordSet ExecuteQuery(string Query) {
        
           
            //Trim the sides 
            Query = Query.Trim();

            if (Query.Length == 0)
            {
                return null;
            }

            
            //Find The Main Statement
            string FirstStatement = (Query.IndexOf(' ') != -1 )? Query.Substring(0, Query.IndexOf(' ')).ToLower()  :  Query.Substring(0).ToLower();



            if (string.CompareOrdinal(FirstStatement, "select") == 0)
            {
                //Select Statement proces here
                Console.WriteLine("Statement :" + FirstStatement);
                return SelectStatementProcessQuery(Query.Substring(FirstStatement.Length).Trim());

            }
            else if(string.CompareOrdinal(FirstStatement, "update") == 0)
            {
                //update statement process here
                Console.WriteLine("Statement :" + FirstStatement);


            }
            else if (string.CompareOrdinal(FirstStatement, "delete") == 0)
            {
                //delete statement process here
                Console.WriteLine("Statement :" + FirstStatement);


            }
            else if (string.CompareOrdinal(FirstStatement, "insert") == 0)
            {
                //insert statement process here
                Console.WriteLine("Statement :" + FirstStatement);


            }
            else
            {

                throw new Exception($"Invalide query string there no {FirstStatement} statement");

            }







            return null;
        
        }


        private clsRecordSet SelectStatementProcessQuery(string SubQuery)
        {

            int IndexOfKeyWordFrom = SubQuery.IndexOf("from", StringComparison.CurrentCultureIgnoreCase);
            int IndexOfKeyWordWhere = SubQuery.IndexOf("where", StringComparison.CurrentCultureIgnoreCase);
            List<string> TablesExpression;
            List<string> Columns ;

            //lets get the Columns Expression
            if (IndexOfKeyWordFrom == -1)
            {
                throw new Exception("There is no from Key word in Your query.");
            }
            Columns = SubQuery.Substring(0, IndexOfKeyWordFrom).Split(',').ToList();
            
            
            //First we need to delete all the empty element in our list
            Columns.RemoveAll(x => string.IsNullOrEmpty(x));


            //lets get the Tables Expression
            //start index = IndexOfKeyWordFrom + 4 because we want the substring to start right after the from key word
            if (IndexOfKeyWordWhere!=-1)
                 TablesExpression = SubQuery.Substring(IndexOfKeyWordFrom + 4, IndexOfKeyWordWhere - IndexOfKeyWordFrom - 4).Split(' ').ToList();
            else
                 TablesExpression = SubQuery.Substring(IndexOfKeyWordFrom + 4).Split(' ').ToList();

            //First we need to delete all the space element in our list
            TablesExpression.RemoveAll(x => string.IsNullOrEmpty(x));


            if (TablesExpression.Count == 0)
            {
                throw new Exception("There is no Tables To Select from in Your query.");
            }


            //lets get the where expression ( if it does exist )

            if (IndexOfKeyWordWhere == -1)
            {
                //there is no where statement 
                //here we are gonna use the select_all method

                if (TablesExpression.Count == 1)
                {
                    //First the case where we select from one table


                    var ColumnsFormated=Columns.Select(x =>
                    {
                        int PointIndex = x.IndexOf('.');
                        
                        if(PointIndex!= -1)
                        {
                            if (string.CompareOrdinal(x.Substring(0, PointIndex), TablesExpression[0]) != 0)
                                throw new Exception("Invalide prefic at one of the columns");
                            else
                                return x.Substring(PointIndex + 1).Trim();

                        }
                        else
                            return x.Trim();

                    }).ToList();




                    return SelectAll(TablesExpression.First(), ColumnsFormated);




                }
                else
                {
                    //multiple Tables 
                    //here we find the join statement and the On 

                    Console.WriteLine("Select from multiple Table not implimented yet");

                }




                



            }
            else
            {
                //there is a where statement
                //here we gonna use the selectwithcondition method

                Console.WriteLine("Where statement not implimented yet");


            }






            return null;

        }

        private clsRecordSet SelectWithConditions(string TableName,List<string> Columns,string ConditionPart)
        {

            
            


           
            
            
            
            return null;



        }
        private clsRecordSet SelectAll(string TableName, List<string> Columns)
        {

            Console.WriteLine($"Data from Table {TableName} , columns " + string.Join("/", Columns));


            Command.Select(TableName, Columns,x=>true);

            return Command.Records;



            
        }



    }
}
