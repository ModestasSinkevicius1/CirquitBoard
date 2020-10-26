using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    class CircuitChecker
    {
        private HighlighterGUIControl hgc;
        private ListContainer lc;
        public bool circuitFull = false;       
        public CircuitChecker(ListContainer lc, HighlighterGUIControl hgc)
        {
            this.lc = lc;
            this.hgc = hgc;
        }

        public void UpdateListContainer(ListContainer lc)
        {
            this.lc = lc;
        }

        public void CheckCircuit(SpecificElement se, int level)
        {           
            se.visited = true;
            int ACfound = 0;           
            foreach (SpecificElement se2 in se.GetElements())
            {                
                if (RemoveNumbers(se2.GetName()) != "AC" && !se2.visited)
                {                   
                    CheckCircuit(se2, level+1);
                }
                else if (!se2.visited || ACfound>0)
                {
                    if (level > 0)
                    {
                        circuitFull = true;
                        hgc.ShowCheckCircuitBox(se2.GetElement());
                        break;
                    }
                }                
                ACfound++;
            }
            if (circuitFull)
                hgc.ShowCheckCircuitBox(se.GetElement());
                
        }
                   
        private string RemoveNumbers(string name)
        {
            foreach (char w in name)
            {
                if (Char.IsNumber(w))
                {
                    name = name.Remove(name.Length - 1);
                }
            }

            return name;
        }
    }
}
