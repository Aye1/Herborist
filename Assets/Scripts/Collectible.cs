using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using Unisloth.Localization;

public class CollectiblePackage : ISerializationCallbackReceiver
{
    [NonSerialized, ShowInInspector] public CollectibleScriptableObject type;
    public int count;

    private static string UNKNOWN_TYPE = "unknown";

    [SerializeField, HideInInspector] private string typeDevelopmentName;

    public void OnAfterDeserialize()
    {

        if (typeDevelopmentName == UNKNOWN_TYPE)
        {
            Debug.LogWarning("Unknown type has been serialized, skip deserialization");
        }
        else if (ResourcesManager.Instance != null && typeDevelopmentName != UNKNOWN_TYPE)
        {
            // Fetch CollectibleScriptableObject from Resources
            type = ResourcesManager.Instance.GetCollectibleScriptableObjectWithName(typeDevelopmentName);
        }
    }

    public void OnBeforeSerialize()
    {
        // Serialize the development name as a key to find the object after deserialization
        typeDevelopmentName = type == null ? UNKNOWN_TYPE : type.developmentName;
    }
}

public class Collectible : MonoBehaviour, IInteractable
{
    [ReadOnly, ShowInInspector]
    private int _collectibleCount;
    private int _maxCollectibleCount;

    [Required]
    public CollectibleScriptableObject collectible;
    [Required]
    public MMFeedbacks feedback;
    [SerializeField, Required, TranslationKey]
    private string _translationKey;
    [SerializeField]
    private List<SpriteDependsOnQuantity> _spriteChangers;

    private void Awake()
    {
        _maxCollectibleCount = Alea.GetIntInc(collectible.spawnQuantity.x, collectible.spawnQuantity.y);
        _collectibleCount = _maxCollectibleCount;
        UpdateSpriteIfNecessary();
    }

    public void Interact(GameObject aPlayer)
    {
        Inventory inventory = aPlayer.GetComponent<Inventory>();
        List<CollectiblePackage> packages = GetCollectibles();
        bool isPreventingToAdd = false;
        foreach (CollectiblePackage package in packages)
        {
            if (inventory.CanAddCollectible(package))
            {
                _collectibleCount -= package.count;

                feedback.PlayFeedbacks();
                inventory.Add(package);
                UpdateSpriteIfNecessary();
                DynamicUIManager.Instance.SpawnCollectiblePicked(package);
            }
            else
            {
                isPreventingToAdd = true;
            }
        }

        if (isPreventingToAdd)
        {
            inventory.PlayInventoryFull();
        }
    }

    public bool CanInteract()
    {
        return _collectibleCount > 0;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public List<CollectiblePackage> GetCollectibles()
    {
        CollectiblePackage pck = new CollectiblePackage();
        pck.type = collectible;
        int randomPickQuantity = Alea.GetIntInc(collectible.handGatherQuantity.x, collectible.handGatherQuantity.y);
        pck.count = Mathf.Min(randomPickQuantity, _collectibleCount);

        List<CollectiblePackage> res = new List<CollectiblePackage>() {
                pck
            };
        return res;
    }

    public string GetInteractionTextLocKey()
    {
        return _translationKey;
    }

    private void UpdateSpriteIfNecessary()
    {
        if (_spriteChangers != null)
        {
            int percent = Mathf.CeilToInt(_collectibleCount / (float)_maxCollectibleCount * 100.0f);
            _spriteChangers.ForEach(s => s.CurrentPercentage = percent);
        }
    }
}
