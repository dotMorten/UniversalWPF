using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace UniversalWPF
{
    /// <summary>
    /// Represents a control that can be used to display and edit numbers.
    /// </summary>
    /// <remarks>
    /// This control supports validation, increment stepping, and computing inline calculations of basic equations such as multiplication, division, addition, and subtraction.
    /// </remarks>
    public class NumberBox : Control
    {
        private const string c_numberBoxDownButtonName = "DownSpinButton";
        private const string c_numberBoxUpButtonName = "UpSpinButton";
        private const string c_numberBoxTextBoxName = "InputBox";
        private const string c_numberBoxPopupButtonName = "PopupButton";
        private const string c_numberBoxPopupName = "UpDownPopup";
        private const string c_numberBoxPopupDownButtonName = "PopupDownSpinButton";
        private const string c_numberBoxPopupUpButtonName = "PopupUpSpinButton";
        private const string c_numberBoxPopupContentRootName = "PopupContentRoot";
        private const double c_popupShadowDepth = 16.0;
        private const string c_numberBoxPopupShadowDepthName = "NumberBoxPopupShadowDepth";

        private TextBox m_textBox;
        private Popup m_popup;
        private bool m_textUpdating;
        private bool m_valueUpdating;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberBox"/> class.
        /// </summary>
        public NumberBox()
        {
            DefaultStyleKey = typeof(NumberBox);
            NumberFormatter = GetRegionalSettingsAwareDecimalFormatter();
        }

        private DecimalFormatter GetRegionalSettingsAwareDecimalFormatter()
        {
            DecimalFormatter formatter = new DecimalFormatter(CultureInfo.CurrentCulture);
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 0;
            return formatter;
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            if (GetTemplateChild(c_numberBoxDownButtonName) is RepeatButton spinDown)
            {
                spinDown.Click += OnSpinDownClick;
            }
            if (GetTemplateChild(c_numberBoxUpButtonName) is RepeatButton spinUp)
            {
                spinUp.Click += OnSpinUpClick;
            }
            m_textBox = GetTemplateChild(c_numberBoxTextBoxName) as TextBox;
            if (m_textBox != null)
            {
                m_textBox.PreviewKeyDown += OnNumberBoxKeyDown;
                m_textBox.KeyUp += OnNumberBoxKeyUp;
                m_textBox.GotFocus += (s, e) => _OnGotFocus(e);
                m_textBox.LostFocus += (s, e) => _OnLostFocus(e);
            }
            m_popup = GetTemplateChild(c_numberBoxPopupName) as Popup;
            if (GetTemplateChild(c_numberBoxPopupDownButtonName) is RepeatButton spinDown2)
            {
                spinDown2.Click += OnSpinDownClick;
            }
            if (GetTemplateChild(c_numberBoxPopupUpButtonName) is RepeatButton spinUp2)
            {
                spinUp2.Click += OnSpinUpClick;
            }
            UpdateSpinButtonPlacement();
            UpdateSpinButtonEnabled();
            if (ReadLocalValue(ValueProperty) == null && ReadLocalValue(TextProperty) != null)
            {
                UpdateValueToText();
            }
            else
            {
                UpdateTextToValue();
            }
            base.OnApplyTemplate();
        }

        private void OnValuePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            // This handler may change Value; don't send extra events in that case.
            if (!m_valueUpdating)
            {
                var oldValue = (double)args.OldValue;
                m_valueUpdating = true;

                CoerceValue();

                var newValue = Value;
                if (newValue != oldValue && !(double.IsNaN(newValue) && double.IsNaN(oldValue)))
                {
                    ValueChanged?.Invoke(this, new NumberBoxValueChangedEventArgs(oldValue, newValue));
                }

                UpdateTextToValue();
                UpdateSpinButtonEnabled();
                m_valueUpdating = false;
            }
        }
        void OnMinimumPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            CoerceMaximum();
            CoerceValue();

            UpdateSpinButtonEnabled();
        }

        void OnMaximumPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            CoerceMinimum();
            CoerceValue();

            UpdateSpinButtonEnabled();
        }

        void OnSmallChangePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateSpinButtonEnabled();
        }

        void OnIsWrapEnabledPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateSpinButtonEnabled();
        }

        void OnNumberFormatterPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            // Update text with new formatting
            UpdateTextToValue();
        }
        void OnSpinButtonPlacementModePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateSpinButtonPlacement();
        }

        void OnTextPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (!m_textUpdating)
            {
                UpdateValueToText();
            }
        }

        void UpdateValueToText()
        {
            if (m_textBox != null)
            {
                m_textBox.Text = Text;
                ValidateInput();
            }
        }

        void OnValidationModePropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            ValidateInput();
            UpdateSpinButtonEnabled();
        }

        /// <inheritdoc/>
        void _OnGotFocus(RoutedEventArgs e)
        {
            // When the control receives focus, select the text
            if (m_textBox != null)
            {
                m_textBox.SelectAll();
            }

            if (SpinButtonPlacementMode == NumberBoxSpinButtonPlacementMode.Compact)
            {
                if (m_popup != null)
                {
                    //m_popup.Visibility = Visibility.Visible;
                    m_popup.IsOpen = true;
                    m_popup.Placement = PlacementMode.Right;
                    m_popup.PlacementTarget = m_textBox;
                }
            }
            base.OnGotFocus(e);
        }
        /// <inheritdoc/>
        void _OnLostFocus(RoutedEventArgs e)
        {
            ValidateInput();
            if (m_popup != null)
            {
                //m_popup.Visibility = Visibility.Collapsed;
                m_popup.IsOpen = false;
            }
            base.OnLostFocus(e);
        }

        void CoerceMinimum()
        {
            var max = Maximum;
            if (Minimum > max)
            {
                Minimum = max;
            }
        }

        void CoerceMaximum()
        {
            var min = Minimum;
            if (Maximum < min)
            {
                Maximum = min;
            }
        }

        void CoerceValue()
        {
            // Validate that the value is in bounds
            var value = Value;
            if (!double.IsNaN(value) && !IsInBounds(value) && ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
            {
                // Coerce value to be within range
                var max = Maximum;
                if (value > max)
                {
                    Value = max;
                }
                else
                {
                    Value = Minimum;
                }
            }
        }

        void ValidateInput()
        {
            // Validate the content of the inner textbox
            if (m_textBox != null)
            {
                var text = m_textBox.Text.Trim();

                // Handles empty TextBox case, set text to current value
                if (string.IsNullOrEmpty(text))
                {
                    Value = double.NaN;
                }
                else
                {
                    // Setting NumberFormatter to something that isn't an INumberParser will throw an exception, so this should be safe
                    var numberParser = NumberFormatter as INumberParser;                    
                    double? value = AcceptsExpression ? NumberBoxParser.Compute(text, numberParser) : numberParser.ParseDouble(text);

                    if (!value.HasValue)
                    {
                        if (ValidationMode == NumberBoxValidationMode.InvalidInputOverwritten)
                        {
                            // Override text to current value
                            UpdateTextToValue();
                        }
                    }
                    else
                    {
                        if (value.Value == Value)
                        {
                            // Even if the value hasn't changed, we still want to update the text (e.g. Value is 3, user types 1 + 2, we want to replace the text with 3)
                            UpdateTextToValue();
                        }
                        else
                        {
                            Value = value.Value;
                        }
                    }
                }
            }
        }
        private void OnSpinDownClick(object sender, RoutedEventArgs e) => StepValue(-SmallChange);
        private void OnSpinUpClick(object sender, RoutedEventArgs e) => StepValue(SmallChange);

        private void OnNumberBoxKeyDown(object sender, KeyEventArgs args)
        {
            // Handle these on key down so that we get repeat behavior.
            switch (args.Key)
            {
                case Key.Up:
                    StepValue(SmallChange);
                    args.Handled = true;
                    break;

                case Key.Down:
                    StepValue(-SmallChange);
                    args.Handled = true;
                    break;

                case Key.PageUp:
                    StepValue(LargeChange);
                    args.Handled = true;
                    break;

                case Key.PageDown:
                    StepValue(-LargeChange);
                    args.Handled = true;
                    break;
            }
        }

        private void OnNumberBoxKeyUp(object sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Enter:
                    ValidateInput();
                    args.Handled = true;
                    break;

                case Key.Escape:
                    UpdateTextToValue();
                    args.Handled = true;
                    break;
            }
        }
        
        /// <inheritdoc/>
        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            if (m_textBox != null)
            {
                if (m_textBox.IsFocused)
                {
                    var delta = args.Delta;
                    if (delta > 0)
                    {
                        StepValue(SmallChange);
                    }
                    else if (delta < 0)
                    {
                        StepValue(-SmallChange);
                    }
                    // Only set as handled when we actually changed our state.
                    args.Handled = true;
                }
            }
            base.OnMouseWheel(args);
        }
        void StepValue(double change)
        {
            // Before adjusting the value, validate the contents of the textbox so we don't override it.
            ValidateInput();

            var newVal = Value;
            if (!double.IsNaN(newVal))
            {
                newVal += change;

                if (IsWrapEnabled)
                {
                    var max = Maximum;
                    var min = Minimum;

                    if (newVal > max)
                    {
                        newVal = min;
                    }
                    else if (newVal < min)
                    {
                        newVal = max;
                    }
                }

                Value = newVal;
            }
        }
        // Updates TextBox.Text with the formatted Value
        private void UpdateTextToValue()
        {
            if (m_textBox != null)
            {
                var newText = string.Empty;

                var value = Value;
                if (!double.IsNaN(value))
                {
                    // Rounding the value here will prevent displaying digits caused by floating point imprecision.
                    newText = NumberFormatter.FormatDouble(Math.Round(value, 12));
                }

                m_textBox.Text = newText;

                m_textUpdating = true;
                Text = newText;

                // This places the caret at the end of the text.
                m_textBox.Select(newText.Length, 0);
                m_textUpdating = false;

            }
        }
        void UpdateSpinButtonPlacement()
        {
            var spinButtonMode = SpinButtonPlacementMode;

            if (spinButtonMode == NumberBoxSpinButtonPlacementMode.Inline)
            {
                VisualStateManager.GoToState(this, "SpinButtonsVisible", false);
            }
            else if (spinButtonMode == NumberBoxSpinButtonPlacementMode.Compact)
            {
                VisualStateManager.GoToState(this, "SpinButtonsPopup", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "SpinButtonsCollapsed", false);
            }
        }

        void UpdateSpinButtonEnabled()
        {
            var value = Value;
            bool isUpButtonEnabled = false;
            bool isDownButtonEnabled = false;

            if (!double.IsNaN(value))
            {
                if (IsWrapEnabled || ValidationMode != NumberBoxValidationMode.InvalidInputOverwritten)
                {
                    // If wrapping is enabled, or invalid values are allowed, then the buttons should be enabled
                    isUpButtonEnabled = true;
                    isDownButtonEnabled = true;
                }
                else
                {
                    if (value < Maximum)
                    {
                        isUpButtonEnabled = true;
                    }
                    if (value > Minimum)
                    {
                        isDownButtonEnabled = true;
                    }
                }
            }

            VisualStateManager.GoToState(this, isUpButtonEnabled ? "UpSpinButtonEnabled" : "UpSpinButtonDisabled", false);
            VisualStateManager.GoToState(this, isDownButtonEnabled ? "DownSpinButtonEnabled" : "DownSpinButtonDisabled", false);
        }

        bool IsInBounds(double value)
        {
            return (value >= Minimum && value <= Maximum);
        }

        /// <summary>
        /// Occurs after the user triggers evaluation of new input by pressing the Enter key, clicking a spin button, or by changing focus.
        /// </summary>
        public event EventHandler<NumberBoxValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Gets or sets the numerical minimum for <see cref="Value"/>.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Maximum"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumberBox), new PropertyMetadata(double.MinValue, (d, e) => ((NumberBox)d).OnMinimumPropertyChanged(e)));

        /// <summary>
        /// Gets or sets the numerical maximum for <see cref="Value"/>.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Maximum"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumberBox), new PropertyMetadata(double.MaxValue, (d, e) => ((NumberBox)d).OnMaximumPropertyChanged(e)));

        /// <summary>
        /// Gets or sets the numeric value of a <see cref="NumberBox"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="Text"/> exists to faciliate getting the string representation of this property's numeric contents without the need for type conversion. 
        /// Note the content of this property will overwrite the content of Text in initial set up. After the initial set up, changes to one will be 
        /// progrogated to the other, but consistently making programmatic changes through this property helps avoid any conceptual misunderstanding 
        /// that NumberBox will accept non-numeric characters through the Text property.
        /// </remarks>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumberBox), new PropertyMetadata(double.NaN, (d,e) => ((NumberBox)d).OnValuePropertyChanged(e)));

        /// <summary>
        /// Gets or sets the value that is added to or subtracted from Value when a small change is made, such as with an arrow key or scrolling.
        /// </summary>
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(NumberBox), new PropertyMetadata(1d, (d, e) => ((NumberBox)d).OnSmallChangePropertyChanged(e)));

        /// <summary>
        /// Gets or sets the value that is added to or subtracted from Value when a large change is made, such as with the PageUP and PageDown keys.
        /// </summary>
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(NumberBox), new PropertyMetadata(10d));

        /// <summary>
        /// Gets or sets the string type representation of the Value property.
        /// </summary>
        /// <remarks>
        /// This property exists to faciliate getting the string representation of Value 's numeric contents without the need for type conversion.
        /// Note the content of Value will overwrite the content of this property in initial set up. After the initial set up, changes to one will
        /// be progrogated to the other, but consistently making programmatic changes through Value helps avoid any conceptual misunderstanding 
        /// that NumberBox will accept non-numeric characters through this property.
        /// </remarks>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NumberBox), new PropertyMetadata(null, (d, e) => ((NumberBox)d).OnTextPropertyChanged(e)));

        /// <summary>
        /// Gets or sets the content for the control's header.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(NumberBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the content of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(NumberBox), new PropertyMetadata(null));


        /*
        public Flyout SelectionFlyout
        {
            get { return (FlyoutBase)GetValue(SelectionFlyoutProperty); }
            set { SetValue(SelectionFlyoutProperty, value); }
        }

        public static readonly DependencyProperty SelectionFlyoutProperty =
            DependencyProperty.Register("SelectionFlyout", typeof(FlyoutBase), typeof(NumberBox), new PropertyMetadata(null));
            */

        /// <summary>
        /// Gets or sets the selection highlight color
        /// </summary>
        public SolidColorBrush SelectionHighlightColor
        {
            get { return (SolidColorBrush)GetValue(SelectionHighlightColorProperty); }
            set { SetValue(SelectionHighlightColorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectionHighlightColor"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectionHighlightColorProperty =
            DependencyProperty.Register("SelectionHighlightColor", typeof(SolidColorBrush), typeof(NumberBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets content that is shown below the control. The content should provide guidance about the input expected by the control.
        /// </summary>
        public object Description
        {
            get { return (object)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Description"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(object), typeof(NumberBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the input validation behavior to invoke when invalid input is entered.
        /// </summary>
        public NumberBoxValidationMode ValidationMode
        {
            get { return (NumberBoxValidationMode)GetValue(ValidationModeProperty); }
            set { SetValue(ValidationModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ValidationMode"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValidationModeProperty =
            DependencyProperty.Register("ValidationMode", typeof(NumberBoxValidationMode), typeof(NumberBox), new PropertyMetadata(NumberBoxValidationMode.InvalidInputOverwritten, (d, e) => ((NumberBox)d).OnValidationModePropertyChanged(e)));

        /// <summary>
        /// Gets or sets a value that indicates the placement of buttons used to increment or decrement the Value property.
        /// </summary>
        public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
        {
            get { return (NumberBoxSpinButtonPlacementMode)GetValue(SpinButtonPlacementModeProperty); }
            set { SetValue(SpinButtonPlacementModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SpinButtonPlacementMode"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty SpinButtonPlacementModeProperty =
            DependencyProperty.Register("SpinButtonPlacementMode", typeof(NumberBoxSpinButtonPlacementMode), typeof(NumberBox), new PropertyMetadata(NumberBoxSpinButtonPlacementMode.Hidden, (d, e) => ((NumberBox)d).OnSpinButtonPlacementModePropertyChanged(e)));

        /// <summary>
        /// Toggles whether line breaking occurs if a line of text extends beyond the available width of the control.
        /// </summary>
        public bool IsWrapEnabled
        {
            get { return (bool)GetValue(IsWrapEnabledProperty); }
            set { SetValue(IsWrapEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsWrapEnabled"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsWrapEnabledProperty =
            DependencyProperty.Register("IsWrapEnabled", typeof(bool), typeof(NumberBox), new PropertyMetadata(false, (d, e) => ((NumberBox)d).OnIsWrapEnabledPropertyChanged(e)));

        /// <summary>
        /// Toggles whether the control will accept and evaluate a basic formulaic expression entered as input.
        /// </summary>
        /// <remarks>
        /// NumberBox uses infix notation to evaluate expressions. In order of precedence, the allowable operators are: ^ * / + -. 
        /// Note that parentheses can be used to override precedence rules.
        /// </remarks>
        public bool AcceptsExpression
        {
            get { return (bool)GetValue(AcceptsExpressionProperty); }
            set { SetValue(AcceptsExpressionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AcceptsExpression"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty AcceptsExpressionProperty =
            DependencyProperty.Register("AcceptsExpression", typeof(bool), typeof(NumberBox), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the object used to specify the formatting of <see cref="Value"/>.
        /// </summary>
        public INumberFormatter2 NumberFormatter
        {
            get { return (INumberFormatter2)GetValue(NumberFormatterProperty); }
            set { SetValue(NumberFormatterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="NumberFormatter"/> Dependency Property
        /// </summary>
        public static readonly DependencyProperty NumberFormatterProperty =
            DependencyProperty.Register(nameof(NumberFormatter), typeof(INumberFormatter2), typeof(NumberBox), new PropertyMetadata(null, (d, e) => ((NumberBox)d).OnNumberFormatterPropertyChanged(e)));

    }

    /// <summary>
    /// Provides event data for the <see cref="NumberBox.ValueChanged"/> event.
    /// </summary>
    public class NumberBoxValueChangedEventArgs : EventArgs
    {
        internal NumberBoxValueChangedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Contains the old <see cref="NumberBox.Value"/> being replaced in a <see cref="NumberBox"/>.
        /// </summary>
        public double OldValue { get;  }

        /// <summary>
        /// Contains the new <see cref="NumberBox.Value"/> to be set for a <see cref="NumberBox"/>.
        /// </summary>
        public double NewValue { get; }
    }

    /// <summary>
    /// Preconfigured input validation behavior invoked when invalid input is entered.
    /// </summary>
    public enum NumberBoxValidationMode
    {
        /// <summary>
        /// Invalid input will be overwritten
        /// </summary>
        InvalidInputOverwritten = 0,
        /// <summary>
        /// Disabled
        /// </summary>
        Disabled = 1
    };

    /// <summary>
    /// Placement configuration for spin buttons which are used to increment or decrement the Value of a NumberBox.
    /// </summary>
    public enum NumberBoxSpinButtonPlacementMode
    {
        /// <summary>
        /// Hidden
        /// </summary>
        Hidden = 0,

        /// <summary>
        /// Compact
        /// </summary>
        Compact = 1,

        /// <summary>
        /// Inline
        /// </summary>
        Inline = 2
    };
}
