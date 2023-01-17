using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;

namespace _unit
{
	public class _unit
	{
		#region attribute

		private readonly _unitconfiguration? _configuration;
		private TypeBuilder? _type { get; set; }

		#endregion

		#region constructor

		public _unit() { }

		/// <summary>
		/// construct this unit
		/// </summary>
		/// <param name="_configuration">this unit configuration</param>
		public _unit(_unitconfiguration _configuration)
		{
			this._configuration = _configuration;

			if (this._checkconfiguration())
			{
				this._proceed();
			}
		}

		#endregion

		#region private

		/// <summary>
		/// check configuration
		/// </summary>
		private bool _checkconfiguration()
		{
			// check if configuration file not null
			if (this._configuration != null)
			{
				// check if name of this unit not null
				if (!string.IsNullOrEmpty(this._configuration._unitname))
				{
					bool _ispropertiescompliant = true;

					// check if properties of this unit not null
					if (this._configuration._properties != null)
					{
						// check if properties of this unit are compliant
						foreach (_propertyconfiguration _property in this._configuration._properties)
						{
							if (
								String.IsNullOrEmpty(_property._name) ||
								String.IsNullOrEmpty(_property._type.ToString())
							)
							{
								_ispropertiescompliant = false;
							}
						}

                        // return configuration checked as valid
                        if (_ispropertiescompliant)
						{
							return true;
						}
					}
					else if (!_ispropertiescompliant)
					{
						throw new Exception("Property(s) of this unit is/are not compliant in provided unit configuration file.");
					}
					else
					{
						throw new Exception("Property(s) of this unit returned null in provided unit configuration file.");
					}
				}
				else
				{
					throw new Exception("Name of this unit returned null or empty in provided unit configuration file.");
				}
			}
			else
			{
				throw new Exception("Unit configuration file not provided at startup.");
			}
			return false;
		}

		/// <summary>
		/// proceed to create this unit , detail
		/// </summary>
		private void _proceed()
		{
            // define startup structure of this unit
            this._defineunit();

            // define this unit off constructor
            this._defineunitconstructor();
			
			// define this unit off properties
            foreach (_propertyconfiguration _property in this._configuration._properties)
            {
                this._defineunitproperty(_property._type, _property._name); // TODO: instead , pass object off _propertyconfiguration
            }
        }

		/// <summary>
		/// carete this unit off assembly definition
		/// </summary>
		private void _defineunit()
		{
			AssemblyName _assemblyname = new AssemblyName(this._configuration._unitname);
			AssemblyBuilder _assembly = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.RunAndCollect);

			ModuleBuilder _unit = _assembly.DefineDynamicModule(this._configuration._unitname);
			this._type = _unit.DefineType(_assemblyname.FullName,
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoLayout,
				null
			);
		}

		/// <summary>
		/// create this unit off constructor
		/// </summary>
		private void _defineunitconstructor()
		{
			this._type?.DefineDefaultConstructor(
				MethodAttributes.Public |
				MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName
			);
		}

        /// <summary>
        /// create this unit off properties
        /// </summary>
        /// <param name="_propertytype"></param>
        /// <param name="_propertyname"></param>
        private void _defineunitproperty(Type _propertytype, string _propertyname)
        {
            if (this._type != null)
            {
				// TODO: consider renaming conventions here
                // basic field
                FieldBuilder _field = this._type.DefineField("_field" + _propertyname, _propertytype, FieldAttributes.Private);

                // get method for basic field
                MethodBuilder _get_method = this._type.DefineMethod("_get" + _propertyname,
					MethodAttributes.Public |
					MethodAttributes.SpecialName |
					MethodAttributes.HideBySig,
					_propertytype,
					Type.EmptyTypes
				);
                ILGenerator _get_immediatelanguage = _get_method.GetILGenerator();
                _get_immediatelanguage.Emit(OpCodes.Ldarg_0);
                _get_immediatelanguage.Emit(OpCodes.Ldfld, _field);
                _get_immediatelanguage.Emit(OpCodes.Ret);

                // set method for basic field
                MethodBuilder _set_method = this._type.DefineMethod("_set" + _propertyname,
					MethodAttributes.Public |
					MethodAttributes.SpecialName |
					MethodAttributes.HideBySig,
					null,
					new[] { _propertytype }
				);
                ILGenerator _set_immediatelanguage = _set_method.GetILGenerator();
                Label _modifyproperty = _set_immediatelanguage.DefineLabel();
                Label _exitset = _set_immediatelanguage.DefineLabel();
                _set_immediatelanguage.MarkLabel(_modifyproperty);
                _set_immediatelanguage.Emit(OpCodes.Ldarg_0);
                _set_immediatelanguage.Emit(OpCodes.Ldarg_1);
                _set_immediatelanguage.Emit(OpCodes.Stfld, _field);
                _set_immediatelanguage.Emit(OpCodes.Nop);
                _set_immediatelanguage.MarkLabel(_exitset);
                _set_immediatelanguage.Emit(OpCodes.Ret);

                // attaching newly created basic field to new property
                PropertyBuilder _property = this._type.DefineProperty(_propertyname, PropertyAttributes.HasDefault, _propertytype, null);
                _property.SetGetMethod(_get_method);
                _property.SetSetMethod(_set_method);
            }
        }

