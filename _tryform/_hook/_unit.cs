﻿using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;
using System.Runtime.InteropServices;
using System.Text.Json;

using static _unit._typeconfiguration;

namespace _unit
{
	#region class _unit

	/// <summary>
	/// _unit
	/// </summary>
	public class _unit
	{
		#region attribute

		public _typeconfiguration? _typeconfiguration;
		private TypeBuilder? _typebuilder { get; set; }

        #endregion

        #region constructor

        public _unit(_typeconfiguration _typeconfigurations)
        {
            this._typeconfiguration = _typeconfigurations;
            if (!this._process())
            {
                throw new Exception("_unit is not processed.");
            }
        }

        #endregion

        #region private

        private bool _process()
		{
			bool _issuccess = true;

			if (this._typeconfiguration != null)
			{
				List<_typeform> _typeformset = this._typeconfiguration._retrievetypeformset();
				if (_typeformset != null)
				{
					foreach (_typeform _typeform in _typeformset)
					{
						if (_datacontainer._unitcontainer._fetchtypecontainerbyhook(_typeform._hook) == null)
						{
							this._resettypebuilder();
                            if (this._structure(_typeform))
							{
								Type? _createdtype = this._trycreatetype();
                                if (_createdtype != null)
                                {
									if (!_datacontainer._unitcontainer._assigntype(_typeform._hook, _typeform, _createdtype))
									{
										_issuccess = false;
										break;
									}
                                }
                            }
						}
                    }
                }
                else
                {
                    throw new Exception("Provided _typeformset is null.");
                }
            }
			else
			{
				throw new Exception("Provided _typeconfiguration is null.");
			}

			return _issuccess;
		}
		
		private bool _structure(_typeform _typeform)
		{
			bool _issuccess = true;
			// _unit off class
			if (this._structuretype(_typeform))
			{
				// _unit off constructor
				if (this._structuretypeconstructor())
				{
					// _unit off propertieset
					foreach (KeyValuePair<string, string> _property in _typeform._propertyset)
					{
						_issuccess = this._structuretypeproperty(_typeform, _property);
						if (!_issuccess)
						{
							break;
						}
                    }
				}
			}
			return _issuccess;
		}

		private bool _structuretype(_typeform _typeform)
		{
			bool _issuccess = false;
			if (_typeform != null)
			{
				try
				{
					// _assembly , name
					AssemblyName _assemblyname = new AssemblyName(_typeform._name);

					// _assembly , structure
					AssemblyBuilder _assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.RunAndCollect);

					// _module , structure
					ModuleBuilder _modulebuilder = _assemblybuilder.DefineDynamicModule(_typeform._name);

					// _class , structure
					Type? _typeparent = null;
					if (_typeform._typeparent > 0)
					{
                        _datacontainer._typecontainer? _typecontainer = _datacontainer._unitcontainer._fetchtypecontainerbyhook(_typeform._typeparent);
						if (_typecontainer != null && _typecontainer._type != null)
						{
							_typeparent = _typecontainer._type;
                        }
						else
						{
							throw new Exception("Either respective _typecontainer or _type respective is null.");
						}
					}
					this._typebuilder = _modulebuilder.DefineType(_assemblyname.FullName,
						TypeAttributes.Public |
						TypeAttributes.Class |
						TypeAttributes.AutoClass |
						TypeAttributes.AnsiClass |
						TypeAttributes.BeforeFieldInit |
						TypeAttributes.AutoLayout |
						TypeAttributes.Serializable,
                        _typeparent
                    );
					_issuccess = true;
				}
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
			}
			else
			{
				throw new Exception("Provided _typeform is null.");
			}
			return _issuccess;
		}

