// Module Name: TemplateTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

[Trait( "Category", "Compiler" )]
public class TemplateTest
{
  #region Tests

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsEmpty()
  {
    Action act = () => new Template( "", new MacroTableBuilder().Build(), [] );

    act.Should().Throw<ArgumentException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsNull()
  {
    Action act = () => new Template( "", new MacroTableBuilder().Build(), null! );

    act.Should().Throw<ArgumentNullException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Template_CreateValues_ShouldReturnMacroValues()
  {
    var template = CreateTemplate( "Hello $name$", "name" );

    var values = template.CreateValues();

    values.Should().NotBeNull();
  }

  [Fact]
  public void Template_CreateValues_ShouldReturnMacroValuesWithSameMacroTable()
  {
    var template = CreateTemplate( "Hello $name$", "name" );

    var values = template.CreateValues();

    values.MacroTable.Should().BeSameAs( template.MacroTable );
  }

  [Fact]
  public void Template_CreateValues_ShouldReturnNewInstanceEachTime()
  {
    var template = CreateTemplate( "Hello $name$", "name" );

    var values1 = template.CreateValues();
    var values2 = template.CreateValues();

    values1.Should().NotBeSameAs( values2 );
  }

  [Fact]
  public void Template_CreateValues_ShouldReturnMacroValuesWithCorrectCount()
  {
    var template = CreateTemplate( "Hello $name$ and $title$", "name", "title" );

    var values = template.CreateValues();

    values.MacroTable.Count.Should().Be( 2 );
  }

  [Fact]
  public void Template_CreateValues_ShouldAllowSettingAndGettingValues()
  {
    var template = CreateTemplate( "Hello $name$", "name" );
    var values = template.CreateValues();

    values.SetValue( "name", "World" );
    var result = values.GetValue( "name" );

    result.Should().Be( "World" );
  }

  [Fact]
  public void Template_CreateValues_ShouldWorkWithEmptyMacroTable()
  {
    var macroTable = new MacroTableBuilder().Build();
    var segments = new[] { Segment.CreateConstant( 0, 11 ) };
    var template = new Template( "Hello World", macroTable, segments );

    var values = template.CreateValues();

    values.Should().NotBeNull();
    values.MacroTable.Count.Should().Be( 0 );
  }

  [Fact]
  public void Template_CreateValues_ShouldWorkWithMultipleMacros()
  {
    var template = CreateTemplate( "$greeting$ $name$ from $location$", "greeting", "name", "location" );
    var values = template.CreateValues();

    values.SetValue( "greeting", "Hello" );
    values.SetValue( "name", "World" );
    values.SetValue( "location", "Earth" );

    values.GetValue( "greeting" ).Should().Be( "Hello" );
    values.GetValue( "name" ).Should().Be( "World" );
    values.GetValue( "location" ).Should().Be( "Earth" );
  }

  [Fact]
  public void Template_CreateValues_ShouldInitializeWithNullGenerators()
  {
    var template = CreateTemplate( "Hello $name$", "name" );
    var values = template.CreateValues();

    var result = values.GetValue( "name" );

    result.Should().BeNull();
  }

  [Fact]
  public void Template_CreateValues_ShouldSupportValueGenerators()
  {
    var template = CreateTemplate( "Hello $name$", "name" );
    var values = template.CreateValues();

    values.SetValue( "name", _ => "Generated Value" );
    var result = values.GetValue( "name" );

    result.Should().Be( "Generated Value" );
  }

  [Fact]
  public void Template_CreateValues_ShouldHandleCaseInsensitiveMacroNames()
  {
    var template = CreateTemplate( "Hello $Name$", "name" );
    var values = template.CreateValues();

    values.SetValue( "NAME", "World" );
    var result = values.GetValue( "name" );

    result.Should().Be( "World" );
  }

  #endregion

  #region Implementation

  private static Template CreateTemplate(
    string text,
    params string[] macroNames )
  {
    var builder = new MacroTableBuilder();

    foreach( var macroName in macroNames )
    {
      builder.Declare( macroName );
    }

    var macroTable = builder.Build();
    var segments = new[] { Segment.CreateConstant( 0, text.Length ) };

    return new Template( text, macroTable, segments );
  }

  #endregion
}
