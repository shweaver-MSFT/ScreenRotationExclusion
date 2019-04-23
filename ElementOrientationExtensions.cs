using Windows.Graphics.Display;

namespace ScreenRotationExclusion
{
    public static class ElementOrientationExtensions
    {
        public static int GetRotationAngle(this ElementOrientations elementOrientation, OrientationOrigins orientationOrigin = OrientationOrigins.Auto)
        {
            int rotationAngle = 0;
            var displayInfo = DisplayInformation.GetForCurrentView();
            var currentOrientation = displayInfo.CurrentOrientation;

            DisplayOrientations targetOrientation;
            switch (orientationOrigin)
            {
                default:
                case OrientationOrigins.Auto:
                    targetOrientation = currentOrientation;
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
                case ElementOrientations.Reverse:
                    rotationAngle = 0;
                    break;

                case ElementOrientations.Fixed:
                case ElementOrientations.ReverseFixed:
                    if (orientationOrigin == OrientationOrigins.Auto && targetOrientation == currentOrientation)
                    {
                        rotationAngle = 0;
                    }
                    else
                    {
                        rotationAngle = targetOrientation.GetRotationAngle();
                        rotationAngle += currentOrientation.GetRotationAngle();
                    }
                    break;
            }

            switch (elementOrientation)
            {
                case ElementOrientations.Reverse:
                case ElementOrientations.ReverseFixed:
                    rotationAngle -= 180;
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
