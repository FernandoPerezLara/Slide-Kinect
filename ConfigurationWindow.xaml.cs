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

namespace Slide_Kinect {
    public partial class ConfigurationWindow : Window {
        public ConfigurationWindow() {
            InitializeComponent();
        }

        // When import button is pressed
        private void btn_Import_Click(object sender, RoutedEventArgs e) {
            LoadConfiguration.readFile(); // Read JSON file
        }
    }
}
