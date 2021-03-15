namespace DemoMapper.Tests
{
    using System.IO;
    using HL7.Dotnetcore;

    public class DemoMapperFixture 
    {

        public DemoMapper Mapper { get; private set; }

        public DemoMapperFixture() {
            var fileText = File.ReadAllText("./Resources/sample_0.hl7");
            var hl7Message = new Message(fileText);
            hl7Message.ParseMessage();
            var logger = new Hl7Logger(hl7Message, s => {});
            Mapper = new DemoMapper(hl7Message, logger);
        }
    }
}
