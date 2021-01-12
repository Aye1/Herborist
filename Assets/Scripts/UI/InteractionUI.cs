using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;

public class InteractionUI : MonoBehaviour
{
    [SerializeField, Required]
    private PlayerInteractionManager player;

    [SerializeField, Required]
    private Translator _interactText;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerInteractionManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            Display(player.CanInteract && !GameManager.Instance.IsInPause);
        }
        UpdateInteractionText();
    }

    private void Display(bool visible)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }

    private void UpdateInteractionText()
    {
        if(_interactText != null && player != null && player.CanInteract)
        {
            _interactText.Key = player.GetCurrentInteractable().GetInteractionTextLocKey();
        }
    }
}
