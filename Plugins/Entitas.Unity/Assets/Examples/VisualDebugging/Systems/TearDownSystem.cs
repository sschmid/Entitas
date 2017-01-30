using Entitas.Api;

public class TearDownSystem : ITearDownSystem {

    public void TearDown() {
        UnityEngine.Debug.Log("TearDown");
    }
}
