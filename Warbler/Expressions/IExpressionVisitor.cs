namespace Warbler.Expressions;
public interface IExpressionVisitor<out T>
{
	T VisitUnaryExpression(UnaryExpression expression);
	T VisitBinaryExpression(BinaryExpression expression);
	T VisitTernaryExpression(TernaryExpression expression);
	T VisitLiteralExpression(LiteralExpression expression);
	T VisitVariableDeclarationExpression(VariableDeclarationExpression expression);
	T VisitVariableExpression(VariableExpression expression);
	T VisitAssignmentExpression(AssignmentExpression expression);
}

