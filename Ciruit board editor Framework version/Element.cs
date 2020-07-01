using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciruit_board_editor_Framework_version
{
    class Element
    {
        private string type;
        private int connectionCount;

        public Element (string type, int connectionCount)
        {
            this.type = type;
            this.connectionCount = connectionCount;
        }

        public string GetType()
        {
            return type;
        }

        public int GetConnectionCount()
        {
            return connectionCount;
        }
    }
}
