using TMPro;
using UnityEngine;

// Fonts and sprites shared by every screen.
[CreateAssetMenu(menuName = "Nightline/UI Theme")]
public class UITheme : ScriptableObject
{
    [Header("Fonts")]
    public TMP_FontAsset heading;   // Rajdhani Bold - titles, numbers, buttons
    public TMP_FontAsset body;      // Barlow Regular - descriptions
    public TMP_FontAsset bodyBold;  // Barlow SemiBold - labels, small caps

    [Header("Sprites")]
    public Sprite round;   // Unity built-in UISprite (rounded box)
    public Sprite circle;  // Unity built-in Knob
    public Sprite[] icons; // Google Material Icons

    public Sprite Icon(string name)
    {
        foreach (var s in icons)
            if (s != null && s.name == name) return s;
        Debug.LogWarning("Missing sprite " + name);
        return null;
    }
}
