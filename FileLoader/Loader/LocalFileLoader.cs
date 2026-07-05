
using Cysharp.Threading.Tasks;

namespace Core.FileLoader
{
    public class LocalFileLoader : IFileLoader
    {
        public int RetryCount => 1;
        public int TimeOut => 3;

        private string _filePath;

        public LocalFileLoader(string filePath)
        {
            _filePath = filePath;
        }

        public UniTask<byte[]> LoadAsync()
        {
            return UniTask.FromResult(System.IO.File.ReadAllBytes(_filePath));
        }
    }
}