using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    public class ElementControl
    {
        public List<SpecificElement> seList = new List<SpecificElement>();
        
        public ElementControl()
        {

        }
        
        public void AddElementToList(string name, Image img)
        {
            SpecificElement se = new SpecificElement(name, img);
            seList.Add(se);
        }

        public void AddElementToParentElement(string parentName, string childName)
        {
            foreach(SpecificElement se in seList)
            {
                if(se.GetName() == parentName)
                {
                    foreach(SpecificElement se2 in seList)
                    {
                        if(se2.GetName() == childName)
                        {
                            se.AddElement(se2);
                        }
                    }
                }
            }
        }

        public List<SpecificElement> GetAllElements()
        {
            return seList;
        }       

        public void RemoveChildElementFromElement(string parent, string child)
        {
            int target = -1;
            int i = 0;

            foreach (SpecificElement se in seList)
            {
                if(se.GetName() == parent)
                {
                    foreach(SpecificElement se2 in se.GetElements())
                    {
                        if(se2.GetName() == child)
                        {
                            target = i;
                        }
                        i++;
                    }
                    if(target > -1)
                        se.RemoveChildElement(target);
                }
            }
        }

        public void RemoveElementFromList(string name)
        {
            int target = 0;
            int i = 0;
            foreach(SpecificElement se in seList)
            {
                if(se.GetName()==name)
                {
                    target = i;
                    break;
                }
                i++;
            }
            seList.RemoveAt(target);
        }

        public void AddConnectionCountToSpecificElement(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    se.AddConnection();
                    break;
                }                
            }
        }

        public void RemoveConnectionCountFromSpecificElement(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    se.RemoveConnection();
                    break;
                }
            }
        }

        public void AddLineForElement(string name, Polyline l)
        {
            foreach(SpecificElement se in seList)
            {
                if(se.GetName() == name)
                {
                    se.AddPolyline(l);
                    break;
                }
            }
        }

        public List<Polyline> GetLineListFromElement(string name)
        {
            foreach(SpecificElement se in seList)
            {
                if(se.GetName() == name)
                {
                    return se.GetPolylineList();
                }
            }
            return null;
        }

        public List<Dot> GetDots(string name)
        {
            foreach (SpecificElement se in seList)
            {               
                if (se.GetName() == name)
                {                   
                    return se.GetDots();
                }
            }
            return null;
        }

        public void AddDot(string name, Dot d)
        {
            foreach (SpecificElement se in seList)
            {              
                if (se.GetName() == name)
                {                   
                    se.AddDot(d);
                    break;
                }
            }
        }

        public bool GetConnectionAvailability(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    return se.GetStatus();                  
                }
            }
            return false;
        }

        public int GetConnectionCount(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    return se.GetConnectionCount();
                }
            }
            return 0;
        }

        public double GetElementPositionX(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    return se.GetPositionX();
                }
            }
            return 0;
        }

        public double GetElementPositionY(string name)
        {
            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    return se.GetPositionY();
                }
            }
            return 0;
        }

        public void EnableConnectionAvailability(string name)
        {
            DatabaseControl dc = new DatabaseControl();

            foreach (SpecificElement se in seList)
            {
                if (se.GetName() == name)
                {
                    foreach (DatabaseElement de in dc.GetElements())
                    {
                        if (name.Remove(name.Length - 1) == de.GetElementType())
                        {
                            se.SetConnection(true, de.GetConnectionCount());
                            break;
                        }
                    }
                    break;
                }
            }
        } 
        
        public void UpdatePostitionValues(string name)
        {
            foreach(SpecificElement se in seList)
            {
                if(name==se.GetName())
                {
                    se.SetPostitions();
                }
            }
        }
    }
}
