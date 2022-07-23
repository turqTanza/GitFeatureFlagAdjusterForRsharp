# GitFeatureFlagAdjusterForRsharp

A Visual Studio extension for avoiding error "Refactoring Failed. Files still read-only" in Refactoring by ReSharper.

## Getting Started
Download and install the extension from [Releases](https://github.com/turqTanza/GitFeatureFlagAdjusterForRsharp/releases).

## Mechanism
This extension overwrites feature flag called "Git.Operation.EphemeralEditsUseDirtyAttributeOnly" to false.

Timing: Solution load fully completed or periodcally 10s after loaded.

## Precondition

This extension perhaps effective in follow conditions:

* Using Visual Studio 2019.
* Using default Git source code control plugin in Visual Studio.
* Using ReSharper.

