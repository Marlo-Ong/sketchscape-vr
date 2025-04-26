using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaintMenuController : MonoBehaviour
{
    [SerializeField] private GameObject changeColorPanel;
    [SerializeField] private GameObject changeBrushPanel;
    [SerializeField] private GameObject buttonsPanel; 
    [SerializeField] private GameObject POVReference;
    [SerializeField] private InputActionReference openPaintMenuAction;
    
    [SerializeField] private Button colorButton;
    [SerializeField] private Button brushButton;
    
    private bool isMenuOpen = false;

    void Start()
    {
        openPaintMenuAction.action.Enable();
        openPaintMenuAction.action.performed += TogglePaintMenu;
        InputSystem.onDeviceChange += OnDeviceChange;
        
        changeBrushPanel.SetActive(false);
        changeColorPanel.SetActive(false);
        buttonsPanel.SetActive(false);
        
        colorButton.onClick.AddListener(() => OpenPanel(changeColorPanel));
        brushButton.onClick.AddListener(() => OpenPanel(changeBrushPanel));
    }

    private void OnDestroy()
    {
        openPaintMenuAction.action.Disable();
        openPaintMenuAction.action.performed -= TogglePaintMenu;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void TogglePaintMenu(InputAction.CallbackContext context)
    {
        isMenuOpen = !isMenuOpen;
        
        if (isMenuOpen)
        {
            UpdateUIPosition();
            
            buttonsPanel.SetActive(true);
            changeColorPanel.SetActive(true);
            changeBrushPanel.SetActive(false);
            
            colorButton.interactable = false;
            brushButton.interactable = true;
        }
        else
        {
            buttonsPanel.SetActive(false);
            changeColorPanel.SetActive(false);
            changeBrushPanel.SetActive(false);
        }
    }
    
    private void UpdateUIPosition()
    {
        buttonsPanel.transform.position = POVReference.transform.position;
        buttonsPanel.transform.rotation = POVReference.transform.rotation;
        changeColorPanel.transform.position = POVReference.transform.position;
        changeColorPanel.transform.rotation = POVReference.transform.rotation;
        changeBrushPanel.transform.position = POVReference.transform.position;
        changeBrushPanel.transform.rotation = POVReference.transform.rotation;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch(change)
        {
            case InputDeviceChange.Disconnected:
                openPaintMenuAction.action.Disable();
                openPaintMenuAction.action.performed -= TogglePaintMenu;
                break;
            case InputDeviceChange.Reconnected:
                openPaintMenuAction.action.Enable();
                openPaintMenuAction.action.performed += TogglePaintMenu;
                break;
        }
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        changeColorPanel.SetActive(false);
        changeBrushPanel.SetActive(false);
        
        panelToOpen.SetActive(true);
        
        colorButton.interactable = panelToOpen != changeColorPanel;
        brushButton.interactable = panelToOpen != changeBrushPanel;
    }
}