namespace Warbler.Expressions;

public record Signature(
    SignatureId Id,
    WarblerType ReturnType,
    List<WarblerType> Parameters);
