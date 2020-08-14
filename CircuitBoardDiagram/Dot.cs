using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    public class Dot
    {
        public double length = 10;
        public string name;
        public bool direction = false;
        public int oposite = 1;

        private Image d;

        public string core;
        public string wireName;
        
        public Dot()
        {

        }

        public Dot(string name, string core, Image d, bool direction, int oposite)
        {
            this.name = name;
            this.core = core;
            this.d = d;
            this.direction = direction;
            this.oposite = oposite;
        }

        public void SetDot(Image d)
        {
            this.d = d;
        }

        public Image GetDot()
        {
            return d;
        }

        public string GetWireName()
        {
            return wireName;
        }

        public bool GetDirection()
        {
            return direction;
        }

        public int GetOposite()
        {
            return oposite;
        }

        public void SetWireName(string wireName)
        {
            this.wireName = wireName;
        }

        public string GetCore()
        {
            return core;
        }

        public string GetName()
        {
            return name;
        }
        public void SetLength(double length)
        {
            this.length = length;
        }

        public double GetLength()
        {
            return length;
        }
    }
}
