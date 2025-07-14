using TMPro;
using UnityEngine;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
    public static MessageDisplay Instance { get; private set; }

    public TextMeshProUGUI messageText;
    public float displayTime = 2f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        if (messageText != null)
            messageText.text = "";
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        messageText.text = message;
        yield return new WaitForSeconds(displayTime);
        messageText.text = "";
    }
}
