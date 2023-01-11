namespace AutoDataUpdate
{
    partial class mainscreen
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.execbtn = new System.Windows.Forms.Button();
            this.accessurlstr = new System.Windows.Forms.TextBox();
            this.URL_Label = new System.Windows.Forms.Label();
            this.info1 = new System.Windows.Forms.Label();
            this.last_update_str = new System.Windows.Forms.Label();
            this.lastUpdateText = new System.Windows.Forms.TextBox();
            this.savepath = new System.Windows.Forms.TextBox();
            this.savepatlabel = new System.Windows.Forms.Label();
            this.savebtn = new System.Windows.Forms.Button();
            this.stopDateInput = new System.Windows.Forms.TextBox();
            this.stopDayCountLabel = new System.Windows.Forms.Label();
            this.deletebtn = new System.Windows.Forms.Button();
            this.replyUpdateBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // execbtn
            // 
            this.execbtn.Location = new System.Drawing.Point(657, 49);
            this.execbtn.Name = "execbtn";
            this.execbtn.Size = new System.Drawing.Size(106, 31);
            this.execbtn.TabIndex = 0;
            this.execbtn.Text = "最新データに更新";
            this.execbtn.UseVisualStyleBackColor = true;
            this.execbtn.Click += new System.EventHandler(this.execbtnClick);
            // 
            // accessurlstr
            // 
            this.accessurlstr.Location = new System.Drawing.Point(43, 114);
            this.accessurlstr.Multiline = true;
            this.accessurlstr.Name = "accessurlstr";
            this.accessurlstr.ReadOnly = true;
            this.accessurlstr.Size = new System.Drawing.Size(720, 19);
            this.accessurlstr.TabIndex = 2;
            this.accessurlstr.Text = "https://www.taisyaku.jp/sys-list/data/other.xlsx";
            this.accessurlstr.UseSystemPasswordChar = true;
            // 
            // URL_Label
            // 
            this.URL_Label.AutoSize = true;
            this.URL_Label.Location = new System.Drawing.Point(41, 87);
            this.URL_Label.Name = "URL_Label";
            this.URL_Label.Size = new System.Drawing.Size(310, 12);
            this.URL_Label.TabIndex = 3;
            this.URL_Label.Text = "売禁データ取得先URL（日本取引所JPX） ※自動入力されます";
            // 
            // info1
            // 
            this.info1.AutoSize = true;
            this.info1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info1.ForeColor = System.Drawing.Color.Red;
            this.info1.Location = new System.Drawing.Point(553, 25);
            this.info1.Name = "info1";
            this.info1.Size = new System.Drawing.Size(210, 14);
            this.info1.TabIndex = 4;
            this.info1.Text = "※当日データは16:30以降に更新されます。";
            // 
            // last_update_str
            // 
            this.last_update_str.AutoSize = true;
            this.last_update_str.Location = new System.Drawing.Point(41, 27);
            this.last_update_str.Name = "last_update_str";
            this.last_update_str.Size = new System.Drawing.Size(223, 12);
            this.last_update_str.TabIndex = 5;
            this.last_update_str.Text = "売禁データ最終更新日　※自動入力されます";
            // 
            // lastUpdateText
            // 
            this.lastUpdateText.Location = new System.Drawing.Point(43, 55);
            this.lastUpdateText.Name = "lastUpdateText";
            this.lastUpdateText.ReadOnly = true;
            this.lastUpdateText.Size = new System.Drawing.Size(73, 19);
            this.lastUpdateText.TabIndex = 6;
            this.lastUpdateText.Text = "----/--/--";
            // 
            // savepath
            // 
            this.savepath.Location = new System.Drawing.Point(43, 179);
            this.savepath.Name = "savepath";
            this.savepath.ReadOnly = true;
            this.savepath.Size = new System.Drawing.Size(706, 19);
            this.savepath.TabIndex = 7;
            // 
            // savepatlabel
            // 
            this.savepatlabel.AutoSize = true;
            this.savepatlabel.Location = new System.Drawing.Point(43, 161);
            this.savepatlabel.Name = "savepatlabel";
            this.savepatlabel.Size = new System.Drawing.Size(222, 12);
            this.savepatlabel.TabIndex = 8;
            this.savepatlabel.Text = "売禁データ格納場所（追記書き込みをします）";
            // 
            // savebtn
            // 
            this.savebtn.Location = new System.Drawing.Point(755, 179);
            this.savebtn.Name = "savebtn";
            this.savebtn.Size = new System.Drawing.Size(33, 19);
            this.savebtn.TabIndex = 9;
            this.savebtn.Text = "...";
            this.savebtn.UseVisualStyleBackColor = true;
            this.savebtn.Click += new System.EventHandler(this.savebtn_Click);
            // 
            // stopDateInput
            // 
            this.stopDateInput.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.stopDateInput.Location = new System.Drawing.Point(45, 241);
            this.stopDateInput.MaxLength = 3;
            this.stopDateInput.Name = "stopDateInput";
            this.stopDateInput.ShortcutsEnabled = false;
            this.stopDateInput.Size = new System.Drawing.Size(40, 19);
            this.stopDateInput.TabIndex = 10;
            this.stopDateInput.Text = "3";
            this.stopDateInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.stopDayCountKeyDown);
            this.stopDateInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.stopDayCountKey);
            // 
            // stopDayCountLabel
            // 
            this.stopDayCountLabel.AutoSize = true;
            this.stopDayCountLabel.Location = new System.Drawing.Point(43, 214);
            this.stopDayCountLabel.Name = "stopDayCountLabel";
            this.stopDayCountLabel.Size = new System.Drawing.Size(336, 12);
            this.stopDayCountLabel.TabIndex = 11;
            this.stopDayCountLabel.Text = "売禁停止日数　※本日対象銘柄を何日禁止にするか指定して下さい";
            // 
            // deletebtn
            // 
            this.deletebtn.Location = new System.Drawing.Point(657, 229);
            this.deletebtn.Name = "deletebtn";
            this.deletebtn.Size = new System.Drawing.Size(106, 31);
            this.deletebtn.TabIndex = 13;
            this.deletebtn.Text = "設定削除";
            this.deletebtn.UseVisualStyleBackColor = true;
            this.deletebtn.Click += new System.EventHandler(this.deleteBtnClick);
            // 
            // replyUpdateBtn
            // 
            this.replyUpdateBtn.Location = new System.Drawing.Point(531, 49);
            this.replyUpdateBtn.Name = "replyUpdateBtn";
            this.replyUpdateBtn.Size = new System.Drawing.Size(104, 31);
            this.replyUpdateBtn.TabIndex = 14;
            this.replyUpdateBtn.Text = "データ先再読込み";
            this.replyUpdateBtn.UseVisualStyleBackColor = true;
            this.replyUpdateBtn.Click += new System.EventHandler(this.dataReadBtn);
            // 
            // mainscreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 288);
            this.Controls.Add(this.replyUpdateBtn);
            this.Controls.Add(this.deletebtn);
            this.Controls.Add(this.stopDayCountLabel);
            this.Controls.Add(this.stopDateInput);
            this.Controls.Add(this.savebtn);
            this.Controls.Add(this.savepatlabel);
            this.Controls.Add(this.savepath);
            this.Controls.Add(this.lastUpdateText);
            this.Controls.Add(this.last_update_str);
            this.Controls.Add(this.info1);
            this.Controls.Add(this.URL_Label);
            this.Controls.Add(this.accessurlstr);
            this.Controls.Add(this.execbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "mainscreen";
            this.Text = "売禁ファイル自動更新";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.mainscreen_FormClosed);
            this.Load += new System.EventHandler(this.main_scrennload);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button execbtn;
        private System.Windows.Forms.TextBox accessurlstr;
        private System.Windows.Forms.Label URL_Label;
        private System.Windows.Forms.Label info1;
        private System.Windows.Forms.Label last_update_str;
        private System.Windows.Forms.TextBox lastUpdateText;
        private System.Windows.Forms.TextBox savepath;
        private System.Windows.Forms.Label savepatlabel;
        private System.Windows.Forms.Button savebtn;
        private System.Windows.Forms.TextBox stopDateInput;
        private System.Windows.Forms.Label stopDayCountLabel;
        private System.Windows.Forms.Button deletebtn;
        private System.Windows.Forms.Button replyUpdateBtn;
    }
}

