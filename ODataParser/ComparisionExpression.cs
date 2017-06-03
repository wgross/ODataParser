using Sprache;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ODataParser
{
    public class ComparisionExpression<T>
    {
        public Expression<Func<T, bool>> MakeWhere(string whereClause)
        {
            var parameterExpression = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, bool>>(
                body: this.CreatePredicateBody(parameterExpression, whereClause),
                parameters: new[] { parameterExpression }
            );
        }

        public Expression CreatePredicateBody(ParameterExpression instanceOfT, string whereClause)
        {
            return this.PredicateBodyParser()(instanceOfT).Parse(whereClause);
        }

        private Func<ParameterExpression, Parser<Expression>> PredicateBodyParser()
        {
            return (ParameterExpression instanceOfT) => (from propertyName in ScalarValues.PropertyName
                                                         from comparisionOperator in Operators.ComparisionOperators
                                                         from constantValue in ScalarValues.ContantOfAnyType
                                                         select PropertyComparisionExpression(instanceOfT, propertyName, comparisionOperator, constantValue));
        }

        private Expression PropertyComparisionExpression(ParameterExpression instanceOfT, string propertyName, ExpressionType comparisionOperator, string constantComparisionValue)
        {
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property {propertyName} doesn't exist in type {typeof(T)}");
            }

            var propertyType = property.PropertyType;
            var getPropertyValueExpr = Expression.Property(instanceOfT, propertyName);
            var constantValueExpr = Expression.Constant(this.ConvertTo(propertyType, constantComparisionValue));

            switch (comparisionOperator)
            {
                case ExpressionType.LessThan:
                    return Expression.LessThan(getPropertyValueExpr, constantValueExpr);

                case ExpressionType.Equal:
                    return Expression.Equal(getPropertyValueExpr, constantValueExpr);

                default: throw new NotImplementedException($"The comparision operation {comparisionOperator} is unknown");
            }
        }

        private object ConvertTo(Type destinationType, string constantComparisionValue)
        {
            if (destinationType == typeof(int))
            {
                return int.Parse(constantComparisionValue);
            }
            else if (destinationType == typeof(string))
            {
                return constantComparisionValue;
            }
            else if (destinationType == typeof(bool))
            {
                return bool.Parse(constantComparisionValue);
            }
            else throw new InvalidOperationException($"Value type {destinationType.Name} is unknown");
        }
    }
}