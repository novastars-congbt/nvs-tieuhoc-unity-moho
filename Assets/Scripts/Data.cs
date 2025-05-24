using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Data", order = 1)]
public class Data : ScriptableObject
{
    public TypeClass[] typeClasses; // lớp
    public TypeMusic[] typeMusics; // nhạc

    [Serializable]
    public struct TypeClass
    {
        public string name; // tên lớp
        public TypeLesson[] typeLessons; // bài học
        public string scheme;
        
        [Serializable]
        public struct TypeLesson
        {
            public string name; // mã bài học
            public string text; // tên bài học
            public bool doneFixTimeline;
            public TypeActive[] typeActives;
            public TypeSecondaryActive[] typeSecondaryActives;

            [Serializable]
            public struct TypeSecondaryActive
            {
                public int idActive;
                public TypeActive[] typeActives;
            }

            [Serializable]
            public struct TypeActive
            {
                public string name; // mã hoạt động
                public string text; // tên họat động
                public float time; // thời gian của hoạt động
                public Sprite sprLessonPanel;
                public TypeStep[] typeSteps;
                //public Sprite sprGame; // ảnh game
                //public float[] timePause; // điểm dừng của hoạt động

                [Serializable]
                public struct TypeStep
                {
                    public string text;
                    public float time;
                    public TypeSlide[] typeSlides;
                    //public float[] timePause;
                }

                [Serializable]
                public struct TypeSlide
                {
                    //public bool isCartoon;
                    public float time;
                    public string textBtnForward;
                    public MusicPlay musicPlay;
                    public MusicWeb musicWeb;

                    [Serializable]
                    public struct MusicPlay
                    {
                        public AudioClip audioClipMusic;
                        public bool loopMusic;
                    }

                    [Serializable]
                    public struct MusicWeb
                    {
                        public string nameMusic;
                        public string urlMusic;
                        public string timeMusic;
                    }
                }
            }
        }
    }

    [Serializable]
    public struct TypeMusic
    {
        public string name;
        public float time;
        public AudioClip audio;
    }

}
