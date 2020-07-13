using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class ElementControl
    {
        private List<SpecificElement> seList = new List<SpecificElement>();
        public void AddElementToList(string name)
        {
            SpecificElement se = new SpecificElement(name);
            seList.Add(se);
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
    }
}
