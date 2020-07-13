using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class SpecificElement
    {
        private string name;
        private bool isConnected=false;
        private int connectionCount = 0;

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
            if (state && connectionRequiredCount<connectionCount)
                isConnected = true;
            else
                isConnected = false;
        }

        public bool GetStatus()
        {
            return isConnected;
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
