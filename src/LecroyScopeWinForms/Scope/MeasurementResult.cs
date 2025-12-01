using System;

namespace LecroyScopeWinForms.Scope
{
    public sealed class MeasurementResult
    {
        public string Channel { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public double Value { get; init; }
        public string Unit { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
    }
}
