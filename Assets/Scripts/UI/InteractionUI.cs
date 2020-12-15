using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;

public class InteractionUI : MonoBehaviour
{
    PlayerInteractionManager _player;

    [SerializeField, Required]
    private Translator _interactText;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerInteractionManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(_player != null)
        {
            Display(_player.CanInteract);
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
        if(_interactText != null && _player != null && _player.CanInteract)
        {
            // TODO: update key instead of text, when fetching the latest localization package version
            _interactText.SetText(LocalizationManager.Instance.GetTranslation(_player.GetCurrentInteractable().GetInteractionTextLocKey()));
        }
    }
}
