using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Entitas.Unity
{
    public class ContextObserverBehaviour : MonoBehaviour
    {
        public IContext Context => _context;

        public readonly List<IGroup> Groups = new List<IGroup>();

        readonly Stack<EntityBehaviour> _entityBehaviourPool = new Stack<EntityBehaviour>();
        readonly StringBuilder _toStringBuilder = new StringBuilder();

        IContext _context;

        public void Initialize(IContext context)
        {
            _context = context;
            context.OnEntityCreated += OnEntityCreated;
            context.OnGroupCreated += OnGroupCreated;
            Update();
        }

        void OnEntityCreated(IContext context, Entity entity)
        {
            var entityBehaviour = _entityBehaviourPool.Count > 0
                ? _entityBehaviourPool.Pop()
                : new GameObject().AddComponent<EntityBehaviour>();

            entityBehaviour.Initialize(context, entity, _entityBehaviourPool);
            entityBehaviour.transform.SetParent(transform, false);
            entityBehaviour.transform.SetAsLastSibling();
        }

        void OnGroupCreated(IContext context, IGroup group)
        {
            Groups.Add(group);
        }

        void Update()
        {
            name = ToString();
        }

        void OnDestroy()
        {
            _context.OnEntityCreated -= OnEntityCreated;
            _context.OnGroupCreated -= OnGroupCreated;
        }

        public override string ToString()
        {
            _toStringBuilder.Length = 0;
            _toStringBuilder
                .Append(_context.ContextInfo.Name).Append(" (")
                .Append(_context.Count).Append(" entities, ")
                .Append(_context.ReusableEntitiesCount).Append(" reusable, ");

            if (_context.RetainedEntitiesCount != 0)
            {
                _toStringBuilder.Append(_context.RetainedEntitiesCount).Append(" retained, ");
            }

            _toStringBuilder.Append(Groups.Count).Append(" groups)");

            return _toStringBuilder.ToString();
        }
    }
}
