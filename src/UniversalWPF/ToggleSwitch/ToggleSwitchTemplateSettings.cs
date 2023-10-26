using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalWPF
{
    /// <summary>
    /// Provides calculated values that can be referenced as TemplatedParent sources when defining
    /// templates for a ToggleSwitch control. Not intended for general use.
    /// </summary>
    public class ToggleSwitchTemplateSettings : DependencyObject
    {
        public double KnobCurrentToOffOffset { get; internal set; }
        public double KnobCurrentToOnOffset { get; internal set; }
        public double CurtainCurrentToOffOffset { get; internal set; }
        public double CurtainCurrentToOnOffset { get; internal set; }
        //public double KnobOffToOnOffset { get; internal set; }

        public double KnobOffToOnOffset
        {
            get { return (double)GetValue(KnobOffToOnOffsetProperty); }
            internal set { SetValue(KnobOffToOnOffsetProperty, value); }
        }

        public static readonly DependencyProperty KnobOffToOnOffsetProperty =
            DependencyProperty.Register(nameof(KnobOffToOnOffset), typeof(double), typeof(ToggleSwitchTemplateSettings), new PropertyMetadata(0d));
        //public double KnobOnToOffOffset { get; internal set; }



        public double KnobOnToOffOffset
        {
            get { return (double)GetValue(KnobOnToOffOffsetProperty); }
            internal set { SetValue(KnobOnToOffOffsetProperty, value); }
        }

        public static readonly DependencyProperty KnobOnToOffOffsetProperty =
            DependencyProperty.Register(nameof(KnobOnToOffOffset), typeof(double), typeof(ToggleSwitchTemplateSettings), new PropertyMetadata(0d));


        public double CurtainOffToOnOffset { get; internal set; }
        public double CurtainOnToOffOffset { get; internal set; }
    }
}
