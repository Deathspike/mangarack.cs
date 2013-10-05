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
		private Dictionary<string, object> _KeyValueStore;

		#region Abstract
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
			return MemberExpression.Member.Name;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initialize a new instance of the KeyValueStore class.
		/// </summary>
		public KeyValueStore() {
			// Initialize a new instance of the KeyValueStore class.
			_KeyValueStore = new Dictionary<string, object>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		public bool Contains(Expression<Func<object>> Expression) {
			// Check if the key-value store contains the key.
			return Contains(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Check if the key-value store contains the key.
		/// </summary>
		/// <param name="Key">The key.</param>
		public bool Contains(string Key) {
			// Check if the key-value store contains the key.
			return _KeyValueStore.ContainsKey(Key);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		public T Get<T>(Expression<Func<object>> Expression) {
			// Get a value.
			return (T) Get(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		/// <param name="Default">The default value.</param>
		public T Get<T>(Expression<Func<object>> Expression, T Default) {
			// Get a value.
			return (T) Get(_ExpressionToKey(Expression), Default);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		public T Get<T>(string Key) {
			// Get a value.
			return (T) Get(Key);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		public T Get<T>(string Key, T Default) {
			// Get a value.
			return (T) Get(Key, (object) Default);
		}
		
		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		public object Get(Expression<Func<object>> Expression) {
			// Get a value.
			return Get(_ExpressionToKey(Expression), null);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		/// <param name="Default">The default value.</param>
		public object Get(Expression<Func<object>> Expression, object Default) {
			// Get a value.
			return Get(_ExpressionToKey(Expression), Default);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		public object Get(string Key) {
			// Get a value.
			return Get(Key, null);
		}

		/// <summary>
		/// Get a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Default">The default value.</param>
		public object Get(string Key, object Default) {
			// Get a value.
			return Contains(Key) ? _KeyValueStore[Key] : Default;
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		public bool Remove(Expression<Func<object>> Expression) {
			// Remove a value.
			return Remove(_ExpressionToKey(Expression));
		}

		/// <summary>
		/// Remove a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		public virtual bool Remove(string Key) {
			// Check if the key-value store contains the key.
			if (Contains(Key)) {
				// Remove the key from the key-value store.
				_KeyValueStore.Remove(Key);
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="Expression">The expression.</param>
		/// <param name="Value">The value.</param>
		public bool Set(Expression<Func<object>> Expression, object Value) {
			// Set a value.
			return Set(_ExpressionToKey(Expression), Value);
		}

		/// <summary>
		/// Set a value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		public virtual bool Set(string Key, object Value) {
			// Check if the key-value store does not contain the key or differs.
			if (!Contains(Key) || _KeyValueStore[Key] != Value) {
				// Set a value.
				_KeyValueStore[Key] = Value;
				// Return true.
				return true;
			}
			// Return false.
			return false;
		}
		#endregion
	}
}