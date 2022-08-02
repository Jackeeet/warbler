using System;
using Warbler.Utils;

namespace Tests.Mocks;

public class TestGuidProvider : IGuidProvider
{
    public Guid Get()
    {
        return Guid.Empty;
    }
}