using System;
using ConcreteMC.MolangSharp.Parser;
using ConcreteMC.MolangSharp.Parser.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MolangSharp.Json.Newtonsoft
{
    /// <summary>
    ///     Implements a JsonConverter for reading <see cref="IExpression"/>
    /// </summary>
    public class MolangExpressionConverter : JsonConverter
    {
        public MolangExpressionConverter()
        {
            
        }
        
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                string molang = token.Value<string>();
                return MoLangParser.Parse(molang);
            }
            else if (token.Type == JTokenType.Integer)
            {
                return new NumberExpression(token.Value<double>());
            }
            else if (token.Type == JTokenType.Float)
            {
                return new NumberExpression(token.Value<double>());
            }
            else if (token.Type == JTokenType.Boolean)
            {
                return new BooleanExpression(token.Value<bool>());
            }

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IExpression).IsAssignableFrom(objectType);
        }
    }
}