using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UniversalWPF
{
    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
    /// <remarks>
    /// Use a <see cref="ToggleSwitch"/> control to let the user switch an option between on and off states.
    /// Use the <see cref="IsOn"/> property to determine the state of the switch. Handle the 
    /// <see cref="Toggled"/> event to respond to changes in the state.
    /// </remarks>
    public partial class ToggleSwitch : Control
    {
        bool m_isDragging = false;
        bool m_isTapping = false;
        bool m_wasDragged = false;
        bool m_isPointerOver = false;

        // The translations for the curtain and knob template parts.
        double m_knobTranslation = 0;
        double m_minKnobTranslation = 0;
        double m_maxKnobTranslation = 0;
        double m_curtainTranslation = 0;
        double m_minCurtainTranslation = 0;
        double m_maxCurtainTranslation = 0;
        bool m_handledKeyDown = false;

        // The template parts.
        UIElement m_tpCurtainClip;
        FrameworkElement m_tpKnob;
        FrameworkElement m_tpKnobBounds;
        FrameworkElement m_tpCurtainBounds;
        System.Windows.Controls.Primitives.Thumb m_tpThumb;
        UIElement m_tpHeaderPresenter;

        // The translate transforms from template parts.
        TranslateTransform m_spKnobTransform;
        TranslateTransform m_spCurtainTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitch"/> class.
        /// </summary>
        public ToggleSwitch()
        {
            TemplateSettings = new ToggleSwitchTemplateSettings();
            DefaultStyleKey = typeof(ToggleSwitch);
            IsVisibleChanged += OnVisibilityChanged;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        private void ChangeVisualState(bool useTransitions)
        {
            bool isEnabled = IsEnabled;
            bool focusState = IsFocused || IsKeyboardFocused;

            if (!isEnabled)
            {
                GoToState(useTransitions, "Disabled");
            }
            else if (m_isDragging)
            {
                GoToState(useTransitions, "Pressed");
            }
            else if (m_isPointerOver)
            {
                GoToState(useTransitions, "PointerOver");
            }
            else
            {
                GoToState(useTransitions, "Normal");
            }

            if (focusState && isEnabled)
            {
                
                if (!IsKeyboardFocused)
                {
                    GoToState(useTransitions, "PointerFocused");
                }
                else
                {
                    GoToState(useTransitions, "Focused");
                }
            }
            else
            {
                GoToState(useTransitions, "Unfocused");
            }


            if (m_isDragging)
            {
                GoToState(useTransitions, "Dragging");
            }
            else
            {
                bool isOn = IsOn;
                GoToState(useTransitions, isOn ? "On" :"Off");
                GoToState(useTransitions, isOn ? "OnContent" : "OffContent");
            }
        }

        private bool GoToState(bool useTransitions, string stateName) => VisualStateManager.GoToState(this, stateName, useTransitions);

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            if (m_tpThumb != null)
            {
                m_tpThumb.DragStarted -= DragStartedHandler;
                m_tpThumb.DragDelta -= DragDeltaHandler;
                m_tpThumb.DragCompleted -= DragCompletedHandler;
                m_tpThumb.MouseLeftButtonDown -= Thumb_MouseLeftButtonDown;
                m_tpThumb.MouseLeftButtonUp -= Thumb_MouseLeftButtonUp;
            }
            if (m_tpKnob != null)
            {
                m_tpKnob.SizeChanged -= SizeChangedHandler;
            }
            if (m_tpKnobBounds != null)
            {
                m_tpKnobBounds.SizeChanged -= SizeChangedHandler;
            }
            m_spCurtainTransform = null;
            m_spKnobTransform = null;

            base.OnApplyTemplate();

            UIElement spCurtainIUIElement = GetTemplateChild("SwitchCurtain") as UIElement;
            m_tpCurtainBounds = GetTemplateChild("SwitchCurtainBounds") as FrameworkElement;
            m_tpCurtainClip = GetTemplateChild("SwitchCurtainClip") as FrameworkElement;
            m_tpKnob = GetTemplateChild("SwitchKnob") as FrameworkElement;
            m_tpKnobBounds = GetTemplateChild("SwitchKnobBounds") as FrameworkElement;
            m_tpThumb = GetTemplateChild("SwitchThumb") as System.Windows.Controls.Primitives.Thumb;
            m_spCurtainTransform = spCurtainIUIElement?.RenderTransform as TranslateTransform;
            m_spKnobTransform = m_tpKnob?.RenderTransform as TranslateTransform;

            if (m_tpThumb != null)
            {
                m_tpThumb.DragStarted += DragStartedHandler;
                m_tpThumb.DragDelta += DragDeltaHandler;
                m_tpThumb.DragCompleted += DragCompletedHandler;
                m_tpThumb.MouseLeftButtonDown += Thumb_MouseLeftButtonDown;
                m_tpThumb.MouseLeftButtonUp += Thumb_MouseLeftButtonUp;
            }

            if (m_tpKnob != null || m_tpKnobBounds != null)
            {
                this.SizeChanged += SizeChangedHandler;
                if (m_tpKnob != null)
                {
                    m_tpKnob.SizeChanged += SizeChangedHandler;
                }
                if (m_tpKnobBounds != null)
                {
                    m_tpKnobBounds.SizeChanged += SizeChangedHandler;
                }
            }
            UpdateHeaderPresenterVisibility();
            ChangeVisualState(false);
        }

        private void GetTranslations()
        {
            if (m_spKnobTransform != null)
            {
                m_knobTranslation = m_spKnobTransform.X;
            }

            if (m_spCurtainTransform != null)
            {
                m_curtainTranslation = m_spCurtainTransform.X;
            }
        }

        private void SetTranslations()
        {
            double translation = 0;
            var pToggleSwitchTemplateSettingsNoRef = TemplateSettings;
            if (m_spKnobTransform != null)
            {
                translation = Math.Min(m_knobTranslation, m_maxKnobTranslation);
                translation = Math.Max(translation, m_minKnobTranslation);

                m_spKnobTransform.X = translation;

                if (pToggleSwitchTemplateSettingsNoRef != null)
                {
                    pToggleSwitchTemplateSettingsNoRef.KnobCurrentToOffOffset = translation - m_minKnobTranslation;
                    pToggleSwitchTemplateSettingsNoRef.KnobCurrentToOnOffset = translation - m_maxKnobTranslation;
                }
            }

            if (m_spCurtainTransform != null)
            {
                translation = Math.Min(m_curtainTranslation, m_maxCurtainTranslation);
                translation = Math.Max(translation, m_minCurtainTranslation);

                m_spCurtainTransform.X = translation;

                if (pToggleSwitchTemplateSettingsNoRef != null)
                {
                    pToggleSwitchTemplateSettingsNoRef.CurtainCurrentToOffOffset = translation - m_minCurtainTranslation;
                    pToggleSwitchTemplateSettingsNoRef.CurtainCurrentToOnOffset = translation - m_maxCurtainTranslation;
                }
            }
        }

        private void ClearTranslations()
        {
            m_spKnobTransform?.ClearValue(TranslateTransform.XProperty);
            m_spCurtainTransform?.ClearValue(TranslateTransform.XProperty);
        }

        private void Toggle()
        {
            IsOn = !IsOn;
        }

        private void MoveDelta(double translationDelta)
        {
            m_curtainTranslation = translationDelta;
            m_knobTranslation = translationDelta;
            SetTranslations();
        }

        private void MoveCompleted(bool wasMoved)
        {
            bool wasToggled = false;
            if (wasMoved)
            {
                double halfOfTranslationRange = (m_maxKnobTranslation - m_minKnobTranslation) / 2;
                wasToggled = IsOn ? m_knobTranslation <= halfOfTranslationRange : m_knobTranslation >= halfOfTranslationRange;
            }
            ClearTranslations();
            if (wasToggled)
            {
                Toggle();
            }
            else
            {
                ChangeVisualState(true);
            }
        }

        private void OnToggled()
        {
            Toggled?.Invoke(this, new RoutedEventArgs(null, source: this));
            if(!m_isDragging)
            {
                ChangeVisualState(true);
            }
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnGotFocus(e);
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnLostFocus(e);
        }

        /// <inheritdoc/>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnGotKeyboardFocus(e);
        }

        /// <inheritdoc/>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnLostKeyboardFocus(e);
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            m_isPointerOver = true;
            ChangeVisualState(true);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            m_isPointerOver = false;
            ChangeVisualState(true);
        }

        private void Thumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_isTapping = false;
            if (m_isTapping)
            {
                TapHandler(sender, e);
            }
            base.OnMouseLeftButtonUp(e);
        }

        private void Thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_isTapping = true;
            base.OnMouseLeftButtonDown(e);
        }


        // We need to add this event handler because in the "Vertical Pan" case, the pointer would most likely
        // move out of the control but OnPointerExit() would not be invoked so the control might remain in PointerOver
        // visual state. This handler rectifies this issue by clearing the PointerOver state.
        /// <inheritdoc/>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            // We are checking to make sure dragging has finished before resetting the PointerOver state,
            // because in the "Vertical Pan" case, we get a call to DragCompletedHandler() before
            // OnPointerCaptureLost() unlike the case of "Tap"/"Horizontal Drag" where Thumb::OnPointerReleased()
            // invokes ReleasePointerCapture() so OnPointerCaptureLost() is called before DragCompletedHandler().
            if (!m_isDragging)
            {
                m_isPointerOver = false;
            }
            ChangeVisualState(false);
        }

        // We don't perform any action in OnKeyDown because we wait for the key to be
        // released before performing a Toggle().  However, if we don't handle OnKeyDown,
        // it bubbles up to the parent ScrollViewer and may cause scrolling, which is
        // undesirable.  Therefore, we check to see if we will handle OnKeyUp for this key
        // press, and if so, we set Handled=TRUE for OnKeyDown to stop bubbling this event.
        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled || m_isDragging) goto Cleanup;
            base.OnKeyDown(e);
            m_handledKeyDown = e.Handled = HandlesKey(e.Key);

        Cleanup:
            return;
        }

        /// <inheritdoc/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            Key key = e.Key;
            bool pbHandled = false;
            bool shouldToggleOff = false;
            bool shouldToggleOn = false;
            bool handlesKey = false;
            bool handledKeyDown = false;
            bool isOn = false;

            var flowDirection = FlowDirection;
            bool isLTR = flowDirection == FlowDirection.LeftToRight;

            handlesKey = HandlesKey(key);
            if (handlesKey)
            {
                m_handledKeyDown = false;
            }

            if (handlesKey && handledKeyDown && (!pbHandled && !m_isDragging))
            {
                isOn = IsOn;

                if ((key == Key.Left && isLTR)          // Left toggles us off in LTR
                    || (key == Key.Right && !isLTR)     // Right toggles us off in RTR
                    || key == Key.Down
                    || key == Key.Home)
                {
                    shouldToggleOff = true;
                }
                else if ((key == Key.Right && isLTR)    // Right toggles us on in LTR
                    || (key == Key.Left && !isLTR)      // Left toggles us off in RTL
                    || key == Key.Up
                    || key == Key.End)
                {
                    shouldToggleOn = true;
                }

                if ((!isOn && shouldToggleOn) || (isOn && shouldToggleOff) || key == Key.Space)
                {
                    Toggle();
                    pbHandled = true;
                }
            }
            e.Handled = pbHandled;
        }

        private void DragStartedHandler(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            m_isDragging = true;
            m_wasDragged = false;
            Focus();
            GetTranslations();
            ChangeVisualState(true);
            SetTranslations();
        }

        private void DragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if(e.HorizontalChange != 0)
            {
                m_wasDragged = true;
                MoveDelta(e.HorizontalChange);
            }
        }

        private void DragCompletedHandler(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (e.Canceled)
                return;
            m_isDragging = false;
            MoveCompleted(m_wasDragged);
        }

        private void TapHandler(object sender, RoutedEventArgs e)
        {
            if (e.Handled || (m_isDragging && Math.Abs(m_knobTranslation) > 2))
                return;
            Toggle();
            e.Handled = true;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            double knobWidth = 0;
            double knobBoundsWidth = 0;
            double knobTranslation = 0;
            double curtainBoundsWidth = 0;
            double curtainTranslation = 0;
            double curtainBoundsHeight = 0;
            Rect clipRect = new();
            Thickness knobMarginThickness = new();

            // Set the clip.
            if (m_tpCurtainBounds != null)
            {
                RectangleGeometry spClipRectangleGeometry = new RectangleGeometry();

                curtainBoundsWidth = m_tpCurtainBounds.ActualWidth;

                if (m_tpCurtainClip != null)
                {
                    curtainBoundsHeight = m_tpCurtainBounds.ActualHeight;

                    clipRect.X = clipRect.Y = 0;
                    clipRect.Width = curtainBoundsWidth;
                    clipRect.Height = curtainBoundsHeight;

                    spClipRectangleGeometry.Rect = clipRect;
                    m_tpCurtainClip.Clip = spClipRectangleGeometry;
                }
            }

            bool isOn = IsOn;

            // Compute the knob translation bounds.
            if (m_tpKnob != null && m_tpKnobBounds != null && m_spKnobTransform != null)
            {
                knobTranslation = m_spKnobTransform.X;
                knobBoundsWidth = m_tpKnobBounds.ActualWidth;
                knobWidth = m_tpKnob.ActualWidth;
                knobMarginThickness = m_tpKnob.Margin;

                if (isOn)
                {
                    m_maxKnobTranslation = knobTranslation;
                    m_minKnobTranslation = m_maxKnobTranslation - knobBoundsWidth + knobWidth;
                }
                else
                {
                    m_minKnobTranslation = knobTranslation;
                    m_maxKnobTranslation = m_minKnobTranslation + knobBoundsWidth - knobWidth;
                }

                // Enable the negative margin effects used with the phone version.
                if (knobMarginThickness.Left < 0)
                {
                    m_maxKnobTranslation -= knobMarginThickness.Left;
                }

                if (knobMarginThickness.Right < 0)
                {
                    m_maxKnobTranslation -= knobMarginThickness.Right;
                }
            }

            // Compute the curtain translation bounds.
            if (m_tpCurtainBounds != null && m_spCurtainTransform != null)
            {
                curtainTranslation = m_spCurtainTransform.X;

                if (isOn)
                {
                    m_maxCurtainTranslation = curtainTranslation;
                    m_minCurtainTranslation = m_maxCurtainTranslation - curtainBoundsWidth;
                }
                else
                {
                    m_minCurtainTranslation = curtainTranslation;
                    m_maxCurtainTranslation = m_minCurtainTranslation + curtainBoundsWidth;
                }
            }

            // flow these values into interested parties
            var pTemplateSettingsConcreteNoRef = TemplateSettings;
            if (pTemplateSettingsConcreteNoRef != null)
            {
                pTemplateSettingsConcreteNoRef.KnobOffToOnOffset = m_minKnobTranslation - m_maxKnobTranslation;
                pTemplateSettingsConcreteNoRef.KnobOnToOffOffset = m_maxKnobTranslation - m_minKnobTranslation;

                pTemplateSettingsConcreteNoRef.CurtainOffToOnOffset = m_minCurtainTranslation - m_maxCurtainTranslation;
                pTemplateSettingsConcreteNoRef.CurtainOnToOffOffset = m_maxCurtainTranslation - m_minCurtainTranslation;
            }
        }

        private bool HandlesKey(Key key) => key == Key.Space;

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool bIsEnabled = (bool)e.NewValue;
            if (!bIsEnabled)
            {
                m_isDragging = false;
                m_isPointerOver = false;
            }
            ChangeVisualState(false);
        }

        private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;
            if(!isVisible)
            {
                m_isDragging = false;
                m_isPointerOver = false;
            }
        }

        private void UpdateHeaderPresenterVisibility()
        {
            ConditionallyGetTemplatePartAndUpdateVisibility("HeaderContentPresenter", Header != null || HeaderTemplate != null, m_tpHeaderPresenter);
        }

        private void ConditionallyGetTemplatePartAndUpdateVisibility(string strName, bool visible, UIElement element)
        {
            if (element == null && visible)
            {
                // If element should be visible or is not deferred, then fetch it.
                element = GetTemplateChild(strName) as UIElement;
            }

            // If element was found then set its Visibility - this is behavior consistent with pre-Threshold releases.
            if (element != null)
            {
                element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets an object that provides calculated values that can be referenced as TemplateBinding
        /// sources when defining templates for a <see cref="ToggleSwitch"/> control.
        /// </summary>
        //public ToggleSwitchTemplateSettings TemplateSettings { get; } = new ToggleSwitchTemplateSettings();



        public ToggleSwitchTemplateSettings TemplateSettings
        {
            get { return (ToggleSwitchTemplateSettings)GetValue(TemplateSettingsProperty); }
            private set { SetValue(TemplateSettingsProperty, value); }
        }

        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register("TemplateSettings", typeof(ToggleSwitchTemplateSettings), typeof(ToggleSwitch), new PropertyMetadata(null));



        /// <summary>
        /// Occurs when "On"/"Off" state changes for this <see cref="ToggleSwitch"/>.
        /// </summary>
        public event RoutedEventHandler Toggled;
    }
}
