using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp.Samples.Numberboxes
{
    /// <summary>
    /// Interaction logic for NumberBoxSample1.xaml
    /// </summary>
    public partial class NumberBoxSample1 : UserControl
    {
        public NumberBoxSample1()
        {
            DataContext = this;
            InitializeComponent();
        }
        public DataModelWithINPC DataModelWithINPC { get; } = new DataModelWithINPC();

        private void SetTwoWayBoundNaNButton_Click(object sender, RoutedEventArgs e)
        {
            DataModelWithINPC.Value = double.NaN;
            TwoWayBoundNumberBoxValue.Text = TwoWayBoundNumberBox.Value.ToString();
        }
    }

    public class DataModelWithINPC : INotifyPropertyChanged
    {
        private double _value;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(this.Value));
                }
            }
        }
    }
}
