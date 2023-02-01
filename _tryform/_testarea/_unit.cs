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

        #endregion

        #region Functionality named Module Operations

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

        #endregion
    }
}
