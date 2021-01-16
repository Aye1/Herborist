using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;
using Sirenix.OdinInspector;

public class StorageBox : MonoBehaviour, IInteractable
{
    [SerializeField, TranslationKey]
    private string _localizationKey;

    [SerializeField, Required, SceneObjectsOnly]
    private StorageBoxUI _storageBoxUI;

    #region IInteractable implementation
    public bool CanInteract()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetInteractionTextLocKey()
    {
        return _localizationKey;
    }

    public void Interact(GameObject aPLayer)
    {
        _storageBoxUI.gameObject.SetActive(true);
        List<CollectiblePackage> newCollectibles = PlayerMovement.Instance.GetComponent<Inventory>().EmptyInventory();
        _storageBoxUI.Collectibles = newCollectibles;
        HouseStorage.Instance.StorageInventory.Add(newCollectibles);
    }
    #endregion
}
