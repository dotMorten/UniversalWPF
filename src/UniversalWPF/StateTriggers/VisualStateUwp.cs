using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UniversalWPF.StateTriggers;

namespace UniversalWPF
{
    public class VisualStateUwp : VisualState
    {
        private FrameworkElement element;

        public static readonly DependencyProperty EnableStateTriggersProperty = DependencyProperty.RegisterAttached(
            "EnableStateTriggers",
            typeof(bool),
            typeof(VisualStateUwp),
            new PropertyMetadata(false, EnableStateTriggersChanged));

        public VisualStateUwp()
        {
            // Make sure the state has a name, if the user forgot to assign one
            this.Name = Guid.NewGuid().ToString();

            this.Setters = new ObservableCollection<Setter>();
            this.StateTriggers = new ObservableCollection<StateTriggerBase>();
            this.StateTriggers.CollectionChanged += this.Triggers_CollectionChanged;
            this.Setters.CollectionChanged += this.Setters_CollectionChanged;
        }

        /// <summary>
        /// Gets a collection of Setter objects
        /// </summary>
        /// <returns>A collection of Setter objects. The default is an empty collection.</returns>
        public ObservableCollection<Setter> Setters { get; }

        /// <summary>
        /// Gets a collection of StateTriggerBase objects.
        /// </summary>
        /// <returns>A collection of StateTriggerBase objects. The default is an empty collection.</returns>
        public ObservableCollection<StateTriggerBase> StateTriggers { get; }

        /// <summary>
        /// Gets or sets the the UI element the VisualStateManager is bound to
        /// </summary>
        internal FrameworkElement Element
        {
            get
            {
                return this.element;
            }

            set
            {
                this.element = value;

                if (this.element.IsLoaded)
                {
                    this.SetActive();
                }
                else
                {
                    this.element.Loaded += this.Element_Loaded;
                }
            }
        }

        /// <summary>
        /// Sets the value indicating whether state triggers should be enabled for the given element
        /// </summary>
        /// <param name="element">Element to enable state triggers on</param>
        /// <param name="value">Value indicating whether state triggers should be enabled</param>
        public static void SetEnableStateTriggers(FrameworkElement element, bool value)
        {
            element.SetValue(EnableStateTriggersProperty, value);
        }

        /// <summary>
        /// Gets the value indicating whether state triggers are enabled for the given element
        /// </summary>
        /// <param name="element">Element to enable state triggers on</param>
        /// <returns>True if the state triggers are enabled on the given element, False otherwise</returns>
        public static bool GetEnableStateTriggers(FrameworkElement element)
        {
            return (bool)element.GetValue(EnableStateTriggersProperty);
        }
        
        private static void EnableStateTriggersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as FrameworkElement;

            if (element == null)
            {
                throw new NotSupportedException("EnableStateTriggers can only be applied on elements of type System.Windows.FrameworkElement");
            }

            if ((bool)e.NewValue)
            {
                VisualStateManagerHook.Set(element);
            }
            else
            {
                VisualStateManagerHook.UnSet(element);
            }
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= this.Element_Loaded;

            this.SetActive();
        }

        private void Setters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SetActive();
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<StateTriggerBase>())
                {
                    item.Owner = this;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<StateTriggerBase>())
                {
                    if (item.Owner == this)
                    {
                        item.Owner = null;
                    }
                }
            }

            this.SetActive(this.StateTriggers.Any(t => t.IsTriggerActive));
        }
        
        internal void SetActive(bool active)
        {
            if (this.Element == null || !active)
            {
                return;
            }
            
            // TODO: Not sure whether the transitions should be enabled. Need to check the behavior of UWP state triggers
            VisualStateManager.GoToElementState(this.Element, this.Name, false);

            foreach (var setter in this.Setters)
            {
                var property = setter.Property;
                var value = setter.Value; // Why doesn't this  return the actual value???
                var targetName = setter.TargetName;

                var target = this.Element.FindName(targetName) as DependencyObject;

                target?.SetValue(property, value);
            }
        }

        private void SetActive()
        {
            this.SetActive(this.StateTriggers.Any(t => t.IsTriggerActive));
        }
    }
}
