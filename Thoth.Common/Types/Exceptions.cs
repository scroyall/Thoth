namespace Thoth;

public class MismatchedTypeException(IType expected, IType actual)
    : Exception($"Type mismatch; expected {expected} not {actual}.");

public class UnresolvedTypeException()
    : Exception();
