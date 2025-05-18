namespace AnimeTaste
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainBlazorWebView = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            SuspendLayout();
            // 
            // mainBlazorWebView
            // 
            mainBlazorWebView.Dock = DockStyle.Fill;
            mainBlazorWebView.Location = new Point(0, 0);
            mainBlazorWebView.Name = "mainBlazorWebView";
            mainBlazorWebView.Size = new Size(2208, 1100);
            mainBlazorWebView.TabIndex = 0;
            mainBlazorWebView.Text = "blazorWebView1";
            mainBlazorWebView.Click += mainBlazorWebView_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2208, 1100);
            Controls.Add(mainBlazorWebView);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AnimeTaste";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView mainBlazorWebView;
    }
}
