using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkflowApi.Client.Test.Runner;

namespace WorkflowApi.Client.Test.Tests;

[TestClass]
public sealed class AssemblyInitializer
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        //This call will cause lazy initialization of the TestServiceSet instance
        //It's unnecessary call, but it's here to show that the TestServiceSet
        //is initialized before the first test is run
        var instance = TestServiceSet.Instance;
    }

    [AssemblyCleanup]
    public static Task AssemblyCleanup()
    {
        TestServiceSet.Instance.Dispose();
        return Task.CompletedTask;
    }
}
