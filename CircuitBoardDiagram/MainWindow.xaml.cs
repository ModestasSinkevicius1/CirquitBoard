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
        private ImageGUIControl igc;
        private CanvasGUIControl cgc;
        private WireGUIControl wgc;
        private MessageGUIControl mgc;
        private ListImageGUIControl lgc = new ListImageGUIControl();
        private DotGUIControl dgc;
        private HighlighterGUIControl hgc;
        private MenuGUIControl mngc;
        private ConnectorGUIControl cogc;

        private CircuitChecker cc;

        private ListContainer lc = new ListContainer();

        public MainWindow()
        {            
            InitializeComponent();
            
            indicating_rectangle.Visibility = Visibility.Hidden;
            highlighting_rectangle.Visibility = Visibility.Hidden;

            mngc = new MenuGUIControl(canvas, canvasGrid, lc, menu);
            mgc = new MessageGUIControl(canvas, lc);
            hgc = new HighlighterGUIControl(canvas, canvasGrid, highlighting_rectangle, indicating_rectangle);
            wgc = new WireGUIControl(this, canvas, canvasGrid, mgc, lc);
            dgc = new DotGUIControl(this, canvas, canvasGrid, wgc, lc);
            igc = new ImageGUIControl(this, canvas, canvasGrid, dgc, hgc, wgc, mgc, mngc, lc);
            cgc = new CanvasGUIControl(this, canvas, canvasGrid, lc, dock_bottom, highlighting_rectangle, indicating_rectangle, igc, dgc, lgc, hgc);
            cogc = new ConnectorGUIControl(this, canvas, canvasGrid, mgc, wgc, mngc, lc);
            
            mngc.cgc = cgc;
            mngc.hgc = hgc;
            mngc.igc = igc;
            mngc.wgc = wgc;
            mngc.cogc = cogc;

            wgc.cogc = cogc;

            cc = new CircuitChecker(lc);

            lgc.LoadImages(grid_expander);                       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(SpecificElement se in lc.ec.GetAllElements())
            {
                if(RemoveNumbers(se.GetName()) == "AC")
                {
                    cc.CheckCircuit(se);
                    break;
                }
            }
            
            foreach(SpecificElement se in lc.ec.GetAllElements())
            {
                se.visited = false;
                cc.RemoveDotVisited(se);
            }
            if(cc.circuitFull)
                MessageBox.Show("Circuit is complete!");
            else
                MessageBox.Show("Circuit is not complete!");
            cc.circuitFull = false;
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

        /*private void BeginHide()
        {
            Thread th = new Thread(UpdateCurrentDotVisibilityStatus);
            th.IsBackground = true;
            th.Start();
        }
        private void UpdateCurrentDotVisibilityStatus()
        {
            double maxWait = 10;
            t1 = new System.Timers.Timer();
            t1.Interval = 1000;

            while (true)
            {
                Thread.Sleep(50);
                form.Dispatcher.BeginInvoke(new Action(() =>
                {

                }));
            }
        }
        */
    }
}
