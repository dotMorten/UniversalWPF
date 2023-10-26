using System.Windows;
using System.Windows.Controls;

namespace UniversalWPF
{
    public partial class ToggleSwitch : Control
    {
        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        /// <value>The header content for the <see cref="ToggleSwitch"/>.</value>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null, (s, e) => ((ToggleSwitch)s).OnHeaderPropertyChanged(e)));

        private void OnHeaderPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateHeaderPresenterVisibility();
            //OnHeaderChangedProtected(e.OldValue, e.NewValue);
        }


        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's header.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/> used to display the control's header.</value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null, (s, e) => ((ToggleSwitch)s).OnHeaderTemplatePropertyChanged(e)));

        private void OnHeaderTemplatePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateHeaderPresenterVisibility();
        }


        /// <summary>
        /// Gets or sets a value that declares whether the state of the <see cref="ToggleSwitch"/> is "On".
        /// </summary>
        /// <remarks>
        /// "On" state uses the template from <see cref="OnContentTemplate"/>. "Off" state uses the template from <see cref="OffContentTemplate"/>.</remarks>
        /// <value><c>true</c> if the state is "On"; <c>false</c> if the state is "Off".</value>
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }


        /// <summary>
        /// Identifies the <see cref="IsOn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(false, (s, e) => ((ToggleSwitch)s).OnIsOnPropertyChanged(e)));

        private void OnIsOnPropertyChanged(DependencyPropertyChangedEventArgs e) => OnToggled();


        /// <summary>
        /// Provides the object content that should be displayed using the <see cref="OffContentTemplate"/> 
        /// when this <see cref="ToggleSwitch"/> has state of "Off".
        /// </summary>
        /// <remarks>
        /// The object content. In some cases this is a string, in other cases it is a single element that provides a root
        /// for further composition content. Probably the most common "set" usage is to place a binding here.
        /// </remarks>
        public object OffContent
        {
            get { return (object)GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OffContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register(nameof(OffContent), typeof(object), typeof(ToggleSwitch), new PropertyMetadata("Off"));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's content while in "Off" state.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/> that displays the control's content while in "Off" state.</value>
        public DataTemplate OffContentTemplate
        {
            get { return (DataTemplate)GetValue(OffContentTemplateProperty); }
            set { SetValue(OffContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OffContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffContentTemplateProperty =
            DependencyProperty.Register(nameof(OffContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Provides the object content that should be displayed using the <see cref="OnContentTemplate"/> when
        /// this <see cref="ToggleSwitch"/> has state of "On".
        /// </summary>
        /// <value>The object content. In some cases this is a string, in other cases it is a single element that
        /// provides a root for further composition content.
        /// Probably the most common "set" usage is to place a binding here.</value>
        public object OnContent
        {
            get { return (object)GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OnContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register(nameof(OnContent), typeof(object), typeof(ToggleSwitch), new PropertyMetadata("On"));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's content while in "On" state.
        /// </summary>
        /// <value>The <see cref="DataTemplate"/> that displays the control's content while in "On" state.</value>
        public DataTemplate OnContentTemplate
        {
            get { return (DataTemplate)GetValue(OnContentTemplateProperty); }
            set { SetValue(OnContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OnContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnContentTemplateProperty =
            DependencyProperty.Register(nameof(OnContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(Thickness), typeof(ToggleSwitch), new PropertyMetadata(new Thickness(2)));
    }
}
