using Cysharp.Threading.Tasks;

namespace Mine.Code.Framework.Manager.History
{
    public interface ICommand
    {
        UniTask ExecuteAsync();
        UniTask UndoAsync();
        UniTask RedoAsync();
    }
}
