using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static _unit._unit._classcontainer;

namespace _unit
{
    public class _test
    {
        #region attribute

        private Dictionary<Guid, List<object>> _entitycontainer = new Dictionary<Guid, List<object>>() { };

        #endregion

        #region Functionality named Module Operations

        private void _assignenitycontainer(object _entity)
        {
            if (this._entitycontainer != null)
            {
                // block , start
                if (_entity != null)
                {
                    Guid _guid = _entity.GetType().GUID;
                    // add new unit off guid , if not already added
                    if (!_entitycontainer.ContainsKey(_guid))
                    {
                        _entitycontainer.Add(_guid, new List<object>());
                    }
                    // now , connect instance to guid respective
                    if (_entitycontainer.ContainsKey(_guid))
                    {
                        _entitycontainer[_guid].Add(_entity);
                    }
                }
                else
                {
                    throw new Exception("Provided _enity is null");
                }
                // block , end
            }
            else
            {
                throw new Exception("Provided container off _entity is null");
            }
        }
        
        public void _traversemodules(char _operation)
        {
            // unitcontainer , traverse
            if (this._entitycontainer != null)
            {
                // block , start

                int _unitcount = 0; // unit counter
                // unitcategory , loop through
                foreach (KeyValuePair<Guid, List<Object>> _unitcategory in this._entitycontainer)
                {
                    string _message = _unitcount > 0 ? Environment.NewLine + Environment.NewLine : string.Empty;

                    // unit order , output
                    _message += "--(**unit (" + ++_unitcount + ".))" + Environment.NewLine;

                    // unittype
                    Type _unittype = _unitcategory.Value[0].GetType(); // retrieve unittype from first value of unitcategory

                    // unittype , description
                    _message += _unittype.FullName + " [" + _unitcategory.Key + "]" + Environment.NewLine + "[[[";
                    Console.Write(_message);

                    int _instancecount = 0; // instance counter
                    // instance , loop through
                    foreach (Object _instance in _unitcategory.Value)
                    {
                        // instance order , output
                        _message = Environment.NewLine + "\t(**instance (" + ++_instancecount + ".))" + Environment.NewLine;
                        // instance , description
                        _message += "\t" + _instance.GetType().Name + " [" + _unitcategory.Key + "]"; // TODO: add instance GUID here
                        Console.Write(_message);

                        // pass the instance object to action propagator
                        int _indentcount = 0;
                        this._instanceactions(_operation, _indentcount, _instance, _unitcategory.Key);
                    }

                    _message = Environment.NewLine + "]]]";
                    Console.Write(_message);
                }

                // block , end
            }
        }
        
        private void _instanceactions(char _operation, int _indentcount, Object _object, Guid _guid)
        {
            if (_object != null)
            {
                // instance , indent reset
                ++_indentcount; // instance , indent level current
                string _indentmessage = string.Empty;
                for (int _index = 0; _index < _indentcount; _index++)
                {
                    _indentmessage += "\t";
                }

                Type _instance = _object.GetType();

                string _message = string.Empty;

                // block , start

                // add indent reset only if instance level is first
                _message += (_indentcount == 1) ? _indentmessage : string.Empty;
                switch (_operation)
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

                // property , loop through
                foreach (PropertyInfo _property in _instance.GetProperties())
                {
                    _message = string.Empty;

                    // get if property is system default type
                    bool _ispropertysystemdefaulttype = _classconfiguration._propertyconfiguration._ispropertysystemdefault(_property.PropertyType);

                    // output property order number
                    _message += Environment.NewLine + _indentmessage + "\t(" + ++_propertycount + ".) ";
                    // output property description
                    // TODO: update GUID plated here with 'original' module GUID
                    _message += _property.Name + " [" + (_ispropertysystemdefaulttype ? _property?.PropertyType.FullName : _guid) + "]";

                    switch (_operation)
                    {
                        case 'c':
                            this._outputpropertyculturalbehaviour(_property);
                            break;
                        case 'o':
                            string _propertyvalueinstring = this._getpropertyvalue(_property, _object)?.ToString() ?? "N/A (null)"; // TODO: 
                            _message += " = " + (!_propertyvalueinstring.Equals("N/A (null)") ? _ispropertysystemdefaulttype ? _propertyvalueinstring : string.Empty : _propertyvalueinstring);
                            break;
                        case 'i':
                            // TODO: take input from console
                            Object? _propertyvalue = new Object();
                            this._setpropertyvalue(_property, _object, _propertyvalue);
                            break;
                    }

                    Console.Write(_message);

                    // call a recursive here if it's (property) not a system default type
                    if (_property != null && !_ispropertysystemdefaulttype)
                    {
                        int _indentcountnext = _indentcount;
                        this._instanceactions(_operation, _indentcountnext, this._getpropertyvalue(_property, _object), _guid); // TODO: 
                    }
                }

                // block , end
                Console.Write(Environment.NewLine + _indentmessage + "}");
            }
        }
        
