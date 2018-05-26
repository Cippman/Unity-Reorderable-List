using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CippSharp.Reorderable
{
    [Serializable]
    public class UnityEventBaseReferences
    {
        [SerializeField] private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;
        [SerializeField] private UnityEngine.Object m_Target = null;
        [SerializeField] private string m_MethodName = "";

        [SerializeField, HideInInspector]
        private PersistentListenerMode m_PersistentListenerMode = PersistentListenerMode.EventDefined;

#if UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element">must be a persistent call</param>
        public UnityEventBaseReferences(SerializedProperty element)
        {
            this.m_CallState = (UnityEventCallState) element.FindPropertyRelative("m_CallState").enumValueIndex;
            this.m_Target = element.FindPropertyRelative("m_Target").objectReferenceValue;
            this.m_MethodName = element.FindPropertyRelative("m_MethodName").stringValue;
            this.m_PersistentListenerMode = (PersistentListenerMode) element.FindPropertyRelative("m_Mode").enumValueIndex;
        }
#endif
    }

    public class UnityEventsHolder : ScriptableObject
    {
        private static UnityEventsHolder instance = null;
        
        public static UnityEventsHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<UnityEventsHolder>();
                }

                return instance;
            }
        }

        private static Dictionary<int, List<UnityEventBaseReferences>> unityEvents = new Dictionary<int, List<UnityEventBaseReferences>>();

      
        
        public static void GetList(int id)
        {
            
        }

    }
}