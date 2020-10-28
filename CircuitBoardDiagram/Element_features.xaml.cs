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
    /// Interaction logic for Element_features.xaml
    /// </summary>
    public partial class Element_features : Window
    {
        SpecificElement se;
       
        string oldValue_double = "0";

        string oldValue_int = "0";

        public Element_features(SpecificElement se)
        {
            InitializeComponent();
            this.se = se;
            UpdateTextBoxes();
        }

        private void UpdateTextBoxes()
        {
            label_element.Content = se.GetName();

            textBox_current.Text = se.current.ToString();
            textBox_voltage.Text = se.voltage.ToString();
            textBox_resistance.Text = se.resistance.ToString();
            textBox_power.Text = se.power.ToString();

            textBox_max_connection.Text = se.maxCount.ToString();
            textBox_required_connection.Text = se.requiredCount.ToString();

            checkBox_custom.IsChecked = se.isCustom;

            isCustomChecked();
        }

        private void UpdateElement()
        {
            se.current = Convert.ToDouble(textBox_current.Text);
            se.voltage = Convert.ToDouble(textBox_voltage.Text);
            se.resistance = Convert.ToDouble(textBox_resistance.Text);
            se.power = Convert.ToDouble(textBox_power.Text);

            se.maxCount = Convert.ToInt32(textBox_max_connection.Text);
            se.requiredCount = Convert.ToInt32(textBox_required_connection.Text);

            se.isCustom = checkBox_custom.IsChecked.Value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateElement();
            Close();
        }
       
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;            

            string value = text.Text;            

            if (!double.TryParse(value, out _))
            {
                text.Text = oldValue_double;
            }
            else
            {
                oldValue_double = value;
            }
        }

        private void textBox_TextChanged_int(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;

            string value = text.Text;

            if (!int.TryParse(value, out _))
            {               
                text.Text = oldValue_int;
            }
            else
            {
                if (Convert.ToInt32(value) <= 4)
                {
                    oldValue_int = value;
                }
                else
                {
                    text.Text = "4";
                    value = "4";
                    oldValue_int = value;                    
                }
            }
        }

        private void checkBox_custom_Checked(object sender, RoutedEventArgs e)
        {
            isCustomChecked();            
        }

        private void checkBox_custom_Unchecked(object sender, RoutedEventArgs e)
        {
            isCustomChecked();
        }

        private void isCustomChecked()
        {
            if (checkBox_custom.IsChecked == false)
            {
                textBox_max_connection.IsEnabled = false;
                textBox_required_connection.IsEnabled = false;
            }
            else
            {
                textBox_max_connection.IsEnabled = true;
                textBox_required_connection.IsEnabled = true;
            }
        }

        
    }
}
