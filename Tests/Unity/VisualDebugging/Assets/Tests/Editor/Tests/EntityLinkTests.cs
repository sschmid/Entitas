using System;
using Entitas.Core;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity.Editor;
using NUnit.Framework;
using UnityEngine;
using Entitas;

public class EntityLinkTests {

    IContext _context;
    IEntity _entity;
    GameObject _gameObject;
    EntityLink _link;

    [SetUp]
    public void BeforeEach() {
        _context = new GameContext();
        _entity = _context.CreateEntity();
        _gameObject = new GameObject("TestGameObject");
        _link = _gameObject.AddComponent<EntityLink>();
    }

    [Test]
    public void LinksEntityAndContextAndRetainsEntity() {

        // given
        var retainCount = _entity.retainCount;

        // when
        _link.Link(_entity, _context);

        // then
        Assert.AreSame(_entity, _link.entity);
        Assert.AreSame(_context, _link.context);
        Assert.AreEqual(retainCount + 1, _entity.retainCount);
        #if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsTrue(((SafeAERC)_entity.aerc).owners.Contains(_link));
        #endif
    }

    [Test, ExpectedException(typeof(Exception))]
    public void ThrowsWhenAlreadyLinked() {

        // when
        _link.Link(_entity, _context);
        _link.Link(_entity, _context);
    }

    [Test]
    public void UnlinksEntityReleasesEntity() {

        // given
        _link.Link(_entity, _context);
        var retainCount = _entity.retainCount;

        // when
        _link.Unlink();

        Assert.AreEqual(retainCount - 1, _entity.retainCount);
        Assert.IsNull(_link.entity);
        Assert.IsNull(_link.context);
        #if !ENTITAS_FAST_AND_UNSAFE
        Assert.IsFalse(((SafeAERC)_entity.aerc).owners.Contains(_link));
        #endif
    }

    [Test, ExpectedException(typeof(Exception))]
    public void ThrowsWhenAlreadyUnlinked() {

        // when
        _link.Unlink();
    }

    [Test]
    public void GetSameEntityLink() {
        Assert.AreSame(_link, _gameObject.GetEntityLink());
    }

    [Test]
    public void AddsEntityLinkAndLinks() {

        // given
        var gameObject = new GameObject();
        var retainCount = _entity.retainCount;

        // when
        var link = gameObject.Link(_entity, _context);

        // then
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreSame(_entity, link.entity);
        Assert.AreSame(_context, link.context);
        Assert.AreEqual(retainCount + 1, _entity.retainCount);
    }

    [Test]
    public void Unlinks() {

        // given
        var gameObject = new GameObject();
        var link = gameObject.Link(_entity, _context);
        var retainCount = _entity.retainCount;

        // when
        gameObject.Unlink();

        // then
        Assert.AreSame(link, gameObject.GetEntityLink());
        Assert.AreEqual(retainCount - 1, _entity.retainCount);
        Assert.IsNull(link.entity);
        Assert.IsNull(link.context);
    }

    [Test]
    public void ReusesLink() {

        // given
        var gameObject = new GameObject();
        var link1 = gameObject.Link(_context.CreateEntity(), new GameContext());
        gameObject.Unlink();

        // when
        var link2 = gameObject.Link(_entity, _context);

        // then
        Assert.AreEqual(1, gameObject.GetComponents<EntityLink>().Length);
        Assert.AreSame(link1, link2);
        Assert.AreSame(_entity, link2.entity);
        Assert.AreSame(_context, link2.context);
    }

    [Test]
    public void CanToString() {
        Assert.AreEqual("EntityLink(TestGameObject)", _link.ToString());
    }
}
