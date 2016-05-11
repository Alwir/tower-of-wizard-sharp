using System;
using System.Drawing;
using System.Windows.Forms;

namespace Find_Form
{
    public enum FindType {
        FIND,
        REPLACE
    };
    public partial class FindForm : Form
	{
		RichTextBox _richTextBox;
		
		bool _close;


		
		public FindForm(RichTextBox richTextBox)
		{
			InitializeComponent();
			
			_richTextBox=richTextBox;
		}
		
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if(!_close)
			{
				// hide instead of close
				this.Hide();
				e.Cancel=true;
			}
			
//			base.OnFormClosing(e);
		}
		public void ForceClose()
		{
			_close=true;
			this.Close();
		}
		
		public void ShowFind(FindType replaceMode)
		{

			this.Text=(replaceMode==FindType.REPLACE) ? "Replace":"Find";

            pnlReplace.Visible=(replaceMode==FindType.REPLACE) ? true : false;

            if(!this.Visible)
				this.Show(_richTextBox);
			// resize form
			this.ClientSize = new Size(this.ClientSize.Width,pnlOptions.Bottom);
			
			txtFindText.Focus();
			txtFindText.SelectAll();
		}
		
		void BtnFindNextClick(object sender, EventArgs e)
		{
			Find(_richTextBox, txtFindText.Text,chkMatchCase.Checked,chkMatchWholeWord.Checked,radDirectionUp.Checked);
		}
		void Find(RichTextBox richText, string text, bool matchCase, bool matchWholeWord, bool upDirection)
		{
			RichTextBoxFinds options=RichTextBoxFinds.None;
			if(matchCase)
				options|= RichTextBoxFinds.MatchCase;
			if(matchWholeWord)
				options|=RichTextBoxFinds.WholeWord;
			if(upDirection)
				options|=RichTextBoxFinds.Reverse;
			
			int index;
			if(upDirection)
				index = richText.Find(text,0,richText.SelectionStart,options);
			else
				index = richText.Find(text,richText.SelectionStart+richText.SelectionLength,options);
			
			if(index>=0)
			{
				richText.SelectionStart=index;
				richText.SelectionLength=text.Length;
			}
			else // text not found
			{
				MessageBox.Show(Application.ProductName + " has finished searching the document.",
				                Application.ProductName,MessageBoxButtons.OK,
				                MessageBoxIcon.Information);
			}
		}
		
		void BtnReplaceClick(object sender, EventArgs e)
		{
            if (_richTextBox.SelectedText!="") {
                _richTextBox.SelectedText=txtReplace.Text;
            }
			
		}
		
		void BtnReplaceAllClick(object sender, EventArgs e)
		{
			_richTextBox.Text=_richTextBox.Text.Replace(txtFindText.Text,txtReplace.Text);
		}
	}
}
