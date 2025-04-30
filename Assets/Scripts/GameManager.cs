using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private string[] prompts;
    [SerializeField] private int timeLimit;

    [Header("References To Assign")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text promptText;

    private List<string> unusedPrompts;
    private Coroutine roundEndTimer;

    private void Start()
    {
        this.unusedPrompts = new();
    }


    #region Game Loop Methods

    public void StartRound()
    {
        Debug.Assert(this.roundEndTimer == null);

        this.SetPromptRandom();
        this.promptPanel.SetActive(true);
        this.roundEndTimer = StartCoroutine(this.WaitForCallback(this.timeLimit, this.EndRound));
    }

    public void EndRound()
    {
        this.roundEndTimer = null;
        this.promptPanel.SetActive(false);
    }

    #endregion

    #region Prompt Text Methods

    public void SetText(string text)
    {
        Debug.Assert(this.promptText != null);
        this.promptText.text = text;
    }

    public void SetPromptRandom()
    {
        Debug.Assert(this.prompts != null);
        Debug.Assert(this.prompts.Length != 0);
        Debug.Assert(this.unusedPrompts != null);

        // Refresh prompts if all have been used.
        if (this.unusedPrompts.Count == 0)
            this.unusedPrompts.AddRange(this.prompts);

        // Pick a random unused prompt.
        int i = UnityEngine.Random.Range(0, this.unusedPrompts.Count);
        this.SetText(this.unusedPrompts[i]);
    }

    #endregion

    private IEnumerator WaitForCallback(int seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback();
    }
}