using Riff.UWP.Converters;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Xunit;

namespace Riff.UWP.Test.UI
{
    public sealed class GridColumnVisibilityConverterTest
    {
        private readonly GridColumnVisibilityConverter converter = new GridColumnVisibilityConverter();

        [Fact]
        public void Convert_NullValue_Throw()
        {
            Assert.ThrowsAny<Exception>(() => converter.Convert(null, typeof(GridLength), string.Empty, string.Empty));
        }

        [Fact]
        public void Convert_NonBooleanValue_Throw()
        {
            Assert.ThrowsAny<Exception>(() => converter.Convert(30, typeof(GridLength), string.Empty, string.Empty));
        }

        [Fact]
        public void Convert_NullParameter_Throw()
        {
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), null, string.Empty));
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), string.Empty, string.Empty));
        }

        [Fact]
        public void Convert_NonStringAsParameter_Throw()
        {
            Assert.ThrowsAny<ArgumentException>(() => converter.Convert(true, typeof(GridLength), 10, string.Empty));
            Assert.ThrowsAny<ArgumentException>(() => converter.Convert(true, typeof(GridLength), 20.0, string.Empty));
            Assert.ThrowsAny<ArgumentException>(() => converter.Convert(true, typeof(GridLength), true, string.Empty));
            Assert.ThrowsAny<ArgumentException>(() => converter.Convert(true, typeof(GridLength), 20.0f, string.Empty));
            Assert.ThrowsAny<ArgumentException>(() => converter.Convert(true, typeof(GridLength), new List<string>(), string.Empty));
        }

        [Fact]
        public void Convert_RandomStringAsParameter_Throw()
        {
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), "test", string.Empty));
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), "double", string.Empty));
        }

        [Fact]
        public void Convert_FalseValueWithAnyParamater_VerifyGridLength()
        {
            Assert.Equal(new GridLength(0), converter.Convert(false, typeof(GridLength), null, string.Empty));
            Assert.Equal(new GridLength(0), converter.Convert(false, typeof(GridLength), string.Empty, string.Empty));
            Assert.Equal(new GridLength(0), converter.Convert(false, typeof(GridLength), "AUTO", string.Empty));
            Assert.Equal(new GridLength(0), converter.Convert(false, typeof(GridLength), "auto", string.Empty));
            Assert.Equal(new GridLength(0), converter.Convert(false, typeof(GridLength), "2*", string.Empty));
        }

        [Fact]
        public void Convert_AutoParamaterCaps_VerifyGridLength()
        {
            Assert.Equal(new GridLength(1, GridUnitType.Auto), converter.Convert(true, typeof(GridLength), "AUTO", string.Empty));
            Assert.Equal(new GridLength(1, GridUnitType.Auto), converter.Convert(true, typeof(GridLength), "auto", string.Empty));
            Assert.Equal(new GridLength(1, GridUnitType.Auto), converter.Convert(true, typeof(GridLength), "Auto", string.Empty));
        }

        [Fact]
        public void Convert_StarParameterWrongFactor_Throw()
        {
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), "s*", string.Empty));
            Assert.ThrowsAny<Exception>(() => converter.Convert(true, typeof(GridLength), "true*", string.Empty));
        }

        [Fact]
        public void Convert_StarParameterValidFactor_VerifyGridLength()
        {
            Assert.Equal(new GridLength(1, GridUnitType.Star), converter.Convert(true, typeof(GridLength), "*", string.Empty));
            Assert.Equal(new GridLength(3, GridUnitType.Star), converter.Convert(true, typeof(GridLength), "3*", string.Empty));
        }

        [Fact]
        public void Convert_PixelParameter_VerifyGridLength()
        {
            Assert.Equal(new GridLength(2), converter.Convert(true, typeof(GridLength), "2.0", string.Empty));
            Assert.Equal(new GridLength(4), converter.Convert(true, typeof(GridLength), "4", string.Empty));
        }
    }
}
