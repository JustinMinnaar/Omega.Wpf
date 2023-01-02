namespace Omega.WpfModels1.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FodyRaisesPropertyChanged()
        {
            bool working = false;
            var p = new NamedModel { Name = "Bart" };
            p.PropertyChanged += (sender, e) => { working = true; };

            Assert.IsFalse(working);
            p.Name  = "Bart Simpson";
            Assert.IsTrue(working);
        }
    }
}