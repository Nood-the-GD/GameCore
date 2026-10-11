using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Extension;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = GDKSymbol.Module_Sound.Debug;

namespace Core.SoundManager
{
    public class SoundManager
    {
        private SoundDatabase _dataBase;
        private AudioSource _mainMusicSource;
        private AudioSource _backupMusicSource;
        private AudioListener _audioListener;
        private MusicEnum _playingMainSound;
        private float _globalSoundVolume = 1;
        private float _globalMusicVolume = 1;
        private bool _loadingDatabase = false;

        public void SetGlobalSoundVolume(float volume)
        {
            _globalSoundVolume = volume;
        }
        public void SetGlobalMusicVolume(float volume)
        {
            _globalMusicVolume = volume;
            _mainMusicSource.volume = volume;
        }

        public SoundManager()
        {
            LoadDataBaseIfNeed().Forget();
            SetupSound();
        }

        public void SetupSound()
        {
            GameObject soundObject = new GameObject("SoundManager");
            GameObject.DontDestroyOnLoad(soundObject);
            _mainMusicSource = soundObject.AddComponent<AudioSource>();
            _backupMusicSource = soundObject.AddComponent<AudioSource>();
            _audioListener = GameObject.FindAnyObjectByType<AudioListener>();
        }

        public void ChangeMusic(MusicEnum musicToChange, float duration = 0.5f)
        {
            if (_playingMainSound == musicToChange)
            {
                return;
            }

            _playingMainSound = musicToChange;
            _backupMusicSource.clip = GetClip(musicToChange);
            float backupVolume = 0;
            float endVolume = _globalMusicVolume;
            DOTween.To(() => backupVolume, t =>
            {
                backupVolume = t;
                _backupMusicSource.volume = backupVolume;
                _mainMusicSource.volume = endVolume - backupVolume;
            }, endVolume, duration).OnComplete(() =>
            {
                _mainMusicSource.clip = GetClip(musicToChange);
                _mainMusicSource.volume = _globalMusicVolume;
                _mainMusicSource.time = duration;
                _mainMusicSource.Play();
                _backupMusicSource.Stop();
                _backupMusicSource.clip = null;
            });

        }

        public async void PlayMusic(MusicEnum musicEnum)
        {
            await UniTask.WaitUntil(() => _loadingDatabase == false);
            if (_playingMainSound == musicEnum) return;

            _mainMusicSource.clip = GetClip(musicEnum);
            _mainMusicSource.volume = _globalMusicVolume;
            _mainMusicSource.loop = true;
            _mainMusicSource.Play();
        }

        public async void PlaySound(SoundListEnum soundListEnum)
        {
            await UniTask.WaitUntil(() => _loadingDatabase == false);
            var clip = _dataBase.soundListDic[soundListEnum.ToString()].GetRandom();
            Debug.Log("clip: " + clip.name);
            if (_audioListener == null)
            {
                _audioListener = GameObject.FindAnyObjectByType<AudioListener>();
            }
            AudioSource.PlayClipAtPoint(clip, _audioListener.transform.position, _globalSoundVolume);
        }

        public async void PlaySound(SoundEnum soundEnum)
        {
            await UniTask.WaitUntil(() => _loadingDatabase == false);
            if (_audioListener == null)
            {
                _audioListener = GameObject.FindAnyObjectByType<AudioListener>();
            }
            AudioSource.PlayClipAtPoint(GetClip(soundEnum), _audioListener.transform.position, _globalSoundVolume);
        }

        private AudioClip GetClip(MusicEnum musicEnum)
        {
            if (_dataBase.musicCipDic.ContainsKey(musicEnum.ToString()))
            {
                return _dataBase.musicCipDic[musicEnum.ToString()];
            }
            else
            {
                Debug.LogError($"No music for key {musicEnum}");
                return null;
            }
        }

        private AudioClip GetClip(SoundEnum soundEnum)
        {
            if (_dataBase.soundClipDic.ContainsKey(soundEnum.ToString()))
            {
                return _dataBase.soundClipDic[soundEnum.ToString()];
            }
            else
            {
                Debug.LogError("No sound for " + soundEnum.ToString());
                return null;
            }
        }

        private async UniTask LoadDataBaseIfNeed()
        {
            if (_dataBase == null && _loadingDatabase == false)
            {
                _loadingDatabase = true;
                _dataBase = await SmartAddressable.LoadAsync<SoundDatabase>("SoundDatabase");
                _loadingDatabase = false;
            }
        }
    }
}