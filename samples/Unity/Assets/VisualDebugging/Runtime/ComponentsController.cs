using System;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using static GameMyGameObjectMatcher;
using static GameMyVector3Matcher;

public class ComponentsController : MonoBehaviour
{
    void Start()
    {
        ContextInitialization.InitializeAllContexts();
        var gameContext = new GameContext();
        gameContext.CreateContextObserver();

        CreateTestGroups(gameContext);
        CreateTestEntities(gameContext);
        CreateTestEntityWithNullValues(gameContext);
        CreateTestEntityError(gameContext);
    }

    void CreateTestGroups(GameContext context)
    {
        context.GetGroup(MyVector3);
        context.GetGroup(MyGameObject);
        context.GetGroup(Game.Matcher.AllOf(MyGameObject, MyVector3));
        context.GetGroup(Game.Matcher.AllOf(MyGameObject, MyVector3));
    }

    void CreateTestEntities(GameContext context)
    {
        for (var i = 0; i < 2; i++)
        {
            var entity = context.CreateEntity();

            // Unity's builtIn
            entity.AddMyAnimationCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
            entity.AddMyBool(true);
            entity.AddMyBounds(new Bounds());
            entity.AddMyColor(Color.red);
            entity.AddMyDouble(4.2f);
            entity.AddMyEnum(MyEnum.Item2);
            entity.AddMyFlags(MyFlags.Item2);
            entity.AddMyFloat(4.2f);
            entity.AddMyGameObject(new GameObject("Player").Link(entity).gameObject);
            entity.AddMyInt(42);
            entity.AddMyRect(new Rect(1f, 2f, 3f, 4f));
            entity.AddMyString("Hello, world!");
            entity.AddMyTexture2D(new Texture2D(2, 2));
            entity.AddMyTexture(new CustomRenderTexture(32, 32));
            entity.AddMyUnityObject(new UnityEngine.Object());
            entity.AddMyVector2(new Vector2(1f, 2f));
            entity.AddMyVector3(new Vector3(1f, 2f, 3f));
            entity.AddMyVector4(new Vector4(1f, 2f, 3f, 4f));

            // Custom
            entity.AddMyArray2D(new string[2, 3]);
            entity.AddMyArray3D(new string[2, 3, 4]);
            entity.AddMyArray(new[] { "1", "2", "3" });
            entity.AddMyChar('x');
            entity.AddMyCustomObject(new MyCustomObject("My Custom Object!"));
            entity.AddMyDateTime(DateTime.Now);
            entity.AddMyDictArray(
                new Dictionary<int, string[]>
                {
                    { 1, new[] { "One", "Two", "Three" } },
                    { 2, new[] { "Four", "Five", "Six" } }
                },
                new[]
                {
                    new Dictionary<int, string[]>
                    {
                        { 1, new[] { "One", "Two", "Three" } },
                        { 2, new[] { "Four", "Five", "Six" } }
                    },
                    new Dictionary<int, string[]>
                    {
                        { 3, new[] { "One", "Two", "Three" } },
                        { 4, new[] { "Four", "Five", "Six" } }
                    }
                }
            );
            entity.AddMyDictionary(new Dictionary<string, string>
            {
                { "1", "One" },
                { "2", "Two" },
                { "3", "Three" }
            });
            entity.AddMyDontDraw(new MySimpleObject());
            entity.AddMyFlag();
            entity.AddMyHashSet(new HashSet<string> { "1", "2", "3" });
            var jaggedArray = new string[2][];
            jaggedArray[0] = new[] { "Entity", "Component", "System" };
            jaggedArray[1] = new[] { "For", "C#" };
            entity.AddMyJaggedArray(jaggedArray);
            entity.AddMyListArray(new[]
            {
                new List<string> { "1", "2", "3" },
                new List<string> { "One", "Two", "Three" }
            });
            entity.AddMyList(new List<string> { "One", "Two", "Three" });
            entity.AddMyMemberList("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12");
            entity.AddMyMonoBehaviourSubClass(new GameObject().AddComponent<MyMonoBehaviourSubClass>());
            entity.AddMyPerson("Max", "Male");
            entity.AddMyProperty("My Property!");
            entity.AddMySimpleObject(new MySimpleObject());
            entity.AddMySystemObject(new object());
            entity.AddMyUnique("My Unique!");
            entity.AddMyUnsupportedObject(new UnsupportedObject("Unsupported Object"));
        }
    }

    void CreateTestEntityWithNullValues(GameContext context)
    {
        var entity = context.CreateEntity();

        entity.AddMyAnimationCurve(null);
        entity.AddMyGameObject(null);
        entity.AddMyString(null);
        entity.AddMyTexture2D(null);
        entity.AddMyTexture(null);
        entity.AddMyUnityObject(null);

        // Custom
        entity.AddMyArray2D(null);
        entity.AddMyArray3D(null);
        entity.AddMyArray(null);
        entity.AddMyCustomObject(null);
        entity.AddMyDictArray(null, null);
        entity.AddMyDictionary(null);
        entity.AddMyDontDraw(null);
        entity.AddMyHashSet(null);
        entity.AddMyJaggedArray(null);
        entity.AddMyListArray(null);
        entity.AddMyList(null);
        entity.AddMyMemberList(null, null, null, null, null, null, null, null, null, null, null, null);
        entity.AddMyMonoBehaviourSubClass(null);
        entity.AddMyPerson(null, null);
        entity.AddMyProperty(null);
        entity.AddMySimpleObject(null);
        entity.AddMySystemObject(null);
        entity.AddMyUnique(null);
        entity.AddMyUnsupportedObject(null);
    }

    void CreateTestEntityError(GameContext context)
    {
        var entity = context.CreateEntity();
        entity.Retain(this);
        entity.Destroy();
    }
}
