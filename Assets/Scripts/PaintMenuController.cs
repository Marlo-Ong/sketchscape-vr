using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaintMenuController : MonoBehaviour
{
    [SerializeField] private GameObject changeColorPanel;
    [SerializeField] private GameObject changeBrushPanel;
    [SerializeField] private GameObject buttonsRef;
    [SerializeField] private GameObject POVReference;
    [SerializeField] private InputActionReference openPaintMenuAction;
    [SerializeField] private GameObject currentPanel;

    [SerializeField] private Button colorButton;
    [SerializeField] private Button brushButton;

  void Start()
  {
    openPaintMenuAction.action.Enable();
    openPaintMenuAction.action.performed += TogglePaintMenu;
    InputSystem.onDeviceChange += OnDeviceChange;

    changeBrushPanel.SetActive(false);
    changeColorPanel.SetActive(false);
    buttonsRef.SetActive(false);

    currentPanel = changeColorPanel;

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
    changeColorPanel.transform.position = POVReference.transform.position;
    changeColorPanel.transform.rotation = POVReference.transform.rotation;
    changeBrushPanel.transform.position = POVReference.transform.position;
    changeBrushPanel.transform.rotation = POVReference.transform.rotation;
    buttonsRef.transform.position = POVReference.transform.position;
    buttonsRef.transform.rotation = POVReference.transform.rotation;

    changeBrushPanel.SetActive(false);
    changeColorPanel.SetActive(false);
    buttonsRef.SetActive(!buttonsRef.activeSelf);
    currentPanel.SetActive(buttonsRef.activeSelf);

    if(currentPanel == changeColorPanel) colorButton.interactable = false;
    else brushButton.interactable = false;
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

    if(panelToOpen == changeColorPanel)
    {
        colorButton.interactable = false;
        brushButton.interactable = true;
    }
    else
    {
        colorButton.interactable = true;
        brushButton.interactable = false;
    }

    currentPanel = panelToOpen;
  }
}
