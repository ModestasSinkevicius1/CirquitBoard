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

namespace CircuitBoardDiagram.GUIControls
{
    class CanvasGUIControl
    {
        /*public bool isOutOfBounds(MouseEventArgs e)
        {
            Point cursorP = e.GetPosition(this);

            double gridX = 50 - canvasGrid.ColumnDefinitions[0].Width.Value;
            double gridY = 50 - canvasGrid.RowDefinitions[0].Height.Value;

            if (cursorP.X < canvas.Margin.Left || cursorP.Y < canvas.Margin.Top || cursorP.X > (canvas.Width - gridX) + canvas.Margin.Left || cursorP.Y > (canvas.Height - gridY) + canvas.Margin.Top)
            {
                return false;
            }
            return true;
        }

        public void ResetColumn()
        {
            int count = canvasGrid.ColumnDefinitions.Count;
            int i = 0;

            while (count > 0)
            {
                canvasGrid.ColumnDefinitions.RemoveAt(0);
                count = canvasGrid.ColumnDefinitions.Count;
                i++;
            }
        }

        public void ResetRow()
        {
            int count = canvasGrid.RowDefinitions.Count;
            int i = 0;

            while (count > 0)
            {
                canvasGrid.RowDefinitions.RemoveAt(0);
                count = canvasGrid.RowDefinitions.Count;
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
                canvasGrid.ColumnDefinitions.Add(c);

                i++;
            }

            foreach (ColumnDefinition column in canvasGrid.ColumnDefinitions)
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
                canvasGrid.RowDefinitions.Add(r);

                i++;
            }

            foreach (RowDefinition row in canvasGrid.RowDefinitions)
            {
                row.Height = new GridLength(value);
            }
        }
        public void LoadGrids()
        {
            canvasGrid.Width = 600 * 8;
            canvasGrid.Height = 350 * 5;

            ResetColumn();
            ResetRow();

            AddColumn(50, 12 * 8);
            AddRow(50, 7 * 5);
        }*/
    }
}
