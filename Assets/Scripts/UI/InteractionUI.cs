using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;

public class InteractionUI : MonoBehaviour
{
    private PlayerInteractionManager _player;

    [SerializeField, Required]
    private Translator _interactText;

    [SerializeField, Required, ChildGameObjectsOnly] PlantInformationUI _plantInformation;


    // Start is called before the first frame update
    void Start()
    {
        _player = PlayerMovement.Instance.GetComponent<PlayerInteractionManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(_player != null)
        {
            Display(_player.CanInteract && !GameManager.Instance.IsInPause);
            if(_player.GetCurrentInteractable() is Collectible)
            {
                DisplayPlantInformation(_player.GetCurrentInteractable() as Collectible);
            } else
            {
                HidePlantInformation();
            }
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
            _interactText.Key = _player.GetCurrentInteractable().GetInteractionTextLocKey();
        }
    }

    private void DisplayPlantInformation(Collectible collectible)
    {
        _plantInformation.gameObject.SetActive(true);
        _plantInformation.CurrentCollectible = collectible;
    }

    private void HidePlantInformation()
    {
        _plantInformation.gameObject.SetActive(false);
    }
}
