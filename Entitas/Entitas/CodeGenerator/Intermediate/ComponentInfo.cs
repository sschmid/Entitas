using System.Collections.Generic;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {
    public class ComponentInfo {

        public string fullTypeName { get { return _fullTypeName; } }
        public List<PublicMemberInfo> memberInfos { get { return _memberInfos; } }
        public string[] pools { get { return _pools; } }
        public bool isSingleEntity { get { return _isSingleEntity; } }
        public string singleComponentPrefix { get { return _singleComponentPrefix; } }
        public bool generateMethods { get { return _generateMethods; } }
        public bool generateIndex { get { return _generateIndex; } }

        public bool isSingletonComponent { get { return _isSingletonComponent; } }
        public string typeName { get { return _typeName; } }

        readonly string _fullTypeName;
        readonly List<PublicMemberInfo> _memberInfos;
        readonly string[] _pools;
        readonly bool _isSingleEntity;
        readonly string _singleComponentPrefix;
        readonly bool _generateMethods;
        readonly bool _generateIndex;

        readonly string _typeName;
        readonly bool _isSingletonComponent;

        public ComponentInfo(string fullTypeName, List<PublicMemberInfo> memberInfos, string[] pools,
            bool isSingleEntity, string singleComponentPrefix, bool generateMethods, bool generateIndex) {
            _fullTypeName = fullTypeName;
            _memberInfos = memberInfos;
            _pools = pools;
            _isSingleEntity = isSingleEntity;
            _singleComponentPrefix = singleComponentPrefix;
            _generateMethods = generateMethods;
            _generateIndex = generateIndex;

            var nameSplit = fullTypeName.Split('.');
            _typeName = nameSplit[nameSplit.Length - 1];

            _isSingletonComponent = memberInfos.Count == 0;
        }
    }
}