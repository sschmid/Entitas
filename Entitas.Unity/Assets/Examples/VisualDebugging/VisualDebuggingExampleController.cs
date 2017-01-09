using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class VisualDebuggingExampleController : MonoBehaviour {

    void Start() {
        var context = Contexts.sharedInstance.visualDebugging = Contexts.CreateVisualDebuggingContext();

        createTestGroups(context);

        createTestEntities(context);
        createTestEntityWithNullValues(context);

        createTestEntityError(context);
    }

    void createTestGroups(Context context) {
        context.GetGroup(VisualDebuggingMatcher.Vector3);
        context.GetGroup(VisualDebuggingMatcher.GameObject);
        context.GetGroup(Matcher.AllOf(VisualDebuggingMatcher.GameObject, VisualDebuggingMatcher.Vector3));
        context.GetGroup(Matcher.AllOf(VisualDebuggingMatcher.GameObject, VisualDebuggingMatcher.Vector3));
    }

    void createTestEntities(Context context) {
        for (int i = 0; i < 2; i++) {
            var e = context.CreateEntity();

            // Unity's builtIn
            e.AddBounds(new Bounds());
            e.AddColor(Color.red);
            e.AddAnimationCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            e.AddMyEnum(MyEnumComponent.MyEnum.Item2);
            e.AddMyFlags(MyFlagsComponent.MyFlags.Item2);
            e.AddMyDouble(4.2f);
            e.AddMyFloat(4.2f);
            e.AddMyInt(42);
            e.AddRect(new Rect(1f, 2f, 3f, 4f));
            e.AddMyString("Hello, world!");
            e.AddVector2(new Vector2(1f, 2f));
            e.AddVector3(new Vector3(1f, 2f, 3f));
            e.AddVector4(new Vector4(1f, 2f, 3f, 4f));
            e.AddMyBool(true);
            e.AddUnityObject(new UnityEngine.Object());
            e.AddGameObject(new GameObject("Player"));
            e.AddTexture(new Texture());
            e.AddTexture2D(new Texture2D(2, 2));

            // Custom
            e.AddMonoBehaviourSubClass(new GameObject().AddComponent<MonoBehaviourSubClass>());
            e.AddCustomObject(new CustomObject("Custom Object"));
            e.AddSystemObject(new object());
            e.AddDateTime(DateTime.Now);
            e.AddAnArray(new [] { "Hello", ", ", "world", "!" });
            e.AddArray2D(new string[2, 3]);
            e.AddArray3D(new string[2, 3, 4]);
            string[][] jaggedArray = new string[2][];
            jaggedArray[0] = new [] { "Entity", "Component", "System" };
            jaggedArray[1] = new [] { "For", "C#" };
            e.AddJaggedArray(jaggedArray);
            var listArray = new List<string>[] {
                new List<string> { "1", "2", "3" },
                new List<string> { "One", "Two", "Three" }
            };
            e.AddListArray(listArray);
            e.AddList(new List<string>{ "Apple", "Banana", "Peach" });
            var dict = new Dictionary<string, string> {
                { "1", "One" },
                { "2", "Two" },
                { "3", "Three" },
            };
            e.AddDictionary(dict);
            var dict2 = new Dictionary<int, string[]> {
                { 1, new [] { "One", "Two", "Three" } },
                { 2, new [] { "Four", "Five", "Six" } }
            };
            var dictArray = new Dictionary<int, string[]>[] {
                new Dictionary<int, string[]> {
                    { 1, new [] { "One", "Two", "Three" } },
                    { 2, new [] { "Four", "Five", "Six" } }
                }, new Dictionary<int, string[]> {
                    { 3, new [] { "One", "Two", "Three" } },
                    { 4, new [] { "Four", "Five", "Six" } }
                }
            };
            e.AddDictArray(dict2, dictArray);
            e.AddHashSet(new HashSet<string> { "One", "Two", "Three" });
            e.AddMyChar('c');
            e.AddUnsupportedObject(new UnsupportedObject("Unsupported Object"));
            e.AddProperty("My Property");
            e.AddPerson("Max", "Male");
        }
    }

    void createTestEntityWithNullValues(Context context) {
        var e = context.CreateEntity();

        // Unity's builtIn
        AnimationCurve animationCurve = null;
        e.AddAnimationCurve(animationCurve);
        string myString = null;
        e.AddMyString(myString);
        UnityEngine.Object unityObject = null;
        e.AddUnityObject(unityObject);
        GameObject go = null;
        e.AddGameObject(go);
        Texture texture = null;
        e.AddTexture(texture);
        Texture2D texture2D = null;
        e.AddTexture2D(texture2D);

        // Custom
        MonoBehaviourSubClass monoBehaviourSubClass = null;
        e.AddMonoBehaviourSubClass(monoBehaviourSubClass);
        CustomObject customObject = null;
        e.AddCustomObject(customObject);
        object systemObject = null;
        e.AddSystemObject(systemObject);
        string[] array = null;
        e.AddAnArray(array);
        string[,] array2d = null;
        e.AddArray2D(array2d);
        string[,,] array3d = null;
        e.AddArray3D(array3d);
        string[][] jaggedArray = null;
        e.AddJaggedArray(jaggedArray);
        List<string>[] listArray = null;
        e.AddListArray(listArray);
        List<string> list = null;
        e.AddList(list);
        Dictionary<string, string> dict = null;
        e.AddDictionary(dict);
        Dictionary<int, string[]> dict2 = null;
        Dictionary<int, string[]>[] dictArray = null;
        e.AddDictArray(dict2, dictArray);
        HashSet<string> hashset = null;
        e.AddHashSet(hashset);
        char c = default(char);
        e.AddMyChar(c);
        UnsupportedObject unsupportedObject = null;
        e.AddUnsupportedObject(unsupportedObject);
        e.AddProperty(myString);
        string personName = null;
        string personGender = null;
        e.AddPerson(personName, personGender);
    }

    void createTestEntityError(Context context) {
        context.DestroyEntity(context.CreateEntity().Retain(this));
    }
}
