using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jem.CommonLibrary22.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RectIntersectsRect()
        {
            var rect = new CRect(100, 100, 100, 100);
            Assert.IsTrue(new CRect(100, 100, 100, 100).IntersectsWith(rect));
            Assert.IsTrue(new CRect(100, 100, 101, 100).IntersectsWith(rect));
            Assert.IsTrue(new CRect(99, 100, 101, 100).IntersectsWith(rect));
            Assert.IsTrue(new CRect(100, 100, 100, 101).IntersectsWith(rect));
            Assert.IsTrue(new CRect(100, 99, 100, 101).IntersectsWith(rect));
        }

        [TestMethod]
        public void RectInsideRect()
        {
            var rect = new CRect(100, 100, 100, 100);
            Assert.IsTrue(new CRect(100, 100, 100, 100).Inside(rect));
            Assert.IsFalse(new CRect(100, 100, 101, 100).Inside(rect));
            Assert.IsFalse(new CRect(99, 100, 101, 100).Inside(rect));
            Assert.IsFalse(new CRect(100, 100, 100, 101).Inside(rect));
            Assert.IsFalse(new CRect(100, 99, 100, 101).Inside(rect));
        }
    }
}