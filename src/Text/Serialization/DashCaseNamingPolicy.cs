using System.Text.Json;
using Verbox.Extensions;

namespace Verbox.Text.Serialization
{
    internal class DashCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.PascalToDash();
        }
    }
}
