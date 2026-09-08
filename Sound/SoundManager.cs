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

        public SoundManager()
        {
            LoadDataBaseIfNeed().Forget();
        }

        public void SetupSound()
        {
            GameObject soundObject = new GameObject("SoundManager");
            _mainMusicSource = soundObject.AddComponent<AudioSource>();
            _backupMusicSource = soundObject.AddComponent<AudioSource>();
            _audioListener = GameObject.FindAnyObjectByType<AudioListener>();
        }

        public void ChangeMusic(MusicEnum musicToChange, float duration = 0.5f)
        {
            _backupMusicSource.clip = GetClip(musicToChange);
            float backupVolume = 0;
            DOTween.To(() => backupVolume, t =>
            {
                backupVolume = t;
                _backupMusicSource.volume = backupVolume;
                _mainMusicSource.volume = 1 - backupVolume;
            }, 1, duration).OnComplete(() =>
            {
                _mainMusicSource.clip = GetClip(musicToChange);
                _mainMusicSource.volume = 1;
                _mainMusicSource.time = duration;
                _mainMusicSource.Play();
                _backupMusicSource.Stop();
                _backupMusicSource.clip = null;
            });

        }

        public void PlayMusic(MusicEnum musicEnum)
        {
            _mainMusicSource.clip = GetClip(musicEnum);
            _mainMusicSource.volume = 1;
            _mainMusicSource.loop = true;
            _mainMusicSource.Play();
        }

        public void PlaySound(SoundListEnum soundListEnum)
        {
            var clip = _dataBase.soundListDic[soundListEnum.ToString()].GetRandom();
            Debug.Log("clip: " + clip.name);
            AudioSource.PlayClipAtPoint(clip, _audioListener.transform.position);
        }

        public void PlaySound(SoundEnum soundEnum)
        {
            AudioSource.PlayClipAtPoint(GetClip(soundEnum), _audioListener.transform.position);
        }

        private AudioClip GetClip(MusicEnum musicEnum)
        {
            if(_dataBase.musicCipDic.ContainsKey(musicEnum.ToString()))
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
            if(_dataBase == null)
            {
                _dataBase = await SmartAddressable.LoadAsync<SoundDatabase>("SoundDatabase");
            }
        }
    }
}