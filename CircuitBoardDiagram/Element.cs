using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class Element
    {
        private int ID;
        private string type;
        private int connectionCount;

        public Element(int ID, string type, int connectionCount)
        {
            this.ID = ID;
            this.type = type;
            this.connectionCount = connectionCount;
        }

        public int GetID()
        {
            return ID;
        }

        public string GetElementType()
        {
            return type;
        }

        public int GetConnectionCount()
        {
            return connectionCount;
        }
    }
}
