using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace UniversalWPF.StateTriggers
{
    /// <summary>
    /// Hook a VisualStateManager to keep the UWP visual states updated
    /// </summary>
    internal class VisualStateManagerHook : IDisposable
    {
        private static readonly DependencyProperty VisualStateManagerHookProperty = DependencyProperty.RegisterAttached(
            "VisualStateManagerHookInternal",
            typeof(VisualStateManagerHook),
            typeof(VisualStateManagerHook));

        private VisualStateManagerHook(FrameworkElement element)
        {
            this.Element = element;

            this.VisualStateGroups = (ObservableCollection<VisualStateGroup>)element.GetValue(VisualStateManager.VisualStateGroupsProperty);

            this.VisualStateGroups.CollectionChanged += this.CollectionChanged;

            this.ApplyElement(this.VisualStateGroups);
        }

        private FrameworkElement Element { get; set; }

        private ObservableCollection<VisualStateGroup> VisualStateGroups { get; set; }

        internal static void Set(FrameworkElement element)
        {
            var hook = new VisualStateManagerHook(element);

            element.SetValue(VisualStateManagerHookProperty, hook);
        }

        internal static void UnSet(FrameworkElement element)
        {
            var hook = element.GetValue(VisualStateManagerHookProperty) as VisualStateManagerHook;

            if (hook != null)
            {
                element.SetValue(VisualStateManagerHookProperty, null);
                hook.Dispose();
            }
        }

        public void Dispose()
        {
            this.RemoveElement(this.VisualStateGroups);

            this.VisualStateGroups.CollectionChanged -= this.CollectionChanged;

            // Probably useless, but still an extra precaution to avoid memory leaks
            this.Element = null;
            this.VisualStateGroups = null;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                this.ApplyElement(e.NewItems.OfType<VisualStateGroup>());
            }

            if (e.OldItems != null)
            {
                this.RemoveElement(e.OldItems.OfType<VisualStateGroup>());
            }
        }

        private void ApplyElement(IEnumerable<VisualStateGroup> visualStateGroups)
        {
            foreach (var group in visualStateGroups)
            {
                foreach (var state in group.States.OfType<VisualStateUwp>())
                {
                    state.Element = this.Element;
                }
            }
        }

        private void RemoveElement(IEnumerable<VisualStateGroup> visualStateGroups)
        {
            foreach (var group in visualStateGroups)
            {
                foreach (var state in group.States.OfType<VisualStateUwp>())
                {
                    state.Element = null;
                }
            }
        }
    }
}
