using AtomUI.Base;
using Shouldly;
using Xunit;

namespace AtomUI.Base.Tests;

public sealed class BaseAssemblyMarkerTests
{
    [Fact]
    public void BaseAssemblyMarker_ShouldExposeAssemblyName()
    {
        BaseAssemblyMarker.AssemblyName.ShouldBe("AtomUI.Base");
    }
}
