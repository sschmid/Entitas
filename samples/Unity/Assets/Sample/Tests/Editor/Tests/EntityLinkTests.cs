using System;
using Entitas;
using Entitas.Unity;
using NUnit.Framework;
using UnityEngine;

public class EntityLinkTests
{
    GameContext _context;
    Entity _entity;
    GameObject _gameObject;
    EntityLink _link;

    [SetUp]
    public void BeforeEach()
    {
        ContextInitialization.InitializeAllContexts();
        _context = new GameContext();
        _context.CreateContextObserver();
        _entity = _context.CreateEntity();
        _gameObject = new GameObject("TestGameObject");
        _link = _gameObject.AddComponent<EntityLink>();
    }

    [Test]
    public void LinksEntityAndContextAndRetainsEntity()
    {
        var retainCount = _entity.RetainCount;
        _link.Link(_entity);
        Assert.AreSame(_entity, _link.Entity);
        Assert.AreEqual(retainCount + 1, _entity.RetainCount);
#if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsTrue(((SafeAERC)_entity.Aerc).Owners.Contains(_link));
#endif
    }

    [Test]
    public void ThrowsWhenAlreadyLinked()
    {
        Assert.Throws(typeof(Exception), () =>
        {
            _link.Link(_entity);
            _link.Link(_entity);
        });
    }

    [Test]
    public void UnlinksEntityReleasesEntity()
    {
        _link.Link(_entity);
        var retainCount = _entity.RetainCount;
        _link.Unlink();
        Assert.AreEqual(retainCount - 1, _entity.RetainCount);
        Assert.IsNull(_link.Entity);
#if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsFalse(((SafeAERC)_entity.Aerc).Owners.Contains(_link));
#endif
    }

    [Test]
    public void ThrowsWhenAlreadyUnlinked()
    {
        Assert.Throws(typeof(Exception), () => _link.Unlink());
    }

    [Test]
    public void GetSameEntityLink()
    {
        Assert.AreSame(_link, _gameObject.GetEntityLink());
    }

    [Test]
    public void AddsEntityLinkAndLinks()
    {
        var gameObject = new GameObject();
        var retainCount = _entity.RetainCount;
        var link = gameObject.Link(_entity);
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreSame(_entity, link.Entity);
        Assert.AreEqual(retainCount + 1, _entity.RetainCount);
    }

    [Test]
    public void Unlinks()
    {
        var gameObject = new GameObject();
        var link = gameObject.Link(_entity);
        var retainCount = _entity.RetainCount;
        gameObject.Unlink();
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreEqual(retainCount - 1, _entity.RetainCount);
        Assert.IsNull(link.Entity);
    }

    [Test]
    public void ReusesLink()
    {
        var gameObject = new GameObject();
        var link1 = gameObject.Link(_context.CreateEntity());
        gameObject.Unlink();
        var link2 = gameObject.Link(_entity);
        Assert.AreEqual(1, gameObject.GetComponents<EntityLink>().Length);
        Assert.AreSame(link1, link2);
        Assert.AreSame(_entity, link2.Entity);
    }

    [Test]
    public void CanToString()
    {
        Assert.AreEqual("EntityLink(TestGameObject)", _link.ToString());
    }
}
