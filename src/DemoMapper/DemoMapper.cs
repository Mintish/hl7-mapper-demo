namespace DemoMapper 
{
    using System;
    using System.Globalization;
    using Hl7.Fhir.Model;
    using HL7.Dotnetcore;
    using System.Collections.Generic;

    public class DemoMapper 
    {
        Hl7Logger _logger;
        Message _message;

        public DemoMapper(Message message, Hl7Logger logger)
        {
            _message = message;
            _logger = logger;
        }

        public Resource Map() {
            _logger.LogSegment("PID.3");
            _logger.LogSegment("PID.5");
            _logger.LogSegment("PID.5.1");
            _logger.LogSegment("PID.5.2");
            _logger.LogSegment("PID.5.3");
            _logger.LogSegment("PID.5");
            _logger.LogSegment("PID.9");
            _logger.LogSegment("PID.13");
            _logger.LogSegment("PID.14");
            _logger.LogSegment("PID.40");
            _logger.LogSegment("PID.8");
            _logger.LogSegment("PID.7");
            _logger.LogSegment("PID.30");
            _logger.LogSegment("PID.29");
            _logger.LogSegment("PID.11");
            _logger.LogSegment("PID.16");
            _logger.LogSegment("PID.24");
            _logger.LogSegment("PID.25");
            _logger.LogSegment("PID.16");

            _logger.LogSegment("NK1.7");
            _logger.LogSegment("NK1.3");
            _logger.LogSegment("NK1.2");
            _logger.LogSegment("NK1.5");
            _logger.LogSegment("NK1.6");
            _logger.LogSegment("NK1.40");
            _logger.LogSegment("NK1.4");
            _logger.LogSegment("NK1.15");
            _logger.LogSegment("NK1.13");
            _logger.LogSegment("NK1.30");
            _logger.LogSegment("NK1.31");
            _logger.LogSegment("NK1.32");
            _logger.LogSegment("NK1.41");

            var hl7Gender = _message.GetValue("PID.8");
            AdministrativeGender gender;
            switch (hl7Gender) {
                case "M": gender = AdministrativeGender.Male; break;
                case "F": gender = AdministrativeGender.Female; break;
                case "A":
                case "N":
                case "O": gender = AdministrativeGender.Other; break;
                default: gender = AdministrativeGender.Unknown; break;
            }

            DateTime? birthDate;
            if (DateTime.TryParseExact(
                    _message.GetValue("PID.7"),
                    "yyyyMMdd",
                    new CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out DateTime outBirthDate
                )) 
            {
                birthDate = outBirthDate;
            } else {
                birthDate = null;
            }

            var hl7MaritalStatus = _message.GetValue("PID.16");
            (string code, string text) maritalStatus;
            switch (hl7MaritalStatus) {
                case "A": maritalStatus = ("L", "Legally Separated"); break;
                case "B": maritalStatus = ("U", "Unmarried"); break;
                case "C": maritalStatus = ("C", "Common Law"); break;
                case "D": maritalStatus = ("D", "Divorced"); break;
                case "E": maritalStatus = ("L", "Legally Separated"); break;
                case "G": maritalStatus = ("U", "Unmarried"); break;
                case "I": maritalStatus = ("I", "Interlocutory"); break;
                case "M": maritalStatus = ("M", "Married"); break;
                case "N": maritalStatus = ("A", "Annulled"); break;
                case "O": maritalStatus = ("OTH", "Other"); break;
                case "P": maritalStatus = ("T", "Domestic partner"); break;
                case "R": maritalStatus = ("T", "Domestic partner"); break;
                case "S": maritalStatus = ("S", "Never Married"); break;
                case "T": maritalStatus = ("NAVU", "Not Available"); break;
                case "U": maritalStatus = ("UNK", "Unknown"); break;
                case "W": maritalStatus = ("W", "Widowed"); break;
                default: maritalStatus = ("NI", "No Information"); break;
            }

            var hl7Contacts = _message.Segments("NK1");
            var contacts = new List<Patient.ContactComponent>();
            foreach (var contact in hl7Contacts) {
                contacts.Add(new Patient.ContactComponent{
                    Relationship = new List<CodeableConcept> {
                        new CodeableConcept {
                            Coding = new List<Coding> {
                                new Coding {
                                    System = "http://hl7.org/fhir/ValueSet/patient-contactrelationship",
                                    Code = "N",
                                    Display = "Next-of-Kin"
                                }
                            },
                            Text = "Next-of-Kin"
                        }
                    },
                    Name = new HumanName {
                        Use = HumanName.NameUse.Official,
                        Family = contact.Fields(2)?.Components(1)?.Value,
                        Given = new [] {
                            contact.Fields(2)?.Components(2)?.Value,
                            contact.Fields(2)?.Components(3)?.Value
                        }
                    },
                    Telecom = new List<ContactPoint> {
                        new ContactPoint {
                            System = ContactPoint.ContactPointSystem.Phone,
                            Use = ContactPoint.ContactPointUse.Home,
                            Value = contact.Fields(5)?.Value
                        }
                    }
                });
            }

            var patient = new Patient {
                Identifier = new List<Identifier> {
                    new Identifier {
                        Use = Identifier.IdentifierUse.Official,
                        Value = _message.GetValue("PID.3")
                    }
                },
                Name = new List<HumanName> {
                    new HumanName {
                        Use = HumanName.NameUse.Official,
                        Family = _message.GetValue("PID.5.1"),
                        Given = new [] {
                            _message.GetValue("PID.5.2"),
                            _message.GetValue("PID.5.3")
                        }
                    }
                },
                Telecom = null,
                Gender = gender,
                BirthDate = string.Format("{0:yyyy-MM-dd}", birthDate),
                Address = new List<Address> {
                    new Address {
                        Line = new [] {
                            _message.GetValue("PID.11.1"),
                        },
                        City = _message.GetValue("PID.11.3"),
                        State = _message.GetValue("PID.11.4"),
                        PostalCode = _message.GetValue("PID.11.5")
                    }
                },
                MaritalStatus = new CodeableConcept {
                    Coding = new List<Coding> {
                        new Coding {
                            System = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus",
                            Code = maritalStatus.code,
                            Display = maritalStatus.text
                        }
                    },
                    Text = maritalStatus.text
                },
                Contact = contacts
            };

            return new Bundle {
                Identifier = new Identifier {
                    Value = Guid.NewGuid().ToString(),
                },
                TypeElement = new Code<Bundle.BundleType>(Bundle.BundleType.Transaction),
                Entry = new List<Bundle.EntryComponent> {
                    new Bundle.EntryComponent {
                        Request = new Bundle.RequestComponent {
                            Method = Bundle.HTTPVerb.POST
                        },
                        Resource = patient
                    }
                }
            };
        }
    }
}