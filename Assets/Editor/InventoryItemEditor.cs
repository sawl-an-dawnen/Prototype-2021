using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryItem))]
public class InventoryItemEditor : Editor
{
    private SerializedProperty isEquipProp;
    private SerializedProperty itemNameProp;
    private SerializedProperty itemIconProp;
    private SerializedProperty itemButtonPrefabProp;
    private SerializedProperty itemInfoProp;
    private SerializedProperty equipTypeProp;
    private SerializedProperty equipNameProp;
    private SerializedProperty equipIconProp;
    private SerializedProperty equipAttackProp;
    private SerializedProperty equipDefenseProp;
    private SerializedProperty equipSpecialProp;
    private SerializedProperty specialInfoProp;

    private void OnEnable()
    {
        isEquipProp = serializedObject.FindProperty("isEquip");
        itemNameProp = serializedObject.FindProperty("itemName");
        itemIconProp = serializedObject.FindProperty("itemIcon");
        itemButtonPrefabProp = serializedObject.FindProperty("itemButtonPrefab");
        itemInfoProp = serializedObject.FindProperty("itemInfo");
        equipTypeProp = serializedObject.FindProperty("equipType");
        equipNameProp = serializedObject.FindProperty("equipName");
        equipIconProp = serializedObject.FindProperty("equipIcon");
        equipAttackProp = serializedObject.FindProperty("equipAttack");
        equipDefenseProp = serializedObject.FindProperty("equipDefense");
        equipSpecialProp = serializedObject.FindProperty("equipSpecial");
        specialInfoProp = serializedObject.FindProperty("specialInfo");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isEquipProp);

        if (isEquipProp.boolValue)
        {
            EditorGUILayout.PropertyField(equipTypeProp);
            EditorGUILayout.PropertyField(equipNameProp);
            EditorGUILayout.PropertyField(equipIconProp);
            EditorGUILayout.PropertyField(equipAttackProp);
            EditorGUILayout.PropertyField(equipDefenseProp);
            EditorGUILayout.PropertyField(equipSpecialProp);

            if (equipSpecialProp.boolValue)
            {
                EditorGUILayout.PropertyField(specialInfoProp);
            }
        }
        else
        {
            EditorGUILayout.PropertyField(itemNameProp);
            EditorGUILayout.PropertyField(itemIconProp);
            EditorGUILayout.PropertyField(itemButtonPrefabProp);
            EditorGUILayout.PropertyField(itemInfoProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
