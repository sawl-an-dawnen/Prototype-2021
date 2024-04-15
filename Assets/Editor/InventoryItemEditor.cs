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
    private SerializedProperty equippedProp;
    private SerializedProperty equipTypeProp;
    private SerializedProperty equipRarityProp;
    private SerializedProperty equipAttackProp;
    private SerializedProperty equipDefenseProp;
    private SerializedProperty specialInfoProp;

    private void OnEnable()
    {
        isEquipProp = serializedObject.FindProperty("isEquip");
        itemNameProp = serializedObject.FindProperty("itemName");
        itemIconProp = serializedObject.FindProperty("itemIcon");
        itemButtonPrefabProp = serializedObject.FindProperty("itemButtonPrefab");
        itemInfoProp = serializedObject.FindProperty("itemInfo");
        equippedProp = serializedObject.FindProperty("equipped");
        equipTypeProp = serializedObject.FindProperty("equipType");
        equipRarityProp = serializedObject.FindProperty("equipRarity");
        equipAttackProp = serializedObject.FindProperty("equipAttack");
        equipDefenseProp = serializedObject.FindProperty("equipDefense");
        specialInfoProp = serializedObject.FindProperty("specialInfo");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isEquipProp);
        EditorGUILayout.PropertyField(itemNameProp);
        EditorGUILayout.PropertyField(itemIconProp);

        if (isEquipProp.boolValue)
        {
            EditorGUILayout.PropertyField(equippedProp);
            EditorGUILayout.PropertyField(equipTypeProp);
            EditorGUILayout.PropertyField(equipRarityProp);
            EditorGUILayout.PropertyField(equipAttackProp);
            EditorGUILayout.PropertyField(equipDefenseProp);
            EditorGUILayout.PropertyField(specialInfoProp);
        }
        else
        {
            EditorGUILayout.PropertyField(itemButtonPrefabProp);
            EditorGUILayout.PropertyField(itemInfoProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
