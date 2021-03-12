namespace hl7_mapper_demo
{
    using System;
    using HL7.Dotnetcore;

    class Hl7Logger
    {
        Action<string> _appender;
        Message _message;

        public Hl7Logger(Message message, Action<string> appender)
        {
            _message = message;
            _appender = appender;
        }

        public void LogSegment(string segment)
        {
            try {
                var value = _message.GetValue(segment);
                _appender($"{segment}: {value}");
            } catch (Exception ex) {
                _appender($"{segment}: {ex.Message}");
            }
        }
    }
}