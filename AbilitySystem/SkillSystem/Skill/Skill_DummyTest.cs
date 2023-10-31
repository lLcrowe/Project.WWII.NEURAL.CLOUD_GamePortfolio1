using lLCroweTool.DestroyManger;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class Skill_DummyTest : Skill_Base
    {
        //스킬매니저 작동을 보기위한 더미테스트용 스킬 프리팹
        //사용하는 스킬카테고리 : 모든카테고리

        public GameObject targetSpriteObject;
        public override void ActionSkill()
        {
            Debug.Log("더미스킬 작동");
        }

        public override void ActionSkillCast()
        {
            Debug.Log("캐스팅 시작");
        }

        public override void CancelSkillCast()
        {
            Debug.Log("캐스팅 취소");
        }

        public override void InitSkill()
        {
            
        }

        public override void ResetSkill()
        {
            DestroyManager.Instance.AddDestoryGameObject(targetSpriteObject);
        }

        public override void SetPosition(Vector2 _pos)
        {
            targetSpriteObject.transform.parent = null;
            targetSpriteObject.transform.position = _pos;
        }

        
    }
}