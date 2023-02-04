using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;
using System.Runtime.InteropServices;
using System.Text.Json;

using static _unit._typeconfiguration;
using static _unit._datacontainer;

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
						if (_typecontainer != null && _typecontainer._retrievetype() != null)
						{
							_typeparent = _typecontainer._retrievetype();
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
						_type = _datacontainer._unitcontainer._fetchtypecontainerbyhook(_hook ?? 0)?._retrievetype();
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
                new _typeraw() { _hook = 13, _name = "_st", _propertyset = new Dictionary<string, string>() { { "_service", "String" }, { "_api", "Boolean" } }, _typeparent = 2 }
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
                        if (this._ishookunique(_type._hook))
                        {
                            this._hook = _type._hook;
                        }
                        else
                        {
                            _issuccess = false;
                            throw new Exception("Provided _hook is not unique.");
                        }

						if (!string.IsNullOrEmpty((_type._name ?? string.Empty).Trim()))
						{
							this._name = _type._name ?? string.Empty;
                        }
                        else
                        {
                            _issuccess = false;
                            throw new Exception("Provided _name is null or empty.");
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
                                            throw new Exception("Provided _property _hook userdefined is not available.");
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
                                        throw new Exception("Provided _hook of typedefault is error prone.");
                                    }
								}
								else
								{
									_issuccess = false;
									break;
                                    throw new Exception("Provided _property _name is null or empty.");
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
                                throw new Exception("Provided _typeparent _hook userdefined is not available.");
                            }
						}
                    }
					else
					{
						_issuccess = false;
                        throw new Exception("Provided _hook is either invalid or not unique.");
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
                _typeform? _typeform = null;
                try
                {
                    _typeform = this._typeformset.Count > 0 ? this._typeformset.Where(_x => _x._hook == _hook).First() : null;
                }
                catch { }
                if (_typeform != null)
                {
                    _ishookexists = true;
                }
                return _ishookexists;
            }

            public bool _ishookunique(ulong _hook)
            {
                bool _isunique = true;
                KeyValuePair<ulong, _typecontainer>? _typecontainerset = null;
                try
                {
                    _typecontainerset = _datacontainer._unitcontainer._fetchtypecontainerset().Count > 0 ? _datacontainer._unitcontainer._fetchtypecontainerset().Where<KeyValuePair<ulong, _typecontainer>>(_x => _x.Key == _hook).First() : null;
                }
                catch { }
                if (_typecontainerset == null)
                {
                    _typeform? _typeform = null;
                    try
                    {
                        _typeform = this._typeformset.Count > 0 ? this._typeformset.Where(_x => _x._hook == _hook).First() : null;
                    }
                    catch { }
                    if (_typeform != null)
                    {
                        _isunique = false;
                    }
                }
                else
                {
                    _isunique = false;
                }
                return _isunique;
            }

            #endregion
        }

        #endregion
    }

    #endregion

    #region _instanceconfiguration

    public class _instanceconfiguration
    {
        #region attribute

        private string? _instancereal;
        private List<_instanceraw>? _instancerawset;
        private List<_instanceform> _instanceformset = new List<_instanceform>();

        #endregion

        #region constructor

        public _instanceconfiguration([Optional] string _instancereal)
        {
            if (this._isformjsonreal(_typereal))
            {
                List<_typeraw>? _typerawset = _instanceconfiguration._jsonareal(_instancereal);
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

        public _instanceconfiguration([Optional] List<_instanceraw> _instancerawset)
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

        public class _instanceraw
        {
            public ulong _hook;
            public ulong? _fence;
        }

        public class _instanceform
        {
            public _instanceform()
            {

            }
        }
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
		public ulong? _fence;

        #endregion

        #region constructor

        public _instance(ulong _hook, [Optional]ulong? _fence)
        {
			if (_hook > 0)
			{
				this._entity = _trycreateentity(_hook, _fence) ?? throw new Exception("_entity not created.");
                this._type = this._entity.GetType() ?? throw new Exception("_type not created.");
				this._fence = _fence;
			}
			else
			{
				throw new Exception("Provided _entity is null.");
			}
        }

        #endregion

        #region private

        private static object? _trycreateentity(ulong _hook, [Optional]ulong? _fence)
        {
            object? _createdentity = null;
            if (_hook > 0)
            {
                try
                {
                    _datacontainer._typecontainer? _typecontainer = _datacontainer._unitcontainer._fetchtypecontainerbyhook(_hook);
                    if (_typecontainer != null && _typecontainer._retrieveentitycontainer() != null)
                    {
                        Type? _type = _typecontainer._retrievetype();
                        if (_type != null)
                        {
                            object? _createdentitytemp = Activator.CreateInstance(_type);
                            if (_createdentitytemp != null)
                            {
                                if (_typecontainer._retrieveentitycontainer()._assignentity(_createdentitytemp, _fence))
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
                    object? _valuefence = null;
                    if (this._trygetnumericfenceandvalue(_property.PropertyType, _value, out _valuefence) != null)
                    {
                        try
                        {
                            _property.SetValue(_entity, _valuefence, null);
                        }
                        catch (Exception _exception)
                        {
                            throw new Exception(_exception.Message);
                        }
                    }
                    else
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

        public ulong? _trygetnumericfenceandvalue(Type _type, object? _value, out object? _valuefence)
        {
            ulong? _retrievedfence = null;
            _valuefence = null;
            try
            {
                string? _valueasstring = (_value ?? string.Empty).ToString();
                if (!string.IsNullOrEmpty(_valueasstring))
                {
                    bool _ispossiblefence = _valueasstring.All(_charset => "0123456789".Contains(_charset));
                    ulong _fencepossible = 0;
                    ulong.TryParse(_valueasstring, out _fencepossible);
                    if (_fencepossible > 0)
                    {
                        KeyValuePair<ulong, _datacontainer._typecontainer>? _typecontainerset = null;
                        try
                        {
                            _typecontainerset = _datacontainer._unitcontainer._fetchtypecontainerset().Where<KeyValuePair<ulong, _datacontainer._typecontainer>>(_x => _x.Value._retrievetype() == _type).First();
                        }
                        catch { }
                        if (_typecontainerset != null)
                        {
                            KeyValuePair<object, ulong?>? _valuefound = null;
                            try
                            {
                                _valuefound = _typecontainerset.Value.Value._retrieveentitycontainer()._entityset.Where<KeyValuePair<object, ulong?>>(_x => (_x.Value ?? 0) == _fencepossible).First();
                            }
                            catch { }
                            if (_valuefound != null)
                            {
                                _valuefence = _valuefound.Value.Key;
                                _retrievedfence = _fencepossible;
                            }
                        }
                    }
                }
            }
            catch (Exception _exception)
            {
                throw new Exception(_exception.Message);
            }
            return _retrievedfence;
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
            private static Dictionary<ulong, _typecontainer> _typecontainerse = new Dictionary<ulong, _typecontainer>() { };
            public static Dictionary<ulong, _typecontainer> _typecontainerset { get { return _typecontainerse; } }

            public static bool _assigntype(ulong _hook, _typeform _typeform, Type _type)
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
                                if (!_typecontainerse.ContainsKey(_hook))
                                {
                                    _typecontainerse.Add(_hook, new _typecontainer(_hook, _typeform, _type));
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
                    _typecontainerse.TryGetValue(_hook, out _typecontainer);
                }
                catch (Exception _exception)
                {
                    throw new Exception(_exception.Message);
                }
                return _typecontainer;
            }

            public static Dictionary<ulong, _typecontainer> _fetchtypecontainerset()
            {
                return _typecontainerse;
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
            private Type _type { get; set; }
            private ulong _hoo { get; set; }
            private _typeform _typefor { get; set; }
            private _entitycontainer _entitycontaine { get; set; }
            public ulong _hook { get { return _hoo; } }
            public _typeform _typeform { get { return _typefor; } }
            public _entitycontainer _entitycontainer { get { return _entitycontaine; } }

            public _typecontainer(ulong _hook, _typeform _typeform, Type _type)
            {
                if (_typeform != null)
                {
                    if (_type != null)
                    {
                        if (_typeform._hook == _hook)
                        {
                            this._hoo = _hook;
                            this._typefor = _typeform;
                            this._type = _type;
                            this._entitycontaine = new _entitycontainer();
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

			public Type _retrievetype()
			{
				return this._type;
			}

            public _entitycontainer _retrieveentitycontainer()
			{
				return this._entitycontaine;
			}
        }

        #endregion

        #region class _entitycontainer

        public class _entitycontainer
        {
            private List<KeyValuePair<object, ulong?>> _entityse { get; set; }
            public List<KeyValuePair<object, ulong?>> _entityset { get { return _entityse; } }

            public _entitycontainer()
            {
                this._entityse = new List<KeyValuePair<object, ulong?>>() { };
            }

            private bool _isfenceunique(ulong _fence)
            {
                bool _isunique = true;
                foreach (KeyValuePair<object, ulong?> _entity in this._entityse)
                {
                    if ((_entity.Value ?? 0) == _fence)
                    {
                        _isunique = false;
                        break;
                    }
                }
                return _isunique;
            }

            public bool _assignentity(object _entity, [Optional]ulong? _fence)
            {
                bool _issuccess = false;
                if (this._entityse != null)
                {
                    if ((_fence ?? 0) > 0)
                    {
                        if (!this._isfenceunique(_fence ?? 0))
                        {
                            throw new Exception("Provided _fence is not unique.");
                        }
                    }
                    try
                    {
                        this._entityse.Add(new KeyValuePair<object, ulong?>(_entity, _fence) { });
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