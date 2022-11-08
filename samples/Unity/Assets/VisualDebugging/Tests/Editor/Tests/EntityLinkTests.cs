using System;
using Entitas;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity.Editor;
using NUnit.Framework;
using UnityEngine;

public class EntityLinkTests
{
    IContext _context;
    IEntity _entity;
    GameObject _gameObject;
    EntityLink _link;

    [SetUp]
    public void BeforeEach()
    {
        _context = new GameContext();
        _entity = _context.CreateEntity();
        _gameObject = new GameObject("TestGameObject");
        _link = _gameObject.AddComponent<EntityLink>();
    }

    [Test]
    public void LinksEntityAndContextAndRetainsEntity()
    {
        var retainCount = _entity.retainCount;
        _link.Link(_entity);
        Assert.AreSame(_entity, _link.entity);
        Assert.AreEqual(retainCount + 1, _entity.retainCount);
#if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsTrue(((SafeAERC)_entity.aerc).owners.Contains(_link));
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
        var retainCount = _entity.retainCount;
        _link.Unlink();
        Assert.AreEqual(retainCount - 1, _entity.retainCount);
        Assert.IsNull(_link.entity);
#if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsFalse(((SafeAERC)_entity.aerc).owners.Contains(_link));
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
        var retainCount = _entity.retainCount;
        var link = gameObject.Link(_entity);
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreSame(_entity, link.entity);
        Assert.AreEqual(retainCount + 1, _entity.retainCount);
    }

    [Test]
    public void Unlinks()
    {
        var gameObject = new GameObject();
        var link = gameObject.Link(_entity);
        var retainCount = _entity.retainCount;
        gameObject.Unlink();
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreEqual(retainCount - 1, _entity.retainCount);
        Assert.IsNull(link.entity);
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
        Assert.AreSame(_entity, link2.entity);
    }

    [Test]
    public void CanToString()
    {
        Assert.AreEqual("EntityLink(TestGameObject)", _link.ToString());
    }
}
