using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;
using lLCroweTool.ToolTipSystem;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ToolTipUiView), true)]
    [CanEditMultipleObjects]
    public class ToolTipInspecterEditor : Editor
    {
        ToolTipUiView module_Base;

        private void OnEnable()
        {
            module_Base = (ToolTipUiView)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (0 == module_Base.transform.childCount)
            {
                if (GUILayout.Button("-=생성=-"))
                {
                    CreateObjectInitSetting(module_Base);
                }
                EditorGUILayout.HelpBox("생성버튼을 눌려 초기화해주세요.", MessageType.Warning);
            }
            else if (!module_Base.TryGetComponent(out HorizontalOrVerticalLayoutGroup _horizontalOrVerticalLayoutGroup))
            {
                bool isClick = false;
                if (GUILayout.Button("-=Vertical컴포넌트추가=-"))
                {
                    _horizontalOrVerticalLayoutGroup = module_Base.gameObject.AddComponent<VerticalLayoutGroup>();
                    isClick = true;
                }

                if (GUILayout.Button("-=Horizontal컴포넌트추가=-"))
                {
                    _horizontalOrVerticalLayoutGroup = module_Base.gameObject.AddComponent<HorizontalLayoutGroup>();
                    isClick = true;
                }

                if (isClick)
                {
                    //ver or hor 가지면 설정
                    _horizontalOrVerticalLayoutGroup.childControlWidth = true;
                    _horizontalOrVerticalLayoutGroup.childControlHeight = true;
                    _horizontalOrVerticalLayoutGroup.childForceExpandWidth = false;
                    _horizontalOrVerticalLayoutGroup.childForceExpandHeight = false;
                    _horizontalOrVerticalLayoutGroup.padding.left = 10;
                    _horizontalOrVerticalLayoutGroup.padding.right= 10;
                    _horizontalOrVerticalLayoutGroup.padding.top = 10;
                    _horizontalOrVerticalLayoutGroup.padding.bottom = 10;
                    _horizontalOrVerticalLayoutGroup.spacing = 10;

                    isClick = false;
                }

                EditorGUILayout.HelpBox("VerticalLayoutGroup or HorizontalLayoutGroup \n 둘중 하나의 컴포넌트를 삽입해서 설정해주세요.", MessageType.Warning);

            }
            else
            {
                if (GUILayout.Button("-=리셋버튼=-"))
                {
                    //자식들 초기화(삭제-)
                    int count = module_Base.transform.childCount;
                    for (int i = 0; i < count; i++)
                    {
                        //배열중에 삭제라 배열에서 곧장 삭제해버리면 에러남
                        //DestroyImmediate는 게임오브젝트만 삭제가능
                        DestroyImmediate(module_Base.transform.GetChild(0).gameObject);
                    }

                    DestroyImmediate(module_Base.gameObject.GetComponent<Image>());
                    DestroyImmediate(module_Base.gameObject.GetComponent<UIView>());
                    DestroyImmediate(module_Base.gameObject.GetComponent<ContentSizeFitter>());
                    if (module_Base.TryGetComponent(out VerticalLayoutGroup _verticalLayout))
                    {
                        DestroyImmediate(_verticalLayout);
                    }
                    if (module_Base.TryGetComponent(out HorizontalLayoutGroup _horizontalLayout))
                    {
                        DestroyImmediate(_horizontalLayout);
                    }
                    if (module_Base.TryGetComponent(out LayoutElement _layout))
                    {
                        DestroyImmediate(_layout);
                    }
                    //module_Base.toolTipImage = null;
                    //module_Base.toolTipText = null;
                    //module_Base.toolTipView = null;
                    //module_Base.layoutElement = null;
                    module_Base.SendMessage("Reset");
                }
                EditorGUILayout.HelpBox("Padding, Spacing 을 설정해주세요 \n ControlChildSize의 width = true, height = true 로 되있는지 확인해주세요 ", MessageType.Info);
            }
        }
        private void CreateObjectInitSetting(ToolTipUiView _target)
        {
            _target.gameObject.name = "툴팁 박스_";
            GameObject go;

            if (_target.TryGetComponent(out RectTransform _rect))
            {
                _rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 15, 0);
                _rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 15, 0);
                _rect.position = Vector3.zero;
                _rect.pivot = Vector2.zero;
            }
            Image image = _target.gameObject.AddComponent<Image>();//바탕
            image.color = new Color(0.5f, 0.5f, 0.5f);
            _target.toolTipView = _target.gameObject.AddComponent<UIView>();//UIView
            _target.toolTipView.UseCustomStartAnchoredPosition = false;

            //컨텐즈사이즈 필터 설정
            ContentSizeFitter contentSizeFitter = _target.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //레이아웃
            _target.layoutElement = _target.gameObject.AddComponent<LayoutElement>();
            _target.layoutElement.preferredWidth = _target.charLimit;


            //툴팁아이콘 이미지 설정
            go = new GameObject();
            go.name = "툴팁 아이콘 이미지";
            go.transform.parent = _target.transform;
            _target.toolTipImage = go.AddComponent<Image>();
            _target.toolTipImage.raycastTarget = false;
            go.transform.position = _target.transform.position;
            go.transform.localScale = Vector3.one;

            //툴팁 텍스트 설정
            go = new GameObject();
            go.name = "툴팁 텍스트";
            go.transform.parent = _target.transform;
            _target.toolTipText = go.AddComponent<TextMeshProUGUI>();
            _target.toolTipText.text = "Text ToolTip..";            
            _target.toolTipText.color = new Color(0, 0, 0);
            //toolTipText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 15, 500);
            //toolTipText.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 15, 100);
            _target.toolTipText.rectTransform.position = Vector3.zero;
            //toolTipText.rectTransform.rect.Set(4, 4, 0, 0);
            _target.toolTipText.enableWordWrapping = true;
            _target.toolTipText.overflowMode = TextOverflowModes.Overflow;
            _target.toolTipText.alignment = TextAlignmentOptions.BottomLeft;

            go.transform.position = _target.transform.position;
            go.transform.localScale = Vector3.one;
        }
    }
}