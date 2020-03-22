using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ApplicationUI.Views
{
    /// <summary>
    /// Interaction logic for TimeChangerView.xaml
    /// </summary>
    public partial class TimeChangerView : UserControl
    {
        public TimeChangerView()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
           e.Handled = !int.TryParse(e.Text, out var result);
        }
    }
}
