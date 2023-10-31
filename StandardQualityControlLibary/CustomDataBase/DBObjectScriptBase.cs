
using UnityEngine;

namespace lLCroweTool
{
    public abstract class DBObjectScriptBase : ScriptableObject
    {
        //데이터베이스

        /// <summary>
        /// DB 버전
        /// </summary>
        public string version;//적어두는법은 릴리즈노트처럼

        /// <summary>
        /// DB데이터를 적어둔 오너들
        /// </summary>
        public string author;

        /// <summary>
        /// DB 설명
        /// </summary>
        public string description;
    }
}