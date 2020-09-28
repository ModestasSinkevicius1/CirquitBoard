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
        public CircuitChecker(ListContainer lc)
        {
            this.lc = lc;
        }

        public void CheckCircuit()
        {
            List<string> connectedElements = new List<string>();

            string name;

            foreach(SpecificElement se in lc.ec.GetAllElements())
            {
                foreach(Polyline pl in se.GetPolylineList())
                {
                    foreach(Wire w in lc.wList)
                    {
                        if(pl.Name == w.GetName())
                        {
                            if(w.elementA != se.GetName())
                            {
                                connectedElements.Add(w.elementA);
                            }
                            if(w.elementB != se.GetName())
                            {
                                connectedElements.Add(w.elementB);
                            }
                        }
                    }
                }
                foreach (string el in connectedElements)
                {
                    MessageBox.Show(se.GetName() + " is connected with " + el);
                }
                connectedElements.Clear();
                break;
            }
        }
    }
}
