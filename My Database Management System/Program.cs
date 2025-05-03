using My_DataBase_Query_Stuff;
using My_DB_Management_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace My_Database_Management_System
{
                      

   
    public class Program
    {
        static void Main(string[] args)
        {
            //Table Student = new Table("Student");
            //Table Teacher = new Table("Teacher");
            //Table Person = new Table("Person");
           

            //List<Table> tables = new List<Table>() {


            //        Student,
            //        Teacher,
            //        Person,
                    

            //    };


            //Student.AddColumn(
            //    new Column()
            //    {
            //        Name = "id",
            //        IsPrimaryKey = true,
            //        IsForeignKey = false,
            //        type = typeof(int),

            //    }

            //    );

            //Student.AddColumn(
            //   new Column()
            //   {
            //       Name = "Name",
            //       IsPrimaryKey = false,
            //       IsForeignKey = false,
            //       type = typeof(string),

            //   }

            //   );

            //Student.AddColumn(
            //   new Column()
            //   {
            //       Name = "Mark",
            //       IsPrimaryKey = false,
            //       IsForeignKey = false,
            //       type = typeof(short),

            //   }

            //   );
            //Student.AddColumn(
            //  new Column()
            //  {
            //      Name = "TeacherID",
            //      IsPrimaryKey = false,
            //      IsForeignKey = true,
            //      type = typeof(int),

            //  }

            //  );


            //Teacher.AddColumn(
            // new Column()
            // {
            //     Name = "id",
            //     IsPrimaryKey = true,
            //     IsForeignKey = false,
            //     type = typeof(int),

            // }

            // );

            //Teacher.AddColumn(
            //new Column()
            //{
            //    Name = "Name",
            //    IsPrimaryKey = false,
            //    IsForeignKey = false,
            //    type = typeof(string),

            //}

            //);


            //Teacher.AddColumn(
            // new Column()
            // {
            //     Name = "Personid",
            //     IsPrimaryKey = true,
            //     IsForeignKey = false,
            //     type = typeof(int),

            // }

            // );

            //Person.AddColumn(
            //new Column()
            //{
            //    Name = "Name",
            //    IsPrimaryKey = false,
            //    IsForeignKey = false,
            //    type = typeof(string),

            //}

            //);
            //Person.AddColumn(
            //new Column()
            //{
            //    Name = "id_person",
            //    IsPrimaryKey = true,
            //    IsForeignKey = false,
            //    type = typeof(int),

            //}

            //);



            //TablesGraph MyTableGraph = new TablesGraph(tables);


            //Console.WriteLine("Befor making any relationships");
            //MyTableGraph.print();


            //MyTableGraph.AddRelationShip(tables[0], tables[1], new stRelationship
            //{
            //    Name = "Student To Teacher relationship",
            //    ColumnSource = "TeacherID",
            //    ColumnDistination = "id"
            //});

            //MyTableGraph.AddRelationShip(tables[1], tables[2], new stRelationship
            //{
            //    Name = "Teacher To Person relationship",
            //    ColumnSource = "TeacherID",
            //    ColumnDistination = "id"
            //});

            //MyTableGraph.AddRelationShip(tables[2], tables[1], new stRelationship
            //{
            //    Name = "person To teacher relationship",
            //    ColumnSource = "TeacherID",
            //    ColumnDistination = "id"
            //});

            //MyTableGraph.AddRelationShip(tables[2], tables[0], new stRelationship
            //{
            //    Name = "person To student relationship",
            //    ColumnSource = "TeacherID",
            //    ColumnDistination = "id"
            //});





            //Console.WriteLine("After making  relationships");
            //MyTableGraph.print();

            //Console.WriteLine("Making the Database");
            //MyTableGraph.DataBaseName = "SchoolDB";
            //MyTableGraph.ConstructDataBase("A:\\Coding\\Database Project Tests");



            //try
            //{
            //    clsCommand command = new clsCommand("A:\\Coding\\Database Project Tests", "SchoolDB");



            //    //Save 10 records
            //for (int i = 5; i > 0; i--)
            //    {

            //        command.Add("Student", new Dictionary<string, object>()
            //                 {
            //                 {"id",i },
            //                 {"Name",$"Mohamed +{i}" },
            //                 {"Mark",45-i },
            //                 {"TeacherID",45-i }

            //         }
            //        );

            //    }


            //    for (int i = 0; i < 5; i++)
            //    {

            //        command.Add("Student", new Dictionary<string, object>()
            //                 {
            //                 {"id",i },
            //                 {"Name",$"Mohamed +{i}" },
            //                 {"Mark",45-i },
            //                 {"TeacherID",45-i }

            //         }
            //        );

            //    }



            //    command.ClearResultSet();

            //    command.Select("Student", new List<string>() { "id", "Name" }, kvp =>
            //    {
            //        if (kvp.TryGetValue("id", out var Objectid))
            //        {
            //            if (int.TryParse(Objectid.ToString(), out var id))
            //            {
            //                return id > 0;
            //            }
            //            else
            //            {
            //                throw new Exception("Invalide casting (in the func delegate Parametre to the selectallColumns method)");
            //            }


            //        }
            //        else
            //        {
            //            return false; //usualy this will throw an exception(invalide Column Name)
            //        }


            //    });


            //    foreach (var c in command.Records.SetColumns)
            //    {

            //        Console.Write(c.Key + "        ");

            //    }

            //    Console.WriteLine("");
            //    foreach (var record in command.Records.SetRecords)
            //    {
            //        foreach (var element in record)
            //            Console.Write(element.Value + "        ");

            //        Console.WriteLine("");


            //    }



            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}


            //try
            //{

            //    clsCommand command = new clsCommand("A:\\Coding\\Database Project Tests", "SchoolDB");

            //command.Add("Student", new Dictionary<string, object>()
            //{
            //     {"id",10},
            //     {"Name","Ali Amaliche" },
            //     {"Mark",451542 },
            //     {"TeacherID",450125 }
            //});

            //command.Add("Student", new Dictionary<string, object>()
            //{
            //     {"id",20},
            //     {"Name","mohamed Amaliche" },
            //     {"Mark",451542 },
            //     {"TeacherID",450125 }
            //});

            //command.Add("Student", new Dictionary<string, object>()
            //{
            //     {"id",15},
            //     {"Name","fatima zahra Amaliche" },
            //     {"Mark",451542 },
            //     {"TeacherID",450125 }
            //});

            //command.Add("Student", new Dictionary<string, object>()
            //{
            //     {"id",3},
            //     {"Name","salma Amaliche" },
            //     {"Mark",451542 },
            //     {"TeacherID",450125 }
            //});
            //command.Add("Student", new Dictionary<string, object>()
            //{
            //     {"id",11},
            //     {"Name","mama Amaliche" },
            //     {"Mark",451542 },
            //     {"TeacherID",450125 }
            //});


            //    command.PrintTableRecordSorted("Student");


            //    Console.WriteLine("\t\t\t\t\t Befor Update :\n\n");

            //     command.Select("Student", new List<string>() { "*" }, Kvp =>
            //    {
            //        if(Kvp.TryGetValue("id",out object Objectid))
            //        {
            //            if(int.TryParse(Objectid.ToString(), out int id))
            //            {


            //                return true;


            //            }
            //            else
            //            {
            //                throw new Exception("Casting went wrong");
            //            }





            //        }
            //        else
            //        {
            //            throw new Exception("Column does not exist");
            //        }


            //        return false;

            //    });




            //    //print the header
            //    foreach (var c in command.Records.SetColumns)
            //    {


            //       Console.Write(c.Key + "              ");


            //    }

            //    Console.WriteLine("");
            //    foreach (var c in command.Records.SetRecords)
            //    {

            //        foreach (var kvp in c)
            //            Console.Write(kvp.Value.ToString() + "        ");

            //        Console.WriteLine("");

            //    }


            //    Console.WriteLine("\n\n\t\t\t\t\t After Update : \n\n");


            //    command.Delete("Student", keyvp =>
            //    {
            //        if (keyvp.TryGetValue("id", out object Objectid))
            //        {
            //            if (int.TryParse(Objectid.ToString(), out int id))
            //            {


            //                return id==11;


            //            }
            //            else
            //            {
            //                throw new Exception("Casting went wrong");
            //            }





            //        }
            //        else
            //        {
            //            throw new Exception("Column does not exist");
            //        }





            //    });

            //    //Clear result set from old values
            //    command.ClearResultSet();
            //    //get data after the update
            //    command.Select("Student", new List<string>() { "*" }, Kvp =>
            //    {
            //        if (Kvp.TryGetValue("id", out object Objectid))
            //        {
            //            if (int.TryParse(Objectid.ToString(), out int id))
            //            {


            //                return true;


            //            }
            //            else
            //            {
            //                throw new Exception("Casting went wrong");
            //            }





            //        }
            //        else
            //        {
            //            throw new Exception("Column does not exist");
            //        }


            //        return false;

            //    });



            //    //print the header
            //    foreach (var c in command.Records.SetColumns)
            //    {


            //        Console.Write(c.Key + "              ");


            //    }

            //    Console.WriteLine("");
            //    foreach (var c in command.Records.SetRecords)
            //    {

            //        foreach (var kvp in c)
            //            Console.Write(kvp.Value.ToString() + "        ");

            //        Console.WriteLine("");

            //    }









            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine( e.ToString() );
            //}








            //Query class Test
            try
            {
                clsQuery query = new clsQuery("A:\\Coding\\Database Project Tests", "SchoolDB");
                var result=query.ExecuteQuery("  Select *  from    Student ");

                //print the header
                foreach (var c in result.SetColumns)
                {


                    Console.Write(c.Key + "                     ");


                }

                Console.WriteLine("");
                foreach (var c in result.SetRecords)
                {

                    foreach (var kvp in c)
                        Console.Write(kvp.Value.ToString() + "                     ");

                    Console.WriteLine("");

                }




               


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }






            System.Console.Read();


        }
    }
}
