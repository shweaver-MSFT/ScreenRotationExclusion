using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ScreenRotationExclusion
{
    [Bindable]
    public class ElementOrientationExtensions : DependencyObject
    {
        public enum ElementOrientation
        {
            Auto,
            Fixed,
            Reverse,
            ReverseFixed
        }

        // A dict of top-level UIElements and the FrameworkElement that are being listened for
        private static Dictionary<FrameworkElement, TypedEventHandler<DisplayInformation, object>> _listeningElements =
            new Dictionary<FrameworkElement, TypedEventHandler<DisplayInformation, object>>();

        private const string ElementOrientationPropertyName = "ElementOrientation";
        private const ElementOrientation ElementOrientationDefaultValue = ElementOrientation.Auto;

        public static readonly DependencyProperty ElementOrientationProperty = DependencyProperty.RegisterAttached(
            name: ElementOrientationPropertyName,
            propertyType: typeof(ElementOrientation),
            ownerType: typeof(DependencyObject),
            defaultMetadata: new PropertyMetadata(ElementOrientationDefaultValue, new PropertyChangedCallback(OnElementOrientationChanged))
            );

        public static void SetElementOrientation(DependencyObject element, ElementOrientation value)
        {
            (element as FrameworkElement).SetValue(ElementOrientationProperty, value);
        }

        public static ElementOrientation GetElementOrientation(DependencyObject element)
        {
            return (ElementOrientation)(element as FrameworkElement).GetValue(ElementOrientationProperty);
        }

        private static void OnElementOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            void handleValueChange(FrameworkElement e, ElementOrientation? newValue)
            {
                if (!newValue.HasValue || newValue == ElementOrientationDefaultValue)
                {
                    DeregisterRotationListener(e);
                }

                if (newValue.HasValue && newValue != ElementOrientationDefaultValue)
                {
                    RegisterRotationListener(e);
                }
            }

            handleValueChange(sender as FrameworkElement, args.NewValue as ElementOrientation?);
        }

        private static void RegisterRotationListener(FrameworkElement element)
        {
            if (!_listeningElements.ContainsKey(element))
            {
                void handler(DisplayInformation s, object e) => OnOrientationChanged(element, s.CurrentOrientation);

                var displayInfo = DisplayInformation.GetForCurrentView();
                displayInfo.OrientationChanged += handler;

                _listeningElements.Add(element, handler);

                OnOrientationChanged(element, displayInfo.CurrentOrientation);
            }
        }

        private static void DeregisterRotationListener(FrameworkElement element)
        {
            if (_listeningElements.TryGetValue(element, out TypedEventHandler<DisplayInformation, object> handler))
            {
                var displayInfo = DisplayInformation.GetForCurrentView();
                displayInfo.OrientationChanged -= handler;

                _listeningElements.Remove(element);

                OnOrientationChanged(element, displayInfo.CurrentOrientation);
            }
        }

        private static void OnOrientationChanged(FrameworkElement element, DisplayOrientations displayOrientation)
        {
            var elementOrientation = GetElementOrientation(element);
            int rotationAngle = 0;

            switch (elementOrientation)
            {
                case ElementOrientation.Auto:
                    rotationAngle = 0;
                    break;
                case ElementOrientation.Fixed:
                    switch (displayOrientation)
                    {
                        case DisplayOrientations.Landscape:
                            rotationAngle = 0;
                            break;
                        case DisplayOrientations.Portrait:
                            rotationAngle = 90;
                            break;
                        case DisplayOrientations.LandscapeFlipped:
                            rotationAngle = 180;
                            break;
                        case DisplayOrientations.PortraitFlipped:
                            rotationAngle = 270;
                            break;
                    }
                    break;
                case ElementOrientation.Reverse:
                    rotationAngle = 180;
                    break;
                case ElementOrientation.ReverseFixed:
                    switch (displayOrientation)
                    {
                        case DisplayOrientations.Landscape:
                            rotationAngle = 180;
                            break;
                        case DisplayOrientations.Portrait:
                            rotationAngle = 270;
                            break;
                        case DisplayOrientations.LandscapeFlipped:
                            rotationAngle = 0;
                            break;
                        case DisplayOrientations.PortraitFlipped:
                            rotationAngle = 90;
                            break;
                    }
                    break;
            }

            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = new RotateTransform() { Angle = rotationAngle };
        }
    }

    public class PropertyChangeNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainPageViewModel : PropertyChangeNotifier
    {
        private DisplayOrientations _currentScreenOrientation;
        public DisplayOrientations CurrentScreenOrientation
        {
            get => _currentScreenOrientation;
            set => Set(ref _currentScreenOrientation, value);
        }

        private int _rotationAngle;
        public int RotationAngle
        {
            get => _rotationAngle;
            set => Set(ref _rotationAngle, value);
        }

        public MainPageViewModel()
        {
            DisplayInformation.GetForCurrentView().OrientationChanged += (s, e) => RefreshCurrentScreenOrientation();
            RefreshCurrentScreenOrientation();
        }

        private void RefreshCurrentScreenOrientation()
        {
            CurrentScreenOrientation = DisplayInformation.GetForCurrentView().CurrentOrientation;

            switch (CurrentScreenOrientation)
            {
                case DisplayOrientations.Landscape:
                    RotationAngle = 0;
                    break;
                case DisplayOrientations.Portrait:
                    RotationAngle = 90;
                    break;
                case DisplayOrientations.LandscapeFlipped:
                    RotationAngle = 180;
                    break;
                case DisplayOrientations.PortraitFlipped:
                    RotationAngle = 270;
                    break;
            }
        }
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return Enum.GetName(value.GetType(), value);
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
