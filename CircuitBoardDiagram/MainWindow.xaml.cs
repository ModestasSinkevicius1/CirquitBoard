using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
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
        protected bool isDragging;
        private Point clickPosition;
        private TranslateTransform originTT;
        private ElementControl ec = new ElementControl();

        private List<Wire> wList = new List<Wire>();
        private Wire w;

        private bool turn = false;


        private Line previousLine;
        private string previousElementName = "";
        private string previousConnectedElementName = "";
        
        private string currentImageName;
       
        private int queue = 0;
        private int next = 0;

        private bool isOnImage = false;        

        public MainWindow()
        {
            InitializeComponent();

            indicating_rectangle.Visibility = Visibility.Hidden;
            highlighting_rectangle.Visibility = Visibility.Hidden;           

            LoadImages();
        }         

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {          
            Image draggableControl = sender as Image;           

            if (!Keyboard.IsKeyDown(Key.W)&&!Keyboard.IsKeyDown(Key.C)&&!Keyboard.IsKeyDown(Key.X))
            {
                highlighting_rectangle.Visibility = Visibility.Hidden;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(this);               
                draggableControl.CaptureMouse();                
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                ec.RemoveElementFromList(draggableControl.Tag.ToString());
                canvas.Children.Remove(draggableControl);
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {
                bool status = ec.GetConnectionAvailability(draggableControl.Tag.ToString());
                int count = ec.GetConnectionCount(draggableControl.Tag.ToString());

                MessageBox.Show(status.ToString() + " and " +count);
            }
            else
                DrawWireBetweenElements(draggableControl);

        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {                        
            isDragging = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();            
            SnapToClosestCell(draggable);
            //UpdateLineLocation(draggable);
            IndicateCell(highlighting_rectangle);           
            indicating_rectangle.Visibility = Visibility.Hidden;            
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Image draggableControl = sender as Image;            
                                              
            if (isDragging && draggableControl != null && isOutOfBounds(e))
            {          
                Point currentPosition = e.GetPosition(this);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                
                SnapToClosestCell(draggableControl);
                //UpdateLineLocation(draggableControl);
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

                queue++;
            }

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

            originTT = l.RenderTransform as TranslateTransform ?? new TranslateTransform();
            isDragging = true;
            clickPosition = e.GetPosition(this);
            l.CaptureMouse();

            if (Keyboard.IsKeyDown(Key.X))
            {               
                foreach (Wire w2 in wList)
                {
                    if (w2.GetName() == l.Name)
                    {
                        foreach(Line l2 in w2.GetList())
                        {
                            canvas.Children.Remove(l2);
                        }
                        ec.RemoveConnectionCountFromSpecificElement(w2.elementA);
                        ec.RemoveConnectionCountFromSpecificElement(w2.elementB);
                        wList.Remove(w2);                       
                        break;
                    }
                }             
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
            isDragging = false;
            Line draggable = sender as Line;
            draggable.ReleaseMouseCapture();            
            //UpdateLineLocation2(draggable);           
        }


        private void Line_mouseMove(object sender, MouseEventArgs e)
        {
            Line draggableControl = sender as Line;

            if (isDragging && draggableControl != null && isOutOfBounds(e))
            {
                Point currentPosition = e.GetPosition(this);
                
                foreach(Wire w in wList)
                {
                    if(draggableControl.Name==w.GetName())
                    {
                        foreach(Line l in w.GetList())
                        {
                            if(l != draggableControl)
                            {
                                if(l.X1==draggableControl.X2)
                                {                                   
                                    draggableControl.Y1 = e.GetPosition(canvas).Y;
                                }
                                else
                                    draggableControl.X1 = e.GetPosition(canvas).X;
                            }
                        }
                    }
                }
                if (draggableControl.X1 != draggableControl.X2)
                {
                    draggableControl.X2 = e.GetPosition(canvas).X;
                }
                
                /*TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);*/

                //UpdateLineLocation2(draggableControl);
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            currentImageName = img.Tag.ToString();
        }

        private void canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
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

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            isOnImage = true;
            Image draggableControl = sender as Image;
            Highlight_cell(draggableControl);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            isOnImage = false;
            highlighting_rectangle.Visibility = Visibility.Hidden;
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

                AddColumn(opWindow.slider.Value);
                AddRow(opWindow.slider_Copy.Value);

                UpdateIndicatorSize();
                UpdateHighlightorSize();
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
            foreach (Wire w2 in wList)
            {
                if (w2.GetName() == l.Name)
                {
                    foreach (Line l2 in w2.GetList())
                    {
                        l2.Stroke = bc;
                        l2.StrokeThickness = thickness;
                    }
                    break;
                }
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

        private void DrawWireBetweenElements(Image draggableControl)
        {
            if (!turn && previousElementName!=draggableControl.Tag.ToString())
            {
                Line l = CreateLine();
                l.Name = "line" + draggableControl.Tag.ToString();

                ec.AddConnectionCountToSpecificElement(draggableControl.Tag.ToString());              

                l.X1 = draggableControl.RenderTransform.Value.OffsetX+draggableControl.Width/2;
                l.Y1 = draggableControl.RenderTransform.Value.OffsetY+draggableControl.Height/2;

                l.X2 = draggableControl.RenderTransform.Value.OffsetX+draggableControl.Width/2;
                l.Y2 = draggableControl.RenderTransform.Value.OffsetY+draggableControl.Height/2;                                 

                previousElementName = draggableControl.Tag.ToString();
                previousLine = l;

                turn = true;
            }
            else if (draggableControl.Tag.ToString() != previousElementName)
            {
                Line l = CreateLine();               

                ec.AddConnectionCountToSpecificElement(draggableControl.Tag.ToString());                

                previousLine.X2 = draggableControl.RenderTransform.Value.OffsetX+draggableControl.Width/2;                            

                l.X1 = previousLine.X2;
                l.Y1 = previousLine.Y2;

                l.X2 = previousLine.X2;
                l.Y2 = draggableControl.RenderTransform.Value.OffsetY + draggableControl.Height / 2;

                previousLine.Name += draggableControl.Tag.ToString();
                l.Name = previousLine.Name;

                w = new Wire(previousLine.Name);
                w.elementA = previousElementName;
                w.elementB = draggableControl.Tag.ToString();
                w.AddList(previousLine);
                w.AddList(l);
                wList.Add(w);

                previousElementName = draggableControl.Tag.ToString();                           

                turn = false;
            }           
            ec.EnableConnectionAvailability(draggableControl.Tag.ToString());
        }
        
        private Line CreateLine()
        {
            Line l = new Line();
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            l.StrokeThickness = 2;
            l.Stroke = bc;

            l.MouseLeftButtonDown += new MouseButtonEventHandler(Line_mouseLeftButtonDown);
            l.MouseEnter += new MouseEventHandler(Line_mouseEnter);
            l.MouseLeave += new MouseEventHandler(Line_mouseLeave);
            l.MouseMove += new MouseEventHandler(Line_mouseMove);
            l.MouseLeftButtonUp += new MouseButtonEventHandler(Line_mouseLeftButtonUp);

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
                            if (w.elementA == draggableControl.Tag.ToString())
                            {                               
                                foreach(Line l2 in w.GetList())
                                {
                                    if(l != l2)
                                    {                                       
                                        l2.X2 = l.X1;
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

            foreach(Wire w2 in wList)
            {               
                if (w2.elementA == draggableControl.Tag.ToString())
                {
                    foreach (Line l in w2.GetList())
                    {
                        if (i <= 0)
                        {
                            l.X1 = draggableControl.RenderTransform.Value.OffsetX + draggableControl.Width / 2;
                            l.Y1 = draggableControl.RenderTransform.Value.OffsetY + draggableControl.Height / 2;

                            l.Y2 = draggableControl.RenderTransform.Value.OffsetY + draggableControl.Height / 2;
                        }
                        else
                        {
                            l.Y1 = draggableControl.RenderTransform.Value.OffsetY + draggableControl.Height / 2;
                        }
                        i++;
                    }
                    i = 0;
                }              
                if(w2.elementB == draggableControl.Tag.ToString())
                {
                    for(int j=w2.GetList().Count-1;j>=0;j--)
                    {
                        if (i <= 0)
                        {                           
                            w2.GetList()[j].X2 = draggableControl.RenderTransform.Value.OffsetX + draggableControl.Width / 2;
                            w2.GetList()[j].Y2 = draggableControl.RenderTransform.Value.OffsetY + draggableControl.Height / 2;

                            w2.GetList()[j].X1 = draggableControl.RenderTransform.Value.OffsetX + draggableControl.Width / 2;
                        }
                        else
                        {                           
                            w2.GetList()[j].X2 = draggableControl.RenderTransform.Value.OffsetX + draggableControl.Width / 2;
                        }
                        i++;
                    }
                    i = 0;
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

        private void AddColumn(double value)
        {
            double originalValue = 50;

            int i = 0;

            int newCount = 12;

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

        private void AddRow(double value)
        {
            double originalValue = 50;           

            int i = 0;
            
            int newCount = 7;

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
    }
}
