using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class BooleanExpressionParser
    {
        public static bool Evaluate(string text) => ParseBooleanExpression(text).Compile().Invoke();

        public static Expression<Func<bool>> ParseBooleanExpression(string text) => Lambda.Parse(text) as Expression<Func<bool>>;

        public static Parser<LambdaExpression> Lambda => BooleanExpression.End().Select(body => Expression.Lambda<Func<bool>>(body));

        #region Parse boolean operations

        public static Parser<Expression> BooleanExpressionInParanthesis => from lparen in Parse.Char('(')
                                                                           from booleanExpression in BooleanExpression
                                                                           from rparen in Parse.Char(')')
                                                                           select booleanExpression;

        public static Parser<Expression> BooleanExpression => UnaryBooleanExpression.XOr(BinaryBooleanExpression).XOr(BooleanExpressionInParanthesis);

        public static Parser<Expression> UnaryBooleanExpression => from not in Not
                                                                   from expression in BooleanValue
                                                                   select Expression.MakeUnary(ExpressionType.Not, expression, typeof(bool));

        public static Parser<ExpressionType> Not => Parse.String("not").Token().Return(ExpressionType.Not);

        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(BinaryBooleanOperator, BooleanValue, Expression.MakeBinary);
        public static Parser<ExpressionType> BinaryBooleanOperator => And.XOr(Or).XOr(XOr);
        public static Parser<ExpressionType> And => Parse.String("and").Token().Return(ExpressionType.And);
        public static Parser<ExpressionType> Or => Parse.String("or").Token().Return(ExpressionType.OrElse);
        public static Parser<ExpressionType> XOr => Parse.String("xor").Token().Return(ExpressionType.ExclusiveOr); // not supported in EF?!

        #endregion Parse boolean operations

        #region Define boolean values: <boolean value> ::= <true|false>

        public static Parser<Expression> BooleanValueInParentheses =>
            from lparen in Parse.Char('(')
            from booleanValue in BooleanValue
            from rparen in Parse.Char(')')
            select booleanValue;

        public static Parser<Expression> BooleanValue => True.XOr(False).XOr(BooleanValueInParentheses);
        public static Parser<Expression> True = Parse.String("true").Token().Return(Expression.Constant(true));
        public static Parser<Expression> False = Parse.String("false").Token().Return(Expression.Constant(false));

        #endregion Define boolean values: <boolean value> ::= <true|false>
    }
}