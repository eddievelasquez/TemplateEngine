// Module Name: MacroTableTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

[Trait( "Category", "Processing" )]
public class MacroTableTest
{
  #region Tests

  [Fact]
  public void GetSlot_WithString_ShouldPreferUserDefinedMacro_WhenNameMatchesStandardMacro()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.GuidMacroName );
    var table = builder.Build();

    var slot = table.GetSlot( StandardMacros.GuidMacroName );
    slot.Should().BePositive();
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnCorrectSlot_WhenMultipleMacrosDeclared()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "A" );
    builder.Declare( "B" );
    builder.Declare( "C" );

    var table = builder.Build();
    table.GetSlot( "A" ).Should().Be( 1 );
    table.GetSlot( "B" ).Should().Be( 2 );
    table.GetSlot( "C" ).Should().Be( 3 );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnMinusOne_WhenMacroDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "BAR" ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnNotFound_WhenMacroNameIsWhitespace()
  {
    var table = new MacroTableBuilder().Build();
    table.GetSlot( "   " ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnSlotIndex_WhenMacroExists_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "FOO" ).Should().Be( 1 );
    table.GetSlot( "foo" ).Should().Be( 1 );
    table.GetSlot( "FoO" ).Should().Be( 1 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void GetSlot_WithString_ShouldThrow_WhenMacroNameIsNullOrEmpty(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "Foo" );

    var table = builder.Build();
    Action act = () => table.GetSlot( macroName! );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void GetMacroName_WithInt_ShouldReturnMacroName_WhenSlotExists()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "BAR" );

    var table = builder.Build();
    table.GetMacroName( 1 ).Should().Be( "FOO" );
    table.GetMacroName( 2 ).Should().Be( "BAR" );
  }

  [Fact]
  public void GetMacroName_WithInt_ShouldThrow_WhenSlotDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    Action act = () => table.GetMacroName( 999 );
    act.Should().Throw<KeyNotFoundException>();
  }

  [Fact]
  public void GetMacroName_WithSegment_ShouldReturnMacroName_WhenUserMacroSegment()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "BAR" );

    var table = builder.Build();
    var segment = Segment.CreateMacro( 1, -1, 0 );
    table.GetMacroName( segment ).Should().Be( "FOO" );

    segment = Segment.CreateMacro( 2, -1, 0 );
    table.GetMacroName( segment ).Should().Be( "BAR" );
  }

  [Fact]
  public void GetMacroName_WithSegment_ShouldThrow_WhenConstantSegment()
  {
    var table = new MacroTableBuilder().Build();
    var segment = Segment.CreateConstant( 0, 10 );
    Action act = () => table.GetMacroName( segment );
    act.Should().Throw<ArgumentException>().WithMessage( "Must be a macro segment*" );
  }

  [Fact]
  public void TryGetMacroName_WithInt_ShouldReturnTrueAndMacroName_WhenSlotExists()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "BAR" );

    var table = builder.Build();
    var result = table.TryGetMacroName( 1, out var macroName );
    result.Should().BeTrue();
    macroName.Should().Be( "FOO" );

    result = table.TryGetMacroName( 2, out macroName );
    result.Should().BeTrue();
    macroName.Should().Be( "BAR" );
  }

  [Fact]
  public void TryGetMacroName_WithInt_ShouldReturnFalseAndNull_WhenSlotDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    var result = table.TryGetMacroName( 999, out var macroName );
    result.Should().BeFalse();
    macroName.Should().BeNull();
  }

  [Fact]
  public void TryGetMacroName_WithSegment_ShouldReturnTrueAndMacroName_WhenUserMacroSegment()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "BAR" );

    var table = builder.Build();
    var segment = Segment.CreateMacro( 1, -1, 0 );
    var result = table.TryGetMacroName( segment, out var macroName );
    result.Should().BeTrue();
    macroName.Should().Be( "FOO" );

    segment = Segment.CreateMacro( 2, -1, 0 );
    result = table.TryGetMacroName( segment, out macroName );
    result.Should().BeTrue();
    macroName.Should().Be( "BAR" );
  }

  [Fact]
  public void TryGetMacroName_WithSegment_ShouldReturnFalseAndNull_WhenSlotDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    var segment = Segment.CreateMacro( 999, -1, 0 );
    var result = table.TryGetMacroName( segment, out var macroName );
    result.Should().BeFalse();
    macroName.Should().BeNull();
  }

  [Fact]
  public void TryGetMacroName_WithSegment_ShouldThrow_WhenConstantSegment()
  {
    var table = new MacroTableBuilder().Build();
    var segment = Segment.CreateConstant( 0, 10 );
    Action act = () => table.TryGetMacroName( segment, out _ );
    act.Should().Throw<ArgumentException>().WithMessage( "Must be a macro segment*" );
  }

  #endregion

#if NET9_0_OR_GREATER
  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnMinusOne_WhenMacroDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "BAR".AsSpan() ).Should().Be( 0 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnMinusOne_WhenMacroIsEmpty()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( ReadOnlySpan<char>.Empty ).Should().Be( 0 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnNotFound_WhenMacroWhitespace()
  {
    var table = new MacroTableBuilder().Build();
    table.GetSlot( "   ".AsSpan() ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldPreferUserDefinedMacro_WhenNameMatchesStandardMacro()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.EnvMacroName );
    var table = builder.Build();

    table.GetSlot( StandardMacros.EnvMacroName.AsSpan() ).Should().BePositive();
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnSlotIndex_WhenMacroExists_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "FOO".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "foo".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "FoO".AsSpan() ).Should().Be( 1 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnCorrectSlot_WhenMultipleMacrosDeclared()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "A" );
    builder.Declare( "B" );
    builder.Declare( "C" );

    var table = builder.Build();
    table.GetSlot( "A".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "B".AsSpan() ).Should().Be( 2 );
    table.GetSlot( "C".AsSpan() ).Should().Be( 3 );
  }

#endif
}
