using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_DataBase_Query_Stuff
{
    public class clsLogicalOperations<T>
    {


        //> / >= operations
        public static bool Greater(T left, T right) {
            //this method would work on int double short decimal string
            //but not on reference types and on datetype struct


            if(typeof(T)==typeof(string))
                return string.CompareOrdinal(left.ToString(), right.ToString())>0;

            if (typeof(T) == typeof(decimal))
            {
                if(decimal.TryParse(left.ToString(), out decimal LeftParsed))
                {
                    if(decimal.TryParse(right.ToString(),out decimal RightParsed))
                    {
                        return LeftParsed > RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }


            }
            else
            {
                //if we are here we just parce to the double , because all numirical types can be parced to double

                if (double.TryParse(left.ToString(), out double LeftParsed))
                {
                    if (double.TryParse(right.ToString(), out double RightParsed))
                    {
                        return LeftParsed > RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }

            }




        }
        public static bool GreaterOrEqual(T left, T right)
        {
            //this method would work on int double short decimal string
            //but not on reference types and on datetype struct


            if (typeof(T) == typeof(string))
            {
                
                return string.CompareOrdinal(left.ToString(), right.ToString()) >= 0;

            }

            if (typeof(T) == typeof(decimal))
            {
                if (decimal.TryParse(left.ToString(), out decimal LeftParsed))
                {
                    if (decimal.TryParse(right.ToString(), out decimal RightParsed))
                    {
                        return LeftParsed >= RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }


            }
            else
            {
                //if we are here we just parce to the double , because all numirical types can be parced to double

                if (double.TryParse(left.ToString(), out double LeftParsed))
                {
                    if (double.TryParse(right.ToString(), out double RightParsed))
                    {
                        return LeftParsed >= RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }

            }




        }



        //== / != operations
        public static bool NotEqual(T left, T right)
        {
            //this method would work on int double short decimal string
            //but not on reference types and on datetype struct


            if (typeof(T) == typeof(string))
                return string.CompareOrdinal(left.ToString(), right.ToString()) != 0;

            if (typeof(T) == typeof(decimal))
            {
                if (decimal.TryParse(left.ToString(), out decimal LeftParsed))
                {
                    if (decimal.TryParse(right.ToString(), out decimal RightParsed))
                    {
                        return LeftParsed != RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }


            }
            else
            {
                //if we are here we just parce to the double , because all numirical types can be parced to double

                if (double.TryParse(left.ToString(), out double LeftParsed))
                {
                    if (double.TryParse(right.ToString(), out double RightParsed))
                    {
                        return LeftParsed != RightParsed;
                    }
                    else
                    {
                        throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                    }


                }
                else
                {
                    throw new InvalidCastException("casting went wrong on the greater method at the clsLogicalOperations class");
                }

            }




        }
        public static bool Equal(T left, T right)
        {

            return !NotEqual(left, right);


        }


        //< / <= operations
        public static bool Less(T left, T right)
        {
            //just return not GreaterOrEqual
            return !GreaterOrEqual(left, right);

        }
        public static bool LessOrEqual(T left, T right) {
        
            //just return not Greater
            return !Greater(left, right);


        }



        //And Operation
        public static bool And(bool left, bool right)
        {
            return left && right;
        }



        //Or Operation
        public static bool Or(bool left, bool right)
        {
            return left || right;
        }



        //Not Operation
        public static bool Not(bool condition)
        {
            return !condition;
        }




        //processing a string condition 

        public static bool ProcessConditionsPart(Dictionary<string, object> Columns,List<string> ConditionPart)
        {
            //First i need to split the conditionPart into indicidual parts 
            //usualy conditions will be separated by And / Or
            //First Seperat different Conditions
            //proccess each condition and put the value in bitarray
            //move on to the next condition
            int CuurentConditionIndex = 0;
            List<string> ConditionComponents;
            int NextAnd_OrKeyWordsIndex=0;
            int NextOrIndex = 0;
            int NextAndIndex = 0;



            //loop 
            NextOrIndex = ConditionPart.FindIndex(CuurentConditionIndex, x => x.Equals("or", StringComparison.CurrentCultureIgnoreCase));
            NextAndIndex = ConditionPart.FindIndex(CuurentConditionIndex,x => x.Equals("and", StringComparison.CurrentCultureIgnoreCase));

            NextAnd_OrKeyWordsIndex= NextAndIndex > NextOrIndex ? (NextOrIndex == -1? NextAndIndex : NextOrIndex) : (NextAndIndex == -1 ? NextOrIndex : NextAndIndex);

            if (NextAnd_OrKeyWordsIndex != -1)
            {
                 ConditionComponents = ConditionPart.GetRange(CuurentConditionIndex, NextAnd_OrKeyWordsIndex - CuurentConditionIndex - 1);
                //Point to the next condition index
                CuurentConditionIndex = NextAnd_OrKeyWordsIndex + 1;

                //process the condition 


                //append the result in a bitarray



            }
            else
            {
                 ConditionComponents = ConditionPart.GetRange(CuurentConditionIndex, ConditionPart.Count - CuurentConditionIndex);
               
                //process the condition


                //append the result in a bitarray


                //get out of the loop
            }
            




            //after the loop we proccess the bitarray data
            //get the first And/Or Operator and send it to the and method




            return false;
        }

        private static bool ProccessSingleCondition(Dictionary<string, object> Columns, List<string> ConditionComponents)
        {
            //Now we need to seperate the condition by operatio(> < >= <= )
            //How?
            //1 for each item we check if it conatains an operation
            //    if it does we seperat the string and send the compinents to the coresponding method
            // if we found that there is more than one operation at one condition we throw an exception

            //Problem :/ we also need to beware of the arithmitique ops 
            // but lets leave that for later

            //Note we will need to get the type of each Column and used to cast our string value







            return false;
        }





    }
}
