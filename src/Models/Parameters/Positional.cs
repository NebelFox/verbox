using System;

namespace Verbox.Models.Parameters
{
    public record Positional(string Name, 
                             Type Type, 
                             int MinValuesCount, 
                             int MaxValuesCount);
}
