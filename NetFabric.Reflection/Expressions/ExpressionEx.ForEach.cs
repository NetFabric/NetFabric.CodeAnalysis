using System;
using System.Collections;
using System.Collections.Generic;
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
            
            if (errors.HasFlag(Errors.MissingGetEnumerator))
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
                    For(
                        Assign(indexVariable, Constant(0)),
                        LessThan(indexVariable, Property(array, typeof(Array).GetPublicInstanceDeclaredOnlyReadProperty(nameof(Array.Length))!)),
                        PostIncrementAssign(indexVariable),
                        body(ArrayIndex(array, indexVariable))
                    )
                );
            }
            
            static Expression HandleSpan(Expression span, Func<Expression, Expression> body)
            {
                // throws exception because the indexer returns a reference to the item and 
                // references are not yet supported by expression trees
                throw new NotSupportedException("ForEach Expression creation for Span<> or ReadOnlySpan<> is not supported.");
                
                // var indexVariable = Variable(typeof(int), "index");
                // return Block(
                //     new[] { indexVariable },
                //     For(
                //         Assign(indexVariable, Constant(0)),
                //         LessThan(indexVariable, Property(span, span.Type.GetPublicInstanceDeclaredOnlyProperty(nameof(Span<int>.Length))!)),
                //         PostIncrementAssign(indexVariable),
                //         body(Property(span, span.Type.GetPublicInstanceDeclaredOnlyProperty("Item")!, indexVariable))
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
                    Assign(enumeratorVariable, CallGetEnumerator(enumerableInfo, enumeratorType, enumerable)),
                    enumeratorInfo switch
                    {
                        {Dispose: null} => NotDisposable(enumeratorInfo, enumeratorVariable, body),
                        _ => Disposable(enumeratorInfo, enumeratorVariable, body)
                    });

                static Expression CallGetEnumerator(EnumerableInfo enumerableInfo, Type enumeratorType, Expression enumerable)
                {
                    if (enumeratorType.IsGenericType && enumeratorType.GetGenericTypeDefinition() == typeof(IEnumerator<>))
                    {
                        var enumerableType = typeof(IEnumerable<>).MakeGenericType(enumerableInfo.GetEnumerator.ReturnType.GetGenericArguments());
                        return Call(
                            Convert(enumerable, enumerableType), 
                            enumerableType.GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable<int>.GetEnumerator), Type.EmptyTypes)!);
                    }

                    if (enumeratorType == typeof(IEnumerator))
                        return Call(
                            Convert(enumerable, typeof(IEnumerable)), 
                            typeof(IEnumerable).GetPublicInstanceDeclaredOnlyMethod(nameof(IEnumerable.GetEnumerator), Type.EmptyTypes)!);
                    
                    return Call(enumerable, enumerableInfo.GetEnumerator);
                }

                static Expression Disposable(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                    => Using(enumerator, 
                        EnumerationLoop(enumeratorInfo, enumerator, body)
                    );

                static Expression NotDisposable(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                {
                    return enumerator.Type switch
                    {
                        {IsValueType: true} => NotDisposableValueType(enumeratorInfo, enumerator, body),
                        _ => NotDisposableReferenceType(enumeratorInfo, enumerator, body)
                    };

                    static Expression NotDisposableValueType(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                        => EnumerationLoop(enumeratorInfo, enumerator, body);

                    static Expression NotDisposableReferenceType(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                    {
                        var disposable = Variable(typeof(IDisposable), "disposable");
                        return TryFinally(
                            EnumerationLoop(enumeratorInfo, enumerator, body),
                            Block(
                                new[] {disposable},
                                Assign(disposable, TypeAs(enumerator, typeof(IDisposable))),
                                IfThen(
                                    NotEqual(disposable, Constant(null)),
                                    Call(disposable,
                                        typeof(IDisposable).GetPublicInstanceDeclaredOnlyMethod(
                                            nameof(IDisposable.Dispose), Type.EmptyTypes)!)
                                )
                            )
                        );
                    }
                }

                static Expression EnumerationLoop(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                    => While(Call(enumerator, enumeratorInfo.MoveNext),
                        body(Property(enumerator, enumeratorInfo.Current))
                    );
            }
        }
    }
}