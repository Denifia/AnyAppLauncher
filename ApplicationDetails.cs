using System.Collections.Generic;
using System.Configuration;

namespace AnyApp
{
    public class ApplicationDetailsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("applications", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(List<ApplicationDetails>),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public GenericConfigurationElementCollection<ApplicationDetails> Applications
        {
            get
            {
                return (GenericConfigurationElementCollection<ApplicationDetails>)base["applications"];
            }
        }
    }

    public class ApplicationDetails : ConfigurationElement
    {
        public enum ApplicationPathType
        {
            Unknown = 0,
            Registry,
            File
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get
            { 
                return (string)this["name"]; 
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("path")]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        [ConfigurationProperty("pathType")]
        public int PathType
        {
            get
            {
                return (int)this["pathType"];
            }
            set
            {
                this["pathType"] = value;
            }
        }

        [ConfigurationProperty("format", DefaultValue = "{0}", IsRequired = false)]
        public string Format
        {
            get
            {
                return (string)this["format"];
            }
            set
            {
                this["format"] = value;
            }
        }
    }

    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement, new()
    {
        List<T> _elements = new List<T>();

        protected override ConfigurationElement CreateNewElement()
        {
            T newElement = new T();
            _elements.Add(newElement);
            return newElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return _elements.Find(e => e.Equals(element));
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
