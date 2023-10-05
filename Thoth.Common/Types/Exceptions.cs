namespace Thoth;

public class MismatchedTypeException(object expected, object actual)
    : Exception($"Type mismatch; expected {expected} not {actual}.");

public class UnresolvedTypeException()
    : Exception();

public class UndefinedTypeException(string id)
    : Exception($"Type '{id}' is not defined.");

public class InvalidParameterCountException(int expected, int actual)
    : Exception($"Parameter count mismatch; expected {expected} not {actual}.");
