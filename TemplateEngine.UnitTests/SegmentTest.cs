// Module Name: SegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using ObjectLayoutInspector;
using Xunit.Abstractions;

public class SegmentTest
{
  #region Fields

  private readonly ITestOutputHelper _outputHelper;

  #endregion

  #region Setup/Teardown

  public SegmentTest(
    ITestOutputHelper outputHelper )
  {
    _outputHelper = outputHelper;
  }

  #endregion

  #region Tests

  [Fact]
  public void Segment_ConstantProperty_ShouldBeAccessible_WhenKindIsConstant()
  {
    var segment = Segment.CreateConstant( 42, 100 );

    var constant = segment.Constant;

    constant.TextStart.Should().Be( 42 );
    constant.TextLength.Should().Be( 100 );
  }

  [Fact]
  public void Segment_ConstantSegment_IntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "Hello World, this is a test!" );
    var segment = Segment.CreateConstant( 0, 11 );

    segment.Kind.Should().Be( SegmentKind.Constant );

    var text = segment.Constant.GetText( template );
    text.Should().Be( "Hello World" );

    var span = segment.Constant.GetTextSpan( template );
    span.ToString().Should().Be( "Hello World" );
  }

  [Theory]
  [InlineData( -1, 20, 8 )]
  [InlineData( -5, 200, 20 )]
  public void Segment_CreateMacro_ShouldCreateStandardMacro_WhenSlotIsNegative(
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
  public void Segment_CreateMacro_ShouldCreateUserMacro_WhenSlotIsNonNegative(
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
  public void Segment_CreateMacro_ShouldCreateUserMacro_WhenSlotIsZero()
  {
    var segment = Segment.CreateMacro( 0, 20, 8 );

    segment.Kind.Should().Be( SegmentKind.Macro );
    segment.Macro.Slot.Should().Be( 0 );
    segment.Macro.ArgumentStart.Should().Be( 20 );
    segment.Macro.ArgumentLength.Should().Be( 8 );
  }

  [Fact]
  public void Segment_CreateMacro_StandardMacroIntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$now:yyyy-MM-dd$" );

    // Using CreateMacro with slot=-1 creates a standard macro (slot 0 internally after adjustment)
    var segment = Segment.CreateMacro( -1, 5, 10 );

    segment.Kind.Should().Be( SegmentKind.Macro );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "yyyy-MM-dd" );
  }

  [Fact]
  public void Segment_CreateMacro_UserMacroIntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$userName:John$" );

    // Using CreateMacro with slot=2 creates a user macro (slot 1 internally after adjustment)
    var segment = Segment.CreateMacro( 2, 10, 4 );

    segment.Kind.Should().Be( SegmentKind.Macro );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "John" );
  }

  [Fact]
  public void Segment_GetDebuggerString_ShouldReturnConstantString_WhenKindIsConstant()
  {
    var segment = Segment.CreateConstant( 10, 20 );

    var result = segment.GetDebuggerString();

    result.Should().Be( "Constant { TextStart: 10, TextLength: 20 }" );
  }

  [Theory]
  [InlineData( -1, 20, 8 )]
  [InlineData( 1, 20, 8 )]
  public void Segment_GetDebuggerString_ShouldReturnMacroString(
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
  public void Segment_MacroProperty_ShouldBeAccessible_WhenKindIsStandardMacro()
  {
    var segment = Segment.CreateMacro( 7, 80, 18 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 7 );
    macro.ArgumentStart.Should().Be( 80 );
    macro.ArgumentLength.Should().Be( 18 );
  }

  [Fact]
  public void Segment_MacroProperty_ShouldBeAccessible_WhenKindIsUserMacro()
  {
    var segment = Segment.CreateMacro( 3, 70, 15 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 3 );
    macro.ArgumentStart.Should().Be( 70 );
    macro.ArgumentLength.Should().Be( 15 );
  }

  [Fact]
  public void Segment_ShouldBeExactly20Bytes()
  {
    var layout = TypeLayout.GetLayout<Segment>();
    var s = layout.ToString( true );
    _outputHelper.WriteLine( s );

    layout.FullSize.Should().Be( 20 );
  }

  [Fact]
  public void Segment_StandardMacroSegment_IntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$now:yyyy-MM-dd$" );
    var segment = Segment.CreateMacro( -1, 5, 10 );

    segment.Kind.Should().Be( SegmentKind.Macro );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "yyyy-MM-dd" );
  }

  [Fact]
  public void Segment_UnionBehavior_ConstantAndMacroShareSameMemory()
  {
    var segment = Segment.CreateConstant( 12345, 67890 );

    segment.Kind.Should().Be( SegmentKind.Constant );
    segment.Constant.TextStart.Should().Be( 12345 );
    segment.Constant.TextLength.Should().Be( 67890 );
  }

  [Fact]
  public void Segment_UserMacroSegment_IntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$userName:John$" );
    var segment = Segment.CreateMacro( 1, 10, 4 );

    segment.Kind.Should().Be( SegmentKind.Macro );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "John" );
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
