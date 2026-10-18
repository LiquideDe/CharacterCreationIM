using System;
using System.IO;
using UnityEngine;

namespace CharacterCreation
{
    public static class ApplicationExit
    {
        public static void OpenCharactersFolderAndQuit()
        {            
            Application.OpenURL((Application.dataPath) + "/StreamingAssets/Персонажи");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
