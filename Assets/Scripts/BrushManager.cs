using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrushManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    
    [SerializeField] private int minSize = 1;
    [SerializeField] private int maxSize = 10;
    [SerializeField] private int sizeStep = 1;
    
    private int currentSize = 5;
    
    public delegate void SizeChangedEvent(int newSize);
    public event SizeChangedEvent OnSizeChanged;
    
    private void Start()
    {
        UpdateSizeDisplay();

        increaseButton.onClick.AddListener(IncreaseSize);
        decreaseButton.onClick.AddListener(DecreaseSize);
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