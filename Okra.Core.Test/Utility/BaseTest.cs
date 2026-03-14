using Xunit.Abstractions;
using Okra.Core.Common;

namespace Okra.Core.Test.Utility;

public abstract class BaseTest
{
    protected readonly ITestOutputHelper XUnitLogger;

    protected BaseTest(ITestOutputHelper xUnitLogger)
    {
        XUnitLogger = xUnitLogger;
    }
}