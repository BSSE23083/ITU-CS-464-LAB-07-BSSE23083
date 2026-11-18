using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Helper functions for creating UI objects from code. Positions are for a 1920x1080 canvas.
public static class UIKit
{
    public static UITheme T;

    public static RectTransform New(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform)) { layer = 5 };
        var r = (RectTransform)go.transform;
        r.SetParent(parent, false);
        return r;
    }

    // Anchor + pivot at the same normalised point, then offset.
    public static RectTransform At(this RectTransform r, float ax, float ay, float x, float y, float w, float h)
    {
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(ax, ay);
        r.anchoredPosition = new Vector2(x, y);
        r.sizeDelta = new Vector2(w, h);
        return r;
    }

    public static RectTransform Stretch(this RectTransform r, float left = 0, float top = 0, float right = 0, float bottom = 0)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.pivot = new Vector2(.5f, .5f);
        r.offsetMin = new Vector2(left, bottom);
        r.offsetMax = new Vector2(-right, -top);
        return r;
    }

    // Full width band anchored to the top (ay=1) or bottom (ay=0).
    public static RectTransform Band(this RectTransform r, float ay, float left, float right, float y, float h)
    {
        r.anchorMin = new Vector2(0, ay);
        r.anchorMax = new Vector2(1, ay);
        r.pivot = new Vector2(.5f, ay);
        r.sizeDelta = new Vector2(-(left + right), h);
        r.anchoredPosition = new Vector2((left - right) / 2f, y);
        return r;
    }

    public static RectTransform RT(this Component c) => (RectTransform)c.transform;

    public static Image Img(Transform parent, string name, Color color, Sprite sprite = null, bool raycast = false)
    {
        var img = New(name, parent).gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = raycast;
        if (sprite != null && sprite.border != Vector4.zero) img.type = Image.Type.Sliced;
        return img;
    }

    public static Image Box(Transform parent, string name, Color color, float radius = 1f)
    {
        var img = Img(parent, name, color, T.round);
        img.pixelsPerUnitMultiplier = radius;
        return img;
    }

    // Border = Unity's built-in Outline effect on the panel itself.
    public static UnityEngine.UI.Outline Outline(Transform target, string name, Color color, bool thick = false)
    {
        var o = target.gameObject.AddComponent<UnityEngine.UI.Outline>();
        o.effectColor = color;
        o.effectDistance = thick ? new Vector2(4, -4) : new Vector2(2, -2);
        o.useGraphicAlpha = false;
        return o;
    }

    public static TextMeshProUGUI Text(Transform parent, string name, string text, float size, Color color,
        TMP_FontAsset font = null, TextAlignmentOptions align = TextAlignmentOptions.Left, float spacing = 0)
    {
        var t = New(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
        t.font = font != null ? font : T.body;
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.characterSpacing = spacing;
        t.raycastTarget = false;
        t.textWrappingMode = TextWrappingModes.Normal;
        t.overflowMode = TextOverflowModes.Overflow;
        return t;
    }

    public static TextMeshProUGUI Heading(Transform parent, string name, string text, float size, Color color,
        TextAlignmentOptions align = TextAlignmentOptions.Left, float spacing = 2)
        => Text(parent, name, text, size, color, T.heading, align, spacing);

    // Standard button: rounded background + border + label (+ optional icon on the left).
    public static Button Button(Transform parent, string name, string text, ButtonStyle style, string iconName = null, float fontSize = 30)
    {
        var bg = Box(parent, name, Palette.Raised);
        bg.raycastTarget = true;
        var btn = bg.gameObject.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.targetGraphic = bg;
        var mb = bg.gameObject.AddComponent<MenuButton>();
        mb.style = style;
        mb.bg = bg;
        mb.border = Outline(bg.transform, "Border", Palette.Line);
        mb.label = Heading(bg.transform, "Label", text, fontSize, Palette.Text, TextAlignmentOptions.Center, 3);
        mb.label.RT().Stretch(iconName != null ? 44 : 12, 0, 12, 0);
        if (iconName != null)
        {
            mb.icon = Img(bg.transform, "Icon", Palette.Text, T.Icon(iconName));
            mb.icon.RT().At(0, .5f, 26, 0, 30, 30);
        }
        mb.Apply(true);
        return btn;
    }

    // A card that behaves like a button (mode cards, tiles, shop items).
    public static Button Card(Transform parent, string name)
    {
        var bg = Box(parent, name, Palette.Panel);
        bg.raycastTarget = true;
        var btn = bg.gameObject.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.targetGraphic = bg;
        var mb = bg.gameObject.AddComponent<MenuButton>();
        mb.style = ButtonStyle.Card;
        mb.hotScale = 1.03f;
        mb.bg = bg;
        mb.border = Outline(bg.transform, "Border", Palette.Line);
        mb.Apply(true);
        return btn;
    }

    public static CanvasGroup Group(GameObject go)
    {
        var g = go.GetComponent<CanvasGroup>();
        return g != null ? g : go.AddComponent<CanvasGroup>();
    }

    // dark background for tags so the text is readable in greyscale too
    public static Color ChipFill(Color c) => Color.Lerp(Palette.Bg, c, 0.12f);

    // Small rounded chip with text, e.g. "CLASS B" or "LOCKED".
    public static TextMeshProUGUI Chip(Transform parent, string name, string text, Color color, float w, float h = 34, float size = 20)
    {
        var bg = Box(parent, name, ChipFill(color), 2f);
        bg.RT().sizeDelta = new Vector2(w, h);
        Outline(bg.transform, "Border", color.A(.8f));
        var t = Text(bg.transform, "Text", text, size, color, T.bodyBold, TextAlignmentOptions.Center, 4);
        t.RT().Stretch();
        return t;
    }
}
