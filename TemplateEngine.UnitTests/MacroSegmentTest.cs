// Module Name: MacroSegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;

public class MacroSegmentTest
{
  #region Tests

  [Fact]
  public void GetArgumentSpan_ShouldReturnCorrectSpan_WhenArgumentExists()
  {
    var template = CreateTemplate( "$macro:argument$" );
    var segment = Segment.CreateMacro( 1, 7, 8 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.ToString().Should().Be( "argument" );
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnEmptySpan_WhenArgumentLengthIsZero()
  {
    var template = CreateTemplate( "$macro$" );
    var segment = Segment.CreateMacro( 1, 0, 0 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.IsEmpty.Should().BeTrue();
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnSingleCharacter_WhenArgumentLengthIsOne()
  {
    var template = CreateTemplate( "$m:X$" );
    var segment = Segment.CreateMacro( 1, 3, 1 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.ToString().Should().Be( "X" );
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnEmptySpan_WhenArgumentStartIsMinus1()
  {
    var template = CreateTemplate( "$macro$" );
    var segment = Segment.CreateMacro( 1, -1, 0 ).Macro;

    var result = segment.GetArgumentSpan( template );

    result.IsEmpty.Should().BeTrue();
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleAllZeroValues()
  {
    var segment = Segment.CreateMacro( 0, 0, 0 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be( "Macro { Slot: 0, ArgumentStart: 0, ArgumentLength: 0 }" );
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleMaxValues()
  {
    var segment = Segment.CreateMacro( ushort.MaxValue, int.MaxValue, ushort.MaxValue ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    var expected =
      $"Macro {{ Slot: {ushort.MaxValue}, ArgumentStart: {int.MaxValue}, ArgumentLength: {ushort.MaxValue} }}";

    builder.ToString().Should().Be( expected );
  }

  [Fact]
  public void GetDebuggerString_ShouldOmitArgument_WhenNameLengthIsZero()
  {
    var segment = Segment.CreateMacro( 5, 20, 25 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be( "Macro { Slot: 5, ArgumentStart: 20, ArgumentLength: 25 }" );
  }

  [Fact]
  public void GetDebuggerString_ShouldReturnFormattedString_WithArgument()
  {
    var segment = Segment.CreateMacro( 5, 20, 25 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be( "Macro { Slot: 5, ArgumentStart: 20, ArgumentLength: 25 }" );
  }

  #endregion

  #region Implementation

  internal static Template CreateTemplate(
    string text )
  {
    var macroTable = new MacroTableBuilder().Declare( "test" ).Build();
    var segments = new[] { Segment.CreateConstant( 0, text.Length ) };

    return new Template( text, macroTable, segments );
  }

  #endregion
}
