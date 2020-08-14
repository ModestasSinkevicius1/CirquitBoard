using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CircuitBoardDiagram
{
    public class Test
    {
        public string name;
        private Image img;
        public Test()
        {

        }
        
        public Test(string name, Image img)
        {
            this.name = name;
            this.img = img;
        }
        
        public Image GetImage()
        {
            return img;
        }
    }
}
