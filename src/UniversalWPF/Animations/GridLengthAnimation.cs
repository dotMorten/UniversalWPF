namespace UniversalWPF
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class GridLengthAnimation : AnimationTimeline
    {
        public override object GetCurrentValue(object defaultOriginValue,
            object defaultDestinationValue, AnimationClock animationClock)
        {
            var from = ((GridLength)GetValue(GridLengthAnimation.FromProperty));
            var to = ((GridLength)GetValue(GridLengthAnimation.ToProperty));
            if (from.GridUnitType != to.GridUnitType) //We can't animate different types, so just skip straight to it
                return to;
            double fromVal = from.Value;
            double toVal = to.Value;

            if (fromVal > toVal)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) *
                    (fromVal - toVal) + toVal, GridUnitType.Star);
            }
            else
            {
                return new GridLength(animationClock.CurrentProgress.Value *
                    (toVal - fromVal) + fromVal, GridUnitType.Star);
            }
        }

        public override Type TargetPropertyType
        {
            get { return typeof(GridLength); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength From
        {
            get { return (GridLength)GetValue(GridLengthAnimation.FromProperty); }
            set { SetValue(GridLengthAnimation.FromProperty, value); }
        }
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        public GridLength To
        {
            get { return (GridLength)GetValue(GridLengthAnimation.ToProperty); }
            set { SetValue(GridLengthAnimation.ToProperty, value); }
        }
    }
}