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

## Uninstall

1. Use the following pages to open the Manage Extensions dialog box.
https://learn.microsoft.com/en-us/visualstudio/ide/finding-and-using-visual-studio-extensions?view=vs-2019
1. Select "Installed" node in left side pane.
1. Find "GitFeatureFlagAdjusterForRsharp".
1. Select it and click "Uninstall" button.
1. Restart Visual Studio.