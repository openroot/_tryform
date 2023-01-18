using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;

namespace _unit
{
	/// <summary>
	/// _unit
	/// </summary>
	public class _unit
	{
		#region attribute

		public readonly _unitconfiguration _unitconfiguration;
		private TypeBuilder? _typebuilder { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// construct this unit
		/// </summary>
		/// <param name="_unitconfiguration">this unit configuration</param>
		public _unit(_unitconfiguration _unitconfiguration)
		{
			this._unitconfiguration = _unitconfiguration;

			if (this._checkconfiguration())
			{
				this._proceed();
			}
		}

		#endregion

		#region private

		private bool _checkconfiguration()
		{
			// check if configuration file not null
			if (this._unitconfiguration != null)
			{
				// check if name of this unit not null
				if (!string.IsNullOrEmpty(this._unitconfiguration._name))
				{
					bool _ispropertiescompliant = true;

					// check if properties of this unit not null
					if (this._unitconfiguration._properties != null)
					{
						// check if properties of this unit are compliant
						foreach (_propertyconfiguration _property in this._unitconfiguration._properties)
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

		private void _proceed()
		{
            // define startup structure of this unit
            this._defineunit();

            // define this unit off constructor
            this._defineunitconstructor();
			
			// define this unit off properties
            foreach (_propertyconfiguration _property in this._unitconfiguration._properties)
            {
                this._defineunitproperty(_property._type, _property._name); // TODO: instead , pass object off _propertyconfiguration
            }
        }

		private void _defineunit()
		{
			AssemblyName _assemblyname = new AssemblyName(this._unitconfiguration._name);
			AssemblyBuilder _assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.RunAndCollect);

			ModuleBuilder _unit = _assemblybuilder.DefineDynamicModule(this._unitconfiguration._name);
			this._typebuilder = _unit.DefineType(_assemblyname.FullName,
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoLayout,
				null
			);
		}

		private void _defineunitconstructor()
		{
			this._typebuilder?.DefineDefaultConstructor(
				MethodAttributes.Public |
				MethodAttributes.SpecialName |
				MethodAttributes.RTSpecialName
			);
		}

        private void _defineunitproperty(Type _type, string _name)
        {
            if (this._typebuilder != null)
            {
				// TODO: consider renaming conventions here
                // basic field
                FieldBuilder _field = this._typebuilder.DefineField("_field" + _name, _type, FieldAttributes.Private);

                // get method for basic field
                MethodBuilder _get_method = this._typebuilder.DefineMethod("_get" + _name,
					MethodAttributes.Public |
					MethodAttributes.SpecialName |
					MethodAttributes.HideBySig,
					_type,
					Type.EmptyTypes
				);
                ILGenerator _get_immediatelanguage = _get_method.GetILGenerator();
                _get_immediatelanguage.Emit(OpCodes.Ldarg_0);
                _get_immediatelanguage.Emit(OpCodes.Ldfld, _field);
                _get_immediatelanguage.Emit(OpCodes.Ret);

                // set method for basic field
                MethodBuilder _set_method = this._typebuilder.DefineMethod("_set" + _name,
					MethodAttributes.Public |
					MethodAttributes.SpecialName |
					MethodAttributes.HideBySig,
					null,
					new[] { _type }
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
                PropertyBuilder _property = this._typebuilder.DefineProperty(_name, PropertyAttributes.HasDefault, _type, null);
                _property.SetGetMethod(_get_method);
                _property.SetSetMethod(_set_method);
            }
        }

        #endregion

        #region public

        /// <summary>
        /// _unit off separate instance
        /// </summary>
        /// <returns>instance or null on failure</returns>
        internal object? _separateinstance()
		{
			object? _instance = null;
			try
			{
				// create an instance of the TypeBuilder
				Type _type = this._typebuilder?.CreateType() ?? typeof(Nullable);
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

	/// <summary>
	/// _unit off configuration
	/// </summary>
    public class _unitconfiguration
	{
		#region attribute

		public readonly string _name;
		public readonly List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };

        #endregion

        #region constructor

        /// <summary>
        /// create _unit off _unitconfiguration
        /// </summary>
        /// <param name="_name">_name , e.g., _classloremipsum</param>
        /// <param name="_properties">_properties</param>
        public _unitconfiguration(string _name, List<_propertyconfiguration> _properties)
		{
			this._name = _name;
			this._properties = _properties;
		}

		#endregion
	}

	/// <summary>
	/// _property off configuration
	/// </summary>
	public class _propertyconfiguration
	{
		#region attribute

		/// <summary>
		/// _systemdefault values
		/// </summary>
		public enum _systemdefaultvalue : byte { Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Char, Boolean, String };

		public readonly Type _type;
		public readonly string _name;
		
        #endregion

        #region constructor

        /// <summary>
        /// create _unit off _propertyconfiguration
        /// </summary>
        /// <param name="_type">_type</param>
        /// <param name="_name">_name</param>
        public _propertyconfiguration(Type _type, string _name)
		{
			this._type = _type;
			this._name = _name;
		}

        /// <summary>
        /// create _unit off _propertyconfiguration
        /// </summary>
        /// <param name="_type">_type , string format</param>
        /// <param name="_name">_name</param>
        public _propertyconfiguration(string _type, string _name)
        {
            this._type = _getsystemtypebystring(_type);
            this._name = _name;
        }

        /// <summary>
        /// create _unit off _propertyconfiguration
        /// </summary>
        /// <param name="_type">_type , enum format</param>
        /// <param name="_name">_name</param>
        public _propertyconfiguration(_systemdefaultvalue _type, string _name)
		{
			this._type = _getsystemtypebyenum(_type);
			this._name = _name;
		}

		#endregion

		#region private

		private static Type _getsystemtypebyenum(_systemdefaultvalue _systemdefaulttype)
		{
			try
			{
				string _typeinstring = _systemdefaulttype.ToString();
				if (!string.IsNullOrEmpty(_typeinstring))
				{
					return _getsystemtype(_typeinstring);
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

		private static Type _getsystemtypebystring(string _systemdefaulttype)
		{
			return _getsystemtype(_systemdefaulttype);
		}

        private static Type _getsystemtype(string _typeinstring)
        {
            Type _type = typeof(System.Nullable);

			try
			{
				string _typeunformatted = "System." + _typeinstring;
				_type = Type.GetType(_typeunformatted) ?? _type;
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}

            return _type;
        }

        #endregion

        #region public

		/// <summary>
		/// check if provided type is systemdefault
		/// </summary>
		/// <param name="_type">_type</param>
		/// <returns>true\false</returns>
        public static bool _ispropertysystemdefault(Type _type)
		{
			bool _issystemdefault;

			if (_type != null)
			{
				try
				{
					string _typeinstring = _type.ToString().Substring(7);
					_issystemdefault = Enum.IsDefined(typeof(_propertyconfiguration._systemdefaultvalue), _typeinstring);
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

	/// <summary>
	/// _unit off _instance
	/// </summary>
    public class _instance
    {
        #region attribute

        private readonly object _unit;
        private readonly Type _entity;

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

				this._unit = _unit;
				this._entity = _unit._separateinstance()?.GetType() ?? throw new Exception("Instance not created.");
				
				// block , end
			}
			else
			{
				throw new Exception("Provided unit is null.");
			}
        }

        #endregion

        #region private

        private void _setpropertyvalue(Type _entity, PropertyInfo _property, object? _value)
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

        private object? _getpropertyvalue(Type _entity, PropertyInfo _property)
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
        /// assign valuset
        /// </summary>
        /// <param name="_valueset">valuset in form off Dictionary<string, object?></param>
        /// <exception cref="Exception"></exception>
        public void _setvalueset(Dictionary<string, object?> _valueset)
		{
            if (this._entity != null)
            {
				if (_valueset != null)
				{
					foreach (KeyValuePair<string, object?> _value in _valueset)
					{
						try
						{
							PropertyInfo? _property = this._entity.GetProperty(_value.Key);
							if (_property != null)
							{
								this._setpropertyvalue(this._entity, _property, _value.Value);
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
        /// retrieve valuset
        /// </summary>
        /// <returns>valuset in form off Dictionary<string, object?></returns>
        /// <exception cref="Exception"></exception>
        public Dictionary<string, object?> _getvalueset()
        {
			Dictionary<string, object?> _valueset = new Dictionary<string, object?>() { };
            if (this._entity != null)
            {
                foreach (PropertyInfo _property in this._entity.GetProperties())
                {
					try
					{
						_valueset.Add(_property.Name, this._getpropertyvalue(this._entity, _property));
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

		/// <summary>
		/// retrieve _unit parent
		/// </summary>
		/// <returns>_unit</returns>
        public object _retrieveunit()
        {
            return this._unit;
        }

        /// <summary>
        /// retrieve _instance type
        /// </summary>
        /// <returns>_entity</returns>
        public object _retrieveentity()
        {
            return this._entity;
        }

		#endregion
	}
}