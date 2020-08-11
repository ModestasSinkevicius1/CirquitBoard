using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    class Wire
    {
        private string name;
        public string elementA { get; set; }
        public string elementB { get; set; }

        public string dotA { get; set; }
        public string dotB { get; set; }

        private Polyline pl = new Polyline();

        public Wire(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void AddPolyline(Polyline l)
        {
            pl = l;
        }

        public Polyline GetPolyline()
        {
            return pl;
        }
    }
}
