using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using CippSharp.Reorderable;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CippSharpEditor.Reorderable
{
    [CustomPropertyDrawer(typeof(ReorderableUnityEventAttribute))]
    public class ReorderableUnityEventDrawer : PropertyDrawer
    {
        private static MethodInfo FindMethod = typeof(UnityEventBase).GetMethod("FindMethod",BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        private static PropertyInfo mixedValueContent = typeof(EditorGUI).GetProperty("mixedValueContent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        private static MethodInfo Temp = typeof(GUIContent).GetMethod("Temp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        
        
        private static Dictionary<int, ReorderableList> lists = new Dictionary<int, ReorderableList>();
        private static object cache0;
        private static object cache1;
        
        private UnityEventBase m_DummyEvent = null;
        
        
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReorderableUnityEventAttribute unityEventAttribute = attribute as ReorderableUnityEventAttribute;

            ReorderableList list = GetList(property, new ReorderableAttribute());

            return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        private SerializedProperty calls;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            ReorderableUnityEventAttribute unityEventAttribute = attribute as ReorderableUnityEventAttribute;
            calls = property.FindPropertyRelative("m_PersistentCalls.m_Calls");
            if (calls == null)
            {
                GUI.Label(position, "ReorderableUnityEvent attribute work with UnityEvents only.", EditorStyles.label);
                return;
            }

            m_DummyEvent = GetDummyEvent(property);
            if (m_DummyEvent == null)
            {
                GUI.Label(position, "Failed to retrieve Dummy Event.", EditorStyles.label);
                return;
            }

            if (calls != null)
            {
                ReorderableList list = GetList(property, new ReorderableAttribute());
                if (list != null)
                {
                    list.drawElementCallback += DrawElementCallback;
                    list.DoList(EditorGUI.IndentedRect(position), label);
                    list.drawElementCallback -= DrawElementCallback;
                }
            }
        }

        #region UnityEventDrawer Replica
        private static UnityEventBase GetDummyEvent(SerializedProperty prop)
        {
            System.Type type = System.Type.GetType(prop.FindPropertyRelative("m_TypeName").stringValue, false);
            if (type == null)
                return (UnityEventBase) new UnityEvent();
            return Activator.CreateInstance(type) as UnityEventBase;
        }
        
        private void DrawElementCallback(Rect rect, SerializedProperty element, GUIContent guiContent, bool selected, bool focused)
        {
            ++rect.y;
            Rect[] rowRects = GetRowRects(rect);
            Rect position1 = rowRects[0];
            Rect position2 = rowRects[1];
            Rect rect1 = rowRects[2];
            Rect position3 = rowRects[3];
            SerializedProperty callState = element.FindPropertyRelative("m_CallState");
            SerializedProperty listenerMode = element.FindPropertyRelative("m_Mode");
            SerializedProperty argumentsCache = element.FindPropertyRelative("m_Arguments");
            SerializedProperty target = element.FindPropertyRelative("m_Target");
            SerializedProperty methdoName = element.FindPropertyRelative("m_MethodName");
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            EditorGUI.PropertyField(position1, callState, GUIContent.none);
            EditorGUI.BeginChangeCheck();
            GUI.Box(position2, GUIContent.none);
            EditorGUI.PropertyField(position2, target, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                methdoName.stringValue = (string) null;
            }
            
            PersistentListenerMode persistentListenerMode = (PersistentListenerMode)listenerMode.enumValueIndex;
            if (target.objectReferenceValue == (UnityEngine.Object) null || string.IsNullOrEmpty(methdoName.stringValue))
            {
                persistentListenerMode = PersistentListenerMode.Void;
            }
            SerializedProperty argumentsCacheValue;
            switch (persistentListenerMode)
            {
                case PersistentListenerMode.Object:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_ObjectArgument");
                    break;
                case PersistentListenerMode.Int:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_IntArgument");
                    break;
                case PersistentListenerMode.Float:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_FloatArgument");
                    break;
                case PersistentListenerMode.String:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_StringArgument");
                    break;
                case PersistentListenerMode.Bool:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_BoolArgument");
                    break;
                default:
                    argumentsCacheValue = argumentsCache.FindPropertyRelative("m_IntArgument");
                    break;
            }
            
            string stringValue = argumentsCache.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
            Type type = typeof(UnityEngine.Object);
            if (!string.IsNullOrEmpty(stringValue))
            {
                type = Type.GetType(stringValue, false) ?? typeof(UnityEngine.Object);
            }

            switch (persistentListenerMode)
            {
                case PersistentListenerMode.EventDefined:
                case PersistentListenerMode.Void:
                    using (new EditorGUI.DisabledScope(target.objectReferenceValue == (UnityEngine.Object) null))
                    {
                        EditorGUI.BeginProperty(rect1, GUIContent.none, methdoName);
                        GUIContent content;
                        if (EditorGUI.showMixedValue)
                        {
                            content = (GUIContent)mixedValueContent.GetValue(Activator.CreateInstance(typeof(EditorGUI)), null);
                            //EditorGUI.mixedValueContent;
                        }
                        else
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            if (target.objectReferenceValue == (UnityEngine.Object) null || string.IsNullOrEmpty(methdoName.stringValue))
                            {
                                stringBuilder.Append("No Function");
                            }
                            else if (!IsPersistantListenerValid(this.m_DummyEvent, methdoName.stringValue, target.objectReferenceValue, persistentListenerMode, type))
                            {
                                string str = "UnknownComponent";
                                UnityEngine.Object objectReferenceValue = target.objectReferenceValue;
                                if (objectReferenceValue != (UnityEngine.Object) null)
                                {
                                    str = objectReferenceValue.GetType().Name;
                                }
                                stringBuilder.Append(string.Format("<Missing {0}.{1}>", (object) str, (object) methdoName.stringValue));
                            }
                            else
                            {
                                stringBuilder.Append(target.objectReferenceValue.GetType().Name);
                                if (!string.IsNullOrEmpty(methdoName.stringValue))
                                {
                                    stringBuilder.Append(".");
                                    if (methdoName.stringValue.StartsWith("set_"))
                                    {
                                        stringBuilder.Append(methdoName.stringValue.Substring(4));
                                    }
                                    else
                                    {
                                        stringBuilder.Append(methdoName.stringValue);
                                    }
                                }
                            }

                            content = (GUIContent)Temp.Invoke(Activator.CreateInstance(typeof(GUIContent)), new object[] {stringBuilder.ToString()});
                            //GUIContent.Temp(stringBuilder.ToString());
                        }

                        if (GUI.Button(rect1, content, EditorStyles.popup))
                        {
                            BuildPopupList(target.objectReferenceValue, this.m_DummyEvent, element).DropDown(rect1);
                        }
                        EditorGUI.EndProperty();
                    }
                    GUI.backgroundColor = backgroundColor;
                    break;
                case PersistentListenerMode.Object:
                    EditorGUI.BeginChangeCheck();
                    UnityEngine.Object @object = EditorGUI.ObjectField(position3, GUIContent.none, argumentsCacheValue.objectReferenceValue, type, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        argumentsCacheValue.objectReferenceValue = @object;
                        goto case PersistentListenerMode.EventDefined;
                    }
                    else
                    {
                        goto case PersistentListenerMode.EventDefined;
                    }
                default:
                    EditorGUI.PropertyField(position3, argumentsCacheValue, GUIContent.none);
                    goto case PersistentListenerMode.EventDefined;
            }
        }

        private Rect[] GetRowRects(Rect rect)
        {
            Rect[] rectArray = new Rect[4];
            rect.height = 16f;
            rect.y += 2f;
            Rect rect1 = rect;
            rect1.width *= 0.3f;
            Rect rect2 = rect1;
            rect2.y += EditorGUIUtility.singleLineHeight + 2f;
            Rect rect3 = rect;
            rect3.xMin = rect2.xMax + 5f;
            Rect rect4 = rect3;
            rect4.y += EditorGUIUtility.singleLineHeight + 2f;
            rectArray[0] = rect1;
            rectArray[1] = rect2;
            rectArray[2] = rect3;
            rectArray[3] = rect4;
            return rectArray;
        }
        
        private static bool IsPersistantListenerValid(UnityEventBase dummyEvent, string methodName, UnityEngine.Object uObject, PersistentListenerMode modeEnum, System.Type argumentType)
        {
            if (uObject == (UnityEngine.Object) null || string.IsNullOrEmpty(methodName))
            {
                return false;
            }
            return FindMethod.Invoke(dummyEvent, new object[] {methodName, (object) uObject, (object)modeEnum, argumentType}) != null;
            //dummyEvent.FindMethod(methodName, (object) uObject, modeEnum, argumentType) != null;
        }
        

        private static GenericMenu BuildPopupList(UnityEngine.Object target, UnityEventBase dummyEvent, SerializedProperty listener)
        {
            UnityEngine.Object tmpTarget = target;
            if (tmpTarget is Component)
            {
                tmpTarget = (UnityEngine.Object) (target as Component).gameObject;
            }
            
            SerializedProperty methodName = listener.FindPropertyRelative("m_MethodName");
            GenericMenu menu = new GenericMenu();
            GenericMenu genericMenu = menu;
            GUIContent content = new GUIContent("No Function");
            int num = string.IsNullOrEmpty(methodName.stringValue) ? 1 : 0;
            
            if (cache0 == null)
            {
                cache0 = new GenericMenu.MenuFunction2(ClearEventFunction);
            }
            
            GenericMenu.MenuFunction2 fMgCache0 = (GenericMenu.MenuFunction2)cache0;
            object local = (ValueType) new UnityEventFunction(listener, (UnityEngine.Object) null, (MethodInfo) null, PersistentListenerMode.EventDefined);
            genericMenu.AddItem(content, num != 0, fMgCache0, (object) local);
            
            if (tmpTarget == (UnityEngine.Object) null)
            {
                return menu;
            }
            
            menu.AddSeparator("");
            System.Type[] array = ((IEnumerable<System.Reflection.ParameterInfo>) dummyEvent.GetType().GetMethod("Invoke").GetParameters()).Select<System.Reflection.ParameterInfo, System.Type>((Func<System.Reflection.ParameterInfo, System.Type>) (x => x.ParameterType)).ToArray<System.Type>();
            GeneratePopUpForType(menu, tmpTarget, false, listener, array);
            if (tmpTarget is GameObject)
            {
                Component[] components = (tmpTarget as GameObject).GetComponents<Component>();
                List<string> list = ((IEnumerable<Component>) components).Where<Component>((Func<Component, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) null)).Select<Component, string>((Func<Component, string>) (c => c.GetType().Name)).GroupBy<string, string>((Func<string, string>) (x => x)).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (g => g.Count<string>() > 1)).Select<IGrouping<string, string>, string>((Func<IGrouping<string, string>, string>) (g => g.Key)).ToList<string>();
                foreach (Component component in components)
                {
                    if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
                    {
                        GeneratePopUpForType(menu, (UnityEngine.Object) component, list.Contains(component.GetType().Name), listener, array);
                    }
                }
            }

            return menu;
        }

        private struct UnityEventFunction
        {
            private readonly SerializedProperty m_Listener;
            private readonly UnityEngine.Object m_Target;
            private readonly MethodInfo m_Method;
            private readonly PersistentListenerMode m_Mode;

            public UnityEventFunction(SerializedProperty listener, UnityEngine.Object target, MethodInfo method,
                PersistentListenerMode mode)
            {
                this.m_Listener = listener;
                this.m_Target = target;
                this.m_Method = method;
                this.m_Mode = mode;
            }

            public void Assign()
            {
                SerializedProperty propertyRelative1 = this.m_Listener.FindPropertyRelative("m_Target");
                SerializedProperty propertyRelative2 = this.m_Listener.FindPropertyRelative("m_MethodName");
                SerializedProperty propertyRelative3 = this.m_Listener.FindPropertyRelative("m_Mode");
                SerializedProperty propertyRelative4 = this.m_Listener.FindPropertyRelative("m_Arguments");
                propertyRelative1.objectReferenceValue = this.m_Target;
                propertyRelative2.stringValue = this.m_Method.Name;
                propertyRelative3.enumValueIndex = (int) this.m_Mode;
                if (this.m_Mode == PersistentListenerMode.Object)
                {
                    SerializedProperty propertyRelative5 =
                        propertyRelative4.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                    System.Reflection.ParameterInfo[] parameters = this.m_Method.GetParameters();
                    propertyRelative5.stringValue =
                        parameters.Length != 1 ||
                        !typeof(UnityEngine.Object).IsAssignableFrom(parameters[0].ParameterType)
                            ? typeof(UnityEngine.Object).AssemblyQualifiedName
                            : parameters[0].ParameterType.AssemblyQualifiedName;
                }

                this.ValidateObjectParamater(propertyRelative4, this.m_Mode);
                this.m_Listener.serializedObject.ApplyModifiedProperties();
            }

            private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
            {
                SerializedProperty propertyRelative1 =
                    arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                SerializedProperty propertyRelative2 = arguments.FindPropertyRelative("m_ObjectArgument");
                UnityEngine.Object objectReferenceValue = propertyRelative2.objectReferenceValue;
                if (mode != PersistentListenerMode.Object)
                {
                    propertyRelative1.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
                    propertyRelative2.objectReferenceValue = (UnityEngine.Object) null;
                }
                else
                {
                    if (objectReferenceValue == (UnityEngine.Object) null)
                        return;
                    System.Type type = System.Type.GetType(propertyRelative1.stringValue, false);
                    if (typeof(UnityEngine.Object).IsAssignableFrom(type) &&
                        type.IsInstanceOfType((object) objectReferenceValue))
                        return;
                    propertyRelative2.objectReferenceValue = (UnityEngine.Object) null;
                }
            }

            public void Clear()
            {
                this.m_Listener.FindPropertyRelative("m_MethodName").stringValue = (string) null;
                this.m_Listener.FindPropertyRelative("m_Mode").enumValueIndex = 1;
                this.m_Listener.serializedObject.ApplyModifiedProperties();
            }
        }
        
        private static void ClearEventFunction(object source)
        {
            ((UnityEventFunction)source).Clear();
        }

        private static void GeneratePopUpForType(GenericMenu menu, UnityEngine.Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
        {
            List<ValidMethodMap> methods = new List<ValidMethodMap>();
            string targetName = !useFullTargetName ? target.GetType().Name : target.GetType().FullName;
            bool flag = false;
            if (delegateArgumentsTypes.Length != 0)
            {
                GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods, PersistentListenerMode.EventDefined);
                if (methods.Count > 0)
                {
                    menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " + string.Join(", ",
                                                            ((IEnumerable<System.Type>) delegateArgumentsTypes)
                                                            .Select<System.Type, string>(
                                                                (Func<System.Type, string>) (e =>
                                                                    GetTypeName(e)))
                                                            .ToArray<string>())));
                    AddMethodsToMenu(menu, listener, methods, targetName);
                    flag = true;
                }
            }

            methods.Clear();
            GetMethodsForTargetAndMode(target, new System.Type[1]
            {
                typeof(float)
            }, methods, PersistentListenerMode.Float);
            GetMethodsForTargetAndMode(target, new System.Type[1]
            {
                typeof(int)
            }, methods, PersistentListenerMode.Int);
            GetMethodsForTargetAndMode(target, new System.Type[1]
            {
                typeof(string)
            }, methods, PersistentListenerMode.String);
            GetMethodsForTargetAndMode(target, new System.Type[1]
            {
                typeof(bool)
            }, methods, PersistentListenerMode.Bool);
            GetMethodsForTargetAndMode(target, new System.Type[1]
            {
                typeof(UnityEngine.Object)
            }, methods, PersistentListenerMode.Object);
            GetMethodsForTargetAndMode(target, new System.Type[0], methods,
                PersistentListenerMode.Void);
            if (methods.Count <= 0)
            {
                return;
            }

            if (flag)
            {
                menu.AddItem(new GUIContent(targetName + "/ "), false, (GenericMenu.MenuFunction) null);
            }

            if (delegateArgumentsTypes.Length != 0)
            {
                menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
            }
            
            AddMethodsToMenu(menu, listener, methods, targetName);
        }
        
        private struct ValidMethodMap
        {
            public UnityEngine.Object target;
            public MethodInfo methodInfo;
            public PersistentListenerMode mode;
        }
        
        private static void GetMethodsForTargetAndMode(UnityEngine.Object target, System.Type[] delegateArgumentsTypes, List<ValidMethodMap> methods, PersistentListenerMode mode)
        {
            ValidMethodMap[] validMethodMaps = CalculateMethodMap(target, delegateArgumentsTypes, mode == PersistentListenerMode.Object).ToArray();
            
            for (int i = 0; i < validMethodMaps.Length; i++)
            {
                var method = validMethodMaps[i];
                method.mode = mode;
                methods.Add(method);
            }
        }
        
        private static IEnumerable<ValidMethodMap> CalculateMethodMap(UnityEngine.Object target, System.Type[] t, bool allowSubclasses)
        {
            List<ValidMethodMap> validMethodMapList = new List<ValidMethodMap>();
            if (target == (UnityEngine.Object) null || t == null)
            {
                return (IEnumerable<ValidMethodMap>) validMethodMapList;
            }
            System.Type type = target.GetType();
            List<MethodInfo> list = ((IEnumerable<MethodInfo>) type.GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (x => !x.IsSpecialName)).ToList<MethodInfo>();
            IEnumerable<PropertyInfo> source = ((IEnumerable<PropertyInfo>) type.GetProperties()).AsEnumerable<PropertyInfo>().Where<PropertyInfo>((Func<PropertyInfo, bool>) (x => x.GetCustomAttributes(typeof (ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null));
            list.AddRange(source.Select<PropertyInfo, MethodInfo>((Func<PropertyInfo, MethodInfo>) (x => x.GetSetMethod())));
            foreach (MethodInfo methodInfo in list)
            {
                System.Reflection.ParameterInfo[] parameters = methodInfo.GetParameters();
                if (parameters.Length == t.Length && methodInfo.GetCustomAttributes(typeof (ObsoleteAttribute), true).Length <= 0 && methodInfo.ReturnType == typeof (void))
                {
                    bool flag = true;
                    for (int index = 0; index < t.Length; ++index)
                    {
                        if (!parameters[index].ParameterType.IsAssignableFrom(t[index]))
                            flag = false;
                        if (allowSubclasses && t[index].IsAssignableFrom(parameters[index].ParameterType))
                            flag = true;
                    }
                    if (flag)
                        validMethodMapList.Add(new ValidMethodMap()
                        {
                            target = target,
                            methodInfo = methodInfo
                        });
                }
            }
            return (IEnumerable<ValidMethodMap>) validMethodMapList;
        }
        
        private static string GetTypeName(System.Type t)
        {
            if (t == typeof (int))
                return "int";
            if (t == typeof (float))
                return "float";
            if (t == typeof (string))
                return "string";
            if (t == typeof (bool))
                return "bool";
            return t.Name;
        }
        
        private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, string targetName)
        {
            foreach (ValidMethodMap method in (IEnumerable<ValidMethodMap>) methods
                .OrderBy<ValidMethodMap, int>(
                    (Func<ValidMethodMap, int>) (e => !e.methodInfo.Name.StartsWith("set_") ? 1 : 0))
                .ThenBy<ValidMethodMap, string>((Func<ValidMethodMap, string>) (e => e.methodInfo.Name)))
            {
                AddFunctionsForScript(menu, listener, method, targetName);
            }
        }

        private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method,
            string targetName)
        {
            PersistentListenerMode mode1 = method.mode;
            UnityEngine.Object objectReferenceValue = listener.FindPropertyRelative("m_Target").objectReferenceValue;
            string stringValue = listener.FindPropertyRelative("m_MethodName").stringValue;
            PersistentListenerMode mode2 =
                (PersistentListenerMode) listener.FindPropertyRelative("m_Mode").enumValueIndex;
            SerializedProperty propertyRelative = listener.FindPropertyRelative("m_Arguments")
                .FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
            StringBuilder stringBuilder = new StringBuilder();
            int length = method.methodInfo.GetParameters().Length;
            for (int index = 0; index < length; ++index)
            {
                System.Reflection.ParameterInfo parameter = method.methodInfo.GetParameters()[index];
                stringBuilder.Append(string.Format("{0}", (object) GetTypeName(parameter.ParameterType)));
                if (index < length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            bool flag = objectReferenceValue == method.target && stringValue == method.methodInfo.Name &&
                        mode1 == mode2;
            if (flag && mode1 == PersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
                flag &= method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName ==
                        propertyRelative.stringValue;
            string formattedMethodName = GetFormattedMethodName(targetName, method.methodInfo.Name,
                stringBuilder.ToString(), mode1 == PersistentListenerMode.EventDefined);
            GenericMenu genericMenu = menu;
            GUIContent content = new GUIContent(formattedMethodName);
            int num = flag ? 1 : 0;
            // ISSUE: reference to a compiler-generated field
            if (cache1 == null)
            {
                cache1 = new GenericMenu.MenuFunction2(SetEventFunction);
            }

            // ISSUE: reference to a compiler-generated field
            GenericMenu.MenuFunction2 fMgCache1 = (GenericMenu.MenuFunction2) cache1;
            // ISSUE: variable of a boxed type
            object local = (ValueType) new UnityEventFunction(listener, method.target, method.methodInfo, mode1);
            genericMenu.AddItem(content, num != 0, fMgCache1, (object) local);
        }

        private static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
        {
            if (dynamic)
            {
                if (methodName.StartsWith("set_"))
                {
                    return string.Format("{0}/{1}", (object) targetName, (object) methodName.Substring(4));
                }
                return string.Format("{0}/{1}", (object) targetName, (object) methodName);
            }

            if (methodName.StartsWith("set_"))
            {
                return string.Format("{0}/{2} {1}", (object) targetName, (object) methodName.Substring(4), (object) args);
            }
            return string.Format("{0}/{1} ({2})", (object) targetName, (object) methodName, (object) args);
        }

        private static void SetEventFunction(object source)
        {
            ((UnityEventFunction) source).Assign();
        }

        
        #endregion

        public static int GetListId(SerializedProperty property)
        {

            if (property != null)
            {

                int h1 = property.serializedObject.targetObject.GetHashCode();
                int h2 = property.propertyPath.GetHashCode();

                return (((h1 << 5) + h1) ^ h2);
            }

            return 0;
        }

        public static ReorderableList GetList(SerializedProperty property)
        {

            return GetList(property, null, GetListId(property));
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib)
        {

            return GetList(property, attrib, GetListId(property));
        }

        public static ReorderableList GetList(SerializedProperty property, int id)
        {

            return GetList(property, null, id);
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, int id)
        {

            if (property == null)
            {

                return null;
            }

            ReorderableList list = null;
            SerializedProperty array = property.FindPropertyRelative("m_PersistentCalls.m_Calls");

            if (array != null && array.isArray)
            {

                if (!lists.TryGetValue(id, out list))
                {

                    if (attrib != null)
                    {

                        Texture icon = !string.IsNullOrEmpty(attrib.elementIconPath)
                            ? AssetDatabase.GetCachedIcon(attrib.elementIconPath)
                            : null;

                        list = new ReorderableList(array, attrib.add, attrib.remove, attrib.draggable,
                            ReorderableList.ElementDisplayType.Auto, attrib.elementNameProperty,
                            attrib.elementNameOverride, icon);
                    }
                    else
                    {

                        list = new ReorderableList(array, true, true, true);
                    }

                    lists.Add(id, list);
                }
                else
                {

                    list.List = array;
                }
            }

            return list;
        }

    }
}