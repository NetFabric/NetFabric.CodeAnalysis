using System;
using System.Collections;
using System.Linq.Expressions;
using NetFabric.Reflection;
using static System.Linq.Expressions.Expression;

namespace NetFabric.Expressions
{
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