using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTT.Hangman
{
    public class SFXController : SingletonBehaviour<SFXController>
    {
        [Header("Cài đặt hiệu ứng")]
        [SerializeField]private Transform _spawn;
        [SerializeField]private Vector2 mouse;

        [Header("Hiệu ứng nền")]
        public AudioClip BackgroundMusic;

        [Header("Hiệu ứng hoàn thành màn chơi")]
        public AudioClip VictorySFX;
        public GameObject VictoryVFX;

        [Header("Hiệu ứng trả lời câu hỏi")]
        public AudioClip TrueSFX;
        public GameObject TrueVFX;
        public AudioClip FalseSFX;
        public GameObject FalseVFX;

        [Header("Hiệu ứng nút bắt đầu và tuỳ chỉnh")]
        public AudioClip ClickSFX;
        public GameObject ClickVFX;

        [Header("Vị trí âm thanh hiệu ứng")]
        [SerializeField] private AudioSource _audioSourse;
        [SerializeField] private AudioSource _audioSourseBGM;

        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        private void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            mouse = Camera.main.ScreenToWorldPoint(mousePos);
            if (Input.GetMouseButtonDown(0)) Effect("click");
        }
        #endregion

        #region Public Method
        public void PlayBackgroundMusic()
        {
            if (BackgroundMusic)
            {
                _audioSourseBGM.clip = BackgroundMusic;
                _audioSourseBGM.Play();
            }
        }
        public void Effect(string fx){
            switch(fx){
                case "true": PlayVFX(TrueVFX, TrueSFX);
                    break;
                case "false": PlayVFX(FalseVFX, FalseSFX);
                    break;
                case "victory": PlayVFX(VictoryVFX, VictorySFX);
                    break;
                case "click": PlayVFX(ClickVFX, ClickSFX);
                    break;                 
                default:
                    break;
            }
        }
        public GameObject PlayVFX(GameObject vfx, AudioClip sfx){
            _spawn.position = new Vector2(mouse.x,mouse.y);
            var fx = vfx ? Instantiate(vfx, _spawn) : null;
            PlaySFX(sfx);
            if (fx){
                if (fx.GetComponentInChildren<ParticleSystem>().main.loop) Destroy(fx, 10);
                else Destroy(fx, 1.5f);
            }
            return fx;
        }

        public void PlaySFX(AudioClip sfx){
            if (sfx) _audioSourse.PlayOneShot(sfx);
        }

        public void StopSound()
        {
            _audioSourse.Stop();
            _audioSourseBGM.Stop();
        }

        public void DestroyAll()
        {
            foreach (var child in _spawn)
            {
                Destroy((child as Transform).gameObject);
            }
        }
        #endregion
    }
}