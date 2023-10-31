#define lLcroweProject2D
#define lLcroweProject3D
#define lLcroweHexMapProject

//edit=>PlayerSettings=>OtherSetting=>Scripting Define Symbols 에서
//위의 심볼을 입력해서 처리

using lLCroweTool.Dictionary;
using lLCroweTool.GamePlayRuleSystem;
using lLCroweTool.LogSystem;
using lLCroweTool.TileMap.HexTileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using static lLCroweTool.DataBase.DataBaseInfo;
using static lLCroweTool.ScoreSystem.UI.MVPScoreBoard;
using static lLCroweTool.ScoreSystem.UI.ScoreBoard;

namespace lLCroweTool
{

    /// <summary>
    /// lLcrowe 작업에 필요한 여러 유틸들
    /// </summary>
    public static class lLcroweUtil 
    {
        /// <summary>
        /// 함수에서 float형 데이터를 리턴이나 Ref할때 사용하는 값
        /// </summary>
        public static float valueF;

        /// <summary>
        /// 함수에서 int형 데이터를 리턴이나 Ref할때 사용하는 값
        /// </summary>
        public static int value;


        //  String 클래스를 사용하는 경우 
        //  -문자열을 수정하는 수가 적을 경우(stringbuilder는 string에 비해서 무시해도 좋을 수준의 성능향상을 제공하거나 전혀 제공하지 않을 수 있음)
        //  -부분적인 문자열 글자로 고정된 수의 문자열 연결 작업을 할때(컴파일러가 연결 작업을 단일 작업으로 결합할 수 있음)
        //  -문자열을 작성하는 동안 광법위한 검색 작업을 수행할 때(StringBuilder 클래스는 IndexOf 또는 StartsWith같은 함수가 없다)

        //  StringBuiler 클래스를 사용 하는 경우
        //  -응용 프로그램이 설계시에는 알 수 없는 횟수의 문자열을 변경해야 할 때(사용자의 입력등으로 조합할때 )
        //  -문자열에서 많은 횟수의 변경이 예상될때

        //---------------스트링더하기&스트링빌더원리----------------
        //txt = txt + "1";
        //1. 힙에 특정문자열을 담는 공간을 할당
        //2. 스택에 있는 txt변수에 1번과정에서 할당된 힙의 주소를 저장
        //3. txt + "1" 동작을 수행하기 위해 txt.length + "1".length에 해당되는 크기의 메모리를 힙에 할당.
        //해당 메모리에 txt변수가 가리키는 힙의 문자열과 "1"문자열을 복사한다.
        //4. 다시 스택에 있는 txt변수에 3번과정에서 새롭게 할당된 힙의 주소를 저장
        //5. 3번, 4번의 과정을 X만큼 반복

        //stringBuilder.append("1");
        //1. stringBuilder는 내부적으로 일정한 양의 메모리를 미리할당한다.
        //2. Append 메서드에 들어온 이자를 미리 할당한 메모리에 복사한다.
        //3. 2번과정을 X만큼 반복. Append로 추가된 문자열이 미리 할당한 메모리보다 많아지면 새롭게 여유분의 메모리를 할당
        //4. ToString 메서드를 호출하면 연속적으로 연결된 하나의 문자열을 반환.

        private static StringBuilder builder = new StringBuilder();

        /// <summary>
        /// 여러문자열들 결합해주는 함수
        /// </summary>
        /// <param name="strings">집어넣을 문자열들</param>
        /// <returns>합쳐진 문자열</returns>
        public static string GetCombineString(params string[] strings)
        {   
            //builder.Clear();
            builder.Length = 0;

            for (int i = 0; i < strings.Length; i++)
            {
                builder.Append(strings[i]);
            }
            return builder.ToString(); //반환
        }

        /// <summary>
        /// 컴포넌트복사
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="original">컴포넌트  타입</param>
        /// <param name="target">붙여넣을게임오브젝트</param>
        /// <returns>복사한 컴포넌트</returns>
        public static T GetCopyOf<T>(T original, GameObject target) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = target.AddComponent(type);

            System.Reflection.FieldInfo[] fields = type.GetFields();

            //foreach (System.Reflection.FieldInfo field in fields)
            //{
            //    field.SetValue(copy, field.GetValue(original));
            //}

            for (value = 0; value < fields.Length; value++)
            {
                System.Reflection.FieldInfo field = fields[value];
                field.SetValue(copy, field.GetValue(original));
            } 

            return copy as T;
        }

