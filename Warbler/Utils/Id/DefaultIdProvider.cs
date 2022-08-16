namespace Warbler.Utils.Id;

public class DefaultIdProvider : IIdProvider
{
    public EnvId GetEnvironmentId()
    {
        return new EnvId(Guid.NewGuid());
    }

    public SignatureId GetSignatureId()
    {
        return new SignatureId(Guid.NewGuid());
    }
}