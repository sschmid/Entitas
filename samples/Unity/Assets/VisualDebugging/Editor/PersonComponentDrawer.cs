using System;
using Entitas;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;

public class PersonComponentDrawer : IComponentDrawer {
    
    public bool HandlesType(Type type) {
        return type == typeof(PersonComponent);
    }

    public IComponent DrawComponent(IComponent component) {
        var person = (PersonComponent)component;

        person.name = EditorGUILayout.TextField("Name", person.name);

        if (person.gender == null) {
            person.gender = PersonGender.Male.ToString();
        }

        var gender = (PersonGender)Enum.Parse(typeof(PersonGender), person.gender);
        gender = (PersonGender)EditorGUILayout.EnumPopup("Gender", gender);
        person.gender = gender.ToString();

        return person;
    }

    enum PersonGender {
        Male,
        Female
    }
}
