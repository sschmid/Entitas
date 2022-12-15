using DesperateDevs.Extensions;
using Jenny;

namespace Entitas.Plugins
{
    public class ComponentData : CodeGeneratorData
    {
        public const string TypeKey = "Component.Type";
        public const string NameKey = "Component.Name";
        public const string ValidLowerFirstNameKey = "Component.ValidName";
        public const string MemberDataKey = "Component.MemberData";
        public const string ContextsKey = "Component.Contexts";
        public const string IsUniqueKey = "Component.IsUnique";
        public const string FlagPrefixKey = "Component.FlagPrefix";
        public const string GeneratesKey = "Component.Generates";
        public const string GeneratesIndexKey = "Component.GeneratesIndex";
        public const string GeneratesObjectKey = "Component.GeneratesObject";
        public const string ObjectTypeKey = "Component.ObjectType";
        public const string IsEventKey = "Component.IsEvent";
        public const string EventDataKey = "Component.EventData";

        public ComponentData() { }

        public ComponentData(CodeGeneratorData data) : base(data) { }

        public string Type
        {
            get => (string)this[TypeKey];
            set
            {
                this[TypeKey] = value;

                var name = CodeGeneratorExtensions.IgnoreNamespaces
                    ? value.ShortTypeName().RemoveComponentSuffix()
                    : value.RemoveDots().RemoveComponentSuffix();

                this[NameKey] = name;
                this[ValidLowerFirstNameKey] = name.ToLowerFirst().AddPrefixIfIsKeyword();
            }
        }

        public string Name => (string)this[NameKey];
        public string ValidLowerFirstName => (string)this[ValidLowerFirstNameKey];

        public MemberData[] MemberData
        {
            get => (MemberData[])this[MemberDataKey];
            set => this[MemberDataKey] = value;
        }

        public string[] Contexts
        {
            get => (string[])this[ContextsKey];
            set => this[ContextsKey] = value;
        }

        public bool IsUnique
        {
            get => (bool)this[IsUniqueKey];
            set => this[IsUniqueKey] = value;
        }

        public string FlagPrefix
        {
            get => (string)this[FlagPrefixKey];
            set => this[FlagPrefixKey] = value;
        }

        public bool Generates
        {
            get => (bool)this[GeneratesKey];
            set => this[GeneratesKey] = value;
        }

        public bool GeneratesIndex
        {
            get => (bool)this[GeneratesIndexKey];
            set => this[GeneratesIndexKey] = value;
        }

        public bool GeneratesObject
        {
            get => (bool)this[GeneratesObjectKey];
            set => this[GeneratesObjectKey] = value;
        }

        public string ObjectType
        {
            get => (string)this[ObjectTypeKey];
            set => this[ObjectTypeKey] = value;
        }

        public bool IsEvent
        {
            get => (bool)this[IsEventKey];
            set => this[IsEventKey] = value;
        }

        public EventData[] EventData
        {
            get => (EventData[])this[EventDataKey];
            set => this[EventDataKey] = value;
        }
    }
}
