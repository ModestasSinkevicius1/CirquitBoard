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

        private MainWindow form;

        private Canvas canvas;
        private Grid grid;

        private int row = 0;

        private Ellipse el;

        private Point clickPosition;

        protected bool isDragging;       
       
        private TranslateTransform originTT;

        private int queue = 0;

        public WireGUIControl(MainWindow form, Canvas canvas, Grid grid, MessageGUIControl mgc, ListContainer lc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.mgc = mgc;
            this.lc = lc;
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

        private void ConnectWireToConnector(Polyline pl)
        {            
            string[] nameABC = new string[3];
            string[] dotABC = new string[3];

            string name;
            string dot;

            turn = false;
            
            previousLine.Name = pl.Name.ToString();

            foreach(Wire w in lc.wList)
            {
                if(pl.Name == w.GetName())
                {
                    nameABC[0] = w.elementA;
                    dotABC[0] = w.dotA;

                    nameABC[1] = w.elementB;
                    dotABC[1] = w.dotB;
                }
            }

            nameABC[2] = previousElementName;
            dotABC[2] = previousDotName;

            Image img = CreateConnector();
            SnapToClosestCell(img);
            canvas.Children.Add(img);

            lc.ec.UpdatePostitionValues(img.Tag.ToString());

            DeleteWires(pl);
            canvas.Children.Remove(previousLine);

            for (int i = 0; i < 3; i++)
            {                
                name = nameABC[i];
                dot = dotABC[i];
                
                Polyline newPl = CreatePolyline();                

                w = new Wire(dot + img.Tag.ToString());

                w.elementA = name;
                w.elementB = img.Tag.ToString();

                newPl.Name = dot + img.Tag.ToString();

                w.dotA = dot;
                w.dotB = img.Tag.ToString();

                w.AddPolyline(newPl);
                lc.wList.Add(w);

                lc.ec.AddLineForElement(name, newPl);
                lc.ec.AddLineForElement(img.Tag.ToString(), newPl);

                lc.ec.AddConnectionCountToSpecificElement(name);
                lc.ec.AddConnectionCountToSpecificElement(img.Tag.ToString());

                previousElementName = "";                

                UpdateWireLocation(w.dotA, w.dotB, newPl);
            }
        }        

        public void SnapToClosestCell(Image draggableControl)
        {
            double distanceX;
            double distanceY;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            int offCell = 0;

            double length = 50;

            double widthLength = grid.ColumnDefinitions[0].Width.Value;
            double heightLength = grid.RowDefinitions[0].Height.Value;

            if (widthLength < length)
            {
                if (widthLength < 25)
                {
                    if (widthLength < 12.5)
                    {
                        if (widthLength < 6.25)
                            if (widthLength < 3.125)
                            {
                                offCell = 7;
                            }
                            else
                                offCell = 15;
                        else
                            offCell = 7;
                    }
                    else
                        offCell = 3;
                }
                else
                    offCell = 1;
            }

            foreach (ColumnDefinition column in grid.ColumnDefinitions)
            {
                distanceX = (Math.Abs((Mouse.GetPosition(canvas).X - (column.Width.Value / 2)) - (column.Width.Value * i))) - column.Width.Value / 2;

                if (oldDistanceX > distanceX && i < grid.ColumnDefinitions.Count - offCell)
                {
                    oldDistanceX = distanceX;
                    cellX = i;
                }
                i++;
            }

            i = 0;
            length = 50;
            offCell = 0;

            if (heightLength < length)
            {
                if (heightLength < 25)
                {
                    if (heightLength < 12.5)
                    {
                        if (heightLength < 6.25)
                            if (heightLength < 3.125)
                            {
                                offCell = 7;
                            }
                            else
                                offCell = 15;
                        else
                            offCell = 7;
                    }
                    else
                        offCell = 3;
                }
                else
                    offCell = 1;
            }

            foreach (RowDefinition row in grid.RowDefinitions)
            {
                distanceY = (Math.Abs((Mouse.GetPosition(canvas).Y - (row.Height.Value / 2)) - (row.Height.Value * i))) - row.Height.Value / 2;

                if (oldDistanceY > distanceY && i < grid.RowDefinitions.Count - offCell)
                {
                    oldDistanceY = distanceY;
                    cellY = i;
                }
                i++;
            }

            cellWidth = grid.ColumnDefinitions[(int)cellX].Width.Value;
            cellHeight = grid.RowDefinitions[(int)cellY].Height.Value;

            Canvas.SetLeft(draggableControl, 0);
            Canvas.SetTop(draggableControl, 0);

            draggableControl.RenderTransform = new TranslateTransform((cellWidth * cellX)+19, (cellHeight * cellY) + 19);

            //Grid.SetRow(draggableControl, 0);
            //Grid.SetColumn(draggableControl, 0);
        }

        private Image CreateConnector()
        {
            Image img = new Image();

            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireConnectors/wire_connector.png"));
            img.Width = 10;
            img.Height = 10;
            img.Tag = "wire_connector" + queue;

            img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
            img.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
            img.MouseMove += new MouseEventHandler(Image_MouseMove);

            Panel.SetZIndex(img, 2);

            lc.ec.AddElementToList(img.Tag.ToString(), img);

            queue++;

            return img;
        }
        public void DeleteElement(Image draggableControl)
        {
            foreach (Polyline pl in lc.ec.GetLineListFromElement(draggableControl.Tag.ToString()))
            {
                DeleteWires(pl);
            }
            foreach (Dot d in lc.ec.GetDots(draggableControl.Tag.ToString()))
            {
                canvas.Children.Remove(d.GetDot());
            }
            lc.ec.RemoveElementFromList(draggableControl.Tag.ToString());
            canvas.Children.Remove(draggableControl);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image draggableControl = sender as Image;

            if (!Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.X))
            {                
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(form);
                draggableControl.CaptureMouse();
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                DeleteElement(draggableControl);
            }
            else
            {
                DrawWireBetweenElements(draggableControl, draggableControl.Tag.ToString(), lc.ec, lc.dList);
            }
                
                       
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();            
            SnapToClosestCell(draggable);   
            
            //lc.ec.UpdatePostitionValues(draggable.Tag.ToString());
            //wgc.FindWireConnectedDots(draggable.Tag.ToString());
            //hgc.IndicateCell(highlighting_rectangle);           
            //indicating_rectangle.Visibility = Visibility.Hidden;

            //ec.UpdatePostitionValues(draggable.Tag.ToString());
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Image draggableControl = sender as Image;

            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(form);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);          
                SnapToClosestCell(draggableControl);                
                FindWireConnectedDots(draggableControl.Tag.ToString());
                //dgc.UpadateDotsLocation(draggableControl, lc.ec);
                //hgc.Highlight_cell(draggableControl);

                //wgc.FindWireConnectedDots(draggableControl.Tag.ToString());
            }

        }

        private void Polyline_mouseLeave(object sender, MouseEventArgs e)
        {
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
            bool toLow = false;

            Image img = new Image();

            Polyline pl = sender as Polyline;

            if (Keyboard.IsKeyDown(Key.X))
            {
                foreach(SpecificElement se in lc.ec.GetAllElements())
                {
                    if (se.GetName().Length > 13)
                    {
                        if (se.GetName().Substring(0, 14) == "wire_connector")
                        {
                            if (se.GetConnectionCount() < 3)
                            {
                                img = se.GetElement();
                                toLow = true;
                            }
                        }
                    }
                }
                if (!toLow)
                    DeleteWires(pl);
                else
                    DeleteElement(img);
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
            else if(pl != previousLine && previousElementName != "")
            {                
                ConnectWireToConnector(pl);
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
            bool direction=false;
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
                if (ec.GetConnectionAvailability(name))
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
        
    }
}
