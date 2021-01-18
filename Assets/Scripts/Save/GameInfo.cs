using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoState : SaveState
{
    public int saveNumber;
    public int timePlayedSeconds;
}

[CreateAssetMenu(fileName = "Game Info", menuName = "ScriptableObjects/Game Info")]
public class GameInfo : ScriptableObject, ISavable
{
    public int saveNumber;

    public int TimePlayedSeconds { get; private set; }

    public SaveState GetObjectToSave()
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
    }
}
