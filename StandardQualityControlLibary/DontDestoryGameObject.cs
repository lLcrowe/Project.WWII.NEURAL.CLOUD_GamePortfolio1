using System.Collections;
using UnityEngine;

namespace lLCroweTool.QC.DontDestroy
{
    public class DontDestoryGameObject : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Destroy(this);
        }
    }
}