using UnityEngine;

public class KeyItem : MonoBehaviour, IInteractable
{
    public string GetInteractionText()
    {
        return "[E] Take Key";
    }

    public void Interact()
    {
        GameManager.Instance.hasKey = true;
        if (AudioManager.Instance != null && AudioManager.Instance.successSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.successSound);
        }
        gameObject.SetActive(false);
    }
}
