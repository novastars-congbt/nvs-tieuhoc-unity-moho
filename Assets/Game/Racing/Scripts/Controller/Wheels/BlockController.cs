using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.Mathematics;
namespace Novastars.MiniGame.DuaXe
{

    public class BlockController : SingletonBehaviour<BlockController>
    {
        private int c, num;
        private float y;
        private float off;
        public readonly Transform car;
        public GameObject spawn;
        public GameObject[] wheel;
        private float[] x;
        public void Start(){
            x = new float[5];
        }
        public void WheelBlock(Image car){
            foreach (Transform child in car.transform)
            {
                Destroy(child.gameObject);
            }
            WheelPicker(car.sprite.name);
            for(int i = 0;i < num;i++){
                var wheels = Instantiate(wheel[c], car.transform.position, Quaternion.identity, transform);
                wheels.transform.SetParent(car.transform, true);
                wheels.transform.localPosition = new Vector2(x[i], y+off);
                wheels.transform.localScale = Vector2.one;
                wheels.transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
            }
        }
        public void WheelPicker(string name){
            switch(name){
                case "Racing_Obst01":
                        c = 0;
                        num = 5;
                        y = -94;
                        off = 119;
                        x[0]=194;
                        x[1]=83.5f;
                        x[2]=28.5f;
                        x[3]=-179;
                        x[4]=-234;
                        break;
                case "Racing_Obst02":
                        c = 1;
                        num = 2;
                        y = -84;
                        off = 125.5f;
                        x[0]=125;
                        x[1]=-161;
                        break;
                case "Racing_Obst03":
                        c = 2;
                        num = 5;
                        y = -95.5f;
                        off = 122.5f;
                        x[0]=375.5f;
                        x[1]=149.5f;
                        x[2]=-256;
                        x[3]=-311;
                        x[4]=-367;
                        break;
                default:
                        break;
                }
            }
    }
}