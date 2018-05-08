using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using CippSharp.Reorderable;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditorInternal;

namespace CippSharpEditor.Reorderable
{
    [CustomPropertyDrawer(typeof(ReorderableUnityEventAttribute))]
    public class ReorderableUnityEventDrawer : PropertyDrawer
    {
        private static Dictionary<int, ReorderableList> lists = new Dictionary<int, ReorderableList>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReorderableUnityEventAttribute unityEventAttribute = attribute as ReorderableUnityEventAttribute;

            ReorderableList list = GetList(property, new ReorderableAttribute());

            return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableUnityEventAttribute unityEventAttribute = attribute as ReorderableUnityEventAttribute;
            SerializedProperty calls = property.FindPropertyRelative("m_PersistentCalls.m_Calls");

            if (calls != null)
            {
                ReorderableList list = GetList(property, new ReorderableAttribute());

                if (list != null)
                {
                    list.DoList(EditorGUI.IndentedRect(position), label);
                }
                else
                {
                    GUI.Label(position, "Property not found!", EditorStyles.label);
                }
            }
            else
            {
                GUI.Label(position, "ReorderableUnityEvent attribute work with UnityEvents only.", EditorStyles.label);
            }
        }

        /*private void DrawEventListener( Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(index);
            ++rect.y;
            Rect[] rowRects = this.GetRowRects(rect);
            Rect position1 = rowRects[0];
            Rect position2 = rowRects[1];
            Rect rect1 = rowRects[2];
            Rect position3 = rowRects[3];
            SerializedProperty propertyRelative1 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
            SerializedProperty propertyRelative2 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
            SerializedProperty propertyRelative3 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
            SerializedProperty propertyRelative4 = arrayElementAtIndex.FindPropertyRelative("m_Target");
            SerializedProperty propertyRelative5 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            EditorGUI.PropertyField(position1, propertyRelative1, GUIContent.none);
            EditorGUI.BeginChangeCheck();
            GUI.Box(position2, GUIContent.none);
            EditorGUI.PropertyField(position2, propertyRelative4, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                propertyRelative5.stringValue = (string) null;
            PersistentListenerMode persistentListenerMode = UnityEventDrawer.GetMode(propertyRelative2);
            if (propertyRelative4.objectReferenceValue == (UnityEngine.Object) null ||
                string.IsNullOrEmpty(propertyRelative5.stringValue))
                persistentListenerMode = PersistentListenerMode.Void;
            SerializedProperty propertyRelative6;
            switch (persistentListenerMode)
            {
                case PersistentListenerMode.Object:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_ObjectArgument");
                    break;
                case PersistentListenerMode.Int:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_IntArgument");
                    break;
                case PersistentListenerMode.Float:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_FloatArgument");
                    break;
                case PersistentListenerMode.String:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_StringArgument");
                    break;
                case PersistentListenerMode.Bool:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_BoolArgument");
                    break;
                default:
                    propertyRelative6 = propertyRelative3.FindPropertyRelative("m_IntArgument");
                    break;
            }

            string stringValue = propertyRelative3.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
            System.Type type = typeof(UnityEngine.Object);
            if (!string.IsNullOrEmpty(stringValue))
                type = System.Type.GetType(stringValue, false) ?? typeof(UnityEngine.Object);
            switch (persistentListenerMode)
            {
                case PersistentListenerMode.EventDefined:
                case PersistentListenerMode.Void:
                    using (new EditorGUI.DisabledScope(propertyRelative4.objectReferenceValue ==
                                                       (UnityEngine.Object) null))
                    {
                        EditorGUI.BeginProperty(rect1, GUIContent.none, propertyRelative5);
                        GUIContent content;
                        if (EditorGUI.showMixedValue)
                        {
                            content = EditorGUI.mixedValueContent;
                        }
                        else
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            if (propertyRelative4.objectReferenceValue == (UnityEngine.Object) null ||
                                string.IsNullOrEmpty(propertyRelative5.stringValue))
                                stringBuilder.Append("No Function");
                            else if (!UnityEventDrawer.IsPersistantListenerValid(this.m_DummyEvent, propertyRelative5.stringValue, propertyRelative4.objectReferenceValue, UnityEventDrawer.GetMode(propertyRelative2), type))
                            {
                                string str = "UnknownComponent";
                                UnityEngine.Object objectReferenceValue = propertyRelative4.objectReferenceValue;
                                if (objectReferenceValue != (UnityEngine.Object) null)
                                    str = objectReferenceValue.GetType().Name;
                                stringBuilder.Append(string.Format("<Missing {0}.{1}>", (object) str,
                                    (object) propertyRelative5.stringValue));
                            }
                            else
                            {
                                stringBuilder.Append(propertyRelative4.objectReferenceValue.GetType().Name);
                                if (!string.IsNullOrEmpty(propertyRelative5.stringValue))
                                {
                                    stringBuilder.Append(".");
                                    if (propertyRelative5.stringValue.StartsWith("set_"))
                                        stringBuilder.Append(propertyRelative5.stringValue.Substring(4));
                                    else
                                        stringBuilder.Append(propertyRelative5.stringValue);
                                }
                            }

                            content = GUIContent.Temp(stringBuilder.ToString());
                        }

                        if (GUI.Button(rect1, content, EditorStyles.popup))
                            UnityEventDrawer.BuildPopupList(propertyRelative4.objectReferenceValue, this.m_DummyEvent,
                                arrayElementAtIndex).DropDown(rect1);
                        EditorGUI.EndProperty();
                    }

                    GUI.backgroundColor = backgroundColor;
                    break;
                case PersistentListenerMode.Object:
                    EditorGUI.BeginChangeCheck();
                    UnityEngine.Object @object = EditorGUI.ObjectField(position3, GUIContent.none,
                        propertyRelative6.objectReferenceValue, type, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        propertyRelative6.objectReferenceValue = @object;
                        goto case PersistentListenerMode.EventDefined;
                    }
                    else
                        goto case PersistentListenerMode.EventDefined;
                default:
                    EditorGUI.PropertyField(position3, propertyRelative6, GUIContent.none);
                    goto case PersistentListenerMode.EventDefined;
            }
        }*/

//        private static GenericMenu BuildPopupList( /*Parameter with token 08005158*/
//            UnityEngine.Object target, /*Parameter with token 08005159*/
//            UnityEventBase dummyEvent, /*Parameter with token 0800515A*/SerializedProperty listener)
//        {
//            UnityEngine.Object target1 = target;
//            if (target1 is Component)
//                target1 = (UnityEngine.Object) (target as Component).gameObject;
//            SerializedProperty propertyRelative = listener.FindPropertyRelative("m_MethodName");
//            GenericMenu menu = new GenericMenu();
//            GenericMenu genericMenu = menu;
//            GUIContent content = new GUIContent("No Function");
//            int num = string.IsNullOrEmpty(propertyRelative.stringValue) ? 1 : 0;
//            if (UnityEventDrawer.\u003C\u003Ef__mg\u0024cache0 == null)
//            {
//                // ISSUE: method pointer
//                UnityEventDrawer.\u003C\u003Ef__mg\u0024cache0 =
//                    new GenericMenu.MenuFunction2((object) null, __methodptr(ClearEventFunction));
//            }
//
//            GenericMenu.MenuFunction2 fMgCache0 = UnityEventDrawer.\u003C\u003Ef__mg\u0024cache0;
//            // ISSUE: variable of a boxed type
//            __Boxed<UnityEventDrawer.UnityEventFunction> local =
//                (ValueType) new UnityEventDrawer.UnityEventFunction(listener, (UnityEngine.Object) null,
//                    (MethodInfo) null, PersistentListenerMode.EventDefined);
//            genericMenu.AddItem(content, num != 0, fMgCache0, (object) local);
//            if (target1 == (UnityEngine.Object) null)
//                return menu;
//            menu.AddSeparator("");
//            System.Reflection.ParameterInfo[] parameters = dummyEvent.GetType().GetMethod("Invoke").GetParameters();
//            if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache4 == null)
//            {
//                // ISSUE: method pointer
//                UnityEventDrawer.\u003C\u003Ef__am\u0024cache4 =
//                    new Func<System.Reflection.ParameterInfo, System.Type>((object) null,
//                        __methodptr(\u003CBuildPopupList\u003Em__4));
//            }
//
//            Func<System.Reflection.ParameterInfo, System.Type> fAmCache4 =
//                UnityEventDrawer.\u003C\u003Ef__am\u0024cache4;
//            System.Type[] array = ((IEnumerable<System.Reflection.ParameterInfo>) parameters)
//                .Select<System.Reflection.ParameterInfo, System.Type>(fAmCache4).ToArray<System.Type>();
//            UnityEventDrawer.GeneratePopUpForType(menu, target1, false, listener, array);
//            if (target1 is GameObject)
//            {
//                Component[] components = (target1 as GameObject).GetComponents<Component>();
//                Component[] componentArray = components;
//                if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache5 == null)
//                {
//                    // ISSUE: method pointer
//                    UnityEventDrawer.\u003C\u003Ef__am\u0024cache5 =
//                        new Func<Component, bool>((object) null, __methodptr(\u003CBuildPopupList\u003Em__5));
//                }
//
//                Func<Component, bool> fAmCache5 = UnityEventDrawer.\u003C\u003Ef__am\u0024cache5;
//                IEnumerable<Component> source1 = ((IEnumerable<Component>) componentArray).Where<Component>(fAmCache5);
//                if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache6 == null)
//                {
//                    // ISSUE: method pointer
//                    UnityEventDrawer.\u003C\u003Ef__am\u0024cache6 =
//                        new Func<Component, string>((object) null, __methodptr(\u003CBuildPopupList\u003Em__6));
//                }
//
//                Func<Component, string> fAmCache6 = UnityEventDrawer.\u003C\u003Ef__am\u0024cache6;
//                IEnumerable<string> source2 = source1.Select<Component, string>(fAmCache6);
//                if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache7 == null)
//                {
//                    // ISSUE: method pointer
//                    UnityEventDrawer.\u003C\u003Ef__am\u0024cache7 =
//                        new Func<string, string>((object) null, __methodptr(\u003CBuildPopupList\u003Em__7));
//                }
//
//                Func<string, string> fAmCache7 = UnityEventDrawer.\u003C\u003Ef__am\u0024cache7;
//                IEnumerable<IGrouping<string, string>> source3 = source2.GroupBy<string, string>(fAmCache7);
//                if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache8 == null)
//                {
//                    // ISSUE: method pointer
//                    UnityEventDrawer.\u003C\u003Ef__am\u0024cache8 =
//                        new Func<IGrouping<string, string>, bool>((object) null,
//                            __methodptr(\u003CBuildPopupList\u003Em__8));
//                }
//
//                Func<IGrouping<string, string>, bool> fAmCache8 = UnityEventDrawer.\u003C\u003Ef__am\u0024cache8;
//                IEnumerable<IGrouping<string, string>> source4 = source3.Where<IGrouping<string, string>>(fAmCache8);
//                if (UnityEventDrawer.\u003C\u003Ef__am\u0024cache9 == null)
//                {
//                    // ISSUE: method pointer
//                    UnityEventDrawer.\u003C\u003Ef__am\u0024cache9 =
//                        new Func<IGrouping<string, string>, string>((object) null,
//                            __methodptr(\u003CBuildPopupList\u003Em__9));
//                }
//
//                Func<IGrouping<string, string>, string> fAmCache9 = UnityEventDrawer.\u003C\u003Ef__am\u0024cache9;
//                List<string> list = source4.Select<IGrouping<string, string>, string>(fAmCache9).ToList<string>();
//                foreach (Component component in components)
//                {
//                    if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
//                        UnityEventDrawer.GeneratePopUpForType(menu, (UnityEngine.Object) component,
//                            list.Contains(component.GetType().Name), listener, array);
//                }
//            }
//
//            return menu;
//        }
//
//        // Method GeneratePopUpForType with token 06004A74
//        private static void GeneratePopUpForType( /*Parameter with token 0800515B*/
//            GenericMenu menu, /*Parameter with token 0800515C*/
//            UnityEngine.Object target, /*Parameter with token 0800515D*/
//            bool useFullTargetName, /*Parameter with token 0800515E*/
//            SerializedProperty listener, /*Parameter with token 0800515F*/System.Type[] delegateArgumentsTypes)
//        {
//            List<UnityEventDrawer.ValidMethodMap> methods = new List<UnityEventDrawer.ValidMethodMap>();
//            string targetName = !useFullTargetName ? target.GetType().Name : target.GetType().FullName;
//            bool flag = false;
//            if (delegateArgumentsTypes.Length != 0)
//            {
//                UnityEventDrawer.GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods,
//                    PersistentListenerMode.EventDefined);
//                if (methods.Count > 0)
//                {
//                    GenericMenu genericMenu = menu;
//                    string str1 = targetName;
//                    string str2 = "/Dynamic ";
//                    string separator = ", ";
//                    System.Type[] typeArray = delegateArgumentsTypes;
//                    if (UnityEventDrawer.\u003C\u003Ef__am\u0024cacheA == null)
//                    {
//                        // ISSUE: method pointer
//                        UnityEventDrawer.\u003C\u003Ef__am\u0024cacheA = new Func<System.Type, string>((object) null,
//                            __methodptr(\u003CGeneratePopUpForType\u003Em__A));
//                    }
//
//                    Func<System.Type, string> fAmCacheA = UnityEventDrawer.\u003C\u003Ef__am\u0024cacheA;
//                    string[] array = ((IEnumerable<System.Type>) typeArray).Select<System.Type, string>(fAmCacheA)
//                        .ToArray<string>();
//                    string str3 = string.Join(separator, array);
//                    GUIContent content = new GUIContent(str1 + str2 + str3);
//                    genericMenu.AddDisabledItem(content);
//                    UnityEventDrawer.AddMethodsToMenu(menu, listener, methods, targetName);
//                    flag = true;
//                }
//            }
//
//            methods.Clear();
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[1]
//            {
//                typeof(float)
//            }, methods, PersistentListenerMode.Float);
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[1]
//            {
//                typeof(int)
//            }, methods, PersistentListenerMode.Int);
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[1]
//            {
//                typeof(string)
//            }, methods, PersistentListenerMode.String);
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[1]
//            {
//                typeof(bool)
//            }, methods, PersistentListenerMode.Bool);
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[1]
//            {
//                typeof(UnityEngine.Object)
//            }, methods, PersistentListenerMode.Object);
//            UnityEventDrawer.GetMethodsForTargetAndMode(target, new System.Type[0], methods,
//                PersistentListenerMode.Void);
//            if (methods.Count <= 0)
//                return;
//            if (flag)
//                menu.AddItem(new GUIContent(targetName + "/ "), false, (GenericMenu.MenuFunction) null);
//            if (delegateArgumentsTypes.Length != 0)
//                menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
//            UnityEventDrawer.AddMethodsToMenu(menu, listener, methods, targetName);
//        }
//
// // Method AddMethodsToMenu with token 06004A75
//        private static void AddMethodsToMenu( /*Parameter with token 08005160*/
//            GenericMenu menu, /*Parameter with token 08005161*/
//            SerializedProperty listener, /*Parameter with token 08005162*/
//            List<UnityEventDrawer.ValidMethodMap> methods, /*Parameter with token 08005163*/string targetName)
//        {
//            List<UnityEventDrawer.ValidMethodMap> source1 = methods;
//            if (UnityEventDrawer.\u003C\u003Ef__am\u0024cacheB == null)
//            {
//                // ISSUE: method pointer
//                UnityEventDrawer.\u003C\u003Ef__am\u0024cacheB =
//                    new Func<UnityEventDrawer.ValidMethodMap, int>((object) null,
//                        __methodptr(\u003CAddMethodsToMenu\u003Em__B));
//            }
//
//            Func<UnityEventDrawer.ValidMethodMap, int> fAmCacheB = UnityEventDrawer.\u003C\u003Ef__am\u0024cacheB;
//            IOrderedEnumerable<UnityEventDrawer.ValidMethodMap> source2 =
//                source1.OrderBy<UnityEventDrawer.ValidMethodMap, int>(fAmCacheB);
//            if (UnityEventDrawer.\u003C\u003Ef__am\u0024cacheC == null)
//            {
//                // ISSUE: method pointer
//                UnityEventDrawer.\u003C\u003Ef__am\u0024cacheC =
//                    new Func<UnityEventDrawer.ValidMethodMap, string>((object) null,
//                        __methodptr(\u003CAddMethodsToMenu\u003Em__C));
//            }
//
//            Func<UnityEventDrawer.ValidMethodMap, string> fAmCacheC = UnityEventDrawer.\u003C\u003Ef__am\u0024cacheC;
//            foreach (UnityEventDrawer.ValidMethodMap method in (IEnumerable<UnityEventDrawer.ValidMethodMap>) source2
//                .ThenBy<UnityEventDrawer.ValidMethodMap, string>(fAmCacheC))
//                UnityEventDrawer.AddFunctionsForScript(menu, listener, method, targetName);
//        }
//
//        // Method GetMethodsForTargetAndMode with token 06004A76
//        private static void GetMethodsForTargetAndMode( /*Parameter with token 08005164*/
//            UnityEngine.Object target, /*Parameter with token 08005165*/
//            System.Type[] delegateArgumentsTypes, /*Parameter with token 08005166*/
//            List<UnityEventDrawer.ValidMethodMap> methods, /*Parameter with token 08005167*/PersistentListenerMode mode)
//        {
//            foreach (UnityEventDrawer.ValidMethodMap method in UnityEventDrawer.CalculateMethodMap(target,
//                delegateArgumentsTypes, mode == PersistentListenerMode.Object))
//            {
//                method.mode = mode;
//                methods.Add(method);
//            }
//        }


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