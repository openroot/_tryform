using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _customunit
{
    public class _test
    {
        #region attribute

        private Dictionary<Guid, List<Object>> _unitcontainer = new Dictionary<Guid, List<Object>>() { };

        #endregion

        #region Functionality named Module Operations

        private void _addinstanceobjecttomodulescontainer(Object? _instance)
        {
            if (this._unitcontainer != null)
            {
                if (_instance != null)
                {
                    Object _newinstance = _instance ?? new Object();

                    Guid _unitguid = _newinstance.GetType().GUID;
                    // add new unit guid , if not already added
                    if (!_unitcontainer.ContainsKey(_unitguid))
                    {
                        _unitcontainer.Add(_unitguid, new List<Object>());
                    }
                    // now , connect instance to guid respective
                    if (_unitcontainer.ContainsKey(_unitguid))
                    {
                        _unitcontainer[_unitguid].Add(_newinstance);
                    }
                }
                else
                {
                    throw new Exception("Provided instance is null");
                }
            }
            else
            {
                throw new Exception("Provided container off instance is null");
            }
        }

        private void _traversemodules(char _instanceoperation)
        {
            // traverse the module container
            if (this._modulescontainer != null)
            {
                int _modulecount = 0; // module counter

                // loop through modules (if any) found in module container
                foreach (KeyValuePair<Guid, List<Object>> _modulecontainer in _modulescontainer)
                {
                    string _message = string.Empty;

                    // module type
                    Type? _module = _modulecontainer.Value[0].GetType();

                    _message += _modulecount > 0 ? Environment.NewLine + Environment.NewLine : string.Empty;

                    // output module order number
                    _message += "--(**module (" + ++_modulecount + ".))" + Environment.NewLine;
                    // output module description
                    _message += _module.FullName + " [" + _modulecontainer.Key + "]" + Environment.NewLine;
                    _message += "[[[";
                    Console.Write(_message);

                    int _instancecount = 0; // module counter

                    // loop through each instances for the module (if any)
                    foreach (Object _instanceobject in _modulecontainer.Value)
                    {
                        // output instance order number
                        _message = Environment.NewLine + "\t(**instance (" + ++_instancecount + ".))" + Environment.NewLine;
                        // output instance description
                        _message += "\t" + _instanceobject.GetType().Name + " [" + _modulecontainer.Key + "]"; // TODO: add instance GUID here
                        Console.Write(_message);

                        // pass the instance object to action propagator
                        int _objectindentcount = 0;
                        this._instanceactions(_instanceobject, _instanceoperation, _objectindentcount, _modulecontainer.Key);
                    }

                    _message = Environment.NewLine + "]]]";
                    Console.Write(_message);
                }
            }
        }

        private void _instanceactions(Object? _instanceobject, char _instanceoperation, int _objectindentcount, Guid _moduleguid)
        {
            if (_instanceobject != null)
            {
                // set object indent
                ++_objectindentcount; // it denotes this instances current level
                string _objectindent = string.Empty;
                for (int _index = 0; _index < _objectindentcount; _index++)
                {
                    _objectindent += "\t";
                }

                Type _instance = _instanceobject.GetType();

                string _message = string.Empty;

                // output START
                // add indent to START only if instance level is first
                _message += (_objectindentcount == 1) ? _objectindent : string.Empty;
                switch (_instanceoperation)
                {
                    case 'c':
                        _message += " {";
                        break;
                    case 'o':
                        _message += " {";
                        break;
                    case 'i':
                        _message += " (Enter values for each property:) {";
                        break;
                }
                Console.Write(_message);

                int _propertycount = 0; // property counter

                // loop through each property of instance (if any)
                foreach (PropertyInfo _property in _instance.GetProperties())
                {
                    _message = string.Empty;

                    // get if property is system default type
                    bool _ispropertysystemdefaulttype = _propertyconfiguration._ispropertysystemdefaulttype(_property.PropertyType);

                    // output property order number
                    _message += Environment.NewLine + _objectindent + "\t(" + ++_propertycount + ".) ";
                    // output property description
                    // TODO: update GUID plated here with 'original' module GUID
                    _message += _property.Name + " [" + (_ispropertysystemdefaulttype ? _property?.PropertyType.FullName : _moduleguid) + "]";

                    switch (_instanceoperation)
                    {
                        case 'c':
                            this._outputpropertyculturalbehaviour(_property);
                            break;
                        case 'o':
                            string _propertyvalueinstring = this._getpropertyvalue(_property, _instanceobject)?.ToString() ?? "N/A (null)";
                            _message += " = " + (!_propertyvalueinstring.Equals("N/A (null)") ? _ispropertysystemdefaulttype ? _propertyvalueinstring : string.Empty : _propertyvalueinstring);
                            break;
                        case 'i':
                            // TODO: take input from console
                            Object? _propertyvalue = new Object();
                            this._setpropertyvalue(_property, _instanceobject, _propertyvalue);
                            break;
                    }

                    Console.Write(_message);

                    // call a recursive here if it's (property) not a system default type
                    if (_property != null && !_ispropertysystemdefaulttype)
                    {
                        int _objectindentcountnext = _objectindentcount;
                        this._instanceactions(this._getpropertyvalue(_property, _instanceobject), _instanceoperation, _objectindentcountnext, _moduleguid);
                    }
                }

                // output END
                Console.Write(Environment.NewLine + _objectindent + "}");
            }
        }

        private void _createsamplemodule()
        {
            // creating sample module configuration
            _moduleconfiguration _samplemoduleconfiguration = new _moduleconfiguration(
                "_student",
                new List<_propertyconfiguration>() {
                    new _propertyconfiguration("Int32", "_id"),
                    new _propertyconfiguration("String", "_fullname"),
                    new _propertyconfiguration("String", "_address"),
                    new _propertyconfiguration("Boolean", "_isdied")
                }
            );

            try
            {
                // creating sample dynamic module
                _builddynamicmodule _module_student = new _builddynamicmodule(_samplemoduleconfiguration);

                // creating an arbitrary instance of newly created module
                Object? _instanceobject_student = _module_student._haveaninstance();

                // adding the newly created instance to module container
                this._addinstanceobjecttomodulescontainer(_instanceobject_student);

                // assigning sample values to the properties of the newly created instance
                Type _instance_student = _instanceobject_student?.GetType() ?? typeof(Nullable);
                if (_instanceobject_student != null && _instance_student != typeof(Nullable))
                {
                    this._setpropertyvalue(_instance_student.GetProperty("_id"), _instanceobject_student, 796);
                    this._setpropertyvalue(_instance_student.GetProperty("_fullname"), _instanceobject_student, "Debaprasad Tapader");
                    this._setpropertyvalue(_instance_student.GetProperty("_address"), _instanceobject_student, "Deoghar, JH, IN");
                    this._setpropertyvalue(_instance_student.GetProperty("_isdied"), _instanceobject_student, true);
                }


                // Sampling dynamic module inside another dynamic module
                _moduleconfiguration _samplemoduleconfiguration_scholar = new _moduleconfiguration(
                    "_scholar",
                    new List<_propertyconfiguration>() {
                        new _propertyconfiguration(_instance_student, "_student"),
                        new _propertyconfiguration("String", "_sector"),
                        new _propertyconfiguration("Int32", "_year"),
                        new _propertyconfiguration("Boolean", "_isactive")
                    }
                );
                _builddynamicmodule _module_scholar = new _builddynamicmodule(_samplemoduleconfiguration_scholar);
                Object? _instanceobject_scholar1 = _module_scholar._haveaninstance();
                this._addinstanceobjecttomodulescontainer(_instanceobject_scholar1);
                // assigning sample values to the properties of the newly created instance
                Type _instance_scholar1 = _instanceobject_scholar1?.GetType() ?? typeof(Nullable);
                if (_instanceobject_scholar1 != null && _instance_scholar1 != typeof(Nullable) && _instanceobject_student != null)
                {
                    this._setpropertyvalue(_instance_scholar1.GetProperty("_student"), _instanceobject_scholar1, _instanceobject_student);
                    this._setpropertyvalue(_instance_scholar1.GetProperty("_sector"), _instanceobject_scholar1, "Matter Design");
                    this._setpropertyvalue(_instance_scholar1.GetProperty("_year"), _instanceobject_scholar1, 2003);
                    this._setpropertyvalue(_instance_scholar1.GetProperty("_isactive"), _instanceobject_scholar1, true);
                }
                Object? _instanceobject_scholar2 = _module_scholar._haveaninstance();
                this._addinstanceobjecttomodulescontainer(_instanceobject_scholar2);
                // assigning sample values to the properties of the newly created instance
                Type _instance_scholar2 = _instanceobject_scholar2?.GetType() ?? typeof(Nullable);
                if (_instanceobject_scholar2 != null && _instance_scholar2 != typeof(Nullable))
                {
                    this._setpropertyvalue(_instance_scholar2.GetProperty("_student"), _instanceobject_scholar2, null);
                    this._setpropertyvalue(_instance_scholar2.GetProperty("_sector"), _instanceobject_scholar2, "Classic Culture");
                    this._setpropertyvalue(_instance_scholar2.GetProperty("_year"), _instanceobject_scholar2, 2005);
                    this._setpropertyvalue(_instance_scholar2.GetProperty("_isactive"), _instanceobject_scholar2, true);
                }

                Console.Write("Sample Module Has Been Created. Congrats!");
            }
            catch (Exception _exception)
            {
                Console.WriteLine("EXCEPTION: " + _exception.Message);
            }
        }

        private void _setpropertyvalue(PropertyInfo? _property, Object? _instanceobject, Object? _value)
        {
            if (_property != null && _instanceobject != null)
            {
                try
                {
                    _property.SetValue(_instanceobject, _value, null);
                }
                catch (Exception _exception)
                {
                    throw new Exception("EXCEPTION: " + _exception.Message);
                }
            }
        }

        private Object? _getpropertyvalue(PropertyInfo? _property, Object? _instanceobject)
        {
            Object? _value = null;
            if (_property != null && _instanceobject != null)
            {
                try
                {
                    _value = _property?.GetValue(_instanceobject, null);
                }
                catch (Exception _exception)
                {
                    throw new Exception("EXCEPTION: " + _exception.Message);
                }
            }
            return _value;
        }

        private void _inputmoduleconfiguration()
        {
            // TODO: Have code here
        }

        public void _createmodulemanually()
        {
            // TODO: Make modules manually
        }

        private void _outputpropertyculturalbehaviour(PropertyInfo? _property)
        {
            if (_property != null)
            {
                // TODO: Might add more details on Property Culture
            }
        }

        #endregion
    }
}
