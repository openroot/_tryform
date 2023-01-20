﻿using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;
using System.Text.Json;

using static _unit._unit._classcontainer;

namespace _unit
{
	/// <summary>
	/// _unit
	/// </summary>
	public class _unit
	{
		#region attribute

		private readonly _classconfiguration _classconfiguration;
		private TypeBuilder? _classbuilder { get; set; }
		private Type? _class { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// construct _unit
		/// </summary>
		/// <param name="_classconfiguration">_classconfiguration</param>
		public _unit(_classconfiguration _classconfiguration)
		{
			this._classconfiguration = _classconfiguration;
			this._class = null;

			if (!this._process())
			{
                throw new Exception("_unit is not created.");
            }
		}

		#endregion

		#region private

		private bool _process()
		{
			bool _issuccess = false;

            if (this._rectifyclassconfiguration())
            {
                if (this._structure())
                {
                    if (this._createunit())
                    {
                        if (_classcontainer._assigntype(this._retrievetype()))
						{
							_issuccess = true;
						}
						else
                        {
                            this._reset();
                            throw new Exception("_unit is not created.");
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Provided _classconfiguration is invalid.");
            }

            return _issuccess;
		}

		private bool _rectifyclassconfiguration()
		{
			if (this._classconfiguration != null)
			{
				if (!string.IsNullOrEmpty(this._classconfiguration._retrievename()))
				{
					bool _ispropertiescompliant = true;

					if (this._classconfiguration._retrieveproperties() != null)
					{
						foreach (_classconfiguration._propertyconfiguration _property in this._classconfiguration._retrieveproperties())
						{
							if (
								String.IsNullOrEmpty(_property._retrievename()) ||
								String.IsNullOrEmpty(_property._retrievetype()?.ToString() ?? string.Empty)
							)
							{
								_ispropertiescompliant = false;
							}
						}

						// return _classconfiguration checked as valid
						if (_ispropertiescompliant)
						{
							return true;
						}
					}
					else if (!_ispropertiescompliant)
					{
						throw new Exception("Property(s) of this unit is/are not compliant in provided _classconfiguration.");
					}
					else
					{
						throw new Exception("Property(s) of this unit returned null in provided _classconfiguration.");
					}
				}
				else
				{
					throw new Exception("Name of this unit returned null or empty in provided _classconfiguration.");
				}
			}
			else
			{
				throw new Exception("_classconfiguration not provided at startup.");
			}
			return false;
		}

		private bool _structure()
		{
			bool _issuccess = true;
			// _unit off class
			if (this._structureunitclass())
			{
				// _unit off constructor
				if (this._structureunitconstructor())
				{
					// _unit off properties
					for (int _index = 0; _index < this._classconfiguration._retrieveproperties().Count; _index++)
					{
                        _issuccess = this._structureunitproperty(_index);
						if (!_issuccess)
						{
							break;
						}
					}
				}
			}
			return _issuccess;
		}

		private bool _structureunitclass()
		{
			bool _issuccess = false;
			try
			{
				// _assembly , name
				AssemblyName _assemblyname = new AssemblyName(this._classconfiguration._retrievename());

				// _assembly , structure
				AssemblyBuilder _assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.RunAndCollect);

				// _module , structure
				ModuleBuilder _modulebuilder = _assemblybuilder.DefineDynamicModule(this._classconfiguration._retrievename());

				// _class , structure
				this._classbuilder = _modulebuilder.DefineType(_assemblyname.FullName,
					TypeAttributes.Public |
					TypeAttributes.Class |
					TypeAttributes.AutoClass |
					TypeAttributes.AnsiClass |
					TypeAttributes.BeforeFieldInit |
					TypeAttributes.AutoLayout,
					null
				);
				_issuccess = true;
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
			return _issuccess;
		}

		private bool _structureunitconstructor()
		{
			bool _issuccess = false;
			if (this._classbuilder != null)
			{
				try
				{
					// _class constructor , structure
					this._classbuilder?.DefineDefaultConstructor(
						MethodAttributes.Public |
						MethodAttributes.SpecialName |
						MethodAttributes.RTSpecialName
					);
					_issuccess = true;
				}
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
			}
			return _issuccess;
		}

		private bool _structureunitproperty(int _index)
		{
			bool _issuccess = false;
			if (this._classbuilder != null)
			{
                try
				{
					_classconfiguration._propertyconfiguration _property = this._classconfiguration._retrieveproperties()[_index];

                    Type? _type = _property._retrievetype();
					string? _name = _property._retrievename();
					if (_type != null)
					{
						if (_name != null)
						{
							// _property _field , structure
							FieldBuilder _fieldbuilder = this._classbuilder.DefineField("_field" + _name, _type, FieldAttributes.Private);

							// _property _method , structure , get
							MethodBuilder _methodbuilderget = this._classbuilder.DefineMethod("_get" + _name,
								MethodAttributes.Public |
								MethodAttributes.SpecialName |
								MethodAttributes.HideBySig,
								_type,
								Type.EmptyTypes
							);
							// _property _method , structure , get immediate language generator
							ILGenerator _immediatelanguagegeneratorget = _methodbuilderget.GetILGenerator();
							_immediatelanguagegeneratorget.Emit(OpCodes.Ldarg_0);
							_immediatelanguagegeneratorget.Emit(OpCodes.Ldfld, _fieldbuilder);
							_immediatelanguagegeneratorget.Emit(OpCodes.Ret);

							// _property _method , structure , set
							MethodBuilder _methodbuilderset = this._classbuilder.DefineMethod("_set" + _name,
								MethodAttributes.Public |
								MethodAttributes.SpecialName |
								MethodAttributes.HideBySig,
								null,
								new[] { _type }
							);
							// _property _method , structure , set immediate language generator
							ILGenerator _immediatelanguagegeneratorset = _methodbuilderset.GetILGenerator();
							Label _modifyproperty = _immediatelanguagegeneratorset.DefineLabel();
							Label _exitset = _immediatelanguagegeneratorset.DefineLabel();
							_immediatelanguagegeneratorset.MarkLabel(_modifyproperty);
							_immediatelanguagegeneratorset.Emit(OpCodes.Ldarg_0);
							_immediatelanguagegeneratorset.Emit(OpCodes.Ldarg_1);
							_immediatelanguagegeneratorset.Emit(OpCodes.Stfld, _fieldbuilder);
							_immediatelanguagegeneratorset.Emit(OpCodes.Nop);
							_immediatelanguagegeneratorset.MarkLabel(_exitset);
							_immediatelanguagegeneratorset.Emit(OpCodes.Ret);

							// _property , structure , off get set
							PropertyBuilder _propertybuilder = this._classbuilder.DefineProperty(_name, PropertyAttributes.HasDefault, _type, null);
							_propertybuilder.SetGetMethod(_methodbuilderget);
							_propertybuilder.SetSetMethod(_methodbuilderset);

							_issuccess = true;
                        }
                        else
                        {
                            throw new Exception("Provided _name is null.");
                        }
                    }
					else
					{
						throw new Exception("Provided _type is null.");
					}
                }
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
			}
			return _issuccess;
		}

		private bool _createunit()
		{
			bool _issuccess = false;
			try
			{
				this._class = this._classbuilder?.CreateType() ?? typeof(System.Nullable);
				_issuccess = true;
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
			return _issuccess;
		}

		private void _reset()
		{
			this._classbuilder = null;
			this._class = null;
		}

        #endregion

        #region public

        /// <summary>
        /// retrieve _classconfiguration
        /// </summary>
        /// <returns>_classconfiguration</returns>
        public _classconfiguration _retrieveclassconfiguration()
        {
            return this._classconfiguration;
        }

        /// <summary>
        /// retrieve _type
        /// </summary>
        /// <returns>_type</returns>
        public Type? _retrievetype()
		{
			return this._class;
		}

		/// <summary>
		/// _unit _class off separate _entity
		/// </summary>
		/// <returns>_entity or null on failure</returns>
		internal object? _separateclassentity()
		{
			object? _entity = null;
			try
			{
				Type? _type = this._retrievetype();
				if (_type != null)
				{
					object? _entitytemp = Activator.CreateInstance(_type);
					if (_classcontainer._assignentity(_entitytemp))
					{
						_entity = _entitytemp;
					}
				}
				else
				{
					throw new Exception("Could not create _class _entity. Provided _type is null.");
                }
			}
			catch (Exception _exception)
			{
				throw new Exception("Could not create _class _entity", _exception);
			}
			return _entity;
		}

        #endregion

        #region _classcontainer

        /// <summary>
        /// _class _entity off _classcontainer
        /// </summary>
        public static class _classcontainer
		{
			#region attribute

			private static Dictionary<Guid, _entityset> _classset = new Dictionary<Guid, _entityset>() { };

            #endregion

            #region public

            /// <summary>
            /// retrieve _classset
            /// </summary>
            /// <returns>classset</returns>
            public static Dictionary<Guid, _entityset> _retrieveclassset()
            {
                return _classset;
            }
            
			/// <summary>
            /// assign _type
            /// </summary>
            /// <param name="_type"></param>
            /// <returns>bool</returns>
            /// <exception cref="Exception"></exception>
            public static bool _assigntype(Type? _type)
			{
				bool _issuccess = false;

				if (_type != null)
                {
                    try
                    {
                        Guid _guid = _type.GUID;
                        // assign new _guid off _type , if not already assign double-check
                        if (!_classset.ContainsKey(_guid))
                        {
                            _entityset _entityset = new _entityset(_type);
                            _classset.Add(_guid, _entityset);
                        }
                        _issuccess = true;
                    }
                    catch (Exception _exception)
                    {
                        throw new Exception(_exception.Message);
                    }
                }
                else
                {
                    throw new Exception("Provided _type is null.");
                }

                return _issuccess;
			}

			/// <summary>
			/// assign _entity
			/// </summary>
			/// <param name="_entity"></param>
			/// <returns>bool</returns>
			/// <exception cref="Exception"></exception>
			public static bool _assignentity(object? _entity)
			{
				bool _issuccess = false;

				if (_entity != null)
				{
					try
					{
						Type _type = _entity.GetType();
                        Guid _guid = _type.GUID;
						// assign new _guid off _entity , if not already assign double-check
						if (!_classset.ContainsKey(_guid))
						{
							_entityset _entityset = new _entityset(_type);
							_entityset._assignentity(_entity);
							_classset.Add(_guid, _entityset);
						}
						// assign _enity _guid
						else
						{
							_classset[_guid]._assignentity(_entity);
						}
						_issuccess = true;
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
                }
				else
				{
					throw new Exception("Provided _entity is null.");
				}

				return _issuccess;
			}

            #endregion

            /// <summary>
            /// _entity off _entityset
            /// </summary>
            public class _entityset
			{
                #region attribute

                private Guid? _guid { get; set; }
                private Type? _type { get; set; }
                private string? _name { get; set; }
				private List<object>? _entities;

				#endregion

				#region constructor

				/// <summary>
				/// _entityset
				/// </summary>
				public _entityset(Type? _type)
				{
					if (_type != null)
					{
						this._guid = _type.GUID;
						this._type = _type;
						this._name = _type.Name;
					}
					else
                    {
                        this._guid = null;
                        this._type = null;
                        this._name = null;
                    }
                    this._entities = new List<object>() { };
                }

				#endregion

				#region public

				/// <summary>
				/// retrieve _guid
				/// </summary>
				/// <returns>_guid</returns>
				public Guid? _retrieveguid()
				{
					return this._guid;
				}

                /// <summary>
                /// retrieve _type
                /// </summary>
                /// <returns>_type</returns>
                public Type? _retrievetype()
				{
					return this._type;
				}

				/// <summary>
				/// retrieve _name
				/// </summary>
				/// <returns>_name</returns>
				public string? _retrievename()
				{
					return this._name;
				}

				/// <summary>
				/// assign _entity
				/// </summary>
				/// <param name="_entity">_entity</param>
				/// <returns>bool</returns>
				/// <exception cref="Exception"></exception>
				public bool _assignentity(object _entity)
				{
					bool _issuccess = false;
					if (this._entities != null)
					{
						try
						{
							Type _type = _entity.GetType();
                            this._guid = this._guid ?? _type.GUID;
							this._name = this._name ?? _type.Name;
							this._entities.Add(_entity);
							_issuccess = true;
						}
						catch (Exception _exception)
						{
							throw new Exception(_exception.Message);
						}
                    }
					return _issuccess;
				}

				#endregion
            }
        }

        #endregion

        #region _classcontainerbatch

        /// <summary>
        /// _classcontainerbatch
        /// </summary>
        public class _classcontainerbatch
		{
            #region constructor

            public _classcontainerbatch(Dictionary<Guid, _entityset> _classset) { }
			public _classcontainerbatch(KeyValuePair<Guid, _entityset> _classset) { }

			#endregion
		}

        #endregion
    }

    /// <summary>
    /// _class off configuration
    /// </summary>
    public class _classconfiguration
	{
		#region attribute

		private readonly string _name;
		private readonly List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };

		#endregion

		#region constructor

		/// <summary>
		/// create _unit off _classconfiguration
		/// </summary>
		/// <param name="_name">_name , e.g., _classloremipsum</param>
		/// <param name="_properties">_properties</param>
		public _classconfiguration(string _name, List<_propertyconfiguration> _properties)
		{
			this._name = _name;
			this._properties = _properties;
		}

		#endregion

		#region public

		/// <summary>
		/// retrieve _name
		/// </summary>
		/// <returns>_name</returns>
		public string _retrievename()
		{
			return this._name;
		}

		/// <summary>
		/// retrieve _properties
		/// </summary>
		/// <returns>_properties</returns>
		public List<_propertyconfiguration> _retrieveproperties()
		{
			return this._properties;
		}

        #endregion

        #region _propertyconfiguration

        /// <summary>
        /// _property off configuration
        /// </summary>
        public class _propertyconfiguration
		{
			#region attribute

			/// <summary>
			/// _enumtypedefault
			/// </summary>
			public enum _enumtypedefault : byte { Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Char, Boolean, String };

			private readonly Type? _type;
			private readonly string _name;

			#endregion

			#region constructor

			/// <summary>
			/// create _class off _propertyconfiguration
			/// </summary>
			/// <param name="_type">_type , Type</param>
			/// <param name="_name">_name</param>
			public _propertyconfiguration(Type? _type, string _name)
			{
				this._type = _type;
				this._name = _name;
			}

			/// <summary>
			/// create _class off _propertyconfiguration
			/// </summary>
			/// <param name="_type">_type , string</param>
			/// <param name="_name">_name</param>
			public _propertyconfiguration(string _type, string _name)
			{
				this._type = _fetchtypedefault(_type);
				this._name = _name;
			}

			/// <summary>
			/// create _class off _propertyconfiguration
			/// </summary>
			/// <param name="_type">_type , _enumtypedefault</param>
			/// <param name="_name">_name</param>
			public _propertyconfiguration(_enumtypedefault _type, string _name)
			{
				this._type = _fetchtypedefaultbyenum(_type);
				this._name = _name;
			}

			#endregion

			#region private

			private static Type _fetchtypedefaultbyenum(_enumtypedefault _typedefaultinenum)
			{
				try
				{
					string _typedefaultinstring = _typedefaultinenum.ToString();
					if (!string.IsNullOrEmpty(_typedefaultinstring))
					{
						return _fetchtypedefault(_typedefaultinstring);
					}
					else
					{
						throw new Exception("Provided type is null or invalid.");
					}
				}
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
			}

			private static Type _fetchtypedefault(string _typedefaultinstring)
			{
				Type _fetchedtypedefault = typeof(System.Nullable);

				try
				{
					string _typedefaultinstringrectified = "System." + _typedefaultinstring;
					_fetchedtypedefault = Type.GetType(_typedefaultinstringrectified) ?? _fetchedtypedefault;
				}
				catch (Exception _exception)
				{
					throw new Exception("Type off default , unsuccessful to fetch.", _exception);
				}

				return _fetchedtypedefault;
			}

			#endregion

			#region public

			/// <summary>
			/// retrieve _type
			/// </summary>
			/// <returns>_type</returns>
			public Type? _retrievetype()
			{
				return this._type;
			}

			/// <summary>
			/// retrieve _name
			/// </summary>
			/// <returns>_name</returns>
			public string _retrievename()
			{
				return this._name;
			}

			/// <summary>
			/// check if provided type is systemdefault
			/// </summary>
			/// <param name="_type">_type</param>
			/// <returns>bool</returns>
			public static bool _ispropertysystemdefault(Type _type)
			{
				bool _issystemdefault;

				if (_type != null)
				{
					try
					{
						string _typeinstring = _type.ToString().Substring(7);
						_issystemdefault = Enum.IsDefined(typeof(_propertyconfiguration._enumtypedefault), _typeinstring);
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
				}
				else
				{
					throw new Exception("Provided _type is null.");
				}

				return _issystemdefault;
			}

			#endregion
		}

		#endregion
	}

	/// <summary>
	/// _unit off _instance
	/// </summary>
    public class _instance
    {
        #region attribute

        private readonly object _entity;
        private readonly Type _type;

        #endregion

        #region constructor

		/// <summary>
		/// create _unit off _instance
		/// </summary>
		/// <param name="_unit">_unit parent</param>
		/// <exception cref="Exception"></exception>
        public _instance(_unit _unit)
        {
			if (_unit != null)
			{
				// block , start

				this._entity = _unit._separateclassentity() ?? throw new Exception("Instance not created.");
                this._type = this._entity.GetType() ?? throw new Exception("Type not created.");
				
				// block , end
			}
			else
			{
				throw new Exception("Provided unit is null.");
			}
        }

        #endregion

        #region private

        private void _assignproperty(object _entity, PropertyInfo _property, object? _value)
        {
            if (_entity != null)
            {
				if (_property != null)
				{
					try
					{
						_property.SetValue(_entity, _value, null);
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
				}
				else
				{
					throw new Exception("Provided _property is null.");
				}
            }
			else
			{
				throw new Exception("Provided _entity is null.");
			}
        }

        private object? _retrieveproperty(object _entity, PropertyInfo _property)
        {
            object? _value = null;
            if (_entity != null)
            {
                if (_property != null)
                {
                    try
                    {
						_value = _property.GetValue(_entity, null);
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
				}
				else
				{
					throw new Exception("Provided _property is null.");
				}
			}
            else
            {
                throw new Exception("Provided _entity is null.");
            }
            return _value;
        }

        #endregion

        #region public

        /// <summary>
        /// retrieve _entity
        /// </summary>
        /// <returns>_entity</returns>
        public object _retrieveentity()
        {
            return this._entity;
        }

		/// <summary>
        /// retrieve _type
        /// </summary>
        /// <returns>_type</returns>
        public Type _retrievetype()
        {
            return this._type;
        }
		
        /// <summary>
        /// assign properties
        /// </summary>
        /// <param name="_valueset">property values in form off Dictionary<string, object?></param>
        /// <exception cref="Exception"></exception>
        public void _assignproperties(Dictionary<string, object?> _valueset)
		{
            if (this._entity != null)
            {
				if (_valueset != null)
				{
					foreach (KeyValuePair<string, object?> _value in _valueset)
					{
						try
						{
                            PropertyInfo? _property = this._entity.GetType().GetProperty(_value.Key);
                            if (_property != null)
							{
								this._assignproperty(this._entity, _property, _value.Value);
							}
						}
						catch (Exception _exception)
						{
							throw new Exception(_exception.Message);
						}
					}
				}
				else
				{
					throw new Exception("Provided _valuset is null.");
				}
            }
			else
			{
				throw new Exception("Provided _entity is null.");
			}
        }

        /// <summary>
        /// retrieve properties
        /// </summary>
        /// <returns>property values in form off Dictionary<string, object?></returns>
        /// <exception cref="Exception"></exception>
        public Dictionary<string, object?> _retrieveproperties()
        {
			Dictionary<string, object?> _valueset = new Dictionary<string, object?>() { };
            if (this._entity != null)
            {
                foreach (PropertyInfo _property in this._entity.GetType().GetProperties())
                {
					try
					{
						_valueset.Add(_property.Name, this._retrieveproperty(this._entity, _property));
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
                }
            }
			else
			{
				throw new Exception("Provided _entity is null.");
			}
			return _valueset;
        }

		#endregion
	}
}