// Module Name: MacroSegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;

[Trait( "Category", "Segments" )]
public class MacroSegmentTest
{
  #region Tests

  [Theory]
  [InlineData( -1, 20, 8 )]
  [InlineData( -5, 200, 20 )]
  public void CreateMacro_ShouldCreateStandardMacro_WhenSlotIsNegative(
    int slot,
    int argStart,
    int argLength )
  {
    var segment = Segment.CreateMacro( slot, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.Macro );
    segment.Macro.Slot.Should().Be( slot );
    segment.Macro.IsStandardMacro.Should().BeTrue();
    segment.Macro.IsUserMacro.Should().BeFalse();
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( ( ushort ) argLength );
  }

  [Theory]
  [InlineData( 1, 20, 8 )]
  [InlineData( 5, 200, 20 )]
  public void CreateMacro_ShouldCreateUserMacro_WhenSlotIsNonNegative(
    int slot,
    int argStart,
    int argLength )
  {
    var segment = Segment.CreateMacro( slot, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.Macro );
    segment.Macro.Slot.Should().Be( ( ushort ) slot );
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( ( ushort ) argLength );
  }

  [Fact]
  public void CreateMacro_ShouldCreateUserMacro_WhenSlotIsZero()
  {
    var segment = Segment.CreateMacro( 0, 20, 8 );

    segment.Kind.Should().Be( SegmentKind.Macro );
    segment.Macro.Slot.Should().Be( 0 );
    segment.Macro.ArgumentStart.Should().Be( 20 );
    segment.Macro.ArgumentLength.Should().Be( 8 );
  }

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
  public void GetArgumentSpan_ShouldReturnEmptySpan_WhenArgumentStartIsMinus1()
  {
    var template = CreateTemplate( "$macro$" );
    var segment = Segment.CreateMacro( 1, -1, 0 ).Macro;

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
  public void GetDebuggerString_ShouldReturnFormattedString()
  {
    var segment = Segment.CreateMacro( 5, 20, 25 ).Macro;
    var builder = new StringBuilder();

    segment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be( "Macro { Slot: 5, ArgumentStart: 20, ArgumentLength: 25 }" );
  }

  [Theory]
  [InlineData( -1, 20, 8 )]
  [InlineData( 1, 20, 8 )]
  public void GetDebuggerString_ShouldReturnMacroString(
    int slot,
    int argStart,
    int argLength )
  {
    var segment = Segment.CreateMacro( slot, argStart, argLength );

    var result = segment.GetDebuggerString();

    result.Should()
          .Be(
            $"Macro {{ Slot: {slot}, ArgumentStart: {argStart}, ArgumentLength: {argLength} }}"
          );
  }

  [Fact]
  public void MacroProperty_ShouldBeAccessible_WhenKindIsStandardMacro()
  {
    var segment = Segment.CreateMacro( 7, 80, 18 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 7 );
    macro.ArgumentStart.Should().Be( 80 );
    macro.ArgumentLength.Should().Be( 18 );
  }

  [Fact]
  public void MacroProperty_ShouldBeAccessible_WhenKindIsUserMacro()
  {
    var segment = Segment.CreateMacro( 3, 70, 15 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 3 );
    macro.ArgumentStart.Should().Be( 70 );
    macro.ArgumentLength.Should().Be( 15 );
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
