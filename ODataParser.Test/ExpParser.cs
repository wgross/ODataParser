using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class BooleanExpressionParser
    {
        #region Parse boolean constants: <boolean constant> ::= <true|false>

        private static Parser<Expression> True = Parse.String("true").Token().Return(Expression.Constant(true));
        private static Parser<Expression> False = Parse.String("false").Token().Return(Expression.Constant(false));
        private static Parser<Expression> BooleanConstant => True.XOr(False);

        private static Parser<Expression> BooleanConstantInParenthesis => from left in Parse.Char('(').Token()
                                                                          from booleanConst in Parse.Ref(() => AnyBooleanConstant)
                                                                          from right in Parse.Char(')').Token()
                                                                          select booleanConst;

        /// <summary>
        /// A constant might in parenthesis or not.
        /// </summary>
        public static Parser<Expression> AnyBooleanConstant => BooleanConstantInParenthesis.XOr(BooleanConstant); // must be XOR

        #endregion Parse boolean constants: <boolean constant> ::= <true|false>

        #region Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        private static Parser<ExpressionType> Not => Parse.String("not").Token().Return(ExpressionType.Not);

        /// <summary>
        /// An unary operator may have a constant or another expressions as a parameter
        /// </summary>
        public static Parser<Expression> UnaryBooleanExpression => from not in Not
                                                                   from value in AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)) // Ref: delay access to avoid circular dependency
                                                                   select Expression.MakeUnary(ExpressionType.Not, value, typeof(bool));

        #endregion Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        #region Parse boolean binary expression <boolean binary expression> ::= <boolean constant|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        private static Parser<ExpressionType> And => Parse.String("and").Token().Return(ExpressionType.And);
        private static Parser<ExpressionType> Or => Parse.String("or").Token().Return(ExpressionType.OrElse);
        private static Parser<ExpressionType> XOr => Parse.String("xor").Token().Return(ExpressionType.ExclusiveOr);

        public static Parser<ExpressionType> BinaryBooleanOperator => And.XOr(Or).XOr(XOr);

        /// <summary>
        /// A binary bolean expression receives a constant or another boolean expression as a parameter
        /// </summary>
        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(BinaryBooleanOperator, AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary);

        #endregion Parse boolean binary expression <boolean binary expression> ::= <boolean constant|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        #region Generalize unary and binary boolean expression: <boolean expression> ::= <unary boolean expression|binary boolean expression>

        private static Parser<Expression> BooleanExpression => UnaryBooleanExpression.XOr(BinaryBooleanExpression);

        private static Parser<Expression> BooleanExpressionInParenthesis => from left in Parse.Char('(').Token()
                                                                            from booleanExpr in Parse.Ref(() => AnyBooleanExpression) // Ref: delay access to avoid circular dependency
                                                                            from right in Parse.Char(')').Token()
                                                                            select booleanExpr;

        /// <summary>
        /// A boolean expression might be in parenthesis or not.
        /// </summary>
        public static Parser<Expression> AnyBooleanExpression => BooleanExpressionInParenthesis.XOr(BooleanExpression); // must be XOR

        #endregion Generalize unary and binary boolean expression: <boolean expression> ::= <unary boolean expression|binary boolean expression>

        /// <summary>
        /// Parses the given boolean expression text and evaluates the result.
        /// </summary>
        /// <param name="text">bolle exprsssion text</param>
        /// <returns></returns>
        public static bool Evaluate(string text) => Evaluate((Expression<Func<bool>>)ParsePredicate.Parse(text));

        private static bool Evaluate(Expression<Func<bool>> expression) => Evaluate(expression.Compile());

        private static bool Evaluate(Func<bool> predicate) => predicate();

        /// <summary>
        /// A prediate body may consist of just a constant, a single expression, or a set of expressions linked with a binary operator
        /// </summary>
        private static Parser<LambdaExpression> ParsePredicate => AnyBooleanConstant.End()
            .Or(AnyBooleanExpression.End())
            .Or(Parse.ChainOperator(BinaryBooleanOperator, AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary))
            .Select(body => Expression.Lambda<Func<bool>>(body));
    }
}