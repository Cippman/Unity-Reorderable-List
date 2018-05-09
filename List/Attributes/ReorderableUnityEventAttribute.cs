using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CippSharp.Reorderable
{
    [Serializable]
    public class UnityEventBaseReferencesListHolder : ScriptableObject
    {
        [SerializeField] public List<UnityEventBaseReferences> references = new List<UnityEventBaseReferences>();
    }

    [Serializable]
    public class UnityEventBaseReferences
    {
        [SerializeField] private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;
        [SerializeField] private UnityEngine.Object m_Target = null;
        [SerializeField] private string m_MethodName = "";
        [SerializeField, HideInInspector] private PersistentListenerMode m_PersistentListenerMode = PersistentListenerMode.EventDefined;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">must be a persistent call</param>
        public UnityEventBaseReferences(SerializedProperty element)
        {
            this.m_CallState = (UnityEventCallState)element.FindPropertyRelative("m_CallState").enumValueIndex;
            this.m_Target = element.FindPropertyRelative("m_Target").objectReferenceValue;
            this.m_MethodName = element.FindPropertyRelative("m_MethodName").stringValue;
            this.m_PersistentListenerMode = (PersistentListenerMode)element.FindPropertyRelative("m_Mode").enumValueIndex;
        }
    }
    
    public class ReorderableUnityEventAttribute : PropertyAttribute
    {
        public readonly UnityEventBaseReferencesListHolder holder = ScriptableObject.CreateInstance<UnityEventBaseReferencesListHolder>();
    }
}