        /// <summary>
        /// 클래스복사
        /// </summary>
        /// <typeparam name="T">클래스</typeparam>
        /// <param name="original">원본타입</param>
        /// <param name="copyTarget">복사할 new한 클래스</param>
        /// <returns>값을 복사한 클래스</returns>
        public static T GetCopyOf<T>(T original, T copyTarget) where T : class
        {
            System.Type type = original.GetType();

            System.Reflection.FieldInfo[] fields = type.GetFields();

            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copyTarget, field.GetValue(original));
            }
            
            return copyTarget as T;
        }

        //public static T GetCopyOf<T>(T target) where T : class
        //{   
        //    //작동되나 테스트해봐야됨
        //    object tempObject = target.Clone();
        //    return tempObject as T;
        //}

        /// <summary>
        /// 해당 게임오브젝트에서 컴포넌트를 추가하거나 찾는 함수
        /// </summary>
        /// <typeparam name="T">추가하거나 찾을 컴포넌트타입</typeparam>
        /// <param name="go">타겟팅할 게임오브젝트</param>
        /// <returns>찾은 컴포넌트</returns>
        public static T GetAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent(out T component))
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static T GetAddComponent<T>(this Component component) where T : Component
        {   
            return component.gameObject.GetAddComponent<T>();
        }

        /// <summary>
        /// 유니티이벤트를 체크하여 없으면 집어넣는 함수
        /// </summary>
        /// <param name="unityEvent">유니티이벤트</param>
        public static void GetAddUnitEvent(ref UnityEvent unityEvent)
        {
            //if (unityEvent == null)
            if (ReferenceEquals(unityEvent, null))
            {
                unityEvent = new UnityEvent();
            }
        }


        //https://daveoh.wordpress.com/2013/05/02/unity3d-vector3-magnitude-vs-sqrmagnitude/
        //https://answers.unity.com/questions/307612/inversetargetpoint-vs-vector3distance-which-is.html
        //https://answers.unity.com/questions/384932/best-way-to-find-distance.html
        //https://answers.unity.com/questions/125882/vectordistance-performance.html

        //PC(Intel Core i5, 10 million executions per run per function, averaged result over 10 runs)
        //sqrMagnitude: 2853 ms. (96.13% of magnitude’s time)
        //magnitude: 2968 ms. (104.03% of sqrMagnitude’s time)

        //Android(Samsung Galaxy S II, 10 million executions per run per function, averaged result over 10 runs)
        //sqrMagnitude: 6155 ms. (77.11% of magnitude’s time)
        //magnitude: 7982 ms. (129.68% of sqrMagnitude’s time)

        //명시적 형변환과 묵시적 형변환의 차이점
        //*결과적인 차이는 없다
        //*명시적 형변환의 경우 내부적으로 임시변수를 생성에 대입하는 방식으로 성능 저하를 일으킬 수 있다.
        //*묵시적 형변환의 경우 데이터 손실에 대한 경고가 발생한다.
        //CLR(공용 언어 런타임)은 값 형식을 boxing할 때 값을 System.Object 인스턴스 내부에 래핑하고 관리되는 힙에 저장합니다.
        //unboxing하면 개체에서 값 형식이 추출됩니다.
        //Boxing은 암시적이며 unboxing은 명시적입니다.
        //단순 할당에서는 boxing과 unboxing을 수행하는 데 많은 계산 과정이 필요합니다.
        //값 형식을 boxing할 때는 새로운 개체를 할당하고 생성해야 합니다. <=값 형식이 boxing되면 완전히 새로운 개체가 생성되어야 합니다.
        //정도는 약간 덜하지만 unboxing에 필요한 캐스트에도 상당한 계산 과정이 필요합니다
        //Boxing 및 unboxing은 계산을 많이 해야 하는 프로세스입니다. 
        //이 작업은 단순 참조 할당보다 20배나 오래 걸립니다.
        //unboxing 시 캐스팅 프로세스는 할당의 4배에 달하는 시간이 소요될 수 있습니다.
        //성능속도: 박싱(느림) < 언박싱 < 단순할당(빠름)
        //박싱 언박싱 참고할것
        //박싱
        //int a = 1;
        //object b = a;

        //public enum STATE{A = 1, B = 2,}
        //STATE a = STATE.A;
        //STATE b = STATE.B;
        //// Enum 비교 시 a, b가 boxing이 발생한다
        //if (a.Equals(b)) { }
        //int ia = 1;
        //int ib = 2;
        //// 단순 enum 비교로 boxing이 발생하지 않는다
        //if (ia.Equals(ib)) { }

        //boxing은 암시적으로 사용되고 있기 때문에 주의 깊게 생각하고 사용하지 않으면 생각보다 많은 곳에서 발생하게 된다.
        //꼭 필요한 곳에서 사용하는 것은 어쩔 수 없지만, 불필요한 곳에서 사용하게 되는 경우 성능 저하를 발생 시키기 때문에 유의해야한다.
        //=>언제한번 전체다 처리해버려야겠는걸. 결국 값형식으로 비교하면 Equal 할때 GC가 안쌓인다 라는것이군
        //=>ilspy로보니 Equals이 해당값과 object가 있다. object로 가면 GC가 쌓임
        //=> == 로 할시 ceq가 작동됨. equals로 할시 boxing callvirt 진행
        //ceq 스택의 두 값을 꺼내고, 같으면 1, 다르면 0을 스택에 넣는다.


        //타임.타임만 메인스레드말고는 작동안함//유니티가 동기화하면서 하면서 안되는 스레드들이 있다.//랜덤도 안됨//티슈에서 뽑아쓰듯이


        //CMP 연산자
        //0040150E 메모리주소
        //C74424 오퍼레이션코드 
        //Dword 4byte 
        //PTR 포인트
        //stack segment 
        //: []
        //오피코드 확인//exe. CPU가 적힘

        //JMP 점프
        //JNZ 두개의 연산을 해가주고 
        //SS 소스타겟

        //0040150E  |.  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0
        //00401516  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0

        //0040151E  |.  837C24 0C 01  CMP DWORD PTR SS:[ESP+C],1
        //00401523  |.  75 0A JNZ SHORT a.0040152F
        //00401525  |.  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //0040152D  |.  EB 2A JMP SHORT a.00401559

        //0040152F  |>  837C24 0C 02  CMP DWORD PTR SS:[ESP+C],2
        //00401534  |.  75 0A JNZ SHORT a.00401540
        //00401536  |.  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040153E  |.  EB 19         JMP SHORT a.00401559

        //00401540  |>  837C24 0C 00  CMP DWORD PTR SS:[ESP+C],0
        //00401545  |.  75 0A JNZ SHORT a.00401551
        //00401547  |.  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //0040154F  |.  EB 08         JMP SHORT a.00401559

        //00401551  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4


        //00401559  |>  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0
        //00401561  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0


        //00401569  |.  8B4424 0C MOV EAX,DWORD PTR SS:[ESP+C]
        //0040156D  |.  83F8 01       CMP EAX,1
        //00401570  |.  74 0B         JE SHORT a.0040157D
        //00401572  |.  83F8 02       CMP EAX,2
        //00401575  |.  74 10         JE SHORT a.00401587
        //00401577  |.  85C0 TEST EAX,EAX
        //00401579  |.  74 16         JE SHORT a.00401591
        //0040157B  |.  EB 1E         JMP SHORT a.0040159B
        //0040157D  |>  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //00401585  |.  EB 1C JMP SHORT a.004015A3
        //00401587  |>  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040158F  |.  EB 12         JMP SHORT a.004015A3
        //00401591  |>  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //00401599  |.  EB 08         JMP SHORT a.004015A3
        //0040159B  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4
        //004015A3  |>  C9 LEAVE
        //004015A4  \.  C3 RETN

        //0040150E  |.  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0   int a = 0;
        //00401516  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0   int b = 0;
        //0040151E  |.  837C24 0C 01  CMP DWORD PTR SS:[ESP+C],1   if (a == 1
        //00401523  |.  75 0A JNZ SHORT a.0040152F         goto ax)
        //00401525  |.  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1   b = 1;
        //0040152D  |.  EB 2A JMP SHORT a.00401559         goto end
        //0040152F  |>  837C24 0C 02  CMP DWORD PTR SS:[ESP+C],2   ax, (if a == 2
        //00401534  |.  75 0A JNZ SHORT a.00401540         goto bx)
        //00401536  |.  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2   b = 2;
        //0040153E  |.  EB 19         JMP SHORT a.00401559         goto end
        //00401540  |>  837C24 0C 00  CMP DWORD PTR SS:[ESP+C],0   bx, (if a == 0
        //00401545  |.  75 0A JNZ SHORT a.00401551         goto cx)
        //00401547  |.  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3   b = 3;
        //0040154F  |.  EB 08         JMP SHORT a.00401559         goto end
        //00401551  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4   cx, b = 4;
        //00401559  |>  C74424 0C 000>MOV DWORD PTR SS:[ESP+C],0   end, a = 0;
        //00401561  |.  C74424 08 000>MOV DWORD PTR SS:[ESP+8],0   b = 0;
        //00401569  |.  8B4424 0C MOV EAX,DWORD PTR SS:[ESP+C]
        //0040156D  |.  83F8 01       CMP EAX,1
        //00401570  |.  74 0B         JE SHORT a.0040157D
        //00401572  |.  83F8 02       CMP EAX,2
        //00401575  |.  74 10         JE SHORT a.00401587
        //00401577  |.  85C0 TEST EAX,EAX
        //00401579  |.  74 16         JE SHORT a.00401591
        //0040157B  |.  EB 1E         JMP SHORT a.0040159B
        //0040157D  |>  C74424 08 010>MOV DWORD PTR SS:[ESP+8],1
        //00401585  |.  EB 1C JMP SHORT a.004015A3
        //00401587  |>  C74424 08 020>MOV DWORD PTR SS:[ESP+8],2
        //0040158F  |.  EB 12         JMP SHORT a.004015A3
        //00401591  |>  C74424 08 030>MOV DWORD PTR SS:[ESP+8],3
        //00401599  |.  EB 08         JMP SHORT a.004015A3
        //0040159B  |>  C74424 08 040>MOV DWORD PTR SS:[ESP+8],4
        //004015A3  |>  C9 LEAVE
        //004015A4  \.  C3 RETN

        /// <summary>
        /// 게임오브젝트를 씬이 변경될때 파괴되지않도록 하는 함수
        /// </summary>
        /// <param name="_targetGameObject">파괴시키지않을 게임오브젝트</param>
        public static void DontDestroyTargetObject(GameObject _targetGameObject)
        {
            Object.DontDestroyOnLoad(_targetGameObject);
        }

        /// <summary>
        /// 타겟이 될 트랜스폼을 최상단부모로 옮기
        /// </summary>
        /// <param name="target">트랜스폼</param>
        public static void DeActiveSetNullParent(Transform target)
        {
            if (target.childCount == 0)
            {
                target.SetParent(null);
                target.gameObject.SetActive(false);
            }
            else
            {
                Transform[] transformArray = target.GetComponentsInChildren<Transform>();
                for (int i = 0; i < transformArray.Length; i++)
                {
                    transformArray[i].SetParent(null);
                    transformArray[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 특정부모에 타겟팅한 오브젝트를 자식으로 집어넣고 부모위치와 회전값을 집어넣음
        /// </summary>
        /// <param name="parent">집어넣을 부모</param>
        /// <param name="target">타겟팅될 객체NotNull</param>
        public static void SetParentToTarget(Transform parent, Transform target)
        {
            target.SetParent(parent);
            if (!ReferenceEquals(parent, null))
            {
                target.SetPositionAndRotation(parent.position, Quaternion.identity);
            }
        }

        /// <summary>
        /// 거리를 체크해주는 함수.유니티거리체크보다 빠름//차이는 솔직히 매우 미세함
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <param name="distance">거리</param>
        /// <returns>해당거리보다 가까운지 여부</returns>
        public static bool CheckDistance(Vector3 a, Vector3 b, float distance)
        {
            bool check = GetDistance(a, b) < distance * distance + 0.001f;
            return check;
        }

        /// <summary>
        /// 거리에 대한 크기를 가져오는 함수//비교할경우 거리의 제곱을 비교할것
        /// </summary>
        /// <param name="a">a위치좌표</param>
        /// <param name="b">b위치좌표</param>
        /// <returns>거리의 크기</returns>
        public static float GetDistance(Vector3 a, Vector3 b)
        {
            float distacne = (a - b).sqrMagnitude;
            return distacne;
        }


        /// <summary>
        /// A->B방향으로 회전값 가져오는 함수
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="removeAngle">필요시 따로 설정하여 각도변환하기</param>
        /// <returns>회전값</returns>
        public static Quaternion GetRotation(Vector3 rotateTarget, Vector3 lookTarget, float removeAngle = 90)
        {
            Vector2 targetDir = lookTarget - rotateTarget;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - removeAngle;
            //Debug.Log("Angle :" + newangle + "angle2" + Vector2.Angle(rotateTarget, lookTarget));
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            return quaternion;
        }

        /// <summary>
        /// A->B방향으로 제한된 회전값 가져오는 함수
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="limitAngle">제한값</param>
        /// <param name="removeAngle">필요시 따로 설정하여 각도변환하기</param>
        /// <returns>회전값</returns>
        public static Quaternion GetRotation(Transform rotateTarget, Transform lookTarget, float limitAngle, float removeAngle = 90)
        {
            //각도 제한 테스트해야됨
            //봐야될 위치 기준으로 각도를 일정정해주고 회저

            //회전각도체크할 오브젝트 => lookTarget
            //회전시킬오브젝트 => rotateTarget;
            //볼대상의 각도에서 체크            

            Vector3 euler = lookTarget.rotation.eulerAngles;
            euler.z = euler.z > 180 ? euler.z - 360 : euler.z;

            Vector3 rotateEuler = rotateTarget.rotation.eulerAngles;
            rotateEuler.z = rotateEuler.z > 180 ? rotateEuler.z - 360 : rotateEuler.z;

            rotateEuler.z = Mathf.Clamp(rotateEuler.z, euler.z - limitAngle, euler.z + limitAngle);



            Debug.Log($"봐야되는 오브젝트각도 : {euler.z} 회전오브젝트각도  {rotateEuler.z}");
            //if (newangle > limitAngle)
            //{
            //    return targetObject.rotation;
            //}
        

            Vector2 targetDir = lookTarget.position - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - removeAngle;
            //Debug.Log("Angle :" + newangle + "angle2" + Vector2.Angle(rotateTarget, lookTarget));
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            quaternion *= Quaternion.Euler(rotateEuler);
            return quaternion;
        }

        /// <summary>
        /// 투사체가 충돌체와 충돌시 특정각도내에 있을시 투사체의 발사각을 바꾸는 함수
        /// </summary>
        /// <param name="collision">충돌한 충돌체</param>
        /// <param name="projectileObject">투사체 오브젝트</param>
        /// <param name="reflectAngle">최대 도탄각도</param>
        /// <returns>도탄됫는지 여부</returns>
        public static bool ActionProjectileReflect(Collision2D collision, Transform projectileObject, float reflectAngle)
        {   
            //반사위치체크
            //결과값=Vector2.Reflect(입사각의 위치(충돌각도), 노말값의 위치(충돌시 타겟의 각도)
            //노말값은 충돌했을시 각도를 말하는것
            Vector2 direction = Vector2.Reflect(projectileObject.transform.up, collision.contacts[0].normal);



            //테스트하기
            //Vector2 direction = Vector2.Reflect(collision.contacts[0].point, collision.contacts[0].normal);
            //Gizmos.DrawLine(hitPos, normal * size);//동일1
            //Gizmos.DrawLine(hitPos, Vector2.Reflect(transform.up, normal));//동일2


            //반사각도 확인
            float InAngle = Vector2.Angle(projectileObject.transform.up, direction);
            //Debug.Log($"{collision.collider.name} 충돌, 입사각 : {InAngle}, 입사각 위치 : { (Vector2)refectionAttackBox.transform.up} , 반사 결과값 : {direction } ");

            //왠만해선60~70각도이하부터가 볼만함//55도가 적당한것같기도하구
            if (InAngle > reflectAngle)
            {
                return false;
            }

            projectileObject.transform.up = direction;//방향동기화
            return true;
        }

        //사용안함
        public static float CalPow(int num, int RepeatValue)
        {
            float value = 0;

            value = (Mathf.Pow(num, RepeatValue + 1) - 1) / (num - 1);


            return value;
        }

        //사용안함
        public static Vector2 GetWorldPosToUIPos(Vector2 worldPos, Canvas canvas, Camera camera)
        {
            //미완
            RectTransform CanvasRect = canvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPos);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //Rect의 앵커포지션으로 반환
            return WorldObject_ScreenPosition;
        } 

        /// <summary>
        /// 지정한 좌표간의 센터위치 구하는 함수
        /// </summary>
        /// <param name="_points">좌표 값들</param>
        /// <returns>중앙위치</returns>
        public static Vector2 Centroid(Vector2[] _points)
        {
            Vector2 center = Vector2.zero;
            for (int i = 0; i < _points.Length; i++)
            {
                center += _points[i];
            }
            center /= _points.Length;
            return center;
        }

        /// <summary>
        /// 지정한 좌표간의 센터위치 구하는 함수
        /// </summary>
        /// <param name="_points">좌표 값들</param>
        /// <returns>중앙위치</returns>
        public static Vector2 Centroid(Vector3Int[] _points)
        {
            Vector3 center = Vector2.zero;
            for (int i = 0; i < _points.Length; i++)
            {
                center += _points[i];
            }
            center /= _points.Length;
            return center;
        }

        /// <summary>
        /// 특정값의 퍼센트 값만큼 가져오는 함수
        /// </summary>
        /// <param name="value">특정값</param>
        /// <param name="percent">퍼센트</param>
        /// <returns>특정값의 몇퍼센트값</returns>
        public static float GetPercentForValue(float value, float percent)
        {
            return value * (percent * 0.01f);
        }

        /// <summary>
        /// enum형식에 정의된 값들을 가져오는 함수
        /// </summary>
        /// <typeparam name="T">enum형식</typeparam>
        /// <returns>enum에 정의된 값리스트</returns>
        public static List<T> GetEnumDefineData<T>() where T : struct
        {
            var tempArray = System.Enum.GetNames(typeof(T));
            
            List<T> list = new List<T>();
            var tempArray2 = System.Enum.GetValues(typeof(T));
            foreach (var item in tempArray2)
            {
                list.Add((T)item);
            }


            //foreach (var item in tempArray)
            //{
            //    if (System.Enum.TryParse(typeof(T).Name, out T result))
            //    {
            //        list.Add(result);
            //    }
            //}
            return list;
        }

        /// <summary>
        /// enum형식에 정의된 값들의 이름을 가져오는 함수
        /// </summary>
        /// <typeparam name="T">enum형식</typeparam>
        /// <returns>enum에 정의된 이름리스트</returns>
        public static List<string> GetEnumDefineStringData<T>() where T : struct
        {
            var tempArray = System.Enum.GetNames(typeof(T));
            List<string> list = new List<string>(tempArray);
            return list;
        }

        //로그
        public static void LogIList<T>(T target) where T : IList
        {
            string content = "";
            foreach (var item in target)
            {
                content = GetCombineString(content, $"{item}\n");
            }
            Debug.Log(content);
        }

        //======================================================================
        //랜덤부분---------------------------------------------------------------
        //======================================================================

        /// <summary>
        /// 확률 계산
        /// </summary>
        /// <param name="probabilityNum">집어넣을 확률</param>
        /// <returns>확률에 들어갔는가 여부</returns>
        public static bool ProbabilityCal(int probabilityNum)
        {
            //bool isTure = false;
            int num = Random.Range(0, 101);
            return probabilityNum >= num ? true : false;
            //if (probabilityNum >= num)
            //{
            //    isTure = true;
            //}
            //else
            //{
            //    isTure = false;
            //}
            //return isTure;
        }



        /// <summary>
        /// 랜덤한 원형위치를 가져오는 함수
        /// </summary>
        /// <param name="targetPos">타겟 위치</param>
        /// <param name="size">크기</param>
        /// <param name="isCheck">체크여부</param>
        /// <returns>랜덤위치</returns>
        public static Vector2 GetRandomCirclePosition(Transform targetPos, float size, bool isCheck = false)
        {
            Vector2 randomPos = isCheck ? (Vector2)targetPos.position + Random.insideUnitCircle * size : targetPos.position;
            return randomPos;
        }

        /// <summary>
        /// 랜덤한 원형위치를 가져오는 함수
        /// </summary>
        /// <param name="targetPos">타겟 위치</param>
        /// <returns>랜덤위치</returns>
        public static Vector2 GetRandomCirclePosition(Vector2 targetPos, float size, bool isCheck = false)
        {
            Vector2 randomPos = isCheck ? targetPos + Random.insideUnitCircle * size : targetPos;
            return randomPos;
        }

        //======================================================================


        //타일맵관련

        /// <summary>
        /// 타일맵에 배치되있는 포지션을 모두가져오기. 매프레임으로 돌리지 말기
        /// </summary>
        /// <returns>타일이 있는 위치들</returns>
        public static Vector3Int[] GetAllTilePos(Tilemap tilemap)
        {
            //모든타일가져오기
            BoundsInt bounds = tilemap.cellBounds;
            List<Vector3Int> posList = new List<Vector3Int>();

            //int xMax = floorTileMap.GetTileMap().cellBounds.xMax;
            //int xMin = floorTileMap.GetTileMap().cellBounds.xMin;
            //int yMax = floorTileMap.GetTileMap().cellBounds.yMax;
            //int yMin = floorTileMap.GetTileMap().cellBounds.yMin;
            //Debug.Log(xMax + ", " + yMin + ", " + xMax + ", " + yMin + "");
            
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    Vector3Int target = new Vector3Int(x, y, 0);

                    TileBase tile = tilemap.GetTile(target);
                    if (tile != null)
                    {
                        //Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                        posList.Add(new Vector3Int(x, y, 0));
                        //count++;
                    }
                    else
                    {
                        // Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                    }
                }
            }
            return posList.ToArray();
        }

        //-===========박스(네모)타일용

        /// <summary>
        /// 해당타일위치의 근처타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>위, 아래, 좌, 우  위치반환</returns>
        public static Vector3Int[] GetSideTilePos(Vector3Int tilePos)
        {
            Vector3Int upTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
            Vector3Int downTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
            Vector3Int leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            Vector3Int rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);

            Vector3Int[] sidePosArray = { upTilePos, downTilePos, leftTilePos, rightTilePos };

            return sidePosArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일을 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일반환</returns>
        public static Tile[] GetSideTile(Vector3Int tilePos, Tilemap tilemap)
        {
            Vector3Int[] sidePosArray = GetSideTilePos(tilePos);
            //upTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
            //downTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
            //leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            //rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
            Tile upTile = tilemap.GetTile<Tile>(sidePosArray[0]);
            Tile downTile = tilemap.GetTile<Tile>(sidePosArray[1]);
            Tile leftTile = tilemap.GetTile<Tile>(sidePosArray[2]);
            Tile rightTile = tilemap.GetTile<Tile>(sidePosArray[3]);

            Tile[] sideTileArray = { upTile, downTile, leftTile, rightTile };

            return sideTileArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">체크할 위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideTileIsHas(Vector3Int tilePos, Tilemap tilemap)
        {
            Vector3Int[] sidePosArray = GetSideTilePos(tilePos);

            bool upTile = tilemap.HasTile(sidePosArray[0]);
            bool downTile = tilemap.HasTile(sidePosArray[1]);
            bool leftTile = tilemap.HasTile(sidePosArray[2]);
            bool rightTile = tilemap.HasTile(sidePosArray[3]);

            bool[] sideIsHasArray = { upTile, downTile, leftTile, rightTile };

            return sideIsHasArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="sidePosArray">위, 아래, 좌, 우 타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideTileIsHas(Vector3Int[] sidePosArray, Tilemap tilemap)
        {
            bool upTile = tilemap.HasTile(sidePosArray[0]);
            bool downTile = tilemap.HasTile(sidePosArray[1]);
            bool leftTile = tilemap.HasTile(sidePosArray[2]);
            bool rightTile = tilemap.HasTile(sidePosArray[3]);

            bool[] sideIsHasArray = { upTile, downTile, leftTile, rightTile };

            return sideIsHasArray;
        }


        /// <summary>
        /// 좌표에 있는 타일이 사이드타일인지 확인하는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>사이드타일여부</returns>
        public static bool GetIsSideTile(Vector3Int tilePos, Tilemap tilemap)
        {
            bool[] checkArray = GetSideTileIsHas(tilePos, tilemap);

            for (int i = 0; i < checkArray.Length; i++)
            {
                if (!checkArray[i])
                {
                    return true;
                }
            }
            return false;
        }


#if lLcroweHexMapProject

        //-===========2D 헥스타일용



        /// <summary>
        /// 해당타일위치의 근처타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>우상 좌하//우 좌//우하 좌상 위치반환</returns>
        public static Vector3Int[] GetSideHexTilePos(Vector3Int tilePos)
        {
            bool isOdd = tilePos.y % 2 == 1;
            return GetSideHexTilePos(tilePos, isOdd);
        }

        /// <summary>
        /// 해당타일위치의 근처타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>우상 좌하//우 좌//우하 좌상 위치반환</returns>
        public static Vector3Int[] GetSideHexTilePos(Vector3Int tilePos, bool isOdd)
        {
            int addXPos = isOdd ? 1 : 0;

            Vector3Int rightUpTilePos = new Vector3Int(tilePos.x + addXPos, tilePos.y + 1, tilePos.z);
            Vector3Int rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
            Vector3Int rightDownTilePos = new Vector3Int(tilePos.x + addXPos, tilePos.y - 1, tilePos.z);

            Vector3Int leftDownTilePos = new Vector3Int(tilePos.x - 1 + addXPos, tilePos.y - 1, tilePos.z);
            Vector3Int leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            Vector3Int leftUpTilePos = new Vector3Int(tilePos.x - 1 + addXPos, tilePos.y + 1, tilePos.z);

            //우상 좌하//우 좌//우하 좌상
            Vector3Int[] sidePosArray = { rightUpTilePos, leftDownTilePos, rightTilePos, leftTilePos, rightDownTilePos, leftUpTilePos };

            return sidePosArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>우상 좌하//우 좌//우하 좌상 위치반환</returns>
        public static Vector3Int[] GetSideHexTilePos(Vector3Int tilePos, int size)
        {
            //bool isOdd = tilePos.y % 2 == 1;
            //int addXPos = isOdd ? 1 : 0;
            int addXPosUp = GetHexTileMapAddXPos(tilePos.y);
            int addXPosDown = GetHexTileMapAddXPos(tilePos.y);

            //1일시


            Vector3Int rightUpTilePos = new Vector3Int(tilePos.x + addXPosUp, tilePos.y + size, tilePos.z);
            Vector3Int rightTilePos = new Vector3Int(tilePos.x + size, tilePos.y, tilePos.z);//작동잘됨
            Vector3Int rightDownTilePos = new Vector3Int(tilePos.x + addXPosDown, tilePos.y - size, tilePos.z);



            Vector3Int leftDownTilePos = new Vector3Int(tilePos.x - size + addXPosDown, tilePos.y - size, tilePos.z);
            Vector3Int leftTilePos = new Vector3Int(tilePos.x - size, tilePos.y, tilePos.z);//작동잘됨
            Vector3Int leftUpTilePos = new Vector3Int(tilePos.x - size + addXPosUp, tilePos.y + size, tilePos.z);


            //원본
            //Vector3Int rightUpTilePos = new Vector3Int(tilePos.x + addXPos, tilePos.y + 1, tilePos.z);
            //Vector3Int rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
            //Vector3Int rightDownTilePos = new Vector3Int(tilePos.x + addXPos, tilePos.y - 1, tilePos.z);

            //Vector3Int leftDownTilePos = new Vector3Int(tilePos.x - 1 + addXPos, tilePos.y - 1, tilePos.z);
            //Vector3Int leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            //Vector3Int leftUpTilePos = new Vector3Int(tilePos.x - 1 + addXPos, tilePos.y + 1, tilePos.z);






            //우상 좌하//우 좌//우하 좌상
            Vector3Int[] sidePosArray = { rightUpTilePos, leftDownTilePos, rightTilePos, leftTilePos, rightDownTilePos, leftUpTilePos };

            return sidePosArray;
        }

        private static int GetHexTileMapAddXPos(int tileYPos)
        {
            bool isOdd = tileYPos % 2 == 1;
            return isOdd ? 1 : 0;
        }

        /// <summary>
        /// 해당타일위치의 근처타일을 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>우상 좌하//우 좌//우하 좌상  타일반환</returns>
        public static Tile[] GetSideHexTile(Vector3Int tilePos, Tilemap tilemap, bool isOdd)
        {
            Vector3Int[] sidePosArray = GetSideHexTilePos(tilePos, isOdd);
            //upTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
            //downTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
            //leftTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
            //rightTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);
            Tile rightUpTile = tilemap.GetTile<Tile>(sidePosArray[0]);
            Tile leftDownTile = tilemap.GetTile<Tile>(sidePosArray[1]);

            Tile rightTile = tilemap.GetTile<Tile>(sidePosArray[2]);
            Tile leftTile = tilemap.GetTile<Tile>(sidePosArray[3]);

            Tile rightDownTile = tilemap.GetTile<Tile>(sidePosArray[4]);
            Tile leftUpTile = tilemap.GetTile<Tile>(sidePosArray[5]);


            Tile[] sideTileArray = { rightUpTile, leftDownTile, rightTile, leftTile, rightDownTile, leftUpTile };

            return sideTileArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">체크할 위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideHexTileIsHas(Vector3Int tilePos, Tilemap tilemap, bool isOdd)
        {
            Vector3Int[] sidePosArray = GetSideHexTilePos(tilePos, isOdd);

            bool rightUpTile = tilemap.HasTile(sidePosArray[0]);
            bool leftDownTile = tilemap.HasTile(sidePosArray[1]);

            bool rightTile = tilemap.HasTile(sidePosArray[2]);
            bool leftTile = tilemap.HasTile(sidePosArray[3]);

            bool rightDownTile = tilemap.HasTile(sidePosArray[4]);
            bool leftUpTile = tilemap.HasTile(sidePosArray[5]);

            bool[] sideIsHasArray = { rightUpTile, leftDownTile, rightTile, leftTile, rightDownTile, leftUpTile };

            return sideIsHasArray;
        }

        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="sidePosArray">위, 아래, 좌, 우 타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideHexTileIsHas(Vector3Int[] sidePosArray, Tilemap tilemap)
        {
            bool rightUpTile = tilemap.HasTile(sidePosArray[0]);
            bool leftDownTile = tilemap.HasTile(sidePosArray[1]);

            bool rightTile = tilemap.HasTile(sidePosArray[2]);
            bool leftTile = tilemap.HasTile(sidePosArray[3]);

            bool rightDownTile = tilemap.HasTile(sidePosArray[4]);
            bool leftUpTile = tilemap.HasTile(sidePosArray[5]);

            bool[] sideIsHasArray = { rightUpTile, leftDownTile, rightTile, leftTile, rightDownTile, leftUpTile };


            return sideIsHasArray;
        }

        /// <summary>
        /// 좌표에 있는 타일이 사이드타일인지 확인하는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns>사이드타일여부</returns>
        public static bool GetIsSideHexTile(Vector3Int tilePos, Tilemap tilemap, bool isOdd)
        {
            bool[] checkArray = GetSideHexTileIsHas(tilePos, tilemap, isOdd);

            for (int i = 0; i < checkArray.Length; i++)
            {
                if (!checkArray[i])
                {
                    return true;
                }
            }
            return false;
        }


        public static class HexTileMatrix
        {
            /// <summary>
            /// 헥스타일 타입
            /// </summary>
            public enum HexTileType
            {
                FlatTop,        //선이 위쪽에 있는 넓적한 헥스타일 
                PointyTop,      //점이 위쪽에 있는 길쭉한 헥스타일
            }

            /// <summary>
            /// 타일생성할때 어느방향으로 생성시킬건지에 대한 타입
            /// </summary>
            public enum CreateTileAxisType
            {
                XY,
                XZ,
            }


            //public const float outerRadius = 10;
            //public const float innerRadius = outerRadius * 0.866025404f;

            private static float outerSize;
            private static float InnerSize
            {
                get
                {
                    return outerSize * 0.866f;
                }
            }


            /// <summary>
            /// 헥스타일의 점정을 가져오는 함수
            /// </summary>
            /// <param name="centerPos">중앙위치값</param>
            /// <param name="size">크기</param>
            /// <param name="hexTileType">타일타입</param>
            /// <param name="createTileAxisType">제작될 축</param>
            /// <returns>정점위치들</returns>
            public static Vector3[] GetHexTilePoint(Vector3 centerPos, float size, HexTileType hexTileType, CreateTileAxisType createTileAxisType)
            {
                outerSize = size;
                switch (hexTileType)
                {
                    case HexTileType.FlatTop:
                        return GetFlatTopHexTypeArray(centerPos, outerSize, InnerSize, createTileAxisType);
                    case HexTileType.PointyTop:
                        return GetPointTopHexTypeArray(centerPos, outerSize, InnerSize, createTileAxisType);
                }

                return new Vector3[0];
            }

            //x z //d\위아래가 긴거
            private static Vector3[] GetPointTopHexTypeArray(Vector3 centerPos, float outerRadius, float innerRadius, CreateTileAxisType createTileAxisType)
            {
                //위
                //위오른쪽
                //위왼쪽

                //아래
                //아래 좌측
                //아래 우측


                switch (createTileAxisType)
                {
                    case CreateTileAxisType.XY:
                        Vector3[] tempArray1 =
                        {
                            new Vector3(0f, outerRadius) + centerPos,
                            new Vector3(innerRadius, 0.5f * outerRadius)+ centerPos,
                            new Vector3(innerRadius, -0.5f * outerRadius)+ centerPos,
                            new Vector3(0f, -outerRadius)+ centerPos,
                            new Vector3(-innerRadius, -0.5f * outerRadius)+ centerPos,
                            new Vector3(-innerRadius, 0.5f * outerRadius)+ centerPos
                        };
                        return tempArray1;
                    case CreateTileAxisType.XZ:
                        Vector3[] tempArray2 =
                        {
                            new Vector3(0f, 0f, outerRadius) + centerPos,
                            new Vector3(innerRadius, 0f, 0.5f * outerRadius)+ centerPos,
                            new Vector3(innerRadius, 0f, -0.5f * outerRadius)+ centerPos,
                            new Vector3(0f, 0f, -outerRadius)+ centerPos,
                            new Vector3(-innerRadius, 0f, -0.5f * outerRadius)+ centerPos,
                            new Vector3(-innerRadius, 0f, 0.5f * outerRadius)+ centerPos
                        };
                        return tempArray2;
                }

                return new Vector3[0];
            }

            //넓적한거
            private static Vector3[] GetFlatTopHexTypeArray(Vector3 centerPos, float outerRadius, float innerRadius, CreateTileAxisType createTileAxisType)
            {
                //육각형타입
                //상단 1 2
                //중간 6 3
                //하단 4 5


                switch (createTileAxisType)
                {
                    case CreateTileAxisType.XY:
                        Vector3[] tempArray1 =
                        {
                            new Vector3(outerRadius * -0.5f, innerRadius) + centerPos,
                            new Vector3(outerRadius  * 0.5f,innerRadius) + centerPos,
                            new Vector3(outerRadius, 0f) + centerPos,
                            new Vector3(outerRadius * 0.5f, -innerRadius) + centerPos,
                            new Vector3(outerRadius  * -0.5f, -innerRadius) + centerPos,
                            new Vector3(-outerRadius, 0f ) + centerPos
                        };
                        return tempArray1;
                    case CreateTileAxisType.XZ:
                        Vector3[] tempArray2 =
                        {
                            new Vector3(outerRadius * -0.5f, 0f, innerRadius) + centerPos,
                            new Vector3(outerRadius  * 0.5f,0f, innerRadius) + centerPos,
                            new Vector3(outerRadius, 0f, 0f) + centerPos,
                            new Vector3(outerRadius * 0.5f, 0f, -innerRadius) + centerPos,
                            new Vector3(outerRadius  * -0.5f,0f, -innerRadius) + centerPos,
                            new Vector3(-outerRadius, 0f, 0f) + centerPos
                        };
                        return tempArray2;
                }


                return new Vector3[0];
            }

            /// <summary>
            /// 헥스타일의 점정을 가져오는 함수 <para> 삼각형만들때 사용함</para>
            /// </summary>
            /// <param name="center">중앙위치값</param>
            /// <param name="size">크기</param>
            /// <param name="height">높이</param>
            /// <param name="index">인덱스</param>
            /// <param name="hexTileType">타일타입</param>
            /// <param name="createTileAxisType">제작될 축</param>
            /// <returns>점정위치</returns>
            public static Vector3 GetHexTilePoint(Vector3 center, float size , float height, int index, HexTileType hexTileType, CreateTileAxisType createTileAxisType)
            {
                float addAngle = hexTileType == HexTileType.PointyTop ? 30f : 0f;
                var angle_deg = 60 * index - addAngle;
                var angle_rad = Mathf.PI / 180 * angle_deg;
                switch (createTileAxisType)
                {
                    case CreateTileAxisType.XY:
                        return new Vector3(center.x + size * Mathf.Cos(angle_rad), center.y + size * Mathf.Sin(angle_rad), height);
                    case CreateTileAxisType.XZ:
                        return new Vector3(center.x + size * Mathf.Cos(angle_rad), height, center.y + size * Mathf.Sin(angle_rad));
                }
                return Vector3.zero;
            }

            /// <summary>
            /// 헥스타일의 점정을 가져오는 함수 <para> 삼각형만들때 사용함</para>
            /// </summary>
            /// <param name="center">중앙위치값</param>
            /// <param name="size">크기</param>
            /// <param name="height">높이</param>            
            /// <param name="hexTileType">타일타입</param>
            /// <param name="createTileAxisType">제작될 축</param>
            /// <returns>앵글로 만든 헥스타일포인트 배열6</returns>
            public static Vector3[] GetHexTilePoint(Vector3 center, float size, float height, HexTileType hexTileType, CreateTileAxisType createTileAxisType)
            {
                Vector3[] hexTilePointArray = new Vector3[6];

                for (int i = 0; i < hexTilePointArray.Length; i++)
                {
                    hexTilePointArray[i] = GetHexTilePoint(center, size, height, i,hexTileType, createTileAxisType);
                }

                return hexTilePointArray;
            }
        }

        //-===========커스텀3D 헥스타일용 맵 유틸

        public static Vector3Int[] GetAllTilePos(Custom3DHexTileMap custom3DHexTileMap)
        {
            Vector3Int[] vector3IntArray = new Vector3Int[custom3DHexTileMap.hexTileObjectBible.Count];
            int i = 0;
            foreach (var item in custom3DHexTileMap.hexTileObjectBible)
            {
                vector3IntArray[i] = item.Key;
                i++;
            }

            return vector3IntArray;
        }

        public static bool GetIsExistTile(Vector3Int tilePos, Custom3DHexTileMap custom3DHexTileMap)
        {
            return custom3DHexTileMap.hexTileObjectBible.ContainsKey(tilePos);
        }

        public static Vector3 GetCellCenterWorld(Vector3Int tilePos, Custom3DHexTileMap custom3DHexTileMap)
        {
            Vector3 pos = Vector3.zero;
            if (custom3DHexTileMap.hexTileObjectBible.TryGetValue(tilePos, out HexTileObject hexTileObject))
            {
                pos = hexTileObject.transform.position;
            }
            return pos;
        }

        public static void SetTile(Vector3Int tilePos, Color color, Custom3DHexTileMap custom3DHexTileMap)
        {
            if (custom3DHexTileMap.hexTileObjectBible.TryGetValue(tilePos, out HexTileObject hexTileObject))
            {
                hexTileObject.TileColor = color;
            }
        }


        /// <summary>
        /// 해당타일위치의 근처타일이 존재하는 여부를 가져오는 함수
        /// </summary>
        /// <param name="sidePosArray">위, 아래, 좌, 우 타일위치</param>
        /// <returns>위, 아래, 좌, 우  타일존재여부반환</returns>
        public static bool[] GetSideHexTileIsHas(Vector3Int[] sidePosArray, Custom3DHexTileMap custom3DHexTileMap)
        {
            if (custom3DHexTileMap == null)
            {
                bool[] sideIsHasFalseArray = { false, false, false, false, false, false };
                return sideIsHasFalseArray;
            }


            bool rightUpTile = custom3DHexTileMap.HasTile(sidePosArray[0]);
            bool leftDownTile = custom3DHexTileMap.HasTile(sidePosArray[1]);

            bool rightTile = custom3DHexTileMap.HasTile(sidePosArray[2]);
            bool leftTile = custom3DHexTileMap.HasTile(sidePosArray[3]);

            bool rightDownTile = custom3DHexTileMap.HasTile(sidePosArray[4]);
            bool leftUpTile = custom3DHexTileMap.HasTile(sidePosArray[5]);

            bool[] sideIsHasArray = { rightUpTile, leftDownTile, rightTile, leftTile, rightDownTile, leftUpTile };

            return sideIsHasArray;
        }

        /// <summary>
        /// 월드위치의 타일맵셀위치로 반환
        /// </summary>
        /// <param name="pos">월드위치</param>
        /// <param name="custom3DHexTileMap">커스텀3D헥스타일맵</param>
        /// <returns>커스텀3D헥스타일맵 셀위치</returns>
        public static Vector3Int GetWorldToCell(Vector3 pos, Custom3DHexTileMap custom3DHexTileMap)
        {
            //Vector3Int cellLocalPos = custom3DHexTileMap.LocalToCell(pos);//로컬 투 셀 위치로 변환//사용안함                    
            //Debug.Log("셀 위치:" + cellPos + ",로컬셀 위치:" + cellLocalPos + ",마우스위치:" + pos);

            //이거는 좀 문제가 있어보임//여기를 어덯게할지가 관건이긴한데
            return custom3DHexTileMap.WorldToCell(pos); // custom3DHexTileMap.WorldToCell(pos);//월드 투 셀 위치로 변환                
        }

        //축좌표 체크
        //큐브좌표
        //큐브좌표를 사용하다가 축좌표를 사용하면 Z를 사용하지말기

        //축좌표를 사용하다가 큐브좌표가 필요하면 Z를 얻는 알고리즘 작동(z = -xy)
        //이해가 안되는데//s = -posX-posY 계산해보기

        //posX = posX
        //posY = posY
        //s = z

        //내가 지금 오프셋좌표인데//일단구함 최소최대치만 도움됫음

        /// <summary>
        /// 타일위치끼리의 거리를 가져오는 함수
        /// </summary>
        /// <param name="startPos">시작 타일위치</param>
        /// <param name="endPos">끝 타일위치</param>
        /// <returns>거리</returns>
        public static int GetTileDistance(Vector3Int startPos, Vector3Int endPos)
        {
            Vector3Int dir = endPos - startPos;//방향구하기           
            //절대값 처리
            int x = Mathf.Abs(dir.x);
            int y = Mathf.Abs(dir.y);
            // 홀수 행에서 시작하거나 음의 posX 방향으로 이동하는 경우 특별한 경우//xor
            x = (dir.x < 0) ^ ((startPos.y & 1) == 1) ? Mathf.Max(0, x - (y + 1) / 2) : Mathf.Max(0, x - (y) / 2);
            return x + y;
        }

        private static List<Vector3Int> nearRangePosList = new List<Vector3Int>(32);

        /// <summary>
        /// 일정거리의 사이드 타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">선택된 타일위치</param>
        /// <param name="distance">거리</param>
        /// <param name="custom3DHexTileMap">타일맵</param>
        /// <returns>일정거리의 타일위치들</returns>
        public static List<Vector3Int> GetNearDistancePos(Vector3Int tilePos, int distance, Custom3DHexTileMap custom3DHexTileMap)
        {
            nearRangePosList.Clear();
            int cashAxisPos = -distance;
            switch (custom3DHexTileMap.hexTileType)
            {
                case HexTileMatrix.HexTileType.FlatTop:
                    bool isSelectOddXPos = (tilePos.x & 1) == 1;

                    for (int posX = -distance; posX <= distance; posX++)
                    {
                        int minYPosRange = Mathf.Max(-distance, -cashAxisPos - distance);
                        int maxYPosRange = Mathf.Min(distance, -cashAxisPos + distance);

                        int yValue = 0;

                        bool checkLeftAndRight = Mathf.Abs(posX) == distance;

                        for (int posY = minYPosRange; posY <= maxYPosRange; posY++)
                        {
                            if (checkLeftAndRight || posY == minYPosRange || posY == maxYPosRange)
                            {
                                int differDistance = Mathf.Abs(posX) / 2;
                                int selectOddOffset = 0;
                                if (isSelectOddXPos)
                                {
                                    //짝수번째 y타일들의 x를 +1
                                    //지금선택한타일이 홀수이니까 
                                    //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                    selectOddOffset = (posX & 1) == 1 ? 1 : 0;
                                }

                                int tempYPos = (yValue - distance) + differDistance + selectOddOffset;

                                Vector3Int finalTilePos = tilePos + new Vector3Int(posX, tempYPos);
                                nearRangePosList.Add(finalTilePos);
                            }

                            //더하고
                            yValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
                case HexTileMatrix.HexTileType.PointyTop:
                    bool isSelectOddYPos = (tilePos.y & 1) == 1;
                    //y축 먼저처리//변하지않으니                    
                    for (int posY = -distance; posY <= distance; posY++)
                    {
                        //x의 최소 최대치를 맞추는 역할
                        int minXPosRange = Mathf.Max(-distance, -cashAxisPos - distance);
                        int maxXPosRange = Mathf.Min(distance, -cashAxisPos + distance);

                        //원점기준으로 X구하기
                        int xValue = 0;

                        //거리만 구하는거니 //반복문으로 다 구할필..요는 없긴한데 귀찮
                        //어차피 맨상단과 맨하단은 다 구해야됨=>반복문하기
                        bool checkTopAndBottom = Mathf.Abs(posY) == distance;

                        for (int posX = minXPosRange; posX <= maxXPosRange; posX++)
                        {
                            //위아래이고//양옆끝단일시에만 작동
                            if (checkTopAndBottom || posX == minXPosRange || posX == maxXPosRange)
                            {
                                int differDistance = Mathf.Abs(posY) / 2;

                                int selectOddOffset = 0;
                                if (isSelectOddYPos)
                                {
                                    selectOddOffset = (posY & 1) == 1 ? 1 : 0;
                                }

                                int tempXPos = (xValue - distance) + differDistance + selectOddOffset;

                                Vector3Int finalTilePos = tilePos + new Vector3Int(tempXPos, posY);
                                nearRangePosList.Add(finalTilePos);
                            }
                            xValue++;//더하고
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
            }

            return nearRangePosList;
        }


        /// <summary>
        /// 일정거리의 사이드 타일위치에 있는지 체크하는 함수
        /// </summary>
        /// <param name="tilePos">선택된 타일위치</param>
        /// <param name="checkTilePos">체크할 타일위치</param>
        /// <param name="distance">거리</param>
        /// <param name="custom3DHexTileMap">탕리맵</param>
        /// <returns>일정거리의 사이드타일위치에 있는지 여부</returns>
        public static bool CheckNearDistancePos(Vector3Int tilePos, Vector3Int checkTilePos, int distance, Custom3DHexTileMap custom3DHexTileMap)
        {   
            int cashAxisPos = -distance;
            switch (custom3DHexTileMap.hexTileType)
            {
                case HexTileMatrix.HexTileType.FlatTop:
                    bool isSelectOddXPos = (tilePos.x & 1) == 1;

                    for (int posX = -distance; posX <= distance; posX++)
                    {
                        int minYPosRange = Mathf.Max(-distance, -cashAxisPos - distance);
                        int maxYPosRange = Mathf.Min(distance, -cashAxisPos + distance);
                        int yValue = 0;//원점기준으로 y구하기
                        bool checkLeftAndRight = Mathf.Abs(posX) == distance;

                        for (int posY = minYPosRange; posY <= maxYPosRange; posY++)
                        {
                            if (checkLeftAndRight || posY == minYPosRange || posY == maxYPosRange)
                            {
                                int differDistance = Mathf.Abs(posX) / 2;
                                int selectOddOffset = 0;
                                if (isSelectOddXPos)
                                {
                                    //짝수번째 y타일들의 x를 +1
                                    //지금선택한타일이 홀수이니까 
                                    //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                    selectOddOffset = (posX & 1) == 1 ? 1 : 0;
                                }

                                int tempYPos = (yValue - distance) + differDistance + selectOddOffset;

                                Vector3Int finalTilePos = tilePos + new Vector3Int(posX, tempYPos);
                                if (finalTilePos == checkTilePos)
                                {
                                    return true;
                                }
                            }

                            //더하고
                            yValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
                case HexTileMatrix.HexTileType.PointyTop:
                    bool isSelectOddYPos = (tilePos.y & 1) == 1;
                    //y축 먼저처리//변하지않으니                    
                    for (int posY = -distance; posY <= distance; posY++)
                    {
                        //x의 최소 최대치를 맞추는 역할
                        int minXPosRange = Mathf.Max(-distance, -cashAxisPos - distance);
                        int maxXPosRange = Mathf.Min(distance, -cashAxisPos + distance);
                        int xValue = 0;//원점기준으로 X구하기

                        //거리만 구하는거니 //반복문으로 다 구할필..요는 없긴한데 귀찮
                        //어차피 맨상단과 맨하단은 다 구해야됨=>반복문하기
                        bool checkTopAndBottom = Mathf.Abs(posY) == distance;

                        for (int posX = minXPosRange; posX <= maxXPosRange; posX++)
                        {
                            //위아래이고//양옆끝단일시에만 작동
                            if (checkTopAndBottom || posX == minXPosRange || posX == maxXPosRange)
                            {
                                int differDistance = Mathf.Abs(posY) / 2;

                                int selectOddOffset = 0;
                                if (isSelectOddYPos)
                                {
                                    selectOddOffset = (posY & 1) == 1 ? 1 : 0;
                                }

                                int tempXPos = (xValue - distance) + differDistance + selectOddOffset;

                                Vector3Int finalTilePos = tilePos + new Vector3Int(tempXPos, posY);
                                if (finalTilePos == checkTilePos)
                                {
                                    return true;
                                }
                            }
                            xValue++;//더하고
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 일정범위내의 모든 타일위치를 가져오는 함수
        /// </summary>
        /// <param name="tilePos">선택된 타일위치</param>
        /// <param name="range">범위</param>
        /// <param name="custom3DHexTileMap">타일맵</param>
        /// <returns>일정범위내의 타일위치들</returns>
        public static List<Vector3Int> GetNearRangePos(Vector3Int tilePos, int range, Custom3DHexTileMap custom3DHexTileMap)
        {
            //된다 십..//20230501//약 일주일 소모한듯

            // 타일의 개행 기준 selectOffset 값 계산
            //  for each qmin ≤ posX ≤ qmax
            //for each max(rmin, -posX - smax) ≤ posY ≤ min(rmax, -posX - smin) :
            //results.append(Hex(posX, posY))


            //이해가 안되는데//s = -posX-posY 계산해보기
            //posX = posX
            //posY = posY
            //s = z

            //-posX -r의 max?
            //qr = max
            //posX = differDistance
            //posY = ? => r은 자기자신이라 무시

            //그럼 smax와 smin은 posX == range인데
            //+- ㅇㄷ?
            //smax = differDistance
            //smin = -differDistance
            nearRangePosList.Clear();            
            int cashAxisPos = -range;//최소 최대치 정하기 위한 요소
            switch (custom3DHexTileMap.hexTileType)
            {
                case HexTileMatrix.HexTileType.FlatTop:
                    bool isSelectOddXPos = (tilePos.x & 1) == 1;

                    for (int posX = -range; posX <= range; posX++)
                    {
                        int minYPosRange = Mathf.Max(-range, -cashAxisPos - range);
                        int maxYPosRange = Mathf.Min(range, -cashAxisPos + range);

                        int yValue = 0;
                        for (int posY = minYPosRange; posY <= maxYPosRange; posY++)
                        {
                            int differDistance = Mathf.Abs(posX) / 2;
                            int selectOddOffset = 0;
                            if (isSelectOddXPos)
                            {
                                //짝수번째 y타일들의 x를 +1
                                //지금선택한타일이 홀수이니까 
                                //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                selectOddOffset = (posX & 1) == 1 ? 1 : 0;
                            }

                            int tempYPos = (yValue - range) + differDistance + selectOddOffset;

                            Vector3Int finalTilePos = tilePos + new Vector3Int(posX, tempYPos);
                            nearRangePosList.Add(finalTilePos);

                            //더하고
                            yValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
                case HexTileMatrix.HexTileType.PointyTop:
                    bool isSelectOddYPos = (tilePos.y & 1) == 1;//오프셋은 이게 맞는데//뭐야 &였는데 언제 바뀐거
                    //y축 먼저처리//변하지않으니                    
                    for (int posY = -range; posY <= range; posY++)
                    {
                        //x의 최소 최대치를 맞추는 역할
                        int minXPosRange = Mathf.Max(-range, -cashAxisPos - range);
                        int maxXPosRange = Mathf.Min(range, -cashAxisPos + range);

                        //원점기준으로 X구하기
                        int xValue = 0;
                        for (int posX = minXPosRange; posX <= maxXPosRange; posX++)
                        {
                            //자체 오프셋으로 하기
                            //1. 원점기준 => xValue
                            //2. y축이 차이나는 거리 나누기 2만큼 더하기
                            //3. 선택한 타일의 Y축이 홀수이면 계산되는 짝수번째 y타일들의 x를 +1

                            int differDistance = Mathf.Abs(posY) / 2;//차이나는거리 체크

                            //선택한 타일이 홀수일시
                            int selectOddOffset = 0;
                            if (isSelectOddYPos)
                            {
                                //짝수번째 y타일들의 x를 +1
                                //지금선택한타일이 홀수이니까 
                                //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                selectOddOffset = (posY & 1) == 1 ? 1 : 0;
                            }

                            int tempXPos = (xValue - range) + differDistance + selectOddOffset;

                            Vector3Int finalTilePos = tilePos + new Vector3Int(tempXPos, posY);
                            nearRangePosList.Add(finalTilePos);

                            //더하고
                            xValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
            }

            return nearRangePosList;
        }
        
        /// <summary>
        /// 일정범위내의 모든 타일위치에 체크하는 함수
        /// </summary>
        /// <param name="tilePos">선택된 타일위치</param>
        /// <param name="checkTilePos">체크할 타일위치</param>
        /// <param name="range">범위</param>
        /// <returns>일정범위내의 타일위치에 존재하는지 여부</returns>
        public static bool CheckNearRangePos(Vector3Int tilePos, Vector3Int checkTilePos, int range, Custom3DHexTileMap custom3DHexTileMap)
        {   
            int cashAxisPos = -range;//최소 최대치 정하기 위한 요소
            switch (custom3DHexTileMap.hexTileType)
            {
                case HexTileMatrix.HexTileType.FlatTop:
                    bool isSelectOddXPos = (tilePos.x & 1) == 1;

                    for (int posX = -range; posX <= range; posX++)
                    {
                        int minYPosRange = Mathf.Max(-range, -cashAxisPos - range);
                        int maxYPosRange = Mathf.Min(range, -cashAxisPos + range);

                        int yValue = 0;
                        for (int posY = minYPosRange; posY <= maxYPosRange; posY++)
                        {
                            int differDistance = Mathf.Abs(posX) / 2;
                            int selectOddOffset = 0;
                            if (isSelectOddXPos)
                            {
                                //짝수번째 y타일들의 x를 +1
                                //지금선택한타일이 홀수이니까 
                                //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                selectOddOffset = (posX & 1) == 1 ? 1 : 0;
                            }

                            int tempYPos = (yValue - range) + differDistance + selectOddOffset;

                            Vector3Int finalTilePos = tilePos + new Vector3Int(posX, tempYPos);

                            if (finalTilePos == checkTilePos)
                            {
                                return true;
                            }

                            //더하고
                            yValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
                case HexTileMatrix.HexTileType.PointyTop:
                    bool isSelectOddYPos = (tilePos.y & 1) == 1;//오프셋은 이게 맞는데//뭐야 &였는데 언제 바뀐거
                    //y축 먼저처리//변하지않으니                    
                    for (int posY = -range; posY <= range; posY++)
                    {
                        //x의 최소 최대치를 맞추는 역할
                        int minXPosRange = Mathf.Max(-range, -cashAxisPos - range);
                        int maxXPosRange = Mathf.Min(range, -cashAxisPos + range);

                        //원점기준으로 X구하기
                        int xValue = 0;
                        for (int posX = minXPosRange; posX <= maxXPosRange; posX++)
                        {
                            //자체 오프셋으로 하기
                            //1. 원점기준 => xValue
                            //2. y축이 차이나는 거리 나누기 2만큼 더하기
                            //3. 선택한 타일의 Y축이 홀수이면 계산되는 짝수번째 y타일들의 x를 +1

                            int differDistance = Mathf.Abs(posY) / 2;//차이나는거리 체크

                            //선택한 타일이 홀수일시
                            int selectOddOffset = 0;
                            if (isSelectOddYPos)
                            {
                                //짝수번째 y타일들의 x를 +1
                                //지금선택한타일이 홀수이니까 
                                //반복문으로 작동되고 있는 PosY의 홀수값이 짝수계열
                                selectOddOffset = (posY & 1) == 1 ? 1 : 0;
                            }

                            int tempXPos = (xValue - range) + differDistance + selectOddOffset;

                            Vector3Int finalTilePos = tilePos + new Vector3Int(tempXPos, posY);
                            if (finalTilePos == checkTilePos)
                            {
                                return true;
                            }

                            //더하고
                            xValue++;
                        }
                        cashAxisPos++;//축이동
                    }
                    break;
            }

            return false;
        }
#endif

        /// <summary>
        /// 월드위치의 타일맵셀위치로 반환
        /// </summary>
        /// <param name="pos">월드위치</param>
        /// <param name="tilemap">타일맵</param>
        /// <returns>타일맵 셀위치</returns>
        public static Vector3Int GetWorldToCell(Vector2 pos, Tilemap tilemap)
        {
            //Vector3Int cellLocalPos = custom3DHexTileMap.LocalToCell(pos);//로컬 투 셀 위치로 변환//사용안함                    
            //Debug.Log("셀 위치:" + cellPos + ",로컬셀 위치:" + cellLocalPos + ",마우스위치:" + pos);        
            return tilemap.WorldToCell(pos);//월드 투 셀 위치로 변환                
        }

        //셀 위치:(-1, 3, 0),로컬셀 위치:(-1, 3, 0),마우스위치:(-0.4, 3.2, 0.0)
        //셀 위치:(-1, 3, 0),로컬셀 위치:(-1, 3, 0),마우스위치:(-0.6, 3.4, 0.0)
        //셀 위치:(2, 4, 0),로컬셀 위치:(-2, 1, 0),마우스위치:(-1.2, 1.6, 0.0)
        //셀 위치:(-1, 3, 0),로컬셀 위치:(-5, 0, 0),마우스위치:(-4.2, 0.9, 0.0)
        //월드포지션으로 하느게 맞아보임
        //로컬테스트 =>작동안됨      

        /// <summary>
        /// 특정타일을 위치에 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="targetTile">타일</param>
        public static void SetTile(Vector3Int pos, TileBase targetTile, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetTile(pos, targetTile);
        }

        /// <summary>
        /// 타일맵상에 존재여부를 체크후 특정타일을 위치에 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="targetTile">타일</param>
        /// <param name="targetTileMap">확인할 타일맵</param>
        /// <param name="isExist">확인할 존재여부</param>
        public static void SetTile(Vector3Int pos, TileBase targetTile, Tilemap originTilemap, Tilemap targetTileMap, bool isExist)
        {
            if (targetTileMap.HasTile(pos) == isExist)//존재여부 체크
            {
                originTilemap.SetTile(pos, targetTile);
            }
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="color">색깔</param>
        public static void SetTile(Vector3Int pos, Color color, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, TileFlags.None);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="alpha">알파값</param>
        public static void SetTile(Vector3Int pos, float alpha, Tilemap tilemap)
        {
            Color color = tilemap.GetColor(pos);
            color.a = alpha;
            //Debug.Log(custom3DHexTileMap.GetTile(cellPos));
            //Debug.Log(custom3DHexTileMap.GetTileFlags(cellPos));
            //타일플래그 설정
            //custom3DHexTileMap.SetTileFlags(cellPos, TileFlags.LockTransform);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치 색깔을 세팅해주는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="red">레드값</param>
        /// <param name="green">그린값</param>
        /// <param name="blue">블루값</param>
        /// <param name="alpha">알파값</param>
        public static void SetTile(Vector3Int pos, float red, float green, float blue, float alpha, Tilemap tilemap)
        {
            Color color = tilemap.GetColor(pos);
            color.r = red;
            color.g = green;
            color.b = blue;
            color.a = alpha;

            //Debug.Log(custom3DHexTileMap.GetTile(cellPos));
            //Debug.Log(custom3DHexTileMap.GetTileFlags(cellPos));
            //타일플래그 설정
            //custom3DHexTileMap.SetTileFlags(cellPos, TileFlags.LockTransform);
            tilemap.SetColor(pos, color);
        }

        /// <summary>
        /// 해당위치의 타일작동조건설정을 세팅하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <param name="tileFlags">타일플래그</param>
        public static void SetTile(Vector3Int pos, TileFlags tileFlags, Tilemap tilemap)
        {
            tilemap.SetTileFlags(pos, tileFlags);
        }

        /// <summary>
        /// 해당위치의 타일을 가져오는함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>위치의 타일</returns>
        public static TileBase GetTile(Vector3Int pos, Tilemap tilemap)
        {
            return tilemap.GetTile(pos);
        }

        /// <summary>
        /// 박스형태로 채워주기 함수
        /// </summary>
        /// <param name="targetStartPos">시작위치</param>
        /// <param name="targetEndPos">마지막위치</param>
        /// <param name="tile">배치할 타일</param>
        public static void BoxFill(Vector2 targetStartPos, Vector2 targetEndPos, TileBase tile, Tilemap tilemap)
        {
            Vector3Int startPos = tilemap.WorldToCell(targetStartPos);
            Vector3Int endPos = tilemap.WorldToCell(targetEndPos);
            
            //방향처리
            var xDir = startPos.x < endPos.x ? 1 : -1;
            var yDir = startPos.y < endPos.y ? 1 : -1;
            
            //놓을 타일수
            int xcolumn = 1 + Mathf.Abs(startPos.x - endPos.x);
            int ycolumn = 1 + Mathf.Abs(startPos.y - endPos.y);
            
            //그리기시작
            for (var x = 0; x < xcolumn; x++)
            {
                for (var y = 0; y < ycolumn; y++)
                {
                    var tilePos = startPos + new Vector3Int(x * xDir, y * yDir, 0);
                    tilemap.SetTile(tilePos, tile);
                }
            }
        }

        /// <summary>
        /// 박스형태로 채워주기 함수
        /// </summary>
        /// <param name="targetStartPos">시작위치</param>
        /// <param name="targetEndPos">마지막위치</param>
        /// <param name="tile">배치할 타일</param>
        public static void BoxFill(Vector3Int targetStartPos, Vector3Int targetEndPos, TileBase tile, Tilemap tilemap)
        {
            //Determine directions on X and Y axis
            //방향처리
            var xDir = targetStartPos.x < targetEndPos.x ? 1 : -1;
            var yDir = targetStartPos.y < targetEndPos.y ? 1 : -1;
            //How many tiles on each axis?
            //놓을 타일수
            int xcolumn = 1 + Mathf.Abs(targetStartPos.x - targetEndPos.x);
            int ycolumn = 1 + Mathf.Abs(targetStartPos.y - targetEndPos.y);
            //Start painting
            //놓기시작
            for (var x = 0; x < xcolumn; x++)
            {
                for (var y = 0; y < ycolumn; y++)
                {
                    var tilePos = targetStartPos + new Vector3Int(x * xDir, y * yDir, 0);
                    tilemap.SetTile(tilePos, tile);
                }
            }
        }

        /// <summary>
        /// 그리드에 맞게 좌표를 스냅하는 함수
        /// </summary>
        /// <param name="position">원본 좌표</param>
        /// <returns>그리드에 맞게 스냅된 좌표</returns>
        public static Vector2 SnapPosToGridPos(Vector2 position, Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(position);
            position = tilemap.GetCellCenterWorld(cellPos);
            return position;
        }

        /// <summary>
        /// 특정영역파괴
        /// </summary>
        /// <param name="radius">반경</param>
        /// <param name="position">위치</param>
        public static void DestroyArea(float radius, Vector2 position, Tilemap tilemap)
        {
            int radiusInt = Mathf.RoundToInt(radius) + 1;//1개의 크기를 더확인해줌//위치확인용

            for (int i = -radiusInt; i <= radiusInt; i++)
            {
                for (int j = -radiusInt; j <= radiusInt; j++)
                {
                    //새위치를 지정
                    Vector2 newPos = new Vector2(position.x + i, position.y + j);

                    //해당위치에서부터 거리체크
                    if (Vector2.Distance(newPos, position) <= radius)
                    //if (Vector3.Distance(targetDestroyPos, position) - 0.001f <= radius) 
                    {
                        //파괴로직
                        Vector3Int targetDestroyPos = tilemap.WorldToCell(newPos);
                        tilemap.SetTile(targetDestroyPos, null);
                        //추가처리
                    }
                }
            }
        }

        //https://playground10.tistory.com/62
        //DDA 알고리즘
        public static void DDALine(Vector2 targetStartPos, Vector2 targetEndPos, Tile tile, Tilemap tilemap, bool fillGaps)
        {

            Vector3Int startPos = tilemap.WorldToCell(targetStartPos);
            Vector3Int endPos = tilemap.WorldToCell(targetEndPos);


            foreach (Vector3Int point in GetPointsOnLine(startPos, endPos, fillGaps))
            {
                Vector3Int paintPos = new Vector3Int(point.x, point.y, point.z);
                tilemap.SetTile(paintPos, tile);
            }
            
            

            //GetPointsOnLine(startPos, endPos, lineBrush.fillGaps)
            ////초기값
            //Vector3Int startPos = custom3DHexTileMap.WorldToCell(targetStartPos);
            //Vector3Int endPos = custom3DHexTileMap.WorldToCell(targetEndPos);

            ////방향처리
            //var xDir = startPos.x < endPos.x ? 1 : -1;
            //var yDir = startPos.y < endPos.y ? 1 : -1;

            ////놓을 타일수
            //int xcolumn = 1 + Mathf.Abs(startPos.x - endPos.x);
            //int ycolumn = 1 + Mathf.Abs(startPos.y - endPos.y);


            //int x = xcolumn;
            //int y = ycolumn;
            //int w = endPos.x - startPos.x;
            //int h = endPos.y - startPos.y;
            //int f = 2 * h - w;

            ////각 판별식 공식
            //int dF1 = 2 * h;
            //int dF2 = 2 * (h - w);

            //for (x = startPos.x; x <= endPos.x; x++)
            //{
            //    //점 그리기
            //    custom3DHexTileMap.SetTile(new Vector3Int(x, y), tile);

            //    if (f < 0)
            //    {
            //        //0보다 작으면 그에 맞는 공식으로 판별식 갱신, y값은 그대로 
            //        f += dF1;
            //    }
            //    else
            //    {
            //        //0보다 크거나 같으면
            //        //그에 맞는 공식으로 반별식 갱신, y값은 증가
            //        ++y;
            //        f += dF2;
            //    }
            //}
        }
       
        /// <summary>
        /// Enumerates all the points between the start and end position which are
        /// linked diagonally or orthogonally.
        /// </summary>
        /// <param name="startPos">Start position of the line.</param>
        /// <param name="endPos">End position of the line.</param>
        /// <param name="fillGaps">Fills any gaps between the start and end position so that
        /// all points are linked only orthogonally.</param>
        /// <returns>Returns an IEnumerable which enumerates all the points between the start and end position which are
        /// linked diagonally or orthogonally.</returns>
        public static IEnumerable<Vector3Int> GetPointsOnLine(Vector3Int startPos, Vector3Int endPos, bool fillGaps)
        {
            var points = GetPointsOnLine(startPos, endPos);
            if (fillGaps)
            {
                var rise = endPos.y - startPos.y;
                var run = endPos.x - startPos.x;

                if (rise != 0 || run != 0)
                {
                    var extraStart = startPos;
                    var extraEnd = endPos;


                    if (Mathf.Abs(rise) >= Mathf.Abs(run))
                    {
                        // up
                        if (rise > 0)
                        {
                            extraStart.y += 1;
                            extraEnd.y += 1;
                        }
                        // down
                        else // rise < 0
                        {

                            extraStart.y -= 1;
                            extraEnd.y -= 1;
                        }
                    }
                    else // Mathf.Abs(rise) < Mathf.Abs(run)
                    {

                        // right
                        if (run > 0)
                        {
                            extraStart.x += 1;
                            extraEnd.x += 1;
                        }
                        // left
                        else // run < 0
                        {
                            extraStart.x -= 1;
                            extraEnd.x -= 1;
                        }
                    }

                    var extraPoints = GetPointsOnLine(extraStart, extraEnd);
                    extraPoints = extraPoints.Except(new[] { extraEnd });
                    points = points.Union(extraPoints);
                }

            }

            return points;
        }

        /// <summary>
        /// Gets an enumerable for all the cells directly between two points
        /// http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
        /// </summary>
        /// <param name="p1">A starting point of a line</param>
        /// <param name="p2">An ending point of a line</param>
        /// <returns>Gets an enumerable for all the cells directly between two points</returns>
        private static IEnumerable<Vector3Int> GetPointsOnLine(Vector3Int p1, Vector3Int p2)
        {
            int x0 = p1.x;
            int y0 = p1.y;
            int x1 = p2.x;
            int y1 = p2.y;

            bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Mathf.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Vector3Int((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        /// <summary>
        /// 해당위치에 타일이 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>존재여부</returns>
        public static bool GetIsExistTile(Vector3 pos, Tilemap tilemap)
        {
            Vector3Int cellPos = tilemap.WorldToCell(pos);//월드 투 셀 위치로 변환    
            return tilemap.HasTile(cellPos);
            //TileBase target = custom3DHexTileMap.GetTile(cellPos);
            //Debug.Log(target);        
        }

        /// <summary>
        /// 해당위치에 타일이 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="pos">위치</param>
        /// <returns>존재여부</returns>
        public static bool GetIsExistTile(Vector3Int pos, Tilemap tilemap)
        {
            return tilemap.HasTile(pos);
            //TileBase target = custom3DHexTileMap.GetTile(cellPos);
            //Debug.Log(target);        
        }

        /// <summary>
        /// 타일맵 새로고침함수
        /// </summary>
        public static void RefreshTileMap(Tilemap tilemap)
        {
            tilemap.RefreshAllTiles();
        }

        /// <summary>
        /// 타일맵 새로고침함수
        /// </summary>
        /// <param name="pos">위치</param>
        public static void RefreshTileMap(Vector3Int pos, Tilemap tilemap)
        {
            tilemap.RefreshTile(pos);
        }

        /// <summary>
        /// 타일맵 리셋시키는 함수
        /// </summary>
        public static void ResetTilemap(Tilemap tilemap)
        {
            tilemap.ClearAllTiles();
        }



        //무브모듈//20220601 옳김
        //움직임에 관련해서 모든걸 여기서 관할함    
        //물리이용하는건 여기서 안다룬다
        //20221008
        //움직임모듈에서 가져온 몇몇개들에서 굳이 잘게 쪼개논것보다 뭉쳐놓는게 좋아서 잘게쪼개진건 주석처리

        //x좌표를 움직인다. 
        //대입할때 + -로 오른족 왼쪽으로 움직이게 할수 있다,
        //public static void MoveXPos(Transform target, float speed)
        //{
        //    target.position += Vector3.right * speed;
        //    target.Translate(Vector3.right * speed);
        //}
        //public static void MoveXPos(Transform target, float speed, float time)
        //{
        //    //target.position += Vector3.right * speed * time;
        //    target.Translate(Vector3.right * speed * time);
        //}

        ////y좌표를 움직인다. 
        ////대입할때 + -로 위 아래로 움직이게 할수 있다,
        //public static void MoveYPos(Transform target, float speed)
        //{
        //    //target.position += Vector3.up * speed;
        //    target.Translate(Vector3.up * speed);
        //}
        //public static void MoveYPos(Transform target, float speed, float time)
        //{
        //    //target.position += Vector3.up * speed * time;
        //    target.Translate(Vector3.up * speed * time);
        //}

        //z좌표를 움직인다. 
        //대입할때 + -로 forward, backword로 움직이게 할수 있다,
        //public static void MoveZPos(Transform target, float speed)
        //{
        //    //target.position += Vector3.forward * speed;
        //    target.Translate(Vector3.forward * speed);
        //}
        //public static void MoveZPos(Transform target, float speed, float time)
        //{
        //    target.position += Vector3.forward * speed * time;
        //    target.Translate(Vector3.forward * speed * time);
        //}

        //XY좌표를 움직이게 한다.
        //XY를 같이 움직이게할려면 이걸로 하는게 좋다
        //public static void MoveXYPos(Transform target, float Xspeed, float Yspeed)
        //{
        //    target.SetPositionAndRotation(target.position += new Vector3(Xspeed, Yspeed, 0), Quaternion.identity);
        //}

        //public static void MoveXYPos(Transform target, float Xspeed, float Yspeed, float time)
        //{
        //    target.SetPositionAndRotation(target.position += new Vector3(Xspeed * time, Yspeed * time, 0), Quaternion.identity);
        //}


        //XYZ좌료를 움직이게 한다.
        //XYZ를 같이 움직이게할려면 이걸로 하는게 좋다
        //public static void MoveXYZPos(Transform target, float Xspeed, float Yspeed, float Zspeed)
        //{
        //    target.SetPositionAndRotation(target.position += new Vector3(Xspeed, Yspeed, Zspeed), Quaternion.identity);
        //}
        //public static void MoveXYZPos(Transform target, float Xspeed, float Yspeed, float Zspeed, float time)
        //{
        //    target.SetPositionAndRotation(target.position += new Vector3(Xspeed * time, Yspeed * time, Zspeed * time), Quaternion.identity);
        //}

        //20190823 새로추가한 함수
        //월드트랜스폼 무브
        //public static void MoveLerpXYPos(Transform target, float XPos, float YPos, float moveSpeed)
        //{
        //    //target.position = Vector3.Slerp(target.position, target.position += new Vector3(XPos, YPos, 0), moveSpeed * Time.deltaTime);
        //    target.position = Vector2.Lerp(target.position, target.position += new Vector3(XPos, YPos, 0), moveSpeed * Time.deltaTime);//문제없음
        //}
        ////로컬트랜스폼 무브
        //public static void MoveLerpYPos(Transform target, float YPos, float moveSpeed)
        //{
        //    //target.position = Vector3.Slerp(target.position, target.position += new Vector3(XPos, YPos, 0), moveSpeed * Time.deltaTime);
        //    //target.position = Vector2.Lerp(target.position, target.position += new Vector3(0, YPos, 0), moveSpeed * Time.deltaTime);//문제없음
        //    target.position = Vector2.Lerp(target.position, target.position += target.up * YPos, moveSpeed * Time.deltaTime);//문제없음
        //}
        //public static void MoveLerpXPos(Transform target, float XPos, float moveSpeed)
        //{
        //    //target.position = Vector3.Slerp(target.position, target.position += new Vector3(XPos, YPos, 0), moveSpeed * Time.deltaTime);
        //    //target.position = Vector2.Lerp(target.position, target.position += new Vector3(0, YPos, 0), moveSpeed * Time.deltaTime);//문제없음
        //    target.position = Vector2.Lerp(target.position, target.position += target.right * XPos, moveSpeed * Time.deltaTime);//문제없음
        //}

        public static void MoveLerp(Transform target, Vector2 targetPos, float moveSpeed)
        {
            target.position = Vector2.Lerp(target.position, targetPos, moveSpeed);
        }
        public static Vector2 GetMoveLerp(Vector2 target, Vector2 targetPos, float moveSpeed)
        {
            return Vector3.Lerp(target, targetPos, moveSpeed);
        }
        public static void MoveSmoothDamp(Transform target, Vector2 targetPos, ref Vector2 refSpeed, float moveSpeed)
        {   
            target.position = Vector2.SmoothDamp(target.position, targetPos, ref refSpeed, moveSpeed);
        }

        //회전모듈
        //회전에 관련해서 모든걸 여기서 관할함

        //X축을 기준으로 돌린다.
        public static void RotationX(Transform target, float speed)
        {
            target.Rotate(speed * Time.deltaTime, 0, 0);
        }
        public static void RotationX(Transform target, float speed, float time)
        {
            target.Rotate(speed * time * Time.deltaTime, 0, 0);
        }

        //Y축을 기준으로 돌린다.
        public static void RotationY(Transform target, float speed)
        {
            target.Rotate(0, speed * Time.deltaTime, 0);
        }
        public static void RotationY(Transform target, float speed, float time)
        {
            target.Rotate(0, speed * time * Time.deltaTime, 0);
        }

        //Z축을 기준으로 돌린다.
        public static void RotationZ(Transform target, float speed)
        {
            target.Rotate(0, 0, speed * Time.deltaTime);
        }
        public static void RotationZ(Transform target, float speed, float time)
        {
            target.Rotate(0, 0, speed * time * Time.deltaTime);
        }

        /// <summary>
        /// Slerp로 회전하는 함수
        /// </summary>
        /// <param name="rotateTarget">회전하는 오브젝트</param>
        /// <param name="lookTarget">봐야될 위치</param>
        /// <param name="rotateSpeed">회전속도</param>
        public static void SlerpRotationZ(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed)
        {
            Vector2 targetDir = lookTarget - rotateTarget.position;
            //float newangle = Mathf.Atan2(targetDir.x, targetDir.y) * Mathf.Rad2Deg;
            //if (newangle > 0 && newangle < 180)//정상작동
            //{
            //    newangle = newangle * -1;
            //}
            //else
            //{
            //    newangle = newangle * -1;
            //}
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            //Quaternion quaternion = Quaternion.AngleAxis(newangle - 90, Vector3.forward);
            rotateTarget.rotation = Quaternion.Slerp(rotateTarget.rotation, quaternion, rotateSpeed * Time.deltaTime);//slerp     
        }

        //20190922//신규 추가 회전
        public static void RotationZTarget(Transform rotateObject, Transform lookTarget, float rotateSpeed)
        {   
            //Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            //Debug.Log(lookRotation);
            //Vector3 tmpEuler = Quaternion.RotateTowards(rotateObject.rotation, lookRotation,speed * Time.deltaTime).eulerAngles;
            //Debug.Log(tmpEuler);
            Vector2 targetDir = lookTarget.position - rotateObject.position;
            float mouseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            float turretAngle = Quaternion.Angle(rotateObject.rotation, lookTarget.rotation);
            //if (mouseAngle > 0 && mouseAngle < 180)//정상작동
            //{
            //    mouseAngle = mouseAngle * -1;
            //}
            //else
            //{
            //    mouseAngle = mouseAngle * -1;
            //}
            //if (mouseAngle < 0)
            //{
            //    turretAngle *= -1;
            //}

            //float newangle3 = Quaternion.Dot(rotateObject.rotation, rotateTarget.rotation);
            //Debug.Log(newangle + "///" + newangle2 + "///" + newangle3);

            //테스트중
            //3가지를 고려해서 짜야함
            //마우스 각도  터렛각도 현재 각도와제일 가까운 값을향한 변수

            Debug.Log(mouseAngle + "///" + turretAngle);
            if ((int)turretAngle == (int)mouseAngle)
            {
                return;
            }
            float firResult = mouseAngle - turretAngle;
            float secResult = turretAngle - mouseAngle;

            if (firResult > secResult)
            {
                RotationZ(rotateObject, rotateSpeed, 1);
            }
            else
            {
                RotationZ(rotateObject, -rotateSpeed, 1);
            }

            //아직봉인




            //rotateObject.rotation = Quaternion.Euler(0, 0, rotateObject.rotation.z + -speed * Time.deltaTime);
            //Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            //rotateObject.rotation = Quaternion.Slerp(rotateObject.rotation, quaternion, speed * Time.deltaTime);//slerp
        }

        //20220914새로처리한 회전
        //slerp < Lerp가 더빠름//Slerp는 호를 일정하게//lerp는 선을 일정하게


        

        /// <summary>
        /// Slerp로 회전//구면 선형보간//호에서 일정하게 증가
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateSlerp(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed)
        {
            //Slerp회전
            Vector2 targetDir = lookTarget - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            rotateTarget.rotation = Quaternion.Slerp(rotateTarget.rotation, quaternion, rotateSpeed);//slerp     
        }

        /// <summary>
        /// Lerp로 회전//선형보간//선에서 일정하게 증가
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateLerp(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed)
        {
            //Lerp회전
            Vector2 targetDir = lookTarget - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            Quaternion quaternion = Quaternion.AngleAxis(newangle, Vector3.forward);
            rotateTarget.rotation = Quaternion.Lerp(rotateTarget.rotation, quaternion, rotateSpeed);
        }

        /// <summary>
        /// 일정한 속도로 회전
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        public static void RotateTurret(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed)
        {
            Vector2 targetDir = lookTarget - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90;
            float zAngle = Mathf.MoveTowardsAngle(rotateTarget.eulerAngles.z, newangle, rotateSpeed);//일정하게 움직임
            rotateTarget.rotation = Quaternion.Euler(0, 0, zAngle);
        }

        /// <summary>
        /// 일정한속도로 회전(내적)
        /// </summary>
        /// <param name="rotateTarget">회전할 타겟</param>        
        /// <param name="target">회전할 위치</param>
        /// /// <param name="rotateSpeed">회전속도</param>
        public static void RotateDot(Transform rotateTarget, Vector3 target, float rotateSpeed)
        {
            //방향구하기
            Vector3 dir = (target - rotateTarget.position).normalized;

            // 내적(dot)을 통해 각도를 구함. (Acos로 나온 각도는 방향을 알 수가 없음)
            Vector3 trUp = rotateTarget.up;

            //방향과 방향을 체크하여 정면인지 아닌지 체크
            float dot = Vector3.Dot(trUp, dir);//벡터의 내적은 스칼라(스케일의 어원)으로 나옴
            if (dot < 1.0f)//1이 정면임
            {
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                // 외적을 통해 각도의 방향을 판별.
                Vector3 cross = Vector3.Cross(trUp, dir);
                float tempSpeed = rotateSpeed * Time.deltaTime;
                // 외적 결과 값에 따라 각도 반영
                if (cross.z < 0)
                {
                    angle = rotateTarget.rotation.eulerAngles.z - Mathf.Min(10, angle) * tempSpeed;
                }
                else
                {
                    angle = rotateTarget.rotation.eulerAngles.z + Mathf.Min(10, angle) * tempSpeed;
                }

                rotateTarget.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// 앵글용 클램프//Mathf.Clamp는 각도로 괜찮지가 않음
        /// </summary>
        /// <param name="curAngle">현재 각도</param>
        /// <param name="min">최소각도</param>
        /// <param name="max">최대각도</param>
        /// <returns>반환각도</returns>
        public static float ClampAngle(float curAngle, float min, float max)
        {
            //0~360도 제한이 있음
            //- 각도로 들어갈시 360이 아닌 1로 돌아감
            //FloorToInt 소수점내림
            //-360 ~ +360
            //사이에서 처리해야됨
            float startAngle = (min + max) * 0.5f - PI;
            float floor = Mathf.FloorToInt((curAngle - startAngle) / (PI * 2)) * (PI * 2);
            floor = Mathf.Clamp(curAngle, min + floor, max + floor);
            return floor;
        }

        /// <summary>
        /// 앵글용 클램프//Mathf.Clamp는 각도로 괜찮지가 않음
        /// </summary>
        /// <param name="curEulerAngles">현재 룰러각도</param>
        /// <param name="min">최소 룰러각도</param>
        /// <param name="max">최대 룰러각도</param>
        /// <returns>반환롤러각도</returns>
        public static Vector3 ClampAngle(Vector3 curEulerAngles, Vector3 min, Vector3 max)
        {
            float x = ClampAngle(curEulerAngles.x, min.x, max.x);
            float y = ClampAngle(curEulerAngles.y, min.y, max.y);
            float z = ClampAngle(curEulerAngles.z, min.z, max.z);
            return new Vector3(x, y, z);
        }

        private const float PI = 180;
        private const float addAngle = 90f;

        /// <summary>
        /// 일정한 속도로 제한된 회전
        /// </summary>
        /// <param name="rotateTarget">회전하는 트랜스폼</param>
        /// <param name="lookTarget">봐야할 타겟</param>
        /// <param name="rotateSpeed">회전 속도</param>
        /// <param name="min">최소각도-180 ~ 0</param>
        /// <param name="max">최대각도 0 ~ 180</param>
        public static void RotateLimit(Transform rotateTarget, Vector3 lookTarget, float rotateSpeed, float min, float max, AxisDirectionType axisDirectionType)
        {
            //회전자체는 잘되지만 -180~180 넘어가는 구간에서 회전이 맘에 안듬
            //몇몇구간이 반대로 돌아감=> 시야처리를 통해 작업

            Vector2 targetDir = lookTarget - rotateTarget.position;
            float newangle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;//0~360

            float zAngle = 0;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    zAngle = Mathf.Clamp(newangle, min, max);//x축 기준일시
                    break;
                case AxisDirectionType.Y:
                    zAngle = Mathf.Clamp(newangle, min + addAngle, max + addAngle);//Y축 기준일시
                    break;
            }

            //Debug.Log($"Target : {(int)newangle}, Result : {(int)zAngle}");
            zAngle = Mathf.MoveTowardsAngle(rotateTarget.eulerAngles.z, zAngle - 90, rotateSpeed);
            rotateTarget.localRotation = Quaternion.Euler(0, 0, zAngle);
        }

        /// <summary>
        /// 벡터회전용//테스트해야됨 
        /// </summary>
        /// <param name="angle">각도</param>
        /// <param name="u">벡터</param>
        /// <returns></returns>
        public static Vector3 Rotate(float angle, Vector3 u)
        {
            float x, y;
            float sinValue = Mathf.Sin(Mathf.Deg2Rad * -angle);
            float cosValue = Mathf.Cos(Mathf.Deg2Rad * -angle);

            x = cosValue + u.y * sinValue;
            y = sinValue + u.y * cosValue;

            return new Vector3(x, y, 0);
        }


        /// <summary>
        /// 우주차량유닛의 회전오브젝트들을 회전하게 만드는 함수
        /// </summary>
        /// <param name="rotateTurretObjectArray">회전할 대상들</param>
        /// <param name="targetPos">타겟위치</param>
        public static void RotateTurretArray(RotateTurretObject[] rotateTurretObjectArray, Vector2 targetPos)
        {
            for (int i = 0; i < rotateTurretObjectArray.Length; i++)
            {
                RotateTurretObject temp = rotateTurretObjectArray[i];
                float time = temp.rotateSpeed * Time.deltaTime;
                Transform tempTr = temp.targetObject;
                switch (temp.rotateType)
                {
                    case RotateType.Slerp:
                        RotateSlerp(tempTr, targetPos, time);
                        break;
                    case RotateType.Lerp:
                        RotateLerp(tempTr, targetPos, time);
                        break;
                    case RotateType.Turret:
                        RotateTurret(tempTr, targetPos, time);
                        break;
                }
            }
        }




        public static void RotateLeadCollision(Transform rotateTarget, Transform target, float rotateSpeed, float leadDistance)
        {
            //라티오는 거리를 말함//0되면 타격이 되야됨
            float ratio = Vector2.Distance(rotateTarget.position, target.position) - leadDistance;
            ratio = Mathf.Max(0, ratio);

            //미사일관련해서는 상대방 방향쪽만 확인해도 충분해보인다           
            //Vector3 missileDir = attackBox.transform.position;// + attackBox.transform.up * curRatio;//미사일 위치
            Vector3 targetDir = target.position + target.up * ratio;//타겟의 다음방향과 현재거리체크
            RotateDot(rotateTarget.transform, targetDir, rotateSpeed);
        }

        public static Vector2 GetLeadCollision(Transform rotateTarget, Transform target, float leadDistance)
        {
            //라티오는 거리를 말함//0되면 타격이 되야됨
            float ratio = Vector2.Distance(rotateTarget.position, target.position) - leadDistance;
            ratio = Mathf.Max(0, ratio);

            //미사일관련해서는 상대방 방향쪽만 확인해도 충분해보인다           
            //Vector3 missileDir = attackBox.transform.position;// + attackBox.transform.up * curRatio;//미사일 위치
            Vector3 targetDir = target.position + target.up * ratio;//타겟의 다음방향과 현재거리체크
            return targetDir;
        }










        ////리지드바디모듈
        public static void RigidBody2DMovePos(Rigidbody2D rb2d, Transform target)
        {
            //rb2d.MovePosition((Vector2)transform.position + (NormalizeVecter2(target.position)) * Time.deltaTime);
        }
        public static void RigidBody2DMovePos(Rigidbody2D rb2d, Transform target, float moveSpeed)
        {
            //rb2d.MovePosition((Vector2)target.position + (NormalizeVecter2(target.position))* moveSpeed * Time.deltaTime);
        }

        //노말라이즈를 해놓고 집어넣기
        public static void RigidBody2DMovePos(Rigidbody2D rb2d, Transform target, Vector2 _direction)
        {
            rb2d.MovePosition((Vector2)target.position + (_direction.normalized * Time.deltaTime));
        }

        public static void RigidBody2DMovePos(Rigidbody2D rb2d, Transform target, Vector2 _direction, float moveSpeed)
        {
            rb2d.MovePosition((Vector2)target.position + (_direction * moveSpeed * Time.deltaTime));
        }

        public static void RigidBody2DMoveRotate(Rigidbody2D rb2d, Transform rotateTarget, Transform lookTarget)
        {
            Vector3 direction = lookTarget.position - rotateTarget.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //rb2d.rotation = angle;
            rb2d.SetRotation(angle);
        }
        public static void RigidBody2DMoveRotate(Rigidbody2D rb2d, float _angle)
        {
            rb2d.rotation = _angle;
        }

        //아직 검증안됨
        public static void RotationObject(Rigidbody2D rb2d)
        {
            float angle = Mathf.Atan2(rb2d.velocity.y, rb2d.velocity.x) * Mathf.Rad2Deg + 90;
            rb2d.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        private static float tau = Mathf.PI * 2;
        /// <summary>
        /// Sin웨이브를 주는 
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="frequency"></param>
        /// <returns>값</returns>
        public static float SinWave(float amplitude, float frequency)
        {
            //Time.timeSinceLevelLoad
            return amplitude * Mathf.Sin(tau * Time.time * frequency);
        }

        public static float SinWave(float amplitude, float frequency, int index)
        {
            //Time.timeSinceLevelLoad
            return amplitude * Mathf.Sin(tau * Time.time * frequency + index);
        }
     
        /// <summary>
        /// 두개의 System.object가 동일한지 체크하는 함수
        /// </summary>
        /// <param name="target1">System.Object</param>
        /// <param name="target2">System.Object</param>
        public static void CheckBothTarget(object target1, object target2)
        {
            //동일한 객체이면 트루로 반환함
            Debug.Log(ReferenceEquals(target1, target2));
        }

        public static IEnumerator ActionAndDisable(Component component, lLCroweTool.TimerSystem.TimerModule_Element timer)
        {
            //시간체크
            timer.ResetTime();
            do
            {
                yield return null;
                if (timer.CheckTimer())
                {
                    break;
                }
            } while (true);

            //비활성화
            component.SetActive(false);
        }

        //이거 안쓰는거 같음//20231024
        //public static IEnumerator WaitAndDestroy(Transform targetObject, float timer)
        //{
        //    yield return new WaitForSeconds(timer);
        //    DeActiveSetNullParent(targetObject);

        //    //체크//어차피 오브젝트폴로 작동함
        //    //DestroyManager.Instance.AddWaitDestoryGameObject(emtpyMagazine.gameObject);
        //}

        /// <summary>
        /// 게임오브젝트의 태그가 원하는 태그들중에 있는 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_interectTags">상호작용 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionTag(GameObject _gameObject, string[] _interectTags)//여러 원하는태그중에서 찾아서 있는지 체크하는 함수 //수정함 20190920//20210511수정
        {
            bool isExist = false;
            //태그[]배열로 했을시interectTag.Lengh
            //리스트로 했을시 interectTag.Count
            for (int i = 0; i < _interectTags.Length; i++)
            {
                if (_gameObject.CompareTag(_interectTags[i]))
                {
                    isExist = true;
                    //return returnTag;
                    break;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 게잉오브젝트의 레이어가 원하는 레이어들중에 있는 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_interectLayer">상호작용 레이어들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionLayer(GameObject _gameObject, LayerMask _interectLayer)//원하는 레이어가 여러개일 경우 사용원하는 레이어를 반환해서 있는지 체크//신규제작 20200827//20210511수정
        {
            bool isExist = _gameObject.layer == _interectLayer;
            //태그[]배열로 했을시interectTag.Lengh
            //리스트로 했을시 interectTag.Count
            return isExist;
        }
    
        //조건에 맞는가
        //사용법
        //if(FitCondition)

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="constraintsCondition">타겟된 제약조건</param>
        /// <returns></returns>
        public static bool FitConditionAll(GameObject _gameObject, ConstraintsCondition constraintsCondition)
        {
            //20221111//추가
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (constraintsCondition.useLayerCondition)
            {
                //이거체크하기
                if (!(isRight = FitConditionLayer(_gameObject, constraintsCondition.interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (constraintsCondition.useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, constraintsCondition.interectTags);
            }
            return isRight;
        }

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="useLayerCondition">레이어 체크여부</param>
        /// <param name="interectLayer">체크할 레이어들</param>
        /// <param name="useTagCondition">태그 체크여부</param>
        /// <param name="interectTag">체크할 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionAll(GameObject _gameObject, bool _useLayerCondition, int _interectLayer, bool _useTagCondition, string[] _interectTag)
        {
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (_useLayerCondition)
            {
                if (!(isRight = FitConditionLayer(_gameObject, _interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (_useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, _interectTag);
            }
            return isRight;
        }

        /// <summary>
        /// 게임오브젝트의 레이어와 태그를 체크하는 함수
        /// </summary>
        /// <param name="_gameObject">타겟된 게임오브젝트</param>
        /// <param name="_useLayerCondition">레이어 체크여부</param>
        /// <param name="_interectLayer">체크할 레이어들</param>
        /// <param name="_useTagCondition">태그 체크여부</param>
        /// <param name="_interectTag">체크할 태그들</param>
        /// <returns>조건에 맞는가?</returns>
        public static bool FitConditionAll(GameObject _gameObject, bool _useLayerCondition, LayerMask _interectLayer, bool _useTagCondition, string[] _interectTag)
        {
            bool isRight = false;

            //신규 제작 20200827
            //수정20210511

            //레이어체크
            if (_useLayerCondition)
            {
                if (!(isRight = FitConditionLayer(_gameObject, _interectLayer)))
                {
                    return isRight;
                }
            }
            else
            {
                isRight = true;
            }

            //태그 체크
            if (_useTagCondition)
            {
                isRight = FitConditionTag(_gameObject, _interectTag);
            }
            return isRight;
        }

        /// <summary>
        /// 궤적포인트(타겟오브젝트)를 가져오는 함수
        /// </summary>
        /// <param name="targetObject">타겟오브젝트</param>
        /// <param name="direction">방향</param>
        /// <param name="power">파워</param>
        /// <param name="time">시간</param>
        /// <returns>궤적 포인트</returns>
        public static Vector2 GetArcPoint(Transform targetObject, Vector2 direction, float power, float time , bool worldSpace)
        {
            //transform.position += velocity * Time.deltaTime * Mathf.Lerp(tempTime, 1, acel.Evaluate(tempTime / 1));
            //tempTime += Time.deltaTime;
            //velocity.x += (gravity.x * mass) * Time.deltaTime;
            //velocity.y -= (gravity.y * mass) * Time.deltaTime;
            //공식최적화필요//최적인듯

            Quaternion targetRotation = worldSpace ? Quaternion.identity : targetObject.rotation;
            Vector2 velocity = targetRotation * direction;
            velocity = (Vector2)targetObject.position + (velocity.normalized * power * time) + 0.5f * Physics2D.gravity * (time * time);
            //Vector2 pos = (Vector2)targetObject.position + (direction.normalized * power * time) + 0.5f * Physics2D.gravity * (time * time);
            return velocity;
        }

        /// <summary>
        /// 궤적포인트(벡터0기준)을 가져오는 함수
        /// </summary>
        /// <param name="direction">방향</param>
        /// <param name="power">파워</param>
        /// <param name="time">시간</param>
        /// <returns>궤적포인트</returns>
        public static Vector2 GetArcPoint(Vector2 direction, float power, float time)
        {
            //transform.position += velocity * Time.deltaTime * Mathf.Lerp(tempTime, 1, acel.Evaluate(tempTime / 1));
            //tempTime += Time.deltaTime;
            //velocity.x += (gravity.x * mass) * Time.deltaTime;
            //velocity.y -= (gravity.y * mass) * Time.deltaTime;
            //공식최적화필요//최적인듯
            Vector2 pos = (direction.normalized * power * time) + 0.5f * Physics2D.gravity * (time * time);
            return pos;
        }

        /// <summary>
        /// 궤적포인트(3포인트)를 가져오는 함수//쓸지는 모름
        /// </summary>
        /// <param name="targetObject">x</param>
        /// <param name="direction"></param>
        /// <param name="power"></param>
        /// <param name="time"></param>
        public static Vector2[] GetArc3Point(Transform targetObject, Vector2 direction, float power, float time)
        {
            var velocity = (Vector2)(targetObject.rotation * (direction.normalized * power));//속도
            var p0 = (Vector2)targetObject.position;//첫번쨰위치
            var p1 = p0 + 0.5f * velocity * time;//중간위치
            var p2 = velocity * time + Physics2D.gravity * time * time * 0.5f;//마지막위치

            Vector2[] temp = { p0,p1,p2 };

            return temp;
        }

        /// <summary>
        /// B=>A 방향을 구함
        /// </summary>
        /// <param name="targetAPos">A 위치</param>
        /// <param name="targetBPos">B 위치</param>
        /// <returns></returns>
        public static Vector2 CalDirection(Vector2 targetAPos, Vector2 targetBPos)
        {
            Vector3 direction = targetAPos - targetBPos;
            //float newAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Debug.Log(newAngle);

            ////사분명처리를 위해 라디안사용해야됨
            ////c= 3.14*r
            //float radian = newangle * Mathf.PI / 180; //라디안값//동일값
            //float radian = newAngle * Mathf.Deg2Rad; //라디안값//동일값

            float radian = Mathf.Atan2(direction.y, direction.x);

            //수평속도 = 투사속도 * cos(각도)
            //수직속도 = 투사속도 * sin(각도)
            //Vector2 velocity = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            Vector2 velocity;
            velocity.x = Mathf.Cos(radian);
            velocity.y = Mathf.Sin(radian);

            //수평일시 속도:(-1.0, 0.0)각도:180라디안:3.141593
            //각을 주었을시 속도:(-0.9, -0.4)각도:-153.4669라디안:-2.678504
            //Debug.Log("속도:" + velocity + "각도:" + newAngle + "라디안:" + radian);
            return velocity;
        }

        /// <summary>
        /// 최대최소 정규화
        /// </summary>
        /// <param name="min">최소값</param>
        /// <param name="max"><최대값/param>
        /// <param name="value">현재값</param>
        /// <returns>0~1사이의 값</returns>
        public static float MinMaxNormalize(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        public static float RestoreNormalize(float min, float max, float normalizedValue)
        {
            return Mathf.Lerp(min, max, normalizedValue);
        }

        /// <summary>
        /// 리니어 기능
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float LinearFunc(float start, float end, float t)
        {
            return (1 - t) * start + t * end;
        }

        /// <summary>
        /// 리니어 기능
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 LinearFunc(Vector3 start, Vector3 end, float t)
        {
            return (1 - t) * start + t * end;
        }

      

        public static Vector3 TwoPointBezier(Vector3 p0, Vector3 p1, Vector3 p2 , float t)
        {
            float t2 = (1 - t) * (1 - t);
            return t2 * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        }


        /// <summary>
        /// 3개의 점으로 만드는 곡선(2차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="handle">핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static float ThreePointBezier(float aPoint, float handle, float bPoint, float time)
        {
            //B(t) = P1 + (1 - t)² (P0 - P1) + t²(P2 - P1)
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPoint
            //t = 시간(0 <= t <= 1)

            return handle + Mathf.Pow(1 - time, 2) * (aPoint - handle) + Mathf.Pow(time, 2) * (bPoint - handle);
        }

        /// <summary>
        /// 3개의 점으로 만드는 곡선(2차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="handle">핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static Vector3 ThreePointBezier(Vector3 aPoint, Vector3 handle, Vector3 bPoint, float time)
        {
            //B(t) = P1 + (1 - t)² (P0 - P1) + t²(P2 - P1)
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPoint
            //t = 시간(0 <= t <= 1)

            return handle + Mathf.Pow(1 - time, 2) * (aPoint - handle) + Mathf.Pow(time, 2) * (bPoint - handle);
        }

        /// <summary>
        /// 4개의 점으로 만드는 곡선(3차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="aPointHandle">첫번째 핸들</param>
        /// <param name="bPointHandle">두번째 핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static float FourPointBezier(float aPoint, float aPointHandle, float bPointHandle, float bPoint, float time)
        {
            //B(t) = (1 - t)³ P0 + 3(1 - t)² t P1 + 3(1 - t)t² P2 + t³ P3
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPointHandle
            //P3 = bPoint
            //t = 시간(0 <= t <= 1)

            return Mathf.Pow((1 - time), 3) * aPoint + Mathf.Pow((1 - time), 2) * 3 * time * aPointHandle
                    + Mathf.Pow(time, 2) * 3 * (1 - time) * bPointHandle + Mathf.Pow(time, 3) * bPoint;
        }

        /// <summary>
        /// 4개의 점으로 만드는 곡선(3차 베지어 곡선)
        /// </summary>
        /// <param name="aPoint">첫번째 위치</param>
        /// <param name="aPointHandle">첫번째 핸들</param>
        /// <param name="bPointHandle">두번째 핸들</param>
        /// <param name="bPoint">두번째 위치</param>
        /// <param name="time">0에서 1사이의 시간값</param>
        /// <returns>시간에 따른 곡선값</returns>
        public static Vector3 FourPointBezier(Vector3 aPoint, Vector3 aPointHandle, Vector3 bPointHandle, Vector3 bPoint, float time)
        {
            //B(t) = (1 - t)³ P0 + 3(1 - t)² t P1 + 3(1 - t)t² P2 + t³ P3
            //B(t) = 시간에 따른 최종값
            //P0 = aPoint
            //P1 = aPointHandle
            //P2 = bPointHandle
            //P3 = bPoint
            //t = 시간(0 <= t <= 1)

            return Mathf.Pow((1 - time), 3) * aPoint + Mathf.Pow((1 - time), 2) * 3 * time * aPointHandle
                    + Mathf.Pow(time, 2) * 3 * (1 - time) * bPointHandle + Mathf.Pow(time, 3) * bPoint;
        }


        private static void TestFourPointBezier(Vector3 aPoint, Vector3 aPointHandle, Vector3 bPointHandle, Vector3 bPoint, float time)
        {
            float t4 = Mathf.Pow(1 - time, 4);
            float t3 = Mathf.Pow(1 - time, 3);
            float t2 = Mathf.Pow(1 - time, 2);
            float t1 = 1 - time;


            
            //return t4 * aPoint+
            //    4 * t3
        }

        //각지점을 통과하는 스플라인
        //4개의 점이면 6개의 점이 필요함
        //잘하면 그거 만들수도
        //경로랑 같이 써야할듯
        //맘에듬 대신 각 포인트끼리의 거리는 좀 생각해야할듯하다



        //캣멀룸 
        private static Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t * t * t;

            return 0.5f * ((2.0f * p1) + (-p0 + p2) * t + (2.0f * p0) - 5.0f * p1 +4 *p2 - p3 * t2 + (-p0 + 3.0f * p1 - 3.0f *p2 + p3) * 3);
        }

        /// <summary>
        /// 벡터3 배열을 벡터2 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector3Array">벡터3 배열</param>
        /// <returns>벡터2 배열</returns>
        public static Vector2[] ConvertVector2Array(this Vector3[] vector3Array)
        {
            return System.Array.ConvertAll<Vector3, Vector2>(vector3Array, GetVector3fromVector2);
        }

        /// <summary>
        /// 벡터int3 배열을 벡터2 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector3Array">벡터int3 배열</param>
        /// <returns>벡터2 배열</returns>
        public static Vector2[] ConvertVector2Array(this Vector3Int[] vector3Array)
        {
            return System.Array.ConvertAll<Vector3Int, Vector2>(vector3Array, GetVectorInt3fromVector2);
        }

        /// <summary>
        /// 벡터2 배열을 벡터3 배열로 변환시켜주는 함수
        /// </summary>
        /// <param name="vector2Array">벡터2 배열</param>
        /// <returns>벡터3 배열</returns>
        public static Vector3[] ConvertVector3Array(this Vector2[] vector2Array)
        {
            return System.Array.ConvertAll<Vector2, Vector3>(vector2Array, GetVector2fromVector3);
        }

        /// <summary>
        /// 벡터2를 벡터3로 변환시켜주는함수
        /// </summary>
        /// <param name="vector2">벡터2</param>
        /// <returns>벡터3</returns>
        public static Vector3 GetVector2fromVector3(Vector2 vector2)
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }

        /// <summary>
        /// 벡터3를 벡터2로 변환시켜주는함수
        /// </summary>
        /// <param name="vector3">벡터3</param>
        /// <returns>벡터2</returns>
        public static Vector2 GetVector3fromVector2(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        /// <summary>
        /// 벡터Int3를 벡터2로 변환시켜주는함수
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 GetVectorInt3fromVector2(Vector3Int vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector3Int GetVectorXYFromXZ(Vector3Int vector3Int)
        {
            return new Vector3Int(vector3Int.x, 0, vector3Int.y);
        }

        public static Vector3Int GetVectorXZFromXY(Vector3Int vector3Int)
        {
            return new Vector3Int(vector3Int.x, vector3Int.z);
        }





        /// <summary>
        /// 각도를 방향으로 변경해주는 함수
        /// </summary>
        /// <param name="angle">각도</param>
        /// <param name="axisDirectionType"></param>
        /// <returns>방향</returns>
        public static Vector3 AngleToDirection(float angle, AxisDirectionType axisDirectionType)
        {
            //전거
            //Vector3 direction = GetSightDirectionType(axisDirectionType);
            //var quaternion = Quaternion.Euler(0, 0, angle);
            //Vector3 newDirection = quaternion * direction;

            //신규
            Vector3 direction = GetSightDirectionType(axisDirectionType);
            var quaternion = Quaternion.AngleAxis(angle, direction);
            Vector3 newDirection = quaternion.eulerAngles;
            return newDirection;
        }

        /// <summary>
        /// 축 방향타입 설정에 따른 방향을 가져오는 함수
        /// </summary>
        /// <param name="axisDirectionType">축 방향타입</param>
        /// <returns>방향</returns>
        public static Vector3 GetSightDirectionType(AxisDirectionType axisDirectionType)
        {
            Vector3 direction = Vector3.zero;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    //direction = sightTrigger.transform.right;
                    direction = Vector2.right;
                    break;
                case AxisDirectionType.Y:
                    //direction = sightTrigger.transform.up;
                    direction = Vector3.up;
                    break;
                case AxisDirectionType.Z:
                    direction = Vector3.forward;
                    break;
            }
            return direction;
        }

        /// <summary>
        /// 축 방향타입 설정에 따른 방향을 가져오는 함수
        /// </summary>
        /// <param name="axisDirectionType">축 방향타입</param>
        /// <param name="targetTr">타겟Tr</param>
        /// <returns>방향</returns>
        public static Vector3 GetSightDirectionType(AxisDirectionType axisDirectionType, Transform targetTr)
        {
            Vector3 direction = Vector3.zero;
            switch (axisDirectionType)
            {
                case AxisDirectionType.X:
                    //direction = sightTrigger.transform.right;
                    direction = targetTr.right;
                    break;
                case AxisDirectionType.Y:
                    //direction = sightTrigger.transform.up;
                    direction = targetTr.up;
                    break;
                case AxisDirectionType.Z:
                    direction = targetTr.forward;
                    break;
            }
            return direction;
        }

        /// <summary>
        /// 리스트에서 번호 두가지를 스왑하기 위한 함수
        /// </summary>
        /// <typeparam name="T">타입</typeparam>
        /// <param name="list">리스트</param>
        /// <param name="changeIndexA">변경할 인덱스1</param>
        /// <param name="changeIndexB">변경할 인덱스2</param>
        public static void Swap<T>(this List<T> list, int changeIndexA, int changeIndexB)
        {
            T temp = list[changeIndexA];
            list[changeIndexA] = list[changeIndexB];
            list[changeIndexB] = temp;
        }

        /// <summary>
        /// 하이어라키 순서를 인덱스순서를 정하는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="index">인덱스</param>
        public static void SetSibling(this Component component, int index)
        {
             component.transform.SetSibling(index);
        }

        /// <summary>
        /// 하이어라키 순서를 인덱스순서를 정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="index">인덱스</param>
        public static void SetSibling(this Transform tr, int index)
        {
            tr.SetSiblingIndex(index);
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="pos">위치</param>
        /// <param name="quaternion">회전</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Transform tr, Vector3 pos, Quaternion quaternion, Transform parent = null, bool isWorld = true)
        {
            //프리팹을 주기적으로 초기화하기때문에 만든 함수
            //순서
            //1. 부모정의
            //2. 이동 회전
            //3. 비활성화 키기체크
            tr.SetParent(parent);//디폴트가 true
            //true인 경우 개체가 이전과 동일한 월드 공간 위치, 회전 및 크기를 유지하도록 상위 상대적 위치, 크기 및 회전이 수정
            //tr.SetParent(parent, false);//참일 경우. 이전에 있던걸 유지하고 옮김
            if (isWorld)
            {
                //tr.position = pos;
                //tr.rotation = quaternion;
                tr.SetPositionAndRotation(pos, quaternion);
            }
            else
            {
                //tr.localPosition = pos;
                //tr.localRotation = quaternion;
                tr.SetLocalPositionAndRotation(pos, quaternion);
            }

            if (tr.gameObject.activeSelf == false)
            {
                tr.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="pos">위치</param>
        /// <param name="quaternion">회전</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Component component, Vector3 pos, Quaternion quaternion, Transform parent = null, bool isWorld = true)
        {
            InitTrObjPrefab(component.transform, pos, quaternion, parent, isWorld);
        }

        /// <summary>
        /// 트랜스폼의 위치, 회전, 부모, 월드인지에 따라 설정하는 함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="targetTr">변경시킬 위치와 회전</param>
        /// <param name="parent">부모</param>
        public static void InitTrObjPrefab(this Component component, Transform targetTr , Transform parent = null)
        {
            InitTrObjPrefab(component.transform, targetTr.position, targetTr.rotation, parent);
        }

        /// <summary>
        /// 트랜스폼의 위치, 부모, 월드인지에 따라 설정하는 함수(회전제외)
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="pos">위치</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Transform tr, Vector3 pos, Transform parent = null, bool isWorld = true)
        {   
            InitTrObjPrefab(tr, pos, tr.rotation, parent, isWorld);
        }

        /// <summary>
        /// 트랜스폼의 위치, 부모, 월드인지에 따라 설정하는 함수(회전제외)
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="pos">위치</param>
        /// <param name="parent">부모</param>
        /// <param name="isWorld">월드좌표계여부</param>
        public static void InitTrObjPrefab(this Component component, Vector3 pos, Transform parent = null, bool isWorld = true)
        {
            Transform tr = component.transform;
            InitTrObjPrefab(tr, pos, tr.rotation, parent, isWorld);
        }


        /// 렉트트랜스폼 자기자신의 앵커의 기준으로 위치를 새롭게 배치해주는 함수
        /// </summary>
        /// <param name="rect">렉트트랜스폼</param>
        /// <param name="norDirVec">방향노말벡터</param>
        /// <param name="spacing">간격</param>
        /// <param name="index">인덱스</param>
        public static void FocusRectAnchorPos(this RectTransform rect, Vector2 norDirVec, float spacing, int index)
        {
            //피봇은 건들이유 없음
            //앵커임
            //x, y//좌우, 높낮이//최소 최대 0~1
            Vector2 anchorMin = rect.anchorMin;
            Vector2 anchorMax = rect.anchorMax;
            float width = rect.sizeDelta.x;
            float height = rect.sizeDelta.y;            

            //따로 계산법이 있을만한데
            //1000 20
            //minx maxx pos 
            //0 0 500
            //1 1 -500
            //0.5 0.5 0

            //x끼리
            //500 0 -500
            // 0 0.5  1

            //앵커위치만처리할려고하니 둘중하나만 사용하면 됨
            if (anchorMin == anchorMax)
            {  
                //1000*0.5f;

                //0 => -0.5
                //0.5 => 0
                //1 => 0.5
                //역수처리
                float xPos = -(anchorMin.x - 0.5f) * width;
                float yPos = -(anchorMin.y - 0.5f) * height;
                Vector2 newPos = new Vector2(xPos, yPos);//인덱스가 0 일때의 값

                //방향과 스페이싱처리
                if (index != 0)
                {
                    xPos = norDirVec.x * index * (width + spacing);
                    yPos = norDirVec.y * index * (height + spacing);
                    newPos += new Vector2(xPos, yPos);
                }

                rect.anchoredPosition = newPos;
            }

        }

        /// <summary>
        /// 컬러256세팅 함수
        /// </summary>
        /// <param name="color">컬러</param>
        /// <param name="r">빨간</param>
        /// <param name="g">초록</param>
        /// <param name="b">블루</param>
        /// <param name="a">알파</param>
        public static Color GetColor256(this Color color, float r, float g, float b, float a = 256)
        {
            //컬러구조체의 각요소는 0~1값을 사용
            //0쯤이면 나누기 하지말고 그대로 처리
            r = r < 0.001f ? 0 : Mathf.Clamp(r, 0, 256) / 256f;
            g = g < 0.001f ? 0 : Mathf.Clamp(g, 0, 256) / 256f;
            b = b < 0.001f ? 0 : Mathf.Clamp(b, 0, 256) / 256f;
            a = a < 0.001f ? 0 : Mathf.Clamp(a, 0, 256) / 256f;

            return new Color(r,g,b,a);
        }


        /// <summary>
        /// 컬러256세팅 함수
        /// </summary>        
        /// <param name="r">빨간</param>
        /// <param name="g">초록</param>
        /// <param name="b">블루</param>
        /// <param name="a">알파</param>
        public static Color GetColor256(float r, float g, float b, float a = 256)
        {
            //컬러구조체의 각요소는 0~1값을 사용
            //0쯤이면 나누기 하지말고 그대로 처리
            r = r < 0.001f ? 0 : Mathf.Clamp(r, 0, 256) / 256f;
            g = g < 0.001f ? 0 : Mathf.Clamp(g, 0, 256) / 256f;
            b = b < 0.001f ? 0 : Mathf.Clamp(b, 0, 256) / 256f;
            a = a < 0.001f ? 0 : Mathf.Clamp(a, 0, 256) / 256f;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// HSV + A에 대한 컬러를 가져오는 함수
        /// </summary>
        /// <param name="h">H</param>
        /// <param name="s">S</param>
        /// <param name="v">V</param>
        /// <param name="a">A</param>
        /// <returns>컬러</returns>
        public static Color GetHSVAColor(float h, float s, float v, float a)
        {
            var color = Color.HSVToRGB(h / 360,s / 100, v / 100);
            color.a = a / 100;
            return color;
        }

        /// <summary>
        /// 게임오브젝트를 활성화/비활성화시켜주는 함수
        /// </summary>
        /// <param name="component">컴포넌트</param>
        /// <param name="value">활성화/비활성화</param>
        public static void SetActive(this Component component, bool value)
        {
            component.gameObject.SetActive(value);
        }

        /// <summary>
        /// 컴포넌트를 활성화/비활성화시켜주는 함수
        /// </summary>
        /// <param name="behaviour">컴포넌트</param>
        /// <param name="value">활성화/비활성화</param>
        public static void SetEnable(this Behaviour behaviour, bool value)
        {
            behaviour.enabled = value;
        }

        /// <summary>
        /// vector3로 회전처리하되 짐벌락부분을 처리한 회전함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="rotateEuler">회전될 각</param>
        public static void RotateEuler(this Transform tr, Vector3 rotateEuler)
        {
            //오일러의 짐벌락문제를 해결하기 위해 쿼터니언을 사용한 vector3처리
            //이걸로 해야지만 축의 고정이 안풀림
            tr.rotation = Quaternion.AngleAxis(rotateEuler.x, Vector3.right) * Quaternion.AngleAxis(rotateEuler.y, Vector3.up) * Quaternion.AngleAxis(rotateEuler.z, Vector3.forward);
        }

        /// <summary>
        /// vector3로 회전처리하되 짐벌락부분을 처리한 회전함수
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        /// <param name="rotateEuler">회전될 각</param>
        /// <param name="targetAxisTr">타겟이될 축</param>
        public static void RotateEuler(this Transform tr, Vector3 rotateEuler , Transform targetAxisTr)
        {   
            tr.rotation = Quaternion.AngleAxis(rotateEuler.x, targetAxisTr.right) * Quaternion.AngleAxis(rotateEuler.y, targetAxisTr.up) * Quaternion.AngleAxis(rotateEuler.z, targetAxisTr.forward);
        }

#if lLcroweProject3D


        //3d용


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hitInfo"></param>
        /// <param name="target"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private static bool CheckCast<T>(RaycastHit hitInfo, ref T target, string tag = null)
        {
            //태그 컴포넌트 체크구역 
            //태그체크
            if (!string.IsNullOrEmpty(tag))
            {
                if (!hitInfo.collider.CompareTag(tag))
                {
                    return false;
                }
            }

            //컴포넌트체크
            if (!hitInfo.collider.TryGetComponent(out target))
            {
                return false;
            }
            return true;
        }

        public static bool RayCast<T>(this Component component, out T target, Ray ray, LayerMask checkLayer, float distance = Mathf.Infinity, string tag = null) where T : Component
        {            
            target = null;
            if (!Physics.Raycast(ray, out RaycastHit hitInfo, distance, checkLayer))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        public static bool LineCast<T>(this Component component, out T target, Vector3 startPos, Vector3 endPos, LayerMask checkLayer, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.Linecast(startPos, endPos, out RaycastHit hitInfo, checkLayer))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        public static bool SphereCast<T>(this Component component, out T target, Vector3 pos, float size, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.SphereCast(pos, size, Vector3.up, out RaycastHit hitInfo, distance, layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }

        public static bool BoxCast<T>(this Component component, out T target, Vector3 pos, Vector3 size, Quaternion quaternion, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (Physics.BoxCast(pos, size * 0.5f, Vector3.up, out RaycastHit hitInfo, quaternion,distance,layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }

        public static bool CapsuleCast<T>(this Component component, out T target, Vector3 posDown, Vector3 posUp, float size, LayerMask layerMask, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (!Physics.CapsuleCast(posDown, posUp, size, Vector3.up, out RaycastHit hitInfo, distance, layerMask))
            {
                return false;
            }
            return CheckCast(hitInfo, ref target, tag);
        }
        

        public static bool CylinderCast<T>(this Component component, out T target, Vector3 pos, Vector3 size, Quaternion quaternion, LayerMask layerMask, float cylinderDistance, bool isYAxis, float distance = 0, string tag = null) where T : Component
        {
            target = null;
            if (Physics.BoxCast(pos, size * 0.5f, Vector3.up, out RaycastHit hitInfo, quaternion, distance, layerMask))
            {
                return false;
            }

            Gizmos.DrawWireCube(pos, size);
            Vector3 originTr = quaternion * pos;
            originTr.y = hitInfo.point.y;//

            //Y축기준으로 거리를 잰다면
            Vector3 newPos = isYAxis ? new Vector3(originTr.x, hitInfo.point.y) : new Vector3(hitInfo.point.x, originTr.y);

            //거리 체크구역
            if (!CheckDistance(originTr, newPos, cylinderDistance))
            {
                return false;
            }

            return CheckCast(hitInfo, ref target, tag);
        }

#endif

        //사용법 : CrossVec(transform.up, transform.forward, target.position)
        /// <summary>
        /// 외적구해서 우측인지 좌측인지 알수 있는 함수
        /// </summary>
        /// <param name="norVecUp">노말벡터 위</param>
        /// <param name="norVecForward">노말벡터 전방</param>
        /// <param name="targetPos">타겟위치</param>
        /// <returns>-1 좌측, +1 우측</returns>
        public static float CrossVec(Vector3 norVecUp, Vector3 norVecForward, Vector3 targetPos)
        {
            Vector3 p = norVecForward - norVecUp;
            Vector3 q = targetPos - norVecForward;
            return Vector3.Cross(p, q).y;
        }

        public static bool CheckRight(Vector3 norVecUp, Vector3 norVecForward, Vector3 targetPos)
        {
            float yValue = CrossVec(norVecUp, norVecForward, targetPos);
            return yValue > 0;
        }


        //(1,3) (2,4)
        //내적
        //1*2 + 3*4 = 14(길이값, 크기) //살펴보니까 삼각함수의 코싸인과 같다

        public static float GetDot2DValue(Transform curTr, Transform targetTr)
        {
             return GetDotValue(curTr.up, targetTr.up);
        }

        public static float GetDotValue(Vector3 trNorVec, Vector3 targetNorVec)
        {
            float scale = Vector2.Dot(trNorVec, targetNorVec);
            return scale;
        }

        public static bool CheckForward(Vector3 trNorVec, Vector3 targetNorVec)
        {
            float scale = GetDotValue(trNorVec, targetNorVec);
            //0 초과이면 전방(1)  이하이면 후방(-1)
            return scale > 0;
        }

        /// <summary>
        /// 배열로 받은 맵을 회전시키는 함수
        /// </summary>
        /// <param name="angle">회전각</param>
        /// <param name="isClockwise">시계방향 여부</param>
        /// <param name="posArray">맵으로 되있는 배열</param>
        /// <returns>회전시킨 배열</returns>
        public static Vector3Int[] RotateVector3IntArray(float angle, bool isClockwise, Vector3Int[] posArray)
        {
            //(3by3)이렇게 되있는게 쓰기 편하다

            angle = isClockwise ? -angle : angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationQuaternion);

            for (int i = 0; i < posArray.Length; i++)
            {
                int index = i;

                //현재 타일
                Vector3Int curTilePos = posArray[index];

                //회전
                Vector3 rotatedVector = rotationMatrix.MultiplyPoint3x4(curTilePos);

                //대입
                posArray[index] = Vector3Int.CeilToInt(rotatedVector);
            }
            return posArray;
        }


        /// <summary>
        /// 원본타일맵을 다른타일맵 회전하여 전달하는 함수
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="isClockwise"></param>
        /// <param name="otherTileMap"></param>
        public static void RotateTileMap(float angle, bool isClockwise, Vector3Int[] posArray, Tilemap originTileMap, Tilemap otherTileMap)
        {
            angle = isClockwise ? -angle : angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angle, Vector3.forward);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotationQuaternion);

            for (int i = 0; i < posArray.Length; i++)
            {
                //현재 타일
                Vector3 curTilePos = posArray[i];
                TileBase tile = lLcroweUtil.GetTile(posArray[i], originTileMap);

                //회전
                Vector3 rotatedVector = rotationMatrix.MultiplyPoint3x4(curTilePos);

                //다른타일
                Vector3Int newPos = lLcroweUtil.GetWorldToCell(rotatedVector + originTileMap.tileAnchor, otherTileMap);

                lLcroweUtil.SetTile(newPos, tile, otherTileMap);

            }
        }

        //이방식은 현재 rigidbody2D와 프로젝트 세팅의 physics2d time탭에
        //설정된 값을 사용해서 물리적인 연산처리를 한결과.
        //어떤물체와 어떤 충돌할지는 예측할수 없는 코드.
        //리지드바디에 힘이 주어져 발사하게 되었을때 궤도를 얻기위한 함수

        public static Vector2[] Physics2DSimurate(Vector2 pos, Vector2 velocity, int frameCount, Rigidbody2D rb2d)
        {
            Vector2[] pointArray = new Vector2[frameCount];

            //Time.fixedDeltaTime : 한프레임당 소비되는 물리시간
            //Physics2D.velocityIterations : 물리연산의 반복횟수
            //0.02 / 8
            float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;


            //한프레임당 적용할 중력값
            Vector2 gravityAccel = Physics2D.gravity * rb2d.gravityScale * timeStep * timeStep;

            //한프레임당 적용할 저항력을 구함
            float drag = 1 - timeStep * rb2d.drag;

            //시작스텝을 구하고 시뮬레이션
            Vector2 moveStep = velocity * timeStep;
            for (int i = 0; i < frameCount; i++)
            {
                moveStep += gravityAccel;//속도
                moveStep *= drag;//저항력
                pos += moveStep;//위치값에 누적처리
                pointArray[i] = pos;//지정
            }

            //리턴
            return pointArray;
        }




        //3차원 좌표계를 UI좌표계로 변경하는 함수 (비율계산)
        //주의점 월드맵사이즈를 제대로 맞추어줘야됨//맞추어주는 기능을 가진 무언가가 필요
        //월드 맵
        public static Vector2 WorldPosToMapPos(Vector3 wolrdPos, float worldWidth, float worldDepth, float uiMapWidth, float uiMapHeight)
        {
            Vector2 result = Vector2.zero;
            result.x = (wolrdPos.x * uiMapWidth) / worldWidth;
            result.y = (wolrdPos.z * uiMapHeight) / worldDepth;

            return result;
        }

        //맵에서 월드
        public static Vector3 MapPosToWorldPos(Vector3 uiPos, float worldWidth, float worldDepth, float uiMapWidth, float uiMapHeight)
        {
            Vector3 result = Vector3.zero;
            result.x = (uiPos.x * worldWidth) / uiMapWidth;
            result.z = (uiPos.y * worldDepth) / uiMapHeight;

            return result;
        }

        //월드 방향을 맵 방향으로
        public static void MapLookAt(Transform worldPlayer, Transform uiPlayer)
        {
            float angleZ = Mathf.Atan2(worldPlayer.forward.z, worldPlayer.forward.x) * Mathf.Rad2Deg;
            uiPlayer.rotation = Quaternion.Euler(0, 0, angleZ - 90);
            //uiPlayer.eulerAngles = new Vector3 (0, 0, angleZ - 90);
        }

        /// <summary>
        /// HEX컬러를 RGBA 컬러로 변환해주는 함수
        /// </summary>
        /// <param name="hexColor">HEX컬러</param>
        /// <param name="rgbaColor">RGBA컬러</param>
        /// <returns></returns>
        public static bool ConvertHexColorToRGBAColor(string hexColor, out Color rgbaColor)
        {   
            return ColorUtility.TryParseHtmlString(hexColor, out rgbaColor);
        }

        /// <summary>
        ///RGBA컬러를 HEX컬러로 변환해주는 함수
        /// </summary>
        /// <param name="rgbaColor">RGBA컬러</param>
        /// <returns>HEX컬러</returns>
        public static string ConvertRGBAColorToHEXColor(Color rgbaColor)
        {
            return ColorUtility.ToHtmlStringRGBA(rgbaColor);
        }

        /// <summary>
        /// 일정값을 특정값으로 반올림시켜주는 함수.
        /// RoundToInt는 int로 만되니 대신만드는것
        /// </summary>
        /// <param name="value">현재 값</param>
        /// <param name="checkValue">기준이 될 위치값</param>
        /// <param name="range">범위</param>
        /// <returns>라운드될 값안에 들어왔는지 여부</returns>
        public static bool CheckRoundToNear(float value, float checkValue, float range)
        {
            float lowerBound = checkValue - range;
            float upperBound = checkValue + range;

            //라운드 범위체크
            //0 ~ 0.4, 0.5~0.9
            //0<=0.5, 0.5 < (1 == 0.9999..)
            return lowerBound <= value && value < upperBound;
        }

        /// <summary>
        /// 일정값을 특정값으로 반올림시켜주는 함수.
        /// RoundToInt는 int로 만되니 대신만드는것
        /// </summary>
        /// <param name="value">현재 값</param>
        /// <param name="checkValue">기준이 될 위치값</param>
        /// <param name="range">범위</param>
        /// <returns>라운드된 최종값</returns>
        public static float RoundToNear(float value, float checkValue, float range)
        {
            //범위체크
            if (CheckRoundToNear(value, checkValue, range))
            {
                //안이면 체크할밸류로 넘김
                return checkValue;
            }
            else
            {
                //밖이면 그대로 넘김
                return value;
            }
        }

        public static int RoundToInt(float value)
        {
            int result = (int)((value + 0.5f) / 1);
            return result;
        }



        //============================================================
        //리소스처리관련&어드레서블처리관련
        //============================================================

        /// <summary>
        /// 비어있는 데이터일 경우 대신나오는 스프라이트
        /// </summary>
        private static Sprite NullSprite => Resources.Load<Sprite>("NullSprite");

        //필요할경우
        //CATPathType에 필요한 타입을 추가//폴더이름은 빼기
        //GetResourcesPath에 실질적인 경로 추가

        //CSV에서 데이터를 가져와서 리소시스의 특정경로에 접근하여 가져옴
        //스프라이트일시 => Sprite => SpriteFolder
        //프리팹을 통할시 => UnitObject => Prefab/UnitObjectFolder



        /// <summary>
        /// 카테고리 경로타입(XXFolder => XX)
        /// </summary>
        public enum CATPathType
        {
            /// <summary>
            /// 특정폴더를 참조안함
            /// </summary>
            Default,
            Sprite,
            UIBar,
            UI,
            UnitObject,
            UnitCard,
            DamageObject,
            Effect,
            Font,
            Material,
            UIMaterial,
        }
        
        /// <summary>
        /// 카테고리에 따른 경로를 반환하는 함수
        /// </summary>
        /// <param name="cATPathType">카테고리타입</param>
        /// <returns>카테고리별 경로</returns>
        public static string GetResourcesPath(CATPathType cATPathType)
        {
            string path;
            switch (cATPathType)
            {                
                case CATPathType.Default:
                    //여기는 특정폴더를 참조안함
                    path = "";
                    break;
                case CATPathType.Sprite:
                    //자체폴더
                    path = $"{cATPathType}Folder";
                    break;
                default:
                    //그외거는 프리팹쪽을 참조
                    path = $"Prefab/{cATPathType}Folder";
                    break;
            }
            return path;
        }

        static DataBaseManager dataBaseManager;

        /// <summary>
        /// 리소시스 폴더에서 타입에 따른 오브젝트를 가져오는 함수
        /// </summary>
        /// <typeparam name="T">Object를 상속한 타입</typeparam>
        /// <param name="fileName">파일이름</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>리소시스폴더에서 찾은 오브젝트</returns>
        public static T GetResourcesForObject<T>(string fileName, CATPathType pathType) where T : Object
        {
            //비어있는지 체크
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            //구버전
            //경로체크
            //string path = $"{GetResourcesPath(pathType)}/{fileName}";

            //Object를 사용해서 프리팹을 찾을경우 게임오브젝트를 찾음//component로 찾아야지 컴포넌트로 찾음
            //T targetObject = Resources.Load<T>(path);

            if (dataBaseManager == null)
            {
                dataBaseManager = Object.FindAnyObjectByType<DataBaseManager>();
            }

            //신버전
            if (dataBaseManager.dataBaseInfo == null)
            {
                if (LogManager.CheckRegister("GetResources"))
                {
                    LogManager.Register("GetResources", "GetResources", true, true);
                }
                LogManager.Log("GetResources", "dataBaseInfo is Empty");
                return null;
            }

            //다글거와 버전
            //var pathList = DataBaseManager.Instance.dataBaseInfo.allResourcesPathList;
            T targetObject = null;
            //foreach (var path in pathList)
            //{
            //    targetObject = Resources.Load<T>(path);
            //    if (targetObject != null)
            //    {
            //        break;
            //    }
            //}


            //바이블사용
            ResourcesBible bible = null;
            switch (pathType)
            {
                case CATPathType.Sprite:
                    bible = DataBaseManager.Instance.dataBaseInfo.resourcesSpriteBible;
                    bible.TryGetValue(fileName, out var sprite);
                    targetObject = sprite as T;
                    break;
                case CATPathType.Default:                
                case CATPathType.UIBar:
                case CATPathType.UI:
                case CATPathType.UnitObject:
                case CATPathType.UnitCard:
                case CATPathType.DamageObject:
                case CATPathType.Effect:
                case CATPathType.Font:
                case CATPathType.Material:
                case CATPathType.UIMaterial:
                    bible = DataBaseManager.Instance.dataBaseInfo.resourcesObjectBible;
                    bible.TryGetValue(fileName, out var target);
                    //타입처리//게임오브젝트면 체크
                    GameObject go = target as GameObject;
                    if (go == null)
                    {
                        targetObject = target as T;
                    }
                    else
                    {
                        go.TryGetComponent<T>(out targetObject);
                    }
                    break;
            }
            return targetObject;
        }

        /// <summary>
        /// 리소시스폴더에서 컴포넌트로 프리팹을 찾는 함수.
        /// </summary>
        /// <typeparam name="T">컴포넌트타입</typeparam>
        /// <param name="fileName">파일이름</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>컴포넌트 프리팹</returns>
        public static T GetResourcesForComponent<T>(string fileName, CATPathType pathType) where T : Component
        {
            var target = GetResourcesForObject<T>(fileName, pathType);
            return target;
        }

        /// <summary>
        /// 리소시스폴더에서 컴포넌트로 프리팹을 찾는 함수.
        /// </summary>
        /// <typeparam name="T">컴포넌트 타입</typeparam>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>컴포넌트 프리팹</returns>
        public static T GetResourcesForComponent<T>(this Dictionary<string, object> item, string cellID, CATPathType pathType) where T : Component
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            var target = GetResourcesForObject<T>(cellID, pathType);
            return target;
        }

        /// <summary>
        /// 리소시스 폴더에서 타입에 따른 오브젝트를 가져오는 함수
        /// </summary>
        /// <typeparam name="T">오브젝트</typeparam>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <param name="pathType">경로타입</param>
        /// <returns>Object를 상속받은 Object</returns>
        public static T GetResourcesForObject<T>(this Dictionary<string, object> item, string cellID, CATPathType pathType) where T : Object
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            return GetResourcesForObject<T>(cellID, pathType);
        }

        /// <summary>
        /// 리소시스폴더에서 스프라이트를 가져오는 함수. 비어있으면 NullSprite로 반환한다.
        /// </summary>
        /// <param name="fileName">파일이름</param>
        /// <returns>스프라이트파일</returns>
        public static Sprite GetResourcesForSprite(string fileName)
        {
            var target = GetResourcesForObject<Sprite>(fileName, CATPathType.Sprite);

            if (ReferenceEquals(target, null))
            {
                return NullSprite;
            }
            return target;
        }

        /// <summary>
        /// 리소시스폴더에서 스프라이트를 가져오는 함수.
        /// </summary>
        /// <param name="item">CSV아이템</param>
        /// <param name="cellID">Cell아이디</param>
        /// <returns>스프라이트파일</returns>
        public static Sprite GetResourcesForSprite(this Dictionary<string, object> item, string cellID)
        {
            cellID = item.GetConvertString(cellID);//파일이름으로 변경
            var target = GetResourcesForSprite(cellID);
            return target;
        }

        //============================================================
        //DataBaseManager등 CSV대용량데이터에서 사용하는 함수들
        //============================================================

        /// <summary>
        /// 바이블에 Info리스트에 있는것들을 등록하는 함수
        /// </summary>
        /// <typeparam name="T1">커스텀딕셔너리string,LabelBase</typeparam>
        /// <typeparam name="T2">LabelBase</typeparam>
        /// <param name="bible">커스텀딕셔너리string,LabelBase</param>
        /// <param name="infoList">LabelBase리스트</param>
        public static void AddBibleForInfoList<T1, T2>(this T1 bible, List<T2> infoList) where T1 : CustomDictionary<string, T2> where T2 : LabelBase
        {
            foreach (var item in infoList)
            {
                if (!bible.TryAdd(item.labelID, item))
                {
                    LogManager.Log("DataImport", $"{typeof(T2)}타입의 {item.labelID}이름을 가진 데이터가 중복되어 등록이 안됫습니다.");
                }
            }
        }

        /// <summary>
        /// Info리스트에 있는것을 특정데이터모델에 등록후 바이블에 등록하는 함수
        /// </summary>
        /// <typeparam name="T1">커스텀딕셔너리string,LabelBase</typeparam>
        /// <typeparam name="T2">LabelBase</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="bible">커스텀딕셔너리string,LabelBase</param>
        /// <param name="infoList">LabelBase리스트</param>
        /// <param name="func">데이터할당 기능이 있는 함수(New클래스, Info)</param>
        public static void AddBibleForInfoData<T1, T2, T3>(this T1 bible, List<T2> infoList, System.Func<T3, T2, T3> func) where T1 : CustomDictionary<string, T3> where T2 : LabelBase where T3 : class, new()
        {
            if (func == null)
            {
                LogManager.Log("DataImport", $"{typeof(T3)}타입이 들어갈 이벤트함수가 비어있어서 작동을 안합니다.");
                return;
            }
            foreach (var item in infoList)
            {
                T3 customData = new T3();
                customData = func.Invoke(customData, item);
                if (bible.TryAdd(item.labelID, customData))
                {
                    LogManager.Log("DataImport", $"{typeof(T2)}타입의 {item.labelID}이름을 가진 데이터가 중복되어 등록이 안됫습니다.");
                }
            }
        }

        //Func에 들어갈 함수예시
        //private CustomClass func(CustomClass customClass, LabelBase labelBase)
        //{
        //    customClass.xx1 = labelBase;
        //    customClass.xx2 = false;
        //    return customClass;
        //}



        public static bool GetConvertBool(this Dictionary<string, object> item, string cellID)
        {
            bool.TryParse(item[cellID].ToString(), out bool result);
            return result;
        }

        public static bool GetTryConvertBool(this Dictionary<string, object> item, string cellID, out bool result)
        {
            return bool.TryParse(item[cellID].ToString(), out result);
        }

        public static int GetConvertInt(this Dictionary<string, object> item, string cellID)
        {
            int result = -1;
            int.TryParse(item[cellID].ToString(), out result);
            return result;
        }

        public static bool GetTryConvertInt(this Dictionary<string, object> item, string cellID, out int result)
        {
            return int.TryParse(item[cellID].ToString(), out result);
        }

        public static float GetConvertFloat(this Dictionary<string, object> item, string cellID)
        {
            float result = -1;
            float.TryParse(item[cellID].ToString(), out result);
            return result;
        }

        public static bool GetTryConvertFloat(this Dictionary<string, object> item, string cellID, out float result)
        {
            return float.TryParse(item[cellID].ToString(), out result);
        }

        public static string GetConvertString(this Dictionary<string, object> item, string cellID)
        {
            //return (string)item[cellID];//0같은 숫자일시 문제발생
            return item[cellID].ToString();
        }

        public static T GetConvertEnum<T>(this Dictionary<string, object> item, string cellID) where T : struct
        {
            //(EAchievementActionTagType)Enum.Parse(typeof(EAchievementActionTagType), (string)item["achievementActionType"]);
            T temp = (T)System.Enum.Parse(typeof(T), (string)item[cellID]);

            if (System.Enum.TryParse(typeof(T), (string)item[cellID], out object result))
            {
                temp = (T)result;
            }


            return temp;
        }

        public static T GetConvertEnum1<T>(this Dictionary<string, object> item, string cellID) where T : System.Enum
        {
            T temp = default(T);
            if (System.Enum.TryParse(typeof(T), (string)item[cellID], out object result))
            {
                temp = (T)result;
            }

            return temp;
        }


        /// <summary>
        /// RectTransform의 프리셋을 설정하는 함수
        /// </summary>
        /// <param name="rect">렉트트랜스폼</param>
        /// <param name="anchorPreset">렉트앵커프리셋</param>
        public static void SetAnchorPreset(this RectTransform rect, RectAnchorPreset anchorPreset)
        {
            //테스트끝//작동잘됨
            //rect.anchoredPosition = Vector2.zero;

            //해당위치꺼로 바꿔치기하면 됨
            float leftBottomStretchMin = 0;
            float middle = 0.5f;
            float rightTopStretchMax = 1;

            switch (anchorPreset)
            {
                case RectAnchorPreset.TopLeft:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, rightTopStretchMax);
                    rect.anchorMax = new Vector2(leftBottomStretchMin, rightTopStretchMax);
                    break;
                case RectAnchorPreset.TopCenter:
                    rect.anchorMin = new Vector2(middle, rightTopStretchMax);
                    rect.anchorMax = new Vector2(middle, rightTopStretchMax);
                    break;
                case RectAnchorPreset.TopRight:
                    rect.anchorMin = new Vector2(rightTopStretchMax, rightTopStretchMax);
                    rect.anchorMax = new Vector2(rightTopStretchMax, rightTopStretchMax);
                    break;
                case RectAnchorPreset.MiddleLeft:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, middle);
                    rect.anchorMax = new Vector2(leftBottomStretchMin, middle);
                    break;
                case RectAnchorPreset.MiddleCenter:
                    rect.anchorMin = new Vector2(middle, middle);
                    rect.anchorMax = new Vector2(middle, middle);
                    break;
                case RectAnchorPreset.MiddleRight:
                    rect.anchorMin = new Vector2(rightTopStretchMax, middle);
                    rect.anchorMax = new Vector2(rightTopStretchMax, middle);
                    break;
                case RectAnchorPreset.BottomLeft:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(leftBottomStretchMin, leftBottomStretchMin);
                    break;
                case RectAnchorPreset.BottonCenter:
                    rect.anchorMin = new Vector2(middle, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(middle, leftBottomStretchMin);
                    break;
                case RectAnchorPreset.BottomRight:
                    rect.anchorMin = new Vector2(1, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(1, leftBottomStretchMin);
                    break;

                case RectAnchorPreset.VerticalStretchLeft:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(leftBottomStretchMin, rightTopStretchMax);
                    break;
                case RectAnchorPreset.VerticalStretchCenter:
                    rect.anchorMin = new Vector2(middle, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(middle, rightTopStretchMax);
                    break;
                case RectAnchorPreset.VerticalStretchRight:
                    rect.anchorMin = new Vector2(rightTopStretchMax, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(rightTopStretchMax, rightTopStretchMax);
                    break;
                case RectAnchorPreset.HorizontalStretchTop:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, rightTopStretchMax);
                    rect.anchorMax = new Vector2(rightTopStretchMax, rightTopStretchMax);
                    break;
                case RectAnchorPreset.HorizontalStretchMiddle:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, middle);
                    rect.anchorMax = new Vector2(rightTopStretchMax, middle);
                    break;
                case RectAnchorPreset.HorizontalStretchBottom:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(rightTopStretchMax, leftBottomStretchMin);
                    break;
                case RectAnchorPreset.StretchBoth:
                    rect.anchorMin = new Vector2(leftBottomStretchMin, leftBottomStretchMin);
                    rect.anchorMax = new Vector2(rightTopStretchMax, rightTopStretchMax);

                    //나중에 밑에거 함수로 빼버리기
                    //Left,Bottom
                    rect.offsetMin = new Vector2(0, 0);
                    //Right,Top//최대 높이와 우측에서 빼줘야됨
                    rect.offsetMax = -new Vector2(0, 0);
                    break;
            }
        }

        /// <summary>
        /// 렉트앵커프리셋
        /// </summary>
        public enum RectAnchorPreset
        {
            //16개가 필요
            //9개//xy가 동일하게 작동
            TopLeft,
            TopCenter,
            TopRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,

            BottomLeft,
            BottonCenter,
            BottomRight,

            //3개//수평스트래칭//수평 x가 01
            HorizontalStretchTop,
            HorizontalStretchMiddle,
            HorizontalStretchBottom,

            //3개//수직스트래칭//수직 y가 01
            VerticalStretchLeft,
            VerticalStretchCenter,
            VerticalStretchRight,

            //1개//양쪽스트래칭
            StretchBoth
        }
    }

    [System.Serializable]
    public class RotateTurretObject
    {
        public RotateType rotateType;
        public float rotateSpeed;
        public Transform targetObject;
    }


    /// <summary>
    /// 회전타입
    /// </summary>
    public enum RotateType
    {
        Slerp,
        Lerp,
        Turret,
    }

    /// <summary>
    /// 각 축에 대한 기준을 위한 타입
    /// </summary>
    public enum AxisDirectionType
    {
        X,//X 방향
        Y,//Y 방향
        Z,//Z 방향
    }


    //※비교자 제작시 확인할것

    //Sort() 메서드를 실행하면 내부적으로 해당 리스트 안에 있는 객체의 IComparable 인터페이스의 함수, CompareTo(T)를 실행.
    //CompareTo(T)는 비교할 다른 객체 T를 인자로 받아 해당 객체가 비교할 객체보다 크면 양수를 반환, 비교할 객체가 더 크면 음수를 반환
    //자기가 크면 양수
    //남이 크면 음수


    //이를 토대로 List는 정렬을 진행하게 되는 것이죠.


    //IComparable//객체에 정렬기능을 집어넣어 객체를 정렬할수 있게
    //IComparer//원하는부분들을 정렬하기 위해 존재//정렬하는 방법만 정의하는 객체

    //Using문 체크
    //IComparer => using System.Collections;
    //IComparer<T> => using System.Collections.Generic;

    //차순관련
    //디폴트(기본)는 오름차순
    //ASC//Ascending(오름차순)//작은 값부터 큰 값 쪽으로의 순서
    //DESC//Descending(내림차순)//큰 값부터 작은 값 쪽으로의 순서

    //순서처리를 위한 기능클래스
    public static class SortUtil
    {
        /// <summary>
        /// ASC//Ascending(오름차순)//작은 값부터 큰 값 쪽으로의 순서
        /// DESC//Descending(내림차순)//큰 값부터 작은 값 쪽으로의 순서
        /// </summary>
        /// <param name="isASCState">오름차순 상태</param>
        /// <returns>정렬할 수</returns>
        public static int GetASC(bool isASCState)
        {
            return isASCState ? 1 : -1;
        }
    }

    /// <summary>
    /// 거리비교자
    /// </summary>
    public struct DistanceSort : IComparer<Vector3>, IComparer<Transform>, IComparer<BattleUnitObject>, IComparer<IGameRuleManagingTarget>
    {
        private Vector3 compareVector;
        private bool isAsc;

        public DistanceSort(Vector3 value, bool isAsc = true)
        {
            compareVector = value;
            this.isAsc = isAsc;
        }

        public int Compare(Vector3 posX, Vector3 posY)
        {
            Vector3 offset = posX - compareVector;
            float xDistance = offset.sqrMagnitude;

            offset = posY - compareVector;
            float yDistance = offset.sqrMagnitude;

            //정렬
            int value = SortUtil.GetASC(isAsc);
            return xDistance.CompareTo(yDistance) * value;
        }

        public int Compare(Transform trX, Transform trY)
        {
            return Compare(trX.position, trY.position);
        }

        public int Compare(BattleUnitObject x, BattleUnitObject y)
        {
            return Compare(x.Tr.position, y.Tr.position);
        }

        public int Compare(IGameRuleManagingTarget x, IGameRuleManagingTarget y)
        {
            return Compare(x.GetTr().position, y.GetTr().position);
        }
    }

    /// <summary>
    /// 값비교자
    /// </summary>
    public struct ValueSort : IComparer<float>, IComparer<int>, IComparer<ScoreInfo> , IComparer<MVPScoreData> , IComparer<ScoreTargetData>
    {
        private bool isAsc;

        public ValueSort(bool isAsc)
        {
            this.isAsc = isAsc;
        }

        public int Compare(float x, float y)
        {
            int value = SortUtil.GetASC(isAsc);
            return x.CompareTo(y) * value;
        }

        public int Compare(int x, int y)
        {
            int value = SortUtil.GetASC(isAsc);
            return x.CompareTo(y) * value;
        }

        public int Compare(ScoreInfo x, ScoreInfo y)
        {
            int value = SortUtil.GetASC(isAsc);
            return x.score.CompareTo(y.score) * value;
        }

        public int Compare(MVPScoreData x, MVPScoreData y)
        {
            int value = SortUtil.GetASC(isAsc);
            return x.totalScore.CompareTo(y.totalScore) * value;
        }

        public int Compare(ScoreTargetData x, ScoreTargetData y)
        {
            int value = SortUtil.GetASC(isAsc);
            return x.scoreTypeForScore.CompareTo(y.scoreTypeForScore) * value;
        }
    }
}