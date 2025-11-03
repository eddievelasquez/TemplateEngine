# Contributing to Intercode.Toolbox.TemplateEngine

Thank you for your interest in contributing!

## How to Contribute

- Fork the repository and create your branch from `main`.
- Follow the coding style and guidelines in [.github/copilot-instructions.md](copilot-instructions.md).
- Run `dotnet format` before submitting.
- Ensure all tests pass and add new tests for new features or bug fixes.
- Submit a pull request with a clear description of your changes.

## Pull Request Checklist

- [ ] Build succeeds across all target frameworks.
- [ ] No new analyzer warnings.
- [ ] Code is formatted (`dotnet format`).
- [ ] Tests added/updated.
- [ ] Benchmarks provided if performance claims are made.
- [ ] Public API changes reviewed.

## Code Style

- Use modern .NET and C# features as appropriate.
- Prefer `internal` for non-public APIs.
- Use primary constructors and `readonly record struct` for small immutable types.
- See [.github/copilot-instructions.md](copilot-instructions.md) for full details.

## Reporting Issues

- Use the issue template for bug reports and feature requests.
- Provide as much detail as possible, including reproduction steps.

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
