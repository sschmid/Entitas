using System;
using UnityEngine;

namespace Entitas.Unity
{
    public class EntityLink : MonoBehaviour
    {
        public IEntity Entity => _entity;

        IEntity _entity;
        bool _applicationIsQuitting;

        public void Link(IEntity entity)
        {
            if (_entity != null)
                throw new Exception($"EntityLink is already linked to {_entity}!");

            _entity = entity;
            _entity.Retain(this);
        }

        public void Unlink()
        {
            if (_entity == null)
                throw new Exception("EntityLink is already unlinked!");

            _entity.Release(this);
            _entity = null;
        }

        void OnDestroy()
        {
            if (!_applicationIsQuitting && _entity != null)
                Debug.LogWarning($"EntityLink got destroyed but is still linked to {_entity}!\nPlease call gameObject.Unlink() before it is destroyed.");
        }

        void OnApplicationQuit() => _applicationIsQuitting = true;

        public override string ToString() => $"EntityLink({gameObject.name})";
    }

    public static class EntityLinkExtension
    {
        public static EntityLink GetEntityLink(this GameObject gameObject) => gameObject.GetComponent<EntityLink>();

        public static EntityLink Link(this GameObject gameObject, IEntity entity)
        {
            if (!gameObject.TryGetComponent<EntityLink>(out var entityLink))
            {
                entityLink = gameObject.AddComponent<EntityLink>();
                entityLink.Link(entity);
            }
            else
            {
                entityLink.Link(entity);
            }

            return entityLink;
        }

        public static void Unlink(this GameObject gameObject)
        {
            gameObject.GetEntityLink().Unlink();
        }
    }
}
