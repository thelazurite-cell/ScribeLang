namespace Churva.Interpreter.BluePrints
{
    public enum MultiOperator
    {
        ArithmeticalAddition = 7,
        ArithmeticalSubtraction = 8,
        ArithmeticalMultiplication = 9,
        ArithmeticalDivision = 10,
        ArithmeticalModulo = 11,
        BitwiseAnd = 12,
        BitwiseOr = 13,
        BitwiseXor = 14,
        BitwiseRightShift = 15,
        BitwiseLeftShift = 16,
        Or = 17,
        And = 18,
        LessThan = 19,
        GreaterThan = 20,
        EqualTo = 21,
        LessThanOrEqualTo = 22,
        GreaterThanOrEqualTo = 23,
        Contains = 24,
        StartsWith = 25,
        EndsWith = 26,
        CatchError = 27,
        ClassAccessor= 28,
        EvaluateStart = 29, 
        EvaluateEnd = 30,
        GroupStart = 31,
        GroupEnd = 32
    }
}