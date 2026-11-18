using UnityEngine;

// All the colours used in the menus.
public static class Palette
{
    public static readonly Color Bg = Hex("0B0F17");
    public static readonly Color Panel = Hex("131A28");
    public static readonly Color Raised = Hex("1E2739");
    public static readonly Color Hover = Hex("2B3650");
    public static readonly Color Line = Hex("3E4C6A");
    public static readonly Color Disabled = Hex("161C28");

    public static readonly Color Text = Hex("F3F5FA");
    public static readonly Color TextDim = Hex("A6B0C4");
    public static readonly Color TextMute = Hex("5F6A82");
    public static readonly Color TextOnAccent = Hex("10131B");

    public static readonly Color Accent = Hex("FF6A1A");
    public static readonly Color AccentHot = Hex("FF8E4D");
    public static readonly Color Cyan = Hex("33D1FF");
    public static readonly Color Gold = Hex("FFC53D");
    public static readonly Color Green = Hex("3BE089");
    public static readonly Color Red = Hex("FF4757");

    public static readonly Color Dim = new Color(0.02f, 0.03f, 0.06f, 0.78f);

    public static Color Hex(string hex, float alpha = 1f)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        c.a = alpha;
        return c;
    }

    public static Color A(this Color c, float a) { c.a = a; return c; }
}
