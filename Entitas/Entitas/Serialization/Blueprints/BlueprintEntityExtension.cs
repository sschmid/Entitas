




// TODO Move method into entity
// TODO Add to IEntity



//using Entitas.Serialization.Blueprints;

//namespace Entitas {

//    public partial class XXXEntity {

//        /// Adds all components from the blueprint to the entity.
//        /// When 'replaceComponents' is set to true entity.ReplaceComponent()
//        /// will be used instead of entity.AddComponent().
//        public IEntity ApplyBlueprint(Blueprint blueprint,
//                                     bool replaceComponents = false) {
//            var componentsLength = blueprint.components.Length;
//            for (int i = 0; i < componentsLength; i++) {
//                var componentBlueprint = blueprint.components[i];
//                if(replaceComponents) {
//                    ReplaceComponent(componentBlueprint.index,
//                                     componentBlueprint.CreateComponent(this));
//                } else {
//                    AddComponent(componentBlueprint.index,
//                                 componentBlueprint.CreateComponent(this));
//                }
//            }

//            return this;
//        }
//    }
//}
