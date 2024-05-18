using NPSerialization;
using UnityEngine.UIElements;
using UnityEngine;

namespace NPVisualEditor
{
    public class KVDataItem : VisualElement
    {
        public KVDataItem()
        {
            style.flexDirection = FlexDirection.Row;
        }

        private BlackboardKVData m_kVData;

        public BlackboardKVData KVData
        {
            get
            {
                return m_kVData;
            }
            set
            {
                if (value == null)
                    return;

                Label label = new()
                {
                    text = value.m_compareType.ToString()
                };
                label.style.unityTextAlign = TextAnchor.MiddleCenter;
                Add(label);

                TextField keyText = new()
                {
                    value = value.m_key
                };
                keyText.RegisterCallback<FocusOutEvent>((evt) =>
                {
                    // TODO: Check
                    if (!string.IsNullOrEmpty(keyText.value))
                        value.m_key = keyText.value;
                });
                Add(keyText);

                switch (value.m_compareType)
                {
                    case CompareType.TString:
                        TextField valueText = new()
                        {
                            value = value.m_theStringValue
                        };
                        valueText.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            if (!string.IsNullOrEmpty(valueText.value))
                                value.m_theStringValue = valueText.value;
                        });
                        Add(valueText);
                        break;
                    case CompareType.TFloat:
                        FloatField floatField = new FloatField()
                        {
                            value = value.m_theFloatValue
                        };
                        floatField.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            value.m_theFloatValue = floatField.value;
                        });
                        Add(floatField);
                        break;
                    case CompareType.TInt:
                        IntegerField intField = new()
                        {
                            value = value.m_theIntValue
                        };
                        intField.RegisterCallback<FocusOutEvent>((evt) =>
                        {
                            value.m_theIntValue = intField.value;
                        });
                        Add(intField);
                        break;
                    case CompareType.TBoolean:
                        Toggle toggle = new()
                        {
                            value = value.m_theBoolValue
                        };
                        toggle.RegisterCallback<ChangeEvent<bool>>((evt) =>
                        {
                            value.m_theBoolValue = evt.newValue;
                        });
                        Add(toggle);
                        break;
                }
            }
        }
    }
}