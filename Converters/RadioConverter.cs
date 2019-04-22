using System;
using Windows.UI.Xaml.Data;

namespace ScreenRotationExclusion.Converters
{
    /// <summary>
    /// http://blog.jerrynixon.com/2014/12/lets-code-data-binding-to-radio-button.html
    /// </summary>
    public class RadioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToInt32(value).Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }
}
