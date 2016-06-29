using System.Linq;
using Entitas.CodeGeneration;
using NSpec;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CodeGenerator.Reflection.Providers;
using Entitas.Migration;

class describe_SerializeComponentInfo : nspec {


    void when_serializing() {

        it["serializes Componentinfo"] = () => {
            var infos = TypeReflectionProvider.GetComponentInfos(new [] { typeof(PersonComponent) });
            var info = infos.Single();

            var binaryFormatter = new BinaryFormatter();

            using (var stream = new MemoryStream()) {
                binaryFormatter.Serialize(stream, info);


                stream.Seek(0, SeekOrigin.Begin);

                var newInfo = (ComponentInfo)binaryFormatter.Deserialize(stream);
                var type = newInfo.memberInfos[0].fullTypeName;
            }
        };
    }
}

