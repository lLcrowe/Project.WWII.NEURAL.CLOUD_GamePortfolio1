using System.Collections.Generic;
using System.IO;
using UnityEngine;
using lLCroweTool.Singleton;
using System.Text;

namespace lLCroweTool.LogSystem
{
    public class LogManager : MonoBehaviourSingleton<LogManager>
    {
        //중앙방식인 로그매니저
        //나중에 아카식레코드랑 같이 사용할예정
        //아직좀더 생각해볼것
        //20221010 주석처리
        //로그매니저를 한개는 가지고 있어야하며 로그를 허용해야지만 적용됨
        //유니크키를 등록후 사용해야됨.
        //사용법
        //1. 원하는 유니크키를 등록후 작동. (로그데이터 키값, 파일이름, 콘솔디버그 사용, 로그파일 사용)
        //2-1. 원할때 Log를 호출시켜 원하는 로그메세지를 등록
        //2-2. 일정하게 호출하고 싶으면 최소 1초 간격을 추천. 10초에서 30초도 괜찮아보임//의도에 따라다름
        //3. 끝

        //확장
        //LogData 클래스 맨밑에 두가지 확장함수
        //LogMassageFormat => 로그 메세지를 원하는 변경으로 변경시키기
        //GetLogFileFullPath => 저장 경로 설정(왠만해선 안바뀔뜻함)

        //20221011 디버그매니저와 통합
        //디버그매니저가 따로 있을필요는 없어보임
        //중앙방식인 디버그매니저는 중앙에서 관리하여 Debug제어를 위한게 큼..
        //외부에서 사용하는 함수객체 있음



        //20221104
        //아카식레코드와 연동시킬준비를하자
        //아카식레코드는 태그형식으로 연동되어 스토리를 만드는게 주목적이다

        //20230417
        //계획변경
        //업적매니저가 완성됨
        //아카식레코드와는 다른방식인데..
        //업적매니저를 연동시켜서 아카식레코드랑 연동시키면 재밌는걸 해볼수 있겠다
        //꽤 유기적으로 작동될거 같음
        //좀 확인해보니 로그매니저의 내부를 다 Static으로 빼야할 작업이 필요함
        //따로 스태딕클래스를 만들어서 처리해보기




        [Header("로그를 사용할 여부")]
        public bool isLogMangerUseLog = false;//로그를 사용할 여부


        private static bool checkQuit = false;
        [Header("로그파일을 저장할 경로")]
        [Tooltip("에셋 경로나 루트경로의 상대경로로 저장됨")]
        public string savePath = "Log";
        
        [Header("로그데이터 목록")]
        public List<LogData> logDataList = new List<LogData>();//로그데이터 리스트
        private Dictionary<object, LogData> logDataBible = new Dictionary<object, LogData>();//저장용도는 아니고 빠른검색목적

      

        protected override void Awake()
        {
            base.Awake();
           
            //등록처리후 로그쓰기
            //Register("Test LogKey", "TestLog.txt", true, true);
            //Log("Test LogKey", "테스트맨");
        }

        //private float time;
        //private void Update()
        //{
        //    //5초마다
        //    if (Time.time > time + 5)
        //    {
        //        Log("Test LogKey", "테스트맨");
        //        time = Time.time;
        //    }
        //}

        protected override void OnDestroy()
        {
            //SaveAllLog();
            base.OnDestroy();
        }

        private void OnApplicationQuit()
        {
            checkQuit = true;
            SaveAllLog();
        }

