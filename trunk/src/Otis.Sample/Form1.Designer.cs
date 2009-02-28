namespace Otis.Sample
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.cboUsers = new System.Windows.Forms.ComboBox();
			this.grdRecords = new System.Windows.Forms.DataGridView();
			this.colTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSongCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAvgSongRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCommentCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAvgSongDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize) (this.grdRecords)).BeginInit();
			this.SuspendLayout();
			// 
			// cboUsers
			// 
			this.cboUsers.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cboUsers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboUsers.FormattingEnabled = true;
			this.cboUsers.Location = new System.Drawing.Point(12, 12);
			this.cboUsers.Name = "cboUsers";
			this.cboUsers.Size = new System.Drawing.Size(681, 21);
			this.cboUsers.TabIndex = 0;
			this.cboUsers.SelectedIndexChanged += new System.EventHandler(this.cboUsers_SelectedIndexChanged);
			// 
			// grdRecords
			// 
			this.grdRecords.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grdRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.grdRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTitle,
            this.colCategory,
            this.colSongCount,
            this.colAvgSongRating,
            this.colCommentCount,
            this.colDuration,
            this.colAvgSongDuration});
			this.grdRecords.Location = new System.Drawing.Point(13, 45);
			this.grdRecords.Name = "grdRecords";
			this.grdRecords.ReadOnly = true;
			this.grdRecords.Size = new System.Drawing.Size(679, 314);
			this.grdRecords.TabIndex = 1;
			// 
			// colTitle
			// 
			this.colTitle.DataPropertyName = "Description";
			this.colTitle.FillWeight = 200F;
			this.colTitle.HeaderText = "Album Title";
			this.colTitle.Name = "colTitle";
			this.colTitle.ReadOnly = true;
			// 
			// colCategory
			// 
			this.colCategory.DataPropertyName = "Category";
			this.colCategory.FillWeight = 125F;
			this.colCategory.HeaderText = "Category";
			this.colCategory.Name = "colCategory";
			this.colCategory.ReadOnly = true;
			// 
			// colSongCount
			// 
			this.colSongCount.DataPropertyName = "SongCount";
			this.colSongCount.FillWeight = 75F;
			this.colSongCount.HeaderText = "Song Count";
			this.colSongCount.Name = "colSongCount";
			this.colSongCount.ReadOnly = true;
			// 
			// colAvgSongRating
			// 
			this.colAvgSongRating.DataPropertyName = "AverageSongRating";
			dataGridViewCellStyle1.Format = "N2";
			this.colAvgSongRating.DefaultCellStyle = dataGridViewCellStyle1;
			this.colAvgSongRating.HeaderText = "Average song rating";
			this.colAvgSongRating.Name = "colAvgSongRating";
			this.colAvgSongRating.ReadOnly = true;
			// 
			// colCommentCount
			// 
			this.colCommentCount.DataPropertyName = "TotalCommentCount";
			this.colCommentCount.FillWeight = 50F;
			this.colCommentCount.HeaderText = "No. of comments";
			this.colCommentCount.Name = "colCommentCount";
			this.colCommentCount.ReadOnly = true;
			// 
			// colDuration
			// 
			this.colDuration.DataPropertyName = "AlbumDurationText";
			this.colDuration.HeaderText = "Album Duration";
			this.colDuration.Name = "colDuration";
			this.colDuration.ReadOnly = true;
			// 
			// colAvgSongDuration
			// 
			this.colAvgSongDuration.DataPropertyName = "AverageSongDuration";
			this.colAvgSongDuration.FillWeight = 125F;
			this.colAvgSongDuration.HeaderText = "Average song duration";
			this.colAvgSongDuration.Name = "colAvgSongDuration";
			this.colAvgSongDuration.ReadOnly = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(705, 370);
			this.Controls.Add(this.grdRecords);
			this.Controls.Add(this.cboUsers);
			this.MinimumSize = new System.Drawing.Size(459, 404);
			this.Name = "Form1";
			this.Text = "Records";
			((System.ComponentModel.ISupportInitialize) (this.grdRecords)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cboUsers;
		private System.Windows.Forms.DataGridView grdRecords;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTitle;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
		private System.Windows.Forms.DataGridViewTextBoxColumn colSongCount;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAvgSongRating;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCommentCount;
		private System.Windows.Forms.DataGridViewTextBoxColumn colDuration;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAvgSongDuration;
	}
}

