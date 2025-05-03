the idea of this project is :
               1) you craete the tables for your database   
               2) you create the TableGraph(this class controls the structure of the database such as relationships )   
               3) when you are done you call the contructDatabase method  
                           What does that do exacly?  
                           1)it creates the directory for your database at the given path with the given Name  
                           2)for each Table It Creates a Directory inside the db Directory  
                                       .The table Directory contains: Records.txt , PrimaryKeyIndex.txt , structure.txt  
                                       Note: we have a problem here because normally In Databases if the Table does not Have a primary Key the db Creates a default Index but here  
                                       it must have at least one PK  
                           3)+ a DBStructure file that stores The Relationships between Table  

                            ******************HOW DID I IMPLIMENTED THE RELATIONSHIPS ?******************************************************************
                            i implimented it as a GRAPH (ADJACENCY_MATRIX to be exact)
                            IS THIS IDEAL? ____NO because we want to be able to add delete tables after we created the DB, but here if we wanted to do so will need 
                                              to Allucate new memory for the matric each time we make a modification on the DB Structure

WHAT ABOUT THE QUERY DLL:  
            Contains Tow Main Classes clsCommands,clsQuery  
                    .clsCommand contains Basic operations Like Add Update Delete Find Select  
                        .in the select function you can specifie the columns you want and pass in a predicate(A function That returns a bool Value Using c# Delegation) to filter the data  
                        ...  
                    .clsQuery class uses the command class(composition) (I am still working On this)  
                            this class takes the querystring and proccess it and Uses the corresponding Method in the clsCommand.  
                            querystring is basic slq query.  

                            