        /// <summary>
        /// 지정된 로그 파일 이름으로 로그매니저에 등록해줌(절차1)
        /// </summary>
        /// <param name="logDataKey">로그데이터 키</param>
        /// <param name="fileName">파일이름</param>
        /// <param name="printToConsole">콘솔출력 사용여부</param>
        /// <param name="printToFile">파일출력 사용여부</param>
        public static void Register(object logDataKey, string fileName, bool printToConsole, bool printToFile)
        {
            //어플이 꺼지는 여부 체크
            if (checkQuit)
            {
                return;
            }

            if (!Instance.isLogMangerUseLog || Instance.logDataBible.ContainsKey(logDataKey))
            {
                return;
            }

            LogData logger = new LogData(fileName, true, printToConsole, printToFile, logDataKey);
            Instance.logDataBible.Add(logDataKey, logger);
            Instance.logDataList.Add(logger);
        }

        /// <summary>
        /// 로그목록에 등록되있는지 체크하는 함수
        /// </summary>
        /// <param name="logDataKey">로그 데이터키</param>
        /// <returns>등록 여부</returns>
        public static bool CheckRegister(object logDataKey)
        {
            //어플이 꺼지는 여부 체크
            if (checkQuit)
            {
                return false;
            }

            return Instance.logDataBible.ContainsKey(logDataKey);
        }

        /// <summary>
        /// 로그메시지 기록(절차2)(loggerKey 참조로 구성을 검색)
        /// </summary>
        /// <param name="logDataKey">로그데이터 키</param>
        /// <param name="logMassage">로그메세지</param>
        public static void Log(object logDataKey, string logMassage)
        {
            //어플이 꺼지는 여부 체크
            if (checkQuit)
            {
                return;
            }

            //로그가 작동할것인가
            if (!Instance.isLogMangerUseLog || logDataKey == null)
            {
                return;
            }
 
            //해당 키가 있는지 체크
            if (Instance.logDataBible.TryGetValue(logDataKey, out LogData logger))
            {
                //존재하면 로그처리
                logger.Log(logMassage);
            }
        }

        /// <summary>
        /// 로그메시지 기록(절차2)(loggerKey 참조로 구성을 검색)
        /// </summary>
        /// <param name="logDataKey">로그데이터 키</param>
        /// <param name="logMassage">로그용 내용</param>
        /// <param name="targetObject">타겟되는 오브젝트</param>
        /// <param name="LogType">로그타입</param>
        public static void Log(object logDataKey, string logMassage, Object targetObject, LogType LogType)
        {
            //어플이 꺼지는 여부 체크
            if (checkQuit)
            {
                return;
            }

            //로그가 작동할것인가
            if (!Instance.isLogMangerUseLog || logDataKey == null)
            {
                return;
            }

            string temp = null;
            switch (LogType)
            {
                case LogType.Info:
                    temp = "정보";
                    break;
                case LogType.Waring:
                    temp = "경고";
                    break;
                case LogType.Error:
                    temp = "에러";
                    break;
            }

            logMassage = lLcroweUtil.GetCombineString(targetObject.name, temp, logMassage);
            Log(logDataKey, logMassage);
        }

        /// <summary>
        /// 로그액티브여부
        /// </summary>
        /// <param name="logDataKey">로그데이터 키</param>
        /// <param name="isActive">활성화 여부</param>
        public static void LogActive(string logDataKey, bool isActive)
        {
            //어플이 꺼지는 여부 체크
            if (checkQuit)
            {
                return;
            }

            //해당 키가 있는지 체크
            if (Instance.logDataBible.TryGetValue(logDataKey, out LogData logger))
            {
                //존재하면 로그활성화 처리
                logger.enabled = isActive;
            }
        }

        /// <summary>
        /// 모든 로그 저장
        /// </summary>
        private static void SaveAllLog()
        {
            if (!Instance.isLogMangerUseLog)
            {
                return;
            }

            for (int i = 0; i < Instance.logDataList.Count; i++)
            {
                LogData logger = Instance.logDataList[i];
                logger.Log("게임종료. 오브젝트 파괴");
                logger.Save();
            }

            //foreach (LogData logger in Instance.logDataList)
            //{
            //    logger.Log("게임종료. 오브젝트 파괴");
            //    logger.Save();
            //}
        }

