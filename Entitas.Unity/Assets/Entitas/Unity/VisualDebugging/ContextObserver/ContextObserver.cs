using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class ContextObserver {

        public Context context { get { return _context; } }
        public Group[] groups { get { return _groups.ToArray(); }}
        public GameObject gameObject { get { return _gameObject; } }

        readonly Context _context;
        readonly List<Group> _groups;
        readonly GameObject _gameObject;
        StringBuilder _toStringBuilder = new StringBuilder();

        public ContextObserver(Context context) {
            _context = context;
            _groups = new List<Group>();
            _gameObject = new GameObject();
            _gameObject.AddComponent<ContextObserverBehaviour>().Init(this);

            _context.OnEntityCreated += onEntityCreated;
            _context.OnGroupCreated += onGroupCreated;
            _context.OnGroupCleared += onGroupCleared;
        }

        public void Deactivate() {
            _context.OnEntityCreated -= onEntityCreated;
            _context.OnGroupCreated -= onGroupCreated;
            _context.OnGroupCleared -= onGroupCleared;
        }

        void onEntityCreated(Context context, Entity entity) {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(context, entity);
            entityBehaviour.transform.SetParent(_gameObject.transform, false);
        }

        void onGroupCreated(Context context, Group group) {
            _groups.Add(group);
        }

        void onGroupCleared(Context context, Group group) {
            _groups.Remove(group);
        }

        public override string ToString() {
            _toStringBuilder.Length = 0;
            _toStringBuilder
                .Append(_context.contextInfo.name).Append(" (")
                .Append(_context.count).Append(" entities, ")
                .Append(_context.reusableEntitiesCount).Append(" reusable, ");

            if(_context.retainedEntitiesCount != 0) {
                _toStringBuilder
                    .Append(_context.retainedEntitiesCount).Append(" retained, ");
            }

            _toStringBuilder
                .Append(_groups.Count)
                .Append(" groups)");

            var str = _toStringBuilder.ToString();
            _gameObject.name = str;
            return str;
        }
    }
}
