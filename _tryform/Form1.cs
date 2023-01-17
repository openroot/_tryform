using _unit;
namespace _tryform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var _test = new _test();

            _test._createunit_loremipsum();
            _test._traversemodules('c');
            _test._traversemodules('o');
        }
    }
}