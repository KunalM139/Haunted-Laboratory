using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    public bool requiresKey = false;
    public bool isFinalDoor = false;
    public SlowMotionController slowMoController;
    private bool isOpen = false;

    public string GetInteractionText()
    {
        if (isOpen) return "";
        if (isFinalDoor)
        {
            if (GameManager.Instance.hasKey) return "[E] Open Exit";
            return "Locked. Needs Key.";
        }
        return "[E] Open Door";
    }

    public void Interact()
    {
        if (isOpen) return;

        if (requiresKey && !GameManager.Instance.hasKey)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.errorSound != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.errorSound);
            return;
        }

        isOpen = true;
        if (AudioManager.Instance != null && AudioManager.Instance.doorOpenSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.doorOpenSound);

        transform.Rotate(0, 90, 0);

        if (isFinalDoor)
        {
            if (slowMoController != null) slowMoController.TriggerSlowMotion();
            else GameManager.Instance.Victory();
        }
    }
}
