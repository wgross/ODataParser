using Parser;
using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class ValueComparisons
    {
        #region Parse comparable values <compareable value> ::= <number|bool>

        private static Parser<ConstantExpression> CompareableValues = ScalarValues.Number.XOr(ScalarValues.BooleanConstant);

        #endregion Parse comparable values <compareable value> ::= <number|bool>

        #region Parse comparison expression: <comp expression> ::= <value> <op> <value>

        /// <summary>
        /// A comparision expression receives a value or another comparision expression as a parameter
        /// </summary>
        public static Parser<Expression> ComparisionExpression => Parse.ChainOperator(Operators.ComparisionOperators, CompareableValues.Or(Parse.Ref(() => AnyComparisionExpression)), Expression.MakeBinary);

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
        private static Parser<LambdaExpression> ParsePredicate => AnyComparisionExpression.End()
            .Or(Parse.ChainOperator(Operators.ComparisionOperators, Parse.Ref(() => AnyComparisionExpression), Expression.MakeBinary))
            .Select(body => Expression.Lambda<Func<bool>>(body));
    }
}