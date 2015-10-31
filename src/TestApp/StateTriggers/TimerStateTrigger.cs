using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using UniversalWPF;

namespace TestApp.StateTriggers
{
    public class TimerStateTrigger : StateTrigger
    {
        public TimeSpan Delay
        {
            get { return (TimeSpan)this.GetValue(DelayProperty); }
            set { this.SetValue(DelayProperty, value); }
        }

        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(TimeSpan), typeof(TimerStateTrigger), new PropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(DelayChanged)));

        private static async void DelayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (StateTrigger)obj;

            var delay = (TimeSpan)e.NewValue;

            if (delay == TimeSpan.Zero)
            {
                return;
            }

            await Task.Delay(delay);

            trigger.IsActive = true;
        }
    }
}
