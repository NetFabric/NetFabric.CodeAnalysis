using System;
using System.Linq.Expressions;
using System.Reflection;
using NetFabric.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
    public static partial class ExpressionEx
    {
        public static Expression ForEach(Expression enumerable, Func<Expression, Expression> body)
        {
            var enumerableType = enumerable.Type;
            if (enumerableType.IsEnumerable(out var enumerableInfo, out var errors))
                return ForEach(enumerableInfo, enumerable, body);
            
            if (errors.HasFlag(Errors.MissingGetEnumerable))
                throw new Exception($"'{enumerableType}' does not contain a public definition for 'GetEnumerator'");

            if (errors.HasFlag(Errors.MissingCurrent))
                throw new Exception(
                    $"'{enumerableInfo!.GetEnumerator.ReturnType.Name}' does not contain a public definition for 'Current'");

            if (errors.HasFlag(Errors.MissingMoveNext))
                throw new Exception(
                    $"'{enumerableInfo!.GetEnumerator.ReturnType.Name}' does not contain a public definition for 'MoveNext'");

            throw new Exception("Unhandled error!");
        }

        public static Expression ForEach(EnumerableInfo enumerableInfo, Expression enumerable, Func<Expression, Expression> body)
        {
            var enumeratorType = enumerableInfo.GetEnumerator.ReturnType;
            var enumeratorInfo = enumerableInfo.EnumeratorInfo;
            var enumerator = Variable(enumeratorType, "enumerator");
            return Block(
                new[] { enumerator },
                Assign(enumerator, Call(enumerable, enumerableInfo.GetEnumerator)),
                enumeratorInfo switch
                {
                    { Dispose: not null } => Disposable(enumeratorInfo, enumerator, body),
                    _ => enumeratorType switch
                    {
                        { IsValueType: true } => NonDisposableValueType(enumeratorInfo, enumerator, body),
                        _ => NonDisposableReferenceType(enumeratorInfo, enumerator, body)
                    }
                }
            );
                
            static Expression Disposable(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                => Using(enumerator,
                        Enumeration(enumeratorInfo, enumerator, body)
                    );
                
            static Expression NonDisposableValueType(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                => Enumeration(enumeratorInfo, enumerator, body);
                
            static Expression NonDisposableReferenceType(EnumeratorInfo enumeratorInfo, ParameterExpression  enumerator, Func<Expression, Expression> body)
            {
                var disposable = Variable(typeof(IDisposable), "disposable");
                return TryFinally(
                    Enumeration(enumeratorInfo, enumerator, body),
                    Block(
                        new [] { disposable, enumerator },
                        Assign(disposable, TypeAs(enumerator, typeof(IDisposable))),
                        IfThen(
                            NotEqual(disposable, Constant(null)),
                            Call(disposable, Reflection.TypeExtensions.DisposeInfo)
                        )
                    )
                );
            }

            static Expression Enumeration(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                => While(Call(enumerator, enumeratorInfo.MoveNext),
                    body(Property(enumerator, enumeratorInfo.Current))
                );
        }
    }
}