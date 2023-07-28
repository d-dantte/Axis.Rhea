using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Axis.Rhea.Directives.Contract.Serializers.MSJson
{
    public class ResponsePayloadConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            throw new NotImplementedException();
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
