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
    public partial class RelativePanel : Panel
    {
		//static RelativePanel()
		//{
		//    DefaultStyleKeyProperty.OverrideMetadata(typeof(RelativePanel), new FrameworkPropertyMetadata(typeof(RelativePanel)));
		//}

		protected override Size MeasureOverride(Size availableSize)
		{
			foreach(var child in Children.OfType<FrameworkElement>())
			{
				child.Measure(availableSize);
			}
			return base.MeasureOverride(availableSize);
		}

		/*
			Default position

			By default, any unconstrained element declared as a child of the RelativePanel is given the entire
			available space and positioned at the (0, 0) coordinates (upper left corner) of the panel. So, if you
			are positioning a second element relative to an unconstrained element, keep in mind that the second 
			element might get pushed out of the panel. 

			Conflicting relationships

			If you set multiple relationships that target the same edge of an element, you might have conflicting
			relationships in your layout as a result. When this happens, the relationships are applied in the 
			following order of priority:
			  •   Panel alignment relationships (AlignTopWithPanel, AlignLeftWithPanel, …) are applied first.
			  •   Sibling alignment relationships (AlignTopWith, AlignLeftWith, …) are applied second.
			  •   Sibling positional relationships (Above, Below, RightOf, LeftOf) are applied last.

			The panel-center alignment properties (AlignVerticalCenterWith, AlignHorizontalCenterWithPanel, ...) are
			typically used independently of other constraints and are applied if there is no conflict.

			The HorizontalAlignment and VerticalAlignment properties on UI elements are applied after relationship 
			properties are evaluated and applied. These properties control the placement of the element within the
			available size for the element, if the desired size is smaller than the available size.

		*/

		protected override Size ArrangeOverride(Size finalSize)
		{
			Dictionary<string, object> elements = new Dictionary<string, object>();
			foreach (var child in Children.OfType<FrameworkElement>().Where(c => c.Name != null))
			{
				elements[child.Name] = child;
			}
			foreach(var child in Children.OfType<FrameworkElement>())
			{
				double left = 0;
				double top = 0;
				double width = child.DesiredSize.Width;
				double height = child.DesiredSize.Height;
				var rightWidth = GetAlignRightWith(child);
				if(rightWidth is string)
				{
					if (!elements.ContainsKey((string)rightWidth))
						throw new ArgumentException(string.Format("RelativePanel error: The name '{0}' does not exist in the current context", rightWidth));
					rightWidth = elements[(string)rightWidth];
				}
				if (rightWidth is FrameworkElement)
					left = ((FrameworkElement)rightWidth).DesiredSize.Width;
				else if (GetAlignLeftWithPanel(child))
					left = 0;
				if (GetAlignRightWithPanel(child))
					width = finalSize.Width - left;
				if (GetAlignTopWithPanel(child))
					top = 0;
				if (GetAlignBottomWithPanel(child))
					height = finalSize.Height - top;
				//if (GetAlignRightWithPanel(child))
				//	height = finalSize.Width - left;
				child.Arrange(new Rect(left, top, width, height));
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
