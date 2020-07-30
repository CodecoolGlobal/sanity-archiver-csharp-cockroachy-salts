using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SanityArchiver.DesktopUI.Converters
{
    [ValueConversion(typeof(DirectoryItemType), typeof(BitmapImage))]
    class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var image = "Images/folder-closed.png";

            Debug.Assert(value != null, nameof(value) + " != null");
            if ((DirectoryItemType) value == DirectoryItemType.Drive)
            {
                image = "Images/drive.png";
            }

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}