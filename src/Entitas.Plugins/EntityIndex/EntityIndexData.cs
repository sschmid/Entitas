using DesperateDevs.Extensions;
using Jenny;

namespace Entitas.Plugins
{
    public class EntityIndexData : CodeGeneratorData
    {
        public const string TypeKey = "EntityIndex.Type";
        public const string NameKey = "EntityIndex.Name";
        public const string IsCustomKey = "EntityIndex.IsCustom";
        public const string CustomMethodsKey = "EntityIndex.CustomMethods";

        public const string KeyTypeKey = "EntityIndex.KeyType";
        public const string ComponentTypeKey = "EntityIndex.ComponentType";
        public const string MemberNameKey = "EntityIndex.MemberName";
        public const string HasMultipleKey = "EntityIndex.HasMultiple";
        public const string ContextsKey = "EntityIndex.Contexts";

        public string Type
        {
            get => (string)this[TypeKey];
            set
            {
                this[TypeKey] = value;
                this[NameKey] = CodeGeneratorExtensions.IgnoreNamespaces
                    ? value.ShortTypeName().RemoveComponentSuffix()
                    : value.RemoveDots().RemoveComponentSuffix();
            }
        }

        public string Name => (string)this[NameKey];

        public bool IsCustom
        {
            get => (bool)this[IsCustomKey];
            set => this[IsCustomKey] = value;
        }

        public MethodData[] CustomMethods
        {
            get => (MethodData[])this[CustomMethodsKey];
            set => this[CustomMethodsKey] = value;
        }

        public string KeyType
        {
            get => (string)this[KeyTypeKey];
            set => this[KeyTypeKey] = value;
        }

        public string ComponentType
        {
            get => (string)this[ComponentTypeKey];
            set => this[ComponentTypeKey] = value;
        }

        public string MemberName
        {
            get => (string)this[MemberNameKey];
            set => this[MemberNameKey] = value;
        }

        public bool HasMultiple
        {
            get => (bool)this[HasMultipleKey];
            set => this[HasMultipleKey] = value;
        }

        public string[] Contexts
        {
            get => (string[])this[ContextsKey];
            set => this[ContextsKey] = value;
        }
    }
}
