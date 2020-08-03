using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    class Dot
    {
        double length = 10;
        string name;

        Image d;

        string core;
        string wireName;
        
        public Dot(string name, string core, Image d)
        {
            this.name = name;
            this.core = core;
            this.d = d;
        }

        public Image GetDot()
        {
            return d;
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
