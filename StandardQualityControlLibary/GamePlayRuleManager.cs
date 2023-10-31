using lLCroweTool.Achievement;
using lLCroweTool.Dictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.GamePlayRuleSystem
{
    /// <summary>
    /// 게임플레이 규칙관리자에서 게임규칙관리할 대상
    /// AI유닛에서 요청할 사항
    /// </summary>
    public interface IGameRuleManagingTarget
    {
        /// <summary>
        ///트랜스폼을 가져오는 함수(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>트랜스폼</returns>
        public Transform GetTr();


        /// <summary>1
        /// 팀타입을 가져오는 함수(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>팀타입</returns>
        public TeamType GetTeamType();

        /// <summary>
        /// 팀규칙을 가져오는 함수(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>팀규칙데이터</returns>
        public bool GetGameTeamRule(out TeamRuleData teamRuleData);


        /// <summary>
        /// 팀역할 가져오는 함수(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>팀역할</returns>
        public TeamRole GetTeamRole();

        /// <summary>
        /// 팀역할 우선순위를 가져오는 함수(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>팀역할 우선순위</returns>
        public bool GetTeamRolePriorityBible(out TeamRolePriorityBible teamRolePriorityBible);

        /// <summary>
        /// 타겟이 살아있는지 여부(IGameRuleManagingTarget)
        /// </summary>
        /// <returns>살아있는지 여부</returns>
        public bool IsAlive();
    }


    /// <summary>
    /// 팀타입//원하는 팀을 정의
    /// </summary>
    public enum TeamType
    {
        Player,//플레이어
        Ally,//아군
        Enemy,//적군
        Neutrality,//중립
    }

    /// <summary>
    /// 상대팀을 찾을때 필요한 게임룰 데이터
    /// </summary>
    [System.Serializable]
    public class TeamRuleData
    {
        //적팀을 먼저 찾고 필요시 중립팀을 찾음
        public TeamType[] enemyTeamArray = new TeamType[0];//적팀
        public bool isFindNeutralityTeam = false;
        public TeamType[] neutralityTeamArray = new TeamType[0];//중립팀

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="gameRuleManagingTarget"></param>
        /// <returns></returns>
        public bool CheckIsOtherTeam(IGameRuleManagingTarget gameRuleManagingTarget)
        {
            var detectTeam = gameRuleManagingTarget.GetTeamType();
            //적팀확인
            for (int i = 0; i < enemyTeamArray.Length; i++)
            {
                if (enemyTeamArray[i] == detectTeam)
                {
                    return true;
                }
            }

            //중립팀확인
            if (isFindNeutralityTeam)
            {
                for (int i = 0; i < neutralityTeamArray.Length; i++)
                {
                    if (neutralityTeamArray[i] == detectTeam)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

    /// <summary>
    /// 팀에서의 역할(포지션)
    /// </summary>
    public enum TeamRole
    {
        Rifle,
        Tank,
        Wall,
    }

    /// <summary>
    /// 팀역할 우선순위를 위한 사전//팀역할, 우선순위
    /// </summary>
    [System.Serializable]
    public class TeamRolePriorityBible : CustomDictionary<TeamRole, int>{}

    /// <summary>
    /// 게임플레이 규칙관리자
    /// </summary>
    [System.Serializable]
    public class GamePlayRuleManager
    {
        //게임을 진행할시 
        //기본적인//승리 패배 진행중인걸 체크하기 위한 기능이다.
        //생존게임을 예를 들어 분해해보면//인원이 있고 인원들에 대한 체크를 한후
        //인원들이 죽었는지 살았는지에 대해 체크후 승패를 체크 등
        //특정 룰을 지정해서 승패를 체크한다

        //뉴럴클라우드에서의 룰은 팀원이 다쓰러지면 지는것
        //적군을 다쓰러뜨리면 이기는것

        //모든걸 업데이트하는건 미친짓이긴한데
        //어차피 성능좋잖아

        //흠 그래도 그냥 트리거 형식이 맞아보임
        //간단히 만들자

        //확장성있게 짜야됨.
        //여기서 제한을 주는걸 주면 밑부분에서 확장성있게 작업을 못함

        /// <summary>
        /// 게임진행상태
        /// </summary>
        public enum GamePlayState
        {
            PreMatch,//경기전
            Match,//경기중
            Victory,//승리
            Defeat,//패배
        }

        /// <summary>
        /// 일반딕셔너리를 사용함. 저장할필요가 없음//더빨리검색하고 없애기위해
        /// </summary>
        public class TeamCheckBible : Dictionary<TeamType, Dictionary<IGameRuleManagingTarget, bool>>{}
        public TeamCheckBible teamCheckBible = new TeamCheckBible();

        /// <summary>
        /// 게임룰에 대한 설정
        /// </summary>
        [System.Serializable]
        public class GameRuleData
        {
            public TeamType checkTeamType;
            public int checkCount = 0;
            public Action gameRoleEvent;
        }

        [Header("승리패배 게임룰 데이터")]
        //승리패배 게임룰 데이터
        public GameRuleData gameVictoryRole = new GameRuleData();
        public GameRuleData gameDefeatRole = new GameRuleData();

        [Header("현재 게임플레이 상태")]
        //현재 게임플레이 상태
        public GamePlayState gamePlayState = GamePlayState.PreMatch;

        [Header("이벤트들")]
        //이벤트들
        public Action preMatchUpdateEvent;  //경기전 업뎃
        public Action matchUpdateEvent;     //경기중 업뎃
        public Action victoryUpdateEvent;   //승리 업뎃
        public Action defeatUpdateEvent;    //패배 업뎃

        public void Init(Action preMatchUpdateEvent, Action matchUpdateEvent, Action victoryUpdateEvent, Action defeatUpdateEvent, Action victoryEvent, Action defeatEvent)
        {
            ResetGamePlayState();
            this.preMatchUpdateEvent = preMatchUpdateEvent;
            this.matchUpdateEvent = matchUpdateEvent;
            this.victoryUpdateEvent = victoryUpdateEvent;
            this.defeatUpdateEvent = defeatUpdateEvent;

            gameVictoryRole.gameRoleEvent = victoryEvent;
            gameDefeatRole.gameRoleEvent = defeatEvent;
        }



        public void UpdateGamePlayRuleManager()
        {
            //상태변경을 체크하고 이벤트를 작동시키는 함수
            CheckChangeGamePlayState();

            switch (gamePlayState)
            {
                case GamePlayState.PreMatch:
                    //전투전
                    //매칭전
                    preMatchUpdateEvent?.Invoke();
                    break;
                case GamePlayState.Match:
                    //전투중
                    //매칭중
                    matchUpdateEvent?.Invoke();
                    break;
                case GamePlayState.Victory:
                    //승리상태에서의 업데이트
                    victoryUpdateEvent?.Invoke();
                    break;
                case GamePlayState.Defeat:
                    //패배상태의 업데이트
                    defeatUpdateEvent?.Invoke();
                    break;
            }
        }
        
        /// <summary>
        /// 게임진행
        /// </summary>
        public void PlayGame()
        {
            gamePlayState = GamePlayState.Match;
        }

        /// <summary>
        /// 룰을 확인하여 룰이벤트를 발생시키는 함수
        /// </summary>
        private void CheckChangeGamePlayState()
        {
            /// 승리 패배를 룰을 체크하여 승리했는지 패배했는지 판단후 이벤트를 작동시킨다음
            
            //매칭중이어야지 작동가능//매칭중만 체크해도 승리패배 결과가 나왔는지 까지 처리가능
            if (gamePlayState != GamePlayState.Match)
            {
                return;
            }

            //승리룰부터 체크
            if (CheckRole(gameVictoryRole))
            {
                Debug.Log("GPRM_Vic");
                gamePlayState = GamePlayState.Victory;
                gameVictoryRole.gameRoleEvent?.Invoke();

                //업적갱신
                AchievementManager.Instance.UpdateRecordData("Victory", 1);
                return;
            }

            //패배룰            
            if (CheckRole(gameDefeatRole))
            {
                Debug.Log("GPRM_Def");
                gamePlayState = GamePlayState.Defeat;
                gameDefeatRole.gameRoleEvent?.Invoke();

                //업적갱신
                AchievementManager.Instance.UpdateRecordData("Defeat", 1);
                return;
            }
        }

        public void ResetGamePlayState()
        {   
            gamePlayState = GamePlayState.PreMatch;
        }

        public void ClearTargetTeamCheckBible(TeamType teamType)
        {
            if (!teamCheckBible.ContainsKey(teamType))
            {
                return;
            }
            teamCheckBible[teamType].Clear();
        }

        public void ClearAllTeamCheckData()
        {
            foreach (var item in teamCheckBible)
            {
                item.Value.Clear();
            }
        }

        /// <summary>
        /// 게임룰 데이터에 조건이 맞는지 체크하는 함수
        /// </summary>
        /// <param name="gameRoleData">게임룰데이터</param>
        /// <returns>조건이 맞는지 여부</returns>
        private bool CheckRole(GameRuleData gameRoleData)
        {
            TeamType targetTeamType = gameRoleData.checkTeamType;
            int checkCount = gameRoleData.checkCount;

            if (!teamCheckBible.ContainsKey(targetTeamType))
            {
                return false;
            }

            return teamCheckBible[targetTeamType].Count == checkCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void AddTeamUnit(IGameRuleManagingTarget target)
        {
            var teamType = target.GetTeamType();
            if (!teamCheckBible.ContainsKey(teamType))
            {
                Dictionary<IGameRuleManagingTarget, bool> tempBible = new(10);
                teamCheckBible.Add(teamType, tempBible);
            }
            teamCheckBible[teamType].TryAdd(target, false);
        }


        /// <summary>
        /// 배정된팀에서 유닛을 삭제하는 함수
        /// </summary>
        /// <param name="target">타겟 유닛</param>
        public void RemoveTeamUnit(IGameRuleManagingTarget target)
        {
            var teamType = target.GetTeamType();
            if (!teamCheckBible.ContainsKey(teamType))
            {
                return;
            }

            teamCheckBible[teamType].Remove(target);
        }

        public int GetAmountToTeam(TeamType teamType)
        { 
            int amount = 0;
            if (teamCheckBible.TryGetValue(teamType, out var temp))
            {
                amount = temp.Count;
            }
            return amount;
        }

        public List<IGameRuleManagingTarget> GetTeamCheckTargetList(TeamType teamType)
        {
            checkTargetList.Clear();
            checkTargetList.AddRange(teamCheckBible[teamType].Keys);
            return checkTargetList;
        }


        private static List<IGameRuleManagingTarget> checkTargetList = new List<IGameRuleManagingTarget>(100);

        //밑에함수를 업글해서 팀룰데이터에맞게 가져오기

        /// <summary>
        ///  전장에 있는 거리에 따른 상대유닛을 가져오는 함수
        /// </summary>
        /// <param name="requestTarget">요청한 타겟</param>
        /// <param name="returnTarget">반환될 타겟</param>
        /// <param name="sortASCType">정렬방식</param>
        /// <returns>찾았는지 여부</returns>
        public bool RequestOtherTeamTarget(IGameRuleManagingTarget requestTarget, out IGameRuleManagingTarget returnTarget, bool sortASCType = true)
        {
            returnTarget = null;

            //팀룰체크//다른곳에서 체크하기도하고 지금은 안씀//그냥넘어감
            if (!requestTarget.GetGameTeamRule(out TeamRuleData teamRuleData))
            {
                return false;
            }

            Vector3 worldPos = requestTarget.GetTr().position;

            //적군체크
            for (int i = 0; i < teamRuleData.enemyTeamArray.Length; i++)
            {
                var tempTeam = teamRuleData.enemyTeamArray[i];

                //키값체크
                if (!teamCheckBible.ContainsKey(tempTeam))
                {
                    continue;
                }

                //해당팀을 가져오기
                checkTargetList.Clear();
                checkTargetList.AddRange(teamCheckBible[tempTeam].Keys);

                //거리정렬
                checkTargetList.Sort(new DistanceSort(worldPos, sortASCType));

                //체크
                for (int j = 0; j < checkTargetList.Count; j++)
                {   
                    var target = checkTargetList[j];
                    if (!target.IsAlive())
                    {
                        continue;
                    }
                    returnTarget = target;
                    return true;
                }
            }
            return false;
        }

        //우선순위 정렬을 생각해보자
        public IGameRuleManagingTarget GetPriorityRole(TeamType targetTeam, TeamRolePriorityBible teamRolePriorityBible)
        {
            //키값정렬
            //3가지 방법
            var keyBible = teamCheckBible[targetTeam];
            //public TeamRole[] teamRoleArray = new TeamRole[0];

            //1.링큐 사용
            //var temp = keyBible.OrderBy(x => priorityRole.teamRoleArray[(int)x.Key.GetTeamType()]);

            //2. 링큐IComparer
            //var temp = keyBible.OrderBy(x => new PriorityRoleSort(priorityRole.teamRolePriorityBible));

            //3. 쓰던거 사용//대신 딕셔너리 하나추가됨
            checkTargetList.Clear();
            checkTargetList.AddRange(keyBible.Keys);
            checkTargetList.Sort(new PriorityRoleSort(teamRolePriorityBible));
            var target = checkTargetList[0];

            return target;
            //좀더 생각해보기
            //직업에 대한 우선순위가
            //적군이 안에 들어왔으면 그때 체크하지않나
            //미리 체크하고 찾아오지는 않았던거 같은데
            //기획쪽문제
            //재고해보기
        }


        private struct PriorityRoleSort : IComparer<TeamRole> , IComparer<IGameRuleManagingTarget>
        {   
            TeamRolePriorityBible teamRolePriorityBible;

            public PriorityRoleSort(TeamRolePriorityBible value)
            {
                teamRolePriorityBible = value;
            }

            public int Compare(TeamRole x, TeamRole y)
            {
                int xOrder = teamRolePriorityBible[x];
                int yOrder = teamRolePriorityBible[y];

                return xOrder.CompareTo(yOrder);
            }

            public int Compare(IGameRuleManagingTarget x, IGameRuleManagingTarget y)
            {
                return Compare(x.GetTeamRole(), y.GetTeamRole());
            }
        }
    }
}
