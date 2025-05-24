using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Novastars.MiniGame.LatBai
{
    [Serializable]
    public class MissionData
    {
        [TextArea] public string MissionQuestion;
        [TextArea] public string MissionAnswer;
    }

    [Serializable]
    public class CardData
    {
        public Sprite CardSprite;
        public string CardLabel;
    }

    public class DataManager : SingletonBehaviour<DataManager>
    {
        [LabelText("Data")]
        public DataCard dataCard;
        [Space]
        [Header("Các câu hỏi thử thách")]
        [LabelText("Nội dung câu hỏi và đáp án")] public List<MissionData> MissionDatas = new List<MissionData>();

        [Space]
        [Header("Cài đặt hướng dẫn"), TextArea]
        [LabelText("Hướng dẫn chung")] public string GuideString;

        [Space]
        [LabelText("Hướng dẫn bằng chữ")] public bool IsUseText;
        [LabelText("Hướng dẫn bằng ảnh")] public bool IsUseSprite;
        [LabelText("Hướng dẫn bằng clip")] public bool IsUseClip;
        [Space]
        [Header("Nội dung hướng dẫn chính"), TextArea]
        [LabelText("Dòng chữ hướng dẫn")] public string GuidePopupString;
        [LabelText("Ảnh hướng dẫn")] public Sprite GuidePopupSprite;
        [LabelText("Clip hướng dẫn")] public VideoClip GuidePopupClip;
        
        #region Unity Funcion
        private new void Awake()
        {
            base.Awake();
        }
        #endregion
    }

#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(CardData))]
    public class CardDataDrawer : PropertyDrawer
    {
        const float imageHeight = 30;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var cardSpriteProp = property.FindPropertyRelative("CardSprite");

            if (cardSpriteProp.propertyType == SerializedPropertyType.ObjectReference && (cardSpriteProp.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(cardSpriteProp, label, true) + imageHeight + 5;
            }

            return EditorGUI.GetPropertyHeight(cardSpriteProp, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty cardSpriteProp = property.FindPropertyRelative("CardSprite");
            SerializedProperty cardLabelProp = property.FindPropertyRelative("CardLabel");

            //OnlyWork With Type GameObject
            //if (cardSpriteProp.objectReferenceValue is GameObject go && go != null)
            //{
            //    label = new GUIContent(go.name + " / " + cardLabelProp.floatValue);
            //}

            Rect pos = EditorGUI.PrefixLabel(position, label);
            int imgPreviewWidth = 50;
            int imgPreviewHeight = 50;
            float spacing = 5;
            float leftOverWidthRatio = imgPreviewWidth / pos.width;

            Rect p1 = pos;
            Rect p2 = new Rect(pos.x, pos.y, pos.width * (1 - leftOverWidthRatio) / 2 - 5, pos.height);
            Rect p3 = new Rect(pos.x, pos.y, pos.width * (1 - leftOverWidthRatio) / 2 - 5, pos.height);

            p2.x += imgPreviewWidth + spacing * 1;
            p3.x += imgPreviewWidth + p2.width + spacing * 2;

            if (cardSpriteProp.objectReferenceValue as Sprite)
            {
                Texture texturePreview = (cardSpriteProp.objectReferenceValue as Sprite).texture;
                EditorGUI.DrawPreviewTexture(new Rect(p1.x, p1.y, imgPreviewWidth, imgPreviewHeight), texturePreview, null, scaleMode: ScaleMode.StretchToFill);
            }

            EditorGUI.PropertyField(p2, cardSpriteProp, GUIContent.none);
            EditorGUI.PropertyField(p3, cardLabelProp, GUIContent.none);
        }
    }

    //[CustomPropertyDrawer(typeof(MissionData))]
    public class MissionDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty missionQ = property.FindPropertyRelative("MissionQuestion");
            SerializedProperty missionA = property.FindPropertyRelative("MissionAnswer");

            Rect pos = EditorGUI.PrefixLabel(position, new GUIContent(""));
            float spacing = 5;

            pos.width *= 0.5f;
            Rect p1 = pos;
            Rect p2 = pos;
            p2.x += pos.width + spacing * 1;


            EditorGUI.PropertyField(p1, missionQ, GUIContent.none);
            EditorGUI.PropertyField(p2, missionA, GUIContent.none);
        }
    }
#endif
}