// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MangaRack.ViewModel {
	/// <summary>
	/// Represents a key-value store
	/// </summary>
	public abstract class KeyValueStore {
		/// <summary>
		/// Contains each key and value.
		/// </summary>
		private Dictionary<string, object> _keyValueStore;

		#region Abstract
		/// <summary>
		/// Convert the expression to a key.
		/// </summary>
		/// <param name="expression">The expression.</param>
		private string _Key(Expression<Func<object>> expression) {
			// Return the key.
			return ((MemberExpression)expression.Body).Member.Name;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the KeyValueStore class.
		/// </summary>
		public KeyValueStore() {
			// Initialize a new instance of the Dictionary class.
			_keyValueStore = new Dictionary<string, object>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public bool Contains(Expression<Func<object>> expression) {
			// Check if the key-value store contains the key.
			return Contains(_Key(expression));
		}

		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="key">The key.</param>
		public bool Contains(string key) {
			// Check if the key-value store contains the key.
			return _keyValueStore.ContainsKey(key);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public T Get<T>(Expression<Func<object>> expression) {
			// Return a value.
			return (T) Get(_Key(expression));
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="defaultValue">The default value.</param>
		public T Get<T>(Expression<Func<object>> expression, T defaultValue) {
			// Return a value.
			return (T) Get(_Key(expression), defaultValue);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="key">The key.</param>
		public T Get<T>(string key) {
			// Return a value.
			return (T) Get(key);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		public T Get<T>(string key, T defaultValue) {
			// Return a value.
			return (T) Get(key, (object) defaultValue);
		}
		
		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public object Get(Expression<Func<object>> expression) {
			// Return a value.
			return Get(_Key(expression), null);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="defaultValue">The default value.</param>
		public object Get(Expression<Func<object>> expression, object defaultValue) {
			// Return a value.
			return Get(_Key(expression), defaultValue);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="key">The key.</param>
		public object Get(string key) {
			// Return a value.
			return Get(key, null);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		public object Get(string key, object defaultValue) {
			// Return a value.
			return Contains(key) ? _keyValueStore[key] : defaultValue;
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		public bool Remove(Expression<Func<object>> expression) {
			// Remove a value.
			return Remove(_Key(expression));
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="key">The key.</param>
		public virtual bool Remove(string key) {
			// Check if the key-value store contains the key.
			if (Contains(key)) {
				// Remove the key from the key-value store.
				_keyValueStore.Remove(key);
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <param name="value">The value.</param>
		public bool Set(Expression<Func<object>> expression, object value) {
			// Set a value.
			return Set(_Key(expression), value);
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public virtual bool Set(string key, object value) {
			// Check if the key-value store does not contain the key or differs.
			if (!Contains(key) || _keyValueStore[key] != value) {
				// Set a value.
				_keyValueStore[key] = value;
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}
		#endregion
	}
}