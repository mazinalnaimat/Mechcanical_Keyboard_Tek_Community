using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tek_Mechanical_Keyboard.Business.Keymap;

/// <summary>
/// Working Business-layer implementation used by the current UI.
/// It intentionally keeps data in memory until the real QMK/HID/DataAccess implementation
/// is added. All UI calls already pass through this Business service.
/// </summary>
public sealed class KeyMapBusinessService : IKeyMapBusinessService
{
    private readonly Dictionary<string, KeyboardProfileDto> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly IReadOnlyList<KeyActionDto> _actions;

    public KeyMapBusinessService()
    {
        _actions = CreateActions();
        KeyboardProfileDto defaultProfile = CreateDefaultProfile("Default");
        _profiles[defaultProfile.Name] = Clone(defaultProfile);
    }

    public Task<KeyboardProfileDto> GetKeyboardAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Clone(_profiles["Default"]));
    }

    public Task<IReadOnlyList<KeyActionDto>> GetAvailableActionsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_actions);
    }

    public Task<IReadOnlyList<string>> GetProfileNamesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<string> names = _profiles.Keys.OrderBy(x => x).ToArray();
        return Task.FromResult(names);
    }

    public Task<KeyboardProfileDto> LoadProfileAsync(string profileName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_profiles.TryGetValue(profileName, out KeyboardProfileDto? profile))
        {
            throw new InvalidOperationException($"Keyboard profile '{profileName}' was not found.");
        }

        return Task.FromResult(Clone(profile));
    }

    public Task SaveProfileAsync(KeyboardProfileDto profile, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(profile);
        _profiles[profile.Name] = Clone(profile);
        return Task.CompletedTask;
    }

    public Task SetKeyActionAsync(
        string profileName,
        int layer,
        int keyIndex,
        KeyActionDto action,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        KeyboardProfileDto profile = GetOrCreateProfile(profileName);
        profile.ActiveLayer = layer;
        KeyDefinitionDto key = profile.Keys.First(k => k.KeyIndex == keyIndex);
        key.Action = Clone(action);
        return Task.CompletedTask;
    }

    public Task ResetKeyAsync(
        string profileName,
        int layer,
        int keyIndex,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        KeyboardProfileDto profile = GetOrCreateProfile(profileName);
        profile.ActiveLayer = layer;
        KeyDefinitionDto key = profile.Keys.First(k => k.KeyIndex == keyIndex);
        key.Action = null;
        return Task.CompletedTask;
    }

    private KeyboardProfileDto GetOrCreateProfile(string profileName)
    {
        if (_profiles.TryGetValue(profileName, out KeyboardProfileDto? profile))
        {
            return profile;
        }

        KeyboardProfileDto created = CreateDefaultProfile(profileName);
        _profiles[profileName] = created;
        return created;
    }
    private static IReadOnlyList<KeyActionDto> CreateActions()
    {
        var actions = new List<KeyActionDto>();

        AddBasicKeys(actions);
        AddMediaKeys(actions);
        AddMouseKeys(actions);
        AddLayerKeys(actions);
        AddLightingKeys(actions);
        AddMacroKeys(actions);
        AddSystemKeys(actions);
        AddAdvancedKeys(actions);

        return actions;
    }

    // ============================================================
    // BASIC
    // ============================================================

    private static void AddBasicKeys(List<KeyActionDto> actions)
    {
        Add(actions, "Basic",
            ("KC_NO", "No Action", "Ignore this key"),
            ("KC_TRNS", "Transparent", "Use key from lower layer")
        );

        // A-Z
        for (char c = 'A'; c <= 'Z'; c++)
        {
            actions.Add(
                Action(
                    $"KC_{c}",
                    c.ToString(),
                    "Basic",
                    $"Keyboard {c}"));
        }

        // 1-9
        for (int i = 1; i <= 9; i++)
        {
            actions.Add(
                Action(
                    $"KC_{i}",
                    i.ToString(),
                    "Basic",
                    $"Keyboard {i}"));
        }

        actions.Add(
            Action(
                "KC_0",
                "0",
                "Basic",
                "Keyboard 0"));

        Add(actions, "Basic",

            // Common
            ("KC_ENT", "Enter", "Enter / Return"),
            ("KC_ESC", "Escape", "Escape"),
            ("KC_BSPC", "Backspace", "Backspace"),
            ("KC_TAB", "Tab", "Tab"),
            ("KC_SPC", "Space", "Space"),

            // Symbols
            ("KC_MINS", "-", "Minus"),
            ("KC_EQL", "=", "Equal"),
            ("KC_LBRC", "[", "Left bracket"),
            ("KC_RBRC", "]", "Right bracket"),
            ("KC_BSLS", "\\", "Backslash"),
            ("KC_NUHS", "Non-US #", "Non-US hash"),
            ("KC_SCLN", ";", "Semicolon"),
            ("KC_QUOT", "'", "Quote"),
            ("KC_GRV", "`", "Grave"),
            ("KC_COMM", ",", "Comma"),
            ("KC_DOT", ".", "Period"),
            ("KC_SLSH", "/", "Slash"),

            // Locks
            ("KC_CAPS", "Caps Lock", "Caps Lock"),
            ("KC_SCRL", "Scroll Lock", "Scroll Lock"),
            ("KC_NUM", "Num Lock", "Num Lock"),

            // System/navigation
            ("KC_PSCR", "Print Screen", "Print Screen"),
            ("KC_PAUS", "Pause", "Pause"),
            ("KC_INS", "Insert", "Insert"),
            ("KC_HOME", "Home", "Home"),
            ("KC_PGUP", "Page Up", "Page Up"),
            ("KC_DEL", "Delete", "Delete"),
            ("KC_END", "End", "End"),
            ("KC_PGDN", "Page Down", "Page Down"),

            // Arrows
            ("KC_RGHT", "Right", "Right Arrow"),
            ("KC_LEFT", "Left", "Left Arrow"),
            ("KC_DOWN", "Down", "Down Arrow"),
            ("KC_UP", "Up", "Up Arrow")
        );

        // F1-F24
        for (int i = 1; i <= 24; i++)
        {
            actions.Add(
                Action(
                    $"KC_F{i}",
                    $"F{i}",
                    "Basic",
                    $"Function key F{i}"));
        }

        // Modifiers
        Add(actions, "Basic",

            ("KC_LCTL", "Left Ctrl", "Left Control"),
            ("KC_LSFT", "Left Shift", "Left Shift"),
            ("KC_LALT", "Left Alt", "Left Alt / Option"),
            ("KC_LGUI", "Left GUI", "Windows / Command"),

            ("KC_RCTL", "Right Ctrl", "Right Control"),
            ("KC_RSFT", "Right Shift", "Right Shift"),
            ("KC_RALT", "Right Alt", "Right Alt / AltGr"),
            ("KC_RGUI", "Right GUI", "Windows / Command"),

            ("KC_MEH", "Meh", "Ctrl + Shift + Alt"),
            ("KC_HYPR", "Hyper", "Ctrl + Shift + Alt + GUI")
        );

        // Keypad
        Add(actions, "Basic",

            ("KC_PSLS", "Numpad /", "Keypad Slash"),
            ("KC_PAST", "Numpad *", "Keypad Asterisk"),
            ("KC_PMNS", "Numpad -", "Keypad Minus"),
            ("KC_PPLS", "Numpad +", "Keypad Plus"),
            ("KC_PENT", "Numpad Enter", "Keypad Enter"),
            ("KC_PDOT", "Numpad .", "Keypad Decimal"),
            ("KC_PEQL", "Numpad =", "Keypad Equal"),
            ("KC_PCMM", "Numpad ,", "Keypad Comma")
        );

        for (int i = 0; i <= 9; i++)
        {
            actions.Add(
                Action(
                    $"KC_P{i}",
                    $"Numpad {i}",
                    "Basic",
                    $"Keypad {i}"));
        }

        // Other keyboard HID keys
        Add(actions, "Basic",

            ("KC_NUBS", "Non-US \\", "Non-US Backslash"),

            ("KC_APP", "Application", "Application / Context Menu"),

            ("KC_KB_POWER", "Keyboard Power", "Keyboard Power"),

            ("KC_EXEC", "Execute", "Execute"),
            ("KC_HELP", "Help", "Help"),
            ("KC_MENU", "Menu", "Menu"),
            ("KC_SLCT", "Select", "Select"),
            ("KC_STOP", "Stop", "Stop"),
            ("KC_AGIN", "Again", "Again"),
            ("KC_UNDO", "Undo", "Undo"),
            ("KC_CUT", "Cut", "Cut"),
            ("KC_COPY", "Copy", "Copy"),
            ("KC_PSTE", "Paste", "Paste"),
            ("KC_FIND", "Find", "Find"),

            ("KC_KB_MUTE", "Keyboard Mute", "Keyboard Mute"),
            ("KC_KB_VOLUME_UP", "Keyboard Volume +", "Keyboard Volume Up"),
            ("KC_KB_VOLUME_DOWN", "Keyboard Volume -", "Keyboard Volume Down"),

            ("KC_LCAP", "Locking Caps Lock", "Locking Caps Lock"),
            ("KC_LNUM", "Locking Num Lock", "Locking Num Lock"),
            ("KC_LSCR", "Locking Scroll Lock", "Locking Scroll Lock"),

            ("KC_ERAS", "Alternate Erase", "Alternate Erase"),

            ("KC_SYRQ", "SysReq", "System Request"),
            ("KC_CNCL", "Cancel", "Cancel"),
            ("KC_CLR", "Clear", "Clear"),
            ("KC_PRIR", "Prior", "Prior"),
            ("KC_RETN", "Return", "Return"),
            ("KC_SEPR", "Separator", "Separator"),
            ("KC_OUT", "Out", "Out"),
            ("KC_OPER", "Oper", "Oper"),
            ("KC_CLAG", "Clear Again", "Clear Again"),
            ("KC_CRSL", "CrSel", "CrSel / Props"),
            ("KC_EXSL", "ExSel", "ExSel")
        );

        // International 1-9
        for (int i = 1; i <= 9; i++)
        {
            actions.Add(
                Action(
                    $"KC_INT{i}",
                    $"International {i}",
                    "Basic",
                    $"International Key {i}"));
        }

        // Language 1-9
        for (int i = 1; i <= 9; i++)
        {
            actions.Add(
                Action(
                    $"KC_LNG{i}",
                    $"Language {i}",
                    "Basic",
                    $"Language Key {i}"));
        }

        AddShiftedSymbols(actions);
    }

    // ============================================================
    // SHIFTED SYMBOLS
    // ============================================================

    private static void AddShiftedSymbols(List<KeyActionDto> actions)
    {
        Add(actions, "Basic",

            ("KC_TILD", "~", "Tilde"),
            ("KC_EXLM", "!", "Exclamation Mark"),
            ("KC_AT", "@", "At"),
            ("KC_HASH", "#", "Hash"),
            ("KC_DLR", "$", "Dollar"),
            ("KC_PERC", "%", "Percent"),
            ("KC_CIRC", "^", "Circumflex"),
            ("KC_AMPR", "&", "Ampersand"),
            ("KC_ASTR", "*", "Asterisk"),

            ("KC_LPRN", "(", "Left Parenthesis"),
            ("KC_RPRN", ")", "Right Parenthesis"),

            ("KC_UNDS", "_", "Underscore"),
            ("KC_PLUS", "+", "Plus"),

            ("KC_LCBR", "{", "Left Curly Brace"),
            ("KC_RCBR", "}", "Right Curly Brace"),

            ("KC_PIPE", "|", "Pipe"),

            ("KC_COLN", ":", "Colon"),
            ("KC_DQUO", "\"", "Double Quote"),

            ("KC_LABK", "<", "Left Angle Bracket"),
            ("KC_RABK", ">", "Right Angle Bracket"),

            ("KC_QUES", "?", "Question Mark")
        );
    }

    // ============================================================
    // MEDIA
    // ============================================================

    private static void AddMediaKeys(List<KeyActionDto> actions)
    {
        Add(actions, "Media",

            // Power
            ("KC_PWR", "Power", "System Power"),
            ("KC_SLEP", "Sleep", "System Sleep"),
            ("KC_WAKE", "Wake", "System Wake"),

            // Volume
            ("KC_MUTE", "Mute", "Mute Audio"),
            ("KC_VOLU", "Volume +", "Increase Volume"),
            ("KC_VOLD", "Volume -", "Decrease Volume"),

            // Media
            ("KC_MNXT", "Next Track", "Next Media Track"),
            ("KC_MPRV", "Previous Track", "Previous Media Track"),
            ("KC_MSTP", "Media Stop", "Stop Media"),
            ("KC_MPLY", "Play / Pause", "Play or Pause Media"),
            ("KC_MSEL", "Media Player", "Launch Media Player"),

            ("KC_EJCT", "Eject", "Eject"),

            ("KC_MFFD", "Fast Forward", "Fast Forward"),
            ("KC_MRWD", "Rewind", "Rewind"),

            // Brightness
            ("KC_BRIU", "Brightness +", "Increase Display Brightness"),
            ("KC_BRID", "Brightness -", "Decrease Display Brightness"),

            // Programs
            ("KC_MAIL", "Mail", "Launch Mail"),
            ("KC_CALC", "Calculator", "Launch Calculator"),
            ("KC_MYCM", "My Computer", "Launch My Computer"),

            // Browser
            ("KC_WSCH", "Web Search", "Browser Search"),
            ("KC_WHOM", "Web Home", "Browser Home"),
            ("KC_WBAK", "Web Back", "Browser Back"),
            ("KC_WFWD", "Web Forward", "Browser Forward"),
            ("KC_WSTP", "Web Stop", "Stop Browser Loading"),
            ("KC_WREF", "Web Refresh", "Refresh Browser"),
            ("KC_WFAV", "Web Favorites", "Browser Favorites"),

            // OS
            ("KC_CPNL", "Control Panel", "Open Control Panel"),
            ("KC_ASST", "Assistant", "Launch Assistant"),

            // Apple
            ("KC_MCTL", "Mission Control", "macOS Mission Control"),
            ("KC_LPAD", "Launchpad", "macOS Launchpad")
        );
    }

    // ============================================================
    // MOUSE
    // ============================================================

    private static void AddMouseKeys(List<KeyActionDto> actions)
    {
        Add(actions, "Mouse",

            // Movement
            ("MS_UP", "Mouse Up", "Move Cursor Up"),
            ("MS_DOWN", "Mouse Down", "Move Cursor Down"),
            ("MS_LEFT", "Mouse Left", "Move Cursor Left"),
            ("MS_RGHT", "Mouse Right", "Move Cursor Right"),

            // Wheel
            ("MS_WHLU", "Wheel Up", "Mouse Wheel Up"),
            ("MS_WHLD", "Wheel Down", "Mouse Wheel Down"),
            ("MS_WHLL", "Wheel Left", "Mouse Wheel Left"),
            ("MS_WHLR", "Wheel Right", "Mouse Wheel Right"),

            // Acceleration
            ("MS_ACL0", "Mouse Accel 0", "Mouse Acceleration 0"),
            ("MS_ACL1", "Mouse Accel 1", "Mouse Acceleration 1"),
            ("MS_ACL2", "Mouse Accel 2", "Mouse Acceleration 2")
        );

        // Buttons 1-8
        for (int i = 1; i <= 8; i++)
        {
            actions.Add(
                Action(
                    $"MS_BTN{i}",
                    $"Mouse Button {i}",
                    "Mouse",
                    $"Mouse Button {i}"));
        }
    }

    // ============================================================
    // LAYERS
    // ============================================================

    private const int MaxLayers = 16;

    private static void AddLayerKeys(List<KeyActionDto> actions)
    {
        for (int layer = 0; layer < MaxLayers; layer++)
        {
            actions.Add(
                Action(
                    $"MO({layer})",
                    $"Momentary Layer {layer}",
                    "Layer",
                    $"Enable Layer {layer} While Held"));

            actions.Add(
                Action(
                    $"TG({layer})",
                    $"Toggle Layer {layer}",
                    "Layer",
                    $"Toggle Layer {layer}"));

            actions.Add(
                Action(
                    $"TO({layer})",
                    $"Switch to Layer {layer}",
                    "Layer",
                    $"Switch Directly to Layer {layer}"));

            actions.Add(
                Action(
                    $"TT({layer})",
                    $"Tap Toggle Layer {layer}",
                    "Layer",
                    $"Momentary or Tap-Toggle Layer {layer}"));

            actions.Add(
                Action(
                    $"DF({layer})",
                    $"Default Layer {layer}",
                    "Layer",
                    $"Set Default Layer to {layer}"));

            actions.Add(
                Action(
                    $"PDF({layer})",
                    $"Persistent Layer {layer}",
                    "Layer",
                    $"Persist Default Layer {layer}"));

            actions.Add(
                Action(
                    $"OSL({layer})",
                    $"One Shot Layer {layer}",
                    "Layer",
                    $"Activate Layer {layer} for One Keypress"));
        }

        Add(actions, "Layer",

            ("QK_LAYER_LOCK",
                "Layer Lock",
                "Lock or Unlock Highest Active Layer"),

            ("OS_TOGG",
                "One Shot Toggle",
                "Toggle One Shot Keys"),

            ("OS_ON",
                "One Shot On",
                "Enable One Shot Keys"),

            ("OS_OFF",
                "One Shot Off",
                "Disable One Shot Keys")
        );
    }

    // ============================================================
    // LIGHTING
    // ============================================================

    private static void AddLightingKeys(List<KeyActionDto> actions)
    {
        // --------------------------------------------------------
        // BACKLIGHT
        // --------------------------------------------------------

        Add(actions, "Lighting",

            ("BL_TOGG", "Backlight Toggle", "Toggle Keyboard Backlight"),
            ("BL_STEP", "Backlight Step", "Cycle Backlight Level"),

            ("BL_ON", "Backlight On", "Turn Backlight On"),
            ("BL_OFF", "Backlight Off", "Turn Backlight Off"),

            ("BL_UP", "Backlight +", "Increase Backlight Brightness"),
            ("BL_DOWN", "Backlight -", "Decrease Backlight Brightness"),

            ("BL_BRTG", "Breathing Toggle", "Toggle Backlight Breathing")
        );

        // --------------------------------------------------------
        // RGB UNDERGLOW
        // --------------------------------------------------------

        Add(actions, "Lighting",

            ("UG_TOGG", "RGB Toggle", "Toggle RGB Underglow"),

            ("UG_NEXT", "RGB Mode +", "Next RGB Underglow Mode"),
            ("UG_PREV", "RGB Mode -", "Previous RGB Underglow Mode"),

            ("UG_HUEU", "RGB Hue +", "Increase RGB Hue"),
            ("UG_HUED", "RGB Hue -", "Decrease RGB Hue"),

            ("UG_SATU", "RGB Saturation +", "Increase RGB Saturation"),
            ("UG_SATD", "RGB Saturation -", "Decrease RGB Saturation"),

            ("UG_VALU", "RGB Brightness +", "Increase RGB Brightness"),
            ("UG_VALD", "RGB Brightness -", "Decrease RGB Brightness"),

            ("UG_SPDU", "RGB Speed +", "Increase RGB Effect Speed"),
            ("UG_SPDD", "RGB Speed -", "Decrease RGB Effect Speed")
        );

        // --------------------------------------------------------
        // RGB MATRIX
        // --------------------------------------------------------

        Add(actions, "Lighting",

            ("RM_ON", "RGB Matrix On", "Turn RGB Matrix On"),
            ("RM_OFF", "RGB Matrix Off", "Turn RGB Matrix Off"),
            ("RM_TOGG", "RGB Matrix Toggle", "Toggle RGB Matrix"),

            ("RM_NEXT", "RGB Matrix Mode +", "Next RGB Matrix Animation"),
            ("RM_PREV", "RGB Matrix Mode -", "Previous RGB Matrix Animation"),

            ("RM_HUEU", "Matrix Hue +", "Increase RGB Matrix Hue"),
            ("RM_HUED", "Matrix Hue -", "Decrease RGB Matrix Hue"),

            ("RM_SATU", "Matrix Saturation +", "Increase Matrix Saturation"),
            ("RM_SATD", "Matrix Saturation -", "Decrease Matrix Saturation"),

            ("RM_VALU", "Matrix Brightness +", "Increase Matrix Brightness"),
            ("RM_VALD", "Matrix Brightness -", "Decrease Matrix Brightness"),

            ("RM_SPDU", "Matrix Speed +", "Increase Matrix Animation Speed"),
            ("RM_SPDD", "Matrix Speed -", "Decrease Matrix Animation Speed"),

            ("RM_FLGN", "Matrix Flag +", "Next RGB Matrix Flag"),
            ("RM_FLGP", "Matrix Flag -", "Previous RGB Matrix Flag")
        );

        // --------------------------------------------------------
        // LED MATRIX
        // --------------------------------------------------------

        Add(actions, "Lighting",

            ("LM_ON", "LED Matrix On", "Turn LED Matrix On"),
            ("LM_OFF", "LED Matrix Off", "Turn LED Matrix Off"),
            ("LM_TOGG", "LED Matrix Toggle", "Toggle LED Matrix"),

            ("LM_NEXT", "LED Matrix Mode +", "Next LED Matrix Animation"),
            ("LM_PREV", "LED Matrix Mode -", "Previous LED Matrix Animation"),

            ("LM_BRIU", "LED Brightness +", "Increase LED Matrix Brightness"),
            ("LM_BRID", "LED Brightness -", "Decrease LED Matrix Brightness"),

            ("LM_SPDU", "LED Speed +", "Increase LED Animation Speed"),
            ("LM_SPDD", "LED Speed -", "Decrease LED Animation Speed"),

            ("LM_FLGN", "LED Flag +", "Next LED Matrix Flag"),
            ("LM_FLGP", "LED Flag -", "Previous LED Matrix Flag")
        );
    }

    // ============================================================
    // MACRO
    // ============================================================

    private static void AddMacroKeys(List<KeyActionDto> actions)
    {
        // QMK Dynamic Macros

        Add(actions, "Macro",

            ("DM_REC1",
                "Record Macro 1",
                "Start Recording Dynamic Macro 1"),

            ("DM_REC2",
                "Record Macro 2",
                "Start Recording Dynamic Macro 2"),

            ("DM_PLY1",
                "Play Macro 1",
                "Play Dynamic Macro 1"),

            ("DM_PLY2",
                "Play Macro 2",
                "Play Dynamic Macro 2"),

            ("DM_RSTP",
                "Stop Macro Recording",
                "Stop Dynamic Macro Recording")
        );

        // Custom application/firmware macros.
        // These require your own firmware support.

        for (int i = 0; i < 32; i++)
        {
            actions.Add(
                Action(
                    $"MACRO_{i}",
                    $"Macro {i}",
                    "Macro",
                    $"Custom Macro Slot {i}"));
        }
    }

    // ============================================================
    // SYSTEM
    // ============================================================

    private static void AddSystemKeys(List<KeyActionDto> actions)
    {
        Add(actions, "System",

            // QMK
            ("QK_BOOT", "Bootloader", "Enter QMK Bootloader"),
            ("DB_TOGG", "Debug Toggle", "Toggle QMK Debug Mode"),
            ("EE_CLR", "Clear EEPROM", "Clear Persistent EEPROM Settings"),
            ("QK_MAKE", "QMK Make", "QMK Make Command"),
            ("QK_RBT", "Reboot", "Reboot Keyboard"),

            // Caps Word
            ("CW_TOGG", "Caps Word", "Toggle Caps Word"),

            // Key Lock
            ("QK_LOCK", "Key Lock", "Lock Next Pressed Key"),

            // Leader
            ("QK_LEAD", "Leader", "Start Leader Sequence"),

            // Repeat
            ("QK_REP", "Repeat", "Repeat Previous Key"),
            ("QK_AREP", "Alternate Repeat", "Alternate Repeat"),

            // Dynamic Tapping Term
            ("DT_PRNT", "Print Tapping Term", "Print Current Tapping Term"),
            ("DT_UP", "Tapping Term +", "Increase Tapping Term"),
            ("DT_DOWN", "Tapping Term -", "Decrease Tapping Term"),

            // Auto Shift
            ("AS_DOWN", "Auto Shift Time -", "Decrease Auto Shift Timeout"),
            ("AS_UP", "Auto Shift Time +", "Increase Auto Shift Timeout"),
            ("AS_RPT", "Auto Shift Report", "Report Auto Shift Timeout"),
            ("AS_ON", "Auto Shift On", "Enable Auto Shift"),
            ("AS_OFF", "Auto Shift Off", "Disable Auto Shift"),
            ("AS_TOGG", "Auto Shift Toggle", "Toggle Auto Shift"),

            // Autocorrect
            ("AC_ON", "Autocorrect On", "Enable Autocorrect"),
            ("AC_OFF", "Autocorrect Off", "Disable Autocorrect"),
            ("AC_TOGG", "Autocorrect Toggle", "Toggle Autocorrect"),

            // One Shot
            ("OS_TOGG", "One Shot Toggle", "Toggle One Shot Keys"),
            ("OS_ON", "One Shot On", "Enable One Shot Keys"),
            ("OS_OFF", "One Shot Off", "Disable One Shot Keys"),

            // Swap Hands
            ("SH_ON", "Swap Hands On", "Enable Swap Hands"),
            ("SH_OFF", "Swap Hands Off", "Disable Swap Hands"),

            ("SH_MON",
                "Swap Hands Momentary",
                "Momentarily Enable Swap Hands"),

            ("SH_MOFF",
                "Swap Hands Momentary Off",
                "Momentarily Disable Swap Hands"),

            ("SH_TOGG",
                "Swap Hands Toggle",
                "Toggle Swap Hands"),

            ("SH_TT",
                "Swap Hands Tap Toggle",
                "Tap Toggle Swap Hands"),

            ("SH_OS",
                "Swap Hands One Shot",
                "One Shot Swap Hands"),

            // Unicode mode
            ("UC_NEXT", "Unicode Mode +", "Next Unicode Input Mode"),
            ("UC_PREV", "Unicode Mode -", "Previous Unicode Input Mode"),

            ("UC_MAC", "Unicode macOS", "Use macOS Unicode Input"),
            ("UC_LINX", "Unicode Linux", "Use Linux Unicode Input"),
            ("UC_WIN", "Unicode Windows", "Use Windows Unicode Input"),
            ("UC_WINC", "Unicode WinCompose", "Use WinCompose Unicode Input"),
            ("UC_EMAC", "Unicode Emacs", "Use Emacs Unicode Input")
        );

        AddWirelessKeys(actions);
        AddAudioKeys(actions);
        AddMagicKeys(actions);
        AddProgrammableButtons(actions);
    }

    // ============================================================
    // WIRELESS
    // ============================================================

    private static void AddWirelessKeys(List<KeyActionDto> actions)
    {
        Add(actions, "System",

            // Output selection
            ("OU_AUTO", "Output Auto", "Automatically Select Output"),
            ("OU_NEXT", "Output Next", "Select Next Output"),
            ("OU_PREV", "Output Previous", "Select Previous Output"),
            ("OU_NONE", "Output None", "Disable Output"),
            ("OU_USB", "USB Output", "Use USB Output"),
            ("OU_2P4G", "2.4 GHz Output", "Use 2.4 GHz Output"),
            ("OU_BT", "Bluetooth Output", "Use Bluetooth Output"),

            // Bluetooth
            ("BT_NEXT", "Bluetooth Profile +", "Next Bluetooth Profile"),
            ("BT_PREV", "Bluetooth Profile -", "Previous Bluetooth Profile"),
            ("BT_UNPR", "Bluetooth Unpair", "Unpair Bluetooth Profile")
        );

        for (int i = 1; i <= 5; i++)
        {
            actions.Add(
                Action(
                    $"BT_PRF{i}",
                    $"Bluetooth Profile {i}",
                    "System",
                    $"Select Bluetooth Profile {i}"));
        }
    }

    // ============================================================
    // AUDIO
    // ============================================================

    private static void AddAudioKeys(List<KeyActionDto> actions)
    {
        Add(actions, "System",

            // Audio
            ("AU_ON", "Audio On", "Enable QMK Audio"),
            ("AU_OFF", "Audio Off", "Disable QMK Audio"),
            ("AU_TOGG", "Audio Toggle", "Toggle QMK Audio"),

            // Clicky
            ("CK_TOGG", "Clicky Toggle", "Toggle Clicky Audio"),
            ("CK_ON", "Clicky On", "Enable Clicky Audio"),
            ("CK_OFF", "Clicky Off", "Disable Clicky Audio"),

            ("CK_UP",
                "Click Frequency +",
                "Increase Click Frequency"),

            ("CK_DOWN",
                "Click Frequency -",
                "Decrease Click Frequency"),

            ("CK_RST",
                "Click Frequency Reset",
                "Reset Click Frequency"),

            // Music
            ("MU_ON", "Music On", "Enable Music Mode"),
            ("MU_OFF", "Music Off", "Disable Music Mode"),
            ("MU_TOGG", "Music Toggle", "Toggle Music Mode"),
            ("MU_NEXT", "Music Mode +", "Next Music Mode"),

            // Voice
            ("AU_NEXT", "Audio Voice +", "Next Audio Voice"),
            ("AU_PREV", "Audio Voice -", "Previous Audio Voice")
        );
    }

    // ============================================================
    // MAGIC
    // ============================================================

    private static void AddMagicKeys(List<KeyActionDto> actions)
    {
        Add(actions, "System",

            // Caps / Ctrl
            ("CL_SWAP",
                "Swap Ctrl / Caps",
                "Swap Caps Lock and Left Control"),

            ("CL_NORM",
                "Normal Ctrl / Caps",
                "Restore Caps Lock and Left Control"),

            ("CL_TOGG",
                "Toggle Ctrl / Caps",
                "Toggle Caps Lock and Left Control"),

            ("CL_CTRL",
                "Caps as Ctrl",
                "Use Caps Lock as Control"),

            ("CL_CAPS",
                "Caps Normal",
                "Restore Caps Lock"),

            // Escape / Caps
            ("EC_SWAP",
                "Swap Esc / Caps",
                "Swap Escape and Caps Lock"),

            ("EC_NORM",
                "Normal Esc / Caps",
                "Restore Escape and Caps Lock"),

            ("EC_TOGG",
                "Toggle Esc / Caps",
                "Toggle Escape and Caps Lock"),

            // Ctrl / GUI Left
            ("CG_LSWP",
                "Swap Left Ctrl / GUI",
                "Swap Left Control and GUI"),

            ("CG_LNRM",
                "Normal Left Ctrl / GUI",
                "Restore Left Control and GUI"),

            // Ctrl / GUI Right
            ("CG_RSWP",
                "Swap Right Ctrl / GUI",
                "Swap Right Control and GUI"),

            ("CG_RNRM",
                "Normal Right Ctrl / GUI",
                "Restore Right Control and GUI"),

            // Ctrl / GUI Both
            ("CG_SWAP",
                "Swap Ctrl / GUI",
                "Swap Control and GUI"),

            ("CG_NORM",
                "Normal Ctrl / GUI",
                "Restore Control and GUI"),

            ("CG_TOGG",
                "Toggle Ctrl / GUI",
                "Toggle Control and GUI Swap"),

            // Alt / GUI Left
            ("AG_LSWP",
                "Swap Left Alt / GUI",
                "Swap Left Alt and GUI"),

            ("AG_LNRM",
                "Normal Left Alt / GUI",
                "Restore Left Alt and GUI"),

            // Alt / GUI Right
            ("AG_RSWP",
                "Swap Right Alt / GUI",
                "Swap Right Alt and GUI"),

            ("AG_RNRM",
                "Normal Right Alt / GUI",
                "Restore Right Alt and GUI"),

            // Alt / GUI Both
            ("AG_SWAP",
                "Swap Alt / GUI",
                "Swap Alt and GUI"),

            ("AG_NORM",
                "Normal Alt / GUI",
                "Restore Alt and GUI"),

            ("AG_TOGG",
                "Toggle Alt / GUI",
                "Toggle Alt and GUI Swap"),

            // GUI
            ("GU_OFF", "GUI Off", "Disable GUI Keys"),
            ("GU_ON", "GUI On", "Enable GUI Keys"),
            ("GU_TOGG", "GUI Toggle", "Toggle GUI Keys"),

            // Grave / Escape
            ("GE_SWAP",
                "Swap Grave / Escape",
                "Swap Grave and Escape"),

            ("GE_NORM",
                "Normal Grave / Escape",
                "Restore Grave and Escape"),

            // Backslash / Backspace
            ("BS_SWAP",
                "Swap Backslash / Backspace",
                "Swap Backslash and Backspace"),

            ("BS_NORM",
                "Normal Backslash / Backspace",
                "Restore Backslash and Backspace"),

            ("BS_TOGG",
                "Toggle Backslash / Backspace",
                "Toggle Backslash and Backspace Swap"),

            // NKRO
            ("NK_ON", "NKRO On", "Enable N-Key Rollover"),
            ("NK_OFF", "NKRO Off", "Disable N-Key Rollover"),
            ("NK_TOGG", "NKRO Toggle", "Toggle N-Key Rollover"),

            // Split EE Hands
            ("EH_LEFT", "EE Hands Left", "Set Keyboard as Left Hand"),
            ("EH_RGHT", "EE Hands Right", "Set Keyboard as Right Hand")
        );
    }

    // ============================================================
    // PROGRAMMABLE BUTTONS
    // ============================================================

    private static void AddProgrammableButtons(
        List<KeyActionDto> actions)
    {
        for (int i = 1; i <= 32; i++)
        {
            actions.Add(
                Action(
                    $"PB_{i}",
                    $"Programmable Button {i}",
                    "System",
                    $"QMK Programmable Button {i}"));
        }
    }

    // ============================================================
    // ADVANCED
    // ============================================================

    private static void AddAdvancedKeys(List<KeyActionDto> actions)
    {
        Add(actions, "Advanced",

            ("SC_LCPO",
                "Left Ctrl / (",
                "Left Ctrl When Held, Left Parenthesis When Tapped"),

            ("SC_RCPC",
                "Right Ctrl / )",
                "Right Ctrl When Held, Right Parenthesis When Tapped"),

            ("SC_LSPO",
                "Left Shift / (",
                "Left Shift When Held, Left Parenthesis When Tapped"),

            ("SC_RSPC",
                "Right Shift / )",
                "Right Shift When Held, Right Parenthesis When Tapped"),

            ("SC_LAPO",
                "Left Alt / (",
                "Left Alt When Held, Left Parenthesis When Tapped"),

            ("SC_RAPC",
                "Right Alt / )",
                "Right Alt When Held, Right Parenthesis When Tapped"),

            ("SC_SENT",
                "Right Shift / Enter",
                "Right Shift When Held, Enter When Tapped")
        );
    }

    // ============================================================
    // HELPER
    // ============================================================

    private static void Add(
        List<KeyActionDto> actions,
        string category,
        params (string Code, string Name, string Description)[] items)
    {
        foreach (var item in items)
        {
            actions.Add(
                Action(
                    item.Code,
                    item.Name,
                    category,
                    item.Description));
        }
    }
    private static KeyActionDto Action(string code, string name, string category, string description)
        => new() { Code = code, DisplayName = name, Category = category, Description = description };

    private static KeyboardProfileDto CreateDefaultProfile(string name)
    {
        KeyboardProfileDto profile = new() { Name = name, LayerCount = 4, ActiveLayer = 0 };
        int index = 0;

        AddRow(profile, ref index, 0, new (string Legend, double Width, bool Modifier)[]
        {
            ("Esc",1,false),("1",1,false),("2",1,false),("3",1,false),("4",1,false),("5",1,false),("6",1,false),("7",1,false),("8",1,false),("9",1,false),("0",1,false),("-",1,false),("=",1,false),("Backspace",2,true)
        });
        AddRow(profile, ref index, 1, new (string,double,bool)[]
        {
            ("Tab",1.5,true),("Q",1,false),("W",1,false),("E",1,false),("R",1,false),("T",1,false),("Y",1,false),("U",1,false),("I",1,false),("O",1,false),("P",1,false),("[",1,false),("]",1,false),("\\",1.5,false)
        });
        AddRow(profile, ref index, 2, new (string,double,bool)[]
        {
            ("Caps",1.75,true),("A",1,false),("S",1,false),("D",1,false),("F",1,false),("G",1,false),("H",1,false),("J",1,false),("K",1,false),("L",1,false),(";",1,false),("'",1,false),("Enter",2.25,true)
        });
        AddRow(profile, ref index, 3, new (string,double,bool)[]
        {
            ("Shift",2.25,true),("Z",1,false),("X",1,false),("C",1,false),("V",1,false),("B",1,false),("N",1,false),("M",1,false),(",",1,false),(".",1,false),("/",1,false),("Shift",2.75,true)
        });
        AddRow(profile, ref index, 4, new (string,double,bool)[]
        {
            ("Ctrl",1.25,true),("Win",1.25,true),("Alt",1.25,true),("Space",6.25,false),("Alt",1.25,true),("Fn",1.25,true),("Menu",1.25,true),("Ctrl",1.25,true)
        });

        return profile;
    }

    private static void AddRow(
        KeyboardProfileDto profile,
        ref int index,
        double y,
        IReadOnlyList<(string Legend, double Width, bool Modifier)> keys)
    {
        double x = 0;
        foreach ((string legend, double width, bool modifier) in keys)
        {
            profile.Keys.Add(new KeyDefinitionDto
            {
                KeyIndex = index++,
                Legend = legend,
                X = x,
                Y = y,
                Width = width,
                Height = 1,
                IsModifier = modifier,
                Action = Action(ToDefaultCode(legend), legend, "Basic", $"Default {legend} key")
            });
            x += width;
        }
    }

    private static string ToDefaultCode(string legend)
    {
        if (legend.Length == 1 && char.IsLetterOrDigit(legend[0]))
        {
            return $"KC_{char.ToUpperInvariant(legend[0])}";
        }

        return legend switch
        {
            "Esc" => "KC_ESC",
            "Tab" => "KC_TAB",
            "Caps" => "KC_CAPS",
            "Enter" => "KC_ENT",
            "Backspace" => "KC_BSPC",
            "Shift" => "KC_LSFT",
            "Ctrl" => "KC_LCTL",
            "Alt" => "KC_LALT",
            "Win" => "KC_LGUI",
            "Space" => "KC_SPC",
            "Fn" => "MO(1)",
            _ => "KC_NO"
        };
    }

    private static KeyboardProfileDto Clone(KeyboardProfileDto source)
    {
        KeyboardProfileDto clone = new()
        {
            Name = source.Name,
            LayerCount = source.LayerCount,
            ActiveLayer = source.ActiveLayer
        };

        foreach (KeyDefinitionDto key in source.Keys)
        {
            clone.Keys.Add(new KeyDefinitionDto
            {
                KeyIndex = key.KeyIndex,
                Legend = key.Legend,
                X = key.X,
                Y = key.Y,
                Width = key.Width,
                Height = key.Height,
                IsModifier = key.IsModifier,
                Action = key.Action is null ? null : Clone(key.Action)
            });
        }

        return clone;
    }

    private static KeyActionDto Clone(KeyActionDto source)
        => new()
        {
            Code = source.Code,
            DisplayName = source.DisplayName,
            Category = source.Category,
            Description = source.Description
        };
}
