using Sodanen.LightControl;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Brightness.Utility
{
    /// <summary>
    /// Light Control 관련 메뉴 아이템
    /// </summary>
    public static class SodanenLightMenuItems
    {
        private const string MENU_PATH = "GameObject/Sodanen/Add Light Control";
        private const string COMPONENT_MENU_PATH = "Sodanen/Add Light Control to Avatar";

        /// <summary>
        /// 선택된 아바타에 Light Control 추가
        /// </summary>
        [MenuItem(MENU_PATH, false, 10)]
        public static void AddLightControlToSelection()
        {
            var selection = Selection.activeGameObject;
            if (selection == null)
            {
                EditorUtility.DisplayDialog("오류", "먼저 아바타를 선택해주세요.", "확인");
                return;
            }

            // 아바타 루트 찾기
            var avatarRoot = FindAvatarRoot(selection);
            if (avatarRoot == null)
            {
                EditorUtility.DisplayDialog("오류", "선택된 오브젝트가 VRChat 아바타 내부에 없습니다.", "확인");
                return;
            }

            // 이미 존재하는지 확인
            var existing = avatarRoot.GetComponentInChildren<SodanenLightControl>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("알림", "이미 Light Control이 존재합니다.", "확인");
                Selection.activeObject = existing.gameObject;
                return;
            }

            // 새 오브젝트 생성
            var lightObject = new GameObject("Sodanen Light Control");
            lightObject.transform.SetParent(avatarRoot.transform);
            lightObject.transform.localPosition = Vector3.zero;
            lightObject.transform.localRotation = Quaternion.identity;
            lightObject.transform.localScale = Vector3.one;

            var component = lightObject.AddComponent<SodanenLightControl>();

            Undo.RegisterCreatedObjectUndo(lightObject, "Add Light Control");
            Selection.activeObject = lightObject;

            Debug.Log($"[SodanenLight] '{avatarRoot.name}' 아바타에 Light Control 추가됨");
        }

        [MenuItem(MENU_PATH, true)]
        public static bool ValidateAddLightControl()
        {
            return Selection.activeGameObject != null;
        }

        /// <summary>
        /// 메뉴바에서 아바타에 추가
        /// </summary>
        [MenuItem(COMPONENT_MENU_PATH)]
        public static void AddLightControlFromMenu()
        {
            // 씬의 모든 아바타 찾기
            var avatars = Object.FindObjectsByType<VRCAvatarDescriptor>(FindObjectsSortMode.None);

            if (avatars.Length == 0)
            {
                EditorUtility.DisplayDialog("오류", "씬에 VRChat 아바타가 없습니다.", "확인");
                return;
            }

            if (avatars.Length == 1)
            {
                AddLightControlToAvatar(avatars[0].gameObject);
                return;
            }

            // 여러 아바타가 있으면 GenericMenu로 선택
            var menu = new GenericMenu();
            foreach (var avatar in avatars)
            {
                var avatarObj = avatar.gameObject;
                menu.AddItem(new GUIContent(avatarObj.name), false, () =>
                {
                    AddLightControlToAvatar(avatarObj);
                });
            }
            menu.ShowAsContext();
        }

        private static void AddLightControlToAvatar(GameObject avatarRoot)
        {
            var existing = avatarRoot.GetComponentInChildren<SodanenLightControl>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("알림", $"'{avatarRoot.name}'에 이미 Light Control이 존재합니다.", "확인");
                Selection.activeObject = existing.gameObject;
                return;
            }

            var lightObject = new GameObject("Sodanen Light Control");
            lightObject.transform.SetParent(avatarRoot.transform);
            lightObject.transform.localPosition = Vector3.zero;
            lightObject.transform.localRotation = Quaternion.identity;
            lightObject.transform.localScale = Vector3.one;

            lightObject.AddComponent<SodanenLightControl>();

            Undo.RegisterCreatedObjectUndo(lightObject, "Add Light Control");
            Selection.activeObject = lightObject;

            Debug.Log($"[SodanenLight] '{avatarRoot.name}' 아바타에 Light Control 추가됨");
        }

        private static GameObject FindAvatarRoot(GameObject obj)
        {
            var current = obj.transform;
            while (current != null)
            {
                if (current.GetComponent<VRCAvatarDescriptor>() != null)
                    return current.gameObject;
                current = current.parent;
            }
            return null;
        }
    }
}
