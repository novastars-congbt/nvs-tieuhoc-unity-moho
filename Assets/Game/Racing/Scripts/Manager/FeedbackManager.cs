using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Novastars.MiniGame.DuaXe
{
    public class FeedbackManager : SingletonBehaviour<FeedbackManager>
    {
        [Header("Cài đặt hiệu ứng")]
        [SerializeField][LabelText("Vị trí tạo hiệu ứng")] private Transform _parrentSpawn;

        [Header("Hiệu ứng nền")]
        [LabelText("Nhạc nền")]public AudioClip BackgroundMusic;

        [Header("Hiệu ứng hoàn thành màn chơi")]
        public AudioClip VictorySFX;
        public GameObject VictoryVFX;

        [Header("Hiệu ứng di chuyển của xe")]
        public AudioClip StopCarSFX;
        public GameObject StopVFX;
        public AudioClip StartCarSFX;
        public GameObject StartVFX;

        [Header("Hiệu ứng trả lời câu hỏi")]
        public AudioClip TrueSFX;
        public GameObject TrueVFX;
        public AudioClip FalseSFX;
        public GameObject FalseVFX;

        [Header("Hiệu ứng nút bắt đầu và tuỳ chỉnh")]
        public AudioClip ClickButtonSFX;
        public GameObject ClickButtonVFX;

        [Header("Vị trí âm thanh hiệu ứng")]
        [SerializeField] private AudioSource _audioSourse;
        [SerializeField] private AudioSource _audioSourseBGM;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        #endregion

        #region Public Method
        public GameObject SpawnClickButtonVFX()
        {
            var vfx = ClickButtonVFX ? Instantiate(ClickButtonVFX/*, _parrentSpawn*/) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public GameObject SpawnVictoryVFX()
        {
            var vfx = VictoryVFX ? Instantiate(VictoryVFX/*, _parrentSpawn*/) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public void PlayBackgroundMusic()
        {
            if (BackgroundMusic)
            {
                _audioSourseBGM.clip = BackgroundMusic;
                _audioSourseBGM.Play();
            }
        }

        public void PlayStartCarSFX()
        {
            if (StartCarSFX) _audioSourse.PlayOneShot(StartCarSFX);
        }

        public GameObject PlayStartCarVFX(){
            var vfx = StartVFX ? Instantiate(StartVFX/*, _parrentSpawn*/) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 2.2f);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }
        
        public void PlayStopCarSFX()
        {
            if (StopCarSFX) _audioSourse.PlayOneShot(StopCarSFX);
        }
        
        public GameObject PlayStopCarVFX(){
            var vfx = StopVFX ? Instantiate(StopVFX/*, _parrentSpawn*/) : null;
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 3);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public void PlayTrueSFX()
        {
            if (TrueSFX) _audioSourse.PlayOneShot(TrueSFX);
        }

        public GameObject PlayTrueVFX(){
            var vfx = TrueVFX ? Instantiate(TrueVFX/*, _parrentSpawn*/) : null;
            PlayTrueSFX();
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }
        
        public void PlayFalseSFX()
        {
            if (FalseSFX) _audioSourse.PlayOneShot(FalseSFX);
        }
        
        public GameObject PlayFalseVFX(){
            var vfx = FalseVFX ? Instantiate(FalseVFX/*, _parrentSpawn*/) : null;
            PlayFalseSFX();
            if (vfx)
            {
                if (vfx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(vfx, 10);
                else Destroy(vfx, 1.5f);
            }

            return vfx;
        }

        public void PlayButtonClickSFX()
        {
            if (ClickButtonSFX) _audioSourse.PlayOneShot(ClickButtonSFX);
        }

        public void PlayVictorySFX()
        {
            if (VictorySFX) _audioSourse.PlayOneShot(VictorySFX);
        }

        public void StopSound()
        {
            _audioSourse.Stop();
            _audioSourseBGM.Stop();
        }

        public void DestroyAll()
        {
            foreach (var child in _parrentSpawn)
            {
                Destroy((child as Transform).gameObject);
            }
        }
        #endregion
    }
}