using Sprache;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Parser
{
    public class WhereClause<T>
    {
        /// <summary>
        ///  a where clause is a delegate like: (T t) => t.property == 1.
        ///  <see cref="predicateInputParamater"/> is the placeholder of the instance of T which is matched by the
        ///  buildt predicate.
        /// </summary>
        private readonly ParameterExpression predicateInputParamater = Expression.Parameter(typeof(T));

        public Expression<Func<T, bool>> Of(string whereClause)
        {
            return Expression.Lambda<Func<T, bool>>(
                body: this.CombinedPredicate.Or(this.Predicate).Parse(whereClause),
                parameters: new[] { this.predicateInputParamater }
            );
        }

        #region Parse predicate combinations: <predicate> <and|or> <predicate>

        /// <summary>
        /// A combined predicate is made up from a left and a right predicate. In between there is a binary boolean
        /// operator.
        /// </summary>
        private Parser<Expression> CombinedPredicate => from leftSide in Predicate
                                                        from combineOperator in Operators.BooleanOperators
                                                        from rightSide in Predicate
                                                        select Expression.MakeBinary(combineOperator, leftSide, rightSide);

        #endregion Parse predicate combinations: <predicate> <and|or> <predicate>

        #region Parse comparision predicates: <predicate> ::= <predicate function|predicate expression>

        /// <summary>
        /// A predicate without braces is a plain boolean term in brances like: (a eq 1).
        /// These braces are stripped and the contant is parsed as a predicate again.
        /// This removes redindant braces as well like: ((a eq 1)) -> a eq 1
        /// </summary>
        private Parser<Expression> PredicateWithBraces => from openingBrace in Operators.OpeningBrace
                                                          from embracedContent in Predicate
                                                          from closingBrace in Operators.ClosingBrace
                                                          select embracedContent;

        /// <summary>
        /// A predicate is an expression or a funcion comparing two values resultiong to tru or false.
        /// A value might be a scalar value or an property value of T.
        /// </summary>
        private Parser<Expression> Predicate => this.BinaryPredicateFunction.Or(this.PredicateWithBraces).Or(this.PredicateExpression);

        #endregion Parse comparision predicates: <predicate> ::= <predicate function|predicate expression>

        #region Parse predicate expressions: <predicate expression> ::= <property|value> <op> <property|value>

        /// <summary>
        /// A predicate expression is just a plain boolean term like: a eq 1
        /// In addition it is allowed to have a predicate in braces deliveriong a boolean value instead of a property or scalar value.
        /// </summary>
        private Parser<Expression> PredicateExpression => from leftSide in this.ScalarValueOrProperty
                                                          from comparisionOperator in Operators.ComparisionOperators
                                                          from rightSide in this.ScalarValueOrProperty.Or(PredicateWithBraces)
                                                          select Expression.MakeBinary(comparisionOperator, leftSide, rightSide);

        #endregion Parse predicate expressions: <predicate expression> ::= <property|value> <op> <property|value>

        #region Parse predicate functions: <function>(<property|value>,<property|value>)

        /// <summary>
        /// A binary predicate function calcuates a boolen value from tow nput parameters. Bot parameter may be constant
        /// value or a property of the macted instance ot T.
        /// Currently the syntax is quite restrcited: no spaces are tolerated arount the functins tokens ( , )
        /// </summary>
        private Parser<Expression> BinaryPredicateFunction => from fname in Functions.FunctionName
                                                              from openingBrace in Parse.Char('(')
                                                              from p1 in ScalarValueOrProperty
                                                              from comma in Parse.Char(',')
                                                              from p2 in ScalarValueOrProperty
                                                              select CallExpression(fname, p1, p2);

        private MethodCallExpression CallExpression(string fname, Expression p1, Expression p2)
        {
            if (fname == "startswith")
                return Expression.Call(p1, typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) }), p2);
            else if (fname == "endswith")
                return Expression.Call(p1, typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) }), p2);
            throw new InvalidOperationException(fname);
        }

        #endregion Parse predicate functions: <function>(<property|value>,<property|value>)

        #region Parse scalar values and properties of T for comprison

        /// <summary>
        /// Psoosibe value if the left or the rights ides of a comparision are the scalar vlues parsed by the
        /// <see cref="ScalarValues"/> parser collection or it can be a name of a property of <see cref="T"/>
        /// </summary>
        private Parser<Expression> ScalarValueOrProperty => ScalarValues.All.Or(PropertyNameOf_T);

        /// <summary>
        /// A property name is a string without ticks or quotes.
        /// It is alteast one letter but might have digits after the first char. Special chars are possible too.
        /// Currently only letters are supported.
        /// </summary>
        private Parser<Expression> PropertyNameOf_T => from propertyName in Parse.Letter.AtLeastOnce().Text().Token()
                                                       select PropertyExpression(propertyName);

        /// <summary>
        /// Verify if teh found <paramref name="propertyName"/>is teh name of a property.
        /// If not, throw an exdeption.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private Expression PropertyExpression(string propertyName)
        {
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property {propertyName} doesn't exist in type {typeof(T)}");
            }
            return Expression.Property(this.predicateInputParamater, propertyName);
        }

        #endregion Parse scalar values and properties of T for comprison
    }
}