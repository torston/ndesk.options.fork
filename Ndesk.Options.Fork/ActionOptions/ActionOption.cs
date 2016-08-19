namespace Ndesk.Options.Fork.ActionOptions
{
    using System;

    internal class ActionOption : Option
    {
        private readonly Action<OptionValueCollection> action;

        public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
            : base(prototype, description, count)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;
        }

        protected override void OnParseComplete(OptionContext c)
        {
            this.action(c.OptionValues);
        }
    }
}