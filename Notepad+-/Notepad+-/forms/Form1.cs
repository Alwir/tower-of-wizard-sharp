/*
задание с подсветкой это полный перебор на нашем уровне.
для того чтобы ее сделать в реалтайм режиме, надо запускать в отдельном потоке
а мы этого еще не учили

но это пол беды, реализовал что по нажатию кнопки подсвечивает синтаксис
(набор слов не полный, чисто для примера небольшой набор слов в переменной allOperators)

главная беда, что реализовать подсветку ИМЕН ПЕРЕМЕННЫХ в рамках этой работы нереально
я смотрел компоненты с подсветкой занимают от 5 тысяч до 50 тысяч строк программы, как то перебор для
экзамена по винформам

в простейшем варианте в приницпе и это реализовал, но в простейшем варианте!
(берется следующее слово, после ключевых обозначений переменных)

можно было конечно взять просто напросто самодельный компонент с реализованной подсветкой синтаксиса, 
но думаю это было бы (опять же в рамках данного экзамена) не совсем правильно.

*/


using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Find_Form;
using System.Resources;

namespace Notepad__ {
    public enum Language {
        English,
        Russian
    };

    public enum Scheme {
        White,
        Dark
    };

    public partial class Form1 : Form {
        DocumentForm activeDocument=null;
        FindForm findForm;
        public Language language = Language.English;
        public Scheme scheme = Scheme.White;
        public string formatStatus;

        string[] allOperators = { "while", "for", "if", "using", "public", "partial", "class","private","void","return"};
        string[] allVars = {"var","int","string","auto","double"};

        public Form1() {
            InitializeComponent();
            SetLocalization();
            UseScheme(this,this.scheme);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if(findForm!=null) {
                findForm.ForceClose();
            }
            e.Cancel=false;
            base.OnClosing(e);
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e) {
            statusBarToolStripMenuItem.Checked=!statusBarToolStripMenuItem.Checked;
            statusStrip1.Visible=!statusStrip1.Visible;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutForm form = new AboutForm(this);
            form.ShowDialog(this);
        }

        #region FILE
        private void createToolStripMenuItem_Click(object sender, EventArgs e) {
            CreateNewDocument();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if(openFileDialog1.ShowDialog()==DialogResult.Cancel) {
                return;
            }
            CreateNewDocument();
            try {
                activeDocument.richTextBox1.Lines=File.ReadAllLines(openFileDialog1.FileName);
                activeDocument.Tag=openFileDialog1.FileName;
                activeDocument.Text=openFileDialog1.FileName;
            } catch(Exception exc) {
                CloseDocument();
                MessageBox.Show(exc.Message);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument.Tag==null) {
                SaveFileAs();
                return;
            }
            File.WriteAllLines(activeDocument.Tag.ToString(), activeDocument.richTextBox1.Lines);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileAs();
        }

        private void SaveFileAs() {
            if(activeDocument==null) {
                return;
            }
            if(saveFileDialog1.ShowDialog()==DialogResult.Cancel) {
                return;
            }
            File.WriteAllLines(saveFileDialog1.FileName, activeDocument.richTextBox1.Lines);
            activeDocument.Tag=saveFileDialog1.FileName;
            activeDocument.Text=saveFileDialog1.FileName;
        }
        private void CreateNewDocument() {
            DocumentForm document = new DocumentForm(this);
            document.MdiParent=this;
            document.Show();
            activeDocument=document;
        }

        private void CloseDocument() {
            if(activeDocument==null) {
                return;
            }
            activeDocument.Dispose();
        }

        private void Form1_MdiChildActivate(object sender, EventArgs e) {
            foreach(DocumentForm docs in this.MdiChildren) {
                if(docs.ContainsFocus) {
                    activeDocument=docs;
                    break;
                }
            }
        }
        #endregion

        #region EDIT
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            if (activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.Redo();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.Paste();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.Copy();
        }

        private void insertToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.Cut();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.SelectedText="";
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.SelectAll();
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.DeselectAll();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e) {
            FindReplace(FindType.FIND);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e) {
            FindReplace(FindType.REPLACE);
        }

        private void FindReplace(FindType type) {
            if(activeDocument==null) {
                return;
            }
            if(findForm!=null) {
                findForm.ForceClose();
            }
            activeDocument.richTextBox1.DeselectAll();
            findForm=new FindForm(activeDocument.richTextBox1);

            findForm.ShowFind(type);
        }

        #endregion

