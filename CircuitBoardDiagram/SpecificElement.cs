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
        public double current { get; set; } = 2;
        public double voltage { get; set; } = 220;
        public double resistance { get; set; } = 0.5;
        public double power { get; set; } = 200;      
        
        public string name;
        public bool isConnected=false;
        public bool visited { get; set; } = false;
        public int connectionCount = 0;
        private List<Polyline> plList = new List<Polyline>();
        private List<SpecificElement> seList = new List<SpecificElement>();
        public List<Dot> dList = new List<Dot>();
        public double offSetX;
        public double offSetY;

        private Image img;       
        
        public bool isCustom = false; 

        public int requiredCount = 2;
        public int maxCount = 2;

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
            if (!isCustom)
            {
                if (state && connectionRequiredCount <= connectionCount)
                    isConnected = true;
                else
                    isConnected = false;
            }
            else
            {
                if (requiredCount <= connectionCount)
                    isConnected = true;
                else
                    isConnected = false;
            }
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

        public void AddElement(SpecificElement se)
        {
            seList.Add(se);
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

        public List<SpecificElement> GetElements()
        {
            return seList;
        }

        public void AddConnection()
        {
            connectionCount++;
        }

        public void RemoveConnection()
        {
            connectionCount--;
        }

        public void RemoveChildElement(int target)
        {
            seList.RemoveAt(target);
        }
    }
}
