using DTT.MinigameBase.Editor.Advertisements.AdMob;
using DTT.OrderingWords.Demo;
using DTT.PublishingTools;
using UnityEditor;
using UnityEngine;

namespace DTT.OrderingWords.Editor
{
    [CustomEditor(typeof(OrderingWordsAdHandler))]
    [DTTHeader("dtt.ordering-words", "Ad Handler")]
    public class OrderingWordsAdHandlerEditor : MinigameAdHandlerEditor<GameSettings, OrderingWordsResult, GameManager>
    {
        
    }
}