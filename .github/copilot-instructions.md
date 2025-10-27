## General
This repository targets modern .NET (currently .NET 9.0) and C# 13.0. Use the most appropriate modern language constructs where they add clarity, safety, or performance.

## Target Framework and Language Version
- All contributions must match the target frameworks and language versions declared in each project file.
- `Intercode.Toolbox.TypedPrimitives` (source generator) MUST remain on .NET Standard 2.0 and C# 12.0 (for broad IDE / compiler support). Do not introduce APIs unavailable on netstandard2.0.
- Add new multi-targets only when a concrete feature or perf gain requires it; document rationale in the PR.

## Coding Style
- Follow `.editorconfig` rules; run `dotnet format` before committing.
- Use `internal` for types and members not intended for public API exposure.
- Prefer `sealed` classes unless a clear extensibility scenario exists (document in XML docs if unsealed).
- Prefer primary constructors (C# 13) for simple state-carrying types; prefer `record struct`/`readonly struct` for small immutable value types (≤16 bytes, no defensive copies required).
- Use `static` for pure utility holders; use extension methods instead of static helper classes when enriching existing BCL abstractions.
- Group related private constants and static fields at the top of the file.
- File names must match the primary public (or internal) type in PascalCase.
- Use file-scoped namespaces consistently.
- Avoid `#region` except for large interop blocks or generated code.
- Avoid the null-forgiving operator (`!`) unless accompanied by a short justification comment.

## Nullability & Contracts
- Nullable reference types must be enabled in every project.
- Use appropriate annotations (`NotNullWhen`, `MemberNotNull`, `DisallowNull`, etc.) to express contracts.
- Centralize guard clauses or throwing logic in focused helpers when they appear in hot paths.
- Prefer `ArgumentException` variants (ArgumentNullException, ArgumentOutOfRangeException) over generic `Exception`.

## Performance & Allocation Guidance
- Justify any new abstraction on hot paths with a brief perf note in the PR.
- Prefer spans and `foreach (ref readonly var item in collection)` where it reduces copying and is readable.
- Use `ValueTask` for frequently synchronous async APIs.
- Avoid `async` wrappers over sync unless IO-bound or necessary to prevent blocking.
- Consider `ArrayPool<T>` or pooling patterns for large transient buffers.
- Document non-trivial algorithmic complexity (e.g., O(n log n)) in XML docs of public APIs.
- Benchmark changes that alter complexity or critical paths (see Benchmarks project). Provide summary table in PR when perf is cited.

## Unit Tests
- Test projects named `{ProjectName}.UnitTests` and mirror the namespace of the code under test with `.UnitTests` suffix.
- If the test project does not exist, create it alongside the production project, respecting the naming convention stated above. Add the project to the solution as well.
- One test class per production class. Name: `{ClassName}Test`.
- Use xUnit 3: `[Fact]` for non-parameterized tests; `[Theory]` + `[MemberData]` for parameterized. Only use `[InlineData]` for trivial cases (e.g., single int input).
- Theory data properties: `public static TheoryData<...> {MethodName}{Scenario}Data { get; }` placed immediately above the theory using it.
- Naming:
  - Non-returning: `MethodName_ShouldExpectedResult_WhenConditionOccurs`.
  - Returning: `MethodName_ShouldReturnExpectedValue_WhenConditionOccurs`.
  - Overloads: include parameter type descriptors before `_Should`, e.g., `Parse_WithString_ShouldReturnExpectedValue_WhenInputValid`.
- All public methods must have at least one behavioral test; branch-heavy logic should use theories for coverage.
- Tests must cover all edge cases (null, empty, whitespace, min/max values, etc.) unless explicitly documented otherwise.
- Use `FluentAssertions` for assertions; no direct `Assert.*` unless unavailable in FluentAssertions.
- Source generator tests must assert generated output (snapshot or baseline comparison). Whitespace is significant.
- If an internal method needs to be tested, use `[assembly: InternalsVisibleTo("ProjectName.Tests")]`.
- Do not emit arranged/act/assert comments; structure tests clearly instead.
- Should not attempt to change the code under test to make the unit tests pass unless given permission to do so.
- When testing builder classes, the tests should verify the build object, not the builder's internal state. 
- Should not rely on implementation details of the class under test; only observable behavior is verified.

## Diagnostics & Logging
- Prefer structured (property-based) logging over string concatenation where logging exists.
- Provide actionable diagnostic messages; include the symbol name that triggered the issue.
- No reflection-based logging of private members in generators.

## Multi-Targeting & Conditional Code
- Keep feature parity across targets unless a capability is impossible on lower targets; document divergences.
- Use clear `#if` symbols derived from TFM (e.g., `#if NET9_0_OR_GREATER`). Avoid custom symbols unless absolutely necessary.
- For APIs unavailable on netstandard2.0, isolate with partial classes / partial methods and guard with `#if`.

## Documentation & Generated Artifacts
- Public APIs must have XML documentation. Summaries should include purpose and (when non-trivial) complexity and thread-safety notes.
- Internal complex algorithms need a leading comment with rationale and references (if derived from known papers / sources).
- Generated code: follow the generator header rules (see Source Generators section). Do not include volatile data except timestamp + hash.

## Review & Quality Gates
- PR checklist (implied):
  1. Build succeeds across all TFMs.
  2. Analyzers clean (no new warnings). `TreatWarningsAsErrors` should pass.
  3. Formatting applied (`dotnet format`).
  4. Tests added/updated; generator snapshots updated intentionally.
  5. Benchmarks provided if perf claims made.
  6. Public API changes reviewed (API surface diff if tooling available).
  7. Deterministic generator output confirmed (no churn on re-run).

## Security & Robustness
- Validate all external or file-based inputs (even design-time template inputs) before processing.
- Normalize and validate resource names; reject traversal attempts (`..`).
- Avoid unsafe code unless a measured perf gain is demonstrated (and document reasoning).

## Out of Scope / Prohibited
- No unvetted external dependencies without prior discussion.
- No reflection-heavy runtime generation inside shipping libraries (design-time source generation preferred).
- No dynamic code emission (ILGenerator / Reflection.Emit) in this repository.
- Avoid premature micro-optimizations that degrade readability without measurable benefit.

## Commit Hygiene
- Keep commits logically scoped. Avoid mixing refactors with feature changes.
- Commit messages: `<area>: <short summary>` (e.g., `Generator: Add deterministic ordering`).

## Rationale
These rules minimize ambiguity, ensure deterministic and performant source generation, improve review signal-to-noise, and maintain a stable public API surface.

---

### Reference Documentation Style Guide

When generating API or SDK reference documentation use the style and structure of 
Microsoft Learn technical documentation when generating README files and other developer-facing content.


#### Tone and Purpose
- Objective, technical, and precise
- Avoid narrative or conversational language
- Focus on clarity, completeness, and discoverability

#### Structure

Each API or SDK component should follow this format:

```markdown
## Overview
Briefly describe the purpose of the project, its key features, and intended audience.

## Getting Started
1. Clone the repository.
2. Install dependencies.
3. Run the application using `dotnet run`.

## Testing
Instructions for running unit tests, integration tests, or using xUnit.

## Reference

### ClassName / InterfaceName

#### Summary
A concise description of what this type represents or does.

#### Namespace
`Namespace.Name`

#### Assembly
`Assembly.Name.dll`

#### Syntax
```csharp
public class ClassName : BaseType
```

#### Constructors
- `ClassName()` – Initializes a new instance.
- `ClassName(string name)` – Initializes with a name.

#### Properties
| Property | Type | Description |
|----------|------|-------------|
| `Name`   | `string` | Gets or sets the name. |
| `IsEnabled` | `bool` | Indicates whether the feature is enabled. |

#### Methods
| Method | Return Type | Description |
|--------|-------------|-------------|
| `Initialize()` | `void` | Prepares the component for use. |
| `ToString()` | `string` | Returns a string representation. |

#### Remarks
Include usage notes, edge cases, and performance considerations.

#### See Also
- [RelatedClass](https://learn.microsoft.com/en-us/dotnet/api/system.relatedclass)
- [Microsoft Learn: SDK Reference Guide](https://learn.microsoft.com/en-us/visualstudio/extensibility/visual-studio-sdk-reference?view=vs-2022)
