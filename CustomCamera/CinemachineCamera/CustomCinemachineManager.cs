using UnityEngine;
using lLCroweTool.Dictionary;

namespace lLCroweTool.Cinemachine
{
    [DefaultExecutionOrder(-50)]
    public class CustomCinemachineManager : MonoBehaviour
    {
        //여러 시네머신들을 가져와서 관리해주는 용도
        //호출해서 가져올수 있게함
        //필요한씬에만 배치해서 처리하기

        [System.Serializable]
        public class CustomCinemachineBible : CustomDictionary<string, CustomCinemachine> { }

        public CustomCinemachineBible customCinemachineBible = new CustomCinemachineBible();



        protected void Awake()
        {   
            CustomCinemachine[] customCinemachineArray = GetComponentsInChildren<CustomCinemachine>();

            for (int i = 0; i < customCinemachineArray.Length; i++)
            {
                CustomCinemachine temp = customCinemachineArray[i];
                if (customCinemachineBible.TryAdd(temp.cinemachineID, temp) == false)
                {
                    Debug.Log($"{i}번 {temp.name}의 시네머신ID가 중첩됩니다. 확인해주세요");
                }
            }
        }

        public bool RequestCustomCinemachine(string id, out CustomCinemachine customCinemachine)
        {
            return customCinemachineBible.TryGetValue(id, out customCinemachine);
        }
    }
}