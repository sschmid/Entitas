using System;
using UnityEngine;

namespace Entitas.Unity {

    public class EntityLink : MonoBehaviour {

        public IEntity entity { get { return _entity; } }
        public IContext context { get { return _context; } }

        IEntity _entity;
        IContext _context;
        bool _applicationIsQuitting;

        public void Link(IEntity entity, IContext context) {
            if (_entity != null) {
                throw new Exception("EntityLink is already linked to " + _entity + "!");
            }

            _entity = entity;
            _context = context;
            _entity.Retain(this);
        }

        public void Unlink() {
            if (_entity == null) {
                throw new Exception("EntityLink is already unlinked!");
            }

            _entity.Release(this);
            _entity = null;
            _context = null;
        }

        void OnDestroy() {
            if (!_applicationIsQuitting && _entity != null) {
                throw new EntitasException(
                    "EntityLink got destroyed but is still linked to " + _entity + "!",
                    "Please call gameObject.Unlink() before it is destroyed."
                );
            }
        }

        void OnApplicationQuit() {
            _applicationIsQuitting = true;
        }

        public override string ToString() {
            return "EntityLink(" + gameObject.name + ")";
        }
    }

    public static class EntityLinkExtension {

        public static EntityLink GetEntityLink(this GameObject gameObject) {
            return gameObject.GetComponent<EntityLink>();
        }

        public static EntityLink Link(this GameObject gameObject, IEntity entity, IContext context) {
            var link = gameObject.GetEntityLink();
            if (link == null) {
                link = gameObject.AddComponent<EntityLink>();
            }

            link.Link(entity, context);
            return link;
        }

        public static void Unlink(this GameObject gameObject) {
            gameObject.GetEntityLink().Unlink();
        }
    }
}
