namespace NDesk.Options.Fork.Common
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class OptionException : Exception
    {
        public OptionException()
        {
        }

        public OptionException(string message, string optionName)
            : base(message)
        {
            this.OptionName = optionName;
        }

        public OptionException(string message, string optionName, Exception innerException)
            : base(message, innerException)
        {
            this.OptionName = optionName;
        }

        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.OptionName = info.GetString("OptionName");
        }

        public string OptionName { get; set; }

        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OptionName", this.OptionName);
        }
    }
}