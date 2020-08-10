using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    class SpecificElement
    {
        private string name;
        private bool isConnected=false;
        private int connectionCount = 0;
        private List<Polyline> plList = new List<Polyline>();
        private List<Dot> dList = new List<Dot>();

        public SpecificElement(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public int GetConnectionCount()
        {
            return connectionCount;
        }

        public void SetConnection(bool state, int connectionRequiredCount)
        {
            if (state && connectionRequiredCount<=connectionCount)
                isConnected = true;
            else
                isConnected = false;
        }        

        public bool GetStatus()
        {
            return isConnected;
        }

        public void AddList(Polyline l)
        {
            plList.Add(l);
        }

        public void AddDot(Dot d)
        {
            dList.Add(d);
        }

        public List<Dot> GetDots()
        {
            return dList;
        }

        public List<Polyline> GetList()
        {
            return plList;
        }

        public void AddConnection()
        {
            connectionCount++;
        }

        public void RemoveConnection()
        {
            connectionCount--;
        }
    }
}
