using Autofac.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Matr.Utilities.Test.UnitTests
{
    [TestClass]
    public class GenericFactoryTests
    {
        [TestMethod]
        public void CreateWithoutDependencies_WithoutDependencies_ReturnTestObject()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            var result = factory.Create<TestServiceWithoutDependencies>();

            // Assert
            result.Should().BeOfType<TestServiceWithoutDependencies>();
        }

        [TestMethod]
        public void CreateWithDependencies_DependenciesNotRegistered_Exception()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            Func<TestServiceWithDependencies> func = () => factory.Create<TestServiceWithDependencies>();

            // Assert
            func.Should().Throw<DependencyResolutionException>();
        }

        [TestMethod]
        public void CreateWithDependencies_DependenciesRegistered_TestServiceWithDependenciesObject()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            Mock<ITestDependencyInterface> mock = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock.Object);

            // Act
            var result = factory.Create<TestServiceWithDependencies>();

            // Assert
            result.Should()
                .NotBeNull()
                .And
                .BeOfType<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void CreateTwice_WithoutDependencies_DoesNotThrow()
        {
            GenericFactory factory = new GenericFactory();

            var result1 = factory.Create<TestServiceWithoutDependencies>();
            var result2 = factory.Create<TestServiceWithoutDependencies>();

            result1.Should().BeOfType<TestServiceWithoutDependencies>();
            result2.Should().BeOfType<TestServiceWithoutDependencies>();
        }

        [TestMethod]
        public void CreateTwice_WithDependencies_DoesNotThrow()
        {
            GenericFactory factory = new GenericFactory();

            Mock<ITestDependencyInterface> mock = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(mock.Object);

            var result1 = factory.Create<TestServiceWithDependencies>();
            var result2 = factory.Create<TestServiceWithDependencies>();

            result1.Should().BeOfType<TestServiceWithDependencies>();
            result2.Should().BeOfType<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void RegisterOrReplaceService_ServiceNull_ArgumentNullException()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            ITestDependencyInterface? dependency = null;
            Action func = () => factory.RegisterOrReplaceService(dependency!);
#else
            ITestDependencyInterface dependency = null;
            Action func = () => factory.RegisterOrReplaceService(dependency);
#endif
            // Assert
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisterOrReplaceService_ValidService_DoesNotThrow()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            // Act
            Action func = () => factory.RegisterOrReplaceService(dependency.Object);

            // Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void EmptyDependencies_DependenciesRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            factory.RegisterOrReplaceService(dependency.Object);

            // Act
            factory.EmptyDependencies();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EmptyDependencies_NoDependenciesRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            factory.EmptyDependencies();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRegistered_NotRegisterd_ShouldBeFalse()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRegistered_IsRegistered_ShouldBeTrue()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService(dependency.Object);

            // Act
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void RemoveService_NotRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();

            // Act
            factory.RemoveService<ITestDependencyInterface>();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void RemoveService_DependencyRegistered_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            factory.RegisterOrReplaceService<ITestDependencyInterface>(dependency.Object);

            // Act
            factory.RemoveService<ITestDependencyInterface>();
            bool result = factory.IsRegistered<ITestDependencyInterface>();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void RemoveService_ReRegister_ShouldBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            // Act
            factory.RegisterOrReplaceService(dependency.Object);
            var result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();

            factory.RemoveService<ITestDependencyInterface>();
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeFalse();

            factory.RegisterOrReplaceService(dependency.Object);
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();
        }

        [TestMethod]
        public void RemoveService_DoubleRegistration_ShouldNotBeFound()
        {
            // Arrange
            GenericFactory factory = new GenericFactory();
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            Mock<ITestDependencyInterface> dependency2 = new Mock<ITestDependencyInterface>();

            // Act
            factory.RegisterOrReplaceService(dependency.Object);
            factory.RegisterOrReplaceService(dependency2.Object);

            var result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeTrue();

            factory.RemoveService<ITestDependencyInterface>();
            result = factory.IsRegistered<ITestDependencyInterface>();
            result.Should().BeFalse();
        }

        [TestMethod]
        public void CreateAfterRemove_ShouldNotThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                factory.RegisterOrReplaceService(dependency.Object);
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void CreateAfterRemove_MissingDependency_ShouldThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();

            GenericFactory factory = new GenericFactory();
            factory.RegisterOrReplaceService(dependency.Object);

            _ = factory.Create<TestServiceWithDependencies>();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RemoveService<ITestDependencyInterface>();
                return factory.Create<TestServiceWithDependencies>();
            };

            // Act & Assert
            func.Should().Throw<DependencyResolutionException>();

            factory.RegisterOrReplaceService(dependency.Object);
            _ = factory.Create<TestServiceWithDependencies>();
        }

        [TestMethod]
        public void RemoveAfterCreate_ShouldNotThrow()
        {
            // Arrange
            Mock<ITestDependencyInterface> dependency = new Mock<ITestDependencyInterface>();
            GenericFactory factory = new GenericFactory();

            Func<TestServiceWithDependencies> func = () =>
            {
                factory.RegisterOrReplaceService(dependency.Object);
                var testService = factory.Create<TestServiceWithDependencies>();
                factory.RemoveService<ITestDependencyInterface>();

                return testService;
            };

            // Act & Assert
            func.Should().NotThrow();
        }

        [TestMethod]
        public void GetRegisteredServices_NonRegistered_EmptyList()
        {
            // Arrange
            var factory = new GenericFactory();

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetRegisteredServices_OneRegistered_ListOfOne()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());

            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetRegisteredServices_MultiRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());
            factory.RegisterOrReplaceService(new Mock<ITestDependencyInterface>());
            factory.RegisterOrReplaceService(new Mock<IOtherDependencyInterface>());
            
            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().NotBeEmpty()
                .And.NotHaveCount(1);
        }

        [TestMethod]
        public void FluentRegistration_OneRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplaceService(new TestServiceWithoutDependencies());
            
            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public void FluentRegistration_MultiRegistered_NotBeEmpty()
        {
            // Arrange
            var factory = new GenericFactory();
            factory.RegisterOrReplace(new TestServiceWithoutDependencies())
                .RegisterOrReplace(new Mock<ITestDependencyInterface>())
                .RegisterOrReplace(new Mock<IOtherDependencyInterface>());
            
            // Act
            var result = factory.GetRegisteredServices();

            // Assert
            result.Should().NotBeEmpty()
                .And.NotHaveCount(1);
        }
    }

    public class TestServiceWithoutDependencies { }

    public interface ITestDependencyInterface { }

    public interface IOtherDependencyInterface { }

    public class TestServiceWithDependencies
    {
        public TestServiceWithDependencies(ITestDependencyInterface _)
        {
        }
    }

    public class TestServiceWithOtherDependencies
    {
        public TestServiceWithOtherDependencies(IOtherDependencyInterface _)
        {
        }
    }
}