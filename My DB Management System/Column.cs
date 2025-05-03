using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_DB_Management_System
{
    public class Column
    {

        public Type type { get; set; }
        public string Name { get; set; }

        public int ColumnIndex {  get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsReadOnly { get; set; }
        public bool AutoIncriment { get; set; }
        public int Seed { get; set; }
        public int Start { get; set; }

        public Column()
        {



        }

        public Column(Type type, string name, bool isPrimaryKey, bool isForeignKey, bool isReadOnly, bool autoIncriment, int seed, int start)
        {
            this.type = type;
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            IsForeignKey = isForeignKey;
            IsReadOnly = isReadOnly;
            AutoIncriment = autoIncriment;
            Seed = seed;
            Start = start;
        }

        public override string ToString()
        {
            string delemtre = clsGeneral.Delemtre;

            return type.Name + delemtre+
                   Name+delemtre
                   + IsPrimaryKey + delemtre
                   + IsForeignKey + delemtre
                   + IsReadOnly + delemtre
                   + AutoIncriment + delemtre
                   + Seed+delemtre
                   + Start+delemtre
                   + ColumnIndex;

                 
                
         }
    }
}
