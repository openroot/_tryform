using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                        //this._instanceactions(_operation, _indentcount, _instance, _unitcategory.Key);
                    }

                    _message = Environment.NewLine + "]]]";
                    Console.Write(_message);
                }

                // block , end
            }
        }
        
        /*private void _instanceactions(char _operation, int _indentcount, Object _object, Guid _guid)
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
        }*/
        
        public void _createunit_loremipsum()
        {
            List<_typeconfiguration._typeraw> _sampletypearealconfiguration = _typeconfiguration._fetchsampletyperawset();
            string? _sampletyperealconfiguration = _typeconfiguration._jsonreal(_sampletypearealconfiguration);
            
            _typeconfiguration? _sampletypeconfiguration = null;
            try
            {
                _sampletypeconfiguration = new _typeconfiguration(_sampletyperealconfiguration ?? string.Empty);
                Console.WriteLine("_typeconfiguration created.");
            }
            catch (Exception _exception)
            {
                Console.WriteLine("_typeconfiguration not created. ", _exception.Message);
            }

            if (_sampletypeconfiguration != null)
            {
                _unit? _unit = null;
                try
                {
                    _unit = new _unit(_sampletypeconfiguration);
                    Console.WriteLine("_unit created.");
                }
                catch (Exception _exception)
                {
                    Console.WriteLine("_unit not created. ", _exception.Message);
                }

                if (_unit != null)
                {
                    var _unitcontainer = _datacontainer._unitcontainer._fetchtypecontainerset();

                    try
                    {
                        _instance _xy_i1 = new _instance(1, 1);
                        _instance _xy_i2 = new _instance(1, 2);
                        _instance _pq_i1 = new _instance(2, 1);
                        _instance _st_i1 = new _instance(13);
                        _instance _st_i2 = new _instance(13);

                        _xy_i1._assignproperties(new Dictionary<string, object?>() {
                            {"_id", 796},
                            {"_fullname", "Debaprasad Tapader"},
                            {"_isdead", true}
                        });
                        _pq_i1._assignproperties(new Dictionary<string, object?>() {
                            {"_xy", 1},
                            {"_tag", "Computer Scientist"}
                        });
                        _st_i1._assignproperties(new Dictionary<string, object?>() {
                            {"_xy", 1},
                            {"_tag", "Software"},
                            {"_service", "Engineering"},
                            {"_api", true}
                        });
                        _st_i2._assignproperties(new Dictionary<string, object?>() {
                            {"_service", "R&D"},
                            {"_api", false}
                        });

                        Console.WriteLine("_entity(s) created.");
                        string? _datarealcomplete = _datacontainer._unitcontainer._datareal(true);
                        string? _datarealhook3 = _datacontainer._unitcontainer._datareal(true, 13);
                    }
                    catch (Exception _exception)
                    {
                        Console.WriteLine("_entity(s) not created. ", _exception.Message);
                    }
                }
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
