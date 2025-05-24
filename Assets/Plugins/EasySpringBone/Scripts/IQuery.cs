using UnityEngine;


namespace EasySpringBone
{
    public interface IQuery
    {
        bool hasExtraForce();
        Vector3 getExtraForce();
        bool needAlwaysUpdate();
        bool needCalcScale();
        Vector3 getRight();
    }
}