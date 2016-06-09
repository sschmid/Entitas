using System.Collections.Generic;
using Entitas.Serialization;
using System;

namespace Entitas.CodeGenerator {


    public class ComponentInfo {

        public string fullTypeName;
        public List<PublicMemberInfo> memberInfos;
        public string[] pools;
        public bool isSingleEntity;
        public string singleComponentPrefix;
        public bool generateMethods;
        public bool generateIndex;

        public bool isSingletonComponent;
        public string typeName;

        public ComponentInfo() {
            
        }

        public ComponentInfo(string fullTypeName, List<PublicMemberInfo> memberInfos, string[] pools,
            bool isSingleEntity, string singleComponentPrefix, bool generateMethods, bool generateIndex) {
            this.fullTypeName = fullTypeName;
            this.memberInfos = memberInfos;
            this.pools = pools;
            this.isSingleEntity = isSingleEntity;
            this.singleComponentPrefix = singleComponentPrefix;
            this.generateMethods = generateMethods;
            this.generateIndex = generateIndex;

            var nameSplit = fullTypeName.Split('.');
            this.typeName = nameSplit[nameSplit.Length - 1];

            this.isSingletonComponent = memberInfos.Count == 0;
        }
    }
}
