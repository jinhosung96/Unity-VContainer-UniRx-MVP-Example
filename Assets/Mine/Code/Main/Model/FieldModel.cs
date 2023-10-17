using System;
using System.Collections.Generic;
using Mine.Code.Jelly;

namespace Mine.Code.Main.Model
{
    [Serializable]
    public class FieldModel
    {
        #region Fields

        List<JellyModel> jellies;

        #endregion

        #region Properties

        public List<JellyModel> Jellies
        {
            get => jellies;
            set => jellies = value;
        }

        #endregion
    }
}