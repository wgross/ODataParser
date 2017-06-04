using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser.Test
{
    public class BooleanExpressionParser
    {
        public static bool EvaluateConstant(string text) => ParseConstant(text).Compile().Invoke();

        public static Expression<Func<bool>> ParseConstant(string text) => LambdaConstant.Parse(text) as Expression<Func<bool>>;

        public static Parser<LambdaExpression> LambdaConstant => BooleanConstant.XOr(BooleanConstantInParenthesis).End().Select(body => Expression.Lambda<Func<bool>>(body));

        // ---

        public static bool EvaluateExpresssion(string text) => ParseExpression(text).Compile().Invoke();

        public static Expression<Func<bool>> ParseExpression(string text) => LambdaExpression.Parse(text) as Expression<Func<bool>>;

        public static Parser<LambdaExpression> LambdaExpression => BooleanExpression.Or(BooleanExpressionInParenthesis).End().Select(body => Expression.Lambda<Func<bool>>(body));

        // ---

        //#region Generalize boolean expressions and constants as values: boolean value ::= <boolean expression<boolean value>

        // #endregion Generalize boolean expressions and constants as values: boolean value ::= <boolean expression<boolean value>

        #region Generalize unary and binary expresssion <boolean expression> ::= <unary boolean expression|binary boolean expression>

        public static Parser<Expression> BooleanExpressionInParenthesis => from lparen in Parse.Char('(')
                                                                           from booleanExpression in BooleanExpression.XOr(BooleanExpressionInParenthesis)
                                                                           from rparen in Parse.Char(')')
                                                                           select booleanExpression;

        public static Parser<Expression> BooleanExpression => UnaryBooleanExpression.XOr(BinaryBooleanExpression);

        #endregion Generalize unary and binary expresssion <boolean expression> ::= <unary boolean expression|binary boolean expression>

        #region Parse boolean unary expression <boolean unary expression> ::= not <BooleanExpression>

        public static Parser<Expression> UnaryBooleanExpression => from not in Not
                                                                   from expression in BooleanConstant.XOr(BooleanConstantInParenthesis)
                                                                   select Expression.MakeUnary(ExpressionType.Not, expression, typeof(bool));

        public static Parser<ExpressionType> Not => Parse.String("not").Token().Return(ExpressionType.Not);

        #endregion Parse boolean unary expression <boolean unary expression> ::= not <BooleanExpression>

        #region Parse boolean binary expression <boolean unary expression> ::= <BooleanExpression> <and|or|xor> <BooleanExpression>

        public static Parser<Expression> BinaryBooleanExpression => Parse.ChainOperator(BinaryBooleanOperator, BooleanConstant, Expression.MakeBinary);
        public static Parser<ExpressionType> BinaryBooleanOperator => And.XOr(Or).XOr(XOr);
        public static Parser<ExpressionType> And => Parse.String("and").Token().Return(ExpressionType.And);
        public static Parser<ExpressionType> Or => Parse.String("or").Token().Return(ExpressionType.OrElse);
        public static Parser<ExpressionType> XOr => Parse.String("xor").Token().Return(ExpressionType.ExclusiveOr); // not supported in EF?!

        #endregion Parse boolean binary expression <boolean unary expression> ::= <BooleanExpression> <and|or|xor> <BooleanExpression>

        #region Parse boolean constants: <boolean value> ::= <true|false>|(<true|false>)

        public static Parser<Expression> BooleanConstantInParenthesis => from lparen in Parse.Char('(')
                                                                         from booleanConstan in BooleanConstant.XOr(BooleanConstantInParenthesis)
                                                                         from rparen in Parse.Char(')')
                                                                         select booleanConstan;

        public static Parser<Expression> BooleanConstant => True.XOr(False);
        public static Parser<Expression> True = Parse.String("true").Token().Return(Expression.Constant(true));
        public static Parser<Expression> False = Parse.String("false").Token().Return(Expression.Constant(false));

        #endregion Parse boolean constants: <boolean value> ::= <true|false>|(<true|false>)

        public static bool EvaluateValue(string text) => ParseValue(text).Compile().Invoke();

        public static Expression<Func<bool>> ParseValue(string text) => LambdaValue.Parse(text) as Expression<Func<bool>>;

        public static Parser<LambdaExpression> LambdaValue => BooleanValue.Or(BooleanValueInParenthesis).End().Select(body => Expression.Lambda<Func<bool>>(body));

        public static Parser<Expression> BooleanValueInParenthesis => from lparen in Parse.Char('(')
                                                                      from booleanValue in BooleanValue.Or(BooleanValueInParenthesis)
                                                                      from rparen in Parse.Char(')')
                                                                      select booleanValue;

        public static Parser<Expression> BooleanValue = True.XOr(False).XOr(UnaryBooleanExpression).XOr(BinaryBooleanExpression).XOr(BooleanValueInParenthesis);
    }
}