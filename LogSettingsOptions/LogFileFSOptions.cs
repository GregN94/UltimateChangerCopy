using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSettingsOptions
{
    public class LogFileFSOptions
    {
        public static SortedDictionary<string, string> mode = new SortedDictionary<string, string>
            {
                { "ALL", "ALL"},
                { "DEBUG", "DEBUG"},
                { "ERROR", "ERROR"}
            };

        public LogFileFSOptions()
        {
            Options.Add(new Option());
        }

        internal List<Option> Options { get; set; }
    }

    public class Option
    {
        string name;

        public Option()
        {
            Keys = new List<Key>();
            Values = new List<Value>();
        }

        public Option(string name, List<Key> keys, List<Value> values)
        {
            this.name = name;
            this.Keys = keys;
            this.Values = values;
        }

        public List<Key> Keys { get; set; }
        internal List<Value> Values { get; set; }
    }

    public class Key
    {
        public Key(string key)
        {
            this.key = key;
        }

        public string key { get; set; }
    }

    public class Value
    {
        string value;

        public Value(string value)
        {
            this.value = value;
        }
    }

}
