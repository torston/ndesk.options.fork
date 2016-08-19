using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Ndesk.Options.Fork
{
    using Ndesk.Options.Fork.Common;

    public abstract class Option
    {
        private static readonly char[] NameTerminator = { '=', ':' };

        protected Option(string prototype, string description)
            : this(prototype, description, 1)
        {
        }

        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }

            if (prototype.Length == 0)
            {
                throw new ArgumentException("Cannot be the empty string.", "prototype");
            }

            if (maxValueCount < 0)
            {
                throw new ArgumentOutOfRangeException("maxValueCount");
            }

            this.Prototype = prototype;
            this.Names = prototype.Split('|');
            this.Description = description;
            this.MaxValueCount = maxValueCount;
            this.OptionValueType = this.ParsePrototype();

            if (this.MaxValueCount == 0 && this.OptionValueType != OptionValueType.None)
            {
                throw new ArgumentException(
                    "Cannot provide maxValueCount of 0 for OptionValueType.Required or " + "OptionValueType.Optional.",
                    "maxValueCount");
            }

            if (this.OptionValueType == OptionValueType.None && maxValueCount > 1)
            {
                throw new ArgumentException(
                    string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
                    "maxValueCount");
            }

            if (Array.IndexOf(this.Names, "<>") >= 0
                && ((this.Names.Length == 1 && this.OptionValueType != OptionValueType.None)
                    || (this.Names.Length > 1 && this.MaxValueCount > 1)))
            {
                throw new ArgumentException("The default option handler '<>' cannot require values.", "prototype");
            }
        }

        public string Description { get; set; }

        public int MaxValueCount { get; set; }

        public OptionValueType OptionValueType { get; set; }

        public string Prototype { get; set; }

        internal string[] Names { get; set; }

        internal string[] ValueSeparators { get; private set; }

        public string[] GetNames()
        {
            return (string[])this.Names.Clone();
        }

        public string[] GetValueSeparators()
        {
            if (this.ValueSeparators == null)
            {
                return new string[0];
            }

            return (string[])this.ValueSeparators.Clone();
        }

        public void Invoke(OptionContext c)
        {
            this.OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        public override string ToString()
        {
            return this.Prototype;
        }

        protected static T Parse<T>(string value, OptionContext c)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            var t = default(T);
            try
            {
                if (value != null)
                {
                    t = (T)conv.ConvertFromString(value);
                }
            }
            catch (Exception e)
            {
                throw new OptionException(
                    string.Format(
                        c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."),
                        value,
                        typeof(T).Name,
                        c.OptionName),
                    c.OptionName,
                    e);
            }

            return t;
        }

        protected abstract void OnParseComplete(OptionContext c);

        private static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            var start = -1;
            for (var i = end + 1; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                        {
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                "name");
                        }

                        start = i + 1;
                        break;

                    case '}':
                        if (start == -1)
                        {
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name),
                                "name");
                        }

                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;

                    default:
                        if (start == -1)
                        {
                            seps.Add(name[i].ToString());
                        }

                        break;
                }
            }

            if (start != -1)
            {
                throw new ArgumentException(
                    string.Format("Ill-formed name/value separator found in \"{0}\".", name));
            }
        }

        private OptionValueType ParsePrototype()
        {
            var type = '\0';
            var seps = new List<string>();
            for (var i = 0; i < this.Names.Length; ++i)
            {
                var name = this.Names[i];
                if (name.Length == 0)
                {
                    throw new ArgumentException("Empty option names are not supported.");
                }

                var end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                {
                    continue;
                }

                this.Names[i] = name.Substring(0, end);
                if (type == '\0' || type == name[end])
                {
                    type = name[end];
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]));
                }

                AddSeparators(name, end, seps);
            }

            if (type == '\0')
            {
                return OptionValueType.None;
            }

            if (this.MaxValueCount <= 1 && seps.Count != 0)
            {
                throw new ArgumentException(
                    string.Format(
                        "Cannot provide key/value separators for Options taking {0} value(s).",
                        this.MaxValueCount));
            }

            if (this.MaxValueCount > 1)
            {
                if (seps.Count == 0)
                {
                    this.ValueSeparators = new[] { ":", "=" };
                }
                else if (seps.Count == 1 && seps[0].Length == 0)
                {
                    this.ValueSeparators = null;
                }
                else
                {
                    this.ValueSeparators = seps.ToArray();
                }
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }
    }
}