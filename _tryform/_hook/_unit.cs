using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;
using System.Runtime.InteropServices;
using System.Text.Json;

using static _unit._unit._classcontainer;
using static _unit._classconfiguration._propertyconfiguration;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using static _unit._typeconfigurations;

namespace _unit
{
	#region class _unit

	/// <summary>
	/// _unit
	/// </summary>
	public class _unit
	{
		#region attribute

		private ulong _typehook;

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
						this._typehook = _unit._classidend._getidend();

						if (_classcontainer._assigntype(this._typehook, this._retrievetype()))
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
					TypeAttributes.AutoLayout |
					TypeAttributes.Serializable,
					this._classconfiguration._retrievetypeparent()
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
			this._typehook = default(UInt32);
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
					if (_classcontainer._assignentity(this._typehook, _entitytemp))
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

		#region class _classidend

		public static class _classidend
		{
			#region attribute

			private static UInt32 _idend = 0;

			#endregion

			#region public

			/// <summary>
			/// get _idend
			/// </summary>
			/// <returns>_idend</returns>
			public static UInt32 _getidend()
			{
				return ++_idend;
			}

			#endregion
		}

		#endregion

		#region class _classcontainer

		/// <summary>
		/// _class _entity off _classcontainer
		/// </summary>
		public static class _classcontainer
		{
			#region attribute

			private static Dictionary<ulong, _entityset> _classset = new Dictionary<ulong, _entityset>() { };

            #endregion

            #region public

            /// <summary>
            /// retrieve _classset
            /// </summary>
            /// <returns>classset</returns>
            public static Dictionary<ulong, _entityset> _retrieveclassset()
            {
                return _classset;
            }
            
