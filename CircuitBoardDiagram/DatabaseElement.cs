using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBoardDiagram
{
    class DatabaseElement
    {
        private int ID;
        private string type;
        private int connectionCount;
        private string classElement;

        public DatabaseElement(int ID, string type, int connectionCount, string classElement)
        {
            this.ID = ID;
            this.type = type;
            this.connectionCount = connectionCount;
            this.classElement = classElement;
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

        public string GetClass()
        {
            return classElement;
        }
    }
}
