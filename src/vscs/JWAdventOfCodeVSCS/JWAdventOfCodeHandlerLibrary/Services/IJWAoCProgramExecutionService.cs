using JWAdventOfCodeHandlerLibrary.Settings.Program;
using JWAdventOfCodeHandlingLibrary.HTTP;

namespace JWAdventOfCodeHandlerLibrary.Services;

public interface IJWAoCProgramExecutionService
{
    // methods
    public IJWAoCHTTPResponse CallProgramWithLocalHTTPGet(JWAoCProgram program, string currentHTTPURIString);

    public IJWAoCHTTPResponse CallProgramWithLocalHTTP(JWAoCProgram program, params string[] args);
}