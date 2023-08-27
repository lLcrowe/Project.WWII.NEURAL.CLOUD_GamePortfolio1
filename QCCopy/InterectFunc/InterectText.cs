using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace lLCroweTool
{
    public class InterectText : MonoBehaviour
    {
        public static InterectText Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<InterectText>();

                    if (ReferenceEquals(instance, null))
                    {
                        GameObject tmp = new GameObject();
                        instance = tmp.AddComponent<InterectText>();
                        tmp.AddComponent<TextMeshProUGUI>();
                        //tmp.AddComponent<RectTransform>();
                        tmp.name = "-=InterectText=-";
                        Canvas temp = FindObjectOfType<Canvas>();
                        instance.transform.parent = temp.transform;
                    }
                }
                return instance;
            }
        }
        private static InterectText instance;
        public TextMeshProUGUI targetText;

        private void Awake()
        {
            instance = this;
            targetText = GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        public void ShowInterectText(string content)
        {
            if (!gameObject.activeSelf || content == targetText.text)
            {
                gameObject.SetActive(true);
                targetText.text = content;
            }

        }
        public void OffInterectText()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }



        private void OnDestroy()
        {
            instance = null;
            targetText = null;
        }

    }

}
