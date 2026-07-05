using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.FileLoader
{
    public class ResourcesLoader : IFileLoader
    {
        private string _filePath;
        public int RetryCount => 1;
        public int TimeOut => 10;

        public ResourcesLoader(string filePath)
        {
            _filePath = filePath;
        }

        public async UniTask<byte[]> LoadAsync()
        {
            var request = Resources.LoadAsync<TextAsset>(_filePath);
            await request;

            var textAsset = request.asset as TextAsset;
            return textAsset != null ? textAsset.bytes : null;
        }
    }
}