// Module Name: MacroSegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;

public class MacroSegmentTest
{
  #region Tests

  [Theory]
  [InlineData( 0, 0, 1, 0, 0 )]
  [InlineData( ushort.MaxValue, int.MaxValue, ushort.MaxValue, int.MaxValue, ushort.MaxValue )]
  public void CreateUserMacro_ShouldHandleEdgeCaseValues(
    ushort slot,
    int nameStart,
    ushort nameLength,
    int argStart,
    ushort argLength )
  {
    var segment = Segment.CreateUserMacro( slot, nameStart, nameLength, argStart, argLength )
                         .Macro;

    segment.Slot.Should().Be( slot );
    segment.NameStart.Should().Be( nameStart );
    segment.NameLength.Should().Be( nameLength );
    segment.ArgumentStart.Should().Be( argStart );
    segment.ArgumentLength.Should().Be( argLength );
  }

  [Fact]
  public void CreateUserMacro_ShouldInitializeAllFields()
  {
    var segment = Segment.CreateUserMacro( 5, 10, 15, 20, 25 ).Macro;

    segment.Slot.Should().Be( 5 );
    segment.NameStart.Should().Be( 10 );
    segment.NameLength.Should().Be( 15 );
    segment.ArgumentStart.Should().Be( 20 );
    segment.ArgumentLength.Should().Be( 25 );
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnCorrectSpan_WhenArgumentExists()
  {
    var template = CreateTemplate( "$macro:argument$" );
    var segment = Segment.CreateUserMacro( 1, 1, 5, 7, 8 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.ToString().Should().Be( "argument" );
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnEmptySpan_WhenArgumentLengthIsZero()
  {
    var template = CreateTemplate( "$macro$" );
    var segment = Segment.CreateUserMacro( 1, 1, 5, 0, 0 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.IsEmpty.Should().BeTrue();
  }

  [Fact]
  public void GetArgumentSpan_ShouldReturnSingleCharacter_WhenArgumentLengthIsOne()
  {
    var template = CreateTemplate( "$m:X$" );
    var segment = Segment.CreateUserMacro( 1, 1, 1, 3, 1 ).Macro;
    var result = segment.GetArgumentSpan( template );

    result.ToString().Should().Be( "X" );
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleAllZeroValues()
  {
    var segment = Segment.CreateUserMacro( 0, 0, 1, 0, 0 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be(
             "Macro { Slot: 0, NameStart: 0, NameLength: 1, ArgumentStart: 0, ArgumentLength: 0 }"
           );
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleMaxValues()
  {
    var segment = Segment.CreateUserMacro(
                           ushort.MaxValue,
                           int.MaxValue,
                           ushort.MaxValue,
                           int.MaxValue,
                           ushort.MaxValue
                         )
                         .Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    var expected =
      $"Macro {{ Slot: {ushort.MaxValue}, NameStart: {int.MaxValue}, NameLength: {ushort.MaxValue}, ArgumentStart: {int.MaxValue}, ArgumentLength: {ushort.MaxValue} }}";

    builder.ToString().Should().Be( expected );
  }

  [Fact]
  public void GetDebuggerString_ShouldOmitArgument_WhenNameLengthIsZero()
  {
    var segment = Segment.CreateUserMacro( 5, 10, 1, 20, 25 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be(
             "Macro { Slot: 5, NameStart: 10, NameLength: 1, ArgumentStart: 20, ArgumentLength: 25 }"
           );
  }

  [Fact]
  public void GetDebuggerString_ShouldReturnFormattedString_WithArgument()
  {
    var segment = Segment.CreateUserMacro( 5, 10, 15, 20, 25 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be(
             "Macro { Slot: 5, NameStart: 10, NameLength: 15, ArgumentStart: 20, ArgumentLength: 25 }"
           );
  }

  [Fact]
  public void GetName_ShouldReturnCorrectSubstring()
  {
    var template = CreateTemplate( "prefix$macroName$suffix" );
    var segment = Segment.CreateUserMacro( 1, 7, 9, 0, 0 ).Macro;

    var result = segment.GetName( template );

    result.Should().Be( "macroName" );
  }

  [Fact]
  public void GetName_ShouldReturnSingleCharacter_WhenNameLengthIsOne()
  {
    var template = CreateTemplate( "$X$" );
    var segment = Segment.CreateUserMacro( 1, 1, 1, 0, 0 ).Macro;

    var result = segment.GetName( template );

    result.Should().Be( "X" );
  }

  [Fact]
  public void GetNameSpan_ShouldReturnCorrectSpan()
  {
    var template = CreateTemplate( "prefix$macroName$suffix" );
    var segment = Segment.CreateUserMacro( 1, 7, 9, 0, 0 ).Macro;

    var result = segment.GetNameSpan( template );

    result.ToString().Should().Be( "macroName" );
  }

  [Fact]
  public void GetNameSpan_ShouldReturnEmptySpan_WhenNameLengthIsZero()
  {
    var template = CreateTemplate( "$$" );
    var segment = Segment.CreateUserMacro( 1, 1, 1, 0, 0 ).Macro;

    var result = segment.GetNameSpan( template );

    result.Length.Should().Be( 1 );
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
