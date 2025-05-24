using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour that is executed in editor and playmode to orbit children with a given
    /// offset around the game object this component is attached to.
    /// </summary>
    [ExecuteAlways]
    public class OrbitChildren : MonoBehaviour
    {
        /// <summary>
        /// The offset distance from the parent to the children.
        /// </summary>
        [SerializeField]
        private float _offset = 1f;
        
        /// <summary>
        /// The children of the game object this component is attached to.
        /// </summary>
        private Transform[] _Children
        {
            get
            {
                Transform[] children = new Transform[transform.childCount];
                for (int i = 0; i < children.Length; i++)
                    children[i] = transform.GetChild(i);
                return children;
            }
        }

        /// <summary>
        /// The last count of children checked in the update execution.
        /// </summary>
        private int _lastChildCount;

        /// <summary>
        /// Sets orbit rotations if the amount of children has changed. 
        /// </summary>
        private void Update()
        {
            //if(_lastChildCount != transform.childCount)
            //    SetOrbitRotations();

            //_lastChildCount = transform.childCount;
            //SetOrbitRotations();
        }

        /// <summary>
        /// Sets the orbit rotations of children of this component's game object.
        /// </summary>
        public void SetOrbitRotations()
        {
            const float THREE_SIXTY = 360f;
            const float PETAL_ROTATION = 90f;
            float anglePerChild = THREE_SIXTY / transform.childCount;
            Debug.LogError("==================== angle = " + anglePerChild);
            Transform[] children = _Children;
            Vector3 offset = new Vector3(0f, _offset, 0f);
            Vector3 zAxis = new Vector3(0f, 0f, 1f);
            for(int i = 0; i < children.Length; i++)
            {
                Transform child = children[i];
                child.localPosition = Quaternion.AngleAxis(anglePerChild * i, zAxis) * offset;
                child.localEulerAngles = new Vector3(0f, 0f, PETAL_ROTATION + anglePerChild * i);
            }
        }

        public void InvokeSetOrbitRotations()
        {
            CancelInvoke("SetOrbitRotations");
            Invoke("SetOrbitRotations", 0.1f);
        }
    }
}
