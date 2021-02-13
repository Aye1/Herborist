using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/Plant")]
public class PlantScriptableObject : ScriptableObject
{
    public string unidentifiedNameLocKey;
    public string commonNameLocKey;
    public string speciesName;
    public string familyName;
    public List<string> loreLocKey;
    public Sprite fullPicture;
    public Sprite blackAndWhitePicture;

    public List<PlantComponentScriptableObject> components;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
