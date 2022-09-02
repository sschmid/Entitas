using System.Linq;
using DesperateDevs.Roslyn;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using Microsoft.CodeAnalysis;

namespace Entitas.Roslyn.CodeGeneration.Plugins {

    public class EventComponentDataProvider : IComponentDataProvider {

        public void Provide(INamedTypeSymbol type, ComponentData data) {
            var attrs = type.GetAttributes<EventAttribute>();
            if (attrs.Length > 0) {
                data.IsEvent(true);
                var eventData = attrs
                    .Select(attr => {
                        var args = attr.ConstructorArguments;
                        var eventTarget = (EventTarget)args[0].Value;
                        var eventType = (EventType)args[1].Value;
                        var priority = (int)args[2].Value;

                        return new EventData(eventTarget, eventType, priority);
                    }).ToArray();

                data.SetEventData(eventData);
            } else {
                data.IsEvent(false);
            }
        }
    }
}
