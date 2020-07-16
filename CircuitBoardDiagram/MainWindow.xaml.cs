using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
        private string previousElementName="";
        private string previousConnectedElementName = "";
        
        private string currentImageName;
       
        private int queue = 0;
        private int next = 0;

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
                queue++;
            }

        }
        
        private void Line_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Keyboard.IsKeyDown(Key.X))
            {
                Line l = sender as Line;

                foreach (Wire w2 in wList)
                {
                    if (w2.GetName() == l.Name)
                    {
                        ec.RemoveConnectionCountFromSpecificElement(w2.elementA);
                        ec.RemoveConnectionCountFromSpecificElement(w2.elementB);
                        wList.Remove(w2);
                        break;
                    }
                }
                canvas.Children.Remove(l);
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {
                Line l = sender as Line;
                
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

        private void SnapToClosestCell(Image draggableControl)
        {
            indicating_rectangle.Visibility = Visibility.Visible;

            double distanceX = 0;
            double distanceY = 0;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
            {
                distanceX = (Math.Abs(Mouse.GetPosition(canvas).X - (column.Width.Value * i)))-column.Width.Value/2;

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
                distanceY = (Math.Abs(Mouse.GetPosition(canvas).Y - (row.Height.Value * i)))-row.Height.Value/2;

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
          
            draggableControl.RenderTransform = new TranslateTransform(cellWidth * cellX,cellHeight*cellY);                    
            
            Grid.SetRow(draggableControl, 0);
            Grid.SetColumn(draggableControl, 0);
            

        }

        private void IndicateCell(Shape draggableControl)
        {
            indicating_rectangle.Visibility = Visibility.Visible;

            double distanceX = 0;
            double distanceY = 0;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
            {
                distanceX = Math.Abs(Mouse.GetPosition(canvas).X - (column.Width.Value * i));

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
                distanceY = Math.Abs(Mouse.GetPosition(canvas).Y - (row.Height.Value * i));

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

            canvas.Children.Add(l);

            return l;
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

        private bool isOutOfBounds(MouseEventArgs e)
        {
            Point cursorP = e.GetPosition(this);
            if(cursorP.X<canvas.Margin.Left || cursorP.Y<canvas.Margin.Top || cursorP.X>canvas.Width+canvas.Margin.Left || cursorP.Y > canvas.Height + canvas.Margin.Top)
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

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            currentImageName = img.Tag.ToString();

        }

        private void canvas_MouseMove_1(object sender, MouseEventArgs e)
        {          
            IndicateCell(indicating_rectangle);          
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            indicating_rectangle.Visibility = Visibility.Visible;
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image draggableControl = sender as Image;
            Highlight_cell(draggableControl);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            highlighting_rectangle.Visibility = Visibility.Hidden;
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            indicating_rectangle.Visibility = Visibility.Hidden;
        }
    }
}
