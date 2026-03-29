// Module Name: SegmentTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using ObjectLayoutInspector;
using Xunit.Abstractions;

[Trait( "Category", "Segments" )]
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
  public void Segment_ShouldBeExactly20Bytes()
  {
    var layout = TypeLayout.GetLayout<Segment>();
    var s = layout.ToString( true );
    _outputHelper.WriteLine( s );

    layout.FullSize.Should().Be( 20 );
  }

  [Fact]
  public void Segment_UnionBehavior_ConstantAndMacroShareSameMemory()
  {
    var segment = Segment.CreateConstant( 12345, 67890 );

    segment.Kind.Should().Be( SegmentKind.Constant );
    segment.Constant.TextStart.Should().Be( 12345 );
    segment.Constant.TextLength.Should().Be( 67890 );
  }

  #endregion
}

internal static class SegmentTestTemplateFactory
{
  internal static Template CreateTemplate(
    string text )
  {
    var macroTable = new MacroTableBuilder().Declare( "test" ).Build();
    var segments = new[] { Segment.CreateConstant( 0, text.Length ) };

    return new Template( text, macroTable, segments );
  }
}
