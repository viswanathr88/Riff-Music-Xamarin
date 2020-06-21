using Riff.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Riff.Core.Test
{
    public class ListInterfaceExtensionTest
    {
        [Fact]
        public void IndexOf_NullList_Throw()
        {
            IList<string> l = null;
            Assert.Throws<ArgumentNullException>(() => l.IndexOf(item => item == "blah"));
        }

        [Fact]
        public void IndexOf_EmptyList_ReturnMinusOne()
        {
            IList<string> list = new List<string>();
            Assert.Equal(-1, list.IndexOf(item => item == "blah"));
        }

        [Fact]
        public void IndexOf_NotFoundInList_ReturnMinusOne()
        {
            IList<string> list = new List<string>() { "test1", "test2", "test3", "test5" };
            Assert.Equal(-1, list.IndexOf(item => item == "test4"));
        }

        [Fact]
        public void IndexOf_FirstItemFoundInList_ReturnIndex()
        {
            IList<string> list = new List<string>() { "test1", "test2", "test3", "test5" };
            Assert.Equal(0, list.IndexOf(item => item == "test1"));
        }

        [Fact]
        public void IndexOf_LastItemFoundInList_ReturnIndex()
        {
            IList<string> list = new List<string>() { "test1", "test2", "test3", "test5" };
            Assert.Equal(list.Count - 1, list.IndexOf(item => item == "test5"));
        }
    }
}
