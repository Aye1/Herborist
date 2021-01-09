using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveState { };

public interface ISavable
{
    SaveState GetObjectToSave();
    void LoadObject(SaveState saveState);
    string GetSaveName();
}
