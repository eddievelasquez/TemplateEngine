// Module Name: ConstantSegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;

[Trait( "Category", "Segments" )]
public class ConstantSegmentTest
{
  #region Tests

  [Theory]
  [InlineData( 0, 0 )]
  [InlineData( int.MaxValue, int.MaxValue )]
  [InlineData( 100, 0 )]
  [InlineData( 0, 100 )]
  public void CreateConstant_ShouldHandleEdgeCaseValues(
    int start,
    int length )
  {
    var segment = Segment.CreateConstant( start, length );
    var constantSegment = segment.Constant;

    constantSegment.TextStart.Should().Be( start );
    constantSegment.TextLength.Should().Be( length );
  }

  [Fact]
  public void CreateConstant_ShouldInitializeFields()
  {
    var segment = Segment.CreateConstant( 10, 20 );
    var constantSegment = segment.Constant;

    constantSegment.TextStart.Should().Be( 10 );
    constantSegment.TextLength.Should().Be( 20 );
  }

  [Fact]
  public void CreateConstant_ShouldReturnConstantKindAndExpectedText_WhenTemplateContainsText()
  {
    var template = CreateTemplate( "Hello World, this is a test!" );
    var segment = Segment.CreateConstant( 0, 11 );

    segment.Kind.Should().Be( SegmentKind.Constant );
    segment.Constant.GetText( template ).Should().Be( "Hello World" );
    segment.Constant.GetTextSpan( template ).ToString().Should().Be( "Hello World" );
  }

  [Fact]
  public void CreateConstant_ShouldThrow_WhenTextLengthIsNegative()
  {
    var act = () => Segment.CreateConstant( 0, -1 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "textLength" );
  }

  [Fact]
  public void CreateConstant_ShouldThrow_WhenTextStartIsNegative()
  {
    var act = () => Segment.CreateConstant( -1, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "textStart" );
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleMaxValues()
  {
    var segment = Segment.CreateConstant( int.MaxValue, int.MaxValue );
    var constantSegment = segment.Constant;
    var builder = new StringBuilder();

    constantSegment.GetDebuggerString( builder );

    builder.ToString()
           .Should()
           .Be( $"Constant {{ TextStart: {int.MaxValue}, TextLength: {int.MaxValue} }}" );
  }

  [Fact]
  public void GetDebuggerString_ShouldHandleZeroValues()
  {
    var segment = Segment.CreateConstant( 0, 0 );
    var constantSegment = segment.Constant;
    var builder = new StringBuilder();

    constantSegment.GetDebuggerString( builder );

    builder.ToString().Should().Be( "Constant { TextStart: 0, TextLength: 0 }" );
  }

  [Fact]
  public void GetDebuggerString_ShouldReturnFormattedString()
  {
    var segment = Segment.CreateConstant( 10, 20 );
    var constantSegment = segment.Constant;
    var builder = new StringBuilder();

    constantSegment.GetDebuggerString( builder );

    builder.ToString().Should().Be( "Constant { TextStart: 10, TextLength: 20 }" );
  }

  [Fact]
  public void GetText_ShouldReturnCorrectSubstring_WhenLengthIsNonZero()
  {
    var template = CreateTemplate( "Hello World" );
    var segment = Segment.CreateConstant( 6, 5 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetText( template );

    result.Should().Be( "World" );
  }

  [Fact]
  public void GetText_ShouldReturnEmptyString_WhenLengthIsZero()
  {
    var template = CreateTemplate( "Hello World" );
    var segment = Segment.CreateConstant( 0, 0 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetText( template );

    result.Should().BeEmpty();
  }

  [Fact]
  public void GetText_ShouldReturnEntireString_WhenStartIsZeroAndLengthIsMax()
  {
    const string Text = "Full Text";
    var template = CreateTemplate( Text );
    var segment = Segment.CreateConstant( 0, Text.Length );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetText( template );

    result.Should().Be( Text );
  }

  [Fact]
  public void GetText_ShouldReturnSingleCharacter_WhenLengthIsOne()
  {
    var template = CreateTemplate( "ABCDEF" );
    var segment = Segment.CreateConstant( 3, 1 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetText( template );

    result.Should().Be( "D" );
  }

  [Fact]
  public void GetTextSpan_ShouldReturnCorrectSpan_WhenLengthIsNonZero()
  {
    var template = CreateTemplate( "Hello World" );
    var segment = Segment.CreateConstant( 0, 5 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetTextSpan( template );

    result.ToString().Should().Be( "Hello" );
  }

  [Fact]
  public void GetTextSpan_ShouldReturnEmptySpan_WhenLengthIsZero()
  {
    var template = CreateTemplate( "Hello World" );
    var segment = Segment.CreateConstant( 0, 0 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetTextSpan( template );

    result.IsEmpty.Should().BeTrue();
  }

  [Fact]
  public void GetTextSpan_ShouldReturnEntireSpan_WhenStartIsZeroAndLengthIsMax()
  {
    const string Text = "Complete String";
    var template = CreateTemplate( Text );
    var segment = Segment.CreateConstant( 0, Text.Length );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetTextSpan( template );

    result.ToString().Should().Be( Text );
  }

  [Fact]
  public void GetTextSpan_ShouldReturnLastCharacter_WhenStartIsLastIndex()
  {
    const string Text = "ABCDEF";
    var template = CreateTemplate( Text );
    var segment = Segment.CreateConstant( Text.Length - 1, 1 );
    var constantSegment = segment.Constant;

    var result = constantSegment.GetTextSpan( template );

    result.ToString().Should().Be( "F" );
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
