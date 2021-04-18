using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;
using Sirenix.OdinInspector;
using System.Linq;

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
        IEnumerable<CollectiblePackage> newCollectibles = PlayerMovement.Instance.GetComponent<Inventory>().EmptyAsPackages();

        _storageBoxUI.Collectibles = newCollectibles.ToList();

        HouseStorage.Instance.StorageInventory.Add(newCollectibles);
    }
    #endregion
}
