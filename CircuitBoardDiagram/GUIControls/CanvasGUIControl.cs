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
    class CanvasGUIControl
    {
        protected bool isDragging;
        private bool isOnImage = false;

        private Point clickPosition;

        private TranslateTransform originTT;

        private MainWindow form;
        private Canvas canvas;
        private Grid grid;
        private ListContainer lc;
        private DockPanel dock_bottom;
        private ImageGUIControl igc;
        private DotGUIControl dgc;
        private ListImageGUIControl lgc;
        private HighlighterGUIControl hgc;
        private ShortcutGUIControl sgc;
        public MessageGUIControl mgc { get; set; }

        private Rectangle highlighter;
        private Rectangle indicator;

        private Grid hRulerGrid;
        private UniformGrid uHRulerGrid;

        private Grid vRulerGrid;
        private UniformGrid uVRulerGrid;

        private string gridState = "empty";

        public CanvasGUIControl(MainWindow form, Canvas canvas, Grid grid, ListContainer lc, DockPanel dock_bottom, Rectangle highlighter, Rectangle indicator, Grid hRulerGrid, UniformGrid uHRulerGrid, Grid vRulerGrid, UniformGrid uVRulerGrid, ImageGUIControl igc, DotGUIControl dgc, ListImageGUIControl lgc, HighlighterGUIControl hgc, ShortcutGUIControl sgc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.lc = lc;
            this.dock_bottom = dock_bottom;
            this.highlighter = highlighter;
            this.indicator = indicator;

            this.hRulerGrid = hRulerGrid;
            this.uHRulerGrid = uHRulerGrid;

            this.vRulerGrid = vRulerGrid;
            this.uVRulerGrid = uVRulerGrid;

            this.igc = igc;
            this.dgc = dgc;
            this.lgc = lgc;
            this.hgc = hgc;
            this.sgc = sgc;

            LoadEvents();
            LoadGrids();

            CreateGridLines();

            SetPositionStart();

            CreateRuler();
        }

        private void CreateRuler()
        {
            /*Grid rulerGrid = new Grid();

            rulerGrid.Width = 500;
            rulerGrid.Height = 25;
            rulerGrid.Background = Brushes.Khaki;

            TickBar majorTick = new TickBar();
            majorTick.Minimum = 0;
            majorTick.Maximum = 50;
            majorTick.TickFrequency = 5;
            //majorTick.Placement = TickBarPlacement.Top;
            majorTick.Fill = Brushes.Black;
            //majorTick.VerticalAlignment = VerticalAlignment.Bottom;
            majorTick.Height = 10;

            TickBar minTick = new TickBar();
            minTick.Minimum = 0;
            minTick.Maximum = 50;
            minTick.TickFrequency = 1;
           //minTick.Placement = TickBarPlacement.Top;
            minTick.Fill = Brushes.Black;
            //minTick.VerticalAlignment = VerticalAlignment.Bottom;
            minTick.Height = 6;

            UniformGrid uGrid = new UniformGrid();
            uGrid.Rows = 1;
            uGrid.Width = 400;
            //uGrid.VerticalAlignment = VerticalAlignment.Top;

            for(int i=0;i<5;i++)
            {
                TextBlock tbRuler = new TextBlock();
                tbRuler.Text = Convert.ToString(10 * i);
                tbRuler.FontSize = 10;
                //tbRuler.HorizontalAlignment = HorizontalAlignment.Center;

                uGrid.Children.Add(tbRuler);
            }

            rulerGrid.Children.Add(majorTick);
            rulerGrid.Children.Add(minTick);

            rulerGrid.Children.Add(uGrid);

            canvas.Children.Add(rulerGrid);
            */

            //hRulerGrid.Width = 4000;
            //uHRulerGrid.Width = 4000;

            AddTickNumbersHorizontalRuler();
            AddTickNumbersVerticalRuler();
            

            hRulerGrid.RenderTransform = new TranslateTransform(canvas.RenderTransform.Value.OffsetX, hRulerGrid.RenderTransform.Value.OffsetY);
            vRulerGrid.RenderTransform = new TranslateTransform(vRulerGrid.RenderTransform.Value.OffsetX, canvas.RenderTransform.Value.OffsetY);
            //alignRulerToTop(rulerGrid);
        }

        private void AddTickNumbersHorizontalRuler()
        {
            int n = 500;

            for (int i = 0; i < 47; i++)
            {
                TextBlock hTb = new TextBlock();
                n = n - 10;
                hTb.Text = Convert.ToString(n * -1);
                hTb.FontSize = 10;
                hTb.HorizontalAlignment = HorizontalAlignment.Center;
                uHRulerGrid.Children.Add(hTb);
            }

            for (int i = 0; i < 49; i++)
            {
                TextBlock hTb = new TextBlock();
                hTb.Text = Convert.ToString(10 * i);
                hTb.FontSize = 10;
                hTb.HorizontalAlignment = HorizontalAlignment.Center;
                uHRulerGrid.Children.Add(hTb);
            }
        }

        private void AddTickNumbersVerticalRuler()
        {
            int n = 120;

            for (int i = 0; i < 5*3; i++)
            {
                TextBlock vTb = new TextBlock();
                n = n - 10;
                vTb.Text = Convert.ToString(n * -1);
                vTb.FontSize = 10;
                vTb.VerticalAlignment = VerticalAlignment.Center;
                uVRulerGrid.Children.Add(vTb);
            }           
            for (int i = 0; i < 5*2; i++)
            {
                TextBlock vTb = new TextBlock();
                vTb.Text = Convert.ToString(10 * i);
                vTb.FontSize = 10;
                vTb.VerticalAlignment = VerticalAlignment.Center;
                uVRulerGrid.Children.Add(vTb);
            }
        }

        private void alignRulerToTop(Grid rulerGrid)
        {
            Canvas.SetTop(rulerGrid, 50);
        }

        public void CreateGridLines()
        {
            if (gridState != "full")
            {
                for (int j = 0; j < grid.RowDefinitions.Count; j++)
                {
                    for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
                    {
                        Border bord = new Border();
                        bord.BorderBrush = Brushes.Black;
                        bord.BorderThickness = new Thickness(1);
                        bord.Opacity = 0.2;
                        bord.Width = 50;
                        bord.Height = 50;
                        canvas.Children.Add(bord);
                        Canvas.SetTop(bord, j * 50);
                        Canvas.SetLeft(bord, i * 50);
                    }
                }
                gridState = "full";
            }
        }

        public void RemoveGridLines()
        {
            if (gridState != "empty")
            {
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    UIElement b = canvas.Children[i] as UIElement;

                    if (b.GetType() == typeof(Border))
                    {
                        canvas.Children.Remove(b);
                        i--;
                    }
                }
                gridState = "empty";
            }
        }

        public void SetPositionStart()
        {
            canvas.RenderTransform = new TranslateTransform(-4250f / 2, -1500f / 2);
        }

        public void LoadEvents()
        {
            canvas.MouseLeftButtonDown += new MouseButtonEventHandler(canvas_MouseLeftButtonDown);
            canvas.MouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
            canvas.MouseLeave += new MouseEventHandler(canvas_MouseLeave);
            canvas.MouseEnter += new MouseEventHandler(canvas_MouseEnter);
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.E) && lgc.currentImageName != null)
            {
                igc.CreateElement(lgc.currentImageName);
                dgc.CreateDot(igc.elementName, 4);
                lgc.AddImageToCommon(lgc.currentImageName, dock_bottom);
                igc.UpadateImage();
            }
            else if (Keyboard.IsKeyDown(Key.E))
            {
                mgc.ShowWarningMessage(null, "Select element first");
            }

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Canvas draggableControl = sender as Canvas;
                highlighter.Visibility = Visibility.Hidden;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(form);
                draggableControl.CaptureMouse();
            }

        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Canvas draggable = sender as Canvas;
            draggable.ReleaseMouseCapture();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas draggableControl = sender as Canvas;

            if (isDragging)
            {
                Point currentPosition = e.GetPosition(form);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);

                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);

                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);

                hRulerGrid.RenderTransform = new TranslateTransform(transform.X, hRulerGrid.RenderTransform.Value.OffsetY);
                vRulerGrid.RenderTransform = new TranslateTransform(vRulerGrid.RenderTransform.Value.OffsetX, transform.Y);
            }

            if (draggableControl.RenderTransform.Value.OffsetY > 50.0f)
            {
                draggableControl.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, 50.0f);
                vRulerGrid.RenderTransform = new TranslateTransform(vRulerGrid.RenderTransform.Value.OffsetX, 50.0f);
            }

            if (draggableControl.RenderTransform.Value.OffsetX > 50.0f)
            {
                draggableControl.RenderTransform = new TranslateTransform(50.0f, draggableControl.RenderTransform.Value.OffsetY);
                hRulerGrid.RenderTransform = new TranslateTransform(50.0f, hRulerGrid.RenderTransform.Value.OffsetY);
            }

            if (draggableControl.RenderTransform.Value.OffsetY < -1500f)
            {
                //MessageBox.Show("I have reached my limit");
                draggableControl.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, -1500f);
                vRulerGrid.RenderTransform = new TranslateTransform(vRulerGrid.RenderTransform.Value.OffsetX, -1500f);
            }

            if (draggableControl.RenderTransform.Value.OffsetX < -4250f)
            {
                draggableControl.RenderTransform = new TranslateTransform(-4250f, draggableControl.RenderTransform.Value.OffsetY);
                hRulerGrid.RenderTransform = new TranslateTransform(-4250f, hRulerGrid.RenderTransform.Value.OffsetY);
            }

            if (!isOnImage || !form.IsMouseCaptured)
                hgc.IndicateCell();
            else
            {
                indicator.Visibility = Visibility.Hidden;
            }
        }
        private void Image_MouseEnter_2(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            foreach (Dot d in lc.dList)
            {
                if (d.GetName() == img.Tag.ToString())
                {
                    foreach (Dot d2 in lc.ec.GetDots(d.GetCore()))
                    {
                        d2.GetDot().Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            sgc.UpdateText("");
            indicator.Visibility = Visibility.Hidden;
        }

        private void canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            sgc.UpdateText("OnCanvas");
        }

        /*
        public bool isOutOfBounds(MouseEventArgs e)
        {
            Point cursorP = e.GetPosition(form);

            double gridX = 50 - grid.ColumnDefinitions[0].Width.Value;
            double gridY = 50 - grid.RowDefinitions[0].Height.Value;

            if (cursorP.X < canvas.Margin.Left || cursorP.Y < canvas.Margin.Top || cursorP.X > (canvas.Width - gridX) + canvas.Margin.Left || cursorP.Y > (canvas.Height - gridY) + canvas.Margin.Top)
            {
                return false;
            }
            return true;
        }
        */

        public void ResetColumn()
        {
            int count = grid.ColumnDefinitions.Count;
            int i = 0;

            while (count > 0)
            {
                grid.ColumnDefinitions.RemoveAt(0);
                count = grid.ColumnDefinitions.Count;
                i++;
            }
        }

        public void ResetRow()
        {
            int count = grid.RowDefinitions.Count;
            int i = 0;

            while (count > 0)
            {
                grid.RowDefinitions.RemoveAt(0);
                count = grid.RowDefinitions.Count;
                i++;
            }
        }

        public void AddColumn(double value, int count)
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
                grid.ColumnDefinitions.Add(c);                

                i++;
            }

            foreach (ColumnDefinition column in grid.ColumnDefinitions)
            {
                column.Width = new GridLength(value);                
            }           
        }

        public void AddRow(double value, int count)
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
                grid.RowDefinitions.Add(r);

                i++;
            }

            foreach (RowDefinition row in grid.RowDefinitions)
            {
                row.Height = new GridLength(value);
            }
        }
        
        public void LoadGrids()
        {
            grid.Width = 600 * 8;
            grid.Height = 350 * 5;

            ResetColumn();
            ResetRow();

            AddColumn(50, 12 * 8);
            AddRow(50, 7 * 5);            
        }        
    }
}
