using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected bool isDragging_image;
        protected bool isDragging_canvas;
        private Point clickPosition;
        private TranslateTransform originTT;
        private ElementControl ec = new ElementControl();
        private TextBlock tb = new TextBlock();

        private List<Wire> wList = new List<Wire>();
        private List<Dot> dList = new List<Dot>();
        private Wire w;

        private bool turn = false;


        private Line previousLine;
        private string previousElementName = "";
        private string previousDotName = "";
        
        private string currentImageName;
       
        private int queue = 0;
        private int next = 0;

        private bool isOnImage = false;

        private bool linePosition = false;
        


        public MainWindow()
        {
            InitializeComponent();            
            indicating_rectangle.Visibility = Visibility.Hidden;
            highlighting_rectangle.Visibility = Visibility.Hidden;           

            LoadImages();
            LoadPopupMessage();
            LoadGrids();
            //CheckActivePopupMessage();
        }   
        
        private void CheckActivePopupMessage()
        {
            Thread th = new Thread(UpdatePopupStatus);
            th.IsBackground = true;
            th.Start();           
        }

        private void UpdatePopupStatus()
        {            
            

            while (true)
            {
                Thread.Sleep(50);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!tb.IsMouseOver)
                        tb.Visibility = Visibility.Hidden;                 

                }));
            }

        }       

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {          
            Image draggableControl = sender as Image;           

            if (!Keyboard.IsKeyDown(Key.W)&&!Keyboard.IsKeyDown(Key.C)&&!Keyboard.IsKeyDown(Key.X))
            {
                highlighting_rectangle.Visibility = Visibility.Hidden;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging_image = true;
                clickPosition = e.GetPosition(this);               
                draggableControl.CaptureMouse();                
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {               
                foreach(Line l in ec.GetLineListFromElement(draggableControl.Tag.ToString()))
                {
                    DeleteWires(l);
                }
                foreach(Dot d in ec.GetDots(draggableControl.Tag.ToString()))
                {
                    canvasGrid.Children.Remove(d.GetDot());
                }
                ec.RemoveElementFromList(draggableControl.Tag.ToString());
                canvas.Children.Remove(draggableControl);
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {               
                ShowPopupMessage(draggableControl);
            }
            

        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {                        
            isDragging_image = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();            
            SnapToClosestCell(draggable);
            UpadateDotsLocation(draggable);
            //UpdateLineLocation(draggable);
            IndicateCell(highlighting_rectangle);           
            indicating_rectangle.Visibility = Visibility.Hidden;            
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Image draggableControl = sender as Image;            
                                              
            if (isDragging_image && draggableControl != null && isOutOfBounds(e))
            {          
                Point currentPosition = e.GetPosition(this);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                
                SnapToClosestCell(draggableControl);
                UpadateDotsLocation(draggableControl);
                UpdateLineLocation(draggableControl);
            }
            
        }

        private void canvas_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.E) && currentImageName != null)
            {
                Image r = new Image();
                r.Height = 50;
                r.Width = 50;
                r.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+"Circuit element images/"+currentImageName+".png"));
                r.Tag = currentImageName;
                AddImageToCommon(r.Tag.ToString());
                
                r.Tag = currentImageName + queue;                
                ec.AddElementToList(r.Tag.ToString());

                r.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
                r.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
                r.MouseMove += new MouseEventHandler(Canvas_MouseMove);
                r.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                r.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                canvas.Children.Add(r);
                Canvas.SetTop(r, Mouse.GetPosition(canvas).Y - r.Width / 2);
                Canvas.SetLeft(r, Mouse.GetPosition(canvas).X - r.Height / 2);

                Panel.SetZIndex(r, 1);

                CreateDot(r.Tag.ToString(), 4);
                
                queue++;               
            }

            if(Keyboard.IsKeyDown(Key.LeftShift))
            {
                Canvas draggableControl = sender as Canvas;
                highlighting_rectangle.Visibility = Visibility.Hidden;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging_canvas = true;
                clickPosition = e.GetPosition(this);
                draggableControl.CaptureMouse();
            }

        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging_canvas = false;
            Canvas draggable = sender as Canvas;            
            draggable.ReleaseMouseCapture();           
        }

        private void canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            Canvas draggableControl = sender as Canvas;

            if (isDragging_canvas)
            {
                Point currentPosition = e.GetPosition(this);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);

                
                    transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                
                    draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
            }

            if(draggableControl.RenderTransform.Value.OffsetY > 50.0f)
            {
                draggableControl.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, 50.0f);
            }

            if (draggableControl.RenderTransform.Value.OffsetX > 50.0f)
            {
                draggableControl.RenderTransform = new TranslateTransform(50.0f, draggableControl.RenderTransform.Value.OffsetY);
            }

            if (draggableControl.RenderTransform.Value.OffsetY < -1300f)
            {
                //MessageBox.Show("I have reached my limit");
                draggableControl.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, -1300f);
            }

            if (draggableControl.RenderTransform.Value.OffsetX < -4250f)
            {
                draggableControl.RenderTransform = new TranslateTransform(-4250f, draggableControl.RenderTransform.Value.OffsetY);
            }

            if (!isOnImage || !IsMouseCaptured)
                IndicateCell(indicating_rectangle);
            else
            {
                indicating_rectangle.Visibility = Visibility.Hidden;
            }
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Line_mouseEnter(object sender, MouseEventArgs e)
        {
            Line l = sender as Line;
            SolidColorBrush bc = new SolidColorBrush();

            if(Keyboard.IsKeyDown(Key.X))
            {
                bc.Color = Colors.Red;
            }
            else
                bc.Color = Colors.Green;

            ChangeLineStyle(l, bc, 4);
        }

        private void Line_mouseEnter_1(object sender, MouseEventArgs e)
        {
            Line l = sender as Line;
            SolidColorBrush bc = new SolidColorBrush();
            
            bc.Color = Colors.Green;

            ChangeLineStyle(l, bc, 3);
        }

        private void Line_mouseLeave(object sender, MouseEventArgs e)
        {
            Line l = sender as Line;
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            ChangeLineStyle(l, bc, 1);    
        }

        private void Line_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Line l = sender as Line;
            /*
            originTT = l.RenderTransform as TranslateTransform ?? new TranslateTransform();
            isDragging = true;
            clickPosition = e.GetPosition(this);

            if (l.X1 == l.X2)
            {
                linePosition = true;
            }
            else
                linePosition = false;

            l.CaptureMouse();
            */
            if (Keyboard.IsKeyDown(Key.X))
            {
                DeleteWires(l);                         
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {                              
                foreach (Wire w2 in wList)
                {
                    if (w2.GetName() == l.Name)
                    {
                        MessageBox.Show(w2.elementA + " connected with " + w2.elementB);
                        break;
                    }
                }
            }
        }

        private void Line_mouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //isDragging = false;
            Line draggable = sender as Line;
            //draggable.ReleaseMouseCapture();            
            //UpdateLineLocation2(draggable);           
        }

        private void Line_mouseMove(object sender, MouseEventArgs e)
        {
            Line draggableControl = sender as Line;

            List<Line> lList;

            double length = 10;

            int dotX = 0;
            int dotY = 0;
          

            /*if (isDragging && draggableControl != null && isOutOfBounds(e))
            {                             
                
                //UpdateLineLocation2(draggableControl);               
            }
            */
        }

        private void Line_mouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Line draggableControl = sender as Line;
            string name = "";                           
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            currentImageName = img.Tag.ToString();
        }
       
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {            
            isOnImage = true;
            Image draggableControl = sender as Image;
            foreach (Dot d in ec.GetDots(draggableControl.Tag.ToString()))
            {
                d.GetDot().Visibility = Visibility.Visible;
            }
            Highlight_cell(draggableControl);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            isOnImage = false;
            Image draggableControl = sender as Image;
            /*foreach (Line l in ec.GetLineListFromElement(draggableControl.Tag.ToString()))
            {
                //l.Visibility = Visibility.Hidden;
            }*/
            highlighting_rectangle.Visibility = Visibility.Hidden;
        }

        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            string name = "";

            foreach (Dot d in dList)
            {
                if(img.Tag.ToString()==d.GetName())
                {
                    name = d.GetCore();
                    DrawWireBetweenElements(img,name);
                }
            }
        }

        private void Image_MouseLeave_2(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            foreach(Dot d in dList)
            {
                if(d.GetName()==img.Tag.ToString()&& img != null)
                {
                    foreach(Dot d2 in ec.GetDots(d.GetCore()))
                    {
                        d2.GetDot().Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void Image_MouseEnter_2(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            foreach (Dot d in dList)
            {
                if (d.GetName() == img.Tag.ToString())
                {
                    foreach (Dot d2 in ec.GetDots(d.GetCore()))
                    {
                        d2.GetDot().Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            indicating_rectangle.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OptionWindow opWindow = new OptionWindow(canvasGrid.ColumnDefinitions[0].Width.Value, canvasGrid.RowDefinitions[0].Height.Value);
            bool? result = opWindow.ShowDialog();

            if (opWindow.isPressedOk == true)
            {
                ResetColumn();
                ResetRow();

                AddColumn(opWindow.slider.Value, 12);
                AddRow(opWindow.slider_Copy.Value, 7);

                UpdateIndicatorSize();
                UpdateHighlightorSize();
            }
        }

        private void Textbox_MouseLeave(object sender, MouseEventArgs e)
        {            
            tb.Visibility = Visibility.Hidden;
        }
        private void CreateDot(string name, int count)
        {
            for(int i=0;i<count;i++)
            {
                Image img = new Image();
                img.Visibility = Visibility.Hidden;
                img.Width = 15;
                img.Height = 15;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                img.Tag = name+"_"+i;

                img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown_2);               
                img.MouseLeave += new MouseEventHandler(Image_MouseLeave_2);

                Panel.SetZIndex(img, 2);
                canvasGrid.Children.Add(img);
                Dot d = new Dot(img.Tag.ToString(), name, img);
                ec.AddDot(name, d);
                dList.Add(d);
            }           
        }       

        private void UpadateDotsLocation(Image draggableControl)
        {
            List<Dot> dList = ec.GetDots(draggableControl.Tag.ToString());

            double x = draggableControl.RenderTransform.Value.OffsetX;
            double y = draggableControl.RenderTransform.Value.OffsetY;

            double length = 10;
           
            for (int i = 0; i < dList.Count; i++)
            {                                             
                if (i <= 0)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x+25, y);
                }
                else if(i <= 1)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x, y+25);
                }
                else if(i <= 2)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x-25, y);
                }
                else
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x, y-25);
                }               
                dList[i].GetDot().Visibility = Visibility.Visible;
            }
            
        }      

        private void SnapToClosestCell(Image draggableControl)
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

            double widthLength = canvasGrid.ColumnDefinitions[0].Width.Value;
            double heightLength = canvasGrid.RowDefinitions[0].Height.Value;            

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

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
            {
                distanceX = (Math.Abs((Mouse.GetPosition(canvas).X - (column.Width.Value/2)) - (column.Width.Value * i)))-column.Width.Value/2;

                if (oldDistanceX > distanceX && i<canvasGrid.ColumnDefinitions.Count - offCell)
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

            foreach (RowDefinition row in canvasGrid.RowDefinitions)
            {
                distanceY = (Math.Abs((Mouse.GetPosition(canvas).Y - (row.Height.Value/2)) - (row.Height.Value * i)))-row.Height.Value/2;

                if (oldDistanceY > distanceY && i< canvasGrid.RowDefinitions.Count - offCell)
                {
                    oldDistanceY = distanceY;
                    cellY = i;
                }
                i++;
            }          

            cellWidth = canvasGrid.ColumnDefinitions[(int)cellX].Width.Value;
            cellHeight = canvasGrid.RowDefinitions[(int)cellY].Height.Value;

            Canvas.SetLeft(draggableControl, 0);
            Canvas.SetTop(draggableControl, 0);
          
            draggableControl.RenderTransform = new TranslateTransform(cellWidth * cellX,cellHeight*cellY);                    
            
            Grid.SetRow(draggableControl, 0);
            Grid.SetColumn(draggableControl, 0);
            

        }

        private void ChangeLineStyle(Line l, SolidColorBrush bc, double thickness)
        {
            bool singleLine = true;
            foreach (Wire w2 in wList)
            {
                if (w2.GetName() == l.Name)
                {
                    foreach (Line l2 in w2.GetList())
                    {
                        l2.Stroke = bc;
                        l2.StrokeThickness = thickness;
                    }
                    singleLine = false;
                    break;
                }
            }
            if(singleLine)
            {
                l.Stroke = bc;
                l.StrokeThickness = thickness;
            }
        }

        private void IndicateCell(Shape draggableControl)
        {
            draggableControl.Visibility = Visibility.Visible;

            double distanceX;
            double distanceY;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
            {
                distanceX = Math.Abs((Mouse.GetPosition(canvas).X- (column.Width.Value/2)) - (column.Width.Value * i));

                if (oldDistanceX > distanceX)
                {
                    oldDistanceX = distanceX;
                    cellX = i;
                }
                i++;
            }

            i = 0;

            foreach (RowDefinition row in canvasGrid.RowDefinitions)
            {
                distanceY = Math.Abs((Mouse.GetPosition(canvas).Y- (row.Height.Value/2)) - (row.Height.Value * i));

                if (oldDistanceY > distanceY)
                {
                    oldDistanceY = distanceY;
                    cellY = i;
                }
                i++;
            }

            cellWidth = canvasGrid.ColumnDefinitions[(int)cellX].Width.Value;
            cellHeight = canvasGrid.RowDefinitions[(int)cellY].Height.Value;

            Canvas.SetLeft(draggableControl, 0);
            Canvas.SetTop(draggableControl, 0);

            draggableControl.RenderTransform = new TranslateTransform(cellWidth * cellX, cellHeight * cellY);

            Grid.SetRow(draggableControl, 0);
            Grid.SetColumn(draggableControl, 0);
        }

        private void DrawWireBetweenElements(Image draggableControl, string name)
        {
            if (!ec.GetConnectionAvailability(name))
            {
                draggableControl.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotRed.png"));
                if (!turn && previousElementName != name)
                {
                    Line l = CreateLine(true);
                    l.Name = "line" + name;

                    ec.AddConnectionCountToSpecificElement(name);                   

                    l.X1 = (draggableControl.RenderTransform.Value.OffsetX + 25);
                    l.Y1 = (draggableControl.RenderTransform.Value.OffsetY + 25);

                    l.X2 = l.X1;
                    l.Y2 = l.Y1;

                    previousElementName = name;
                    previousDotName = draggableControl.Tag.ToString();

                    previousLine = l;

                    turn = true;
                }
                else if (name != previousElementName)
                {
                    Line l = CreateLine(true);

                    ec.AddConnectionCountToSpecificElement(name);

                    previousLine.X2 = draggableControl.RenderTransform.Value.OffsetX + 25;

                    l.X1 = previousLine.X2;
                    l.Y1 = previousLine.Y2;

                    l.X2 = previousLine.X2;
                    l.Y2 = draggableControl.RenderTransform.Value.OffsetY + 25;

                    previousLine.Name += name;
                    l.Name = previousLine.Name;

                    w = new Wire(previousLine.Name);
                    w.elementA = previousElementName;
                    w.elementB = name;

                    w.dotA = previousDotName;
                    w.dotB = draggableControl.Tag.ToString();

                    w.AddList(previousLine);
                    w.AddList(l);
                    wList.Add(w);

                    ec.AddLineForElement(previousElementName, previousLine);
                    ec.AddLineForElement(previousElementName, l);

                    ec.AddLineForElement(name, previousLine);
                    ec.AddLineForElement(name, l);

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


                    previousElementName = name;

                    turn = false;
                }
                ec.EnableConnectionAvailability(name);
            }
            else
                MessageBox.Show("This element has max connections used");
        }
        
        private Line CreateLine(bool seperateLine)
        {
            Line l = new Line();
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            l.StrokeThickness = 2;
            l.Stroke = bc;

            if (seperateLine)
            {
                l.MouseLeftButtonDown += new MouseButtonEventHandler(Line_mouseLeftButtonDown);
                l.MouseEnter += new MouseEventHandler(Line_mouseEnter);
                l.MouseLeave += new MouseEventHandler(Line_mouseLeave);
                l.MouseMove += new MouseEventHandler(Line_mouseMove);
                l.MouseLeftButtonUp += new MouseButtonEventHandler(Line_mouseLeftButtonUp);
            }

            canvas.Children.Add(l);            

            return l;
        }

        private void UpdateLineLocation2(Line l)
        {
            foreach(Wire w in wList)
            {
                if(l.Name == w.GetName())
                {
                    foreach(Object obj in canvas.Children)
                    {
                        if (obj.GetType() == typeof(Image))
                        {
                            Image draggableControl = obj as Image;
                            if (w.elementA == draggableControl.Tag.ToString() || w.elementB == draggableControl.Tag.ToString())
                            {                               
                                foreach(Line l2 in w.GetList())
                                {
                                    if(l != l2)
                                    {
                                        if(linePosition)
                                            l2.X2 = l.X1;
                                        else
                                            l2.Y1 = l.Y2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void UpdateLineLocation(Image draggableControl)
        {          
            int i = 0;
            int g = 0;

            int wireNumA = 0;
            int wireNumB = 0;

            List<Dot> lList = ec.GetDots(draggableControl.Tag.ToString());

            foreach(Wire w2 in wList)
            {               
                if (w2.elementA == draggableControl.Tag.ToString())
                {
                    foreach (Dot d in lList)
                    {                       
                        if (w2.dotA == d.GetDot().Tag.ToString())
                        {                            
                            wireNumA = g;
                            break;
                        }
                        g++;
                    }                   
                    foreach (Line l in w2.GetList())
                    {
                        if (i <= 0)
                        {                           
                            l.X1 = lList[wireNumA].GetDot().RenderTransform.Value.OffsetX+25;
                            l.Y1 = lList[wireNumA].GetDot().RenderTransform.Value.OffsetY+25;

                            l.Y2 = l.Y1;                           
                        }
                        else
                        {
                            l.Y1 = lList[wireNumA].GetDot().RenderTransform.Value.OffsetY+25;
                        }
                        i++;
                    }
                    i = 0;
                    g = 0;
                }              
                if(w2.elementB == draggableControl.Tag.ToString())
                {
                    foreach(Dot d in lList)
                    {
                        if (w2.dotB == d.GetDot().Tag.ToString())
                        {
                            wireNumB = g;
                            break;
                        }
                        g++;
                    }                    
                    for (int j=w2.GetList().Count-1;j>=0;j--)
                    {
                        if (i <= 0)
                        {                           
                            w2.GetList()[j].X2 = lList[wireNumB].GetDot().RenderTransform.Value.OffsetX + 25;
                            w2.GetList()[j].Y2 = lList[wireNumB].GetDot().RenderTransform.Value.OffsetY + 25;

                            w2.GetList()[j].X1 = w2.GetList()[j].X2;
                        }
                        else
                        {                           
                            w2.GetList()[j].X2 = lList[wireNumB].GetDot().RenderTransform.Value.OffsetX + 25;
                        }
                        i++;
                    }
                    i = 0;
                    g = 0;
                }
            }
        }

        private void LoadImages()
        {
            DatabaseControl dc = new DatabaseControl();

            Image img;

            int i = 0;
            int y = 0;
            
            foreach(DatabaseElement e in dc.GetElements())
            {
                if(i > 7)
                {
                    i = 0;
                    y++;
                }
                img = new Image();
                img.Width = 22;
                img.Height = 22;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + e.GetElementType() + ".png"));
                img.Tag = e.GetElementType();
                img.Stretch = Stretch.Fill;

                img.MouseLeftButtonDown += new MouseButtonEventHandler(image_MouseLeftButtonDown);

                Grid.SetColumn(img, i);
                Grid.SetRow(img, y);
                grid_expander.Children.Add(img);
                i++;
            }                   
        }

        private void AddImageToCommon(string imageName)
        {           
            int i = 0;
            bool allowAdd = true;

            if (next > 5)
                next = 0;

            foreach(Image img in dock_bottom.Children)
            {
                if(img.Tag!=null)
                    if(img.Tag.ToString()==imageName)
                    {
                        allowAdd = false;
                    }
            }

            if (allowAdd)
            {
                foreach (Image img in dock_bottom.Children)
                {
                    if (img.Tag == null)
                    {
                        img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + imageName + ".png"));
                        img.Tag = imageName;
                        break;
                    }
                    else if (i > 4)
                    {
                        Image img2 = dock_bottom.Children[next] as Image;
                        img2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + imageName + ".png"));
                        img2.Tag = imageName;
                        next++;
                    }
                    i++;
                }
            }
        }

        private void UpdateIndicatorSize()
        {
            double lengthX = canvasGrid.ColumnDefinitions[0].Width.Value;
            double lengthY = canvasGrid.RowDefinitions[0].Height.Value;

            int spanX = 1;
            int spanY = 1;

            if (lengthX<50)
            {
                if (lengthX < 25)
                {
                    if (lengthX < 12.5)
                    {
                        if (lengthX < 6.25)
                        {
                            spanX = 16;
                        }
                        else
                            spanX = 8;
                    }
                    else
                        spanX = 4;
                }
                else
                    spanX = 2;
            }

            if (lengthY < 50)
            {
                if (lengthY < 25)
                {
                    if (lengthY < 12.5)
                    {
                        if (lengthY < 6.25)
                        {
                            spanY = 16;
                        }
                        else
                            spanY = 8;
                    }
                    else
                        spanY = 4;
                }
                else
                    spanY = 2;
            }

            indicating_rectangle.SetValue(Grid.ColumnSpanProperty, spanX);
            indicating_rectangle.SetValue(Grid.RowSpanProperty, spanY);
        }

        private void UpdateHighlightorSize()
        {
            double lengthX = canvasGrid.ColumnDefinitions[0].Width.Value;
            double lengthY = canvasGrid.RowDefinitions[0].Height.Value;

            int spanX = 2;
            int spanY = 2;

            if (lengthX < 50)
            {
                if (lengthX < 25)
                {
                    if (lengthX < 12.5)
                    {
                        if (lengthX < 6.25)
                        {
                            spanX = 32;
                        }
                        else
                            spanX = 16;
                    }
                    else
                        spanX = 8;
                }
                else
                    spanX = 4;
            }

            if (lengthY < 50)
            {
                if (lengthY < 25)
                {
                    if (lengthY < 12.5)
                    {
                        if (lengthY < 6.25)
                        {
                            spanY = 32;
                        }
                        else
                            spanY = 16;
                    }
                    else
                        spanY = 8;
                }
                else
                    spanY = 4;
            }

            highlighting_rectangle.SetValue(Grid.ColumnSpanProperty, spanX);
            highlighting_rectangle.SetValue(Grid.RowSpanProperty, spanY);
        }

        private bool isOutOfBounds(MouseEventArgs e)
        {
            Point cursorP = e.GetPosition(this);

            double gridX = 50 - canvasGrid.ColumnDefinitions[0].Width.Value;
            double gridY = 50 - canvasGrid.RowDefinitions[0].Height.Value;
           
            if (cursorP.X<canvas.Margin.Left || cursorP.Y<canvas.Margin.Top || cursorP.X>(canvas.Width-gridX)+canvas.Margin.Left || cursorP.Y > (canvas.Height-gridY) + canvas.Margin.Top)
            {
                return false;
            }
            return true;
        }

        private void Highlight_cell(Image draggableControl)
        {
            highlighting_rectangle.Visibility = Visibility.Visible;           
            highlighting_rectangle.RenderTransform = draggableControl.RenderTransform;
        }
        
        private void ResetColumn()
        {
            int count = canvasGrid.ColumnDefinitions.Count;
            int i = 0;

            while(count>0)
            {
                canvasGrid.ColumnDefinitions.RemoveAt(0);
                count = canvasGrid.ColumnDefinitions.Count;
                i++;
            }
        }

        private void ResetRow()
        {
            int count = canvasGrid.RowDefinitions.Count;
            int i = 0;

            while (count > 0)
            {
                canvasGrid.RowDefinitions.RemoveAt(0);
                count = canvasGrid.RowDefinitions.Count;
                i++;
            }
        }

        private void AddColumn(double value, int count)
        {
            double originalValue = 50;

            int i = 0;

            int newCount = count;

            while (originalValue > value)
            {
                newCount += newCount;
                originalValue /= 2;
            }            

            while (i < newCount)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(value);
                canvasGrid.ColumnDefinitions.Add(c);

                i++;
            }            

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
            {
                column.Width = new GridLength(value);
            }
        }

        private void AddRow(double value, int count)
        {
            double originalValue = 50;           

            int i = 0;
            
            int newCount = count;

            while (originalValue > value)
            {
                newCount += newCount;
                originalValue /= 2;                
            }                     

            while (i < newCount)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(value);
                canvasGrid.RowDefinitions.Add(r);

                i++;
            }
            
            foreach (RowDefinition row in canvasGrid.RowDefinitions)
            {
                row.Height = new GridLength(value);
            }
        }
        
        private void LoadPopupMessage()
        {
            tb.Visibility = Visibility.Hidden;
            SolidColorBrush scb = Brushes.LightBlue;

            tb.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);

            tb.Width = 100;
            tb.Height = 100;
            tb.Background = scb;
            tb.Opacity = 0.90;            
            Panel.SetZIndex(tb, 2);
            canvas.Children.Add(tb);
        }

        private void LoadGrids()
        {
            canvasGrid.Width = 600 * 8;
            canvasGrid.Height = 350 * 5;

            ResetColumn();
            ResetRow();

            AddColumn(50, 12*8);
            AddRow(50, 7*5);
        }
        private void ShowPopupMessage(Image draggableControl)
        {
            tb.Visibility = Visibility.Visible;
            tb.Text = "Im working!";
            tb.RenderTransform = draggableControl.RenderTransform;
        }
        
        private void DeleteWires(Line l)
        {
            foreach (Wire w2 in wList)
            {
                if (w2.GetName() == l.Name)
                {
                    foreach (Dot d in dList)
                    {
                        if (w2.dotA == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                        }
                        if (w2.dotB == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                        }
                    }

                    foreach (Line l2 in w2.GetList())
                    {
                        canvas.Children.Remove(l2);
                    }
                    ec.RemoveConnectionCountFromSpecificElement(w2.elementA);
                    ec.RemoveConnectionCountFromSpecificElement(w2.elementB);

                    ec.EnableConnectionAvailability(w2.elementA);
                    ec.EnableConnectionAvailability(w2.elementB);

                    wList.Remove(w2);
                    break;
                }
            }
        }        
    }
}
