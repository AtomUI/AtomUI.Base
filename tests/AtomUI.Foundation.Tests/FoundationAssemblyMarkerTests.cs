using AtomUI.Foundation;
using Shouldly;
using Xunit;

namespace AtomUI.Foundation.Tests;

public sealed class FoundationAssemblyMarkerTests
{
    [Fact]
    public void FoundationAssemblyMarker_ShouldExposeAssemblyName()
    {
        FoundationAssemblyMarker.AssemblyName.ShouldBe("AtomUI.Foundation");
    }
}
