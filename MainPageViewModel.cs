using Windows.Graphics.Display;

namespace ScreenRotationExclusion
{
    public class MainPageViewModel : PropertyChangeNotifier
    {
        private DisplayOrientations _nativeDisplayOrientation;
        public DisplayOrientations NativeDisplayOrientation
        {
            get => _nativeDisplayOrientation;
            set => Set(ref _nativeDisplayOrientation, value);
        }

        private DisplayOrientations _preferredDisplayOrientation;
        public DisplayOrientations PreferredDisplayOrientation
        {
            get => _preferredDisplayOrientation;
            set => Set(ref _preferredDisplayOrientation, value);
        }

        private DisplayOrientations _currentDisplayOrientation;
        public DisplayOrientations CurrentDisplayOrientation
        {
            get => _currentDisplayOrientation;
            set => Set(ref _currentDisplayOrientation, value);
        }

        //private int _rotationAngle;
        //public int RotationAngle
        //{
        //    get => _rotationAngle;
        //    set => Set(ref _rotationAngle, value);
        //}

        private DisplayInformation _currentDisplayInfo;

        public MainPageViewModel()
        {
            DisplayInformation.DisplayContentsInvalidated += DisplayInformation_DisplayContentsInvalidated;

            RegisterForCurrentDisplay();
            RefreshForCurrentDisplay();
        }

        private void DisplayInformation_DisplayContentsInvalidated(DisplayInformation sender, object args)
        {
            DeregisterForCurrentDisplay();
            RegisterForCurrentDisplay();

            RefreshForCurrentDisplay();
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            RefreshForCurrentDisplay();
        }

        private void DeregisterForCurrentDisplay()
        {
            _currentDisplayInfo.OrientationChanged -= DisplayInformation_OrientationChanged;
            _currentDisplayInfo = null;
        }

        private void RegisterForCurrentDisplay()
        {
            _currentDisplayInfo = DisplayInformation.GetForCurrentView();
            _currentDisplayInfo.OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private void RefreshForCurrentDisplay()
        {
            PreferredDisplayOrientation = DisplayInformation.AutoRotationPreferences;

            NativeDisplayOrientation = _currentDisplayInfo.NativeOrientation;
            CurrentDisplayOrientation = _currentDisplayInfo.CurrentOrientation;

            //switch (CurrentDisplayOrientation)
            //{
            //    case DisplayOrientations.Landscape:
            //        RotationAngle = 0;
            //        break;
            //    case DisplayOrientations.Portrait:
            //        RotationAngle = 90;
            //        break;
            //    case DisplayOrientations.LandscapeFlipped:
            //        RotationAngle = 180;
            //        break;
            //    case DisplayOrientations.PortraitFlipped:
            //        RotationAngle = 270;
            //        break;
            //}
        }
    }
}
