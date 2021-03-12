namespace hl7_mapper_demo
{
    using System;
    using System.IO;
    using HL7.Dotnetcore;
    using Hl7.Fhir.Serialization;

    class Program
    {
        static void Main(string[] args)
        {

            var fileText = File.ReadAllText("./Resources/sample_0.hl7");
            var hl7Message = new Message(fileText);
            hl7Message.ParseMessage();
            var logger = new Hl7Logger(hl7Message, Console.WriteLine);
            var mapper = new DemoMapper(hl7Message, logger);
            var fhirMessage = mapper.Map();
            var fhirSerializer = new FhirJsonSerializer();
            var fhirJson = fhirSerializer.SerializeToString(fhirMessage);
            Console.WriteLine(fhirJson);
        }
    }
}
