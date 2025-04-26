using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour
{
    [SerializeField] private List<Toggle> colorToggles = new List<Toggle>();
    
    private Color currentColor;
    
    public delegate void ColorChangedEvent(Color newColor);
    public event ColorChangedEvent OnColorChanged;
    
    private void Start()
    {
        foreach (Toggle toggle in colorToggles)
        {
            toggle.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    Image toggleImage = toggle.targetGraphic as Image;
                    SelectColor(toggleImage.color);
                }
            });
            
            if (toggle.isOn)
            {
                Image toggleImage = toggle.targetGraphic as Image;
                SelectColor(toggleImage.color);
            }
        }
    }
    
    private void SelectColor(Color color)
    {
        currentColor = color;
    
        OnColorChanged?.Invoke(currentColor);
        Debug.Log($"Color changed to: {currentColor}");
    }
    
    public Color GetCurrentColor()
    {
        return currentColor;
    }
}