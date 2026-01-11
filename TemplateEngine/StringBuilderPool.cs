// Module Name: StringBuilderPool.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
///   Represents a pool of <see cref="StringBuilder" /> objects.
///   This class is compatible with .NET Standard 2.0.
/// </summary>
public class StringBuilderPool
{
  #region Fields

  private readonly ConcurrentQueue<StringBuilder> _pool;
  private readonly int _initialBuilderCapacity;
  private int _count;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="StringBuilderPool" /> class.
  /// </summary>
  /// <param name="initialBuilderCapacity">The initial capacity of the pooled <see cref="StringBuilder" />s.</param>
  /// <param name="maxPoolSize">The maximum number of <see cref="StringBuilder" /> instances in the pool.</param>
  public StringBuilderPool(
    int initialBuilderCapacity = 1024,
    int maxPoolSize = 100 )
  {
    _initialBuilderCapacity = initialBuilderCapacity;
    MaxPoolSize = maxPoolSize;
    _pool = new ConcurrentQueue<StringBuilder>();
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the default <see cref="StringBuilderPool" /> instance.
  /// </summary>
  public static StringBuilderPool Default { get; } = new ();

  /// <summary>
  ///   The number of <see cref="StringBuilder" /> instances in the pool.
  /// </summary>
  public int Size => _count;

  /// <summary>
  ///   The maximum number of <see cref="StringBuilder" /> instances in the pool.
  /// </summary>
  public int MaxPoolSize { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets a <see cref="StringBuilder" /> object from the pool.
  /// </summary>
  /// <returns>A <see cref="StringBuilder" /> object.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public StringBuilder Get()
  {
    if( _pool.TryDequeue( out var builder ) )
    {
      Interlocked.Decrement( ref _count );
      return builder;
    }

    return CreateBuilder();
  }

  /// <summary>
  ///   Returns a <see cref="StringBuilder" /> object to the pool.
  /// </summary>
  /// <param name="builder">The <see cref="StringBuilder" /> object to return.</param>
  /// <exception cref="ArgumentNullException">Thrown when the <paramref name="builder" /> is <c>null</c>.</exception>
  /// <remarks>
  ///   If the pool is full, the builder instance will not be added to the pool and will be made available for
  ///   collection.
  /// </remarks>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public void Return(
    StringBuilder builder )
  {
    if( builder == null )
    {
      throw new ArgumentNullException( nameof( builder ) );
    }

    builder.Clear();

    if( !TryIncrementCount() )
    {
      return;
    }

    _pool.Enqueue( builder );
  }

  #endregion

  #region Implementation

  [MethodImpl( MethodImplOptions.NoInlining )]
  private StringBuilder CreateBuilder()
  {
    return new StringBuilder( _initialBuilderCapacity );
  }

  /// <summary>
  ///   Atomically increments the count if below <see cref="MaxPoolSize" />.
  /// </summary>
  /// <returns><c>true</c> if the count was incremented; otherwise, <c>false</c>.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  private bool TryIncrementCount()
  {
    int currentCount;
    int newCount;

    do
    {
      currentCount = _count;

      if( currentCount >= MaxPoolSize )
      {
        return false;
      }

      newCount = currentCount + 1;
    } while( Interlocked.CompareExchange( ref _count, newCount, currentCount ) != currentCount );

    return true;
  }

  #endregion
}
