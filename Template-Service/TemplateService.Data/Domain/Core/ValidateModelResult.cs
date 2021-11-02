using System;
using System.Collections.Generic;

namespace TemplateService.Data.Domain.Core
{
    public readonly struct ValidateModelResult
    {
        private static readonly Func<IDictionary<string, string>> EmptyValuesFunc = () => new Dictionary<string, string>(capacity: 0);

        public ValidateModelResult(bool valid, Func<IDictionary<string, string>>? getValuesFunc)
        {
            Valid = valid;
            GetValuesFunc = getValuesFunc ?? EmptyValuesFunc;
        }

        public bool Valid { get; }
        public Func<IDictionary<string, string>> GetValuesFunc { get; }
    }
}
