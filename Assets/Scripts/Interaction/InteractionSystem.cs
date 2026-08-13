using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public Text interactionText;

    private IInteractable currentInteractable;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) 
        {
            if (interactionText != null) interactionText.gameObject.SetActive(false);
            return;
        }

        CheckInteraction();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                if (interactionText != null)
                {
                    interactionText.text = interactable.GetInteractionText();
                    interactionText.gameObject.SetActive(true);
                }
                return;
            }
        }

        currentInteractable = null;
        if (interactionText != null) interactionText.gameObject.SetActive(false);
    }
}
