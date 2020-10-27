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

        string oldValue_current = "0";
        string oldValue_voltage = "0";
        string oldValue_resistance = "0";
        string oldValue_power = "0";

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

        private void textBox_current_TextChanged(object sender, TextChangedEventArgs e)
        {            
            string value = textBox_current.Text;

            if (!double.TryParse(value, out _))
            {
                textBox_current.Text = oldValue_current;
            }
            else
            {
                oldValue_current = value;
            }
        }

        private void textBox_voltage_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = textBox_voltage.Text;

            if (!double.TryParse(value, out _))
            {
                textBox_voltage.Text = oldValue_voltage;
            }
            else
            {
                oldValue_voltage = value;
            }
        }

        private void textBox_resistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = textBox_resistance.Text;

            if (!double.TryParse(value, out _))
            {
                textBox_resistance.Text = oldValue_resistance;
            }
            else
            {
                oldValue_resistance = value;
            }
        }

        private void textBox_power_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = textBox_power.Text;

            if (!double.TryParse(value, out _))
            {
                textBox_power.Text = oldValue_power;
            }
            else
            {
                oldValue_power = value;
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
