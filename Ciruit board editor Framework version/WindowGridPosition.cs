using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciruit_board_editor_Framework_version
{
    class WindowGridPosition
    {
        private int x;
        private int y;

        public WindowGridPosition(int x, int y){

            this.x = x;
            this.y = y;

        }
        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

    }
}