		private bool _structuretypeconstructor()
		{
			bool _issuccess = false;
			if (this._typebuilder != null)
			{
				try
				{
					// _class constructor , structure
					this._typebuilder?.DefineDefaultConstructor(
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

		private bool _structuretypeproperty(_typeform _typeform, KeyValuePair<string, string> _property)
		{
			bool _issuccess = false;
			if (this._typebuilder != null)
			{
				try
				{
					string _name = _property.Key;
					Type? _type = null;

					ulong? _hook = _typeform._trygetnumerichook(_property.Value);
					if (_hook != null)
					{
						_type = _datacontainer._unitcontainer._fetchtypecontainerbyhook(_hook ?? 0)?._type;
					}
					else
					{
						_type = _typeform._trygettypedefault(_property.Value);
					}

					if (_type != null)
					{
						if (!string.IsNullOrEmpty(_name))
						{
							// _property _field , structure
							FieldBuilder _fieldbuilder = this._typebuilder.DefineField("_field" + _name, _type, FieldAttributes.Private);

							// _property _method , structure , get
							MethodBuilder _methodbuilderget = this._typebuilder.DefineMethod("_get" + _name,
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
							MethodBuilder _methodbuilderset = this._typebuilder.DefineMethod("_set" + _name,
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
							PropertyBuilder _propertybuilder = this._typebuilder.DefineProperty(_name, PropertyAttributes.HasDefault, _type, null);
							_propertybuilder.SetGetMethod(_methodbuilderget);
							_propertybuilder.SetSetMethod(_methodbuilderset);

							_issuccess = true;
						}
						else
						{
							throw new Exception("Provided _name is null or empty.");
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

		private Type? _trycreatetype()
		{
			Type? _createdtype = null;
			try
			{
                _createdtype = this._typebuilder?.CreateType();
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
			return _createdtype;
		}

		private void _resettypebuilder()
		{
			this._typebuilder = null;
		}

		#endregion
    }

    #endregion

    #region class _typeconfiguration

    /// <summary>
    /// _unit off _typeconfiguration
    /// </summary>
    public class _typeconfiguration
    {
        #region attribute

        private string? _typereal;
        private List<_typeraw>? _typerawset;
		private List<_typeform> _typeformset = new List<_typeform>();

        public enum _typedefaultenum : byte { Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Char, Boolean, String };

        #endregion

        #region constructor

        public _typeconfiguration([Optional]string _typereal)
		{
			if (this._isformjsonreal(_typereal))
			{
				List<_typeraw>? _typerawset = _typeconfiguration._jsonareal(_typereal);
                if (this._process(_typerawset))
				{
					this._typerawset = _typerawset;
					this._typereal = _typereal;
                }
				else
				{
					throw new Exception("Unable to process _typeconfiguration.");
				}
            }
            else
            {
                throw new Exception("_typereal is not in form.");
            }
        }

		public _typeconfiguration([Optional]List<_typeraw> _typerawset)
		{
			if (_typerawset != null)
			{
				if (this._process(_typerawset))
				{
					this._typerawset = _typerawset;
					this._typereal = _typeconfiguration._jsonreal(_typerawset);
                }
                else
                {
                    throw new Exception("Unable to process _typeconfigurations.");
                }
            }
            else
            {
                throw new Exception("Provided _types is null.");
            }
        }

        #endregion

        #region private

        private bool _process(List<_typeraw>? _typerawset)
        {
            bool _issuccess = false;
            if (_typerawset != null)
            {
                _issuccess = this._feedtypeformset(_typerawset);
            }
            else
            {
                throw new Exception("_types are null.");
            }
            return _issuccess;
        }

		private bool _feedtypeformset(List<_typeraw> _typerawset)
		{
			bool _issuccess = false;
			try
			{
				foreach (_typeraw _type in _typerawset)
				{
					this._typeformset.Add(new _typeform(this._typeformset, _type));
				}
				_issuccess = true;
			}
			catch (Exception _exception)
			{
				throw new Exception("_type not added as _typeform", _exception);
			}
            return _issuccess;
		}

        private bool _isformjsonreal(string _jsonreal)
		{
			bool _isform = false;

			List<_typeraw>? _jsonareal = _typeconfiguration._jsonareal(_jsonreal);
			if (_jsonareal != null)
			{
				string? _triedjsonreal = _typeconfiguration._jsonreal(_jsonareal);
				if (_triedjsonreal != null)
				{
					if (_jsonreal.Equals(_triedjsonreal))
					{
						_isform = true;
					}
				}
			}

			return _isform;
		}

        #endregion

        #region public

		public List<_typeform> _retrievetypeformset()
		{
			return this._typeformset;
		}

        public bool _checktypesreal(string _typereal)
		{
			bool _isform = false;

			_isform = this._isformjsonreal(_typereal);

            return _isform;
        }

		public static string? _jsonreal(List<_typeraw> _typerawset)
		{
			string? _jsonreal = null;
            JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true };
			try
			{
                _jsonreal = JsonSerializer.Serialize<List<_typeraw>>(_typerawset, _options);
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
			return _jsonreal;
        }

		public static List<_typeraw>? _jsonareal(string _typerawset)
		{
			List<_typeraw>? _jsonareal = null;
            JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true };
			try
			{
                _jsonareal = JsonSerializer.Deserialize<List<_typeraw>>(_typerawset, _options)!;
            }
			catch (Exception _exception)
			{
                throw new Exception(_exception.Message);
            }
			return _jsonareal;
        }

        public static List<_typeraw> _fetchsampletyperawset()
		{
            List<_typeraw> _sampletyperawset = new List<_typeraw>() {
                new _typeraw() { _hook = 1, _name = "_xy", _propertyset = new Dictionary<string, string>() { { "_id", "Int32" }, { "_fullname", "String" }, { "_isdead", "Boolean" } } },
                new _typeraw() { _hook = 2, _name = "_pq", _propertyset = new Dictionary<string, string>() { { "_xy", "1" }, { "_tag", "String" } } },
                new _typeraw() { _hook = 3, _name = "_st", _propertyset = new Dictionary<string, string>() { { "_service", "String" }, { "_api", "Boolean" } }, _typeparent = 2 }
            };
			return _sampletyperawset;
        }

        public static bool _istypedefault(string _type)
        {
            bool _typedefault;

            if (!string.IsNullOrEmpty(_type))
            {
                try
                {
                    _typedefault = Enum.IsDefined(typeof(_typeconfiguration._typedefaultenum), _type);
                }
                catch (Exception _exception)
                {
                    throw new Exception(_exception.Message);
                }
            }
            else
            {
                throw new Exception("Provided _type is null or empty.");
            }

            return _typedefault;
        }

        #endregion

        #region class _typeraw

        public class _typeraw
		{
            #region attribute

            public ulong _hook;
			public string? _name;
			public Dictionary<string, string>? _propertyset;
			public ulong? _typeparent;

			#endregion
		}

		#endregion

		#region class _typeform

		public class _typeform
		{
            #region attribute

            private List<_typeform> _typeformset;
            public ulong _hook;
            public string _name;
            public Dictionary<string, string> _propertyset;
            public ulong _typeparent;

            #endregion

            #region constructor

            public _typeform(List<_typeform> _typeformset, _typeraw _type)
			{
				this._typeformset = _typeformset;
                this._hook = 0;
				this._name = string.Empty;
				this._propertyset = new Dictionary<string, string>() { };
				this._typeparent = 0;
				if (!this._process(_type))
				{
					throw new Exception("Provided _type is unsuccessfull processed.");
				}
			}

            #endregion

            #region private

            private bool _process(_typeraw _type)
			{
				bool _issuccess = true;

				if (_type != null)
				{
					if (_type._hook > 0 && _datacontainer._unitcontainer._fetchtypecontainerbyhook(_type._hook) == null)
					{
						this._hook = _type._hook;

						if (!string.IsNullOrEmpty((_type._name ?? string.Empty).Trim()))
						{
							this._name = _type._name ?? string.Empty;
                        }
                        else
                        {
                            _issuccess = false;
                        }

						if (_type._propertyset != null)
						{
							foreach (KeyValuePair<string, string> _property in _type._propertyset)
							{
								if (!string.IsNullOrEmpty(_property.Key.Trim()))
								{
									ulong? _hook = this._trygetnumerichook(_property.Value);
									if (_hook != null)
									{
                                        if (_hook > 0 && (_datacontainer._unitcontainer._fetchtypecontainerbyhook(_hook ?? 0) != null || this._ishookexistslocal(_hook ?? 0)))
										{
											this._propertyset.Add(_property.Key, _property.Value);
										}
										else
										{
											_issuccess = false;
											break;
										}
									}
									else if (!string.IsNullOrEmpty(_property.Value.Trim()) && _typeconfiguration._istypedefault(_property.Value))
									{
										this._propertyset.Add(_property.Key, _property.Value);
                                    }
                                    else
                                    {
                                        _issuccess = false;
										break;
                                    }
								}
								else
								{
									_issuccess = false;
									break;
								}
                            }
                        }

						if (_type._typeparent != null)
						{
							if (_type._typeparent > 0 && (_datacontainer._unitcontainer._fetchtypecontainerbyhook(_type._typeparent ?? 0) != null || this._ishookexistslocal(_type._typeparent ?? 0)))
							{
								this._typeparent = _type._typeparent ?? 0;
							}
							else
							{
								_issuccess = false;
							}
						}
                    }
					else
					{
						_issuccess = false;
					}
				}

				return _issuccess;
			}

            #endregion

            #region public

            public ulong? _trygetnumerichook(string _type)
			{
				ulong? _retrievedhook = null;
				if (!string.IsNullOrEmpty(_type))
				{
					try
					{
						bool _ishook = _type.All(_charset => "0123456789".Contains(_charset));
						if (_ishook)
						{
							ulong _hook = 0;
							ulong.TryParse(_type, out _hook);
							if (_hook > 0)
							{
								_retrievedhook = _hook;
							}
						}
                    }
                    catch (Exception _exception)
                    {
                        throw new Exception(_exception.Message);
                    }
                }
				return _retrievedhook;
            }

            public Type? _trygettypedefault(string _typedefaultinstring)
            {
                Type? _retrievedtypedefault = null;

                try
                {
                    _retrievedtypedefault = Type.GetType("System." + _typedefaultinstring);
                }
                catch (Exception _exception)
                {
                    throw new Exception("Type off default , unsuccessful to retrieve.", _exception);
                }

                return _retrievedtypedefault;
            }

            public bool _ishookexistslocal(ulong _hook)
            {
                bool _ishookexists = false;
                foreach (_typeform _typeform in this._typeformset)
                {
                    if (_typeform._hook == _hook)
                    {
                        _ishookexists = true;
                        break;
                    }
                }
                return _ishookexists;
            }

			#endregion
		}

		#endregion
	}

    #endregion

    #region class _instance

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

        public _instance(ulong _hook)
        {
			if (_hook > 0)
			{
				this._entity = _trycreateentity(_hook) ?? throw new Exception("_entity not created.");
                this._type = this._entity.GetType() ?? throw new Exception("_type not created.");
			}
			else
			{
				throw new Exception("Provided _entity is null.");
			}
        }

        #endregion

        #region private

        private static object? _trycreateentity(ulong _hook)
        {
            object? _createdentity = null;
            if (_hook > 0)
            {
                try
                {
                    _datacontainer._typecontainer? _typecontainer = _datacontainer._unitcontainer._fetchtypecontainerbyhook(_hook);
                    if (_typecontainer != null && _typecontainer._entitycontainer != null)
                    {
                        Type? _type = _typecontainer._type;
                        if (_type != null)
                        {
                            object? _createdentitytemp = Activator.CreateInstance(_type);
                            if (_createdentitytemp != null)
                            {
                                if (_typecontainer._entitycontainer._assignentity(_createdentitytemp))
                                {
                                    _createdentity = _createdentitytemp;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Could not create _entity. Provided _type is null.");
                        }
                    }
                    else
                    {
                        throw new Exception("Could not create _entity. Provided _typecontainer or _entitycontainer is null.");
                    }
                }
                catch (Exception _exception)
                {
                    throw new Exception("Could not create _entity", _exception);
                }
            }
            return _createdentity;
        }

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

    #endregion

    #region class _datacontainer

    /// <summary>
    /// _datacontainer
    /// </summary>
    public class _datacontainer
    {
        #region class _unitcontainer

        public static class _unitcontainer
        {
            public static Dictionary<ulong, _typecontainer> _typecontainerset = new Dictionary<ulong, _typecontainer>() { };

            public static bool _assigntype(ulong _hook, _typeconfiguration._typeform _typeform, Type _type)
            {
                bool _issuccess = false;

                if (_typeform != null)
                {
                    if (_type != null)
                    {
                        if (_typeform._hook == _hook)
                        {
                            try
                            {
                                if (!_typecontainerset.ContainsKey(_hook))
                                {
                                    _typecontainerset.Add(_hook, new _typecontainer(_hook, _typeform, _type));
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
                            throw new Exception("Provided _typehook and _typeform _typehook is missmatch.");
                        }
                    }
                    else
                    {
                        throw new Exception("Provided _type is null.");
                    }
                }
                else
                {
                    throw new Exception("Provided _typeform is null.");
                }

                return _issuccess;
            }

            public static _typecontainer? _fetchtypecontainerbyhook(ulong _hook)
            {
                _typecontainer? _typecontainer = null;
                try
                {
                    _typecontainerset.TryGetValue(_hook, out _typecontainer);
                }
                catch (Exception _exception)
                {
                    throw new Exception(_exception.Message);
                }
                return _typecontainer;
            }

            public static Dictionary<ulong, _typecontainer> _fetchtypecontainerset()
            {
                return _typecontainerset;
            }

            public static string? _datareal(bool _writeindented, [Optional]ulong _hook)
            {
                string? _real = null;

                try
                {
                    JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true, WriteIndented = _writeindented };
                    if (_hook > 0)
                    {
                        _real = JsonSerializer.Serialize<_typecontainer?>(_fetchtypecontainerbyhook(_hook), _options);
                    }
                    else
                    {
                        _real = JsonSerializer.Serialize<Dictionary<ulong, _typecontainer>>(_fetchtypecontainerset(), _options);
                    }
                }
                catch (Exception _exception)
                {
                    throw new Exception("json serialize inevitable.", _exception);
                }

                return _real;
            }
        }

        #endregion

        #region class _typecontainer

        public class _typecontainer
        {
            public ulong _hook { get; set; }
            public _typeform _typeform { get; set; }
            public Type _type { get; set; }
            public _entitycontainer _entitycontainer { get; set; }

            public _typecontainer(ulong _hook, _typeform _typeform, Type _type)
            {
                if (_typeform != null)
                {
                    if (_type != null)
                    {
                        if (_typeform._hook == _hook)
                        {
                            this._hook = _hook;
                            this._typeform = _typeform;
                            this._type = _type;
                            this._entitycontainer = new _entitycontainer(_hook);
                        }
                        else
                        {
                            throw new Exception("Provided _typehook and _typeform _typehook is missmatch.");
                        }
                    }
                    else
                    {
                        throw new Exception("Provided _type is null.");
                    }
                }
                else
                {
                    throw new Exception("Provided _typeform is null.");
                }
            }
        }

        #endregion

        #region class _entitycontainer

        public class _entitycontainer
        {
            public ulong _hook { get; set; }
            public List<object> _entityset = new List<object>() { };

            public _entitycontainer(ulong _hook)
            {
                this._hook = _hook;
            }

            public bool _assignentity(object _entity)
            {
                bool _issuccess = false;
                if (this._entityset != null)
                {
                    try
                    {
                        this._entityset.Add(_entity);
                        _issuccess = true;
                    }
                    catch (Exception _exception)
                    {
                        throw new Exception("_entity not assigned.", _exception);
                    }
                }
                return _issuccess;
            }
        }

        #endregion
    }

    #endregion
}