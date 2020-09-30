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
        private ListContainer lc;
        public bool circuitFull = false;
        public CircuitChecker(ListContainer lc)
        {
            this.lc = lc;
        }

        public void CheckCircuit(SpecificElement se)
        {           
            se.visited = true;
            int ACfound = 0;

            foreach(SpecificElement se2 in se.GetElements())
            {                
                if (RemoveNumbers(se2.GetName()) != "AC" && !se2.visited)
                {                   
                    CheckCircuit(se2);
                }
                else if (!se2.visited || ACfound>0)
                {                                                                                            
                    circuitFull = true;
                    break;                   
                }                
                ACfound++;
            }            
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
