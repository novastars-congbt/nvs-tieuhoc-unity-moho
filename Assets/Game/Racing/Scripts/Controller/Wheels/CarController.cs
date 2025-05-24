using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.Mathematics;
namespace Novastars.MiniGame.DuaXe
{

    public class CarController : SingletonBehaviour<CarController>
    {
    
        private int c, num;
        private float y, off;
        public readonly Transform car;
        public GameObject[] wheel;
        private float[] x;
        public void Start(){
            x = new float[5];
        }
        IEnumerator StartWheel(GameObject wheel, float timer){
            yield return new WaitForSeconds(timer);
            wheel.transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
            
        }
        public void WheelCar(Image car){
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
                StartCoroutine(StartWheel(wheels, 3f));
            }
        }
        public void WheelPicker(string name){
            switch(name){
                case "Racing_Car01":
                        c = 0;
                        num = 2;
                        y = -42;
                        off = 77;
                        x[0]=-108;
                        x[1]=91.5f;
                        break;
                case "Racing_Car02":
                        c = 1;
                        num = 2;
                        y = -41;
                        off = 86.5f;
                        x[0]=-97;
                        x[1]=86;                    
                        break;
                case "Racing_Car03":
                        c = 2;
                        num = 2;
                        y = -45;
                        off = 68;
                        x[0]=-101;
                        x[1]=105;                       
                        break;
                case "Racing_Car04":
                        c = 3;
                        num = 2;
                        y = -35;
                        off = 53.5f;
                        x[0]=-87;
                        x[1]=103;
                        break;
                case "Racing_Car05":
                        c = 4;
                        num = 2;
                        y = -16;
                        off = 47.5f;
                        x[0]=-122.5f;
                        x[1]=101;                     
                        break;
                case "Racing_Obst01":
                        c = 5;
                        num = 5;
                        y = -94;
                        off = 77;
                        x[0]=194;
                        x[1]=83.5f;
                        x[2]=28.5f;
                        x[3]=-179;
                        x[4]=-234;
                        break;
                case "Racing_Obst02":
                        c = 6;
                        num = 2;
                        y = -84;
                        off = 77;
                        x[0]=125;
                        x[1]=-161;
                        break;
                case "Racing_Obst03":
                        c = 7;
                        num = 5;
                        y = -95.5f;
                        off = 77;
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