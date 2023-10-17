using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class VObject<T> where T : LifetimeScope
{
    #region Properties
    
    [Inject] protected LifetimeScope context { private get; set; }

    protected T Context => context as T;

    #endregion
}
