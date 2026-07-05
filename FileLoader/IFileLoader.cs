
using Cysharp.Threading.Tasks;

namespace Core.FileLoader
{
    internal interface IFileLoader
    {
        public int RetryCount { get; }
        public int TimeOut { get; }

        public UniTask<byte[]> LoadAsync();
    }
}