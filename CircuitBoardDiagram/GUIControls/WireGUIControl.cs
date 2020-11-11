using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram.GUIControls
{
    class WireGUIControl
    {
        public bool turn = false;       
        public string previousElementName = "";
        public string previousDotName = "";

        public Polyline previousLine;

        private ListContainer lc;

        private ShortcutGUIControl sgc;

        private Wire w;
        private Image previousDot;


        private MessageGUIControl mgc;
        public ConnectorGUIControl cogc { get; set; }

        private MainWindow form;

        private Canvas canvas;
        private Grid grid;

        private int queue = 0;

        private Ellipse el;                                         

        public WireGUIControl(MainWindow form, Canvas canvas, Grid grid, MessageGUIControl mgc, ListContainer lc, ShortcutGUIControl sgc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.mgc = mgc;           
            this.lc = lc;
            this.sgc = sgc;
        }

        public void BeginDrawing()
        {           
            Thread th = new Thread(UpdateWirePosition);
            th.IsBackground = true;
            th.Start();

            /*foreach(Dot d in tmpList)
            {
                d.GetDot().Visibility = Visibility.Hidden;
            }
            */
        }
        public void UpdateWirePosition()
        {
            //Point startPostition = (Point)obj;
            
            while (turn)
            {
                Thread.Sleep(30);
                form.Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateWireLocation(previousDotName, "mouse", previousLine);
                }));
            }          
        }

        public void UpdateListContainer(ListContainer lc)
        {
            this.lc = lc;
        }

        private void Polyline_mouseEnter(object sender, MouseEventArgs e)
        {
            sgc.UpdateText("OnControl");
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();

            if (!turn)
            {                                
                if (Keyboard.IsKeyDown(Key.X))
                {
                    bc.Color = Colors.Red;
                }
                else
                    bc.Color = Colors.Green;

                ChangeLineStyle(pl, bc, 4);
            }
            else if(pl != previousLine)
            {
                bc.Color = Colors.Gold;
                ChangeLineStyle(pl, bc, 4);                
                /*el = new Ellipse();
                el.Stroke = Brushes.Black;
                el.Fill = Brushes.AliceBlue;
                el.Width = 50;
                el.Height = 50;
                canvas.Children.Add(el);
                */
            }
        }                             
        
        private void Polyline_mouseLeave(object sender, MouseEventArgs e)
        {
            sgc.UpdateText("OnCanvas");
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();

            bc.Color = Colors.Black;
            ChangeLineStyle(pl, bc, 1);
           
            if (pl != previousLine)
            {                
                canvas.Children.Remove(el);
                el = null;                
            }
        }

        private void Polyline_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
            Image img = new Image();

            bool foundA = false;
            bool foundB = false;

            string elementA = "";
            string elementB = "";

            Polyline pl = sender as Polyline;

            if (Keyboard.IsKeyDown(Key.X))
            {
                foreach(Wire w2 in lc.wList)
                {
                    if(w2.GetPolyline().Name == pl.Name)
                    {
                        if(RemoveNumbers(w2.elementA) == "wire_connector" && lc.ec.GetConnectionCount(w2.elementA)<4)
                        {
                            elementA = w2.elementA;
                            elementB = w2.elementB;

                            foundA = true;
                        }
                        if(RemoveNumbers(w2.elementB) == "wire_connector" && lc.ec.GetConnectionCount(w2.elementB)<4)
                        {
                            elementB = w2.elementB;
                            elementA = w2.elementA;

                            foundB = true;
                        }
                    }
                }
                if(foundA)
                {
                    RewireBetweenElements(elementA, elementB);
                }
                if(foundB)
                {
                    RewireBetweenElements(elementB, elementA);
                }
                //RewireBetweenElements(pl);
                DeleteWires(pl);
               
            }
            else if (Keyboard.IsKeyDown(Key.C))
            {
                foreach (Wire w2 in lc.wList)
                {                   
                    if (w2.GetName() == pl.Name)
                    {
                        MessageBox.Show("line: "+ w2.GetName()+ "||" + w2.elementA + " connected with " + w2.elementB);
                        break;
                    }
                }
            }
            else if(pl != previousLine && previousElementName != "")
            {                
                cogc.ConnectWireToConnector(pl, false);
            }
            else if(Keyboard.IsKeyDown(Key.W) && !turn)
            {
                cogc.ConnectWireToConnector(pl, true);
            }
        }       
       
        private void RewireBetweenElements(string deleteElement, string avoidElement)
        {
            string targetElementA = "";
            string targetElementB = "";

            string targetDotA = "";
            string targetDotB = "";

            Image img = null;

            foreach (SpecificElement se in lc.ec.GetAllElements())
            {
                if(se.GetName()==deleteElement)
                {
                    img = se.GetElement();
                    foreach(SpecificElement se2 in se.GetElements())
                    {
                        if(se2.GetName() != avoidElement && targetElementA == "")
                        {                           
                            targetElementA = se2.GetName();
                            targetDotA = se2.GetDots()[0].GetName();
                        }
                        else if(se2.GetName() != avoidElement && targetElementB == "")
                        {
                            targetElementB = se2.GetName();
                            targetDotB = se2.GetDots()[0].GetName();
                        }                        
                    }
                }
            }
            cogc.DeleteElement(img);

            Polyline pl = CreatePolyline();
            pl.Name = targetElementA + targetElementB + queue;

            UpdateWireLocation(targetDotA, targetDotB, pl);
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

            canvas.Children.Add(pl);

            return pl;
        }

        
        public void UpdateWireLocation(string dotNameA, string dotNameB, Polyline pl)
        {
            bool direction = false;
            int n = -1;

            Point startLine1;
            Point startline2;
            Point line1;
            Point line2;
            Point endLine2;
            Point endLine1;            

            PointCollection polylinePoints = new PointCollection();                     

            Image dotA = FindDot(dotNameA);
           
            if(dotA == null)
            {
                foreach (SpecificElement se in lc.ec.GetAllElements())
                {
                    if (se.GetName() == dotNameA)
                    {
                        dotA = se.GetElement();
                    }
                }
            }

            if (dotA != null)
            {
                direction = DetermineDirection(dotNameA);
                n = DetermineDirection2(dotNameA);

                startLine1 = new Point(dotA.RenderTransform.Value.OffsetX + 7, dotA.RenderTransform.Value.OffsetY + 9);

                if (dotNameB == "mouse")
                    endLine1 = new Point(Mouse.GetPosition(canvas).X, Mouse.GetPosition(canvas).Y);
                else
                {
                    Image dotB = FindDot(dotNameB);

                    if (dotB != null)
                    {
                        endLine1 = new Point(dotB.RenderTransform.Value.OffsetX + 7, dotB.RenderTransform.Value.OffsetY + 5);
                    }
                    else
                    {
                        foreach (SpecificElement se in lc.ec.GetAllElements())
                        {
                            if (se.GetName() == dotNameB)
                            {
                                dotB = se.GetElement();
                            }
                        }
                        endLine1 = new Point(dotB.RenderTransform.Value.OffsetX + 5, dotB.RenderTransform.Value.OffsetY + 7);
                    }
                }

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

                    lc.ec.RemoveChildElementFromElement(w2.elementA, w2.elementB);
                    lc.ec.RemoveChildElementFromElement(w2.elementB, w2.elementA);

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
        
        public void DrawWireFromConnector(Image dot, string name, ElementControl ec, List<Dot> dList)
        {
            if (!ec.GetConnectionAvailability(name) && !isDotOccupied(dot, dList))
            {               
                if (!turn && previousElementName != name && previousDotName != dot.Tag.ToString())
                {
                    Polyline pl = CreatePolyline();

                    Panel.SetZIndex(pl, 0);                  

                    previousElementName = name;
                    previousDotName = dot.Tag.ToString();

                    previousLine = pl;
                    previousDot = dot;

                    turn = true;

                    BeginDrawing();
                }
                else if (previousElementName != name && previousDotName != dot.Tag.ToString())
                {
                    Panel.SetZIndex(previousLine, 2);

                    ec.AddConnectionCountToSpecificElement(previousElementName);
                    ec.AddConnectionCountToSpecificElement(name);

                    ec.EnableConnectionAvailability(previousElementName);
                    ec.EnableConnectionAvailability(name);

                    previousLine.Name = previousElementName + name + queue;

                    w = new Wire(previousLine.Name);
                    w.elementA = previousElementName;
                    w.elementB = name;

                    w.dotA = previousDotName;
                    w.dotB = dot.Tag.ToString();

                    foreach (Dot d in dList)
                    {
                        if (d.GetName() == previousDot.Tag.ToString() || d.GetName() == dot.Tag.ToString())
                        {
                            d.occupied = true;
                        }
                    }

                    w.AddPolyline(previousLine);
                    lc.wList.Add(w);

                    ec.AddLineForElement(previousElementName, previousLine);
                    ec.AddLineForElement(name, previousLine);

                    ec.AddElementToParentElement(previousElementName, name);
                    ec.AddElementToParentElement(name, previousElementName);

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

                    queue++;
                }
                else
                {
                    turn = false;                   
                    canvas.Children.Remove(previousLine);

                    previousLine = null;
                    previousDot = null;
                    previousElementName = "";
                    previousDotName = "";


                }
            }
            else
            {
                foreach (SpecificElement se in lc.ec.GetAllElements())
                {
                    if (se.GetName() == name)
                    {
                        if (ec.GetConnectionAvailability(name))
                            mgc.ShowWarningMessage(se.GetElement(), "This element has max connections used");
                        else if (isDotOccupied(dot, dList))
                        {
                            mgc.ShowWarningMessage(se.GetElement(), "This dot is being used by another wire");
                        }
                    }
                }                
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

                    Panel.SetZIndex(pl, 0);

                    //MessageBox.Show(Panel.GetZIndex(pl).ToString());

                    previousElementName = name;
                    previousDotName = dot.Tag.ToString();

                    previousLine = pl;
                    previousDot = dot;

                    turn = true;

                    BeginDrawing();
                }
                else if (previousElementName != name && previousDotName != dot.Tag.ToString())
                {
                    Panel.SetZIndex(previousLine, 2);

                    ec.AddConnectionCountToSpecificElement(previousElementName);
                    ec.AddConnectionCountToSpecificElement(name);

                    ec.EnableConnectionAvailability(previousElementName);
                    ec.EnableConnectionAvailability(name);
                    
                    previousLine.Name = previousElementName + name + queue;

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

                    ec.AddElementToParentElement(previousElementName, name);
                    ec.AddElementToParentElement(name, previousElementName);

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

                    queue++;
                }
                else
                {
                    turn = false;
                    dot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                    if (dot.Tag.ToString() != previousDotName)
                    {                       
                        previousDot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                    }
                    canvas.Children.Remove(previousLine);

                    previousLine = null;
                    previousDot = null;
                    previousElementName = "";
                    previousDotName = "";
                    
                    
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
                if (ec.GetConnectionAvailability(name) && previousDot != null)
                {                    
                    previousDot.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                }
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
        private string RemoveNumbers(string name)
        {
            foreach (char w in name)
            {
                if (Char.IsNumber(w))
                {
                    name = name.Remove(name.Length - 1);
                }
            }

            return name;
        }
    }
}
