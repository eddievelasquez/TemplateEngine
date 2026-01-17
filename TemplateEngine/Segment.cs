// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

#if false
[StructLayout( LayoutKind.Sequential,Pack = 1 )]
internal struct IfElseSegment
{
  #region Fields

  public int PredicateNameStart;
  public byte PredicateNameLength;
  public byte ArgumentLength;
  public int IfBlockLength;
  public int ElseBlockLength;

  #endregion
}
#endif

/// <summary>
///   Represents a parsed segment of a template, which can be either a constant text region or a macro invocation.
/// </summary>
/// <remarks>
///   This struct uses explicit layout to create a discriminated union, storing either a <see cref="ConstantSegment" />
///   or a <see cref="MacroSegment" /> based on the <see cref="Kind" /> discriminator. The struct occupies 20 bytes
///   with all fields aligned to their natural boundaries (4-byte alignment for the union offset) for optimal CPU
///   memory access patterns.
/// </remarks>
[StructLayout( LayoutKind.Explicit, Size = 20 )]
[DebuggerDisplay( "{GetDebuggerString()}" )]
[SuppressMessage( "ReSharper", "ConvertToAutoPropertyWhenPossible" )]
[SuppressMessage( "ReSharper", "ConvertToAutoProperty" )]
internal readonly struct Segment
{
  #region Constants

  /// <summary>
  ///   Represents an empty constant segment with zero length.
  /// </summary>
  public static Segment Empty = CreateConstant( 0, 0 );

  #endregion

  #region Fields

  [FieldOffset( 0 )]
  private readonly SegmentKind _kind;

  [FieldOffset( 4 )]
  private readonly ConstantSegment _constant;

  [FieldOffset( 4 )]
  private readonly MacroSegment _macro;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a constant segment.
  /// </summary>
  /// <param name="constantSegment">The constant segment data.</param>
  private Segment(
    ConstantSegment constantSegment )
  {
    _kind = SegmentKind.Constant;
    _constant = constantSegment;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a macro segment.
  /// </summary>
  /// <param name="kind">The kind of macro segment (user-defined or standard).</param>
  /// <param name="macroSegment">The macro segment data.</param>
  private Segment(
    SegmentKind kind,
    MacroSegment macroSegment )
  {
    _kind = kind;
    _macro = macroSegment;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the kind of segment, indicating whether it represents a constant, user macro, or standard macro.
  /// </summary>
  public SegmentKind Kind => _kind;

  /// <summary>
  ///   Gets the constant segment data. Only valid when <see cref="Kind" /> is <see cref="SegmentKind.Constant" />.
  /// </summary>
  public ConstantSegment Constant => _constant;

  /// <summary>
  ///   Gets the macro segment data. Only valid when <see cref="Kind" /> is <see cref="SegmentKind.UserMacro" />
  ///   or <see cref="SegmentKind.StandardMacro" />.
  /// </summary>
  public MacroSegment Macro => _macro;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a constant segment representing a literal text region within a template.
  /// </summary>
  /// <param name="textStart">The zero-based starting index of the constant text in the template source.</param>
  /// <param name="textLength">The length of the constant text in characters.</param>
  /// <returns>A new <see cref="Segment" /> instance configured as a constant segment.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when <paramref name="textStart" /> or <paramref name="textLength" /> is negative.
  /// </exception>
  public static Segment CreateConstant(
    int textStart,
    int textLength )
  {
    return new Segment( new ConstantSegment( textStart, textLength ) );
  }

  /// <summary>
  ///   Creates a macro segment with automatic determination of the macro kind based on the slot value.
  /// </summary>
  /// <param name="slot">
  ///   The slot index where the macro resolver is stored. Negative values indicate a standard macro;
  ///   non-negative values indicate a user-defined macro. Valid range is [-32768, 65535].
  /// </param>
  /// <param name="nameStart">The zero-based starting index of the macro name in the template source.</param>
  /// <param name="nameLength">The length of the macro name in characters. Must be in the range [1, 65535].</param>
  /// <param name="argumentStart">
  ///   The zero-based starting index of the macro argument in the template source, or -1 if no
  ///   argument is present.
  /// </param>
  /// <param name="argumentLength">The length of the macro argument in characters. Must be in the range [0, 65535].</param>
  /// <returns>
  ///   A new <see cref="Segment" /> instance configured as either a standard macro segment (if <paramref name="slot" /> is
  ///   negative)
  ///   or a user macro segment (if <paramref name="slot" /> is non-negative).
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when any parameter is outside its valid range.
  /// </exception>
  /// <remarks>
  ///   This method provides a convenient way to create macro segments without explicitly specifying the macro kind.
  ///   The sign of the <paramref name="slot" /> parameter determines whether a standard or user-defined macro is created.
  /// </remarks>
  public static Segment CreateMacro(
    int slot,
    int nameStart,
    int nameLength,
    int argumentStart,
    int argumentLength )
  {
    return new Segment(
      slot < 0 ? SegmentKind.StandardMacro : SegmentKind.UserMacro,
      new MacroSegment(
        Math.Abs( slot ),
        nameStart,
        nameLength,
        argumentStart,
        argumentLength
      )
    );
  }

  /// <summary>
  ///   Creates a user-defined macro segment representing a custom macro invocation within a template.
  /// </summary>
  /// <param name="slot">The slot index where the macro resolver is stored. Must be in the range [0, 65535].</param>
  /// <param name="nameStart">The zero-based starting index of the macro name in the template source.</param>
  /// <param name="nameLength">The length of the macro name in characters. Must be in the range [1, 65535].</param>
  /// <param name="argumentStart">
  ///   The zero-based starting index of the macro argument in the template source, or -1 if no
  ///   argument is present.
  /// </param>
  /// <param name="argumentLength">The length of the macro argument in characters. Must be in the range [0, 65535].</param>
  /// <returns>A new <see cref="Segment" /> instance configured as a user macro segment.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when any parameter is outside its valid range.
  /// </exception>
  [Obsolete(
    "Use CreateMacro with a non-negative slot value instead. This method will be removed in a future version."
  )]
  public static Segment CreateUserMacro(
    int slot,
    int nameStart,
    int nameLength,
    int argumentStart,
    int argumentLength )
  {
    return new Segment(
      SegmentKind.UserMacro,
      new MacroSegment(
        slot,
        nameStart,
        nameLength,
        argumentStart,
        argumentLength
      )
    );
  }

  /// <summary>
  ///   Creates a standard macro segment representing a built-in macro invocation within a template.
  /// </summary>
  /// <param name="slot">The slot index where the standard macro handler is stored. Must be in the range [0, 65535].</param>
  /// <param name="nameStart">The zero-based starting index of the macro name in the template source.</param>
  /// <param name="nameLength">The length of the macro name in characters. Must be in the range [1, 65535].</param>
  /// <param name="argumentStart">
  ///   The zero-based starting index of the macro argument in the template source, or -1 if no
  ///   argument is present.
  /// </param>
  /// <param name="argumentLength">The length of the macro argument in characters. Must be in the range [0, 65535].</param>
  /// <returns>A new <see cref="Segment" /> instance configured as a standard macro segment.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when any parameter is outside its valid range.
  /// </exception>
  [Obsolete(
    "Use CreateMacro with a negative slot value instead. This method will be removed in a future version."
  )]
  public static Segment CreateStandardMacro(
    int slot,
    int nameStart,
    int nameLength,
    int argumentStart,
    int argumentLength )
  {
    return new Segment(
      SegmentKind.StandardMacro,
      new MacroSegment(
        slot,
        nameStart,
        nameLength,
        argumentStart,
        argumentLength
      )
    );
  }

  #endregion

  #region Implementation

  /// <summary>
  ///   Generates a human-readable string representation of the segment for debugging purposes.
  /// </summary>
  /// <returns>A formatted string describing the segment's type and contents.</returns>
  /// <remarks>
  ///   This method is used by the debugger display attribute and utilizes a pooled <see cref="StringBuilder" />
  ///   to minimize allocations during debugging sessions.
  /// </remarks>
  internal string GetDebuggerString()
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      if( _kind == SegmentKind.Constant )
      {
        Constant.GetDebuggerString( builder );
        return builder.ToString();
      }

      builder.Append( _kind == SegmentKind.StandardMacro ? "Standard" : "User" );
      Macro.GetDebuggerString( builder );

      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  #endregion
}