			/// <summary>
            /// assign _type
            /// </summary>
            /// <param name="_type"></param>
            /// <returns>bool</returns>
            /// <exception cref="Exception"></exception>
            public static bool _assigntype(ulong _classidthis, Type? _type)
			{
				bool _issuccess = false;

				if (_type != null)
                {
                    try
                    {
                        if (!_classset.ContainsKey(_classidthis))
                        {
                            _entityset _entityset = new _entityset(_type);
                            _classset.Add(_classidthis, _entityset);
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
            /// <param name="_classidthis">_classidthis</param>
            /// <param name="_entity">_entity</param>
            /// <returns>bool</returns>
            /// <exception cref="Exception"></exception>
            public static bool _assignentity(ulong _classidthis, object? _entity)
			{
				bool _issuccess = false;

				if (_entity != null)
				{
					try
					{
						Type _type = _entity.GetType();
						if (!_classset.ContainsKey(_classidthis))
						{
							_entityset _entityset = new _entityset(_type);
							_entityset._assignentity(_entity);
							_classset.Add(_classidthis, _entityset);
						}
						else
						{
							_classset[_classidthis]._assignentity(_entity);
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

            /// <summary>
            /// fetch _classid by _classname
            /// </summary>
            /// <param name="_classname">_classname</param>
            /// <returns>_classid</returns>
            public static ulong? _fetchclassidbyname(string _classname)
			{
				UInt32? _classid = null;

				// TODO: fetch _classid from _unit._classcontainer._retrieveclassset().Values.Where<_classcontainer._entityset>();

                return _classid;
			}

            #endregion

            #region class _entityset

            /// <summary>
            /// _entity off _entityset
            /// </summary>
            public class _entityset
			{
                #region attribute

                private Type? _typ { get; set; }
                private string? _nam { get; set; }
				private List<object>? _entitie;

                public string? _name { get { return this._nam; } }
                public List<object>? _entities { get { return this._entitie; } }

                #endregion

                #region constructor

                /// <summary>
                /// _entityset
                /// </summary>
                /// <param name="_type">_type</param>
                /// <exception cref="Exception"></exception>
                public _entityset(Type? _type)
				{
					if (_type != null)
					{
						this._typ = _type;
						this._nam = _type.Name;
                        this._entitie = new List<object>() { };
                    }
					else
                    {
						throw new Exception("Provided _type is null.");
                    }
                }

				#endregion

				#region public

                /// <summary>
                /// retrieve _type
                /// </summary>
                /// <returns>_type</returns>
                public Type? _retrievetype()
				{
					return this._typ;
				}

				/// <summary>
				/// retrieve _name
				/// </summary>
				/// <returns>_name</returns>
				public string? _retrievename()
				{
					return this._nam;
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
					if (this._entitie != null)
					{
						try
						{
                            this._entitie.Add(_entity);
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

			#endregion
		}

        #endregion

        #region class _classcontainerbatch

        /// <summary>
        /// _classcontainerbatch
        /// </summary>
        public class _classcontainerbatch
        {
            #region attribute

            private Dictionary<ulong, _entityset> _classset = new Dictionary<ulong, _entityset>() { };

            #endregion

            #region constructor

            public _classcontainerbatch()
            {
                this._classset = _classcontainer._retrieveclassset();
            }
            public _classcontainerbatch(Dictionary<ulong, _entityset> _classset)
            {
                this._classset = _classset;
            }
            public _classcontainerbatch(ulong _classid, _entityset _entityset)
            {
                this._classset.Add(_classid, _entityset);
            }

            #endregion

            #region public

            /// <summary>
            /// serialize real json
            /// </summary>
            /// <param name="_writeindented">_writeindented</param>
            /// <returns>json</returns>
            /// <exception cref="Exception"></exception>
            public string? _jsonreal(bool _writeindented = true)
            {
                string? _json = null;

                try
                {
                    JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true, WriteIndented = _writeindented };
                    _json = JsonSerializer.Serialize<Dictionary<ulong, _entityset>>(this._classset, _options);
                }
                catch (Exception _exception)
                {
                    throw new Exception("json serialize inevitable.", _exception);
                }

                return _json;
            }

            #endregion
        }

		#endregion

		#region class _datacontainer

		public class _datacontainer
		{
            #region class _unitcontainer

            public static class _unitcontainer
			{
				public static Dictionary<ulong, _typecontainer> _typecontainerset = new Dictionary<ulong, _typecontainer>() { };

                public static bool _assigntype(ulong _typehook, _typeconfigurations._typeform _typeform)
                {
                    bool _issuccess = false;

					if (_typeform != null)
					{
						if (_typeform._hook == _typehook)
						{
							try
							{
								if (!_typecontainerset.ContainsKey(_typehook))
								{
									_typecontainerset.Add(_typehook, new _typecontainer(_typehook, _typeform));
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
						throw new Exception("Provided _typeform is null.");
					}

                    return _issuccess;
                }

				public static _typecontainer? _fetchtypecontainerbytypehook(ulong _typehook)
				{
					_typecontainer? _typecontainer = null;
					try
					{
						_typecontainer = _typecontainerset[_typehook];
					}
					catch (Exception _exception)
					{
						throw new Exception(_exception.Message);
					}
                    return _typecontainer;
                }
            }

			#endregion

			#region class _typecontainer

			public class _typecontainer
			{
				public ulong _typehook { get; set; }
				public _typeconfigurations._typeform _typeform { get; set; }
				public _entitycontainer _entitycontainer { get; set; }

				public _typecontainer(ulong _typehook, _typeconfigurations._typeform _typeform)
				{
					if (_typeform != null)
					{
						if (_typeform._hook == _typehook)
						{
							this._typehook = _typehook;
							this._typeform = _typeform;
							this._entitycontainer = new _entitycontainer(_typehook);
                        }
                        else
                        {
                            throw new Exception("Provided _typehook and _typeform _typehook is missmatch.");
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
				public ulong _typehook { get; set; }
				public List<object> _entityset = new List<object>() { };

				public _entitycontainer(ulong _typehook)
				{
					this._typehook = _typehook;
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

    #endregion

    #region class _classconfiguration

    /// <summary>
    /// _class off configuration
    /// </summary>
    public class _classconfiguration
	{
		#region attribute

		private readonly string _name;
		private readonly List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };
		private readonly Type? _typeparent;

        #endregion

        #region constructor

        /// <summary>
		/// create _unit off _classconfiguration
		/// </summary>
		/// <param name="_name">_name , e.g., _classloremipsum</param>
		/// <param name="_properties">_properties</param>
		/// <param name="_typeparent">_typeparent</param>
		///
        public _classconfiguration(string _name, List<_propertyconfiguration> _properties, [Optional]Type? _typeparent)
		{
			this._name = _name;
			this._properties = _properties;
			this._typeparent = _typeparent;
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

        /// <summary>
        /// _type off parent
        /// </summary>
        /// <returns>_typeparent</returns>
        public Type? _retrievetypeparent()
		{
			return this._typeparent;
		}

        #endregion

        #region class _propertyconfiguration

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

    #endregion

    #region class _typeconfigurations

    public class _typeconfigurations
    {
        #region attribute

		private List<_type>? _types;
        private string? _typesreal;
		private List<_typeform> _typeforms = new List<_typeform>();

        public enum _typedefaulttypeoptions : byte { Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Char, Boolean, String };

        #endregion

        #region constructor

        public _typeconfigurations([Optional]string _typesreal)
		{
			if (this._isformjsonreal(_typesreal))
			{
				List<_type>? _typesareal = _typeconfigurations._jsonareal(_typesreal);
                if (this._process(_typesareal))
				{
					this._types = _typesareal;
					this._typesreal = _typesreal;
                }
            }
		}

		public _typeconfigurations([Optional]List<_type> _types)
		{
			if (_types != null)
			{
				if (this._process(_types))
				{
					this._types = _types;
					this._typesreal = _typeconfigurations._jsonreal(_types);
                }
            }
		}

        #endregion

        #region private

        private bool _process(List<_type>? _types)
        {
            bool _issuccess = false;

            if (_types != null)
            {
                _issuccess = this._feedtypeforms();
            }
            else
            {
                throw new Exception("_types referred is null.");
            }

            return _issuccess;
        }

		private bool _feedtypeforms()
		{
			bool _issuccess = false;

			if (this._types != null)
			{
				try
				{
					foreach (_type _type in this._types)
					{
						this._typeforms.Add(new _typeform(_type));
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
				throw new Exception("_types are null.");
			}

            return _issuccess;
		}

        private bool _isformjsonreal(string _jsonreal)
		{
			bool _isform = false;

			List<_type>? _jsonareal = _typeconfigurations._jsonareal(_jsonreal);
			if (_jsonareal != null)
			{
				string? _triedjsonreal = _typeconfigurations._jsonreal(_jsonareal);
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

		public List<_typeform> _retrievetypeforms()
		{
			return this._typeforms;
		}

        public bool _checktypesreal(string _typesreal)
		{
			bool _isform = false;

			_isform = this._isformjsonreal(_typesreal);

            return _isform;
        }

		public static string? _jsonreal(List<_type> _types)
		{
			string? _jsonreal = null;
            JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true };
			try
			{
                _jsonreal = JsonSerializer.Serialize<List<_type>>(_types, _options);
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
			return _jsonreal;
        }

		public static List<_type>? _jsonareal(string _types)
		{
			List<_type>? _jsonareal = null;
            JsonSerializerOptions _options = new JsonSerializerOptions() { IncludeFields = true };
			try
			{
                _jsonareal = JsonSerializer.Deserialize<List<_type>>(_types, _options)!;
            }
			catch (Exception _exception)
			{
                throw new Exception(_exception.Message);
            }
			return _jsonareal;
        }

		public static List<_type> _fetchsampletypes()
		{
            List<_type> _sampletypes = new List<_type>() {
                new _type() { _hook = 1, _name = "_xy", _properties = new Dictionary<string, string>() { { "_id", "Int32" }, { "_fullname", "String" }, { "_isdead", "Boolean" } } },
                new _type() { _hook = 2, _name = "_pq", _properties = new Dictionary<string, string>() { { "_xy", "1" }, { "_tag", "String" } } },
                new _type() { _hook = 3, _name = "_st", _properties = new Dictionary<string, string>() { { "_service", "String" }, { "_api", "Boolean" } }, _typeparent = 2 }
            };
			return _sampletypes;
        }

        #endregion

        #region class _type

        public class _type
		{
			public ulong _hook;
			public string? _name;
			public Dictionary<string, string>? _properties;
			public ulong _typeparent;
        }

		#endregion

		#region class _typeform

		public class _typeform
		{
            public Type? _type;

            public ulong _hook;
            public string? _name;
            public Dictionary<string, string>? _properties;
            public ulong _typeparent;
			            
			public _typeform(_type _type)
			{
				if (!this._process(_type))
				{
					throw new Exception("Provided _type is not processed.");
				}
			}

            public _typeform(Type _type, string _name)
            {
				if (_type == null)
				{
					throw new Exception("Provided _type is null.");
				}
				else if (string.IsNullOrEmpty(_name))
				{
					throw new Exception("Provided _name is null or empty.");
				}
				else
				{
					this._type = _type;
					this._name = _name;
				}
            }

            public _typeform(string _type, string _name)
            {
                //this._type = _fetchtypedefault(_type);
                this._name = _name;
            }

            public _typeform(_typeconfigurations._typedefaulttypeoptions _type, string _name)
            {
                //this._type = _fetchtypedefaultbyenum(_type);
                this._name = _name;
            }

			private bool _process(_type _type)
			{
				bool _issuccess = false;

				if (_type != null)
				{
					// TODO: Cross check availability or repeat against _classcontainer
					this._hook = _type._hook;
					this._name = _type._name;
					this._typeparent = _type._typeparent;
					
					this._properties = _type._properties;
				}

				return _issuccess;
			}
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

	#endregion
}