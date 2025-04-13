﻿using NUnit.Framework;
using NSubstitute;

namespace Utilities.Test.NUnit.VSTest.UnitTests
{
    public class Tests : TestBase
    {
        [Test]
        public void SimpleExample_WithDependency_ResolvesMockedDependency()
        {
            var returnValue = 1;

            var mockedDependency = Substitute.For<ITestDependency>();
            mockedDependency.Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(returnValue);

            factory.RegisterOrReplaceService(mockedDependency);
            var testClass = factory.Create<TestClass>();

            var result = testClass.DoAddition(1, 1);

            Assert.That(returnValue, Is.EqualTo(result));
        }

        public class TestClass
        {
            private readonly ITestDependency dependency;

            public TestClass(ITestDependency testDependency)
            {
                this.dependency = testDependency;
            }

            public int DoAddition(int x, int y)
            {
                return dependency.Add(x, y);
            }
        }

        public interface ITestDependency
        {
            int Add(int x, int y);
        }
    }
}