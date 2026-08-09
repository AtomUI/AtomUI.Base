using AtomUI.Modularity;
using Shouldly;
using Xunit;

namespace AtomUI.Foundation.Tests.Modularity;

public sealed class ModuleIdTests
{
    [Fact]
    public void FromSerializedValue_ShouldPreserveValue()
    {
        var moduleId = ModuleId.FromSerializedValue("AtomUI.Foundation.Tests/TestModule");

        moduleId.Value.ShouldBe("AtomUI.Foundation.Tests/TestModule");
        moduleId.ToString().ShouldBe("AtomUI.Foundation.Tests/TestModule");
    }
}
