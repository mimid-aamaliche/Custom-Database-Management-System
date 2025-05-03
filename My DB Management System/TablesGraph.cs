using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace My_DB_Management_System
{
    
    public struct stRelationship
    {
        public string Name;
        public string ColumnSource;
        public string ColumnDistination;

        public override bool Equals(object obj)
        {
            return obj is stRelationship relationship &&
                   Name == relationship.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return Name+clsGeneral.Delemtre+ColumnSource+ clsGeneral.Delemtre + ColumnDistination +clsGeneral.Delemtre;
        }


    }
    public class TablesGraph
    {
        public string DataBaseName {  get; set; }
        public enum enGraphDirectionType { enDirectedGraph = 0, enInDirectedGraph = 1 }
        private enGraphDirectionType _graphDirectionType = enGraphDirectionType.enInDirectedGraph;


        //
        //Adjacency table represents the relationships (Edges) between Tables
        //where the relationships between Table A and B is the list adjacencyMatrix[A index,B index]
        //will represent the relationships as list of stRelationship struct
        //we used list of struct because 2 Tables can have more than one relationship
        //Example:
        //        Person's Country foreign key to Country Table
        //        Person's Birth Country foreign key to the same table
        //
        private List<stRelationship?>[,] _adjacencyMatrix { get; set; }


        // key value pairs linking each table with it index in the _adjacencyMatrix
        public Dictionary<Table, int> _vertexDictionary {  get; set; }

        private int _NumberOfTables { get; set; }




        //Constructors:
        //For Creating A new DataBase
        public TablesGraph(List<Table> vertices)
        {
            //set in the direction type(edges are one way or tow ways)
            //it s gonna be one way almost always in our project
            _graphDirectionType = enGraphDirectionType.enDirectedGraph;


            //get in the Number of Tables From the list of vertices
            _NumberOfTables=vertices.Count;


            //creating a matrix with length and width equal to length of list of vertices
            //note that elements in list are nullable stRelationship
            _adjacencyMatrix = new List<stRelationship?>[vertices.Count, vertices.Count];

            //inserting the verticies in dictionary and assigning to each one an index
            _vertexDictionary = new Dictionary<Table, int>();
            _FillVertexDictionary(vertices);




        }


        //For Loading an existing DataBase Structure Into Memory
        public TablesGraph(string Path,string dataBaseName)
        {
            //set in the direction type(edges are one way or tow ways)
            //it s gonna be one way almost always in our project
            _graphDirectionType = _graphDirectionType = enGraphDirectionType.enDirectedGraph;
            DataBaseName =dataBaseName;


            //initializing the vertexDictionary(Table/Index) pairs
            _vertexDictionary = new Dictionary<Table, int>();
            //Initializing the adjacency matric with count Table 
            //plan:
            //     in the structure file of database
            //             FirstLine: Info about DataBase (creation time,password,username,NumberOfTables...)
            //             Next:      Relationships info Lines Starting with "Relationship"
            //                        Table Information Lines starting with "Vertex:"
            //we are ether gonna load the first line here in the constructure or outside
            //most likely will load the first line Here in the constructure


            //Loading the firstLine Inthe structure file
            //that contains the info about database like creat time,number of tables ...
            // we also have to start saving that Line in the file in the first place

            string FullDirectoryPath = (new StringBuilder(Path).Append("\\" + DataBaseName)).ToString();



            using (StreamReader sr = new StreamReader((new StringBuilder(FullDirectoryPath).Append("\\" + DataBaseName + "_Structure" + "\\" + "Structure.txt")).ToString()))
            {
                if(int.TryParse(clsGeneral.SplitLine(sr.ReadLine(),clsGeneral.Delemtre)[1], out int Value))
                {
                    _NumberOfTables = Value;
                }
                else
                {
                    throw new Exception("Invalide Number Of Tables , Problem At Constructure Of the GraphTable");
                }


                
            }


            
            _adjacencyMatrix = new List<stRelationship?>[_NumberOfTables, _NumberOfTables];

            _LoadDataBaseStructure(FullDirectoryPath);



        }



        //Gets a list of Table Objects and copy them into VertexDictionnary
        private void _FillVertexDictionary(List<Table> vertices)
        {
            int counter = 0;
            foreach (var vertex in vertices)
            {

                if (!_vertexDictionary.ContainsKey(vertex))
                    _vertexDictionary.Add(vertex, counter++);

            }


        }






        //load db_structure from files 
        //     load : relationships
        //            Tables with there indeces
        //            we are not gonna be loading the records , we will let that to the command class





        




        //what we gonna use to manupilate relationships between tables
        public bool RemoveRelationship(Table VertexSource, Table VertexDestination, stRelationship? relationToBeRemoved)
        {
            //this could be used to change the velue of the weight or to Add/Remove Edges 

            int sourceIndex, destinationIndex;

            if (!_vertexDictionary.TryGetValue(VertexSource, out sourceIndex))
            {
                return false;
            }
            if (!_vertexDictionary.TryGetValue(VertexDestination, out destinationIndex))
            {
                return false;
            }


            return _adjacencyMatrix[sourceIndex, destinationIndex].Remove(relationToBeRemoved);

            ////if the graph is indirected then the edge should be both ways
            //if (_graphDirectionType == enGraphDirectionType.enInDirectedGraph)
            //    _adjacencyMatrix[destinationIndex, sourceIndex].Add(relation);



        }
        public bool AddRelationShip(Table VertexSource, Table VertexDestination, stRelationship? relation)
        {

          
            //this could be used to change the velue of the weight or to Add/Remove Edges 

            int sourceIndex, destinationIndex;

            if (!_vertexDictionary.TryGetValue(VertexSource, out sourceIndex))
            {
                return false;
            }
            if (!_vertexDictionary.TryGetValue(VertexDestination, out destinationIndex))
            {
                return false;
            }

            if (_adjacencyMatrix[sourceIndex, destinationIndex]!=null)
                _adjacencyMatrix[sourceIndex, destinationIndex].Add(relation);
            else
            {
                //we should inisialize the list befor inserting the element 
                _adjacencyMatrix[sourceIndex, destinationIndex] = new List<stRelationship?>();
                _adjacencyMatrix[sourceIndex, destinationIndex].Add(relation);

            }


            //if the graph is indirected then the edge should be both ways
            if (_graphDirectionType == enGraphDirectionType.enInDirectedGraph)
                _adjacencyMatrix[destinationIndex, sourceIndex].Add(relation);



            return true;




        }

        


        //Prints each Table and the relationships It Has with Others Tables
        public void print()
        {
            string Adjacent = "";


            foreach (var vertex in _vertexDictionary)
            {

                for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++)
                {

                    if (_adjacencyMatrix[vertex.Value, i] != null )
                    {


                        if (_adjacencyMatrix[vertex.Value, i].Count > 0)
                        {
                            var kvpairs = _vertexDictionary.Where(kpv => kpv.Value == i);
                            //altough it should be just one record because we quered by the index 
                            //will use foreach just to be safe
                            foreach (var kvpair in kvpairs)
                            {

                                Adjacent += "[ To : " + kvpair.Key.Name + " Relationships :" + string.Join(",", _adjacencyMatrix[vertex.Value, i]) + " ]";

                            }
                        }
                        


                    }
                    if (_adjacencyMatrix[i, vertex.Value] != null)
                    {
                        if (_adjacencyMatrix[i,vertex.Value].Count > 0)
                        {
                            var kvpairs = _vertexDictionary.Where(kpv => kpv.Value == i);
                            //altough it should be just one record because we quered by the index 
                            //will use foreach just to be safe
                            foreach (var kvpair in kvpairs)
                            {

                                Adjacent += "[ From : " + kvpair.Key.Name + " Relationships :"+ string.Join(",", _adjacencyMatrix[i, vertex.Value]) + " ]" ;


                            }

                        }
                    }


                }



                System.Console.WriteLine(vertex.Key.Name + " => " + Adjacent);
                Adjacent = string.Empty;

            }




        }



        public void ConstructDataBase(string Path)
        {
            _ConstructDataBase(Path, clsGeneral.Delemtre);
        }


        
        //Constructs the DataBase At The path given
        private void _ConstructDataBase(string Path,string delemtre)
        {
            string FullDirectoryPath=(new StringBuilder(Path).Append("\\"+DataBaseName)).ToString();

            try
            {

                if (Directory.Exists(FullDirectoryPath))
                {
                    
                    Console.WriteLine("Directory with the same name was already created");
                    return;
                }

                //maKing the main directory (ROOT)
                DirectoryInfo directoryInfo = Directory.CreateDirectory(FullDirectoryPath);



                //Making the subDirectory , which conatains info about the structure of the database
                directoryInfo.CreateSubdirectory((new StringBuilder(DataBaseName).Append("_Structure")).ToString());



                //making dir and files for each table
                foreach(var item in _vertexDictionary)
                {
                    //for each Table will call the creat Dir and file of the Table
                    item.Key.CreatDir_Files(FullDirectoryPath,delemtre);

                }



                //Making the Database Structure File
                //to save the relationships between Tables
                using (StreamWriter sw = File.CreateText((new StringBuilder(FullDirectoryPath).Append("\\" + DataBaseName + "_Structure" + "\\" + "Structure.txt")).ToString()))
                {

                    //The first Line in the file should Contain Info About the Database 
                    //Like Number of Table and General info Like This

                    sw.WriteLine("Structure" + delemtre + this._NumberOfTables);


                    //saving Relationships between Table in the file
                    for (int i = 0; i < _adjacencyMatrix.GetLength(0); i++) {

                        for (int j = 0; j < _adjacencyMatrix.GetLength(1); j++) {

                            if (_adjacencyMatrix[i, j] == null)
                                continue;
                            foreach (var relation in _adjacencyMatrix[i, j]) { 
                            
                                sw.WriteLine("Relationship"+delemtre+relation.ToString()+$"{i}{delemtre}{j}");
                            
                            }
                           
                            
                        }
                    
                    
                    }

                    //Saving each Table Name with it s index in the file
                    foreach (var item in _vertexDictionary)
                    {

                        sw.WriteLine("Vertex:" + delemtre 
                                    + item.Key.Name + delemtre  
                                    + item.Value);


                    }





                }

                //i still need to save the Table Names and Indexes 

            }
            catch (Exception ex) {
                Console.WriteLine("Somthing went wrong in Graph class   :"+ex.Message);

            }


        }




        //Method Related to Load the DataBase Structure from Directory Of DataBase
        private void InitializeRelationshipMatrix(List<string> RelationShipInfo)
        {

            stRelationship stRelationship = new stRelationship()
            {
                Name = RelationShipInfo[1],
                ColumnSource = RelationShipInfo[2],
                ColumnDistination = RelationShipInfo[3],
            };

            //we are working with an adjacencyMatrix so getting those Indexes right is crucial
            int LigneIndexOfRelationship = int.TryParse(RelationShipInfo[4], out int I) ? I : -1;
            int ColumnIndexOfRelationship = int.TryParse(RelationShipInfo[5], out int J) ? J : -1;

            if (LigneIndexOfRelationship == -1)
                throw new IndexOutOfRangeException("Problem In the {InitializeRelationshipMatrix} Method (Casting gone wrong)");
            if (ColumnIndexOfRelationship == -1)
                throw new IndexOutOfRangeException("Problem In the {InitializeRelationshipMatrix} Method (Casting gone wrong)");





            if(this._adjacencyMatrix[LigneIndexOfRelationship, ColumnIndexOfRelationship] != null)
            {
                this._adjacencyMatrix[LigneIndexOfRelationship, ColumnIndexOfRelationship].Add(stRelationship);

            }
            else
            {
                this._adjacencyMatrix[LigneIndexOfRelationship, ColumnIndexOfRelationship] = new List<stRelationship?>();
                this._adjacencyMatrix[LigneIndexOfRelationship, ColumnIndexOfRelationship].Add(stRelationship);

            }


        }
        private void _InitializeTableDictionary(List<string> TableInfo,string path)
        {

            if(int.TryParse(TableInfo[2], out int TableIndex))
            {

               this._vertexDictionary.Add(
                         new Table(TableInfo[1],path),
                         TableIndex
                );


            }
            else
            {
                throw new InvalidCastException("Could not cast .method(InitializeTableDictionary) in TablesGraph class");
            }
   


        }
        private void _InitializeDB_Structure(string Line ,string Path)
        {
            //if the Line start with relationship then put it in the adjacency matrix
            //else if it start with vertex or Table load that table info and put it on the dictionnay

            List<string> list = clsGeneral.SplitLine(Line,clsGeneral.Delemtre);

            if (list[0].Equals("Relationship"))
            {
                InitializeRelationshipMatrix(list);
            }
            else if (list[0].Equals("Vertex:"))
            {
                _InitializeTableDictionary(list, Path);

            }






        }
        private void _LoadDataBaseStructure(string FullDirectoryPath)
        {
            //First we load the DataBase Structure
            //         *Relationships Into Adjacency Matrix
            //         *Tables with There Indices
            //               * here we load the table name from DBStructure and index
            //               * and with that Name we load the Table structure into a Table Object
            //                        (we will need a LoadTableStructure(string TableName) in Table class
            //

            //path is the full path to our database
            //example A://somthing//somthing//MyProject
            //and inside MyProject should be our database directory

            



            try
            {
                if (!Directory.Exists(FullDirectoryPath))
                {
                    
                    throw new DirectoryNotFoundException("DataBase Directory Not Found");
                   
                }

                string DBStructureFilePath = (new StringBuilder(FullDirectoryPath).Append("\\" + DataBaseName + "_Structure" + "\\" + "Structure.txt")).ToString();


                //iterate over the db structure file and initialize all TableGraph object components
                using (StreamReader sr = new StreamReader(DBStructureFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string Line = sr.ReadLine();

                        _InitializeDB_Structure(Line, FullDirectoryPath);
                    }

                }


                



            }
            catch (Exception ex) {

                Console.WriteLine(ex.Message);


            }






        }

    }
}
