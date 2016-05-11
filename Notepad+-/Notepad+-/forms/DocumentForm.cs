using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace Notepad__ {
    public partial class DocumentForm : Form {
        Form1 main;

        public DocumentForm(Form1 main) {
            InitializeComponent();
            richTextBox1.Top=0;
            richTextBox1.Left=0;
            richTextBox1.Width=this.Width-15;
            richTextBox1.Height=this.Height-40;
            this.main=main;
            SetScheme();
            Localization();
        }

        private void Localization() {
            ResourceManager mng;
            if(main.language==Language.English) {
                mng=resources.ResourceEng.ResourceManager;
            } else {
                mng=resources.ResourceRus.ResourceManager;
            }
            this.Text=mng.GetString("NewDocument");

        }

        private void DocumentForm_ResizeEnd(object sender, EventArgs e) {
            richTextBox1.Top=0;
            richTextBox1.Left=0;
            richTextBox1.Width=this.Width-15;
            richTextBox1.Height=this.Height-40;
        }

        public void SetScheme() {
            Form1.UseScheme(this, main.scheme);
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e) {
            int cursorPosition = richTextBox1.SelectionStart+richTextBox1.SelectedText.Length ;

            int lineNumber = richTextBox1.GetLineFromCharIndex(cursorPosition);
            int firstPosition = richTextBox1.GetFirstCharIndexFromLine(lineNumber);
            int myPosition = cursorPosition-firstPosition;

            main.statusLabel.Text=string.Format(main.formatStatus, lineNumber, myPosition);
        }
    }
}
