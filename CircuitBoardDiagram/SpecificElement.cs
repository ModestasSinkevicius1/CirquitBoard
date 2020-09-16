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
    public class SpecificElement
    {
        public string name;
        public bool isConnected=false;
        public int connectionCount = 0;
        private List<Polyline> plList = new List<Polyline>();
        public List<Dot> dList = new List<Dot>();
        public double offSetX;
        public double offSetY;

        private Image img;        

        public SpecificElement()
        {

        }


        public SpecificElement(string name, Image img)
        {
            this.name = name;
            this.img = img;            
        }

        public void SetImage(Image img)
        {
            this.img = img; 
        }

        public Image GetElement()
        {
            return img;
        }
        
        public string GetName()
        {
            return name;
        }

        public int GetConnectionCount()
        {
            return connectionCount;
        }

        public double GetPositionX()
        {
            return offSetX;
        }

        public double GetPositionY()
        {
            return offSetY;
        }

        public void SetConnection(bool state, int connectionRequiredCount)
        {
            if (state && connectionRequiredCount<=connectionCount)
                isConnected = true;
            else
                isConnected = false;
        }        

        public void SetPostitions()
        {
            offSetX = img.RenderTransform.Value.OffsetX;
            offSetY = img.RenderTransform.Value.OffsetY;
        }

        public bool GetStatus()
        {
            return isConnected;
        }

        public void AddPolyline(Polyline l)
        {
            plList.Add(l);
        }

        public void AddDot(Dot d)
        {
            dList.Add(d);
        }

        public void ClearDotList()
        {
            dList.Clear();
        }

        public List<Dot> GetDots()
        {
            return dList;
        }

        public List<Polyline> GetPolylineList()
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
