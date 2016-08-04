using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGenerator;
using Entitas.Serialization;

namespace Entitas.CodeGenerator {

    /// <summary>
    /// Generates interfaces that declare what kind of components can be
    /// created from a given pool.
    /// </summary>
    public class PoolEntityGenerator : IPoolEntityCodeGenerator {

        const string FILE_NAME = "I{0}PoolEntity";
        const string FILE_TEMPLATE = @"
using Entitas;

namespace Entitas {{

    public interface {0} : IEntity {{{1}
    }}
}}
";

        public CodeGenFile[] Generate(ComponentInfo[] componentInfos) {

            // Each component can have multiple pools. We want to group the
            // components by their pools which means some components may show
            // up in multiple groups. Here's what happens below:
            // 
            // 1. Filter the components that don't have pools.
            // 2. We go through each component and expand based on the pools field.
            //    e.g. a component with two pools becomes two components with one
            //    pool each.

            // 3. Now that we have a list of components with a single pool, we
            //    perform a simple GroupBy().
            // 4. For each group we create an interface. 

            return componentInfos
                .Where(component => component.pools.Length > 0)
                .Aggregate(new List<ComponentInfo>(), (acc, component) => {
                    var components = component.pools.Select(poolName => {
                        return new ComponentInfo(
                            component.fullTypeName,
                            component.memberInfos,
                            new[] { poolName },
                            component.isSingleEntity,
                            component.singleComponentPrefix,
                            component.generateComponent,
                            component.generateMethods,
                            component.generateIndex
                        );
                    });
                    acc.AddRange(components);
                    return acc;
                })
                .GroupBy(component => component.pools[0])
                .Aggregate(new List<CodeGenFile>(), (acc, componentGroup) => {
                    var interfaceName = string.Format(FILE_NAME, componentGroup.Key.UppercaseFirst());
                    acc.Add(new CodeGenFile {
                        fileName = interfaceName,
                        fileContent = generateFileMethods(interfaceName, componentGroup.ToArray())
                                .ToUnixLineEndings(),
                        generatorName = this.GetType().FullName
                    });
                    return acc;
                })
                .ToArray();
        }

        static string generateFileMethods(string interfaceName, ComponentInfo[] componentInfos) {
            var methods = string.Join(
                "",
                componentInfos.Select(componentInfo => {
                    return 
                        string.Format(
                            "\n        {0} {1} {{ get; }}",
                            componentInfo.fullTypeName,
                            componentInfo.typeName.RemoveComponentSuffix().LowercaseFirst()
                        ) +
                        string.Format(
                            "\n        bool has{0} {{ get; }}",
                            componentInfo.typeName.RemoveComponentSuffix()
                        ) +
                        string.Format(
                            "\n        {0} Add{1}({2});",
                            interfaceName,
                            componentInfo.typeName.RemoveComponentSuffix(),
                            memberNamesWithType(componentInfo.memberInfos)
                        ) +
                        string.Format(
                            "\n        {0} Replace{1}({2});",
                            interfaceName,
                            componentInfo.typeName.RemoveComponentSuffix(),
                            memberNamesWithType(componentInfo.memberInfos)
                        ) +
                        string.Format(
                            "\n        {0} Remove{1}();",
                            interfaceName,
                            componentInfo.typeName.RemoveComponentSuffix()
                        );
                }).ToArray()
            );

            return string.Format(
                FILE_TEMPLATE,
                interfaceName,
                methods
            );
        }

        static string memberNamesWithType(List<PublicMemberInfo> memberInfos) {
            var typedArgs = memberInfos
                .Select(info => info.type.ToCompilableString() + " new" + info.name.UppercaseFirst())
                .ToArray();

            return string.Join(", ", typedArgs);
        }
    }
}
