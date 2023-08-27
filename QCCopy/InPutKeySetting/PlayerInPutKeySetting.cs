using lLCroweTool.Dictionary;
using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;

namespace lLCroweTool
{
    public class PlayerInPutKeySetting : MonoBehaviourSingleton<PlayerInPutKeySetting>
    {
        [System.Serializable] public class KeyBible : CustomDictionary<ECustomKeyCode, KeyCode> { }

        /// <summary>
        /// 기본용 키값
        /// </summary>
        public KeyBible NormalKeyBible { get; private set; }

        /// <summary>
        /// 두번째 키값
        /// </summary>
        public KeyBible SecondaryKeyBible { get; private set; }

        
        protected override void Awake()
        {
            base.Awake();
            //초기화
            InitPlayerInPutKeySetting();            
        }

        /// <summary>
        /// 인풋키코드 초기화
        /// </summary>
        private void InitPlayerInPutKeySetting()
        {
            //--------------------
            //주석설명 관련 내용
            //해당되는 타겟 상태이름
            //상태 설명
            //공통 or 변환
            //추가작업 여부
            //해당되는 모드

            //공통 => 모드에 따라 변경이 없는 상태
            //공통아님 => 모드에 따라 변경이 있는 상태
            //모드변경 => 모드를 변경해주는 상태
            //모드는 3가지 일반, 빌드, RTS 상태
            //일반 상태 키세팅
            //추가작업 세팅        
            //--------------------
            //추가작업시 좀더 세세히 분류해서 주석처리
            //일반 : 기본값으로 들어갈 키들
            //두번쨰 : 기본값에 더하여 추가적으로 인식하기 위한 키들
            //--------------------

            //기본키 사전레이어
            //총키 갯수 : 
            NormalKeyBible = new KeyBible();
            SecondaryKeyBible = new KeyBible();

            //이동 키//캐릭터의 이동을 담당함
            //공통//일반 빌드 RTS 모드에 사용됨
            //추가작업 없음   
            NormalKeyBible.Add(ECustomKeyCode.UpKey, KeyCode.W);
            NormalKeyBible.Add(ECustomKeyCode.DownKey, KeyCode.S);
            NormalKeyBible.Add(ECustomKeyCode.LeftKey, KeyCode.A);
            NormalKeyBible.Add(ECustomKeyCode.RightKey, KeyCode.D);

            SecondaryKeyBible.Add(ECustomKeyCode.UpKey, KeyCode.UpArrow);
            SecondaryKeyBible.Add(ECustomKeyCode.DownKey, KeyCode.DownArrow);
            SecondaryKeyBible.Add(ECustomKeyCode.LeftKey, KeyCode.LeftArrow);
            SecondaryKeyBible.Add(ECustomKeyCode.RightKey, KeyCode.RightArrow);

            //마우스선택
            NormalKeyBible.Add(ECustomKeyCode.MouseLeftButton, KeyCode.Mouse0);
            SecondaryKeyBible.Add(ECustomKeyCode.MouseLeftButton, KeyCode.None);

            //스킬사용
            NormalKeyBible.Add(ECustomKeyCode.SkillSet1, KeyCode.Alpha1);
            NormalKeyBible.Add(ECustomKeyCode.SkillSet2, KeyCode.Alpha2);
            NormalKeyBible.Add(ECustomKeyCode.SkillSet3, KeyCode.Alpha3);
            SecondaryKeyBible.Add(ECustomKeyCode.SkillSet1, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SkillSet2, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SkillSet3, KeyCode.None);

            //유닛 선택관련            
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit1, KeyCode.Alpha1);
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit2, KeyCode.Alpha2);
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit3, KeyCode.Alpha3);
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit4, KeyCode.Alpha4);
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit5, KeyCode.Alpha5);
            NormalKeyBible.Add(ECustomKeyCode.SelectUnit6, KeyCode.Alpha6);

            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit1, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit2, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit3, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit4, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit5, KeyCode.None);
            SecondaryKeyBible.Add(ECustomKeyCode.SelectUnit6, KeyCode.None);

            //추가작업예상됨

            //UI관련 키
            //스킬트리 키//스킬트리를 여는 기능
            //공통아님//일반에서 사용됨
            //추가작업없음         
            NormalKeyBible.Add(ECustomKeyCode.MenuKey, KeyCode.Escape);
        }
    }
}

//키를 지정해준것
public enum ECustomKeyCode
{
    //이동키
    UpKey,//위
    DownKey,//아래
    LeftKey,//좌
    RightKey,//우
  
    //회전
    LeftRotate,
    RightRotate,

    MouseLeftButton,//마우스좌클릭 

    SkillSet1,//1스킬키 Z
    SkillSet2,//2스킬키 X
    SkillSet3,//3스킬키 C

    SelectUnit1,//유닛선택키
    SelectUnit2,
    SelectUnit3,
    SelectUnit4,
    SelectUnit5,
    SelectUnit6,

    MenuKey,//메뉴&탈출 ESC
}