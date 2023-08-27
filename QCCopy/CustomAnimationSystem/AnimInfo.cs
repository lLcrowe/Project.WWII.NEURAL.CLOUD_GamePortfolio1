using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace lLCroweTool.Anim
{
    public class AnimInfo : LabelBase
    {
        //애니멘서를 이용해서 애님 컨트롤러를 조절할 대상
        //여기서 애니멘서의 특정데이터와 애니메이션클립을 정의해서 작업

        //데이터를 더보자
        // ClipTransition 
        //https://kybernetik.com.au/animancer/docs/examples/basics/transitions/
        public AnimationClip[] animationClipArray = new AnimationClip[0];

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.AnimData;
        }
    }
}
