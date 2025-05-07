using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UIMenuController : MonoBehaviour
{
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject changeEnvironmentPanel;
  [SerializeField] private GameObject viewOptionsPanel;
  [SerializeField] private GameObject viewControlsPanel;

  [SerializeField] private Button startButton;
  [SerializeField] private Button changeButton;
  [SerializeField] private Button exitButton;
  [SerializeField] private Button closeButton;
  [SerializeField] private Button viewOptionsButton;
  [SerializeField] private Button viewControlsButton;
  [SerializeField] private List<Button> returnButtons;
  [SerializeField] private InputActionReference openMenuShortcut;

  void Start()
  {
    OpenPanel(viewOptionsPanel);

    openMenuShortcut.action.Enable();
    openMenuShortcut.action.performed += (ctx) => OpenPanel(menuPanel);
    viewOptionsButton.onClick.AddListener(() => OpenPanel(menuPanel));
    changeButton.onClick.AddListener(() => OpenPanel(changeEnvironmentPanel));
    closeButton.onClick.AddListener(() => OpenPanel(viewOptionsPanel));
    viewControlsButton.onClick.AddListener(() => OpenPanel(viewControlsPanel));

    foreach (Button button in returnButtons)
    {
      button.onClick.AddListener(() => OpenPanel(menuPanel));
    }

    exitButton.onClick.AddListener(ExitGame);
  }

  public void ExitGame()
  {
    // works in build, not in unity editor play test
#if !UNITY_EDITOR
    Application.Quit();
#endif
  }

  public void OpenPanel(GameObject panelToOpen)
  {
    menuPanel.SetActive(false);
    changeEnvironmentPanel.SetActive(false);
    viewOptionsPanel.SetActive(false);
    viewControlsPanel.SetActive(false);

    panelToOpen.SetActive(true);
  }
}