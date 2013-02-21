// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MangaRack.Provider {
	/// <summary>
	/// Represents a key-value store
	/// </summary>
	public abstract class KeyValueStore {
		/// <summary>
		/// Contains each key and value.
		/// </summary>
		private Dictionary<string, object> _KeyValueStore;

		/// <summary>
		/// Convert the expression to a key.
		/// </summary>
		/// <param name="Expression">The expression representing the key.</param>
		private string _ExpressionToKey(Expression<Func<object>> Expression) {
			// Retrieve the LambdaExpression.
			LambdaExpression LambdaExpression = Expression as LambdaExpression;
			// Retrieve the MemberExpression.
			MemberExpression MemberExpression = (LambdaExpression.Body is UnaryExpression ? (LambdaExpression.Body as UnaryExpression).Operand : LambdaExpression.Body) as MemberExpression;
			// Return the name.
			return (MemberExpression.Member as PropertyInfo).Name;
		}

		/// <summary>
		/// Initialize a new instance of the KeyValueStore class.
		/// </summary>
		public KeyValueStore() {
			// Initialize a new instance of the KeyValueStore class.
			_KeyValueStore = new Dictionary<string, object>();
		}

		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		protected bool _Contains(Expression<Func<object>> Expression) {
			// Check if the key-value store contains the key.
			return _Contains(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="Key">The key.</param>
		protected bool _Contains(string Key) {
			// Check if the key-value store contains the key.
			return _KeyValueStore.ContainsKey(Key);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		protected object _Get(Expression<Func<object>> Expression) {
			// Get a value.
			return _Get(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		protected object _Get(string Key) {
			// Get a value.
			return _Contains(Key) ? _KeyValueStore[Key] : null;
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		protected void _Remove(Expression<Func<object>> Expression) {
			// Remove a value.
			_Remove(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		protected void _Remove(string Key) {
			// Check if the key-value store contains the key.
			if (_Contains(Key)) {
				// Remove the key from the key-value store.
				_KeyValueStore.Remove(Key);
			}
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		/// <param name="Value">The value.</param>
		protected void _Set(Expression<Func<object>> Expression, object Value) {
			// Set a value.
			_Set(_ExpressionToKey(Expression), Value);
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		protected void _Set(string Key, object Value) {
			// Set a value.
			_KeyValueStore[Key] = Value;
		}
	}
}