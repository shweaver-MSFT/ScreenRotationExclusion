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

        private OrientationOrigins _currentOrientationOrigin;
        public OrientationOrigins CurrentOrientationOrigin
        {
            get => _currentOrientationOrigin;
            set => Set(ref _currentOrientationOrigin, value);
        }

        private DisplayInformation _currentDisplayInfo;

        public MainPageViewModel()
        {
            DisplayInformation.DisplayContentsInvalidated += DisplayInformation_DisplayContentsInvalidated;

            PropertyChanged += OnPropertyChanged;

            CurrentOrientationOrigin = OrientationOrigins.Auto;

            RegisterForCurrentDisplay();
            RefreshForCurrentDisplay();
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentOrientationOrigin):
                    RefreshForCurrentDisplay();
                    break;
            }
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
        }
    }
}
