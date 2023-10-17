#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet
{
    public abstract class Sheet : UIContext
    {
        [field: SerializeField] public bool IsRecycle { get; private set; } = true;
    }
}

#endif