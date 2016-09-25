using Entitas;
using Entitas.CodeGenerator;

public static class Helper {
    public static Pool CreatePool() {
        return new Pool(CP.NumComponents, 0, new PoolMetaData(CodeGenerator.DEFAULT_POOL_NAME, new string[CP.NumComponents], null));
    }
}

