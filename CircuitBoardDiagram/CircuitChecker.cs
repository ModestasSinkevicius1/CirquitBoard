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
            if (RemoveNumbers(se.GetName()) != "AC")
            {
                se.visited = true;
            }
            SetDotVisited(se);

            bool sameDot = false;

            foreach(SpecificElement se2 in se.GetElements())
            {
                if (RemoveNumbers(se2.GetName()) != "AC" && !se2.visited)
                {
                    MessageBox.Show(se2.GetName());
                    CheckCircuit(se2);
                }
                else if (!se2.visited)
                {
                    MessageBox.Show("Found!");
                    sameDot = isDotVisited(se, se2);                                      
                    if (!sameDot)
                    {
                        circuitFull = true;
                        break;
                    }
                }
            }            
        }

        public void RemoveDotVisited(SpecificElement se)
        {
            foreach (Polyline pl in se.GetPolylineList())
            {
                foreach (Wire w in lc.wList)
                {
                    if (pl.Name == w.GetName())
                    {
                        if (w.elementA == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotA == d.GetName())
                                {
                                    d.visited = false;
                                }
                            }
                        }
                        if (w.elementB == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotB == d.GetName())
                                {
                                    d.visited = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool isDotVisited(SpecificElement se, SpecificElement se2)
        {
            foreach (Polyline pl in se2.GetPolylineList())
            {
                foreach (Wire w in lc.wList)
                {
                    if (pl.Name == w.GetName())
                    {
                        if (w.elementA == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotA == d.GetName())
                                {
                                    if (d.visited)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        if (w.elementB == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotB == d.GetName())
                                {
                                    if (d.visited)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void SetDotVisited(SpecificElement se)
        {
            foreach (Polyline pl in se.GetPolylineList())
            {
                foreach (Wire w in lc.wList)
                {
                    if (pl.Name == w.GetName())
                    {
                        if (w.elementA == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotA == d.GetName())
                                {
                                    d.visited = true;
                                }
                            }
                        }
                        if (w.elementB == se.GetName())
                        {
                            foreach (Dot d in lc.dList)
                            {
                                if (w.dotB == d.GetName())
                                {
                                    d.visited = true;
                                }
                            }
                        }
                    }
                }
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
