using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ScreenRotationExclusion
{

    [Bindable]
    public class ElementOrientation : DependencyObject
    {
        // A dict of top-level UIElements and the FrameworkElement that are being listened for
        private static Dictionary<FrameworkElement, TypedEventHandler<DisplayInformation, object>> _listeningElements =
            new Dictionary<FrameworkElement, TypedEventHandler<DisplayInformation, object>>();

        private const string ElementOrientationPropertyName = "ElementOrientation";
        private const ElementOrientations ElementOrientationDefaultValue = ElementOrientations.Auto;

        public static readonly DependencyProperty ElementOrientationProperty = DependencyProperty.RegisterAttached(
            name: ElementOrientationPropertyName,
            propertyType: typeof(ElementOrientations),
            ownerType: typeof(DependencyObject),
            defaultMetadata: new PropertyMetadata(ElementOrientationDefaultValue, new PropertyChangedCallback(OnElementOrientationChanged))
            );

        static ElementOrientation()
        {
            DisplayInformation.DisplayContentsInvalidated += DisplayInformation_DisplayContentsInvalidated;
        }

        private static void DisplayInformation_DisplayContentsInvalidated(DisplayInformation sender, object args)
        {
            // Potentially needed to listen for when the view is moved to another display
        }

        public static void SetElementOrientation(DependencyObject element, ElementOrientations value)
        {
            (element as FrameworkElement).SetValue(ElementOrientationProperty, value);
        }

        public static ElementOrientations GetElementOrientation(DependencyObject element)
        {
            return (ElementOrientations)(element as FrameworkElement).GetValue(ElementOrientationProperty);
        }

        private static void OnElementOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            void handleValueChange(FrameworkElement e, ElementOrientations? newValue)
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

            handleValueChange(sender as FrameworkElement, args.NewValue as ElementOrientations?);
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
                case ElementOrientations.Auto:
                    rotationAngle = 0;
                    break;
                case ElementOrientations.Fixed:
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
                case ElementOrientations.Reverse:
                    rotationAngle = 180;
                    break;
                case ElementOrientations.ReverseFixed:
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

            bool adjustForNative = true;
            if (adjustForNative)
            {
                var preferredOrientation = DisplayInformation.AutoRotationPreferences;
                var nativeOrientation = DisplayInformation.GetForCurrentView().NativeOrientation;

            }

            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = new RotateTransform() { Angle = rotationAngle };
        }
    }
}
