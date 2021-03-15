namespace DemoMapper.Tests
{
    using System;
    using System.Linq;
    using Xunit;
    using Hl7.Fhir.Model;

    public class PatientMapping : IClassFixture<DemoMapperFixture>
    {

        DemoMapperFixture _mapperFixture;

        public PatientMapping(DemoMapperFixture mapperFixture) {
            _mapperFixture = mapperFixture;
        }

        [Fact]
        public void PatientName()
        {
            var resource = _mapperFixture.Mapper.Map();
            var bundle = resource as Bundle;
            var patient = bundle.Entry[0].Resource as Patient;
            var name = patient.Name[0];
            var givenName = name.Given.ToArray();
            Assert.Equal("FLOYD", name.Family);
            Assert.Equal("FRANK", givenName[0]);
            Assert.Equal("", givenName[1]);
        }
    }
}
