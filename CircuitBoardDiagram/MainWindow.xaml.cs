using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
        }         

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Shape draggableControl = sender as Shape;
            originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
            isDragging = true;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();           
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Shape draggable = sender as Shape;
            draggable.ReleaseMouseCapture();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Shape draggableControl = sender as Shape;
            if(isDragging && draggableControl != null && e.GetPosition(this).X>canvas.Margin.Left && e.GetPosition(this).Y > canvas.Margin.Top)
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
            Rectangle r = new Rectangle();
            r.Height = 50;
            r.Width = 50;
            r.Fill = Brushes.Green;
            r.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
            r.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
            r.MouseMove += new MouseEventHandler(Canvas_MouseMove);

            canvas.Children.Add(r);
            Canvas.SetTop(r, Mouse.GetPosition(canvas).Y-r.Width/2);
            Canvas.SetLeft(r, Mouse.GetPosition(canvas).X-r.Height/2);
           

        }
    }
}