        /// <summary>
        /// 로그타입 선별용
        /// </summary>
        public enum LogType
        {
            Info,//특정 정보를 얻기위해 사용됨
            Waring,//안가야될구역에 가면 사용할것
            Error,//문제생기면 안됨
        }

        //private static int GetLineNumber()
        //{
        //    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(1, true);
        //    return st.GetFrame(0).GetFileLineNumber();
        //}
    }

    /// <summary>
    /// 로그데이터//로그매니저에서만 사용하기
    /// </summary>
    [System.Serializable]
    public class LogData
    {
        public string fileName;//이름
        public object key;//키

        public bool enabled;//활성화여부
        public bool printToConsole;//디버그콘솔에 출력
        public bool printToFile;//파일에 출력

        private MemoryStream memoryStream = new MemoryStream();//특정메모리위치를 잡기위한 구역

        /// <summary>
        /// 로그 가능한지 체크
        /// </summary>
        private bool IsActiveLog
        {
            get
            {
                return enabled && LogManager.Instance.isLogMangerUseLog;
            }
        }


        public LogData(string fileName, bool enabled, bool printToConsole, bool printToFile, object key)
        {
            this.fileName = fileName;
            this.enabled = enabled;
            this.printToConsole = printToConsole;
            this.printToFile = printToFile;
            this.key = key;

            WriteToStream(memoryStream, "-= +" + fileName + " 로그시작 : " + System.DateTime.Now.ToString("dd-MM-yyyy") + " =-\n");
        }

        /// <summary>
        /// 로그 작동
        /// </summary>
        /// <param name="logMassage">로그메세지</param>
        public void Log(string logMassage)
        {
            if (IsActiveLog)
            {
                string massage = LogMassageFormat(logMassage.ToString());
                WriteToStream(memoryStream, massage + "\n");

                if (printToConsole)
                {
                    Debug.Log(massage);
                }
            }
        }

        /// <summary>
        /// 스트림에서 읽어오기
        /// </summary>
        /// <param name="stream">스트림계열 클래스</param>
        /// <param name="massage">메세지</param>
        private void WriteToStream(Stream stream, string massage)
        {
            //if (stream != null)
            if (!ReferenceEquals(stream, null))
            {
                //var target = UnicodeEncoding.Unicode;
                //stream.Write(target.GetBytes(massage), 0, target.GetByteCount(massage));
                //유니코드말고 UTF-8
                var encoding = Encoding.UTF8;
                stream.Write(encoding.GetBytes(massage), 0, encoding.GetByteCount(massage));
                //Encoding.UTF8.GetBytes(massage);
                //Encoding.UTF8.GetByteCount(massage);
            }
        }

        /// <summary>
        /// 저장. 로그매니저에서 사용
        /// </summary>
        public void Save()
        {
            if (IsActiveLog && printToFile)
            {
                string filePath = GetLogFileFullPath(fileName);
                FileStream fileWriter = File.Open(filePath, FileMode.Append);
                memoryStream.WriteTo(fileWriter);
                fileWriter.Close();
            }
        }

        //====================================
        //확장성구역===========================
        //====================================

        /// <summary>
        /// 로그에 올린 포맷을 세팅해주는 함수
        /// </summary>
        /// <param name="massage">메세지</param>
        /// <returns>포맷에 맞게 된 메세지</returns>
        protected virtual string LogMassageFormat(string massage)
        {
            //원하는 포맷을 설정
            return System.DateTime.Now.ToLongTimeString() + ":\t" + massage;
        }

        /// <summary>
        /// 로그의 파일경로 가져오기
        /// </summary>
        /// <param name="name">파일이름</param>
        /// <returns>풀 경로</returns>
        protected virtual string GetLogFileFullPath(string name)
        {
            //원하는 경로 설정
            return Application.dataPath + "/" + LogManager.Instance.savePath + "/" + name + "_" + System.DateTime.Now.ToString("yy-MM-dd") + ".txt";
        }
    }
}

