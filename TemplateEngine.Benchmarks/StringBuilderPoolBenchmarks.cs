// Module Name: StringBuilderPoolBenchmarks.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace TemplateEngine.Benchmarks;

using System.Text;
using BenchmarkDotNet.Attributes;
using Intercode.Toolbox.TemplateEngine;
using Microsoft.VSDiagnostics;

[CPUUsageDiagnoser]
public class StringBuilderPoolBenchmarks
{
  #region Fields

  private StringBuilderPool _pool = null!;

  #endregion

  #region Public Methods

  [GlobalSetup]
  public void Setup()
  {
    _pool = new StringBuilderPool( 1024, 100 );

    // Pre-warm the pool with some builders
    var builders = new StringBuilder[10];

    for( var i = 0; i < builders.Length; i++ )
    {
      builders[i] = _pool.Get();
    }

    for( var i = 0; i < builders.Length; i++ )
    {
      _pool.Return( builders[i] );
    }
  }

  [Benchmark( Baseline = true )]
  public StringBuilder GetAndReturn_Single()
  {
    var builder = _pool.Get();
    builder.Append( "Hello, World!" );
    _pool.Return( builder );
    return builder;
  }

  [Benchmark]
  public void GetAndReturn_Multiple()
  {
    for( var i = 0; i < 100; i++ )
    {
      var builder = _pool.Get();
      builder.Append( "Hello, World!" );
      _pool.Return( builder );
    }
  }

  [Benchmark]
  public StringBuilder Get_FromEmptyPool()
  {
    // Create a fresh pool to ensure we're measuring allocation
    var emptyPool = new StringBuilderPool( 1024, 100 );
    return emptyPool.Get();
  }

  [Benchmark]
  public void ConcurrentAccess()
  {
    Parallel.For(
      0,
      10,
      _ =>
      {
        var builder = _pool.Get();
        builder.Append( "Concurrent test string" );
        _pool.Return( builder );
      }
    );
  }

  #endregion
}
