using System;
using Warbler.Utils.Id;

namespace Tests.Mocks;

public class TestIdProvider : IIdProvider
{
    public SignatureId GetSignatureId()
    {
        return new SignatureId(Guid.Empty);
    }

    public EnvId GetEnvironmentId()
    {
        return new EnvId(Guid.Empty);
    }
}