using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineVisualColorChanger : MonoBehaviour
{
    private LineRenderer line;

    void Start()
    {
        this.line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        ColorManager.OnColorChanged += this.ChangeColor;
    }

    void OnDisable()
    {
        ColorManager.OnColorChanged -= this.ChangeColor;
    }

    private void ChangeColor(Color newColor)
    {
        this.line.endColor = newColor;
    }
}
