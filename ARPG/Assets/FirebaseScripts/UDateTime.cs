using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class UDateTime : ISerializationCallbackReceiver
{
    [HideInInspector] public DateTime dateTime;

    [HideInInspector][SerializeField] private string _dateTime;

    public static implicit operator DateTime(UDateTime udt)
    {
        return (udt.dateTime);
    }

    public static implicit operator UDateTime(DateTime dt)
    {
        return new UDateTime() { dateTime = dt };
    }

    public void OnAfterDeserialize()
    {
        DateTime.TryParse(_dateTime, out dateTime);
    }

    public void OnBeforeSerialize()
    {
        _dateTime = dateTime.ToString();
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UDateTime))]
public class UDateTimeDrawer : PropertyDrawer
{
    // ������ Rect�ȿ� property�� �׸�
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // label�� �׸�
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Rects�� �����
        Rect amountRect = new Rect(position.x, position.y, position.width, position.height);

        // labels ���� �׸����� ���� GUIcontent.none�� ������
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_dateTime"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif
