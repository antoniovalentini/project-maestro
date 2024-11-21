using System.Diagnostics.CodeAnalysis;

namespace ProjectMaestro.EngineCore;

/// <summary>
/// Represents a result that can either be a successful value or an error.
/// </summary>
/// <typeparam name="TValue">The type of the successful value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public class Result<TValue, TError>
{
    private readonly TValue? _content;
    private readonly TError? _error;

    private Result(TValue content) { _content = content; }
    private Result(TError error) { _error = error; }

    public bool IsError([NotNullWhen(true)] out TError? error, [NotNullWhen(false)] out TValue? result)
    {
        error = default;
        result = default;

        if (_content is not null)
        {
            result = _content;
            return false;
        }

        if (_error is null)
        {
            throw new InvalidResultException("Both content and error are null");
        }
        error = _error;
        return true;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
    public static implicit operator Result<TValue, TError> (TError error) => new(error);
}

public class InvalidResultException : Exception
{
    public InvalidResultException(string message) : base(message) { }
}
