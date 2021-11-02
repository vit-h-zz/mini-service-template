using System;
using System.Collections.Generic;

namespace TemplateService.Data.Domain.Core
{
    public class ModelValidationException : Exception
    {
        public ModelValidationException(IEnumerable<KeyValuePair<string, string>> keyValuePairs) : base("The model is not valid.")
        {
            foreach (var kvp in keyValuePairs)
                Data.Add(kvp.Key, kvp.Value);
        }
    }
}
