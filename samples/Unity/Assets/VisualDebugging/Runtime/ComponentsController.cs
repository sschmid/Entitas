using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using UnityEngine;

public class ComponentsController : MonoBehaviour
{
    void Start()
    {
        var context = Contexts.sharedInstance.game;
        CreateTestGroups(context);
        CreateTestEntities(context);
        CreateTestEntityWithNullValues(context);
        CreateTestEntityError(context);
    }

    void CreateTestGroups(GameContext context)
    {
        context.GetGroup(GameMatcher.MyVector3);
        context.GetGroup(GameMatcher.MyGameObject);
        context.GetGroup(Matcher<GameEntity>.AllOf(GameMatcher.MyGameObject, GameMatcher.MyVector3));
        context.GetGroup(Matcher<GameEntity>.AllOf(GameMatcher.MyGameObject, GameMatcher.MyVector3));
    }

    void CreateTestEntities(GameContext context)
    {
        for (var i = 0; i < 2; i++)
        {
            var e = context.CreateEntity();

            // Unity's builtIn
            e.AddMyAnimationCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            e.AddMyBool(true);
            e.AddMyBounds(new Bounds());
            e.AddMyColor(Color.red);
            e.AddMyDouble(4.2f);
            e.AddMyEnum(MyEnumComponent.MyEnum.Item2);
            e.AddMyFlags(MyFlagsComponent.MyFlags.Item2);
            e.AddMyFloat(4.2f);
            var go = new GameObject("Player");
            go.Link(e);
            e.AddMyGameObject(go);
            e.AddMyHiddenInt(42);
            e.AddMyInt(42);
            e.AddMyRect(new Rect(1f, 2f, 3f, 4f));
            e.AddMyString("Hello, world!");
            e.AddMyTexture2D(new Texture2D(2, 2));
            e.AddMyTexture(new CustomRenderTexture(32, 32));
            e.AddMyUnityObject(new UnityEngine.Object());
            e.AddMyVector2(new Vector2(1f, 2f));
            e.AddMyVector3(new Vector3(1f, 2f, 3f));
            e.AddMyVector4(new Vector4(1f, 2f, 3f, 4f));

            // Custom
            e.AddIMyInterface(new MyInterfaceClass());
            e.AddMyArray2D(new string[2, 3]);
            e.AddMyArray3D(new string[2, 3, 4]);
            e.AddMyArray(new[] {"1", "2", "3"});
            e.AddMyChar('x');
            e.AddMyClass(new MyClass("My Class!"));
            e.myMyCustomFlag = true;
            e.AddMyCustomObject(new MyCustomObject("My Custom Object!"));
            e.AddMyDateTime(DateTime.Now);
            e.AddMyDictArray(
                new Dictionary<int, string[]> {{1, new[] {"One", "Two", "Three"}}, {2, new[] {"Four", "Five", "Six"}}},
                new[]
                {
                    new Dictionary<int, string[]> {{1, new[] {"One", "Two", "Three"}}, {2, new[] {"Four", "Five", "Six"}}},
                    new Dictionary<int, string[]> {{3, new[] {"One", "Two", "Three"}}, {4, new[] {"Four", "Five", "Six"}}}
                }
            );
            e.AddMyDictionary(new Dictionary<string, string> {{"1", "One"}, {"2", "Two"}, {"3", "Three"}});
            e.AddMyDontDraw(new MySimpleObject());
            e.AddMyEventClass(new MyEventClass());
            e.AddMyEvent("My Event!");
            e.isMyFlag = true;
            e.AddMyHashSet(new HashSet<string> {"1", "2", "3"});
            e.AddPosition(new MyIntVector2());
            e.AddVelocity(new MyIntVector2());
            var jaggedArray = new string[2][];
            jaggedArray[0] = new[] {"Entity", "Component", "System"};
            jaggedArray[1] = new[] {"For", "C#"};
            e.AddMyJaggedArray(jaggedArray);
            e.AddCoolName(new MyLameNameObject());
            e.AddMyListArray(new[]
            {
                new List<string> {"1", "2", "3"},
                new List<string> {"One", "Two", "Three"}
            });
            e.AddMyList(new List<string> {"Apple", "Banana", "Peach"});
            e.AddMyMemberList("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12");
            e.AddMyMonoBehaviourSubClass(new GameObject().AddComponent<MyMonoBehaviourSubClass>());
            e.AddMyPerson("Max", "Male");
            e.AddMyProperty("My Property!");
            e.AddMySimpleObject(new MySimpleObject());
            e.AddMyStruct(new MyStruct("My Struct!"));
            e.AddMySystemObject(new object());
            e.AddMyUnique("My Unique!");
            e.AddMyUnsupportedObject(new UnsupportedObject("Unsupported Object"));
        }
    }

    void CreateTestEntityWithNullValues(GameContext context)
    {
        var e = context.CreateEntity();

        e.AddMyAnimationCurve(null);
        e.AddMyGameObject(null);
        e.AddMyString(null);
        e.AddMyTexture2D(null);
        e.AddMyTexture(null);
        e.AddMyUnityObject(null);

        // Custom
        e.AddIMyInterface(null);
        e.AddMyArray2D(null);
        e.AddMyArray3D(null);
        e.AddMyArray(null);
        e.AddMyClass(null);
        e.AddMyCustomObject(null);
        e.AddMyDictArray(null, null);
        e.AddMyDictionary(null);
        e.AddMyDontDraw(null);
        e.AddMyEventClass(null);
        e.AddMyEvent(null);
        e.AddMyHashSet(null);
        e.AddMyJaggedArray(null);
        e.AddCoolName(null);
        e.AddMyListArray(null);
        e.AddMyList(null);
        e.AddMyMemberList(null, null, null, null, null, null, null, null, null, null, null, null);
        e.AddMyMonoBehaviourSubClass(null);
        e.AddMyPerson(null, null);
        e.AddMyProperty(null);
        e.AddMySimpleObject(null);
        e.AddMySystemObject(null);
        e.AddMyUnique(null);
        e.AddMyUnsupportedObject(null);
    }

    void CreateTestEntityError(GameContext context)
    {
        var entity = context.CreateEntity();
        entity.Retain(this);
        entity.Destroy();
    }
}
