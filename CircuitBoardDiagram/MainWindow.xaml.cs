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
        private bool turn = false;

        private Line previousLine;
        private string previousElementName="";
        private string previousConnectedElementName = "";
        
        private string currentImageName;
       
        private int queue=0;
        public MainWindow()
        {
            InitializeComponent();

            indicating_rectangle.Visibility = Visibility.Hidden;
            LoadImages();
        }         

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {          
            Image draggableControl = sender as Image;

            if (!Keyboard.IsKeyDown(Key.W)&&!Keyboard.IsKeyDown(Key.C))
            {
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(this);
                draggableControl.CaptureMouse();
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {
                bool status = ec.GetConnectionAvailability(draggableControl.Tag.ToString());
                int count = ec.GetConnectionCount(draggableControl.Tag.ToString());

                MessageBox.Show(status.ToString() + " and " +count);
            }
            else
                DrawLineBetweenElements(draggableControl);

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
                r.Tag = currentImageName + queue;                
                ec.AddElementToList(r.Tag.ToString());

                r.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
                r.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
                r.MouseMove += new MouseEventHandler(Canvas_MouseMove);

                canvas.Children.Add(r);
                Canvas.SetTop(r, Mouse.GetPosition(canvas).Y - r.Width / 2);
                Canvas.SetLeft(r, Mouse.GetPosition(canvas).X - r.Height / 2);               
                queue++;
            }

        }
        
        private void Line_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Line l = sender as Line;
            MessageBox.Show(l.Name);
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

        private void DrawLineBetweenElements(Image draggableControl)
        {
            if (!turn && previousElementName!=draggableControl.Tag.ToString())
            {
                Line l = new Line();
                SolidColorBrush bc = new SolidColorBrush();
                bc.Color = Colors.Black;

                ec.AddConnectionCountToSpecificElement(draggableControl.Tag.ToString());              

                l.X1 = draggableControl.RenderTransform.Value.OffsetX;
                l.Y1 = draggableControl.RenderTransform.Value.OffsetY;

                l.X2 = draggableControl.RenderTransform.Value.OffsetX;
                l.Y2 = draggableControl.RenderTransform.Value.OffsetY;

                l.StrokeThickness = 2;
                l.Stroke = bc;
                l.Name = "line" + draggableControl.Tag.ToString();

                l.MouseLeftButtonDown += new MouseButtonEventHandler(Line_mouseLeftButtonDown);

                canvas.Children.Add(l);

                previousElementName = draggableControl.Tag.ToString();
                previousLine = l;

                turn = true;
            }
            else if (draggableControl.Tag.ToString() != previousElementName)
            {
                ec.AddConnectionCountToSpecificElement(draggableControl.Tag.ToString());                

                previousLine.X2 = draggableControl.RenderTransform.Value.OffsetX;
                previousLine.Y2 = draggableControl.RenderTransform.Value.OffsetY;

                previousLine.Name += draggableControl.Tag.ToString();

                previousElementName = draggableControl.Tag.ToString();
                
                turn = false;
            }           
            ec.EnableConnectionAvailability(draggableControl.Tag.ToString());
        }       

        private void LoadImages()
        {
            DatabaseControl dc = new DatabaseControl();
            Image img;
            int i = 0;
            foreach(DatabaseElement e in dc.GetElements())
            {
                img = dock_bottom.Children[i] as Image;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+"Circuit element images/" + e.GetElementType() + ".png"));
                img.Tag = e.GetElementType();
                i++;
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

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            indicating_rectangle.Visibility = Visibility.Hidden;
        }
    }
}
