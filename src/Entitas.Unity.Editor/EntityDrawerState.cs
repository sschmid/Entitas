using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static partial class EntityDrawer
    {
        static Dictionary<string, bool[]> _contextToUnfoldedComponents;

        public static Dictionary<string, bool[]> ContextToUnfoldedComponents =>
            _contextToUnfoldedComponents ??= new Dictionary<string, bool[]>();

        static Dictionary<string, string[]> _contextToComponentMemberSearch;

        public static Dictionary<string, string[]> ContextToComponentMemberSearch =>
            _contextToComponentMemberSearch ??= new Dictionary<string, string[]>();

        public struct ComponentInfo
        {
            public int Index;
            public string Name;
            public Type Type;
        }

        static Dictionary<string, ComponentInfo[]> _contextToComponentInfos;

        public static Dictionary<string, ComponentInfo[]> ContextToComponentInfos =>
            _contextToComponentInfos ??= new Dictionary<string, ComponentInfo[]>();

        static GUIStyle _foldoutStyle;

        public static GUIStyle FoldoutStyle
        {
            get
            {
                if (_foldoutStyle == null)
                {
                    _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    _foldoutStyle.fontStyle = FontStyle.Bold;
                }

                return _foldoutStyle;
            }
        }

        static string _componentNameSearchString;

        public static string ComponentNameSearchString
        {
            get => _componentNameSearchString ??= string.Empty;
            set => _componentNameSearchString = value;
        }

        public static readonly IDefaultInstanceCreator[] DefaultInstanceCreators;
        public static readonly ITypeDrawer[] TypeDrawers;
        public static readonly IComponentDrawer[] ComponentDrawers;

        static EntityDrawer()
        {
            DefaultInstanceCreators = AppDomain.CurrentDomain.GetInstancesOf<IDefaultInstanceCreator>().ToArray();
            TypeDrawers = AppDomain.CurrentDomain.GetInstancesOf<ITypeDrawer>().ToArray();
            ComponentDrawers = AppDomain.CurrentDomain.GetInstancesOf<IComponentDrawer>().ToArray();
        }

        static bool[] GetUnfoldedComponents(IEntity entity)
        {
            if (!ContextToUnfoldedComponents.TryGetValue(entity.ContextInfo.Name, out var unfoldedComponents))
            {
                unfoldedComponents = new bool[entity.TotalComponents];
                for (var i = 0; i < unfoldedComponents.Length; i++)
                    unfoldedComponents[i] = true;

                ContextToUnfoldedComponents.Add(entity.ContextInfo.Name, unfoldedComponents);
            }

            return unfoldedComponents;
        }

        static string[] GetComponentMemberSearch(IEntity entity)
        {
            if (!ContextToComponentMemberSearch.TryGetValue(entity.ContextInfo.Name, out var componentMemberSearch))
            {
                componentMemberSearch = new string[entity.TotalComponents];
                for (var i = 0; i < componentMemberSearch.Length; i++)
                    componentMemberSearch[i] = string.Empty;

                ContextToComponentMemberSearch.Add(entity.ContextInfo.Name, componentMemberSearch);
            }

            return componentMemberSearch;
        }

        static ComponentInfo[] GetComponentInfos(IEntity entity)
        {
            if (!ContextToComponentInfos.TryGetValue(entity.ContextInfo.Name, out var infos))
            {
                var contextInfo = entity.ContextInfo;
                var infosList = new List<ComponentInfo>(contextInfo.ComponentTypes.Length);
                for (var i = 0; i < contextInfo.ComponentTypes.Length; i++)
                {
                    infosList.Add(new ComponentInfo
                    {
                        Index = i,
                        Name = contextInfo.ComponentNames[i],
                        Type = contextInfo.ComponentTypes[i]
                    });
                }

                infos = infosList.ToArray();
                ContextToComponentInfos.Add(entity.ContextInfo.Name, infos);
            }

            return infos;
        }

        static IComponentDrawer GetComponentDrawer(Type type)
        {
            foreach (var drawer in ComponentDrawers)
                if (drawer.HandlesType(type))
                    return drawer;

            return null;
        }

        static ITypeDrawer GetTypeDrawer(Type type)
        {
            foreach (var drawer in TypeDrawers)
                if (drawer.HandlesType(type))
                    return drawer;

            return null;
        }
    }
}
