using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Novastars.MiniGame.TimDiemKhacBiet {
    public class Picture : MonoBehaviour
    {
        public List<DiffernceObject> differnceObjects = new List<DiffernceObject>();

        public void SetListDifferenceObject()
        {
            differnceObjects.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                differnceObjects.Add(transform.GetChild(i).GetComponent<DiffernceObject>());
                differnceObjects[i].id = i;
            }
        }

    }
}
