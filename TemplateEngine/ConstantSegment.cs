// Module Name: ConstantSegment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Runtime.InteropServices;
using System.Text;

// Size: 8 bytes (naturally aligned)

/// <summary>
///   Represents a constant (literal) text segment within a template, storing the position and length
///   of the text in the original template source.
/// </summary>
/// <remarks>
///   This struct uses sequential layout with natural alignment. It occupies exactly 8 bytes with both
///   <see cref="int" /> fields aligned to 4-byte boundaries for optimal CPU memory access patterns.
/// </remarks>
[StructLayout( LayoutKind.Sequential )]
internal readonly struct ConstantSegment
{
  #region Fields

  /// <summary>
  ///   The zero-based starting index of the constant text in the template source string.
  /// </summary>
  public readonly int TextStart;

  /// <summary>
  ///   The length of the constant text in characters.
  /// </summary>
  public readonly int TextLength;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="ConstantSegment" /> struct.
  /// </summary>
  /// <param name="textStart">
  ///   The zero-based starting index of the constant text in the template source. Must be
  ///   non-negative.
  /// </param>
  /// <param name="textLength">The length of the constant text in characters. Must be non-negative.</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when <paramref name="textStart" /> or <paramref name="textLength" /> is negative.
  /// </exception>
  internal ConstantSegment(
    int textStart,
    int textLength )
  {
    if( textStart < 0 )
    {
      throw new ArgumentOutOfRangeException( nameof( textStart ) );
    }

    if( textLength < 0 )
    {
      throw new ArgumentOutOfRangeException( nameof( textLength ) );
    }

    TextStart = textStart;
    TextLength = textLength;
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Retrieves the constant text as a string by extracting the substring from the template source.
  /// </summary>
  /// <param name="template">The template containing the source text.</param>
  /// <returns>
  ///   The constant text as a <see cref="string" />, or <see cref="string.Empty" /> if <see cref="TextLength" /> is zero.
  /// </returns>
  public string GetText(
    Template template )
  {
    return TextLength != 0 ? template.Text.Substring( TextStart, TextLength ) : string.Empty;
  }

  /// <summary>
  ///   Retrieves the constant text as a read-only character span without allocating a new string.
  /// </summary>
  /// <param name="template">The template containing the source text.</param>
  /// <returns>
  ///   A <see cref="ReadOnlySpan{T}" /> of characters representing the constant text,
  ///   or <see cref="ReadOnlySpan{T}.Empty" /> if <see cref="TextLength" /> is zero.
  /// </returns>
  /// <remarks>
  ///   This method provides zero-allocation access to the constant text and should be preferred
  ///   over <see cref="GetText" /> in performance-critical scenarios.
  /// </remarks>
  public ReadOnlySpan<char> GetTextSpan(
    Template template )
  {
    return TextLength != 0
      ? template.Text.AsSpan( TextStart, TextLength )
      : ReadOnlySpan<char>.Empty;
  }

  #endregion

  #region Implementation

  /// <summary>
  ///   Appends a formatted representation of this constant segment to the provided string builder for debugging.
  /// </summary>
  /// <param name="builder">The <see cref="StringBuilder" /> to append the debug information to.</param>
  internal void GetDebuggerString(
    StringBuilder builder )
  {
    builder.Append( "Constant { " );
    builder.Append( "TextStart: " );
    builder.Append( TextStart );
    builder.Append( ", TextLength: " );
    builder.Append( TextLength );
    builder.Append( " }" );
  }

  #endregion
}
