using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Orchard.ContentManagement
{
    /// <summary>
    /// Common traits of <see cref="ContentItem"/>, <see cref="ContentPart"/>
    /// and <see cref="ContentField"/>
    /// </summary>
    public class ContentElement : IContent
    {
        protected ContentElement() : this(new JObject())
        {
        }

        protected ContentElement(JObject data)
        {
            Data = data;
        }

        [JsonIgnore]
        public dynamic Content { get { return Data; } }

        [JsonIgnore]
        internal JObject Data { get; set; }

        [JsonIgnore]
        public ContentItem ContentItem { get; protected set; }

        /// <summary>
        /// Whether the content has a named property or not.
        /// </summary>
        /// <param name="name">The name of the property to look for.</param>
        public bool Has(string name)
        {
            JToken value;
            return Data.TryGetValue(name, out value);
        }

        /// <summary>
        /// Projects the content to a custom type.
        /// </summary>
        public T Get<T>(string name)
        {
            JToken value;
            if (Data.TryGetValue(name, out value))
            {
                var obj = value as JObject;
                if (value == null)
                {
                    return default(T);
                }

                var result = obj.ToObject<T>();
                var contentElement = result as ContentElement;
                contentElement.Data = obj;
                contentElement.ContentItem = ((IContent)this).ContentItem;

                return result;
            }

            return default(T);
        }

        public ContentElement Get(Type t, string name)
        {
            JToken value;
            if (Data.TryGetValue(name, out value))
            {
                var obj = value as JObject;
                if (value == null)
                {
                    return null;
                }

                var result = obj.ToObject(t);
                var contentElement = result as ContentElement;
                contentElement.Data = obj;
                contentElement.ContentItem = ((IContent)this).ContentItem;

                return contentElement;
            }

            return null;
        }

        /// <summary>
        /// Extract some named content.
        /// </summary>
        public ContentElement Get(string name)
        {
            JToken value;
            if (Data.TryGetValue(name, out value))
            {
                var obj = value as JObject;
                if (value == null)
                {
                    return null;
                }

                var contentElement = new ContentElement(obj);
                contentElement.ContentItem = ((IContent)this).ContentItem;

                return contentElement;
            }

            return null;
        }

        /// <summary>
        /// Adds or replaces a projection by its name.
        /// </summary>
        public void Weld(string name, object obj)
        {
            Data[name] = JObject.FromObject(obj);
        }
    }
}