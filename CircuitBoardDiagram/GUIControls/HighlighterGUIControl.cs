using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram.GUIControls
{
    class HighlighterGUIControl
    {
        public Rectangle highlighter { get; set; }
        public Rectangle indicator { get; set; }

        public List<Rectangle> clList = new List<Rectangle>();

        private Canvas canvas;
        private Grid grid;

        public HighlighterGUIControl(Canvas canvas, Grid grid, Rectangle highlighter, Rectangle indicator)
        {
            this.highlighter = highlighter;
            this.indicator = indicator;
            this.canvas = canvas;
            this.grid = grid;
            //LoadCheckCircuitBox();
        }

        /*public void LoadCheckCircuitBox()
        {           
            SolidColorBrush scb = Brushes.DeepSkyBlue;

            cbox.Width = 60;
            cbox.Height = 60;

            cbox.Fill = scb;
            cbox.Opacity = 0.80f;

            canvas.Children.Add(cbox);
        }*/

        public void RemoveCheckCircuitBox()
        {            
            int count = clList.Count;

            for(int i=0;i<count;i++)
            {
                canvas.Children.Remove(clList[i]);
                clList[i] = null;
            }
            clList.Clear();
        }

        public void ShowCheckCircuitBox(Image draggableControl)
        {
            Rectangle cbox = new Rectangle();
            SolidColorBrush scb = Brushes.DeepSkyBlue;

            cbox.Width = 50;
            cbox.Height = 50;

            cbox.Fill = scb;
            cbox.Opacity = 0.80f;           

            canvas.Children.Add(cbox);
            cbox.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX, draggableControl.RenderTransform.Value.OffsetY);
            
            clList.Add(cbox);
        }

        public void IndicateCell()
        {
            indicator.Visibility = Visibility.Visible;

            double distanceX;
            double distanceY;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            foreach (ColumnDefinition column in grid.ColumnDefinitions)
            {
                distanceX = Math.Abs((Mouse.GetPosition(canvas).X - (column.Width.Value / 2)) - (column.Width.Value * i));

                if (oldDistanceX > distanceX)
                {
                    oldDistanceX = distanceX;
                    cellX = i;
                }
                i++;
            }

            i = 0;

            foreach (RowDefinition row in grid.RowDefinitions)
            {
                distanceY = Math.Abs((Mouse.GetPosition(canvas).Y - (row.Height.Value / 2)) - (row.Height.Value * i));

                if (oldDistanceY > distanceY)
                {
                    oldDistanceY = distanceY;
                    cellY = i;
                }
                i++;
            }

            cellWidth = grid.ColumnDefinitions[(int)cellX].Width.Value;
            cellHeight = grid.RowDefinitions[(int)cellY].Height.Value;

            Canvas.SetLeft(indicator, 0);
            Canvas.SetTop(indicator, 0);

            indicator.RenderTransform = new TranslateTransform(cellWidth * cellX, cellHeight * cellY);

            Grid.SetRow(indicator, 0);
            Grid.SetColumn(indicator, 0);
        }
        
        public void UpdateIndicatorSize()
        {
            double lengthX = grid.ColumnDefinitions[0].Width.Value;
            double lengthY = grid.RowDefinitions[0].Height.Value;

            int spanX = 1;
            int spanY = 1;

            if (lengthX < 50)
            {
                if (lengthX < 25)
                {
                    if (lengthX < 12.5)
                    {
                        if (lengthX < 6.25)
                        {
                            spanX = 16;
                        }
                        else
                            spanX = 8;
                    }
                    else
                        spanX = 4;
                }
                else
                    spanX = 2;
            }

            if (lengthY < 50)
            {
                if (lengthY < 25)
                {
                    if (lengthY < 12.5)
                    {
                        if (lengthY < 6.25)
                        {
                            spanY = 16;
                        }
                        else
                            spanY = 8;
                    }
                    else
                        spanY = 4;
                }
                else
                    spanY = 2;
            }

            indicator.SetValue(Grid.ColumnSpanProperty, spanX);
            indicator.SetValue(Grid.RowSpanProperty, spanY);
        }
        public void UpdateHighlightorSize()
        {
            double lengthX = grid.ColumnDefinitions[0].Width.Value;
            double lengthY = grid.RowDefinitions[0].Height.Value;

            int spanX = 2;
            int spanY = 2;

            if (lengthX < 50)
            {
                if (lengthX < 25)
                {
                    if (lengthX < 12.5)
                    {
                        if (lengthX < 6.25)
                        {
                            spanX = 32;
                        }
                        else
                            spanX = 16;
                    }
                    else
                        spanX = 8;
                }
                else
                    spanX = 4;
            }

            if (lengthY < 50)
            {
                if (lengthY < 25)
                {
                    if (lengthY < 12.5)
                    {
                        if (lengthY < 6.25)
                        {
                            spanY = 32;
                        }
                        else
                            spanY = 16;
                    }
                    else
                        spanY = 8;
                }
                else
                    spanY = 4;
            }

            highlighter.SetValue(Grid.ColumnSpanProperty, spanX);
            highlighter.SetValue(Grid.RowSpanProperty, spanY);
        }       
        public void Highlight_cell(Image draggableControl)
        {
            highlighter.Visibility = Visibility.Visible;
            highlighter.RenderTransform = draggableControl.RenderTransform;
        }              
    }
}
