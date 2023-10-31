using UnityEngine;
using UnityEngine.EventSystems;

public class CheckOnCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //HUD속성에는 다집어넣어서 체크하는용도
    //input 매니저에서 제어함
    public static bool onUIPanel = false;
    //필요하면 다른것도 집어넣음.

    //마우스가 해당 패널에 올라가면 이벤트가 와서 
    //마스터컨버스위에 하면어떨까?
    //컨버스위에 하면 하위HUD쪽 감지됨! //180916
    //마우스포인터 확인용도

    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        //print("Cursor Entering " + name);

        //if (name == gameObject.name)
        //{
        //    print("커서와 다은 곳은 해당 오브젝트와 같은 이름 입니다.");
        //}
        //else
        //{
        //    print("커서와 다은 곳은 해당 오브젝트와 다른 이름 입니다.");
        //}
        onUIPanel = true;
    }
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        //print("Cursor Exit" + name);
        onUIPanel = false;

    }
    
}


