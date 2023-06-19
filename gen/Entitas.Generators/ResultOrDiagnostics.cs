using Microsoft.CodeAnalysis;

namespace Entitas.Generators;

public class ResultOrDiagnostics<T>
{
    public T? Result;
    public Diagnostic? Diagnostic;

    public ResultOrDiagnostics(T result)
    {
        Result = result;
    }

    public ResultOrDiagnostics(Diagnostic diagnostic)
    {
        Diagnostic = diagnostic;
    }

    public static implicit operator ResultOrDiagnostics<T>(T result) => new ResultOrDiagnostics<T>(result);
    public static implicit operator ResultOrDiagnostics<T>(Diagnostic diagnostic) => new ResultOrDiagnostics<T>(diagnostic);
}
