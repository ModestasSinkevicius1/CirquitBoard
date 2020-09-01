using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    /// <summary>
    /// Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        private List<RowDefinition> rList = new List<RowDefinition>();
        private List<ColumnDefinition> cList = new List<ColumnDefinition>();
        private Window w;

        public bool isPressedOk = false;
        private string elementBehaviour = "alwaysGrid";

        public OptionWindow(double column, double row)
        {            
            InitializeComponent();
            AddRow();
            AddColumn();
            ChangeSliderValue(column, row);          
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (grid_preview != null)
            {
                foreach (ColumnDefinition column in grid_preview.ColumnDefinitions)
                {
                    column.Width = new GridLength(slider.Value);
                }
                //RemoveColumns();                
            }
        }

        private void slider_Copy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (grid_preview != null)
            {
                foreach (RowDefinition row in grid_preview.RowDefinitions)
                {
                    row.Height = new GridLength(slider_Copy.Value);
                }
                //RemoveRows();                
            }
        }

        private void ChangeSliderValue(double column, double row)
        {
            slider.Value = column;
            slider_Copy.Value = row;
        }

        private void AddColumn()
        {
            double originalValue = 50;
            int i = 0;

            while (i < 32)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(slider.Value);
                grid_preview.ColumnDefinitions.Add(c);

                i++;
            }                    
        }

        private void AddRow()
        {         
            double originalValue = 50;
            int i = 0;

            while (i<32)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(slider_Copy.Value);
                grid_preview.RowDefinitions.Add(r);

                i++;
            }                      
        }

        private void RemoveColumns()
        {
            foreach(ColumnDefinition c in cList)
            {
                grid_preview.ColumnDefinitions.Remove(c);              
            }      
            rList.Clear();
        }

        private void RemoveRows()
        {            
            foreach (RowDefinition r in rList)
            {
                grid_preview.RowDefinitions.Remove(r);               
            }
            cList.Clear();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            isPressedOk = true;
            Close();
        }

        public double GetColumn()
        {
            return slider.Value;
        }

        public double GetRow()
        {
            return slider_Copy.Value;
        }

        public string GetElementBehaviour()
        {
            return elementBehaviour;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(radioButton.IsChecked==true)
            {
                elementBehaviour = "alwaysGrid";               
            }
        }

        private void radioButton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            if (radioButton_Copy.IsChecked == true)
            {
                elementBehaviour = "endGrid";
            }
        }

        private void radioButton_Copy1_Checked(object sender, RoutedEventArgs e)
        {
            if (radioButton_Copy1.IsChecked == true)
            {
                elementBehaviour = "neverGrid";
            }
        }
    }
}
