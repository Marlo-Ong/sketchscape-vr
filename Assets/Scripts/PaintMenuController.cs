using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PaintMenuController : MonoBehaviour
{
    [SerializeField] private GameObject paintMenuPanel;
    private InputActionReference openPaintMenuAction;

  void Awake()
  {
    openPaintMenuAction.action.Enable();
    openPaintMenuAction.action.performed += TogglePaintMenu;
    InputSystem.onDeviceChange += OnDeviceChange;
  }

  private void OnDestroy()
  {
    openPaintMenuAction.action.Disable();
    openPaintMenuAction.action.performed -= TogglePaintMenu;
    InputSystem.onDeviceChange -= OnDeviceChange;
  }

  private void TogglePaintMenu(InputAction.CallbackContext context)
  {
    paintMenuPanel.SetActive(!paintMenuPanel.activeSelf);
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
}
