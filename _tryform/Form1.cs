using _unit;
namespace _tryform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool _istestsuccess = false;

            var _test = new _test();

            string[] _args = new string[] { };
            _istestsuccess = _test._testunitloremipsum(_args);
        }
    }
}