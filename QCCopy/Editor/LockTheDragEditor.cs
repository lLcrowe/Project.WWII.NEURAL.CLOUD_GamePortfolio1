using UnityEngine;
using UnityEditor;

namespace lLCroweTool.QC.EditorOnly
{
    //20230609//아짜리님 요구사항 체크후 제작
    public class LockTheDragEditor : EditorWindow
    {
        public static bool checkHierarchy = false;
        public static bool checkProject = false;

        //윈도우창 샘플
        [MenuItem("lLcroweTool/LockTheDragEditor")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(LockTheDragEditor));
            editorWindow.titleContent.text = "락 관리자";
            editorWindow.minSize = new Vector2(200, 105);
            editorWindow.maxSize = new Vector2(200, 105);
        }

        private void OnGUI()
        {
            string content = checkProject ? "-=프로젝트창 잠금 활성화=-" : "-=프로젝트창 잠금 비활성화=-";
            EditorGUILayout.LabelField(content);

            if (GUILayout.Button("잠금변경"))
            {
                if (checkProject)
                {
                    DragAndDrop.RemoveDropHandler(ProjectHandler);
                    checkProject = false;
                }
                else
                {
                    DragAndDrop.AddDropHandler(ProjectHandler);
                    checkProject = true;
                }
            }

            content = checkHierarchy ? "-=하이어라키창 잠금 활성화=-" : "-=하이어라키창 잠금 비활성화=-";
            EditorGUILayout.LabelField(content);
            if (GUILayout.Button("잠금변경"))
            {
                if (checkHierarchy)
                {
                    DragAndDrop.RemoveDropHandler(HierarchyHandler);
                    checkHierarchy = false;
                }
                else
                {
                    DragAndDrop.AddDropHandler(HierarchyHandler);
                    checkHierarchy = true;
                }
            }

            EditorGUILayout.LabelField("Creat by lLcrowe");
            //HierarchyDropFlags.
            //DragAndDrop.AddDropHandler();
            //EditorApplication.hierarchyChanged += Func;
            //EditorApplication.projectChanged += Func;
            //DragAndDrop.AddDropHandler(SceneHandler);
            //DragAndDrop.AddDropHandler(InspectorHandler);
        }

        private static DragAndDropVisualMode GetSettingMode(bool checkValue)
        {
            //var data = DragAndDropVisualMode.None;
            var data = DragAndDropVisualMode.Generic;
            if (checkValue)
            {
                data = DragAndDropVisualMode.Rejected;
            }
            return data;
        }

        public static DragAndDropVisualMode HierarchyHandler(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            //false//드래그
            //true//드랍
            //Debug.Log(perform);

            return GetSettingMode(checkHierarchy);
        }

        public static DragAndDropVisualMode ProjectHandler(int id, string path, bool perform)
        {
            return GetSettingMode(checkProject);
        }


        //public static DragAndDropVisualMode InspectorHandler(UnityEngine.Object[] targets, bool perform)
        //{
        //    //?
        //    return GetSettingMode();
        //}


        //public static DragAndDropVisualMode SceneHandler(UnityEngine.Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, Transform parentForDraggedObjects, bool perform)
        //{
        //    //?
        //    return GetSettingMode();
        //}


        //static void Func()
        //{
        //    Debug.Log($"업데이트 Test! {Selection.count}");
        //}
    }

    ////에셋데이터베이스 처리되기 전
    //public class TestAssetModificationProcessor : AssetModificationProcessor
    //{
    //    //https://docs.unity3d.com/ScriptReference/AssetModificationProcessor.html
    //    //failed로 리턴해주면 다이어로그 창뜨고 작동중지됨
    //    static void OnWillCreateAsset(string assetName)
    //    {
    //        Debug.Log("OnWillCreateAsset is being called with the following asset: " + assetName + ".");
    //    }

    //    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    //    {
    //        Debug.Log("OnWillDeleteAsset");
    //        Debug.Log(path);
    //        return AssetDeleteResult.DidDelete;
    //    }

    //    private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
    //    {
    //        Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
    //        AssetMoveResult assetMoveResult = AssetMoveResult.DidMove;

    //        // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.

    //        return assetMoveResult;
    //    }
    //}

    ////에셋데이터베이스 처리 후
    //public class TestCustomPostprocessor : AssetPostprocessor
    //{
    //    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //    {
    //        //importedAssets//임포트된
    //        //deletedAssets//지운
    //        //movedAssets//움직인 대상
    //        //movedFromAssetPaths//움직인 대상의 원래 위치

    //        for (int i = 0; i < movedAssets.Length; i++)
    //        {
    //            Debug.Log($"{movedFromAssetPaths[i]} => ({movedAssets[i]}) Move");
    //        }
    //    }

    //    private void OnPreprocessAsset()
    //    {
    //        //처리완료
    //    }
    //}
}

