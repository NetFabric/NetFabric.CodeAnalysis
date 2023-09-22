using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NetFabric.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions;

public static partial class ExpressionEx
{
    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.Expression"/> with behaviour similar to the <c>foreach</c> statement in C#.
    /// </summary>
    /// <param name="enumerable">An <see cref="System.Linq.Expressions.Expression"/> that defines the enumerator.</param>
    /// <param name="body">
    /// A <see cref="System.Func&lt;Expression, Expression&gt;"/> that returns the body, given an
    /// <see cref="System.Linq.Expressions.Expression"/> that defines the item on each loop iteration.
    /// </param>
    /// <returns>The created <see cref="System.Linq.Expressions.Expression"/>.</returns>
    /// <exception cref="Exception">Object is not enumerable.</exception>
    /// <remarks>
    /// <p>
    /// The created <see cref="System.Linq.Expressions.Expression"/> depends on if the object defined in <paramref name="enumerable"/>
    /// is an <c>interface</c>, <c>class</c>, <c>struct</c>, <c>ref struct</c>, and if is disposable.
    /// </p>
    /// </remarks>
    public static Expression ForEach(Expression enumerable, Func<Expression, Expression> body)
    {
        var enumerableType = enumerable.Type;
        if (enumerableType.IsEnumerable(out var enumerableInfo, out var error))
            return ForEach(enumerableInfo, enumerable, body);

        return error switch
        {
            IsEnumerableError.MissingGetEnumerator => throw new Exception($"'{enumerableType}' does not contain a public definition for 'GetEnumerator'"),
            IsEnumerableError.MissingCurrent => throw new Exception($"'{enumerableInfo!.GetEnumerator.ReturnType}' does not contain a public definition for 'Current'"),
            IsEnumerableError.MissingMoveNext => throw new Exception($"'{enumerableInfo!.GetEnumerator.ReturnType}' does not contain a public definition for 'MoveNext'"),
            _ => throw new Exception("Unhandled error!"),
        };
    }

    /// <summary>
    /// Creates a <see cref="System.Linq.Expressions.Expression"/> with behaviour similar to the <c>foreach</c> statement in C#.
    /// </summary>
    /// <param name="enumerableInfo">The information returned by a call to <see cref="NetFabric.Reflection.TypeExtensions.IsEnumerable(Type, out EnumerableInfo)"/>.</param>
    /// <param name="enumerable">An <see cref="System.Linq.Expressions.Expression"/> that defines the enumerator.</param>
    /// <param name="body">
    /// A <see cref="System.Func&lt;Expression, Expression&gt;"/> that returns the body, given an
    /// <see cref="System.Linq.Expressions.Expression"/> that defines the item on each loop iteration.
    /// </param>
    /// <returns>The created <see cref="System.Linq.Expressions.Expression"/>.</returns>
    /// <remarks>
    /// <p>
    /// The created <see cref="System.Linq.Expressions.Expression"/> depends on if the object defined in <paramref name="enumerable"/>
    /// is an <c>interface</c>, <c>class</c>, <c>struct</c>, <c>ref struct</c>, and if is disposable.
    /// </p>
    /// </remarks>
    public static Expression ForEach(EnumerableInfo enumerableInfo, Expression enumerable, Func<Expression, Expression> body)
    {
        if(enumerableInfo.ForEachUsesIndexer)
        {
            if (enumerable.Type.IsSpanOrReadOnlySpan())
            {
                // throws exception because the indexer returns a reference to the item and 
                // references are not yet supported by expression trees
                throw new NotSupportedException("ForEach Expression creation for Span<> or ReadOnlySpan<> is not supported.");
            }

            return UseIndexer(enumerable, body);
        }
        
        return UseEnumerable(enumerableInfo, enumerable, body);
        
        static Expression UseIndexer(Expression array, Func<Expression, Expression> body)
        {
            var arrayType = array.Type;
            return arrayType.IsIndexable(out var indexableInfo, out var error)
                ? For(array, indexableInfo, body)
                : error switch
                {
                    IsIndexableError.MissingIndexer => throw new Exception($"'{arrayType}' does not contain a public definition for 'this'"),
                    IsIndexableError.MissingCountOrLength => throw new Exception($"'{arrayType}' does not contain a public definition for 'Count' or 'Length'"),
                    _ => throw new Exception("Unhandled error!"),
                };
        }

        static Expression UseEnumerable(EnumerableInfo enumerableInfo, Expression enumerable, Func<Expression, Expression> body)
        {
            var enumeratorType = enumerableInfo.GetEnumerator.ReturnType;
            var enumeratorInfo = enumerableInfo.EnumeratorInfo;
            var enumeratorVariable = Variable(enumeratorType, "enumerator");
            return Block(
                new[] { enumeratorVariable },
                Assign(enumeratorVariable, CallGetEnumerator(enumerableInfo, enumerable)),
                enumeratorInfo switch
                {
                    {Dispose: null} => NotDisposable(enumeratorInfo, enumeratorVariable, body),
                    _ => Disposable(enumeratorInfo, enumeratorVariable, body)
                });

            static Expression CallGetEnumerator(EnumerableInfo enumerableInfo, Expression enumerable)
                => enumerableInfo.EnumeratorInfo switch
                {
                    {IsGenericsEnumeratorInterface: true}
                        => Call(Convert(enumerable, enumerableInfo.GetEnumerator.DeclaringType!), enumerableInfo.GetEnumerator),
                    
                    {IsEnumeratorInterface: true}
                        => Call(Convert(enumerable, typeof(IEnumerable)), enumerableInfo.GetEnumerator),
                    
                    _ => Call(enumerable, enumerableInfo.GetEnumerator)
                };

            static Expression Disposable(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
                => Using(enumerator, 
                    EnumerationLoop(enumeratorInfo, enumerator, body)
                );

            static Expression NotDisposable(EnumeratorInfo enumeratorInfo, ParameterExpression enumerator, Func<Expression, Expression> body)
            {
                return enumeratorInfo switch
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
                                    typeof(IDisposable).GetPublicMethod(NameOf.Dispose)!)
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