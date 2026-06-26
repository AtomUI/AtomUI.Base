using AtomUI.Modularity;
using Shouldly;
using Xunit;

namespace AtomUI.Base.Tests.Modularity;

public sealed class ModuleIdTests
{
    [Fact]
    public void FromSerializedValue_ShouldPreserveValue()
    {
        var moduleId = ModuleId.FromSerializedValue("AtomUI.Base.Tests/TestModule");

        moduleId.Value.ShouldBe("AtomUI.Base.Tests/TestModule");
        moduleId.ToString().ShouldBe("AtomUI.Base.Tests/TestModule");
    }
}
