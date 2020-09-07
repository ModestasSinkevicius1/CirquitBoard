using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class ListContainer
    {
        public ElementControl ec { get; set; } = new ElementControl();
        public List<Wire> wList { get; set; } = new List<Wire>();
        public List<Dot> dList { get; set; } = new List<Dot>();
    }
}
