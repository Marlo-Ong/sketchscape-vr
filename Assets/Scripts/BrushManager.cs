using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class BrushManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;

    [Header("Input Properties")]
    [Tooltip("Seconds between each step while holding")]
    [SerializeField] private float repeatDelay = 0.1f;
    [Tooltip("Axis deadzone before we start stepping")]
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private InputActionReference increaseSizeShortcut;
    [SerializeField] private InputActionReference decreaseSizeShortcut;
    [SerializeField] private InputActionReference joystickAction;

    [Header("Brush Properties")]
    [SerializeField] private int minSize = 1;
    [SerializeField] private int maxSize = 10;
    [SerializeField] private int sizeStep = 1;

    private int currentSize = 5;
    float timer = -1;

    public delegate void SizeChangedEvent(int newSize);
    public static event SizeChangedEvent OnSizeChanged;

    private void Start()
    {
        UpdateSizeDisplay();

        increaseButton.onClick.AddListener(IncreaseSize);
        decreaseButton.onClick.AddListener(DecreaseSize);

        joystickAction.action.Enable();
        increaseSizeShortcut.action.Enable();
        decreaseSizeShortcut.action.Enable();
        increaseSizeShortcut.action.performed += (ctx) => IncreaseSize();
        decreaseSizeShortcut.action.performed += (ctx) => DecreaseSize();

        OnSizeChanged.Invoke(currentSize);
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
                if (v > 0f)
                    IncreaseSize();
                else
                    DecreaseSize();
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

    public void ReadJoystick(InputAction.CallbackContext ctx)
    {
        // Take the magnitude of the vertical component.
        int delta = (int)ctx.ReadValue<Vector2>().normalized.y;

        currentSize = delta > 0
            ? Mathf.Min(currentSize + delta, maxSize)
            : Mathf.Max(currentSize + delta, minSize);
        UpdateSizeDisplay();
        OnSizeChanged?.Invoke(currentSize);
        Debug.Log($"Brush size changed to: {currentSize}");
    }

    public void IncreaseSize()
    {
        currentSize = Mathf.Min(currentSize + sizeStep, maxSize);
        UpdateSizeDisplay();
        OnSizeChanged?.Invoke(currentSize);
        Debug.Log($"Brush size increased to: {currentSize}");
    }

    public void DecreaseSize()
    {
        currentSize = Mathf.Max(currentSize - sizeStep, minSize);
        UpdateSizeDisplay();
        OnSizeChanged?.Invoke(currentSize);
        Debug.Log($"Brush size decreased to: {currentSize}");
    }

    private void UpdateSizeDisplay()
    {
        sizeText.text = currentSize.ToString();

        decreaseButton.interactable = currentSize != minSize;
        increaseButton.interactable = currentSize != maxSize;
    }

    public float GetCurrentSize()
    {
        return currentSize;
    }
}