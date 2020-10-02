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

        private Rectangle highlighter;
        private Rectangle indicator;

        public CanvasGUIControl(MainWindow form, Canvas canvas, Grid grid, ListContainer lc, DockPanel dock_bottom, Rectangle highlighter, Rectangle indicator, ImageGUIControl igc, DotGUIControl dgc, ListImageGUIControl lgc, HighlighterGUIControl hgc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.lc = lc;
            this.dock_bottom = dock_bottom;
            this.highlighter = highlighter;
            this.indicator = indicator;
            this.igc = igc;
            this.dgc = dgc;
            this.lgc = lgc;
            this.hgc = hgc;

            LoadEvents();
            LoadGrids();
        }

        public void LoadEvents()
        {
            canvas.MouseLeftButtonDown += new MouseButtonEventHandler(canvas_MouseLeftButtonDown);
            canvas.MouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
            canvas.MouseMove += new MouseEventHandler(canvas_MouseMove);
            canvas.MouseLeave += new MouseEventHandler(canvas_MouseLeave);
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
            }

            if (draggableControl.RenderTransform.Value.OffsetY > 50.0f)
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
            indicator.Visibility = Visibility.Hidden;
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
