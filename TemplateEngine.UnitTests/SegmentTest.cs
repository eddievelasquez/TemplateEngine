// Module Name: SegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using ObjectLayoutInspector;
using Xunit.Abstractions;

public class SegmentTest
{
  private readonly ITestOutputHelper _outputHelper;

  public SegmentTest(
    ITestOutputHelper outputHelper )
  {
    _outputHelper = outputHelper;
  }

  #region Tests

  [Fact]
  public void Segment_ShouldBeExactly20Bytes()
  {
    var layout = TypeLayout.GetLayout<Segment>();
    var s = layout.ToString( true );
    _outputHelper.WriteLine( s );

    layout.FullSize.Should().Be( 20 );
  }

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
  [InlineData( 0, 0 )]
  [InlineData( int.MaxValue, int.MaxValue )]
  [InlineData( 42, 100 )]
  public void Segment_CreateConstant_ShouldHandleVariousValues(
    int start,
    int length )
  {
    var segment = Segment.CreateConstant( start, length );

    segment.Kind.Should().Be( SegmentKind.Constant );
    segment.Constant.TextStart.Should().Be( start );
    segment.Constant.TextLength.Should().Be( length );
  }

  [Fact]
  public void Segment_CreateConstant_ShouldInitializeConstantSegment()
  {
    var segment = Segment.CreateConstant( 10, 20 );

    segment.Constant.TextStart.Should().Be( 10 );
    segment.Constant.TextLength.Should().Be( 20 );
  }

  [Fact]
  public void Segment_CreateConstant_ShouldSetKindToConstant()
  {
    var segment = Segment.CreateConstant( 10, 20 );

    segment.Kind.Should().Be( SegmentKind.Constant );
  }

