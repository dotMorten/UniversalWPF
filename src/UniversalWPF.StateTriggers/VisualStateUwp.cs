using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace UniversalWPF
{
    /// <summary>
    /// Adds the Setters and Triggers to the WPF VisualState clsas
    /// </summary>
    public class VisualStateUwp : VisualState, ISupportInitialize
    {
        public static readonly DependencyProperty EnableStateTriggersProperty = DependencyProperty.RegisterAttached(
            "EnableStateTriggers",
            typeof(bool),
            typeof(VisualStateUwp),
            new PropertyMetadata(false, EnableStateTriggersChanged));
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
        public static void SetEnableStateTriggers(FrameworkElement element, bool value)
        {
            element.SetValue(EnableStateTriggersProperty, value);
        }
        public static bool GetEnableStateTriggers(FrameworkElement element)
        {
            return (bool)element.GetValue(EnableStateTriggersProperty);
        }

        private ObservableCollection<StateTriggerBase> _triggers;
        private FrameworkElement element; //TODO: Make weak

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateUwp"/> class.
        /// </summary>
        public VisualStateUwp()
        {
            this.Name = Guid.NewGuid().ToString();
            Setters = new SetterBaseCollection();
            Setters.CollectionChanged += _setters_CollectionChanged;
            _stateTriggers.CollectionChanged += triggers_CollectionChanged;
        }

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
                    this.UpdateActiveState();
                }
                else
                {
                    this.element.Loaded += this.Element_Loaded; //TODO: Make weak
                    //TODO: Also track unloaded
                }
            }
        }
        private void UpdateActiveState()
        {
            this.SetActive(_stateTriggers.Any(t => t.IsTriggerActive));
        }
        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= this.Element_Loaded;
            this.UpdateActiveState();
        }

        private void _setters_CollectionChanged(object sender, EventArgs e)
        {
            UpdateActiveState();
        }

        private void triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                        item.Owner = null;
                }
            }
            UpdateActiveState();
        }

        private Action afterInit;
        bool lastState;
        internal void SetActive(bool active)
        {
            if (Element == null)
                return;
            if (_isInitializing)
            {
                afterInit = () => SetActive(active);
                return;
            }
            if (Storyboard != null && lastState != active)
            {
                if (active)
                {
                    var state = storyboardStateInvalidated ? Storyboard.GetCurrentState(Element) : System.Windows.Media.Animation.ClockState.Stopped;
                    //TimeSpan ? location = !storyboardStateInvalidated ? null : Storyboard.GetCurrentTime(Element);
                    if (state == System.Windows.Media.Animation.ClockState.Stopped)
                    {
                        Storyboard.Begin(Element, true);
                    }
                }
                else
                {
                    if (storyboardStateInvalidated)
                    {
                        //Storyboard.Seek(TimeSpan.Zero);
                        Storyboard.Stop(Element);
                    }
                }
            }
            if (active)
            {
                foreach (var setter in Setters.OfType<Setter>())
                {
                    DependencyProperty property = setter.Property;
                    object value = setter.Value;
                    string targetName = setter.TargetName;
                    var target = this.Element.FindName(targetName) as DependencyObject;
                    target?.SetValue(property, value);
                }
            }
            lastState = active;
        }
        private bool _isInitializing;

        void ISupportInitialize.BeginInit()
        {
            _isInitializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            _isInitializing = false;
            
            
            afterInit?.Invoke();
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if(e.Property.Name ==nameof(Storyboard))
            {
                if(e.OldValue is System.Windows.Media.Animation.Storyboard sbOld)
                {
                    sbOld.CurrentStateInvalidated -= Storyboard_CurrentStateInvalidated;
                }
                if (e.NewValue is System.Windows.Media.Animation.Storyboard sbNew)
                {
                    sbNew.CurrentStateInvalidated += Storyboard_CurrentStateInvalidated;
                }
                storyboardStateInvalidated = false;
            }
        }
        bool storyboardStateInvalidated;
        private void Storyboard_CurrentStateInvalidated(object sender, EventArgs e)
        {
            storyboardStateInvalidated = true;
        }

        /// <summary>
        /// Gets a collection of Setter objects
        /// </summary>
        /// <returns>A collection of Setter objects. The default is an empty collection.</returns>
        public SetterBaseCollection Setters { get; }

        ObservableCollection<StateTriggerBase> _stateTriggers = new ObservableCollection<StateTriggerBase>();
        /// <summary>
        /// Gets a collection of StateTriggerBase objects.
        /// </summary>
        /// <returns>A collection of StateTriggerBase objects. The default is an empty collection.</returns>
        public IList StateTriggers => _stateTriggers;
    }
}
