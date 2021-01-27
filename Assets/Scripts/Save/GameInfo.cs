using UnityEngine;

/*public class GameInfoState : SaveState
{
    public int saveNumber;
    public float timePlayedSeconds;
}*/

[CreateAssetMenu(fileName = "Game Info", menuName = "ScriptableObjects/Game Info")]
public class GameInfo : ScriptableObject
{
    public int saveNumber;

    public float TimePlayedSeconds { get; private set; }

    public bool isGameStarted;

    /*public SaveState GetObjectToSave()
    {
        return new GameInfoState()
        {
            saveNumber = saveNumber,
            timePlayedSeconds = TimePlayedSeconds
        };
    }

    public string GetSaveName()
    {
        return "game" + saveNumber + ".gameinfo";
    }

    public void LoadObject(SaveState saveState)
    {
        GameInfoState state = saveState as GameInfoState;
        saveNumber = state.saveNumber;
        TimePlayedSeconds = state.timePlayedSeconds;
    }*/

    public void AddGameTime(float timeSec)
    {
        TimePlayedSeconds += timeSec;
    }
}
