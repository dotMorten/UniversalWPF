using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestApp.Samples
{
    public class KeyValuePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            int pairCount = (int)Math.Ceiling(Children.Count / 2d);
            if (pairCount == 0)
                return new Size(0, 0);
            double cellWidth = 0;
            double cellHeight = 0;
            double maxWidth = Orientation == Orientation.Horizontal ? availableSize.Width / pairCount : availableSize.Width / 2;
            double maxHeight = Orientation == Orientation.Horizontal ? availableSize.Height / 2 : availableSize.Height / pairCount;
            foreach (var child in Children.OfType<UIElement>())
            {
                child.Measure(new Size(maxWidth, maxHeight));
                cellWidth = Math.Max(cellWidth, child.DesiredSize.Width);
                cellHeight = Math.Max(cellHeight, child.DesiredSize.Height);
            }
            if (Orientation == Orientation.Horizontal)
                return new Size(cellWidth * pairCount, cellHeight * 2);
            else
                return new Size(cellWidth * 2, pairCount * cellHeight);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            int pairCount = (int)Math.Ceiling(Children.Count / 2d);
            if (pairCount == 0)
                return new Size(0, 0);
            double cellWidth = 0;
            double cellHeight = 0;
            double maxWidth = Orientation == Orientation.Horizontal ? finalSize.Width / pairCount : finalSize.Width / 2;
            double maxHeight = Orientation == Orientation.Horizontal ? finalSize.Height / 2 : finalSize.Height / pairCount;
            foreach (var child in Children.OfType<UIElement>())
            {
                cellWidth = Math.Max(cellWidth, child.DesiredSize.Width);
                cellHeight = Math.Max(cellHeight, child.DesiredSize.Height);
            }
            double x = 0;
            double y = 0;
            int count = 0;
            foreach (var child in Children.OfType<UIElement>())
            {
                child.Arrange(new Rect(x, y, cellWidth, cellHeight));
                if(Orientation == Orientation.Horizontal)
                {
                    if (count % 2 == 1)
                    {
                        x += cellWidth;
                        y = 0;
                    }
                    else
                    {
                        y += cellHeight;
                    }
                }
                else
                {
                    if (count % 2 == 1)
                    {
                        y += cellHeight;
                        x = 0;
                    }
                    else
                    {
                        x += cellWidth;
                    }
                }
                count++;
            }
            if (Orientation == Orientation.Horizontal)
                return new Size(x + cellWidth, cellHeight * 2);
            else
                return new Size(cellWidth * 2, y + cellHeight);
        }



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(KeyValuePanel), new PropertyMetadata(Orientation.Horizontal));


    }
}
