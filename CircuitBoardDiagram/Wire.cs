using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class Wire
    {
        private string name;
        public string elementA { get; set; }
        public string elementB { get; set; }

        public Wire(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }
    }
}
