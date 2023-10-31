using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.UIThema
{
    public class ThemaIcon : MonoBehaviour
    {
        //특정아이디값을 통해 처리
        [ThemaIcon] public string iconID;
        public Image image;

        public string IconID { get => iconID; }

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            var sprite = ThemaUIManager.Instance.RequestIcon(iconID);
            SetImage(sprite);
        }

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}