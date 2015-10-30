using System;
using Hyper.NodeServices;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.Configuration.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hyper.Test
{
    [TestClass]
    public class HyperNodeConfigurationTest
    {
        // TODO: Switch from using CreateAndConfigure to using just the validator by itself. Should do separate set of tests for CreateAndConfigure using the mock config provider.
        // TODO: Also need to organize the tests. All configuration tests should go into a folder because of all of the mock classes and such.
        [TestMethod]
        public void NullConfigurationTest()
        {
            try
            {
                HyperNodeService.CreateAndConfigure(
                    new HyperNodeMockConfigurationProvider(null)
                );

                Assert.Fail("HyperNodeService should throw an exception if the HyperNodeConfigurationProvider returns a null configuration.");
            }
            catch (ArgumentNullException)
            {
                // Success, since we threw an expected exception
            }
        }

        [TestMethod]
        public void EmptyConfigurationTest()
        {
            try
            {
                HyperNodeService.CreateAndConfigure(
                    new HyperNodeMockConfigurationProvider(
                        new HyperNodeConfiguration()
                    )
                );

                Assert.Fail("HyperNodeService should throw an exception indicating that required fields were left blank.");
            }
            catch (HyperNodeConfigurationException)
            {
                // Success, since we threw an expected exception
            }
        }

        [TestMethod]
        public void HyperNodeNameOnlyTest()
        {
            HyperNodeService.CreateAndConfigure(
                new HyperNodeMockConfigurationProvider(
                    new HyperNodeConfiguration
                    {
                        HyperNodeName = "UnitTestNode"
                    }
                )
            );
        }

        [TestMethod]
        public void InvalidTaskIdProviderTypeStringTest()
        {
            const string typeString = "BadType";

            try
            {
                HyperNodeService.CreateAndConfigure(
                    new HyperNodeMockConfigurationProvider(
                        new HyperNodeConfiguration
                        {
                            HyperNodeName = "UnitTestNode",
                            TaskIdProviderType = "BadType"
                        }
                    )
                );

                Assert.Fail("HyperNodeService should throw an exception indicating that the type '" + typeString + "' cannot be parsed into a valid type.");
            }
            catch (HyperNodeConfigurationException)
            {
                // Success, since we threw an expected exception
            }
        }

        [TestMethod]
        public void TaskIdProviderTypeStringDoesNotImplementInterfaceTest()
        {
            const string typeString = "Hyper.Test.HyperNodeMockConfigurationProvider, Hyper.Test";

            try
            {
                HyperNodeService.CreateAndConfigure(
                    new HyperNodeMockConfigurationProvider(
                        new HyperNodeConfiguration
                        {
                            HyperNodeName = "UnitTestNode",
                            TaskIdProviderType = typeString
                        }
                    )
                );

                Assert.Fail("HyperNodeService should throw an exception indicating that the type '" + typeString + "' doesn't implement the correct interface.");
            }
            catch
            {
                // Success, since we threw an expected exception
            }
        }
    }
}
