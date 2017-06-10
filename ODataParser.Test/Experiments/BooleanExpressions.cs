using ODataParser.Primitives;
using Sprache;
using System.Linq.Expressions;

namespace ODataParser.Test.Experiments
{
    public class BooleanExpressions
    {
        #region Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        /// <summary>
        /// An unary operator may have a constant or another expressions as a parameter
        /// </summary>
        public static Parser<Expression> UnaryBooleanExpression => from not in Operators.Not
                                                                   from value in ScalarValues.BooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)) // Ref: delay access to avoid circular dependency
                                                                   select Expression.MakeUnary(ExpressionType.Not, value, typeof(bool));

        #endregion Parse boolean unary expression <boolean unary expression> ::= not <boolean constant|boolean expression>

        #region Parse boolean binary expression <boolean binary expression> ::= <boolean constant|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        /// <summary>
        /// A binary bolean expression receives a constant or another boolean expression as a parameter
        /// </summary>
        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(Operators.BinaryBoolean, ScalarValues.BooleanConstant.Or(Parse.Ref(() => AnyBooleanExpression)), Expression.MakeBinary);

        #endregion Parse boolean binary expression <boolean binary expression> ::= <boolean constant|boolean expression> <and|or|xor> <boolean constant|boolean expression>

        #region Generalize unary and binary boolean expression: <boolean expression> ::= <unary boolean expression|binary boolean expression>

        /// <summary>
        /// general boolean expression.
        /// </summary>
        /// <remarks>
        /// Xor is ok here because unary always starts with n (not) after removeing leading whitespaces
        /// </remarks>
        public static Parser<Expression> BooleanExpression => UnaryBooleanExpression.XOr(BinaryBooleanExpression).Token();

        public static Parser<Expression> BooleanExpressionInParenthesis => from left in Parse.Char('(').Token()
                                                                           from booleanExpr in Parse.Ref(() => AnyBooleanExpression).Or(ScalarValues.BooleanConstant) // Ref: delay access to avoid circular dependency
                                                                           from right in Parse.Char(')').Token()
                                                                           select booleanExpr;

        /// <summary>
        /// A boolean expression might be in parenthesis or not.
        /// </summary>
        /// <remarks>
        /// Xor is ok here because the leading '(' is sgnificant
        /// </remarks>
        public static Parser<Expression> AnyBooleanExpression => BooleanExpressionInParenthesis.XOr(BooleanExpression).Token(); // must be XOR

        #endregion Generalize unary and binary boolean expression: <boolean expression> ::= <unary boolean expression|binary boolean expression>

        public static Parser<Expression> Complete => Parse.ChainOperator(Operators.BinaryBoolean, AnyBooleanExpression.Or(ScalarValues.BooleanConstant), Expression.MakeBinary).End();
    }
}