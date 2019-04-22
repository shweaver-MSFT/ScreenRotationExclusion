using Windows.Graphics.Display;

namespace ScreenRotationExclusion
{
    public static class ElementOrientationExtensions
    {
        public static int GetRotationAngle(this ElementOrientations elementOrientation, OrientationOrigins orientationOrigin = OrientationOrigins.Auto)
        {
            int rotationAngle = 0;
            var displayInfo = DisplayInformation.GetForCurrentView();

            DisplayOrientations targetOrientation;
            switch (orientationOrigin)
            {
                default:
                case OrientationOrigins.Auto:
                    targetOrientation = displayInfo.CurrentOrientation;
                    break;
                case OrientationOrigins.Native:
                    targetOrientation = displayInfo.NativeOrientation;
                    break;
                case OrientationOrigins.AutoRotationPreference:
                    targetOrientation = DisplayInformation.AutoRotationPreferences;
                    break;
            }

            switch (elementOrientation)
            {
                case ElementOrientations.Auto:
                    rotationAngle = 0;
                    break;
                case ElementOrientations.Fixed:
                    rotationAngle = targetOrientation.GetRotationAngle();
                    break;
                case ElementOrientations.Reverse:
                    rotationAngle = 180;
                    break;
                case ElementOrientations.ReverseFixed:
                    var temp = targetOrientation.GetRotationAngle();
                    rotationAngle = (temp == 0) ? 180 : (temp == 180) ? 0 : 360 - temp;
                    
                    // TODO: Fix for portrait/portraitFlipped

                    break;
            }

            return rotationAngle;
        }

        public static int GetRotationAngle(this DisplayOrientations displayOrientation)
        {
            int rotationAngle = 0;

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

            return rotationAngle;
        }
    }
}
