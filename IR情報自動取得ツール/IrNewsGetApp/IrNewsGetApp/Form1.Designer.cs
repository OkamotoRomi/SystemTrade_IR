namespace IrNewsGetApp
{
    partial class Form1
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
            this.readFileNameText = new System.Windows.Forms.TextBox();
            this.getBtn = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.codeDatailLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // readFileNameText
            // 
            this.readFileNameText.Location = new System.Drawing.Point(12, 50);
            this.readFileNameText.Name = "readFileNameText";
            this.readFileNameText.ReadOnly = true;
            this.readFileNameText.Size = new System.Drawing.Size(780, 19);
            this.readFileNameText.TabIndex = 0;
            // 
            // getBtn
            // 
            this.getBtn.Location = new System.Drawing.Point(12, 12);
            this.getBtn.Name = "getBtn";
            this.getBtn.Size = new System.Drawing.Size(123, 23);
            this.getBtn.TabIndex = 1;
            this.getBtn.Text = "IR情報取得";
            this.getBtn.UseVisualStyleBackColor = true;
            this.getBtn.Click += new System.EventHandler(this.irBtl);
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dataGridView.Location = new System.Drawing.Point(12, 91);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.Size = new System.Drawing.Size(709, 150);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cell_doubleclick);
            // 
            // codeDatailLabel
            // 
            this.codeDatailLabel.AutoSize = true;
            this.codeDatailLabel.Location = new System.Drawing.Point(10, 259);
            this.codeDatailLabel.Name = "codeDatailLabel";
            this.codeDatailLabel.Size = new System.Drawing.Size(29, 12);
            this.codeDatailLabel.TabIndex = 3;
            this.codeDatailLabel.Text = "解析";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.codeDatailLabel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.getBtn);
            this.Controls.Add(this.readFileNameText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "IR情報自動取得ツール";
            this.Load += new System.EventHandler(this.scrennLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox readFileNameText;
        private System.Windows.Forms.Button getBtn;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label codeDatailLabel;
    }
}

