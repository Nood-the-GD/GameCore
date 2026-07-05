
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.FileLoader
{
    public class RemoteFileLoader : IFileLoader
    {
        private string _link;
        private string _localPath;
        public int RetryCount => 3;

        public int TimeOut => 10;

        public RemoteFileLoader(string link, string localPath)
        {
            _link = link;
            _localPath = localPath;
        }


        public async UniTask<byte[]> LoadAsync()
        {
            // TODO: download file
            using (UnityWebRequest request = UnityWebRequest.Get(_link))
            {
                request.downloadHandler = new DownloadHandlerFile(_localPath);
                await request.SendWebRequest();


                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("File downloaded successfully");
                    return request.downloadHandler.data;
                }
                else
                {
                    Debug.Log("Failed to download file: " + request.error);
                    return null;
                }
            }
        }
    }
}