        #endregion

        #region public

		public _unitconfiguration? _getconfiguration()
		{
			return this._configuration;
		}

        /// <summary>
        /// create this unit off a new instance runtime
        /// </summary>
        /// <returns>instance or null on failure</returns>
        internal Object? _createinstance()
		{
			Object? _instance = null;
			try
			{
				// create an instance of the TypeBuilder
				Type _type = this._type?.CreateType() ?? typeof(Nullable);
				if (_type != typeof(Nullable))
				{
					_instance = Activator.CreateInstance(_type);
				}
			}
			catch (Exception _exception)
			{
				throw new Exception("Could not create instance", _exception);
			}
			return _instance;
		}

		#endregion
	}

    public class _unitconfiguration
	{
		#region attribute

		public string _unitname { get; set; }
		public List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };

        #endregion

        #region constructor

        /// <summary>
        /// unit configuration
        /// </summary>
        /// <param name="_unitname">unit name</param>
        /// <param name="_properties">unit properties</param>
        public _unitconfiguration(string _unitname, List<_propertyconfiguration> _properties)
		{
			this._unitname = _unitname;
			this._properties = _properties;
		}

		#endregion
	}

	public class _propertyconfiguration
	{
		#region attribute

		public Type _type { get; set; }
		public string _name { get; set; }
		public enum _systemdefaulttype : byte { Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Char, Boolean, String };

		#endregion

		#region constructor

		/// <summary>
		/// property configuration file
		/// </summary>
		/// <param name="_type">type of the property</param>
		/// <param name="_name">property name</param>
		public _propertyconfiguration(Type _type, string _name)
		{
			this._type = _type;
			this._name = _name;
		}

        /// <summary>
        /// property configuration file
        /// </summary>
        /// <param name="_systemdefalttype">type of the property , system default type in plain string</param>
        /// <param name="_name">property name</param>
        public _propertyconfiguration(string _systemdefalttype, string _name)
        {
            this._type = this._getsystemtypebystring(_systemdefalttype);
            this._name = _name;
        }

        /// <summary>
        /// property configuration file
        /// </summary>
        /// <param name="_systemdefaulttype">type of the property , system default type in enum</param>
        /// <param name="_name">property name</param>
        public _propertyconfiguration(_systemdefaulttype _systemdefaulttype, string _name)
		{
			this._type = this._getsystemtypebyenum(_systemdefaulttype);
			this._name = _name;
		}

		#endregion

		#region private

		private Type _getsystemtypebyenum(_systemdefaulttype _systemdefaulttype)
		{
            return this._getsystemtype(_systemdefaulttype.ToString() ?? string.Empty);
        }

		private Type _getsystemtypebystring(string _systemdefaulttype)
		{
			return this._getsystemtype(_systemdefaulttype);
		}

        private Type _getsystemtype(string _typeinstring)
        {
            Type _type = typeof(System.Nullable);

            string _typeunformatted = "System." + _typeinstring;
            _type = Type.GetType(_typeunformatted) ?? _type;

            return _type;
        }

        #endregion

        #region public

		/// <summary>
		/// check if provided type is a system default type
		/// </summary>
		/// <param name="_type">type</param>
		/// <returns>result as true\false</returns>
        public static bool _ispropertysystemdefaulttype(Type _type)
		{
			bool _issystemdefaulttype = false;

			if (_type != null)
			{
				string _typeinstring = _type.ToString().Substring(7);
				_issystemdefaulttype = Enum.IsDefined(typeof(_propertyconfiguration._systemdefaulttype), _typeinstring);
			}

			return _issystemdefaulttype;
		}

        #endregion
    }

    public class _instance
    {
        private readonly Object _unit;
        private readonly Object _entity;

        public _instance(_unit _unit)
        {
			if (_unit != null)
			{
				// block , start
				this._unit = _unit;
				this._entity = _unit._createinstance() ?? throw new Exception("Instance not created.");
				// block , end
			}
			else
			{
				throw new Exception("Provided unit is null.");
			}
        }

        private void _setpropertyvalue(Object _entity, PropertyInfo _property, Object? _value)
        {
            if (_entity != null && _property != null)
            {
                try
                {
                    _property.SetValue(_entity, _value, null);
                }
                catch (Exception _exception)
                {
                    throw new Exception("EXCEPTION: " + _exception.Message);
                }
            }
        }

		public void _setvalueset(Dictionary<string, Object?> _valueset)
		{
            // assigning sample values to the properties of the newly created instance
            if (this._entity != null)
            {
                Type _typeofentity = this._entity.GetType();
				foreach (KeyValuePair<string, Object?> _value in _valueset)
				{
					PropertyInfo? _property = _typeofentity.GetProperty(_value.Key);
					if (_property != null)
                    {
                        this._setpropertyvalue(this._entity, _property, _value.Value);
                    }
				}
            }
        }

        public Object _retrieveunit()
        {
            return this._unit;
        }

        public Object _retrieveinstance()
        {
            return this._entity;
        }
    }
}