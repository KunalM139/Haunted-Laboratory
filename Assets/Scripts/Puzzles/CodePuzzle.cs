using UnityEngine;
using TMPro;

public class CodePuzzle : MonoBehaviour, IInteractable
{
    public string correctCode = "1984";
    public string currentCode = "";
    public TextMeshProUGUI display;
    
    private bool isSolved = false;

    public string GetInteractionText()
    {
        if (!GameManager.Instance.isPowerRestored) return "No Power";
        if (isSolved) return "Code Accepted";
        return "[E] Use Keypad";
    }

    public void Interact()
    {
        if (!GameManager.Instance.isPowerRestored || isSolved) return;
        
        // Simulating entering code for PC keyboard
        currentCode = correctCode; 
        CheckCode();
    }

    void CheckCode()
    {
        if (currentCode == correctCode)
        {
            isSolved = true;
            GameManager.Instance.isCodeSolved = true;
            if (display != null) display.text = "SUCCESS";
            if (AudioManager.Instance != null && AudioManager.Instance.successSound != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.successSound);
            }
        }
        else
        {
            currentCode = "";
            if (display != null) display.text = "ERROR";
            if (AudioManager.Instance != null && AudioManager.Instance.errorSound != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.errorSound);
            }
        }
    }
}
