using Riff.UWP.Storage;
using Windows.UI.Xaml;

namespace Riff.UWP
{
    /// <summary>
    /// Represents different Windows 10 device families
    /// </summary>
    public enum DeviceFamily
    {
        Unidentified,
        Desktop,
        Mobile,
        Xbox,
        Holographic,
        IoT,
        Team,
    }

    public static class Device
    {
        static Device()
        {
            Family = RecognizeDeviceFamily(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
        }
        public static DeviceFamily Family { get; }

        public static void UpdateTheme(Theme appTheme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                UpdateContentTheme(appTheme);
            }
            else
            {
                UpdateAppTheme(appTheme);
            }
        }

        private static void UpdateAppTheme(Theme appTheme)
        {
            switch (appTheme)
            {
                case Theme.Dark:
                    Application.Current.RequestedTheme = ApplicationTheme.Dark;
                    break;
                case Theme.Light:
                    Application.Current.RequestedTheme = ApplicationTheme.Light;
                    break;
                default:
                    break;
            }
        }

        private static void UpdateContentTheme(Theme appTheme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                var elementTheme = ElementTheme.Default;
                switch (appTheme)
                {
                    case Theme.Default:
                        elementTheme = ElementTheme.Default;
                        break;
                    case Theme.Light:
                        elementTheme = ElementTheme.Light;
                        break;
                    case Theme.Dark:
                        elementTheme = ElementTheme.Dark;
                        break;
                }

                frameworkElement.RequestedTheme = elementTheme;
            }
        }

        private static DeviceFamily RecognizeDeviceFamily(string deviceFamily)
        {
            switch (deviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFamily.Mobile;
                case "Windows.Desktop":
                    return DeviceFamily.Desktop;
                case "Windows.Xbox":
                    return DeviceFamily.Xbox;
                case "Windows.Holographic":
                    return DeviceFamily.Holographic;
                case "Windows.IoT":
                    return DeviceFamily.IoT;
                case "Windows.Team":
                    return DeviceFamily.Team;
                default:
                    return DeviceFamily.Unidentified;
            }
        }
    }
}
