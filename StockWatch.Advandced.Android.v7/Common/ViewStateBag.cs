using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Splat;

namespace StockWatch.Advandced
{
    public class ViewStateBag : Dictionary<string, object>, IEnableLogger
    {

        /// <summary>
        /// Gets a value indicating whether [has values].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has values]; otherwise, <c>false</c>.
        /// </value>
        public bool HasValues
        {
            get { return this.Count > 0; }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        public new void Add(string key, object value)
        {

            this.Log().Debug("ViewStateBag.Add({0}) => {1}", key, value);

            if (this.ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                base.Add(key, value);
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (this.ContainsKey(key))
            {
                return (T)this[key];
            }
            return default(T);
        }

    }
}