using System;
using UnityEngine;

namespace Mine.Code.Main.Model
{
    [Serializable]
    public class MainFolderModel
    {
        #region Properties
    
        // Folders
        [field: SerializeField] public Transform JellyFolder { get; private set; }

        #endregion
    }
}