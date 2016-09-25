using System.Collections.Generic;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {

    public class ComponentInfo {

        public readonly string fullTypeName;
        public readonly List<PublicMemberInfo> memberInfos;
        public readonly string[] pools;
        public readonly bool isSingleEntity;
        public readonly string singleComponentPrefix;
        public readonly bool generateComponent;
        public readonly bool generateMethods;
        public readonly bool generateIndex;
        
        public readonly string typeName;
        public readonly bool isSingletonComponent;

        public ComponentInfo(string fullTypeName, List<PublicMemberInfo> memberInfos, string[] pools,
                            bool isSingleEntity, string singleComponentPrefix,
                            bool generateComponent, bool generateMethods, bool generateIndex) {

            this.fullTypeName = fullTypeName;
            this.memberInfos = memberInfos;
            this.pools = pools;
            this.isSingleEntity = isSingleEntity;
            this.singleComponentPrefix = singleComponentPrefix;
            this.generateComponent = generateComponent;
            this.generateMethods = generateMethods;
            this.generateIndex = generateIndex;

            var nameSplit = fullTypeName.Split('.');
            typeName = nameSplit[nameSplit.Length - 1];

            isSingletonComponent = memberInfos.Count == 0;
        }
    }
}