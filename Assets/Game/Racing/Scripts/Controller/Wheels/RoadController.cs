using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.Mathematics;
namespace Novastars.MiniGame.DuaXe
{

    public class RoadController : SingletonBehaviour<RoadController>
    {
    
        private int c, num;
        private float y, off;
        public readonly Transform car;
        public GameObject spawn;
        public GameObject[] wheel;
        private float[] x;
        public void Start(){
            x = new float[5];
        }
        public void WheelRoad(Image car){
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
                case "Racing_Car01":
                        c = 0;
                        num = 2;
                        off = 76.5f;
                        y = -42;
                        x[0]=-108;
                        x[1]=91.5f;
                        break;
                case "Racing_Car02":
                        c = 1;
                        off = 86.5f;
                        num = 2;
                        y = -41;
                        x[0]=-97;
                        x[1]=86;                    
                        break;
                case "Racing_Car03":
                        c = 2;
                        off = 67.5f;
                        num = 2;
                        y = -45;
                        x[0]=-101;
                        x[1]=105;                       
                        break;
                case "Racing_Car04":
                        c = 3;
                        off = 53.5f;
                        num = 2;
                        y = -35;
                        x[0]=-87;
                        x[1]=103;
                        break;
                case "Racing_Car05":
                        c = 4;
                        off = 47.5f;
                        num = 2;
                        y = -16;
                        x[0]=-122.5f;
                        x[1]=101;                     
                        break;
                default:
                        break;
                }
            }
    }
}