using R3;
using UnityEngine.InputSystem;

namespace HK
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Extensions
    {
        public static Observable<PlayerInput> OnControlsChangedAsObservable(this PlayerInput self)
        {
            return Observable.FromEvent<PlayerInput>(x => self.onControlsChanged += x, x => self.onControlsChanged -= x);
        }
    }
}