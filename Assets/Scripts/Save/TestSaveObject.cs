using UnityEngine;

public class TestSaveState : SaveState
{
    public int customInt;
    public string customString;
}

public class TestSaveObject : MonoBehaviour, ISavable
{
    TestSaveState saveState;

    private void Start()
    {
        saveState = new TestSaveState()
        {
            customInt = 22,
            customString = "testString"
        };
    }

    public SaveState GetObjectToSave()
    {
        return saveState;
    }

    public string GetSaveName()
    {
        return "testSave.save";
    }

    public void LoadObject(SaveState saveState)
    {
        TestSaveState res = saveState as TestSaveState;
        Debug.Log("if it's ok, int = "+ res.customInt + " and string = '" + res.customString + "' ");
    }
}
