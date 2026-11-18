using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One row on the settings screen: either a < choice >, an ON/OFF switch or a 0-100 slider.
public class SettingControl : MonoBehaviour
{
    public enum Kind { Choice, Toggle, Slider }

    public Kind kind;
    public string key, title;
    [TextArea] public string description;
    public string[] options;
    public float defaultValue;

    public Button prev, next, toggle;
    public Slider slider;
    public TMP_Text valueText;
    public RectTransform knob;
    public Image track;

    public Action<SettingControl> onChanged, onFocus;
    public float Value { get; private set; }

    public void Init()
    {
        if (prev) prev.onClick.AddListener(() => Set(Mathf.Repeat(Value - 1, options.Length), true, true));
        if (next) next.onClick.AddListener(() => Set(Mathf.Repeat(Value + 1, options.Length), true, true));
        if (toggle) toggle.onClick.AddListener(() => Set(Value > 0.5f ? 0 : 1, true, true));
        if (slider) slider.onValueChanged.AddListener(v => { if (!Mathf.Approximately(v, Value)) Set(v, true, true); });
        foreach (var s in GetComponentsInChildren<Selectable>(true))
        {
            var et = s.gameObject.GetComponent<EventTrigger>();
            if (et == null) et = s.gameObject.AddComponent<EventTrigger>();
            var e = new EventTrigger.Entry { eventID = EventTriggerType.Select };
            e.callback.AddListener(_ => onFocus?.Invoke(this));
            et.triggers.Add(e);
        }
    }

    public Selectable First => kind == Kind.Choice ? next : kind == Kind.Toggle ? (Selectable)toggle : slider;

    public void Set(float v, bool animate, bool notify)
    {
        Value = v;
        switch (kind)
        {
            case Kind.Choice:
                valueText.text = options[Mathf.RoundToInt(v)];
                if (animate)
                {
                    // small pop so you notice the value changed
                    valueText.transform.localScale = Vector3.one * 0.8f;
                    Tween.Scale(valueText.transform, 1, 0.25f, Ease.OutBack);
                }
                break;
            case Kind.Toggle:
                bool on = v > 0.5f;
                valueText.text = on ? "ON" : "OFF";
                valueText.color = on ? Palette.Text : Palette.TextDim;
                var to = new Vector2(on ? 30 : -30, 0);
                var col = on ? Palette.Accent : Palette.Line;
                if (animate && Application.isPlaying)
                {
                    Tween.Move(knob, to, 0.22f, Ease.InOutQuad);
                    Tween.Tint(track, col, 0.22f, Ease.InOutQuad);
                }
                else { knob.anchoredPosition = to; track.color = col; }
                break;
            case Kind.Slider:
                slider.SetValueWithoutNotify(v);
                valueText.text = Mathf.RoundToInt(v).ToString();
                break;
        }
        if (notify) onChanged?.Invoke(this);
    }
}
