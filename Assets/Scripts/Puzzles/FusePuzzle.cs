using UnityEngine;

public class FusePuzzle : MonoBehaviour, IInteractable
{
    public bool isFusePickedUp = false;
    public Light[] emergencyLights;
    public Light[] mainLights;
    public Material screenOnMaterial;
    public MeshRenderer[] screens;

    public string GetInteractionText()
    {
        if (GameManager.Instance.isPowerRestored) return "";
        if (isFusePickedUp) return "[E] Insert Fuse";
        return "[E] Pick Up Fuse";
    }

    public void Interact()
    {
        if (GameManager.Instance.isPowerRestored) return;

        if (!isFusePickedUp)
        {
            isFusePickedUp = true;
            // Optionally play pick up sound
        }
        else
        {
            // Restore power
            GameManager.Instance.isPowerRestored = true;
            if (AudioManager.Instance != null && AudioManager.Instance.powerRestoreSound != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.powerRestoreSound);
            }

            foreach (var light in emergencyLights) light.enabled = false;
            foreach (var light in mainLights) light.enabled = true;
            foreach (var screen in screens) screen.material = screenOnMaterial;
        }
    }
}
