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

        #region ElementOrientation DependencyProperty
        private const string ElementOrientationPropertyName = "ElementOrientation";
        private const ElementOrientations ElementOrientationDefaultValue = ElementOrientations.Auto;

        public static readonly DependencyProperty ElementOrientationProperty = DependencyProperty.RegisterAttached(
            name: ElementOrientationPropertyName,
            propertyType: typeof(ElementOrientations),
            ownerType: typeof(DependencyObject),
            defaultMetadata: new PropertyMetadata(ElementOrientationDefaultValue, new PropertyChangedCallback(OnElementOrientationChanged))
            );

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
        #endregion

        #region OrientationOrigin DependencyProperty
        private const string OrientationOriginPropertyName = "OrientationOrigin";

        private const OrientationOrigins OrientationOriginDefaultValue = OrientationOrigins.Auto;

        public static readonly DependencyProperty OrientationOriginProperty = DependencyProperty.RegisterAttached(
            name: OrientationOriginPropertyName,
            propertyType: typeof(OrientationOrigins),
            ownerType: typeof(DependencyObject),
            defaultMetadata: new PropertyMetadata(OrientationOriginDefaultValue, new PropertyChangedCallback(OnOrientationOriginChanged))
            );

        public static void SetOrientationOrigin(DependencyObject element, OrientationOrigins value)
        {
            (element as FrameworkElement).SetValue(OrientationOriginProperty, value);
        }

        public static OrientationOrigins GetOrientationOrigin(DependencyObject element)
        {
            return (OrientationOrigins)(element as FrameworkElement).GetValue(OrientationOriginProperty);
        }

        private static void OnOrientationOriginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            OnOrientationChanged(sender as FrameworkElement, DisplayInformation.GetForCurrentView().CurrentOrientation);
        }
        #endregion

        private static void RegisterRotationListener(FrameworkElement element)
        {
            if (!_listeningElements.ContainsKey(element))
            {
                void handler(DisplayInformation s, object e) => OnOrientationChanged(element, s.CurrentOrientation);

                var displayInfo = DisplayInformation.GetForCurrentView();
                displayInfo.OrientationChanged += handler;

                element.Unloaded += Element_Unloaded;
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

                element.Unloaded -= Element_Unloaded;
                _listeningElements.Remove(element);

                OnOrientationChanged(element, displayInfo.CurrentOrientation);
            }
        }

        private static void OnOrientationChanged(FrameworkElement element, DisplayOrientations displayOrientation)
        {
            var elementOrientation = GetElementOrientation(element);
            var orientationOrigin = GetOrientationOrigin(element);

            int rotationAngle = elementOrientation.GetRotationAngle(orientationOrigin);

            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = new RotateTransform() { Angle = rotationAngle };
        }

        private static void Element_Unloaded(object sender, RoutedEventArgs e)
        {
            DeregisterRotationListener(sender as FrameworkElement);
        }
    }
}
