// Module Name: SegmentAssertions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests.FluentAssertions;

using global::FluentAssertions.Execution;
using global::FluentAssertions.Primitives;

/// <summary>
///   Provides fluent assertions for <see cref="Segment" />.
/// </summary>
internal sealed class SegmentAssertions: ObjectAssertions<Segment, SegmentAssertions>
{
  #region Constructors

  /// <inheritdoc />
  public SegmentAssertions(
    Template template,
    Segment segment )
    : base( segment )
  {
    Template = template;
  }

  #endregion

  #region Properties

  public Template Template { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Asserts that the segment is a constant segment, optionally matching its text and start.
  /// </summary>
  public AndConstraint<SegmentAssertions> BeConstant(
    string? text = null,
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .ForCondition( Subject.Kind == SegmentKind.Constant )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a constant segment{reason}, but it was a {0} (name: {1}).",
             Subject.Kind,
             Subject.Kind != SegmentKind.Constant ? Subject.Macro.GetName( Template ) : "N/A"
           );

    if( text is not null )
    {
      var actualText = Subject.Constant.GetText( Template );

      Execute.Assertion
             .ForCondition( actualText == text )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} constant text to be {0}{reason}, but found {1}.",
               text,
               actualText
             );
    }

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts that the segment is a macro segment, optionally matching its name, argument and slot.
  /// </summary>
  public AndConstraint<SegmentAssertions> BeMacro(
    string? name = null,
    string? argument = null,
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .ForCondition(
             Subject.Kind == SegmentKind.UserMacro || Subject.Kind == SegmentKind.StandardMacro
           )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} to be a macro segment{reason}, but it was a constant (text: {0}).",
             Subject.Kind == SegmentKind.Constant ? Subject.Constant.GetText( Template ) : "N/A"
           );

    if( name is not null )
    {
      var actualName = Subject.Macro.GetName( Template );

      Execute.Assertion
             .ForCondition( actualName == name )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} macro name to be {0}{reason}, but found {1}.",
               name,
               actualName
             );
    }

    if( argument is not null )
    {
      var actualArg = Subject.Macro.GetArgumentSpan( Template ).ToString();

      Execute.Assertion
             .ForCondition( actualArg == argument )
             .BecauseOf( because, becauseArgs )
             .FailWith(
               "Expected {context:segment} macro argument to be {0}{reason}, but found {1}.",
               argument,
               actualArg
             );
    }

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment text.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveText(
    string expected,
    string because = "",
    params object[] becauseArgs )
  {
    var actualText = Subject.Kind == SegmentKind.Constant
      ? Subject.Constant.GetText( Template )
      : Subject.Macro.GetName( Template );

    Execute.Assertion
           .ForCondition( actualText == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} text to be {0}{reason}, but found {1}.",
             expected,
             actualText
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment argument text.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveArgument(
    string expected,
    string because = "",
    params object[] becauseArgs )
  {
    var actual = Subject.Kind != SegmentKind.Constant
      ? Subject.Macro.GetArgumentSpan( Template ).ToString()
      : string.Empty;

    Execute.Assertion
           .ForCondition( actual == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} argument to be {0}{reason}, but found {1}.",
             expected,
             actual
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  /// <summary>
  ///   Asserts the segment slot value.
  /// </summary>
  public AndConstraint<SegmentAssertions> HaveSlot(
    int expected,
    string because = "",
    params object[] becauseArgs )
  {
    // Convert from Segment representation to original Segment slot representation
    var actualSlot = Subject.Kind switch
    {
      SegmentKind.Constant => -1,
      SegmentKind.StandardMacro => -Subject.Macro.Slot,
      SegmentKind.UserMacro => Subject.Macro.Slot,
      _ => throw new InvalidOperationException( $"Unknown segment kind: {Subject.Kind}" )
    };

    Execute.Assertion
           .ForCondition( actualSlot == expected )
           .BecauseOf( because, becauseArgs )
           .FailWith(
             "Expected {context:segment} slot to be {0}{reason}, but found {1}.",
             expected,
             actualSlot
           );

    return new AndConstraint<SegmentAssertions>( this );
  }

  #endregion
}