        #region VIEW
        private void wordWarpToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            activeDocument.richTextBox1.WordWrap=!activeDocument.richTextBox1.WordWrap;
            wordWarpToolStripMenuItem.Checked=!wordWarpToolStripMenuItem.Checked;
        }
        private void SetLocalization() {
            ResourceManager mng;
            if(language==Language.English) {
                mng=resources.ResourceEng.ResourceManager;
            } else {
                mng=resources.ResourceRus.ResourceManager;
            }

            toolStripButton16.Text=mng.GetString("UnCommentToolTips");
            toolStripButton15.Text=mng.GetString("CommentToolTips");
            toolStripButton14.Text=mng.GetString("AboutMenuItemString");
            toolStripButton13.Text=mng.GetString("ViewWordWarpMenuItemString");
            toolStripButton12.Text=mng.GetString("ViewStatusMenuItemString");
            toolStripButton11.Text=mng.GetString("FormatFontMenuItemString");
            toolStripButton10.Text=mng.GetString("FormatBackColorMenuItemString");
            toolStripButton9.Text=mng.GetString("EditDeleteMenuItemString");
            toolStripButton8.Text=mng.GetString("EditCutMenuItemString");
            toolStripButton7.Text=mng.GetString("EditCopyMenuItemString");
            toolStripButton6.Text=mng.GetString("EditPasteMenuItemString");
            toolStripButton5.Text=mng.GetString("EditRedoMenuItemString");
            toolStripButton4.Text=mng.GetString("EditUndoMenuItemString");
            toolStripButton3.Text=mng.GetString("FileSaveMenuItemString");
            toolStripButton2.Text=mng.GetString("FileOpenMenuItemString");
            toolStripButton1.Text=mng.GetString("FileCreateMenuItemString");

            aboutToolStripMenuItem.Text=mng.GetString("AboutMenuItemString");


            cSintaxisToolStripMenuItem.Text=mng.GetString("ViewCMenuItemString");
            englishToolStripMenuItem.Text=mng.GetString("ViewEnglishMenuItemString");
            russianToolStripMenuItem.Text=mng.GetString("ViewRussianMenuItemString");
            whiteSchemeToolStripMenuItem.Text=mng.GetString("ViewWhiteSchemeMenuItemString");
            darkSchemeToolStripMenuItem.Text=mng.GetString("ViewDarkSchemeMenuItemString");
            wordWarpToolStripMenuItem.Text=mng.GetString("ViewWordWarpMenuItemString");
            statusBarToolStripMenuItem.Text=mng.GetString("ViewStatusMenuItemString");
            viewToolStripMenuItem.Text=mng.GetString("ViewMenuItemString");

            replaceToolStripMenuItem.Text=mng.GetString("EditReplaceMenuItemString");
            findToolStripMenuItem.Text=mng.GetString("EditFindMenuItemString");
            deselectAllToolStripMenuItem.Text=mng.GetString("EditDeselectMenuItemString");
            selectAllToolStripMenuItem.Text=mng.GetString("EditSelectMenuItemString");
            deleteToolStripMenuItem.Text=mng.GetString("EditDeleteMenuItemString");
            insertToolStripMenuItem.Text=mng.GetString("EditCutMenuItemString");
            copyToolStripMenuItem.Text=mng.GetString("EditCopyMenuItemString");
            pasteToolStripMenuItem.Text=mng.GetString("EditPasteMenuItemString");
            redoToolStripMenuItem.Text=mng.GetString("EditRedoMenuItemString");
            undoToolStripMenuItem.Text=mng.GetString("EditUndoMenuItemString");
            editToolStripMenuItem.Text=mng.GetString("EditMenuItemString");

            formatToolStripMenuItem.Text=mng.GetString("FormatMenuItemString");
            backGroundColorToolStripMenuItem.Text=mng.GetString("FormatBackColorMenuItemString");
            fontToolStripMenuItem.Text=mng.GetString("FormatFontMenuItemString");

            fileToolStripMenuItem.Text=mng.GetString("FileMenuItemString");
            createToolStripMenuItem.Text=mng.GetString("FileCreateMenuItemString");
            openToolStripMenuItem.Text=mng.GetString("FileOpenMenuItemString");
            exitToolStripMenuItem.Text=mng.GetString("FileExitMenuItemString");
            saveToolStripMenuItem.Text=mng.GetString("FileSaveMenuItemString");
            saveAsToolStripMenuItem.Text=mng.GetString("FileSaveAsMenuItemString");

            formatStatus=mng.GetString("FormatStatus");

            statusLabel.Text=string.Format(formatStatus, 0, 0);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e) {
            SetLanguage(Language.English);
        }

        private void russianToolStripMenuItem_Click(object sender, EventArgs e) {
            SetLanguage(Language.Russian);
        }

        private void SetLanguage(Language language) {
            englishToolStripMenuItem.Checked=!englishToolStripMenuItem.Checked;
            russianToolStripMenuItem.Checked=!russianToolStripMenuItem.Checked;
            this.language=language;
            SetLocalization();
        }

        private void whiteSchemeToolStripMenuItem_Click(object sender, EventArgs e) {
            SetScheme(Scheme.White);
        }

        private void darkSchemeToolStripMenuItem_Click(object sender, EventArgs e) {
            SetScheme(Scheme.Dark);
        }

        private void SetScheme(Scheme scheme) {
            darkSchemeToolStripMenuItem.Checked=!darkSchemeToolStripMenuItem.Checked;
            whiteSchemeToolStripMenuItem.Checked=!whiteSchemeToolStripMenuItem.Checked;
            this.scheme=scheme;
            UseScheme(this, this.scheme);

            foreach(DocumentForm form in this.MdiChildren) {
                form.SetScheme();
            }
        }

        public static void UseScheme(Form form, Scheme scheme) {
            Color bg = Color.White;
            Color fg = Color.Black;

            if(scheme==Scheme.Dark) {
                bg=Color.Black;
                fg=Color.White;
            }

            foreach(Control contr in form.Controls) {

                contr.BackColor=bg;
                contr.ForeColor=fg;
            }
        }

        #endregion

        #region FORMAT
        private void backGroundColorToolStripMenuItem_Click(object sender, EventArgs e) {
            if (colorDialog1.ShowDialog()==DialogResult.Cancel) {
                return;
            }
            foreach (Control contr in this.Controls) {
                contr.BackColor=colorDialog1.Color;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            if (fontDialog1.ShowDialog()==DialogResult.Cancel) {
                return;
            }

            activeDocument.richTextBox1.Font=fontDialog1.Font;
        }

        #endregion

        #region COMMENT
        /*
         коммент и анкоммент можно сделать другим путем
         высчитать линии с помощью GetLineFromCharIndex(cursorPosition);
         и потом добавить или убрать пометки в начале строк, примерно по такому принципу работает в VS

         но лично мне кажется что лучше дать возможность комменить и с середины строки
        */
        private void toolStripButton15_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            if(activeDocument.richTextBox1.SelectedText.Equals("")) {
                return;
            }

            string[] allText = activeDocument.richTextBox1.SelectedText.Split('\n');

            string newText = "";

            for (int i = 0;i<allText.Length-1;i++) {
                newText+="//"+allText[i]+"\n";
            }
            newText+="//"+allText[allText.Length-1];

            activeDocument.richTextBox1.SelectedText=newText;
        }

        private void toolStripButton16_Click(object sender, EventArgs e) {
            if(activeDocument==null) {
                return;
            }
            if(activeDocument.richTextBox1.SelectedText.Equals("")) {
                return;
            }

            string newText = activeDocument.richTextBox1.SelectedText;
            if (newText[0]=='/' && newText[1]=='/') {
                newText=newText.Remove(0, 2);
            }

            activeDocument.richTextBox1.SelectedText=newText.Replace("\n//", "\n");
        }

        #endregion

        private void cSintaxisToolStripMenuItem_Click(object sender, EventArgs e) {
            UseSintax();
        }

        private void UseSintax() {
            if(activeDocument==null) {
                return;
            }

            Font oldFont=activeDocument.richTextBox1.SelectionFont;
            Color oldColor = activeDocument.richTextBox1.SelectionColor;
            int selector = activeDocument.richTextBox1.SelectionStart;

            List<string> allVarText = new List<string>();

            List<string> allOperator = new List<string>();
            allOperator.AddRange(allOperators);
            allOperator.AddRange(allVars);

            for(int q = 0;q<allOperator.Count;q++) {
                for(int i = 0;i<activeDocument.richTextBox1.Text.Length;i++) {
                    int start = activeDocument.richTextBox1.Text.IndexOf(allOperator[q], i);
                    if(start>=0) {
                        if(allVars.Contains(allOperator[q])) {
                            string text = activeDocument.richTextBox1.Text.Substring(start);
                            string varText = text.Split(' ')[1];
                            allVarText.Add(varText);
                        }

                        activeDocument.richTextBox1.Select(start, allOperator[q].Length);
                        activeDocument.richTextBox1.SelectionColor=Color.Blue;
                        activeDocument.richTextBox1.SelectionFont=new Font("Arial", 10, FontStyle.Bold);
                        activeDocument.richTextBox1.SelectionLength=0;
                        activeDocument.richTextBox1.SelectionColor=oldColor;
                        activeDocument.richTextBox1.SelectionFont=oldFont;
                        i=start+allOperator[q].Length;
                    }
                }
            }

            for(int q = 0;q<allVarText.Count;q++) {
                for(int i = 0;i<activeDocument.richTextBox1.Text.Length;i++) {
                    int start = activeDocument.richTextBox1.Text.IndexOf(allVarText[q], i);
                    if(start>=0) {
                        activeDocument.richTextBox1.Select(start, allVarText[q].Length);
                        activeDocument.richTextBox1.SelectionColor=Color.Red;
                        activeDocument.richTextBox1.SelectionFont=new Font("Arial", 7, FontStyle.Italic);
                        activeDocument.richTextBox1.SelectionLength=0;
                        activeDocument.richTextBox1.SelectionColor=oldColor;
                        activeDocument.richTextBox1.SelectionFont=oldFont;
                        i=start+allVarText[q].Length;
                    }
                }
            }
            activeDocument.richTextBox1.SelectionStart=selector;
        }

        private void Form1_Load(object sender, EventArgs e) {
            CreateNewDocument();
            activeDocument.richTextBox1.Lines=File.ReadAllLines("test.txt");
            activeDocument.Tag="test.txt";
            activeDocument.Text="test.txt";
        }
    }
}
