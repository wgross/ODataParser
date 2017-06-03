using Sprache;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataParser
{
    public class ComparisionExpression<T>
    {
        private readonly ParameterExpression predicateInputParamater = Expression.Parameter(typeof(T));

        public Expression<Func<T, bool>> MakeWhere(string whereClause)
        {
            return Expression.Lambda<Func<T, bool>>(
                body: this.PropertyCombinedOrSingle.Parse(whereClause),
                parameters: new[] { this.predicateInputParamater }
            );
        }

        private Parser<Expression> PropertyCombinedOrSingle => PropertyCombined.Or(PropertyCompare);

        private Parser<Expression> PropertyCombined => (from leftSide in PropertyCompare
                                                        from combineOperator in Operators.And.Or(Operators.Or)
                                                        from rightSide in PropertyCompare
                                                        select Expression.MakeBinary(combineOperator, leftSide, rightSide));

        private Expression PropertyCombinationExpression(Expression leftSide, ExpressionType combineOperator, Expression rightSide)
        {
            return Expression.MakeBinary(combineOperator, leftSide, rightSide);
        }

        private Parser<Expression> PropertyCompareWithBraces => (from openingBrace in Operators.OpeningBrace
                                                                 from embracedContent in PropertyCompare
                                                                 from closingBrace in Operators.ClosingBrace
                                                                 select embracedContent);

        private Parser<Expression> PropertyCompareWithoutBraces => (from property in ScalarValues.PropertyName.Select(PropertyOfInutParameter)
                                                                    from comparisionOperator in Operators.ComparisionOperators
                                                                    from constantValue in ScalarValues.ComparableScalarValue
                                                                    select Expression.MakeBinary(comparisionOperator, property, constantValue));

        private Parser<Expression> PropertyCompare => PropertyCompareWithBraces.Or(PropertyCompareWithoutBraces);

        private Expression PropertyOfInutParameter(string propertyName)
        {
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property {propertyName} doesn't exist in type {typeof(T)}");
            }
            return Expression.Property(this.predicateInputParamater, propertyName);
        }
    }
}