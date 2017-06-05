using Parser;
using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class ValueComparisonParser
    {
        #region Parse comparison expression: <comp expression> ::= <value> <op> <value>

        private static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).Token().Return(opType);

        public static Parser<ExpressionType> LessThan = Operator("lt", ExpressionType.LessThan);
        public static Parser<ExpressionType> LessThanOrEqual = Operator("le", ExpressionType.LessThanOrEqual);
        public static Parser<ExpressionType> Equal = Operator("eq", ExpressionType.Equal);
        public static Parser<ExpressionType> NotEqual = Operator("ne", ExpressionType.NotEqual);
        public static Parser<ExpressionType> GreaterThanOrEqual = Operator("ge", ExpressionType.GreaterThanOrEqual);
        public static Parser<ExpressionType> GreaterThan = Operator("gt", ExpressionType.GreaterThan);

        // must be Or because first char is not unique/significant enough
        public static Parser<ExpressionType> ComparisionOperators = LessThan.Or(LessThanOrEqual).Or(Equal).Or(NotEqual).Or(GreaterThan).Or(GreaterThanOrEqual);

        /// <summary>
        /// A comparision expression receives a value or another comparision expression as a parameter
        /// </summary>
        public static Parser<Expression> ComparisionExpression => Parse.ChainOperator(ComparisionOperators, ScalarValues.SignedInteger.Or(Parse.Ref(() => ComparisionExpression)), Expression.MakeBinary);

        private static Parser<Expression> ComparisionExpressionInParenthesis => from left in Parse.Char('(').Token()
                                                                                from booleanExpr in Parse.Ref(() => AnyComparisionExpression) // Ref: delay access to avoid circular dependency
                                                                                from right in Parse.Char(')').Token()
                                                                                select booleanExpr;

        /// <summary>
        /// A boolean expression might be in parenthesis or not.
        /// </summary>
        public static Parser<Expression> AnyComparisionExpression => ComparisionExpressionInParenthesis.XOr(ComparisionExpression); // must be XOR

        #endregion Parse comparison expression: <comp expression> ::= <value> <op> <value>

        #region Evaluate a comparison expresssion

        /// <summary>
        /// Parses the given boolean expression text and evaluates the result.
        /// </summary>
        /// <param name="text">bolle exprsssion text</param>
        /// <returns></returns>
        public static bool Evaluate(string text) => Evaluate((Expression<Func<bool>>)ParsePredicate.Parse(text));

        private static bool Evaluate(Expression<Func<bool>> expression) => Evaluate(expression.Compile());

        private static bool Evaluate(Func<bool> predicate) => predicate();

        #endregion Evaluate a comparison expresssion

        /// <summary>
        /// A prediate body may consist of just a constant, a single expression, or a set of expressions linked with a binary operator
        /// </summary>
        private static Parser<LambdaExpression> ParsePredicate => AnyComparisionExpression.Select(body => Expression.Lambda<Func<bool>>(body));
    }
}