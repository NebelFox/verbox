using System.IO;
using System.Text.Json;
using Verbox.Extensions;

namespace Verbox.Text.Serialization
{
    /// <summary>
    /// Deserializes complex JSON files of full specifications
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Constructs a new instance with no styles or boxes
        /// </summary>
        public Serializer()
        {
            Styles = new StyleSerializer();
            Boxes = new BoxSerializer(Styles);
        }

        /// <summary>
        /// Access to already deserialized boxes
        /// </summary>
        public BoxSerializer Boxes { get; }

        /// <summary>
        /// Access to already deserialized styles
        /// </summary>
        public StyleSerializer Styles { get; }

        /// <summary>
        /// Deserializes a JSON in the specified file
        /// </summary>
        /// <param name="filepath">A JSON file to deserialize</param>
        public void Deserialize(string filepath)
        {
            Deserialize(JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(filepath)));
        }

        /// <summary>
        /// Deserializes a JSON object with styles and boxes
        /// </summary>
        /// <param name="element">Must contain at least "styles" or "boxes" property</param>
        public void Deserialize(JsonElement element)
        {
            element.AssertValueKind("root", JsonValueKind.Object);
            if (element.TryGetProperty("styles", out JsonElement styles))
                Styles.DeserializeMany(styles);
            if (element.TryGetProperty("boxes", out JsonElement boxes))
                Boxes.DeserializeMany(boxes);
        }
    }
}
