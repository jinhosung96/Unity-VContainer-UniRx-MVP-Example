using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Mine.Code.Framework.Manager.Sound
{
    [Serializable]
    public class SoundClip
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField, Range(0, 1)] public float defaultVolume = 0.5f;
    }

    public class SoundManager : IStartable
    {
        #region Fields

        AudioSource audioSourceForBgm;
        List<AudioSource> audioSourcesForSfx = new();
        bool isMuteBGM;
        bool isMuteEffect;
        GameObject audioPlayer;

        #endregion

        #region Properties

        public bool IsMuteBGM
        {
            get => isMuteBGM;
            set
            {
                isMuteBGM = value;

                audioSourceForBgm.mute = isMuteBGM;
            }
        }

        public bool IsMuteEffect
        {
            get => isMuteEffect;
            set
            {
                isMuteEffect = value;

                foreach (var audioSourceForEffect in audioSourcesForSfx)
                {
                    audioSourceForEffect.mute = isMuteEffect;
                }
            }
        }

        public AudioClip CurrentBGM => audioSourceForBgm.clip;

        #endregion

        #region Entry Point

        void IStartable.Start()
        {
            audioPlayer = new GameObject("AudioPlayer");
            AddAudioSourceForBgm();
        }

        #endregion

        #region Private Methods

        void AddAudioSourceForBgm()
        {
            audioSourceForBgm = audioPlayer.AddComponent<AudioSource>();
            audioSourceForBgm.loop = true;
            audioSourceForBgm.playOnAwake = false;
        }
    
        AudioSource AddAudioSourceForSfx()
        {
            var audioSource = audioPlayer.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSourcesForSfx.Add(audioSource);
            return audioSource;
        }

        void PlaySound(AudioSource audioSource, SoundClip sound, float volume)
        {
            audioSource.clip = sound.audioClip;
            audioSource.volume = volume == 0 ? sound.defaultVolume : volume;
            audioSource.Play();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// BGM을 재생합니다.
        /// </summary>
        /// <param name="sound">재생할 사운드 정보</param>
        /// <param name="volume">볼륨(0일 시 기본 값)</param>
        public void PlayBgm(SoundClip sound, float volume = 0f)
        {
            if (CurrentBGM == sound.audioClip) return;

            audioSourceForBgm.mute = isMuteBGM;
            PlaySound(audioSourceForBgm, sound, volume);
        }

        /// <summary>
        /// 효과음을 재생합니다.
        /// </summary>
        /// <param name="sound">재생할 사운드 정보</param>
        /// <param name="volume">볼륨(0일 시 기본 값)</param>
        public void PlaySfx(SoundClip sound, float volume = 0f)
        {
            if (isMuteEffect) return;

            for (int i = 0; i < audioSourcesForSfx.Count; i++)
            {
                if (!audioSourcesForSfx[i].isPlaying)
                {
                    PlaySound(audioSourcesForSfx[i], sound, volume);
                    return;
                }
            }

            var newAudioSource = AddAudioSourceForSfx();
            PlaySound(newAudioSource, sound, volume);
        }

        #endregion
    }
}