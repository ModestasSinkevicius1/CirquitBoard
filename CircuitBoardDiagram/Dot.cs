using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class Dot
    {
        double length = 10;
        string name;

        string core;
        string wireName;
        
        public Dot(string name, string core)
        {
            this.name = name;
            this.core = core;
        }

        public string GetWireName()
        {
            return wireName;
        }

        public void SetWireName(string wireName)
        {
            this.wireName = wireName;
        }

        public string GetCore()
        {
            return core;
        }

        public string GetName()
        {
            return name;
        }
        public void SetLength(double length)
        {
            this.length = length;
        }

        public double GetLength()
        {
            return length;
        }
    }
}
