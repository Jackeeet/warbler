namespace Warbler.Utils.General;

internal abstract class MathProvider<T> where T : struct
{
    public abstract T RaiseToPower(T left, T right);
    public abstract T Divide(T left, T right);
    public abstract T Multiply(T left, T right);
    public abstract T Modulo(T left, T right);
    public abstract T Add(T left, T right);
    public abstract T Subtract(T left, T right);
    public abstract T Negate(T number);
}

internal class DoubleMathProvider : MathProvider<double>
{
    public override double RaiseToPower(double left, double right) => Math.Pow(left, right);

    public override double Divide(double left, double right) => left / right;

    public override double Multiply(double left, double right) => left * right;

    public override double Modulo(double left, double right) => left % right;

    public override double Add(double left, double right) => left + right;

    public override double Subtract(double left, double right) => left - right;

    public override double Negate(double number) => -number;
}

internal class IntMathProvider : MathProvider<int>
{
    public override int RaiseToPower(int left, int right) => (int)Math.Pow(left, right);

    public override int Divide(int left, int right) => left / right;

    public override int Multiply(int left, int right) => left * right;

    public override int Modulo(int left, int right) => left % right;

    public override int Add(int left, int right) => left + right;

    public override int Subtract(int left, int right) => left - right;

    public override int Negate(int number) => -number;
}