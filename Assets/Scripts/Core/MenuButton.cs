using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonStyle { Primary, Secondary, Danger, Card }

// Hover, click and disabled effects for all buttons.
// Hovering with the mouse also selects the button so mouse and keyboard work the same.
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    public ButtonStyle style;
    public Image bg, icon;
    public Shadow border;
    public TMP_Text label;
    public float hotScale = 1.04f;

    Button button;
    bool focused, wasInteractable = true;

    Button Btn => button != null ? button : (button = GetComponent<Button>());

    void OnEnable()
    {
        focused = false;
        transform.localScale = Vector3.one;
        wasInteractable = Btn.interactable;
        Apply(true);
    }

    void Update()
    {
        if (Btn.interactable != wasInteractable)
        {
            wasInteractable = Btn.interactable;
            if (!wasInteractable) focused = false;
            Apply(false);
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (Btn.interactable && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData e) { focused = true; Apply(false); }
    public void OnDeselect(BaseEventData e) { focused = false; Apply(false); }

    public void OnPointerDown(PointerEventData e)
    {
        if (Btn.interactable) Tween.Scale(transform, 0.95f, 0.07f, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (Btn.interactable) Tween.Scale(transform, focused ? hotScale : 1f, 0.3f, Ease.OutBack);
    }

    public void Apply(bool instant)
    {
        bool on = Btn.interactable;
        bool hot = on && focused;
        Color bgC, borderC, textC;

        switch (style)
        {
            case ButtonStyle.Primary:
                bgC = !on ? Palette.Raised : hot ? Palette.AccentHot : Palette.Accent;
                borderC = !on ? Palette.Line : hot ? Palette.Text : Palette.Accent;
                textC = !on ? Palette.TextMute : Palette.TextOnAccent;
                break;
            case ButtonStyle.Danger:
                bgC = !on ? Palette.Disabled : hot ? Palette.Red : Palette.Raised;
                borderC = !on ? Palette.Line.A(.3f) : Palette.Red;
                textC = !on ? Palette.TextMute : hot ? Palette.TextOnAccent : Palette.Red;
                break;
            case ButtonStyle.Card:
                bgC = !on ? Palette.Disabled : hot ? Palette.Raised : Palette.Panel;
                borderC = !on ? Palette.Line.A(.25f) : hot ? Palette.Accent : Palette.Line.A(.6f);
                textC = !on ? Palette.TextMute : Palette.Text;
                break;
            default:
                bgC = !on ? Palette.Disabled : hot ? Palette.Hover : Palette.Raised;
                borderC = !on ? Palette.Line.A(.3f) : hot ? Palette.Accent : Palette.Line;
                textC = !on ? Palette.TextMute : Palette.Text;
                break;
        }

        float scale = hot ? hotScale : 1f;
        if (instant || !Application.isPlaying)
        {
            if (bg) bg.color = bgC;
            if (border) border.effectColor = borderC;
            if (label) label.color = textC;
            if (icon) icon.color = textC;
            transform.localScale = Vector3.one * scale;
            return;
        }
        const float t = 0.14f;
        if (bg) Tween.Tint(bg, bgC, t, Ease.OutQuad);
        if (border)
        {
            Color from = border.effectColor;
            Tween.Run(border, "tint", t, Ease.OutQuad, k => border.effectColor = Color.Lerp(from, borderC, k));
        }
        if (label) Tween.Tint(label, textC, t, Ease.OutQuad);
        if (icon) Tween.Tint(icon, textC, t, Ease.OutQuad);
        Tween.Scale(transform, scale, 0.18f, Ease.OutQuad);
    }
}
