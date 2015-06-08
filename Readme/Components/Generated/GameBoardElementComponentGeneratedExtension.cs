namespace Entitas {
    public partial class Entity {
        static readonly GameBoardElementComponent gameBoardElementComponent = new GameBoardElementComponent();

        public bool isGameBoardElement {
            get { return HasComponent(ComponentIds.GameBoardElement); }
            set {
                if (value != isGameBoardElement) {
                    if (value) {
                        AddComponent(ComponentIds.GameBoardElement, gameBoardElementComponent);
                    } else {
                        RemoveComponent(ComponentIds.GameBoardElement);
                    }
                }
            }
        }

        public Entity IsGameBoardElement(bool value) {
            isGameBoardElement = value;
            return this;
        }
    }

    public partial class Matcher {
        static AllOfMatcher _matcherGameBoardElement;

        public static AllOfMatcher GameBoardElement {
            get {
                if (_matcherGameBoardElement == null) {
                    _matcherGameBoardElement = new Matcher(ComponentIds.GameBoardElement);
                }

                return _matcherGameBoardElement;
            }
        }
    }
}
