#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Scenes;

namespace Editor {
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            var sceneName = property.FindPropertyRelative("sceneName");

            EditorGUI.BeginChangeCheck();

            var value = EditorGUI.ObjectField(position, label, sceneAsset.objectReferenceValue, 
                typeof(SceneAsset), false);

            if (EditorGUI.EndChangeCheck()) {
                sceneAsset.objectReferenceValue = value;
                if (sceneAsset.objectReferenceValue) sceneName.stringValue = sceneAsset.objectReferenceValue.name;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
