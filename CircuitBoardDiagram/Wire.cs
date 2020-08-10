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

        private List<Polyline> plList = new List<Polyline>();

        public Wire(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void AddList(Polyline l)
        {
            plList.Add(l);
        }

        public List<Polyline> GetList()
        {
            return plList;
        }
    }
}
