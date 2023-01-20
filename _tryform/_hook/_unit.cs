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
		private TypeBuilder? _classbuilder { get; set; }
		private Type _class { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// construct _unit
		/// </summary>
		/// <param name="_unitconfiguration">_unitconfiguration</param>
		public _unit(_unitconfiguration _unitconfiguration)
		{
			this._unitconfiguration = _unitconfiguration;
			this._class = typeof(Nullable);

            if (this._rectifyconfiguration())
			{
				// block , start

				this._structure();

				this._createunit();

				// block , end
			}
			else
			{
				throw new Exception("Provided _unitconfiguration is invalid.");
            }
		}

		#endregion

		#region private

		private bool _rectifyconfiguration()
		{
			// check if configuration file not null
			if (this._unitconfiguration != null)
			{
				// check if name of this unit not null
				if (!string.IsNullOrEmpty(this._unitconfiguration._retrievename()))
				{
					bool _ispropertiescompliant = true;

					// check if properties of this unit not null
					if (this._unitconfiguration._retrieveproperties() != null)
					{
						// check if properties of this unit are compliant
						foreach (_propertyconfiguration _property in this._unitconfiguration._retrieveproperties())
						{
							if (
								String.IsNullOrEmpty(_property._retrievename()) ||
								String.IsNullOrEmpty(_property._retrievetype().ToString())
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

		private void _structure()
		{
            // _unit off class
            this._structureunitclass();

            // _unit off constructor
            this._structureunitconstructor();
			
			// _unit off properties
			for (int _index = 0; _index < this._unitconfiguration._retrieveproperties().Count; _index++)
			{
				this._structureunitproperty(_index);
            }
		}

		private void _structureunitclass()
		{
			try
			{
				// _assembly , name
				AssemblyName _assemblyname = new AssemblyName(this._unitconfiguration._retrievename());

				// _assembly , structure
				AssemblyBuilder _assemblybuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.RunAndCollect);

				// _module , structure
				ModuleBuilder _modulebuilder = _assemblybuilder.DefineDynamicModule(this._unitconfiguration._retrievename());

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
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
		}

		private void _structureunitconstructor()
		{
			if (this._classbuilder != null)
			{
				try
				{
					// _unit constructor , structure
					this._classbuilder?.DefineDefaultConstructor(
						MethodAttributes.Public |
						MethodAttributes.SpecialName |
						MethodAttributes.RTSpecialName
					);
				}
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
			}
		}

        private void _structureunitproperty(int _index)
        {
            if (this._classbuilder != null)
            {
				try
				{
					_propertyconfiguration _property = this._unitconfiguration._retrieveproperties()[_index];

					// _property _field , structure
					FieldBuilder _fieldbuilder = this._classbuilder.DefineField("_field" + _property._retrievename(), _property._retrievetype(), FieldAttributes.Private);

					// _property _method , structure , get
					MethodBuilder _methodbuilderget = this._classbuilder.DefineMethod("_get" + _property._retrievename(),
						MethodAttributes.Public |
						MethodAttributes.SpecialName |
						MethodAttributes.HideBySig,
						_property._retrievetype(),
						Type.EmptyTypes
					);
					// _property _method , structure , get immediate language generator
					ILGenerator _immediatelanguagegeneratorget = _methodbuilderget.GetILGenerator();
					_immediatelanguagegeneratorget.Emit(OpCodes.Ldarg_0);
					_immediatelanguagegeneratorget.Emit(OpCodes.Ldfld, _fieldbuilder);
					_immediatelanguagegeneratorget.Emit(OpCodes.Ret);

					// _property _method , structure , set
					MethodBuilder _methodbuilderset = this._classbuilder.DefineMethod("_set" + _property._retrievename(),
						MethodAttributes.Public |
						MethodAttributes.SpecialName |
						MethodAttributes.HideBySig,
						null,
						new[] { _property._retrievetype() }
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
					PropertyBuilder _propertybuilder = this._classbuilder.DefineProperty(_property._retrievename(), PropertyAttributes.HasDefault, _property._retrievetype(), null);
					_propertybuilder.SetGetMethod(_methodbuilderget);
					_propertybuilder.SetSetMethod(_methodbuilderset);
				}
				catch (Exception _exception)
				{
					throw new Exception(_exception.Message);
				}
            }
        }

		private void _createunit()
		{
			try
			{
				this._class = this._classbuilder?.CreateType() ?? typeof(Nullable);
			}
			catch (Exception _exception)
			{
				throw new Exception(_exception.Message);
			}
        }

        #endregion

        #region public

		/// <summary>
		/// retrieve _unit
		/// </summary>
		/// <returns>_unit</returns>
		public Type _retrieveunit()
		{
			return this._class;
		}

        /// <summary>
        /// _unit off separate instance
        /// </summary>
        /// <returns>instance or null on failure</returns>
        internal object? _separateinstance()
		{
			object? _instance = null;
			try
			{
                _instance = Activator.CreateInstance(this._retrieveunit());
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

		private readonly string _name;
		private readonly List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };

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

		private readonly Type _type;
		private readonly string _name;
		
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
        /// retrieve _type
        /// </summary>
        /// <returns>_type</returns>
        public Type _retrievetype()
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

				this._entity = _unit._separateinstance() ?? throw new Exception("Instance not created.");
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

        private void _setpropertyvalue(object _entity, PropertyInfo _property, object? _value)
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

        private object? _getpropertyvalue(object _entity, PropertyInfo _property)
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
        /// <param name="_valueset">valuset in form off Dictionary<string, KeyValuePair<object?, object?></param>
        /// <exception cref="Exception"></exception>
        public void _setvalueset(Dictionary<string, KeyValuePair<object?, object?>> _valueset)
		{
            if (this._entity != null)
            {
				if (_valueset != null)
				{
					foreach (KeyValuePair<string, KeyValuePair<object?, object?>> _value in _valueset)
					{
						try
						{
                            //object _entity = this._entity;

                            //object? _foreignentity = _value.Value.Value; // assign with null off type , if assigned Type is empty
                            //if (_foreignentity != null) // if _foreignentity not a null off type ; i.e., can not hold instance
                            //                     {
                            //	_entity = _foreignentity;
                            //                     }
                            object _entity = _value.Value.Value ?? this._entity;

                            PropertyInfo? _property = _entity.GetType().GetProperty(_value.Key);
                            if (_property != null)
							{
								this._setpropertyvalue(_entity, _property, _value.Value.Key);
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
                foreach (PropertyInfo _property in this._entity.GetType().GetProperties())
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
        /// retrieve _instance object
        /// </summary>
        /// <returns>_instance object</returns>
        public object _retrieveinstanceobject()
        {
            return this._entity;
        }
		
		/// <summary>
        /// retrieve _instance type
        /// </summary>
        /// <returns>_instance type</returns>
        public Type _retrieveinstancetype()
        {
            return this._type;
        }

		#endregion
	}
}