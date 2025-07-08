using Cysharp.Threading.Tasks;
using System.Threading;

namespace CharacterCreation
{
    public interface IDataCreator
    {
        UniTask LoadAsync(CancellationToken cancellationToken = default);
    }
}

