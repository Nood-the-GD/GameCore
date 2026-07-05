using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.FileLoader
{
    public class FileLoader
    {
        private Action<byte[]> _onSuccess { get; set; }
        private Action<string> _onError { get; set; }

        private List<IFileLoader> _fileLoaders = new List<IFileLoader>();

        public FileLoader AddFileLoader(string filePath)
        {
            var loader = new LocalFileLoader(filePath);
            _fileLoaders.Add(loader);
            return this;
        }

        /// <summary>
        /// Load file from a link
        /// </summary>
        /// <param name="link">link to load</param>
        /// <param name="localPath">if add, save downloaded file to this path</param>
        /// <returns></returns>
        public FileLoader AddRemoteLoader(string link, string localPath)
        {
            var loader = new RemoteFileLoader(link, localPath);
            _fileLoaders.Add(loader);
            return this;
        }

        public FileLoader AddResourcesLoader(string filePath)
        {
            var loader = new ResourcesLoader(filePath);
            _fileLoaders.Add(loader);
            return this;
        }

        public FileLoader WithSuccess(Action<byte[]> onSuccess)
        {
            _onSuccess = onSuccess;
            return this;
        }

        public FileLoader WithError(Action<string> onError)
        {
            _onError = onError;
            return this;
        }

        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(0.5f);

        public async UniTask Start()
        {
            if (_fileLoaders.Count == 0)
            {
                _onError?.Invoke("Can't load file because do loaders");
                return;
            }

            for (int i = 0; i < _fileLoaders.Count; i++)
            {
                var loader = _fileLoaders[i];
                var isLastLoader = i == _fileLoaders.Count - 1;

                for (int attempt = 0; attempt < loader.RetryCount; attempt++)
                {
                    try
                    {
                        var data = await loader.LoadAsync().Timeout(TimeSpan.FromSeconds(loader.TimeOut));
                        _onSuccess?.Invoke(data);
                        return;
                    }
                    catch (Exception e)
                    {
                        var isLastAttempt = attempt == loader.RetryCount - 1;
                        if (isLastAttempt)
                        {
                            Debug.LogError($"[FileLoader] {loader.GetType().Name} exhausted {loader.RetryCount} attempt(s): {e}");
                            break;
                        }

                        Debug.LogWarning($"[FileLoader] {loader.GetType().Name} attempt {attempt + 1}/{loader.RetryCount} failed: {e.Message}, retrying...");
                        await UniTask.Delay(RetryDelay, DelayType.UnscaledDeltaTime);
                    }
                }

                if (isLastLoader)
                {
                    _onError?.Invoke("Can't load file");
                }
            }
        }
    }
}