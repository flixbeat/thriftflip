using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] private bool singleton = true;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float speechInterval = 0.1f;

    private bool doneSpeaking;
    private Coroutine speechRoutine;

    private void Awake()
    {
        if(singleton)
            Instance = this;
    }

    public void PopulateSpeechBubble(string dialogue, float speechAfterDelay, TextMeshProUGUI text = null, Action actionOnEnd = null)
    {
        doneSpeaking = false;

        if (speechRoutine != null)
            StopCoroutine(speechRoutine);

        speechRoutine = StartCoroutine(PopulateDialogueSpeechBubbleRoutine(dialogue, speechAfterDelay, text, actionOnEnd));
    }

    private IEnumerator PopulateDialogueSpeechBubbleRoutine(string dialogue, float speechAfterDelay, TextMeshProUGUI text = null, Action actionOnEnd = null)
    {
        //characterAnim.SetInteger("condition", animCondition);

        var textToAdjust = text == null ? dialogueText : text;
        textToAdjust.text = "";

        foreach (char c in dialogue)
        {
            textToAdjust.text = $"{textToAdjust.text}{c}";
            yield return new WaitForSeconds(speechInterval);
        }
        yield return new WaitForSeconds(speechAfterDelay);
        doneSpeaking = true;

        if (actionOnEnd != null)
            actionOnEnd.Invoke();
    }
}
