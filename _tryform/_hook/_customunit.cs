using System.Reflection;
using System.Reflection.Emit;
using Label = System.Reflection.Emit.Label;

namespace _customunit
{
	public class _customunit
	{
		#region attribute

		readonly _configuration _configuration;
		TypeBuilder? _type { get; set; }

		#endregion

		#region constructor

		/// <summary>
		/// Build a dynamic unit
		/// </summary>
		/// <param name="_configuration">unit configuration</param>
		public _customunit(_configuration _configuration)
		{
			this._configuration = _configuration;

			this._preparestructure();
		}

		#endregion

		#region private

		/// <summary>
		/// Prepare Unit Structure
		/// </summary>
		private void _preparestructure()
		{
			// Check if configuration file not null
			if (this._configuration != null)
			{
				// Check if name of the unit not null
				if (!string.IsNullOrEmpty(this._configuration._unitname))
				{
					bool _ispropertiescompliant = true;

					// Check if properties not null
					if (this._configuration._properties != null)
					{
						// Check if properties are compliant
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

						// Preparing the unit
						if (_ispropertiescompliant)
						{
							// Define initial structure of unit
							this._defineunit();

							// Define unit constructor
							this._defineunitconstructor();

							foreach (_propertyconfiguration _property in this._configuration._properties)
							{
								// Define this property
								this._defineunitproperty(_property._type, _property._name);
							}
						}
					}
					else if (!_ispropertiescompliant)
					{
						throw new Exception("Property(s) of unit is/are not compliant in unit configuration file.");
					}
					else
					{
						throw new Exception("Property(s) of unit returned null in unit configuration file.");
					}
				}
				else
				{
					throw new Exception("Name of unit returned null or empty in unit configuration file.");
				}
			}
			else
			{
				throw new Exception("returned null in unit configuration file.");
			}
		}

		/// <summary>
		/// Assembly Definition
		/// </summary>
		private void _defineunit()
		{
			AssemblyName _assemblyname = new AssemblyName(this._configuration._unitname);
			AssemblyBuilder _assembly = AssemblyBuilder.DefineDynamicAssembly(_assemblyname, AssemblyBuilderAccess.Run);

			ModuleBuilder _unit = _assembly.DefineDynamicModule(this._configuration._unitname);
			this._type = _unit.DefineType(_assemblyname.FullName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null);
		}

		private void _defineunitproperty(Type _propertytype, string _propertyname)
		{
			if (this._type != null)
			{
				// basic field
				FieldBuilder _field = this._type.DefineField("_field" + _propertyname, _propertytype, FieldAttributes.Private);

				// get method for basic field
				MethodBuilder _get_method = this._type.DefineMethod("_get" + _propertyname, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, _propertytype, Type.EmptyTypes);
				ILGenerator _get_immediatelanguage = _get_method.GetILGenerator();
				_get_immediatelanguage.Emit(OpCodes.Ldarg_0);
				_get_immediatelanguage.Emit(OpCodes.Ldfld, _field);
				_get_immediatelanguage.Emit(OpCodes.Ret);

				// set method for basic field
				MethodBuilder _set_method = this._type.DefineMethod("_set" + _propertyname, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { _propertytype });
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

		private void _defineunitconstructor()
		{
			this._type?.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
		}

		#endregion

		#region public

		/// <summary>
		/// Create a new instance of the dynamic unit
		/// </summary>
		/// <returns>Object (nullable)</returns>
		public Object? _haveaninstance()
		{
			Object? _instanceobject = null;
			try
			{
				// Create an instance of the TypeBuilder
				Type _type = this._type?.CreateType() ?? typeof(Nullable);
				if (_type != typeof(Nullable))
				{
					_instanceobject = Activator.CreateInstance(_type);
				}
			}
			catch (Exception _exception)
			{
				throw new Exception("Could not instantiate TypeBuilder", _exception);
			}
			return _instanceobject;
		}

		#endregion
	}

	public class _configuration
	{

		#region attribute

		public string _unitname { get; set; }
		public List<_propertyconfiguration> _properties = new List<_propertyconfiguration>() { };

        #endregion

        #region constructor

        /// <summary>
        /// Unit configuration file
        /// </summary>
        /// <param name="_unitname">Unit name</param>
        /// <param name="_properties">Unit properties</param>
        public _configuration(string _unitname, List<_propertyconfiguration> _properties)
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
		/// Property configuration file
		/// </summary>
		/// <param name="_type">Type of the property</param>
		/// <param name="_name">Property name</param>
		public _propertyconfiguration(Type _type, string _name)
		{
			this._type = _type;
			this._name = _name;
		}

		/// <summary>
		/// Property configuration file
		/// </summary>
		/// <param name="_type">Type of the property system default</param>
		/// <param name="_name">Property name</param>
		public _propertyconfiguration(_systemdefaulttype _type, string _name)
		{
			this._type = this._getsystemtype(_type);
			this._name = _name;
		}

		/// <summary>
		/// Property configuration file
		/// </summary>
		/// <param name="_type">Type of the property system default in plain string</param>
		/// <param name="_name">Property name</param>
		public _propertyconfiguration(string _type, string _name)
		{
			this._type = this._getsystemtypebystring(_type);
			this._name = _name;
		}

		#endregion

		#region private

		private Type _getsystemtype(_systemdefaulttype _systemdefaulttype)
		{
			Type _type = typeof(System.Nullable);

			string _typeunformatted = "System." + _systemdefaulttype.ToString();
			_type = Type.GetType(_typeunformatted) ?? _type;

			return _type;
		}

		private Type _getsystemtypebystring(string _systemdefaulttype)
		{
			Type _type = typeof(System.Nullable);

			string _typeunformatted = "System." + _systemdefaulttype.ToString();
			_type = Type.GetType(_typeunformatted) ?? _type;

			return _type;
		}

		#endregion

		#region public

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
}