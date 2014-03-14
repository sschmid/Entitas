using System;
using System.Collections.Generic;

namespace Entitas {
    public static class EntityMatcher {
        public static IEntityMatcher AllOf(IEnumerable<Type> types) {
            return new AllOfEntityMatcher(types);
        }

        public static IEntityMatcher AnyOf(IEnumerable<Type> types) {
            return new AnyOfEntityMatcher(types);
        }
    }

    public interface IEntityMatcher {
        HashSet<Type> types { get; }

        bool Matches(Entity entity);
    }

    public abstract class AbstractEntityMatcher : IEntityMatcher {
        public HashSet<Type> types { get { return _types; } }

        protected readonly HashSet<Type> _types;
        readonly int _hash;

        protected AbstractEntityMatcher(IEnumerable<Type> types) {
            _types = new HashSet<Type>(types);
            int hash = GetType().GetHashCode();
            foreach (var type in _types)
                hash ^= type.GetHashCode();
            _hash = hash;
        }

        public abstract bool Matches(Entity entity);

        public override bool Equals(object obj) {
            if (obj == this)
                return true;

            if (obj == null || obj.GetType() != GetType())
                return false;

            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode() {
            return _hash;
        }

        public override string ToString() {
            var typesStr = string.Empty;
            const string seperator = ", ";
            foreach (var type in _types)
                typesStr += type + seperator;

            if (typesStr != string.Empty)
                typesStr = typesStr.Substring(0, typesStr.Length - seperator.Length);

            return string.Format("{0}({1})", GetType().Name, typesStr);
        }
    }

    public class AllOfEntityMatcher : AbstractEntityMatcher {
        public AllOfEntityMatcher(IEnumerable<Type> types) : base(types) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasComponents(_types);
        }
    }

    public class AnyOfEntityMatcher : AbstractEntityMatcher {
        public AnyOfEntityMatcher(IEnumerable<Type> types) : base(types) {
        }

        public override bool Matches(Entity entity) {
            return entity.HasAnyComponent(_types);
        }
    }
}

