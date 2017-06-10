using ODataParser.Primitives;
using Sprache;
using System.Linq.Expressions;

namespace ODataParser.Test.Experiments
{
    public class Comparisions
    {
        #region Parse comparable values <compareable value> ::= <number|bool>

        private static Parser<ConstantExpression> CompareableValues = ScalarValues.Number.XOr(ScalarValues.BooleanConstant);

        #endregion Parse comparable values <compareable value> ::= <number|bool>

        #region Parse comparison expression: <comp expression> ::= <value> <op> <value>

        /// <summary>
        /// A comparision expression receives a value or another comparision expression as a parameter
        /// </summary>
        public static Parser<Expression> ComparisionExpression => Parse.ChainOperator(Operators.ComparisionOperators, CompareableValues.Or(Parse.Ref(() => AnyComparisionExpression)), Expression.MakeBinary);

        public static Parser<Expression> ComparisionExpressionInParenthesis => from left in Parse.Char('(').Token()
                                                                               from booleanExpr in Parse.Ref(() => AnyComparisionExpression) // Ref: delay access to avoid circular dependency
                                                                               from right in Parse.Char(')').Token()
                                                                               select booleanExpr;

        /// <summary>
        /// A boolean expression might be in parenthesis or not.
        /// </summary>
        /// <remarks>
        /// Selectct can be XOr becaue they differ in openeing  parenthisis.
        /// </remarks>
        public static Parser<Expression> AnyComparisionExpression => ComparisionExpressionInParenthesis.XOr(ComparisionExpression).Token(); // must be XOR

        #endregion Parse comparison expression: <comp expression> ::= <value> <op> <value>

        #region Parses a text to the end as a chained set of comparision expressions

        /// <summary>
        /// Parse the text to the end. It has to be a chian of value comparsions
        /// </summary>
        public static Parser<Expression> Complete => Parse.ChainOperator(Operators.ComparisionOperators, AnyComparisionExpression, Expression.MakeBinary).End();

        #endregion Parses a text to the end as a chained set of comparision expressions
    }
}