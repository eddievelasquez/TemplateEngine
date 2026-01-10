// Module Name: SegmentKind.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Specifies the kind of segment within a template, determining whether it represents
///   constant text or a macro invocation.
/// </summary>
/// <remarks>
///   This enum uses a byte as the underlying type to minimize memory overhead in the
///   <see cref="Segment" /> discriminated union structure.
/// </remarks>
internal enum SegmentKind: byte
{
  /// <summary>
  ///   Indicates the segment represents a constant (literal) text region that is copied
  ///   directly to the output during template expansion.
  /// </summary>
  Constant,

  /// <summary>
  ///   Indicates the segment represents a user-defined macro invocation that will be resolved
  ///   by a custom macro resolver during template expansion.
  /// </summary>
  UserMacro,

  /// <summary>
  ///   Indicates the segment represents a standard (built-in) macro invocation that will be resolved
  ///   by the template engine's standard macro handler during template expansion.
  /// </summary>
  StandardMacro
}
