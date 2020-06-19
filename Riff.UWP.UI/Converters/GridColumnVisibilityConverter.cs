using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Riff.UWP.Converters
{
    public sealed class GridColumnVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            GridLength gridLength;
            if (value is bool flag)
            {
                if (flag)
                {
                    if (parameter is string gridLengthParam)
                    {
                        if (string.Compare(gridLengthParam, "auto", true) == 0)
                        {
                            gridLength = GridLength.Auto;
                        }
                        else if (gridLengthParam.EndsWith("*"))
                        {
                            string gridLengthValueString = gridLengthParam.Remove(gridLengthParam.Length - 1, 1);
                            double factor = 1;
                            if (!string.IsNullOrEmpty(gridLengthValueString) && !double.TryParse(gridLengthValueString, out factor))
                            {
                                throw new ArgumentException("Invalid factor provided for star grid length");
                            }

                            gridLength = new GridLength(factor, GridUnitType.Star);
                        }
                        else
                        {
                            double pixel = 0;
                            if (!double.TryParse(gridLengthParam, out pixel))
                            {
                                throw new ArgumentException("Invalid value provided for GridLength");
                            }

                            gridLength = new GridLength(pixel);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Parameter type should be string");
                    }
                }
                else
                {
                    gridLength = new GridLength(0);
                }
            }
            else
            {
                throw new InvalidCastException("Target type is not boolean");
            }

            return gridLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
