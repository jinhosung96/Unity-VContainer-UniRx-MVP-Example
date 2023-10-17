#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Modal
{
    public abstract class Modal : UIContext
    {
        #region Propertys

        public CanvasGroup BackDrop { get; internal set; }

        #endregion
    }
}
#endif