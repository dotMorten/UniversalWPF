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
		private UIElement GetDependencyElement(DependencyProperty property, DependencyObject child, Dictionary<string, UIElement> elements)
		{			
			var dependency = child.GetValue(property);
			if(dependency == null)
				return null;
			if (dependency is string)
			{
				string name = (string)dependency;
				if (!elements.ContainsKey(name))
					throw new ArgumentException(string.Format("RelativePanel error: The name '{0}' does not exist in the current context", name));
				return elements[name];
			}
			if (dependency is UIElement)
			{
				if(Children.Contains((UIElement)dependency))
					return (UIElement)dependency;				
				throw new ArgumentException(string.Format("RelativePanel error: Element does not exist in the current context", property.Name));
			}
			
			throw new ArgumentException("RelativePanel error: Value must be of type UIElement");			
		}
        private static readonly DependencyProperty ArrangeStateProperty =
		    DependencyProperty.Register("ArrangeState", typeof(double[]), typeof(StateTrigger), new PropertyMetadata(null));

		protected override Size ArrangeOverride(Size finalSize)
		{
			Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();
			foreach (var child in Children.OfType<FrameworkElement>().Where(c => c.Name != null))
			{
				elements[child.Name] = child;
			}
			List<double[]> arranges = new List<double[]>(Children.Count);
			int i = 0;
            //First pass aligns all sides that aren't constrained by other elements
            int arrangedCount = 0;
            foreach (var child in Children.OfType<UIElement>())
            {
                //NaN means the arrange rectangle is not constrained for that value
                double[] rect = new[] { double.NaN, double.NaN, double.NaN, double.NaN };
                arranges.Add(rect);
                child.SetValue(ArrangeStateProperty, rect);

                //Align with panels always wins, so do these first
                if (GetAlignLeftWithPanel(child) ||
                    child.GetValue(AlignLeftWithProperty) == null && child.GetValue(RightOfProperty) == null &&
					!GetAlignHorizontalCenterWithPanel(child))
                    rect[0] = 0;

                if (GetAlignTopWithPanel(child) ||
                    child.GetValue(AlignTopWithProperty) == null && child.GetValue(BelowProperty) == null &&
					child.GetValue(AlignVerticalCenterWithProperty) == null &&
					!GetAlignVerticalCenterWithPanel(child))
                    rect[1] = 0;

                if (GetAlignRightWithPanel(child))
                    rect[2] = 0;
                   else if(!double.IsNaN(rect[0]) && 
                    child.GetValue(AlignRightWithProperty) == null && child.GetValue(LeftOfProperty) == null &&
					!GetAlignHorizontalCenterWithPanel(child) &&
					!GetAlignVerticalCenterWithPanel(child))
                    rect[2] = finalSize.Width - rect[0] - child.DesiredSize.Width;// finalSize.Width - (double.IsNaN(rect[0]) ? 0 : rect[0]);

                if (GetAlignBottomWithPanel(child))
                    rect[3] = 0;
                else if (!double.IsNaN(rect[1]) &&
                    (child.GetValue(AlignBottomWithProperty) == null && child.GetValue(AboveProperty) == null) &&
                    child.GetValue(AlignVerticalCenterWithProperty) == null)
                    rect[3] = finalSize.Height - rect[1] - child.DesiredSize.Height; // finalSize.Height - (double.IsNaN(rect[1]) ? 0 : rect[1]);

                if (!double.IsNaN(rect[0]) && !double.IsNaN(rect[1]) &&
					!double.IsNaN(rect[2]) && !double.IsNaN(rect[3]))
                    arrangedCount++;
            }
            while (arrangedCount < Children.Count) //Iterative layout process
            {
				int lastArrangeCount = arrangedCount;
                i = 0;
                foreach (var child in Children.OfType<UIElement>())
                {
                    double[] rect = arranges[i++];
                    if (!double.IsNaN(rect[0]) && !double.IsNaN(rect[1]) &&
						!double.IsNaN(rect[2]) && !double.IsNaN(rect[3]))
                        continue; //Control is fully arranged

                    if(double.IsNaN(rect[0])) //Left
                    {
                        var alignLeftWith = GetDependencyElement(RelativePanel.AlignLeftWithProperty, child, elements);
                        if (alignLeftWith != null)
                        {
                            double[] r = (double[])alignLeftWith.GetValue(ArrangeStateProperty);
                            if (!double.IsNaN(r[0]))
                                rect[0] = r[0];
                        }
                        else
                        {
                            var rightOf = GetDependencyElement(RelativePanel.RightOfProperty, child, elements);
                            if (rightOf != null)
                            {
                                double[] r = (double[])rightOf.GetValue(ArrangeStateProperty);
                                rect[0] = finalSize.Width - r[2];
                            }
							else if (!double.IsNaN(rect[2]))
							{
								rect[0] = finalSize.Width - rect[2] - child.DesiredSize.Width;
							}
						}
					}
                    if(double.IsNaN(rect[1])) //Top
                    {
                        var alignTopWith = GetDependencyElement(RelativePanel.AlignTopWithProperty, child, elements);
                        if (alignTopWith != null)
                        {
                            double[] r = (double[])alignTopWith.GetValue(ArrangeStateProperty);
                            if (!double.IsNaN(r[1]))
                                rect[1] = r[1];
                        }
                        else
                        {
                            var below = GetDependencyElement(RelativePanel.BelowProperty, child, elements);
                            if (below != null)
                            {
                                double[] r = (double[])below.GetValue(ArrangeStateProperty);
                                rect[1] = finalSize.Height - r[3];
							}
							else if (!double.IsNaN(rect[3]))
							{
								rect[3] = finalSize.Height - rect[3] - child.DesiredSize.Height;
							}
						}
                    }

                    if (double.IsNaN(rect[2])) //Right
                    {
                        var alignRightWith = GetDependencyElement(RelativePanel.AlignRightWithProperty, child, elements);
                        if (alignRightWith != null)
                        {
                            double[] r = (double[])alignRightWith.GetValue(ArrangeStateProperty);
                            if (!double.IsNaN(r[2]))
                                rect[2] = r[2];
                        }
                        else
                        {
                            var leftOf = GetDependencyElement(RelativePanel.LeftOfProperty, child, elements);
                            if (leftOf != null)
                            {
                                double[] r = (double[])leftOf.GetValue(ArrangeStateProperty);
                                rect[2] = finalSize.Width - r[0];
                            }
							else if(!double.IsNaN(rect[0]))
							{
								//TODO: Consider horizontal alignment
								rect[2] = finalSize.Width - rect[0] - child.DesiredSize.Width;
							}
                        }
                    }

                    if (double.IsNaN(rect[3])) //Bottom
                    {
                        var alignBottomWith = GetDependencyElement(RelativePanel.AlignBottomWithProperty, child, elements);
                        if (alignBottomWith != null)
                        {
                            double[] r = (double[])alignBottomWith.GetValue(ArrangeStateProperty);
                            if (!double.IsNaN(r[3]))
                                rect[3] = r[3];
                        }
                        else
                        {
                            var above = GetDependencyElement(RelativePanel.AboveProperty, child, elements);
                            if (above != null)
                            {
                                double[] r = (double[])above.GetValue(ArrangeStateProperty);
                                rect[3] = finalSize.Height - r[1];
                            }
							else if (!double.IsNaN(rect[1]))
							{
								//TODO: Consider vertical alignment
								rect[3] = finalSize.Height - rect[1] - child.DesiredSize.Height;
							}
						}
					}

					if (double.IsNaN(rect[0]) && double.IsNaN(rect[2]))
					{
						var alignHorizontalCenterWith = GetDependencyElement(RelativePanel.AlignHorizontalCenterWithProperty, child, elements);
						if (alignHorizontalCenterWith != null)
						{
							double[] r = (double[])alignHorizontalCenterWith.GetValue(ArrangeStateProperty);
							if (!double.IsNaN(r[0]) && !double.IsNaN(r[2]))
							{
								rect[0] = r[0] + (finalSize.Width - r[1] - r[0]) * .5 - child.DesiredSize.Width * .5;
								rect[2] = finalSize.Width - rect[0] - child.DesiredSize.Width;
							}
						}
						else
						{
							if (GetAlignHorizontalCenterWithPanel(child))
							{
								var roomToSpare = finalSize.Width - child.DesiredSize.Width;
								rect[0] = roomToSpare * .5;
								rect[2] = roomToSpare * .5;
							}
						}
					}

					if (double.IsNaN(rect[1]) && double.IsNaN(rect[3]))
					{
						var alignVerticalCenterWith = GetDependencyElement(RelativePanel.AlignVerticalCenterWithProperty, child, elements);
						if (alignVerticalCenterWith != null)
						{
							double[] r = (double[])alignVerticalCenterWith.GetValue(ArrangeStateProperty);
							if (!double.IsNaN(r[1]) && !double.IsNaN(r[3]))
							{
								rect[1] = r[1] + (finalSize.Height - r[3] - r[1]) * .5 - child.DesiredSize.Height * .5;
								rect[3] = finalSize.Height - rect[1] - child.DesiredSize.Height;
							}
						}
						//bool alignVerticalCenterWithPanel = GetAlignVerticalCenterWithPanel(child);
					}



					if (!double.IsNaN(rect[0]) && !double.IsNaN(rect[1]) && 
						!double.IsNaN(rect[2]) && !double.IsNaN(rect[3]))
                        arrangedCount++; //Control is fully arranged
                }
				if(lastArrangeCount == arrangedCount)
				{
					throw new ArgumentException("RelativePanel error: Circular dependency detected. Layout could not complete");
				}
            }
			i = 0;
			foreach (var child in Children.OfType<UIElement>())
			{
                double[] rect = arranges[i++];
                child.Arrange(new Rect(rect[0], rect[1], Math.Max(0, finalSize.Width - rect[2] - rect[0]), Math.Max(0, finalSize.Height - rect[3] - rect[1])));
			}
			return base.ArrangeOverride(finalSize);
		}
	}
}
