using UnityEngine;

public class DrawerController : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    public Vector3 openOffset = new Vector3(0, 0, -0.5f);

    public string GetInteractionText()
    {
        if (isOpen) return "";
        if (!GameManager.Instance.isCodeSolved) return "Locked";
        return "[E] Open Drawer";
    }

    public void Interact()
    {
        if (isOpen || !GameManager.Instance.isCodeSolved) return;

        isOpen = true;
        transform.localPosition += openOffset;
        
        if (AudioManager.Instance != null && AudioManager.Instance.doorOpenSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.doorOpenSound);
    }
}
