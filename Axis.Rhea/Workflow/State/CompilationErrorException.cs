using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Axis.Rhea.Core.Workflow.State
{
    public class CompilationErrorException: Exception
    {
        public ImmutableArray<Diagnostic> Errors { get; private set; }

        public CompilationErrorException(string message, params Diagnostic[] diagnostics)
        : base(message)
        {
            Errors = diagnostics
                .Where(diagnostic => diagnostic is not null)
                .ToImmutableArray();
        }
    }
}
