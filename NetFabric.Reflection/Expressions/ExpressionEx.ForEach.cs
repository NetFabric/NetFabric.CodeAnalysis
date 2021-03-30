using System;
using System.Linq.Expressions;
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
            var enumerableType = enumerable.Type;
            if (enumerableType.IsArray)
                return HandleArray(enumerable, body);

            if (enumerableType.FullName is not null 
                && (enumerableType.FullName.StartsWith("System.ReadOnlySpan`1") || enumerableType.FullName.StartsWith("System.Span`1")))
                return HandleSpan(enumerable, body);
            
            return HandleEnumerable(enumerableInfo, enumerable, body);
            
            static Expression HandleArray(Expression array, Func<Expression, Expression> body)
            {
                var indexVariable = Variable(typeof(int), "index");
                return Block(
                    new[] { indexVariable },
                    Assign(indexVariable, Constant(0)),
                    While(
                        LessThan(indexVariable, Property(array, typeof(Array).GetPublicInstanceDeclaredOnlyProperty(nameof(Array.Length))!)),
                        Block(
                            body(ArrayIndex(array, indexVariable)),
                            PostIncrementAssign(indexVariable)
                        )
                    )
                );
            }
            
            static Expression HandleSpan(Expression span, Func<Expression, Expression> body)
            {
                // throw exception because the indexer returns a reference to the item and 
                // references are not yet supported by expression trees
                throw new NotSupportedException("ForEach expression creation for Span<> and ReadOnlySpan<> is not supported.");
                
                // var indexVariable = Variable(typeof(int), "index");
                // return Block(
                //     new[] { indexVariable },
                //     Assign(indexVariable, Constant(0)),
                //     While(
                //         LessThan(indexVariable, Property(span, span.Type.GetPublicInstanceDeclaredOnlyProperty(nameof(Array.Length))!)),
                //         Block(
                //             body(Property(span, span.Type.GetPublicInstanceDeclaredOnlyProperty("Item")!, indexVariable)),
                //             PostIncrementAssign(indexVariable)
                //         )
                //     )
                // );
            }

            static Expression HandleEnumerable(EnumerableInfo enumerableInfo, Expression enumerable, Func<Expression, Expression> body)
            {
                var enumeratorType = enumerableInfo.GetEnumerator.ReturnType;
                var enumeratorInfo = enumerableInfo.EnumeratorInfo;
                var enumeratorVariable = Variable(enumeratorType, "enumerator");
                return Block(
                    new[] { enumeratorVariable },
                    Assign(enumeratorVariable, Call(enumerable, enumerableInfo.GetEnumerator)),
                    enumeratorInfo switch
                    {
                        {Dispose: not null} => Disposable(enumeratorInfo, enumeratorVariable, body),
                        _ => enumeratorType switch
                        {
                            {IsValueType: true} => NonDisposableValueType(enumeratorInfo, enumeratorVariable, body),
                            _ => NonDisposableReferenceType(enumeratorInfo, enumeratorVariable, body)
                        }
                    });

                static Expression Disposable(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                    => Using(enumerator, 
                        EnumerationLoop(enumeratorInfo, enumerator, body)
                    );

                static Expression NonDisposableValueType(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                    => EnumerationLoop(enumeratorInfo, enumerator, body);

                static Expression NonDisposableReferenceType(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                {
                    var disposable = Variable(typeof(IDisposable), "disposable");
                    return TryFinally(
                        EnumerationLoop(enumeratorInfo, enumerator, body),
                        Block(
                            new[] { disposable },
                            Assign(disposable, TypeAs(enumerator, typeof(IDisposable))),
                            IfThen(
                                NotEqual(disposable, Constant(null)),
                                Call(disposable, Reflection.TypeExtensions.DisposeInfo)
                            )
                        )
                    );
                }

                static Expression EnumerationLoop(EnumeratorInfo enumeratorInfo, Expression enumerator, Func<Expression, Expression> body)
                    => While(Call(enumerator, enumeratorInfo.MoveNext),
                        body(Property(enumerator, enumeratorInfo.Current))
                    );
            }
        }
    }
}