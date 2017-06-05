using Parser;
using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class BooleanExpressions
    {
        #region Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        /// <summary>
        /// An unary operator may have a constant or another expressions as a parameter
        /// </summary>
        public static Parser<Expression> UnaryBooleanExpression => from not in Operators.Not
                                                                   from value in ScalarValues.AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)) // Ref: delay access to avoid circular dependency
                                                                   select Expression.MakeUnary(ExpressionType.Not, value, typeof(bool));

        #endregion Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        #region Parse boolean binary expression <boolean binary expression> ::= <boolean constant|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        /// <summary>
        /// A binary bolean expression receives a constant or another boolean expression as a parameter
        /// </summary>
        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(Operators.BinaryBoolean, ScalarValues.AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary);

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
        virtual public bool Evaluate(string text) => Evaluate((Expression<Func<bool>>)ParsePredicate.Parse(text));

        private bool Evaluate(Expression<Func<bool>> expression) => Evaluate(expression.Compile());

        private bool Evaluate(Func<bool> predicate) => predicate();

        /// <summary>
        /// A prediate body may consist of just a constant, a single expression, or a set of expressions linked with a binary operator
        /// </summary>
        private Parser<LambdaExpression> ParsePredicate => ScalarValues.AnyBooleanConstant.End()
            .Or(AnyBooleanExpression.End())
            .Or(Parse.ChainOperator(Operators.BinaryBoolean, ScalarValues.AnyBooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary).End())
            .Select(body => Expression.Lambda<Func<bool>>(body));
    }
}