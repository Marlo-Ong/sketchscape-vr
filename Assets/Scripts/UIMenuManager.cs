using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenuManager : MonoBehaviour
{
  public GameObject menuPanel;
  public GameObject changeEnvironmentPanel;
  public GameObject viewOptionsPanel;

  public Button startButton;
  public Button changeButton;
  public Button exitButton;
  public Button closeButton;
  public Button viewOptionsButton;
  public Button returnButton;

  void Start()
  {
    DisableUIMenu();

    viewOptionsButton.onClick.AddListener(EnableUIMenu);
    changeButton.onClick.AddListener(EnableChangeEnvironmentPanel);
    startButton.onClick.AddListener(StartGame);
    exitButton.onClick.AddListener(ExitGame);
    closeButton.onClick.AddListener(DisableUIMenu);
    startButton.onClick.AddListener(StartGame);
    returnButton.onClick.AddListener(EnableUIMenu);
  }

  public void StartGame()
  {
    //do stuff
    // SceneManager.LoadScene(1);
  }

  public void ExitGame()
  {
    // works in build, not in unity editor play test
    Debug.Log("Exit only works in build.");
    Application.Quit();
  }

  public void EnableChangeEnvironmentPanel()
  {
    menuPanel.SetActive(false);
    changeEnvironmentPanel.SetActive(true);
  }

  public void EnableUIMenu()
  {
    changeEnvironmentPanel.SetActive(false);
    viewOptionsPanel.SetActive(false);
    menuPanel.SetActive(true);
  }

  public void DisableUIMenu()
  {
    changeEnvironmentPanel.SetActive(false);
    menuPanel.SetActive(false);
    viewOptionsPanel.SetActive(true);
  }
}