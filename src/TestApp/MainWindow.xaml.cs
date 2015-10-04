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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;

namespace TestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadSamples();
		}

		private void LoadSamples()
		{
			var samples = typeof(TestApp.MainWindow).GetTypeInfo().Assembly.GetTypes().Where(t => t.BaseType == typeof(UserControl) && t.FullName.Contains(".Samples."));

			sampleList.ItemsSource = samples;
		}

		private void sampleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems != null)
			{
				var t = e.AddedItems[0] as Type;
				var c = t.GetConstructor(new Type[] { });
				var sampleinstance = c.Invoke(new object[] { }) as UserControl;
				sampleView.Children.Clear();
				try
				{
					sampleView.Children.Add(sampleinstance);
				}
				catch (System.Exception ex)
				{
				}
			}
		}
	}
}
