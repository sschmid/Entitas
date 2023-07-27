using System;
using Entitas;
using Entitas.Unity.Editor;
using UnityEditor;

public class PersonComponentDrawer : IComponentDrawer
{
    public bool HandlesType(Type type) => type == typeof(MyPersonComponent);

    public IComponent DrawComponent(IComponent component)
    {
        var person = (MyPersonComponent)component;
        person.Name = EditorGUILayout.TextField("Name", person.Name);
        person.Gender ??= PersonGender.Male.ToString();

        var gender = (PersonGender)Enum.Parse(typeof(PersonGender), person.Gender);
        gender = (PersonGender)EditorGUILayout.EnumPopup("Gender", gender);
        person.Gender = gender.ToString();

        return person;
    }

    enum PersonGender
    {
        Male,
        Female
    }
}
