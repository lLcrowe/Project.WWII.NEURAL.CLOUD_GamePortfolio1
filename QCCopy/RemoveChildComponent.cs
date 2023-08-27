using System;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    public class RemoveChildComponent : MonoBehaviour
    {
        //신디크회사꺼 모델링에 있는 특정 컴포넌트를 일일히 지우는것보다
        //이렇게 지워서 빠르게 지울려고함
        //특정컴포넌트를 지우고 싶을시
        //해당 컴포넌트를 추가후에 지정후 삭제하기

        public Component targetComponent;

        [ButtonMethod]
        public void RemoveTargetComponentForAllChild()
        {
            if (targetComponent == null)
            {
                Debug.Log("타겟팅된 컴포넌트가 없습니다.", gameObject);
                return;
            }

            var type = targetComponent.GetType();
            RemoveComponent(type);
        }

        private void RemoveComponent(Type type)
        {
            var componentArray = gameObject.GetComponentsInChildren(type);

            print(componentArray.Length);

            for (int i = 0; i < componentArray.Length; i++)
            {
                var temp = componentArray[i];
                DestroyImmediate(temp);
            }
        }

        [ButtonMethod]
        public void DeleteThisComponent()
        {
            DestroyImmediate(this);
        }

    }

}


