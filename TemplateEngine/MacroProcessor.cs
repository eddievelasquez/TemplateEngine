// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Text;

/// <summary>
///   Processes macros in a template.
/// </summary>
public static class MacroProcessor
{
  #region Public Methods

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="macroValues">The macro values to use for processing the template.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="macroValues" /> instance is not associated with the same
  ///   <see cref="MacroTable" /> as the <paramref name="template" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   Thrown when an unknown segment kind is encountered during processing.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    MacroValues macroValues )
  {
    if( template.MacroTable != macroValues.MacroTable )
    {
      throw new ArgumentException(
        "The MacroValues instance must be associated with the same MacroTable as the Template.",
        nameof( macroValues )
      );
    }

    // Pre-allocate the StringBuilder capacity to avoid multiple allocations during appends as much as possible
    builder.EnsureCapacity( template.Text.Length );

    var segments = template.Segments;
    var length = segments.Length;

    for( var index = 0; index < length; index++ )
    {
      var segment = segments[index];

      // Handle constant segments
      if( segment.Kind == SegmentKind.Constant )
      {
#if NET6_0_OR_GREATER
        builder.Append( segment.Constant.GetTextSpan( template ) );
#else

        // The .netstandard2.0 StringBuilder.Append method does not have a Span overload.
        builder.Append( segment.Constant.GetText( template ) );
#endif
        continue;
      }

      // Handle macro segments (Macro or StandardMacro)
      string value;

      try
      {
        var macro = segment.Macro;

        value = macroValues.GetValue( macro.Slot, macro.GetArgumentSpan( template ) ) ??
                string.Empty;
      }
      catch( Exception exception )
      {
        value = exception.Message;
      }

      builder.Append( value );
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="values">
  ///   A span of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    params ReadOnlySpan<string?> values )
  {
    if( values.Length < template.MacroTable.Count )
    {
      throw new ArgumentException(
        "The values array must have at least as many elements as the MacroTable has slots.",
        nameof( values )
      );
    }

    // Pre-allocate the StringBuilder capacity to avoid multiple allocations during appends as much as possible
    builder.EnsureCapacity( template.Text.Length );

    var segments = template.Segments;
    var length = segments.Length;

    for( var index = 0; index < length; index++ )
    {
      var segment = segments[index];

      // Handle constant segments
      if( segment.Kind == SegmentKind.Constant )
      {
#if NET6_0_OR_GREATER
        builder.Append( segment.Constant.GetTextSpan( template ) );
#else

        // The .netstandard2.0 StringBuilder.Append method does not have a Span overload.
        builder.Append( segment.Constant.GetText( template ) );
#endif
        continue;
      }

      // Handle macro segments
      var slot = segment.Macro.Slot;

      // Negative slots are standard macros.
      var value = slot < 0 ? StandardMacros.GetValue( slot ) : values[slot - 1];

      if( value is not null )
      {
        builder.Append( value );
      }
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    params string?[] values )
  {
    template.ProcessMacros( builder, values.AsSpan() );
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> and returns the resulting string.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="macroValues">The <see cref="MacroValues" /> providing values for the macros in the template.</param>
  /// <returns>A string with the macros in the template replaced by their corresponding values.</returns>
  /// <remarks>
  ///   This method processes the macros in the provided template using the specified macro values.
  ///   It utilizes a pooled <see cref="StringBuilder" /> for efficient string manipulation.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="template" /> or <paramref name="macroValues" /> is <c>null</c>.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    MacroValues macroValues )
  {
    var pool = StringBuilderPool.Default;
    var builder = pool.Get();

    try
    {
      template.ProcessMacros( builder, macroValues );
      return builder.ToString();
    }
    finally
    {
      pool.Return( builder );
    }
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> using the provided values and returns the result as a
  ///   string.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    params ReadOnlySpan<string?> values )
  {
    var pool = StringBuilderPool.Default;
    var builder = pool.Get();

    try
    {
      template.ProcessMacros( builder, values );
      return builder.ToString();
    }
    finally
    {
      pool.Return( builder );
    }
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> using the provided values and writes the result to the
  ///   specified <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    params string?[] values )
  {
    return template.ProcessMacros( values.AsSpan() );
  }

  #endregion
}
