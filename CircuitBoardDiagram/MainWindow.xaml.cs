using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
        private SaveLoadController slc = new SaveLoadController();
        
        private TextBlock tb = new TextBlock();

        private List<Wire> wList = new List<Wire>();
        private List<Dot> dList = new List<Dot>();
        private Wire w;

        private bool turn = false;


        private Polyline previousLine;
        private string previousElementName = "";
        private string previousDotName = "";
        
        private string currentImageName;
       
        private int queue = 0;
        private int next = 0;

        private bool isOnImage = false;

        private bool linePosition = false;

        double minX = 99999;
        double maxX = -99999;

        double minY = 99999;
        double maxY = -99999;        

        private ImageGUIControl igc;
        private CanvasGUIControl cgc = new CanvasGUIControl();
        private WireGUIControl wgc = new WireGUIControl();
        private MessageGUIControl mgc = new MessageGUIControl();
        private ListImageGUIControl lgc = new ListImageGUIControl();
        private DotGUIControl dgc;
        private HighlighterGUIControl hgc;        

        public MainWindow()
        {
            InitializeComponent();           

            indicating_rectangle.Visibility = Visibility.Hidden;
            highlighting_rectangle.Visibility = Visibility.Hidden;

            hgc = new HighlighterGUIControl(canvas, canvasGrid, highlighting_rectangle, indicating_rectangle);
            dgc = new DotGUIControl(canvas, canvasGrid, wgc);
            igc = new ImageGUIControl(this, canvas, canvasGrid, dgc, hgc, ec);
            lgc.LoadImages(grid_expander);            
            //mgc.LoadPopupMessage();
            //cgc.LoadGrids();
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

        private void canvas_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.E) && lgc.currentImageName != null)
            {               
                igc.CreateElement(lgc.currentImageName);
                dgc.CreateDot(igc.elementName, igc.ec, 4);
                lgc.AddImageToCommon(lgc.currentImageName, dock_bottom);                
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

            if (draggableControl.RenderTransform.Value.OffsetY < -1500f)
            {
                //MessageBox.Show("I have reached my limit");
                draggableControl.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, -1500f);
            }

            if (draggableControl.RenderTransform.Value.OffsetX < -4250f)
            {
                draggableControl.RenderTransform = new TranslateTransform(-4250f, draggableControl.RenderTransform.Value.OffsetY);
            }

            if (!isOnImage || !IsMouseCaptured)
                hgc.IndicateCell();
            else
            {
                indicating_rectangle.Visibility = Visibility.Hidden;
            }
        }        

        private void Polyline_mouseEnter(object sender, MouseEventArgs e)
        {
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();

            if(Keyboard.IsKeyDown(Key.X))
            {
                bc.Color = Colors.Red;
            }
            else
                bc.Color = Colors.Green;

            //wgc.ChangeLineStyle(pl, bc, 4);
        }

        private void Polyline_mouseEnter_1(object sender, MouseEventArgs e)
        {
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();
            
            bc.Color = Colors.Green;

            //wgc.ChangeLineStyle(pl, bc, 3);
        }

        private void Polyline_mouseLeave(object sender, MouseEventArgs e)
        {
            Polyline pl = sender as Polyline;
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            //wgc.ChangeLineStyle(pl, bc, 1);    
        }

        private void Polyline_mouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Polyline pl = sender as Polyline;
            
            if (Keyboard.IsKeyDown(Key.X))
            {
                //wgc.DeleteWires(pl);                         
            }
            else if(Keyboard.IsKeyDown(Key.C))
            {                              
                foreach (Wire w2 in wList)
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

        private void MenuItemOption_Click(object sender, RoutedEventArgs e)
        {
            OptionWindow opWindow = new OptionWindow(canvasGrid.ColumnDefinitions[0].Width.Value, canvasGrid.RowDefinitions[0].Height.Value);
            bool? result = opWindow.ShowDialog();

            if (opWindow.isPressedOk == true)
            {
                //cgc.ResetColumn();
                //cgc.ResetRow();

                //cgc.AddColumn(opWindow.slider.Value, 12 * 8);
                //cgc.AddRow(opWindow.slider_Copy.Value, 7 * 5);

                //hgc.UpdateIndicatorSize();
                //hgc.UpdateHighlightorSize();
               
                //elementBehaviour=opWindow.GetElementBehaviour();
            }
        }

        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
        {           
            Transform transform = canvas.LayoutTransform;
            Transform defaultRender = canvas.RenderTransform;           

            //igc.ResizeBasedElementsArangements();

            try
            {
                canvas.RenderTransform = new TranslateTransform(-minX + 50, -minY + 50);

                canvas.LayoutTransform = null;

                Size size = new Size(maxX - minX + 200, maxY - minY + 200);

                canvas.Measure(size);
                canvas.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(canvas);

                using (FileStream outStream = new FileStream("logo.png", FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    encoder.Save(outStream);
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message + "There are no elements found!");
            }
            canvas.LayoutTransform = transform;
            canvas.RenderTransform = defaultRender;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {            
            slc.WriteXML(ec);
            
            FileStream fs = File.Open("myProject.xaml", FileMode.Create);
            XamlWriter.Save(canvas, fs);
            fs.Close();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Image img;

            for(int i=0;i<canvas.Children.Count;i++)
            {
                if(canvas.Children[i].GetType()==typeof(Image))
                {
                    img = canvas.Children[i] as Image;
                    //igc.DeleteElement(img);
                    i--;
                }
            }
            dList.Clear();           

            ec = slc.ReadXML();            
            //igc.RecreateElementsFromSave();            
        }

        private void Textbox_MouseLeave(object sender, MouseEventArgs e)
        {            
            tb.Visibility = Visibility.Hidden;
        }                                                                                                                                    
    }
}
