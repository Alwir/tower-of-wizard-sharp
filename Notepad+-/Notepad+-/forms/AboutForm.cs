using System.Windows.Forms;
using System.Resources;

namespace Notepad__ {
    public partial class AboutForm : Form {
        Form1 main;
        public AboutForm(Form1 main) {
            this.main=main;
            InitializeComponent();
            Localization();
            SetScheme();
        }

        private void Localization() {
            ResourceManager mng;
            if(main.language==Language.English) {
                mng=resources.ResourceEng.ResourceManager;
            } else {
                mng=resources.ResourceRus.ResourceManager;
            }
            richTextBox1.Text=mng.GetString("AboutBox");
            this.Text=mng.GetString("AboutName");
        }

        public void SetScheme() {
            Form1.UseScheme(this, main.scheme);
        }
    }
}
