namespace Warbler.Utils;

public class DefaultGuidProvider : IGuidProvider
{
    public Guid Get()
    {
        return Guid.NewGuid();
    }
}