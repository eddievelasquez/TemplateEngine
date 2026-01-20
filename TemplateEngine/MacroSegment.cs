// Module Name: MacroSegment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Runtime.InteropServices;
using System.Text;

// Size: 16 bytes (naturally aligned)

/// <summary>
///   Represents a macro segment within a template, storing the position and length of the macro name
///   and its optional argument, along with the slot index for the macro resolver.
/// </summary>
/// <remarks>
///   This struct uses sequential layout with natural alignment for optimal CPU memory access patterns.
///   It occupies exactly 16 bytes with all fields aligned to their natural boundaries (4-byte alignment
///   for <see cref="int" /> fields, 2-byte alignment for <see cref="ushort" /> fields). The slot index
///   references the resolver that will handle the macro expansion during template processing.
/// </remarks>
[StructLayout( LayoutKind.Sequential )]
internal readonly struct MacroSegment
{
  #region Fields

  /// <summary>
  ///   The slot index where the macro resolver is stored.
  /// </summary>
  public readonly int Slot;

  /// <summary>
  ///   The zero-based starting index of the macro argument in the template source string, or -1 if no argument is present.
  /// </summary>
  public readonly int ArgumentStart;

  /// <summary>
  ///   The length of the macro argument in characters. Valid range is [0, 65535]. Zero indicates no argument.
  /// </summary>
  public readonly ushort ArgumentLength;

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroSegment" /> struct.
  /// </summary>
  /// <param name="slot">The slot index where the macro resolver is stored. Must be in the range [0, 65535].</param>
  /// <param name="argumentStart">
  ///   The zero-based starting index of the macro argument in the template source, or -1 if no
  ///   argument is present. Must be greater than or equal to -1.
  /// </param>
  /// <param name="argumentLength">The length of the macro argument in characters. Must be in the range [0, 65535].</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when any parameter is outside its valid range.
  /// </exception>
  internal MacroSegment(
    int slot,
    int argumentStart,
    int argumentLength )
  {
    if( argumentStart < -1 )
    {
      throw new ArgumentOutOfRangeException( nameof( argumentStart ) );
    }

    if( argumentLength is < 0 or > ushort.MaxValue )
    {
      throw new ArgumentOutOfRangeException( nameof( argumentLength ) );
    }

    Slot = slot;
    ArgumentStart = argumentStart;
    ArgumentLength = ( ushort ) argumentLength;
  }

  #endregion

  /// <summary>
  ///   Indicates whether this segment represents a user-defined macro.
  /// </summary>
  public bool IsUserMacro => Slot >= 0;

  /// <summary>
  ///   Indicates whether this segment represents a standard macro.
  /// </summary>
  public bool IsStandardMacro => Slot < 0;

  #region Public Methods

  /// <summary>
  ///   Retrieves the macro argument as a read-only character span without allocating a new string.
  /// </summary>
  /// <param name="template">The template containing the source text.</param>
  /// <returns>
  ///   A <see cref="ReadOnlySpan{T}" /> of characters representing the macro argument,
  ///   or <see cref="ReadOnlySpan{T}.Empty" /> if <see cref="ArgumentLength" /> is zero.
  /// </returns>
  /// <remarks>
  ///   This method provides zero-allocation access to the macro argument text for efficient processing
  ///   in template expansion scenarios.
  /// </remarks>
  public ReadOnlySpan<char> GetArgumentSpan(
    Template template )
  {
    return ArgumentLength != 0
      ? template.Text.AsSpan( ArgumentStart, ArgumentLength )
      : ReadOnlySpan<char>.Empty;
  }

  #endregion

  #region Implementation

  /// <summary>
  ///   Appends a formatted representation of this macro segment to the provided string builder for debugging.
  /// </summary>
  /// <param name="builder">The <see cref="StringBuilder" /> to append the debug information to.</param>
  internal void GetDebuggerString(
    StringBuilder builder )
  {
    builder.Append( "Macro { " );
    builder.Append( "Slot: " );
    builder.Append( Slot );
    builder.Append( ", ArgumentStart: " );
    builder.Append( ArgumentStart );
    builder.Append( ", ArgumentLength: " );
    builder.Append( ArgumentLength );

    builder.Append( " }" );
  }

  #endregion
}