  [Theory]
  [InlineData( 0, -1 )]
  [InlineData( 0, -100 )]
  [InlineData( 0, int.MinValue )]
  public void Segment_CreateConstant_ShouldThrow_WhenTextLengthIsNegative(
    int start,
    int length )
  {
    var act = () => Segment.CreateConstant( start, length );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "textLength" );
  }

  [Theory]
  [InlineData( -1, 0 )]
  [InlineData( -100, 0 )]
  [InlineData( int.MinValue, 0 )]
  public void Segment_CreateConstant_ShouldThrow_WhenTextStartIsNegative(
    int start,
    int length )
  {
    var act = () => Segment.CreateConstant( start, length );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "textStart" );
  }

  [Theory]
  [InlineData( ushort.MaxValue, int.MaxValue, ushort.MaxValue, int.MaxValue, ushort.MaxValue )]
  [InlineData( 10, 50, 15, 100, 25 )]
  public void Segment_CreateStandardMacro_ShouldHandleVariousValues(
    ushort slot,
    int nameStart,
    ushort nameLength,
    int argStart,
    ushort argLength )
  {
    var segment = Segment.CreateMacro( -slot, nameStart, nameLength, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );
    segment.Macro.Slot.Should().Be( slot );
    segment.Macro.NameStart.Should().Be( nameStart );
    segment.Macro.NameLength.Should().Be( nameLength );
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( argLength );
  }

  [Fact]
  public void Segment_CreateStandardMacro_ShouldInitializeMacroSegment()
  {
    var segment = Segment.CreateStandardMacro( 1, 10, 5, 20, 8 );

    segment.Macro.Slot.Should().Be( 1 );
    segment.Macro.NameStart.Should().Be( 10 );
    segment.Macro.NameLength.Should().Be( 5 );
    segment.Macro.ArgumentStart.Should().Be( 20 );
    segment.Macro.ArgumentLength.Should().Be( 8 );
  }

  [Fact]
  public void Segment_CreateStandardMacro_ShouldSetKindToStandardMacro()
  {
    var segment = Segment.CreateStandardMacro( 1, 10, 5, 20, 8 );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );
  }

  [Theory]
  [InlineData( -2 )]
  [InlineData( -100 )]
  [InlineData( int.MinValue )]
  public void Segment_CreateStandardMacro_ShouldThrow_WhenArgumentStartIsLessThanMinus1(
    int argumentStart )
  {
    var act = () => Segment.CreateStandardMacro( 1, 10, 5, argumentStart, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "argumentStart" );
  }

  [Fact]
  public void Segment_CreateStandardMacro_ShouldThrow_WhenNameLengthIsZero()
  {
    var act = () => Segment.CreateStandardMacro( 1, 10, 0, 0, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "nameLength" );
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( -100 )]
  [InlineData( int.MinValue )]
  public void Segment_CreateStandardMacro_ShouldThrow_WhenNameStartIsNegative(
    int nameStart )
  {
    var act = () => Segment.CreateStandardMacro( 1, nameStart, 5, 0, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "nameStart" );
  }

  [Theory]
  [InlineData( ushort.MaxValue, int.MaxValue, ushort.MaxValue, int.MaxValue, ushort.MaxValue )]
  [InlineData( 5, 100, 10, 200, 20 )]
  public void Segment_CreateUserMacro_ShouldHandleVariousValues(
    ushort slot,
    int nameStart,
    ushort nameLength,
    int argStart,
    ushort argLength )
  {
    var segment = Segment.CreateUserMacro( slot, nameStart, nameLength, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.UserMacro );
    segment.Macro.Slot.Should().Be( slot );
    segment.Macro.NameStart.Should().Be( nameStart );
    segment.Macro.NameLength.Should().Be( nameLength );
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( argLength );
  }

  [Fact]
  public void Segment_CreateUserMacro_ShouldInitializeMacroSegment()
  {
    var segment = Segment.CreateUserMacro( 1, 10, 5, 20, 8 );

    segment.Macro.Slot.Should().Be( 1 );
    segment.Macro.NameStart.Should().Be( 10 );
    segment.Macro.NameLength.Should().Be( 5 );
    segment.Macro.ArgumentStart.Should().Be( 20 );
    segment.Macro.ArgumentLength.Should().Be( 8 );
  }

  [Fact]
  public void Segment_CreateUserMacro_ShouldSetKindToUserMacro()
  {
    var segment = Segment.CreateUserMacro( 1, 10, 5, 20, 8 );

    segment.Kind.Should().Be( SegmentKind.UserMacro );
  }

  [Theory]
  [InlineData( -2 )]
  [InlineData( -100 )]
  [InlineData( int.MinValue )]
  public void Segment_CreateUserMacro_ShouldThrow_WhenArgumentStartIsLessThanMinus1(
    int argumentStart )
  {
    var act = () => Segment.CreateUserMacro( 1, 10, 5, argumentStart, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "argumentStart" );
  }

  [Fact]
  public void Segment_CreateUserMacro_ShouldThrow_WhenNameLengthIsZero()
  {
    var act = () => Segment.CreateUserMacro( 1, 10, 0, 0, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "nameLength" );
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( -100 )]
  [InlineData( int.MinValue )]
  public void Segment_CreateUserMacro_ShouldThrow_WhenNameStartIsNegative(
    int nameStart )
  {
    var act = () => Segment.CreateUserMacro( 1, nameStart, 5, 0, 0 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .WithParameterName( "nameStart" );
  }

  [Fact]
  public void Segment_GetDebuggerString_ShouldReturnConstantString_WhenKindIsConstant()
  {
    var segment = Segment.CreateConstant( 10, 20 );

    var result = segment.GetDebuggerString();

    result.Should().Be( "Constant { TextStart: 10, TextLength: 20 }" );
  }

  [Fact]
  public void
    Segment_GetDebuggerString_ShouldReturnStandardMacroString_WhenKindIsStandardMacro()
  {
    var segment = Segment.CreateStandardMacro( 1, 10, 5, 20, 8 );

    var result = segment.GetDebuggerString();

    result.Should()
          .Be(
            "StandardMacro { Slot: 1, NameStart: 10, NameLength: 5, ArgumentStart: 20, ArgumentLength: 8 }"
          );
  }

  [Fact]
  public void Segment_GetDebuggerString_ShouldReturnUserMacroString_WhenKindIsUserMacro()
  {
    var segment = Segment.CreateUserMacro( 1, 10, 5, 20, 8 );

    var result = segment.GetDebuggerString();

    result.Should()
          .Be(
            "UserMacro { Slot: 1, NameStart: 10, NameLength: 5, ArgumentStart: 20, ArgumentLength: 8 }"
          );
  }

  [Fact]
  public void Segment_KindProperty_ShouldCorrectlyIndicateSegmentType_Constant()
  {
    var segment = Segment.CreateConstant( 0, 1 );

    segment.Kind.Should().Be( SegmentKind.Constant );
  }

  [Fact]
  public void Segment_KindProperty_ShouldCorrectlyIndicateSegmentType_StandardMacro()
  {
    var segment = Segment.CreateStandardMacro( 1, 0, 1, 0, 0 );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );
  }

  [Fact]
  public void Segment_KindProperty_ShouldCorrectlyIndicateSegmentType_UserMacro()
  {
    var segment = Segment.CreateUserMacro( 1, 0, 1, 0, 0 );

    segment.Kind.Should().Be( SegmentKind.UserMacro );
  }

  [Fact]
  public void Segment_MacroProperty_ShouldBeAccessible_WhenKindIsStandardMacro()
  {
    var segment = Segment.CreateStandardMacro( 7, 60, 12, 80, 18 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 7 );
    macro.NameStart.Should().Be( 60 );
    macro.NameLength.Should().Be( 12 );
    macro.ArgumentStart.Should().Be( 80 );
    macro.ArgumentLength.Should().Be( 18 );
  }

  [Fact]
  public void Segment_MacroProperty_ShouldBeAccessible_WhenKindIsUserMacro()
  {
    var segment = Segment.CreateUserMacro( 3, 50, 10, 70, 15 );

    var macro = segment.Macro;

    macro.Slot.Should().Be( 3 );
    macro.NameStart.Should().Be( 50 );
    macro.NameLength.Should().Be( 10 );
    macro.ArgumentStart.Should().Be( 70 );
    macro.ArgumentLength.Should().Be( 15 );
  }

  [Fact]
  public void Segment_StandardMacroSegment_IntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$now:yyyy-MM-dd$" );
    var segment = Segment.CreateStandardMacro( 0, 1, 3, 5, 10 );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );

    var name = segment.Macro.GetName( template );
    name.Should().Be( "now" );

    var nameSpan = segment.Macro.GetNameSpan( template );
    nameSpan.ToString().Should().Be( "now" );

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
  public void Segment_UnionBehavior_UserMacroAndStandardMacroShareSameMemory()
  {
    var userSegment = Segment.CreateUserMacro( 5, 100, 10, 200, 20 );
    var standardSegment = Segment.CreateStandardMacro( 5, 100, 10, 200, 20 );

    userSegment.Macro.Slot.Should().Be( standardSegment.Macro.Slot );
    userSegment.Macro.NameStart.Should().Be( standardSegment.Macro.NameStart );
    userSegment.Macro.NameLength.Should().Be( standardSegment.Macro.NameLength );
    userSegment.Macro.ArgumentStart.Should().Be( standardSegment.Macro.ArgumentStart );
    userSegment.Macro.ArgumentLength.Should().Be( standardSegment.Macro.ArgumentLength );
  }

  [Fact]
  public void Segment_UserMacroSegment_IntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$userName:John$" );
    var segment = Segment.CreateUserMacro( 1, 1, 8, 10, 4 );

    segment.Kind.Should().Be( SegmentKind.UserMacro );

    var name = segment.Macro.GetName( template );
    name.Should().Be( "userName" );

    var nameSpan = segment.Macro.GetNameSpan( template );
    nameSpan.ToString().Should().Be( "userName" );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "John" );
  }

  [Theory]
  [InlineData( 1, 10, 5, 20, 8 )]
  [InlineData( 5, 100, 10, 200, 20 )]
  public void Segment_CreateMacro_ShouldCreateUserMacro_WhenSlotIsNonNegative(
    int slot,
    int nameStart,
    int nameLength,
    int argStart,
    int argLength )
  {
    var segment = Segment.CreateMacro( slot, nameStart, nameLength, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.UserMacro );
    segment.Macro.Slot.Should().Be( ( ushort ) slot );
    segment.Macro.NameStart.Should().Be( nameStart );
    segment.Macro.NameLength.Should().Be( ( ushort ) nameLength );
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( ( ushort ) argLength );
  }

  [Theory]
  [InlineData( -1, 10, 5, 20, 8 )]
  [InlineData( -5, 100, 10, 200, 20 )]
  public void Segment_CreateMacro_ShouldCreateStandardMacro_WhenSlotIsNegative(
    int slot,
    int nameStart,
    int nameLength,
    int argStart,
    int argLength )
  {
    var segment = Segment.CreateMacro( slot, nameStart, nameLength, argStart, argLength );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );
    segment.Macro.Slot.Should().Be( ( ushort ) ( Math.Abs( slot ) ) );
    segment.Macro.NameStart.Should().Be( nameStart );
    segment.Macro.NameLength.Should().Be( ( ushort ) nameLength );
    segment.Macro.ArgumentStart.Should().Be( argStart );
    segment.Macro.ArgumentLength.Should().Be( ( ushort ) argLength );
  }

  [Fact]
  public void Segment_CreateMacro_UserMacroIntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$userName:John$" );

    // Using CreateMacro with slot=2 creates a user macro (slot 1 internally after adjustment)
    var segment = Segment.CreateMacro( 2, 1, 8, 10, 4 );

    segment.Kind.Should().Be( SegmentKind.UserMacro );

    var name = segment.Macro.GetName( template );
    name.Should().Be( "userName" );

    var nameSpan = segment.Macro.GetNameSpan( template );
    nameSpan.ToString().Should().Be( "userName" );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "John" );
  }

  [Fact]
  public void Segment_CreateMacro_StandardMacroIntegrationTest_WithRealTemplate()
  {
    var template = CreateTemplate( "$now:yyyy-MM-dd$" );

    // Using CreateMacro with slot=-1 creates a standard macro (slot 0 internally after adjustment)
    var segment = Segment.CreateMacro( -1, 1, 3, 5, 10 );

    segment.Kind.Should().Be( SegmentKind.StandardMacro );

    var name = segment.Macro.GetName( template );
    name.Should().Be( "now" );

    var nameSpan = segment.Macro.GetNameSpan( template );
    nameSpan.ToString().Should().Be( "now" );

    var argSpan = segment.Macro.GetArgumentSpan( template );
    argSpan.ToString().Should().Be( "yyyy-MM-dd" );
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
