using UnityEngine;
using System.Collections;


namespace lLCroweTool
{
    /// <summary>
    /// 스킬오브젝트
    /// 포격 스킬. 해당위치 포격을 떨굼
    /// 사용하는 스킬카테고리는 ActiveSkill_Other_Pos        
    /// </summary>
    public class Skill_ArtilleryFire : Skill_Base
    {
        //포격범위 구한 뒤 작동에 필요한것들


        public int shellCount = 15;//포격횟수
        public float securingSpaceSize = 1;//안전을 위한 공간확보 크기
        public float intervalTimer = 0.1f;//포격간의 떨어지는 시간

        //포격물체
        //public AttackObjectScript artilleryWeaponData;

        public override void InitSkill()
        {
            //아무행동안함
        }

        public override void ActionSkillCast()
        {
            //아무행동안함
        }

        public override void CancelSkillCast()
        {
            //아무행동안함
        }

        public override void ActionSkill()
        {
            //포격떨구는 쪽
            Vector3 originPos = transform.position;
            Vector2 pos;

            //랜덤으로 위치 가져와서 세팅하기
            for (int i = 0; i < shellCount; i++)
            {
                int index = i;
                do
                {
                    pos = lLcroweUtil.GetRandomCirclePosition(originPos, skillAreaSize, true);
                    //해당범위보다 크면 좌표집어넣기
                    if (!lLcroweUtil.CheckDistance(originPos, pos, securingSpaceSize))
                    {
                        break;
                    }

                } while (true);

                StartCoroutine(ActionDropArtillery(intervalTimer * index, pos));
            }
        }

        public override void ResetSkill()
        {

        }

        public override void SetPosition(Vector2 _pos)
        {
            transform.position = _pos;
        }

        private IEnumerator ActionDropArtillery(float timer, Vector2 pos)
        {
            yield return new WaitForSeconds(timer);
            //AttackBox_Base targetObject = ObjectPoolManager.Instance.RequestAttackBox_Base(artilleryWeaponData, GetTargetSkillSlot().GetTargetWorldUnitObject());
            //targetObject.AttackAction();
            //targetObject.transform.position = pos;
        }
    }
}