using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Mine.Code.Framework.Manager.History
{
    public class HistoryManager
    {
        #region Enum
    
        public enum State
        {
            Free,
            Busy, // 전환 중
        }

        #endregion

        #region Fields

        readonly List<ICommand> history = new();
        int focusIndex = -1;
        State state = State.Free;

        #endregion

        #region Public Methods

        public async UniTask ExecuteAsync(ICommand command)
        {
            if(state == State.Busy) return;
        
            state = State.Busy;
        
            history.RemoveRange(focusIndex, history.Count - focusIndex);
            history.Add(command);
            focusIndex = history.Count - 1;
            await command.ExecuteAsync();
        
            state = State.Free;
        }

        public async UniTask UndoAsync()
        {
            if(state == State.Busy) return;
        
            state = State.Busy;
            if (focusIndex - 1 >= 0) await history[focusIndex--].UndoAsync();
        
            state = State.Free;
        }

        public async UniTask RedoAsync()
        {
            if(state == State.Busy) return;
        
            state = State.Busy;
        
            if (focusIndex + 1 < history.Count) await history[++focusIndex].RedoAsync();
        
            state = State.Free;
        }

        #endregion
    }
}