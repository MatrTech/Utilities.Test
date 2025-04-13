using Matr.Utilities.Test;
using NSubstitute;
using Xunit;

namespace Utilities.Test.XUnit.MTP.UnitTests
{
    public class UnitTest1
    {
        public class XUnitTestExamples : TestBase
        {
            [Fact]
            public void Test1()
            {
                var returnValue = 1;

                var mockedDependency = Substitute.For<ITestDependency>();
                mockedDependency.Add(Arg.Any<int>(), Arg.Any<int>())
                    .Returns(returnValue);

                factory.RegisterOrReplaceService(mockedDependency);
                var testClass = factory.Create<TestClass>();

                var result = testClass.DoAddition(1, 1);

                Assert.Equal(returnValue, result);
            }

            public class TestClass
            {
                private readonly ITestDependency _dependency;

                public TestClass(ITestDependency testDependency)
                {
                    _dependency = testDependency;
                }

                public int DoAddition(int x, int y)
                {
                    return _dependency.Add(x, y);
                }
            }

            public interface ITestDependency
            {
                int Add(int x, int y);
            }
        }
    }
}
