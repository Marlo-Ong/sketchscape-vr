using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ColorManager : MonoBehaviour
{
    [SerializeField] private List<Toggle> colorToggles = new List<Toggle>();

    [Header("Input Properties")]
    [Tooltip("Seconds between each step while holding")]
    [SerializeField] private float repeatDelay = 0.1f;
    [Tooltip("Axis deadzone before we start stepping")]
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private InputActionReference increaseSizeShortcut;
    [SerializeField] private InputActionReference decreaseSizeShortcut;
    [SerializeField] private InputActionReference joystickAction;

    private Color currentColor;
    private float timer = -1;
    private int currentColorIndex = 0;
    private List<Color> colors;

    public delegate void ColorChangedEvent(Color newColor);
    public static event ColorChangedEvent OnColorChanged;

    private void Start()
    {
        this.colors = new();

        foreach (Toggle toggle in colorToggles)
        {
            Image toggleImage = toggle.targetGraphic as Image;
            var color = toggleImage.color;
            this.colors.Add(color);

            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    SelectColor(color);
            });

            if (toggle.isOn)
                SelectColor(color);
        }

        OnColorChanged.Invoke(currentColor);
    }

    private void Update()
    {
        // 1. Read the current joystick value
        Vector2 axis = joystickAction.action.ReadValue<Vector2>();
        float v = axis.y;

        // 2. Only run the countdown while past the deadzone
        if (v > threshold || v < -threshold)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                // 3. Increment or decrement
                currentColorIndex = (v > 0f)
                    ? (currentColorIndex + 1) % this.colors.Count
                    : (currentColorIndex - 1) % this.colors.Count;
                SelectColor(this.colors[currentColorIndex]);

                // 4. Reset the timer
                timer = repeatDelay;
            }
        }
        else
        {
            // 5. Back in neutral, zero the timer so next push steps immediately
            timer = 0f;
        }
    }

    private void SelectColor(Color color)
    {
        currentColor = color;
        this.currentColorIndex = this.colors.IndexOf(color);

        OnColorChanged?.Invoke(currentColor);
        Debug.Log($"Color changed to: {currentColor}");
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }
}