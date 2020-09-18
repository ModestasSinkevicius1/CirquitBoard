using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram.GUIControls
{
    class WireGUIControl
    {
        private bool turn = false;       
        private string previousElementName = "";
        private string previousDotName = "";

        private ListContainer lc;

        private Wire w;
        private Image previousDot;

        private Polyline previousLine;

        private MessageGUIControl mgc;

        private Canvas canvas;

        private int row = 0;

        public WireGUIControl(Canvas canvas, MessageGUIControl mgc, ListContainer lc)
        {
            this.canvas = canvas;
            this.mgc = mgc;
            this.lc = lc;
        }

        public void UpdateListContainer(ListContainer lc)
        {
            this.lc = lc;
        }

        private void Polyline_mouseEnter(object sender, MouseEventArgs e)
        {
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();

            if (Keyboard.IsKeyDown(Key.X))
            {
                bc.Color = Colors.Red;
            }
            else
                bc.Color = Colors.Green;

            ChangeLineStyle(pl, bc, 4);
        }        

        private void Polyline_mouseLeave(object sender, MouseEventArgs e)
        {
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            ChangeLineStyle(pl, bc, 1);    
        }

        private void Polyline_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Polyline pl = sender as Polyline;

            if (Keyboard.IsKeyDown(Key.X))
            {
                DeleteWires(pl);                         
            }
            else if (Keyboard.IsKeyDown(Key.C))
            {
                foreach (Wire w2 in lc.wList)
                {                   
                    if (w2.GetName() == pl.Name)
                    {
                        MessageBox.Show(w2.elementA + " connected with " + w2.elementB);
                        break;
                    }
                }
            }
        }

        private void Polyline_mouseMove(object sender, MouseEventArgs e)
        {
            Line draggableControl = sender as Line;

            List<Line> lList;

            double length = 10;

            int dotX = 0;
            int dotY = 0;
        }

        private void Polyline_mouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Line draggableControl = sender as Line;
            string name = "";
        }

        public Polyline CreatePolyline()
        {
            Polyline pl = new Polyline();
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            pl.StrokeThickness = 2;
            pl.Stroke = bc;
            
            pl.MouseLeftButtonDown += new MouseButtonEventHandler(Polyline_mouseLeftButtonDown);
            pl.MouseEnter += new MouseEventHandler(Polyline_mouseEnter);
            pl.MouseLeave += new MouseEventHandler(Polyline_mouseLeave);
            pl.MouseMove += new MouseEventHandler(Polyline_mouseMove);                           

            canvas.Children.Add(pl);

            return pl;
        }

        
        public void UpdateWireLocation(string dotNameA, string dotNameB, Polyline pl)
        {
            Point startLine1;
            Point startline2;
            Point line1;
            Point line2;
            Point endLine2;
            Point endLine1;

            PointCollection polylinePoints = new PointCollection();

            Image dotA = FindDot(dotNameA);
            Image dotB = FindDot(dotNameB);
           

            bool direction = DetermineDirection(dotNameA);
            int n = DetermineDirection2(dotNameA);           

            startLine1 = new Point(dotA.RenderTransform.Value.OffsetX+5, dotA.RenderTransform.Value.OffsetY+10);
            endLine1 = new Point(dotB.RenderTransform.Value.OffsetX+10, dotB.RenderTransform.Value.OffsetY+10);

            if (!direction)
            {
                startline2 = new Point(startLine1.X + 25 * n, startLine1.Y);
            }
            else
            {
                startline2 = new Point(startLine1.X, startLine1.Y + 25 * n);
            }

            direction = DetermineDirection(dotNameB);
            n = DetermineDirection2(dotNameB);

            if (!direction)
            {
                endLine2 = new Point(endLine1.X + 25 * n, endLine1.Y);
            }
            else
            {
                endLine2 = new Point(endLine1.X, endLine1.Y + 25 * n);
            }

            direction = DetermineDirection(dotNameA);

            if (!direction)
            {
                line1 = new Point(startline2.X, endLine2.Y);
                line2 = new Point(endLine2.X, endLine2.Y);
            }
            else
            {
                line1 = new Point(endLine2.X, startline2.Y);
                line2 = new Point(endLine2.X, endLine2.Y);
            }


            polylinePoints.Add(startLine1);
            polylinePoints.Add(startline2);
            polylinePoints.Add(line1);
            polylinePoints.Add(line2);
            polylinePoints.Add(endLine2);
            polylinePoints.Add(endLine1);

            pl.Points = polylinePoints;
        }

        public Image FindDot(string dotName)
        {
            Image d = null;
            foreach (Dot d2 in lc.dList)
            {
                if (d2.GetName() == dotName)
                {
                    d = d2.GetDot();                   
                }
            }
            return d;
        }

        public bool DetermineDirection(string dotName)
        {
            bool direction = false;
            foreach (Dot d in lc.dList)
            {
                if (d.GetName() == dotName)
                {
                    direction = d.GetDirection();
                    break;
                }
            }
            return direction;
        }

        public int DetermineDirection2(string dotName)
        {
            int n = 0;
            foreach (Dot d in lc.dList)
            {
                if (d.GetName() == dotName)
                {
                    n = d.GetOposite();

                }
            }
            return n;
        }

        public void FindWireConnectedDots(string name)
        {
            foreach(Wire w in lc.wList)
            {
                if(w.elementA==name || w.elementB==name)
                {                    
                    UpdateWireLocation(w.dotA, w.dotB, w.GetPolyline());                       
                }
             }
        }      
        public void DeleteWires(Polyline l)
        {            
            foreach (Wire w2 in lc.wList)
            {                
                if (w2.GetName() == l.Name)
                {
                    foreach (Dot d in lc.dList)
                    {
                        if (w2.dotA == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                            d.occupied = false;
                        }
                        if (w2.dotB == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                            d.occupied = false;
                        }
                    }

                    canvas.Children.Remove(w2.GetPolyline());                    

                    lc.ec.RemoveConnectionCountFromSpecificElement(w2.elementA);
                    lc.ec.RemoveConnectionCountFromSpecificElement(w2.elementB);

                    lc.ec.EnableConnectionAvailability(w2.elementA);
                    lc.ec.EnableConnectionAvailability(w2.elementB);

                    lc.wList.Remove(w2);
                    break;
                }
            }
        }

        public void ChangeLineStyle(Polyline pl, SolidColorBrush bc, double thickness)
        {
            bool singleLine = true;
            foreach (Wire w2 in lc.wList)
            {
                if (w2.GetName() == pl.Name)
                {

                    pl.Stroke = bc;
                    pl.StrokeThickness = thickness;
                    singleLine = false;
                    break;
                }
            }
            if (singleLine)
            {
                pl.Stroke = bc;
                pl.StrokeThickness = thickness;
            }
        }
        
        public void DrawWireBetweenElements(Image dot, string name, ElementControl ec, List<Dot> dList)
        {
            if (!ec.GetConnectionAvailability(name) && !isDotOccupied(dot, dList))
            {
                dot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotRed.png"));
                if (!turn && previousElementName != name && previousDotName != dot.Tag.ToString())
                {
                    Polyline pl = CreatePolyline();

                    previousElementName = name;
                    previousDotName = dot.Tag.ToString();

                    previousLine = pl;
                    previousDot = dot;

                    turn = true;
                }
                else if (previousElementName != name && previousDotName != dot.Tag.ToString())
                {
                    ec.AddConnectionCountToSpecificElement(previousElementName);
                    ec.AddConnectionCountToSpecificElement(name);

                    ec.EnableConnectionAvailability(previousElementName);
                    ec.EnableConnectionAvailability(name);
                    
                    previousLine.Name = previousElementName + name;

                    w = new Wire(previousLine.Name);                   
                    w.elementA = previousElementName;
                    w.elementB = name;                  

                    w.dotA = previousDotName;
                    w.dotB = dot.Tag.ToString();

                    foreach(Dot d in dList)
                    {
                        if(d.GetName() == previousDot.Tag.ToString() || d.GetName() == dot.Tag.ToString())
                        {
                            d.occupied = true;
                        }
                    }

                    w.AddPolyline(previousLine);
                    lc.wList.Add(w);                                        

                    ec.AddLineForElement(previousElementName, previousLine);
                    ec.AddLineForElement(name, previousLine);

                    foreach (Dot d in dList)
                    {                        
                        if (d.GetName() == w.dotA)
                        {
                            d.SetWireName(w.elementA);
                        }
                        if (d.GetName() == w.dotB)
                        {
                            d.SetWireName(w.elementB);
                        }
                    }                  

                    UpdateWireLocation(previousDotName, dot.Tag.ToString(), previousLine);

                    previousElementName = "";

                    turn = false;
                }
                else
                {                  
                    dot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                    if (dot.Tag.ToString() != previousDotName)
                    {                       
                        previousDot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                    }
                    previousLine = null;
                    previousDot = null;
                    previousElementName = "";
                    previousDotName = "";
                    
                    turn = false;
                }               
            }
            else
            {
                foreach (SpecificElement se in lc.ec.GetAllElements())
                {
                    if (se.GetName() == name)
                    {
                        if(ec.GetConnectionAvailability(name))
                            mgc.ShowWarningMessage(se.GetElement(), "This element has max connections used");
                        else if(isDotOccupied(dot,dList))
                        {
                            mgc.ShowWarningMessage(se.GetElement(), "This dot is being used by another wire");
                        }
                    }
                }
                //MessageBox.Show("This element has max connections used");
                if (ec.GetConnectionAvailability(name))
                    previousDot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
            }
        }

        public bool isDotOccupied(Image dot, List<Dot> dList)
        {
            foreach(Dot d in dList)
            {
                if (d.GetName() == dot.Tag.ToString())
                {
                    return d.occupied;
                }
            }
            return false;
        }

        public void RecreateWires()
        {           
            foreach (Wire w2 in lc.wList)
            {
                Polyline pl = CreatePolyline();
                pl.Name = w2.GetName();
                w2.AddPolyline(pl);

                foreach(SpecificElement se in lc.ec.GetAllElements())
                {
                    if(w2.elementA == se.GetName()||w2.elementB == se.GetName())
                    {
                        se.AddPolyline(pl);
                        foreach(Dot d in se.GetDots())
                        {
                            if(w2.dotA == d.GetName() || w2.dotB == d.GetName())
                            {
                                d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotRed.png"));
                            }
                        }
                        UpdateWireLocation(w2.dotA, w2.dotB, w2.GetPolyline());
                    }                   
                }              
            }
            mgc.UpdateContainer(lc);
        }
        
    }
}
