using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineVisualColorChanger : MonoBehaviour
{
    private LineRenderer line;

    void Awake()
    {
        this.line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        ColorManager.OnColorChanged += this.ChangeColor;
        BrushManager.OnSizeChanged += this.ChangeSize;
    }

    void OnDisable()
    {
        ColorManager.OnColorChanged -= this.ChangeColor;
        BrushManager.OnSizeChanged -= this.ChangeSize;
    }

    private void ChangeSize(int newSize)
    {
        this.line.widthMultiplier = newSize * .1f;
    }

    private void ChangeColor(Color newColor)
    {
        this.line.endColor = newColor;
    }
}
