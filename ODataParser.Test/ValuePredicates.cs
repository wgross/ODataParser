using Parser;
using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class ValuePredicates
    {
        #region Parse a boolean value : <boolean value> ::= <boolean constant|comparision expression>

        //public static Parser<Expression> BooleanValue = ValueComparisons.AnyComparisionExpression
        //    .Or(Parse.ChainOperator(Operators.ComparisionOperators, Parse.Ref(() => ValueComparisons.AnyComparisionExpression), Expression.MakeBinary));
        public static Parser<Expression> BooleanValue = ScalarValues.BooleanConstant.Or(Comparisions.ComparisionExpression).Or(Parse.LetterOrDigit.Many().Select(t => Expression.Constant(t)));

        #endregion Parse a boolean value : <boolean value> ::= <boolean constant|comparision expression>

        #region Parse boolean unary expression <boolean unary expression> ::= not <boolean value|boolean expression>

        /// <summary>
        /// An unary operator may have a constant or another expressions as a parameter
        /// </summary>
        public static Parser<Expression> UnaryBooleanExpression => from not in Operators.Not
                                                                   from value in BooleanValue.Or(Parse.Ref(() => AnyBooleanExpression)) // Ref: delay access to avoid circular dependency
                                                                   select Expression.MakeUnary(ExpressionType.Not, Inspect(value), typeof(bool));

        private static Expression Inspect(Expression unaryExpression)
        {
            return unaryExpression;
        }

        #endregion Parse boolean unary expression <boolean unary expression> ::= not <boolean value|boolean expression>

        #region Parse boolean binary expression <boolean binary expression> ::= <boolean value|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        public static Parser<ExpressionType> BinaryBooleanOperator => Operators.And.XOr(Operators.Or).XOr(Operators.XOr);

        /// <summary>
        /// A binary bolean expression receives a constant or another boolean expression as a parameter
        /// </summary>
        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(BinaryBooleanOperator, BooleanValue.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary);

        #endregion Parse boolean binary expression <boolean binary expression> ::= <boolean value|boolean expression> <and|or|xor> <boolean constant|boolean expression>

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
        private Parser<LambdaExpression> ParsePredicate => BooleanValue.End()
            .Or(Parse.ChainOperator(BinaryBooleanOperator, BooleanValue.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary).End())
            .Select(body => Expression.Lambda<Func<bool>>(body));
    }
}