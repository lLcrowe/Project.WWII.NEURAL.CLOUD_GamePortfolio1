using lLCroweTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIThemaInfo : LabelBase
{
    //UI들의 테마를 정하기 위한 처리
    //여기는 이미지에 대한 내용만 담아둘예정

    //CSV처리시 네이밍을 일치화해서 처리예정


    //이름좀 생각해봐야겠음
    //background => bg => panel


    //기본구조
    //CustomImage//테두리이미지//이미지//버튼
    //  Panel//패널이미지//테두리밸류//이미지
    //-----확장구조(아이콘,텍스트)
    //      Icon
    //      텍스트

    //아이콘들, 버튼백그라운드 테두리 UI들처리
    //이것저것 레퍼런스보고 규칙찾은 다음 처리


    public class SpritePreset
    {
        //하나의 이미지는 컬러와 스프라이트로 작동
        public Color color = Color.white;
        public Sprite sprite;
    }

    public class ButtonColorPreset
    {
        public Color highLightColor;
        public Color pressedColor;
        public Color selectedColor;
        public Color disabledColor;
        public float fadeDuration;
    }

    public class ButtonSpriteSwapPreset
    {
        public Sprite highLightSprite;
        public Sprite pressedSprite;
        public Sprite selectedSprite;
        public Sprite disabledSprite;
    }

    //사용할지에 대한 Bool처리를 체크
    //CSV 임포트할때 리소시스체크하는데 거기서 null로 비어있으면 nullSprite로 체크되닌 아예 작동안하게 처리

    //뒷배경//패널
    //테두리//=> 처리방법을 띵킹
    public bool isUseBorder;
    public float borderValue;


    //아이콘
    









    //어떤이미지를 사용할것인가
    
    

    public override LabelBaseDataType GetLabelDataType()
    {
        return LabelBaseDataType.Nothing;
    }
}
