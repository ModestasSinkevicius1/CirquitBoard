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

        private List<Line> lList = new List<Line>();

        public Wire(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void AddList(Line l)
        {
            lList.Add(l);
        }

        public List<Line> GetList()
        {
            return lList;
        }
    }
}
