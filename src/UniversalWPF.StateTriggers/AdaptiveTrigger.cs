using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalWPF
{
    /// <summary>
    /// Represents a declarative rule that applies visual states based on window properties.
    /// </summary>
    public class AdaptiveTrigger : StateTriggerBase, IWeakEventListener
    {
        public AdaptiveTrigger()
        {
            var window = Application.Current?.MainWindow;
            if (window != null)
                WindowSizeChangedEventManager.AddListener(window, this);
        }


        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            CheckHeight();
            return true;
        }

        private void CheckHeight()
        {
            SetActive(
                Application.Current?.MainWindow?.ActualWidth >= MinWindowWidth &&
                Application.Current?.MainWindow?.ActualHeight >= MinWindowHeight);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AdaptiveTrigger)d).CheckHeight();
        }

        /// <summary>
        /// Gets or sets the minimum window height at which the <see cref="VisualState"/> should be applied.
        /// </summary>
        public double MinWindowHeight
        {
            get { return (double)GetValue(MinWindowHeightProperty); }
            set { SetValue(MinWindowHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWindowHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWindowHeightProperty =
            DependencyProperty.Register(nameof(MinWindowHeight), typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(0d, OnPropertyChanged));


        /// <summary>
        /// Gets or sets the minimum window width at which the <see cref="VisualState"/> should be applied.
        /// </summary>
        public double MinWindowWidth
        {
            get { return (double)GetValue(MinWindowWidthProperty); }
            set { SetValue(MinWindowWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWindowWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWindowWidthProperty =
            DependencyProperty.Register(nameof(MinWindowWidth), typeof(double), typeof(AdaptiveTrigger), new PropertyMetadata(0d, OnPropertyChanged));

        internal class WindowSizeChangedEventManager : WeakEventManager
        {
            private static WindowSizeChangedEventManager CurrentManager
            {
                get
                {
                    Type typeFromHandle = typeof(LostFocusEventManager);
                    WindowSizeChangedEventManager sizeChangedEventManager = (WindowSizeChangedEventManager)WeakEventManager.GetCurrentManager(typeof(WindowSizeChangedEventManager));
                    if (sizeChangedEventManager == null)
                    {
                        sizeChangedEventManager = new WindowSizeChangedEventManager();
                        WeakEventManager.SetCurrentManager(typeFromHandle, sizeChangedEventManager);
                    }
                    return sizeChangedEventManager;
                }
            }

            private WindowSizeChangedEventManager()
            {
            }

            public static void AddListener(DependencyObject source, IWeakEventListener listener)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                if (listener == null)
                {
                    throw new ArgumentNullException("listener");
                }
                CurrentManager.ProtectedAddListener(source, listener);
            }

            public static void RemoveListener(DependencyObject source, IWeakEventListener listener)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                if (listener == null)
                {
                    throw new ArgumentNullException("listener");
                }
                CurrentManager.ProtectedRemoveListener(source, listener);
            }

            public static void AddHandler(DependencyObject source, EventHandler<RoutedEventArgs> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException("handler");
                }
                CurrentManager.ProtectedAddHandler(source, handler);
            }

            public static void RemoveHandler(DependencyObject source, EventHandler<RoutedEventArgs> handler)
            {
                if (handler == null)
                {
                    throw new ArgumentNullException("handler");
                }
                CurrentManager.ProtectedRemoveHandler(source, handler);
            }

            protected override ListenerList NewListenerList()
            {
                return new ListenerList<RoutedEventArgs>();
            }

            protected override void StartListening(object source)
            {
                Window d = source as Window;
                if (d != null)
                {
                    d.SizeChanged += OnLostFocus;
                }
            }

            /// <summary>Stops listening for the <see cref="E:System.Windows.UIElement.LostFocus" /> event on the given source.</summary>
            /// <param name="source">The source object on which to stop listening for <see cref="E:System.Windows.UIElement.LostFocus" />.</param>
            protected override void StopListening(object source)
            {
                Window d = source as Window;
                if (d != null)
                {
                    d.SizeChanged -= OnLostFocus;
                }
            }

            private void OnLostFocus(object sender, EventArgs args)
            {
                DeliverEvent(sender, args);
            }
        }
    }
}
