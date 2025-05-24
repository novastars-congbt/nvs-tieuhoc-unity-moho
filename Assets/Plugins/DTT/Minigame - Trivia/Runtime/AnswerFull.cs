using System;
using UnityEngine;

namespace DTT.Trivia
{
    [Serializable]
    public class AnswerFull : Answer
    {
        [SerializeField]
        private Sprite _spr;

        [SerializeField]
        public Sprite Spr => _spr;

        [SerializeField]
        private AudioClip _audio;

        [SerializeField]
        public AudioClip Audio => _audio;
    }
}
