// Module Name: MacroExtensionsTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2026, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

public class MacroExtensionsTest
{
  #region Tests

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsValidLowercase()
  {
    var result = "validname".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsValidUppercase()
  {
    var result = "VALIDNAME".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsValidMixedCase()
  {
    var result = "ValidName".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameContainsDigits()
  {
    var result = "macro123".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameStartsWithDigit()
  {
    var result = "123macro".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameContainsUnderscore()
  {
    var result = "macro_name".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameContainsDash()
  {
    var result = "macro-name".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameContainsBothUnderscoreAndDash()
  {
    var result = "macro_name-test".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsSingleCharacter()
  {
    var result = "a".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsSingleDigit()
  {
    var result = "5".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsSingleUnderscore()
  {
    var result = "_".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsSingleDash()
  {
    var result = "-".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameContainsUnicodeLetters()
  {
    var result = "macroÄÖÜ".IsValidMacroName();

    result.Should().BeTrue();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameIsNull()
  {
    string? macroName = null;

    var result = macroName!.IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameIsEmpty()
  {
    var result = string.Empty.IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsSpace()
  {
    var result = "macro name".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsOnlySpace()
  {
    var result = " ".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsDollarSign()
  {
    var result = "$macro".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsPeriod()
  {
    var result = "macro.name".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsExclamation()
  {
    var result = "macro!".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsAtSign()
  {
    var result = "macro@name".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsColon()
  {
    var result = "macro:name".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsTab()
  {
    var result = "macro\tname".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Fact]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameContainsNewline()
  {
    var result = "macro\nname".IsValidMacroName();

    result.Should().BeFalse();
  }

  [Theory]
  [MemberData( nameof( ValidMacroNamesData ) )]
  public void IsValidMacroName_WithString_ShouldReturnTrue_WhenMacroNameIsValid(
    string macroName )
  {
    var result = macroName.IsValidMacroName();

    result.Should().BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidMacroNamesData ) )]
  public void IsValidMacroName_WithString_ShouldReturnFalse_WhenMacroNameIsInvalid(
    string? macroName )
  {
    var result = macroName!.IsValidMacroName();

    result.Should().BeFalse();
  }

  public static TheoryData<string> ValidMacroNamesData => new ()
  {
    "a",
    "A",
    "z",
    "Z",
    "0",
    "9",
    "_",
    "-",
    "name",
    "NAME",
    "Name",
    "name123",
    "123name",
    "name_test",
    "name-test",
    "name_test-123",
    "_name",
    "-name",
    "name_",
    "name-",
    "very_long_macro_name_with_underscores",
    "very-long-macro-name-with-dashes",
    "MixedCase123_Test-Name"
  };

  public static TheoryData<string?> InvalidMacroNamesData => new ()
  {
    null,
    "",
    " ",
    "  ",
    "\t",
    "\n",
    "\r",
    "name with space",
    "name\twith\ttab",
    "name\nwith\nnewline",
    "$name",
    "name$",
    "na$me",
    "name.test",
    "name!test",
    "name@test",
    "name#test",
    "name%test",
    "name^test",
    "name&test",
    "name*test",
    "name(test",
    "name)test",
    "name+test",
    "name=test",
    "name{test",
    "name}test",
    "name[test",
    "name]test",
    "name:test",
    "name;test",
    "name\"test",
    "name'test",
    "name<test",
    "name>test",
    "name,test",
    "name?test",
    "name/test",
    "name\\test",
    "name|test"
  };

  #endregion
}
