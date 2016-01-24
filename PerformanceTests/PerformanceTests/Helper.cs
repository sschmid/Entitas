using Entitas;

public static class Helper {
    public static Pool CreatePool() {
        return new Pool(CP.NumComponents, 0, new PoolMetaData("Pool", new string[CP.NumComponents]));
    }
}

