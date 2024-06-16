using NPSerialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public static class UIFactory
    {
        public static List<VisualElement> CreateElements(GraphNode node, object obj, Action updateSelf)
        {
            List<VisualElement> elements = new();
            foreach (FieldInfo info in obj.GetType().GetFields())
            {
                bool read = IsRead(obj, info);
                VisualElement element = CreateElement(node, obj, info, read, updateSelf);
                if (element != null)
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        public static VisualElement CreateElement(GraphNode node, object obj, FieldInfo fieldInfo, bool read, Action updateSelf)
        {
            VisualElement element = null;

            // TODO: decoupling
            Type type = fieldInfo.FieldType;
            string label = GetLabel(fieldInfo.Name);
            if (type == typeof(long))
            {
                element = new LongField(label)
                {
                    value = (long)fieldInfo.GetValue(obj)
                };

                if (!read)
                {
                    element.RegisterCallback<FocusOutEvent>((evt) =>
                    {
                        fieldInfo.SetValue(obj, ((LongField)element).value);
                        GraphicUtils.UpdateGraphNode(node);
                    });
                }
            }
            else if (type == typeof(float))
            {
                element = new FloatField(label)
                {
                    value = (float)fieldInfo.GetValue(obj)
                };

                if (!read)
                {
                    element.RegisterCallback<FocusOutEvent>((evt) =>
                    {
                        fieldInfo.SetValue(obj, ((FloatField)element).value);
                        GraphicUtils.UpdateGraphNode(node);
                    });
                }
            }
            else if (type == typeof(string))
            {
                element = new TextField(label)
                {
                    value = (string)fieldInfo.GetValue(obj)
                };

                if (!read)
                {
                    element.RegisterCallback<FocusOutEvent>((evt) =>
                    {
                        fieldInfo.SetValue(obj, ((TextField)element).value);
                        GraphicUtils.UpdateGraphNode(node);
                    });
                }
            }
            else if (type.IsEnum) 
            {
                element = new EnumField(label, (Enum)fieldInfo.GetValue(obj));
                if (!read)
                {
                    element.RegisterCallback<ChangeEvent<Enum>>((evt) =>
                    {
                        fieldInfo.SetValue(obj, evt.newValue);
                        GraphicUtils.UpdateGraphNode(node);
                    });
                }
            }
            else if (type == typeof(DelegateData))
            {
                element = new VisualElement();
                element.style.borderBottomWidth = 1;
                element.style.borderBottomColor = Color.gray;
                element.style.borderTopWidth = 1;
                element.style.borderTopColor = Color.gray;

                element.style.flexDirection = FlexDirection.Column;
                var methodContanier = new VisualElement();
                methodContanier.style.flexDirection = FlexDirection.Row;

                DelegateData delegateData = (DelegateData)fieldInfo.GetValue(obj);
                string funcName = delegateData.GetMethodName();
                var funcElement = new TextField(label)
                {
                    value = funcName
                };

                var objectField = new ObjectField("new method");
                objectField.objectType = typeof(MonoScript);
                var dropdownField = new DropdownField();

                objectField.RegisterValueChangedCallback(selectedObject => {
                    if (selectedObject.newValue is MonoScript script)
                    {
                        List<MethodInfo> funcList = new();
                        List<string> funcStringList = new();
                        Type scriptClassType = script.GetClass();

                        if (NodeDataUtils.CheckSerializationID(scriptClassType))
                        {
                            funcList = NodeDataUtils.GetNPTaskMethods(scriptClassType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        }
                        else
                        {
                            funcList = NodeDataUtils.GetNPTaskMethods(scriptClassType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        }

                        foreach (var func in funcList)
                        {
                            string funcString = NodeDataUtils.GetSerializeString(func);
                            if (!string.IsNullOrEmpty(funcString))
                                funcStringList.Add(funcString);
                        }

                        dropdownField.choices = funcStringList;
                    }
                });

                dropdownField.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    if (delegateData.ResetMethod(evt.newValue))
                    {
                        GraphicUtils.UpdateGraphNode(node);
                        funcElement.value = evt.newValue;
                    }
                });

                methodContanier.Add(objectField);
                methodContanier.Add(dropdownField);

                element.Add(funcElement);
                element.Add(methodContanier);
            }
            else if (type == typeof(WaitData.WaitFunctionData))
            {
                element = new VisualElement();
                element.style.borderBottomWidth = 1;
                element.style.borderBottomColor = Color.gray;
                element.style.borderTopWidth = 1;
                element.style.borderTopColor = Color.gray;

                element.style.flexDirection = FlexDirection.Column;
                var methodContanier = new VisualElement();
                methodContanier.style.flexDirection = FlexDirection.Row;

                WaitData.WaitFunctionData waitFuncData = (WaitData.WaitFunctionData)fieldInfo.GetValue(obj);
                string funcName = waitFuncData.GetMethodName();
                var funcElement = new TextField(label)
                {
                    value = funcName
                };

                var objectField = new ObjectField("new method");
                objectField.objectType = typeof(MonoScript);
                var dropdownField = new DropdownField();

                objectField.RegisterValueChangedCallback(selectedObject => {
                    if (selectedObject.newValue is MonoScript script)
                    {
                        List<MethodInfo> funcList = new();
                        List<string> funcStringList = new();
                        Type scriptClassType = script.GetClass();

                        if (NodeDataUtils.CheckSerializationID(scriptClassType))
                        {
                            funcList = NodeDataUtils.GetNPWaitFuncMethods(scriptClassType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        }
                        else
                        {
                            funcList = NodeDataUtils.GetNPWaitFuncMethods(scriptClassType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        }

                        foreach (var func in funcList)
                        {
                            string funcString = NodeDataUtils.GetSerializeString(func);
                            if (!string.IsNullOrEmpty(funcString))
                                funcStringList.Add(funcString);
                        }

                        dropdownField.choices = funcStringList;
                    }
                });

                dropdownField.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    if (waitFuncData.ResetMethod(evt.newValue))
                    {
                        GraphicUtils.UpdateGraphNode(node);
                        funcElement.value = evt.newValue;
                    }
                });

                methodContanier.Add(objectField);
                methodContanier.Add(dropdownField);

                element.Add(funcElement);
                element.Add(methodContanier);
            }
            else if (type == typeof(BlackboardKVData)) 
            {
                element = new VisualElement();
                element.style.flexDirection = FlexDirection.Column;
                element.style.borderBottomWidth = 1;
                element.style.borderBottomColor = Color.gray;
                element.style.borderTopWidth = 1;
                element.style.borderTopColor = Color.gray;

                BlackboardKVData blackboardKVData = (BlackboardKVData)fieldInfo.GetValue(obj);

                var typeElem = new EnumField(label, blackboardKVData.m_compareType);
                typeElem.RegisterCallback<ChangeEvent<Enum>>((evt) =>
                {
                    blackboardKVData.m_compareType = (CompareType)evt.newValue;
                    GraphicUtils.UpdateGraphNode(node);
                    updateSelf?.Invoke();
                });

                element.Add(typeElem);

                var keyElem = new TextField("key")
                {
                    value = blackboardKVData.m_key
                };

                keyElem.RegisterCallback<FocusOutEvent>((evt) =>
                {
                    blackboardKVData.m_key = keyElem.value;
                    GraphicUtils.UpdateGraphNode(node);
                });

                VisualElement valueElem = null;
                switch (blackboardKVData.m_compareType)
                {
                    case CompareType.TString:
                        valueElem = new TextField("value")
                        {
                            value = blackboardKVData.m_theStringValue
                        };

                        valueElem.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            blackboardKVData.m_theStringValue = ((TextField)valueElem).value;
                            GraphicUtils.UpdateGraphNode(node);
                        });
                        break;
                    case CompareType.TFloat:
                        valueElem = new FloatField("value")
                        {
                            value = blackboardKVData.m_theFloatValue
                        };

                        valueElem.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            blackboardKVData.m_theFloatValue = ((FloatField)valueElem).value;
                            GraphicUtils.UpdateGraphNode(node);
                        });
                        break;
                    case CompareType.TInt:
                        valueElem = new IntegerField("value")
                        {
                            value = blackboardKVData.m_theIntValue
                        };

                        valueElem.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            blackboardKVData.m_theIntValue = ((IntegerField)valueElem).value;
                            GraphicUtils.UpdateGraphNode(node);
                        });
                        break;
                    case CompareType.TBoolean:
                        valueElem = new Toggle("value")
                        {
                            value = blackboardKVData.m_theBoolValue
                        };

                        valueElem.RegisterCallback<ChangeEvent<bool>>((evt) =>
                        {
                            blackboardKVData.m_theBoolValue = evt.newValue;
                            GraphicUtils.UpdateGraphNode(node);
                        });
                        break;
                }
                element.Add(keyElem);
                element.Add(valueElem);
            }
            else if (type == typeof(List<string>))
            {
                element = new VisualElement();
                element.style.flexDirection = FlexDirection.Column;

                var labelLabel = new Label(label);
                element.Add(labelLabel);

                var lst = (List<string>)fieldInfo.GetValue(obj);

                var listView = new ListView();
                listView.makeItem = () => new Label();
                listView.bindItem = (e, i) => (e as Label).text = lst[i];
                listView.itemsSource = lst;
                listView.selectionType = SelectionType.Single;
                listView.style.maxHeight = 200;
                element.Add(listView);

                var removeButton = new Button() { text = "Remove" };
                removeButton.RegisterCallback<MouseUpEvent>((evt) => {
                    string selectItem = (string)listView.selectedItem;
                    if (!string.IsNullOrEmpty(selectItem))
                    {
                        lst.Remove(selectItem);
                        updateSelf?.Invoke();
                    }
                });
                element.Add(removeButton);

                VisualElement subElem = new();
                subElem.style.flexDirection = FlexDirection.Row;

                var textElem = new TextField();
                subElem.Add(textElem);

                var addButton = new Button() { text = "Add" };
                addButton.RegisterCallback<MouseUpEvent>((evt) => {
                    if (!string.IsNullOrEmpty(textElem.value))
                    {
                        lst.Add(textElem.value);
                        updateSelf?.Invoke();
                    }
                });
                subElem.Add(addButton);

                element.Add(subElem);
            }

            return element;
        }

        private static string GetLabel(string name)
        {
            if (name.StartsWith("m_"))
                return name[2..];
            return name;
        }

        private static bool IsRead(object obj, FieldInfo fieldInfo)
        {
            return false;
        }
    }
}