﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniversalWPF
{
    /// <summary>
    /// Represents a container with two views that size and position content in the available space, either side-by-side or top-bottom.
    /// </summary>
    public class TwoPaneView : Control
    {
        private const string c_pane1ScrollViewerName = "PART_Pane1ScrollViewer";
        private const string c_pane2ScrollViewerName = "PART_Pane2ScrollViewer";
        private const string c_columnLeftName   = "PART_ColumnLeft";
        private const string c_columnMiddleName = "PART_ColumnMiddle";
        private const string c_columnRightName  = "PART_ColumnRight";
        private const string c_rowTopName       = "PART_RowTop";
        private const string c_rowMiddleName    = "PART_RowMiddle";
        private const string c_rowBottomName    = "PART_RowBottom";

        private const double c_defaultMinWideModeWidth = 641.0 ;
        private const double c_defaultMinTallModeHeight = 641.0;
        static readonly GridLength c_pane1LengthDefault = new GridLength(1, GridUnitType.Star);
        static readonly GridLength c_pane2LengthDefault = new GridLength(1, GridUnitType.Star);
        private bool m_loaded = false;
        private ViewMode m_currentMode = ViewMode.None;
        ColumnDefinition m_columnLeft;
        ColumnDefinition m_columnMiddle;
        ColumnDefinition m_columnRight;
        RowDefinition m_rowTop;
        RowDefinition m_rowMiddle;
        RowDefinition m_rowBottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoPaneView"/> class.
        /// </summary>
        public TwoPaneView()
        {
            DefaultStyleKey = typeof(TwoPaneView);
            SizeChanged += OnSizeChanged;
            //TODO Listen for Window Changed on the correct Window:
            //TODO: Make weak
            Application.Current.MainWindow.SizeChanged += OnWindowSizeChanged;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            m_loaded = true;
            base.OnApplyTemplate();

            // SetScrollViewerProperties(c_pane1ScrollViewerName, m_pane1LoadedRevoker);
            // SetScrollViewerProperties(c_pane2ScrollViewerName, m_pane2LoadedRevoker);

            m_columnLeft = GetTemplateChild(c_columnLeftName) as ColumnDefinition;
            m_columnMiddle = GetTemplateChild(c_columnMiddleName) as ColumnDefinition;
            m_columnRight = GetTemplateChild(c_columnRightName) as ColumnDefinition;
            m_rowTop = GetTemplateChild(c_rowTopName) as RowDefinition;
            m_rowMiddle = GetTemplateChild(c_rowMiddleName) as RowDefinition;
            m_rowBottom = GetTemplateChild(c_rowBottomName) as RowDefinition;
        }

        private void SetScrollViewerProperties(string scrollViewerName, object revoker)
        {
            throw new NotImplementedException();
        }

        private void OnScrollViewerLoaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMode();
        }
        
        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMode();
        }

        private void UpdateMode()
        {
            // Don't bother running this logic until after we hit OnApplyTemplate.
            if (!m_loaded) return;

            double controlWidth = ActualWidth;
            double controlHeight = ActualHeight;

            ViewMode newMode = (PanePriority == TwoPaneViewPriority.Pane1) ? ViewMode.Pane1Only : ViewMode.Pane2Only;

            // Calculate new mode
            DisplayRegionHelperInfo info = DisplayRegionHelper.GetRegionInfo();
            Rect rcControl = GetControlRect();
            bool isInMultipleRegions = IsInMultipleRegions(info, rcControl);

            if (isInMultipleRegions)
            {
                if (info.Mode == TwoPaneViewMode.Wide)
                {
                    // Regions are laid out horizontally
                    if (WideModeConfiguration != TwoPaneViewWideModeConfiguration.SinglePane)
                    {
                        newMode = WideModeConfiguration == TwoPaneViewWideModeConfiguration.LeftRight ? ViewMode.LeftRight : ViewMode.RightLeft;
                    }
                }
                else if (info.Mode == TwoPaneViewMode.Tall)
                {
                    // Regions are laid out vertically
                    if (TallModeConfiguration != TwoPaneViewTallModeConfiguration.SinglePane)
                    {
                        newMode = (TallModeConfiguration == TwoPaneViewTallModeConfiguration.TopBottom) ? ViewMode.TopBottom : ViewMode.BottomTop;
                    }
                }
            }
            else
            {
                // One region
                if (controlWidth > MinWideModeWidth && WideModeConfiguration != TwoPaneViewWideModeConfiguration.SinglePane)
                {
                    // Split horizontally
                    newMode = (WideModeConfiguration == TwoPaneViewWideModeConfiguration.LeftRight) ? ViewMode.LeftRight : ViewMode.RightLeft;
                }
                else if (controlHeight > MinTallModeHeight && TallModeConfiguration != TwoPaneViewTallModeConfiguration.SinglePane)
                {
                    // Split vertically
                    newMode = (TallModeConfiguration == TwoPaneViewTallModeConfiguration.TopBottom) ? ViewMode.TopBottom : ViewMode.BottomTop;
                }
            }

            // Update row/column sizes (this may need to happen even if the mode doesn't change)
            UpdateRowsColumns(newMode, info, rcControl);

            // Update mode if necessary
            if (newMode != m_currentMode)
            {
                m_currentMode = newMode;

                TwoPaneViewMode newViewMode = TwoPaneViewMode.SinglePane;

                switch (m_currentMode)
                {
                    case ViewMode.Pane1Only: VisualStateManager.GoToState(this, "ViewMode_OneOnly", true); break;
                    case ViewMode.Pane2Only: VisualStateManager.GoToState(this, "ViewMode_TwoOnly", true); break;
                    case ViewMode.LeftRight: VisualStateManager.GoToState(this, "ViewMode_LeftRight", true); newViewMode = TwoPaneViewMode.Wide; break;
                    case ViewMode.RightLeft: VisualStateManager.GoToState(this, "ViewMode_RightLeft", true); newViewMode = TwoPaneViewMode.Wide; break;
                    case ViewMode.TopBottom: VisualStateManager.GoToState(this, "ViewMode_TopBottom", true); newViewMode = TwoPaneViewMode.Tall; break;
                    case ViewMode.BottomTop: VisualStateManager.GoToState(this, "ViewMode_BottomTop", true); newViewMode = TwoPaneViewMode.Tall; break;
                }

                if (newViewMode != Mode)
                {
                    SetValue(ModeProperty, newViewMode);
                    ModeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void UpdateRowsColumns(ViewMode newMode, DisplayRegionHelperInfo info, Rect rcControl)
        {
            if (m_columnLeft != null && m_columnMiddle != null && m_columnRight != null && m_rowTop != null && m_rowMiddle != null && m_rowBottom != null)
            {
                // Reset split lengths
                m_columnMiddle.Width = new GridLength(0, GridUnitType.Pixel);
                m_rowMiddle.Height = new GridLength(0, GridUnitType.Pixel);

                // Set columns lengths
                if (newMode == ViewMode.LeftRight || newMode == ViewMode.RightLeft)
                {
                    m_columnLeft.Width = (newMode == ViewMode.LeftRight) ? Pane1Length : Pane2Length;
                    m_columnRight.Width = (newMode == ViewMode.LeftRight) ? Pane2Length : Pane1Length;
                }
                else
                {
                    m_columnLeft.Width = new GridLength(1, GridUnitType.Star);
                    m_columnRight.Width = new GridLength(0, GridUnitType.Pixel);
                }

                // Set row lengths
                if (newMode == ViewMode.TopBottom || newMode == ViewMode.BottomTop)
                {
                    m_rowTop.Height = (newMode == ViewMode.TopBottom) ? Pane1Length : Pane2Length;
                    m_rowBottom.Height = (newMode == ViewMode.TopBottom) ? Pane2Length : Pane1Length;
                }
                else
                {
                    m_rowTop.Height = new GridLength(1, GridUnitType.Star);
                    m_rowBottom.Height = new GridLength(0, GridUnitType.Pixel);
                }

                // Handle regions
                if (IsInMultipleRegions(info, rcControl) && newMode != ViewMode.Pane1Only && newMode != ViewMode.Pane2Only)
                {
                    Rect rc1 = info.Regions[0];
                    Rect rc2 = info.Regions[1];
                    Rect rcWindow = DisplayRegionHelper.WindowRect;

                    if (info.Mode == TwoPaneViewMode.Wide)
                    {
                        m_columnMiddle.Width = new GridLength( rc2.X - rc1.Width, GridUnitType.Pixel);

                        m_columnLeft.Width = new GridLength(rc1.Width - rcControl.X , GridUnitType.Pixel );
                        m_columnRight.Width = new GridLength(rc2.Width - ((rcWindow.Width - rcControl.Width) - rcControl.X) , GridUnitType.Pixel );
                    }
                    else
                    {
                        m_rowMiddle.Height = new GridLength(rc2.Y - rc1.Height, GridUnitType.Pixel );

                        m_rowTop.Height = new GridLength(rc1.Height - rcControl.Y , GridUnitType.Pixel);
                        m_rowBottom.Height = new GridLength(rc2.Height - ((rcWindow.Height - rcControl.Height) - rcControl.Y) , GridUnitType.Pixel);
                    }
                }
            }
        }

        private Rect GetControlRect()
        {
            var root = GetRoot();
            if (root == null)
                return Rect.Empty;
            var rect = System.Windows.Media.VisualTreeHelper.GetContentBounds(root);
            return rect;
        }

        private UIElement GetRoot()
        {
            UIElement result = this;
            while(true)
            {
                var parent = VisualTreeHelper.GetParent(result) as UIElement;
                if (parent == null)
                    break;
                result = parent;
            }
            return result;
        }

        private bool IsInMultipleRegions(DisplayRegionHelperInfo info, Rect rcControl)
        {
            return false; // TODO
        }

        /// <summary>
        /// Gets or sets the minimum width at which panes are shown in wide mode.
        /// </summary>
        /// <value>The minimum width at which panes are shown in wide mode.</value>
        public double MinWideModeWidth
        {
            get { return (double)GetValue(MinWideModeWidthProperty); }
            set { SetValue(MinWideModeWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWideModeWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWideModeWidthProperty =
            DependencyProperty.Register("MinWideModeWidth", typeof(double), typeof(TwoPaneView), new PropertyMetadata(c_defaultMinWideModeWidth, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the minimum height at which panes are shown in tall mode.
        /// </summary>
        /// <value>The minimum height at which panes are shown in tall mode.</value>
        public double MinTallModeHeight
        {
            get { return (double)GetValue(MinTallModeHeightProperty); }
            set { SetValue(MinTallModeHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinTallModeHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinTallModeHeightProperty =
            DependencyProperty.Register(nameof(MinTallModeHeight), typeof(double), typeof(TwoPaneView), new PropertyMetadata(c_defaultMinTallModeHeight, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the content of pane 1.
        /// </summary>
        /// <value>The content of pane 1.</value>
        public UIElement Pane1
        {
            get { return (UIElement)GetValue(Pane1Property); }
            set { SetValue(Pane1Property, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Pane1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Pane1Property =
            DependencyProperty.Register(nameof(Pane1), typeof(UIElement), typeof(TwoPaneView), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content of pane 2.
        /// </summary>
        /// <value>The content of pane 2.</value>
        public UIElement Pane2
        {
            get { return (UIElement)GetValue(Pane2Property); }
            set { SetValue(Pane2Property, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Pane2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Pane2Property =
            DependencyProperty.Register(nameof(Pane2), typeof(UIElement), typeof(TwoPaneView), new PropertyMetadata(null));

        /// <summary>
        /// Gets the calculated width (in wide mode) or height (in tall mode) of pane 1, or sets the GridLength value of pane 1.
        /// </summary>
        /// <value>The GridLength that represents the width or height of the pane.</value>
        public GridLength Pane1Length
        {
            get { return (GridLength)GetValue(Pane1LengthProperty); }
            set { SetValue(Pane1LengthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Pane1Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Pane1LengthProperty =
            DependencyProperty.Register(nameof(Pane1Length), typeof(GridLength), typeof(TwoPaneView), new PropertyMetadata(c_pane1LengthDefault, OnPropertyChanged));

        /// <summary>
        /// Gets the calculated width (in wide mode) or height (in tall mode) of pane 2, or sets the GridLength value of pane 2.
        /// </summary>
        /// <value>The GridLength that represents the width or height of the pane.</value>
        public GridLength Pane2Length
        {
            get { return (GridLength)GetValue(Pane2LengthProperty); }
            set { SetValue(Pane2LengthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Pane2Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Pane2LengthProperty =
            DependencyProperty.Register(nameof(Pane2Length), typeof(GridLength), typeof(TwoPaneView), new PropertyMetadata(c_pane2LengthDefault, OnPropertyChanged));

        /// <summary>
        /// Gets a value that indicates how panes are shown.
        /// </summary>
        /// <value>An enumeration value that indicates how panes are shown.</value>
        public TwoPaneViewMode Mode
        {
            get { return (TwoPaneViewMode)GetValue(ModeProperty); }
        }

        /// <summary>
        /// Identifies the <see cref="Mode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(TwoPaneViewMode), typeof(TwoPaneView), new PropertyMetadata(TwoPaneViewMode.Wide));

        /// <summary>
        /// Gets or sets a value that indicates which pane has priority.
        /// </summary>
        /// <value>An enumeration value that indicates which pane has priority.</value>
        public TwoPaneViewPriority PanePriority
        {
            get { return (TwoPaneViewPriority)GetValue(PanePriorityProperty); }
            set { SetValue(PanePriorityProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PanePriority"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PanePriorityProperty =
            DependencyProperty.Register(nameof(PanePriority), typeof(TwoPaneViewPriority), typeof(TwoPaneView), new PropertyMetadata(TwoPaneViewPriority.Pane1, OnPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates how panes are shown in wide mode.
        /// </summary>
        /// <value>An enumeration value that indicates how panes are shown in wide mode.</value>
        public TwoPaneViewWideModeConfiguration WideModeConfiguration
        {
            get { return (TwoPaneViewWideModeConfiguration )GetValue(WideModeConfigurationProperty); }
            set { SetValue(WideModeConfigurationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="WideModeConfiguration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WideModeConfigurationProperty =
            DependencyProperty.Register(nameof(WideModeConfiguration), typeof(TwoPaneViewWideModeConfiguration ), typeof(TwoPaneView), new PropertyMetadata(TwoPaneViewWideModeConfiguration.LeftRight, OnPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates how panes are shown in tall mode.
        /// </summary>
        /// <value>An enumeration value that indicates how panes are shown in tall mode.</value>
        public TwoPaneViewTallModeConfiguration TallModeConfiguration
        {
            get { return (TwoPaneViewTallModeConfiguration)GetValue(TallModeConfigurationProperty); }
            set { SetValue(TallModeConfigurationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TallModeConfiguration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TallModeConfigurationProperty =
            DependencyProperty.Register(nameof(TallModeConfiguration), typeof(TwoPaneViewTallModeConfiguration), typeof(TwoPaneView), new PropertyMetadata(TwoPaneViewTallModeConfiguration.TopBottom, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var property = args.Property;
            var ctrl = (TwoPaneView)d;
            // Clamp property values -- early return if the values were clamped as we'll come back with the new value.
            if (property == MinWideModeWidthProperty || property == MinTallModeHeightProperty)
            {
                var value = (double)args.NewValue;
                var clampedValue = Math.Max(0.0, value);
                if (clampedValue != value)
                {
                    ctrl.SetValue(property, clampedValue);
                    return;
                }
            }

            if (property == PanePriorityProperty
                || property == Pane1LengthProperty
                || property == Pane2LengthProperty
                || property == WideModeConfigurationProperty
                || property == TallModeConfigurationProperty
                || property == MinWideModeWidthProperty
                || property == MinTallModeHeightProperty)
            {
                ctrl.UpdateMode();
            }
        }

        /// <summary>
        /// Occurs when the <see cref="Mode"/> of the <see cref="TwoPaneView"/> has changed.
        /// </summary>
        public event EventHandler ModeChanged;
    }
}