        public void _createunit_loremipsum()
        {
            try
            {
                // block , start

                // _class _loremipsum , _classconfiguration
                _classconfiguration _loremipsumclassconfig = new _classconfiguration(
                    "_loremipsum",
                    new List<_classconfiguration._propertyconfiguration>() {
                    new _classconfiguration._propertyconfiguration("Int32", "_id"),
                    new _classconfiguration._propertyconfiguration("String", "_fullname"),
                    new _classconfiguration._propertyconfiguration("String", "_address"),
                    new _classconfiguration._propertyconfiguration("Boolean", "_isdied"),
                    new _classconfiguration._propertyconfiguration("String", "_foo")
                    }
                );

                // _class _loremipsum , creating _class
                _unit _loremipsum = new _unit(_loremipsumclassconfig);

                // _class _loremipsum , creating _entity _foobar
                _instance _foobar = new _instance(_loremipsum);

                // _entity _foobar , assiging properties
                if (_foobar != null)
                {
                    _foobar._assignproperties(new Dictionary<string, object?>() {
                        {"_id", 796},
                        {"_fullname", "Debaprasad Tapader"},
                        {"_address", "Deoghar, JH, IN"},
                        {"_isdead", true},
                        {"_foo", null}
                    });

                    // _entitycontainer , assigning _entity _foobar
                    this._assignenitycontainer(_foobar._retrieveentity());
                }

                // block , end

                if (_foobar != null)
                {
                    // block , start

                    _classconfiguration _loremipsumseparateclassconfig = new _classconfiguration(
                        "_loremipsumseparate",
                        new List<_classconfiguration._propertyconfiguration>() {
                            //new _classconfiguration._propertyconfiguration(_foobar._retrievetype(), "_loremipsum"),
                            new _classconfiguration._propertyconfiguration(_loremipsum._retrievetype(), "_loremipsum"),
                            new _classconfiguration._propertyconfiguration("String", "_sector"),
                            new _classconfiguration._propertyconfiguration("Int32", "_year"),
                            new _classconfiguration._propertyconfiguration("Boolean", "_isactive")
                        }
                    );
                    _unit _loremipsumseparate = new _unit(_loremipsumseparateclassconfig);

                    _instance _foobar101 = new _instance(_loremipsumseparate);
                    if (_foobar101 != null)
                    {
                        _foobar101._assignproperties(new Dictionary<string, object?>() {
                            {"_loremipsum", _foobar._retrieveentity()},
                            {"_sector", "Matter Design"},
                            {"_year", 2003},
                            {"_isactive", true}
                        });
                        this._assignenitycontainer(_foobar101._retrieveentity());
                    }

                    _instance _foobar201 = new _instance(_loremipsumseparate);
                    if (_foobar201 != null)
                    {
                        _foobar201._assignproperties(new Dictionary<string, object?>() {
                            {"_loremipsum", null},
                            {"_sector", "Classic Culture"},
                            {"_year", 2004},
                            {"_isactive", true}
                        });
                        this._assignenitycontainer(_foobar201._retrieveentity());
                    }

                    // block , end
                }
                Console.Write("unit loremipsum created");
                Dictionary<Guid, _entityset> _classset = _unit._classcontainer._retrieveclassset();
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
