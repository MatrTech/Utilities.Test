using Matr.Utilities.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Utilities.Test.MSTest.MTP.UnitTests
{
    [TestClass]
    public sealed class Test1 : TestBase
    {
        [TestMethod]
        public void TestMethod1()
        {
            var returnValue = 1;

            var mockedDependency = Substitute.For<ITestDependency>();
            mockedDependency.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(returnValue);

            _factory.RegisterOrReplaceService(mockedDependency);
            var testClass = _factory.Create<TestClass>();

            var result = testClass.DoAddition(1, 1);

            Assert.AreEqual(returnValue, result);

        }

        public class TestClass
        {
            private readonly ITestDependency _someDependency;

            public TestClass(ITestDependency someDependency) => _someDependency = someDependency;
            public int DoAddition(int x, int y) => _someDependency.Add(x, y);
        }

        public interface ITestDependency
        {
            int Add(int x, int y);
        }
    }


}