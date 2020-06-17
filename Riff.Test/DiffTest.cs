using Riff.UWP.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Riff.Core.Test
{
    public class DiffTest
    {
        [Fact]
        public void Test()
        {
            var source = new List<int>() { 1, 4, 5, 6, 7 };
            var target = new List<int>() { 3, 4, 5, 6, 7 };

            var diffList = Diff.Compare(source, target);
            Console.WriteLine(diffList.Count);
        }
    }
}
