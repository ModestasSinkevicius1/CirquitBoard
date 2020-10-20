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


namespace CircuitBoardDiagram.GUIControls
{
    class MenuGUIControl
    {
        private SaveLoadController slc = new SaveLoadController();

        private double minX = 99999;
        private double maxX = -99999;

        private double minY = 99999;
        private double maxY = -99999;

        public string elementBehaviour { get; set; } = "alwaysGrid";

        private Canvas canvas;
        private Grid grid;
        public CanvasGUIControl cgc { get; set; }
        public HighlighterGUIControl hgc { get; set; }
        public ImageGUIControl igc { get; set; }
        public WireGUIControl wgc { get; set; }
        public ConnectorGUIControl cogc { get; set; }

        private ListContainer lc;
        private Menu menu;

        private MenuItem baseItem;
        private MenuItem[] item = new MenuItem[4];

        public MenuGUIControl(Canvas canvas, Grid grid, ListContainer lc, Menu menu)
        {
            this.canvas = canvas;
            this.grid = grid;           
            this.lc = lc;
            this.menu = menu;

            LoadEvents();
        }

        private void LoadEvents()
        {
            baseItem = menu.FindName("File") as MenuItem;

            item[0] = baseItem.FindName("File_New") as MenuItem;
            item[1] = baseItem.FindName("File_Open") as MenuItem;
            item[2] = baseItem.FindName("File_Save") as MenuItem;
            item[3] = baseItem.FindName("File_Export") as MenuItem;

            item[0].Click += new RoutedEventHandler(MenuItemNew_Click);
            item[1].Click += new RoutedEventHandler(MenuItemOpen_Click);
            item[2].Click += new RoutedEventHandler(MenuItemSave_Click);
            item[3].Click += new RoutedEventHandler(MenuItemExport_Click);

            //item = menu.Items[0];
            baseItem = menu.FindName("Tools") as MenuItem;

            item[0] = baseItem.FindName("Tools_Options") as MenuItem;                                    
            item[0].Click += new RoutedEventHandler(MenuItemOption_Click);
        }
        private void MenuItemOption_Click(object sender, RoutedEventArgs e)
        {
            OptionWindow opWindow = new OptionWindow(grid.ColumnDefinitions[0].Width.Value, grid.RowDefinitions[0].Height.Value, elementBehaviour);
            bool? result = opWindow.ShowDialog();

            if (opWindow.isPressedOk == true)
            {
                cgc.ResetColumn();
                cgc.ResetRow();

                cgc.AddColumn(opWindow.slider.Value, 12 * 8);
                cgc.AddRow(opWindow.slider_Copy.Value, 7 * 5);

                hgc.UpdateIndicatorSize();
                hgc.UpdateHighlightorSize();

                elementBehaviour=opWindow.GetElementBehaviour();
            }
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItemExport_Click(object sender, RoutedEventArgs e)
        {
            Transform transform = canvas.LayoutTransform;
            Transform defaultRender = canvas.RenderTransform;

            ResizeBasedElementsArangements();

            string path = "default.png";
            bool isCanceled = false;

            using (System.Windows.Forms.SaveFileDialog fbd = new System.Windows.Forms.SaveFileDialog())
            {
                fbd.FileName = "project";
                fbd.Filter = "PNG format (*.png)|*.png|JPG format (*.jpg)|*.jpg";
                fbd.AddExtension = true;
                fbd.DefaultExt = "png";               
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();                                

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    path = fbd.FileName;
                }

                if(result == System.Windows.Forms.DialogResult.Cancel)
                {
                    isCanceled = true;
                }
            }
            if (!isCanceled)
            {
                try
                {
                    canvas.RenderTransform = new TranslateTransform(-minX + 50, -minY + 50);

                    canvas.LayoutTransform = null;

                    Size size = new Size(maxX - minX + 200, maxY - minY + 200);

                    canvas.Measure(size);
                    canvas.Arrange(new Rect(size));

                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
                    renderBitmap.Render(canvas);

                    using (FileStream outStream = new FileStream(path, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(outStream);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message + "There are no elements found!");
                }
                canvas.LayoutTransform = transform;
                canvas.RenderTransform = defaultRender;
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {            
            slc.WriteXML(lc);

            /*
            FileStream fs = File.Open("myProject.xaml", FileMode.Create);
            XamlWriter.Save(canvas, fs);
            fs.Close();
            */
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {              
            Image img;

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Image))
                {
                    img = canvas.Children[i] as Image;
                    igc.DeleteElement(img);
                    i--;
                }
            }

            lc.dList.Clear();
                   
            lc = slc.ReadXML();

            foreach (SpecificElement se in lc.ec.GetAllElements())
            {               
                se.ClearDotList();
            }
                      
            igc.RecreateElementsFromSave(lc);                
            cogc.RecreateElementsFromSave(lc);           

            wgc.UpdateListContainer(lc);
            wgc.RecreateWires();


            foreach (SpecificElement se in lc.ec.GetAllElements())
            {
                wgc.FindWireConnectedDots(se.GetName());
            }           
        }
        
        public void ResizeBasedElementsArangements()
        {
            maxX = -99999;
            minX = 99999;

            maxY = -99999;
            minY = 99999;


            foreach (SpecificElement se in lc.ec.GetAllElements())
            {
                if (minX > se.GetElement().RenderTransform.Value.OffsetX)
                {
                    minX = se.GetElement().RenderTransform.Value.OffsetX;
                }

                if (maxX < se.GetElement().RenderTransform.Value.OffsetX)
                {
                    maxX = se.GetElement().RenderTransform.Value.OffsetX;
                }

                if (minY > se.GetElement().RenderTransform.Value.OffsetY)
                {
                    minY = se.GetElement().RenderTransform.Value.OffsetY;
                }

                if (maxY < se.GetElement().RenderTransform.Value.OffsetY)
                {
                    maxY = se.GetElement().RenderTransform.Value.OffsetY;
                }
            }
        }
    }
}
