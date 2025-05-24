using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;


namespace EasySpringBone
{
    [RequireComponent(typeof(SpriteSkin))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpringBoneManager : MonoBehaviour, IQuery
    {
        public struct Sort
        {
            public int index;
            public int weight;
        }


        public bool alwaysUpdate = false;

        public bool withExtraForce = false;
        [Range(0f, 360f)]
        public float extraForceDir = 0f;
        [Range(0.001f, 1f)]
        public float forceLength = 0.03f;

        public bool useScaleToFlip = false;
        public Transform scaleObject = null;

        private SpringBone[] springBones = null;
        private Vector3 extraForce = Vector3.zero;




        private void Start()
        {
            Transform[] boneTrans = this.GetComponent<SpriteSkin>().boneTransforms;
            SpriteBone[] bones = this.GetComponent<SpriteRenderer>().sprite.GetBones();
            List<SpringBone> tempSpringBones = new List<SpringBone>();
            Sort[] sorting = new Sort[boneTrans.Length];

            for(int i = 0; i < boneTrans.Length; i++)
            {
                sorting[i].index = i;
                sorting[i].weight = this.stepsToRoot(boneTrans[i]);
            }
            Array.Sort<Sort>(sorting, new weightComparer());

            for(int i = 0; i < boneTrans.Length; i++)
            {
                int index = sorting[i].index;
                SpringBone springBone = boneTrans[index].GetComponent<SpringBone>();

                if(springBone != null)
                {
                    springBone.init(this, bones[index].length);
                    tempSpringBones.Add(springBone);
                }
            }

            this.springBones = new SpringBone[tempSpringBones.Count];
            for(int i = 0; i < this.springBones.Length; i++)
                this.springBones[i] = tempSpringBones[i];
            tempSpringBones.Clear();
            tempSpringBones = null;

            this.calcExtraForce();
        }

        private int stepsToRoot(Transform target)
        {
            int steps = 0;
            int maxSteps = 999;

            for(int i = 0; i < maxSteps; i++)
            {
                if(target == null)
                    return steps;
                if(target != null)
                    target = target.parent;
                steps += 1;
            }

            return steps;
        }

        public class weightComparer : IComparer<Sort>
        {
            public int Compare(Sort a, Sort b)
            {
                return (a.weight - b.weight);
            }
        }

        private void calcExtraForce()
        {
            this.extraForce = Quaternion.Euler(0, 0, this.extraForceDir) * (Vector3.right * this.forceLength);
        }

        private void FixedUpdate()
        {
            if(this.alwaysUpdate)
                if(this.withExtraForce)
                    this.calcExtraForce();

            for(int i = 0; i < this.springBones.Length; i++)
                this.springBones[i].update();
        }

        private void OnDestroy()
        {
            if(this.springBones == null)
                return;

            for(int i = 0; i < this.springBones.Length; i++)
            {
                this.springBones[i].destroy();
                this.springBones[i] = null;
            }
            this.springBones = null;
        }


        //======================================================================
        //  I/F IQuery
        //======================================================================
        public bool hasExtraForce()
        {
            return this.withExtraForce;
        }

        public Vector3 getExtraForce()
        {
            return this.extraForce;
        }

        public bool needAlwaysUpdate()
        {
            return this.alwaysUpdate;
        }

        public bool needCalcScale()
        {
            return this.useScaleToFlip;
        }

        public Vector3 getRight()
        {
            if(this.useScaleToFlip)
                return this.localToWorld(Vector3.right);
            else
                return Vector3.right;
        }

        private Vector3 localToWorld(Vector3 localV)
        {
            if(this.scaleObject == null)
            {
                Debug.LogError("Missing scale object. Assign scale object here if you want to use scale to flip.");
                return localV;
            }

            Matrix4x4 localToWorldMatrix = Matrix4x4.identity;

            localToWorldMatrix.SetColumn(0, this.scaleObject.right * this.scaleObject.localScale.x);
            localToWorldMatrix.SetColumn(1, this.scaleObject.up * this.scaleObject.localScale.y);
            localToWorldMatrix.SetColumn(2, this.scaleObject.forward * this.scaleObject.localScale.z);

            return localToWorldMatrix * localV;
        }
    }
}