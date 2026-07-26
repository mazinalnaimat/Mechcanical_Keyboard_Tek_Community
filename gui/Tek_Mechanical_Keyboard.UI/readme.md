# Keyboard UI Components

A WPF UI implementation for a keyboard settings interface using reusable components and separate pages.

## Implemented Components

### LeftSideButton

Created a reusable `UserControl` for navigation buttons.

Features:

- Custom icon support
- Button text display
- Page key identification
- Navigation click handling

Example:

```xml
<controls:LeftSideButton
    Text="Settings"
    Icon="&#xE713;"
    PageKey="Settings"
    Click="NavigationButton_OnClick" />
```
