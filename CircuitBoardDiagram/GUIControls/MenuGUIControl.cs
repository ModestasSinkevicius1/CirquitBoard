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

        private Canvas canvas;
        private Grid grid;
        private ListContainer lc;
        private Menu menu;

        private ItemCollection item;
        private MenuItem item1;

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
           object name = menu.FindName("Option");
            MessageBox.Show(name.ToString());
           //item1.Click += new RoutedEventHandler(MenuItemOption_Click);
        }

        private void MenuItemOption_Click(object sender, RoutedEventArgs e)
        {
            OptionWindow opWindow = new OptionWindow(grid.ColumnDefinitions[0].Width.Value, grid.RowDefinitions[0].Height.Value);
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
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message + "There are no elements found!");
            }
            canvas.LayoutTransform = transform;
            canvas.RenderTransform = defaultRender;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            slc.WriteXML(lc.ec);

            FileStream fs = File.Open("myProject.xaml", FileMode.Create);
            XamlWriter.Save(canvas, fs);
            fs.Close();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Image img;

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Image))
                {
                    img = canvas.Children[i] as Image;
                    //igc.DeleteElement(img);
                    i--;
                }
            }
            lc.dList.Clear();

            lc.ec = slc.ReadXML();
            //igc.RecreateElementsFromSave();            
        }
    }
}
