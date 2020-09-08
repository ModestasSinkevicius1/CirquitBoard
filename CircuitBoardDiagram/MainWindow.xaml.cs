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
        private ElementControl ec = new ElementControl();               

        private List<Wire> wList = new List<Wire>();
        private List<Dot> dList = new List<Dot>();
        private Wire w;

        private bool turn = false;       
        
        private string currentImageName;
       
        private int queue = 0;
        private int next = 0;
       
        private bool linePosition = false;

           

        private ImageGUIControl igc;
        private CanvasGUIControl cgc;
        private WireGUIControl wgc;
        private MessageGUIControl mgc;
        private ListImageGUIControl lgc = new ListImageGUIControl();
        private DotGUIControl dgc;
        private HighlighterGUIControl hgc;
        private MenuGUIControl mngc;

        private ListContainer lc = new ListContainer();

        public MainWindow()
        {            
            InitializeComponent();
            
            indicating_rectangle.Visibility = Visibility.Hidden;
            highlighting_rectangle.Visibility = Visibility.Hidden;

            mngc = new MenuGUIControl(canvas, canvasGrid, lc, menu);
            mgc = new MessageGUIControl(canvas, lc);
            hgc = new HighlighterGUIControl(canvas, canvasGrid, highlighting_rectangle, indicating_rectangle);
            wgc = new WireGUIControl(canvas, lc);
            dgc = new DotGUIControl(canvas, canvasGrid, wgc, lc);
            igc = new ImageGUIControl(this, canvas, canvasGrid, dgc, hgc, wgc, mgc, mngc, lc);
            cgc = new CanvasGUIControl(this, canvas, canvasGrid, lc, dock_bottom, highlighting_rectangle, indicating_rectangle, igc, dgc, lgc, hgc);            
            lgc.LoadImages(grid_expander);                       
        }   
        
        /*
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
        */                                                                                                                                          
    }
}
