using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LitFramework.Asset;
using UnityEngine;
namespace LitFramework.Audio
{
    public class AudioService : IAudioService
    {
        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        private float _bgmVolume = 1f;
        private float _sfxVolume = 1f;
        private Dictionary<string, AudioClip> _clipCache = new();  // 简单缓存
        private IAssetService _assetManager;
        private IAssetService AssetManager =>
        _assetManager ??= ServiceLocator.Get<IAssetService>();
        public AudioService()
        {
            // 动态创建 AudioSource 组件，不依赖场景预设
            var bgmGo = new GameObject("BGMPlayer");
            var sfxGo = new GameObject("SFXPlayer");
            _bgmSource = bgmGo.AddComponent<AudioSource>();
            _sfxSource = sfxGo.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(bgmGo);
            Object.DontDestroyOnLoad(sfxGo);

            _bgmSource.loop = true;
            _bgmSource.volume = _bgmVolume;
            _sfxSource.volume = _sfxVolume;
        }

        public void PlayBGM(string clipName, bool loop = true, float volume = 1f)
        {
            _bgmSource.loop = loop;
            _bgmSource.volume = _bgmVolume * volume;
            LoadAndPlay(clipName, _bgmSource).Forget();
        }

        public void PlaySFX(string clipName, float volume = 1f)
        {
            _sfxSource.volume = _sfxVolume * volume;
            LoadAndPlay(clipName, _sfxSource, oneShot: true).Forget();
        }

        private async UniTask LoadAndPlay(string clipName, AudioSource source, bool oneShot = false)
        {
            if (_clipCache.TryGetValue(clipName, out var clip))
            {
                PlayInternal(clip, source, oneShot);
                return;
            }
            // 通过资源服务异步加载
            clip = await AssetManager.LoadAsync<AudioClip>(GetClipPath(clipName));
            if (clip != null)
            {
                _clipCache[clipName] = clip;
                PlayInternal(clip, source, oneShot);
            }
            else
            {
                Debug.LogError($"音频加载失败: {clipName}");
            }
        }

        private void PlayInternal(AudioClip clip, AudioSource source, bool oneShot)
        {
            if (oneShot)
                source.PlayOneShot(clip);
            else
                source.clip = clip;
            source.Play();
        }

        private string GetClipPath(string clipName) => $"Audio/{clipName}";  // 可配置

        public void StopBGM() => _bgmSource.Stop();
        public void SetBGMVolume(float vol) { _bgmVolume = vol; _bgmSource.volume = _bgmVolume; }
        public void SetSFXVolume(float vol) { _sfxVolume = vol; _sfxSource.volume = _sfxVolume; }
    }
}