using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquipComponents))]
public class EquipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EquipComponents eq = (EquipComponents)target;
        if (eq.childrenList.Count > 0)
        {
            for (int i = 0; i < eq.childrenList.Count; i++)
            {
                EquipComponents.Child level = eq.childrenList[i];
                level.assemblylevel = EditorGUILayout.IntSlider(eq.childrenList[i].childobject.name, eq.childrenList[i].AssemblyLevel, 0, eq.childrenList.Count-1);
                eq.childrenList[i] = level;
            }
        }

        //EditorGUI.BeginDisabledGroup(eq.childrenList.Count == eq.GetComponentsInChildren<Transform>().Length);
            if (GUILayout.Button("Add children for setting assembly order"))
            {
                eq.LevelSetter(); 
            }
        //EditorGUI.EndDisabledGroup();

    }
}
