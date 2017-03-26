using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warbler.Infrastructure.Chat.Models;

namespace Warbler.Tests
{
    [TestClass]
    public class TestWarblerEntity
    {
        private University FloridaTech { get; set; }
            = new University(1, "FIT", 99F, -99F);
        private University EasternFlorida { get; set; }
            = new University(2, "BCC", 0.0F, 100.0F);

        [TestMethod]
        public void DifferentUniversitiesAreUnequal()
        {
            Assert.IsFalse(FloridaTech.Equals(EasternFlorida));
            Assert.IsFalse(FloridaTech == EasternFlorida);
        }

        [TestMethod]
        public void SameUniversitiesAreEqual()
        {
            Assert.IsTrue(FloridaTech.Equals(FloridaTech));
            Assert.IsTrue(FloridaTech == FloridaTech);
        }
    }
}
