using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unisloth.Localization;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/Plant")]
public class PlantScriptableObject : ScriptableObject
{
    public string unidentifiedPlantLetter;
    [TranslationKey]
    public string commonNameLocKey;
    public string speciesName;
    public string familyName;
    public List<string> loreLocKey;
    public Sprite fullPicture;
    public Sprite blackAndWhitePicture;
    public Sprite inGameSprite; //  TODO add sprite for each seasons

    public List<PlantComponentScriptableObject> components;
}
