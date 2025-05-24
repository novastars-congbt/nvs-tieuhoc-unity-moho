using UnityEngine;


namespace EasySpringBone
{
    public class SpringBone : MonoBehaviour
    {
        private Vector3 STIFFNESS_FACTOR = Vector3.right;

        //======================================================================
        //  Public
        //======================================================================
        [Range(0.01f, 1)]
        public float springDamping = 0.5f;

        [Range(0.01f, 2)]
        public float stiffness = 0.2f;

        public bool useConstraint = false;
        [Range(0f, 360f)]
        public float minAngle = 0;
        [Range(0f, 360f)]
        public float maxAngle = 0;

        public bool reduceStrength = false;
        [Range(0f, 1f)]
        public float strength = 0.5f;

        public bool ignoreSpringBone = false;

        //======================================================================
        //  Private
        //======================================================================
        private Transform tran;
        private Quaternion defaultLocalRotation;
        private Vector3 preTip;
        private Vector3 nowTip;

        //======================================================================
        //  Data from manager
        //======================================================================
        private IQuery query;
        private float boneLength;


        public void init(IQuery _query, float _boneLength)
        {
            this.query = _query;
            this.boneLength = _boneLength;

            this.tran = this.transform;
            this.fixLocalRotation();
            this.defaultLocalRotation = this.tran.localRotation;

            Vector3 boneTipPosi = this.getBoneTipPosi();
            this.nowTip = boneTipPosi;
            this.preTip = boneTipPosi;

            this.calcStiffnessFactor();
        }

        // Limit the rotation angle to 0 ~ 360 degrees
        private void fixLocalRotation()
        {
            float z = this.tran.localRotation.eulerAngles.z;

            while(z < 0)
                z += 360;

            z = z % 360;

            this.tran.localRotation = Quaternion.Euler(0, 0, z);
        }

        private Vector3 getBoneTipPosi()
        {
            Vector3 dir = this.tran.TransformDirection(Vector3.right);

            return this.tran.position + dir * this.boneLength;
        }

        // Cache fixed calculated value to increase performance
        private void calcStiffnessFactor()
        {
            STIFFNESS_FACTOR = this.query.getRight() * this.stiffness;
        }

        public void destroy()
        {
            this.tran = null;
            this.query = null;
        }

        public void update()
        {
            if(this.ignoreSpringBone)
            {
                Vector3 boneTipPosi = this.getBoneTipPosi();
                this.preTip = boneTipPosi;
                this.nowTip = boneTipPosi;

                return;
            }

            if(this.query.needAlwaysUpdate() || this.query.needCalcScale())
                this.calcStiffnessFactor();

            this.tran.localRotation = this.defaultLocalRotation;

            Vector3 bonePosi = this.tran.position;
            Vector3 beginDir = this.tran.TransformDirection(Vector3.right);
            Vector3 force = this.calcForce();
            Vector3 tipDir = ((this.nowTip * 2 - this.preTip) + force - bonePosi).normalized;

            this.applyRotation(Quaternion.FromToRotation(beginDir, tipDir) * this.tran.rotation);

            this.preTip = this.nowTip;
            this.nowTip = bonePosi + tipDir * this.boneLength;

            if(this.useConstraint)
            {
                Vector3 angle = this.tran.localRotation.eulerAngles;

                if(this.outOfConstraintRange(angle.z))
                {
                    float distanceToMin = Mathf.Abs(angle.z - this.minAngle);
                    distanceToMin = (distanceToMin > 180) ? 360 - distanceToMin : distanceToMin;
                    float distanceToMax = Mathf.Abs(angle.z - this.maxAngle);
                    distanceToMax = (distanceToMax > 180) ? 360 - distanceToMax : distanceToMax;

                    angle.z = (distanceToMin < distanceToMax) ? this.minAngle : this.maxAngle;

                    this.tran.localRotation = Quaternion.Euler(angle);

                    Vector3 boneTipPosi = this.getBoneTipPosi();
                    this.preTip = this.nowTip;
                    this.nowTip = boneTipPosi;
                }
            }
        }

        private Vector3 calcForce()
        {
            Vector3 force = this.tran.rotation * STIFFNESS_FACTOR;

            force += (this.preTip - this.nowTip) * this.springDamping;

            if(this.query.hasExtraForce())
                force += this.query.getExtraForce();

            return force;
        }

        private void applyRotation(Quaternion toRotation)
        {
            if(this.reduceStrength)
                toRotation = Quaternion.Lerp(this.tran.rotation, toRotation, this.strength);

            this.tran.rotation = toRotation;
        }

        private bool outOfConstraintRange(float angle)
        {
            angle = (angle + 360) % 360;

            if(this.angleCrossZero())
            {
                float tempMinAngle = this.minAngle - this.maxAngle;
                float newAngle = (angle - this.maxAngle + 360) % 360;

                return (newAngle < tempMinAngle || newAngle > 360);
            }
            else
                return (angle < this.minAngle || angle > this.maxAngle);
        }

        private bool angleCrossZero()
        {
            return (this.minAngle > this.maxAngle);
        }
    }
}
