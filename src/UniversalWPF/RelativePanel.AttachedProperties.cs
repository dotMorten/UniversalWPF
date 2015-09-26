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

namespace UniversalWPF
{
	public partial class RelativePanel
	{
		private static void OnAlignPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var elm = d as FrameworkElement;
			if (elm.Parent is FrameworkElement)
				((FrameworkElement)elm.Parent).InvalidateArrange();
		}

		public static object GetAbove(DependencyObject obj)
		{
			return (object)obj.GetValue(AboveProperty);
		}

		public static void SetAbove(DependencyObject obj, object value)
		{
			obj.SetValue(AboveProperty, value);
		}

		public static readonly DependencyProperty AboveProperty =
			DependencyProperty.RegisterAttached("Above", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));
		

		public static bool GetAlignBottomWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignBottomWithPanelProperty);
		}

		public static void SetAlignBottomWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignBottomWithPanelProperty, value);
		}

		public static readonly DependencyProperty AlignBottomWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignBottomWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));

		public static object GetAlignBottomWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignBottomWithProperty);
		}

		public static void SetAlignBottomWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignBottomWithProperty, value);
		}

		public static readonly DependencyProperty AlignBottomWithProperty =
			DependencyProperty.RegisterAttached("AlignBottomWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));

		public static bool GetAlignHorizontalCenterWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignHorizontalCenterWithPanelProperty);
		}

		public static void SetAlignHorizontalCenterWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignHorizontalCenterWithPanelProperty, value);
		}

		public static readonly DependencyProperty AlignHorizontalCenterWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignHorizontalCenterWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));

		public static object GetAlignHorizontalCenterWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignHorizontalCenterWithProperty);
		}

		public static void SetAlignHorizontalCenterWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignHorizontalCenterWithProperty, value);
		}

		public static readonly DependencyProperty AlignHorizontalCenterWithProperty =
			DependencyProperty.RegisterAttached("AlignHorizontalCenterWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));



		public static bool GetAlignLeftWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignLeftWithPanelProperty);
		}

		public static void SetAlignLeftWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignLeftWithPanelProperty, value);
		}


		public static readonly DependencyProperty AlignLeftWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignLeftWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));


		public static object GetAlignLeftWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignLeftWithProperty);
		}

		public static void SetAlignLeftWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignLeftWithProperty, value);
		}

		// Using a DependencyProperty as the backing store for AlignLeftWith.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AlignLeftWithProperty =
			DependencyProperty.RegisterAttached("AlignLeftWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));


		public static bool GetAlignRightWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignRightWithPanelProperty);
		}

		public static void SetAlignRightWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignRightWithPanelProperty, value);
		}

		public static readonly DependencyProperty AlignRightWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignRightWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));

		public static object GetAlignRightWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignRightWithProperty);
		}

		public static void SetAlignRightWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignRightWithProperty, value);
		}

		public static readonly DependencyProperty AlignRightWithProperty =
			DependencyProperty.RegisterAttached("AlignRightWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));

		public static bool GetAlignTopWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignTopWithPanelProperty);
		}

		public static void SetAlignTopWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignTopWithPanelProperty, value);
		}

		public static readonly DependencyProperty AlignTopWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignTopWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));



		public static object GetAlignTopWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignTopWithProperty);
		}

		public static void SetAlignTopWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignTopWithProperty, value);
		}

		public static readonly DependencyProperty AlignTopWithProperty =
			DependencyProperty.RegisterAttached("AlignTopWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));



		public static bool GetAlignVerticalCenterWithPanel(DependencyObject obj)
		{
			return (bool)obj.GetValue(AlignVerticalCenterWithPanelProperty);
		}

		public static void SetAlignVerticalCenterWithPanel(DependencyObject obj, bool value)
		{
			obj.SetValue(AlignVerticalCenterWithPanelProperty, value);
		}

		// Using a DependencyProperty as the backing store for AlignVerticalCenterWithPanel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AlignVerticalCenterWithPanelProperty =
			DependencyProperty.RegisterAttached("AlignVerticalCenterWithPanel", typeof(bool), typeof(RelativePanel), new PropertyMetadata(false, OnAlignPropertiesChanged));




		public static object GetAlignVerticalCenterWith(DependencyObject obj)
		{
			return (object)obj.GetValue(AlignVerticalCenterWithProperty);
		}

		public static void SetAlignVerticalCenterWith(DependencyObject obj, object value)
		{
			obj.SetValue(AlignVerticalCenterWithProperty, value);
		}

		public static readonly DependencyProperty AlignVerticalCenterWithProperty =
			DependencyProperty.RegisterAttached("AlignVerticalCenterWith", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));

		public static object GetBelow(DependencyObject obj)
		{
			return (object)obj.GetValue(BelowProperty);
		}

		public static void SetBelow(DependencyObject obj, object value)
		{
			obj.SetValue(BelowProperty, value);
		}

		public static readonly DependencyProperty BelowProperty =
			DependencyProperty.RegisterAttached("Below", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));

		public static object GetLeftOf(DependencyObject obj)
		{
			return (object)obj.GetValue(LeftOfProperty);
		}

		public static void SetLeftOf(DependencyObject obj, object value)
		{
			obj.SetValue(LeftOfProperty, value);
		}

		public static readonly DependencyProperty LeftOfProperty =
			DependencyProperty.RegisterAttached("LeftOf", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));

		public static object GetRightOf(DependencyObject obj)
		{
			return (object)obj.GetValue(RightOfProperty);
		}

		public static void SetRightOf(DependencyObject obj, object value)
		{
			obj.SetValue(RightOfProperty, value);
		}

		// Using a DependencyProperty as the backing store for RightOf.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RightOfProperty =
			DependencyProperty.RegisterAttached("RightOf", typeof(object), typeof(RelativePanel), new PropertyMetadata(null, OnAlignPropertiesChanged));



	}
}
