using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetypetest {
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form {
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
        private TextBox textBox1;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1() {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(8, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(560, 464);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(535, 428);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 19);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "18";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(656, 509);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		Bitmap bmp = new Bitmap(640, 480);
		int pen_x = 0;
		int pen_y = 100;

		private void Draw(int x, int y, int level) {
			bmp.SetPixel(x+pen_x, y+pen_y, Color.FromArgb(level, Color.White));
		}

		private void button1_Click(object sender, System.EventArgs e) {
			System.IO.Stream stream = new System.IO.FileStream(@"D:\Repository\clios\resource\ipagui.ttf", System.IO.FileMode.Open, System.IO.FileAccess.Read);
			byte[] buf = new byte[stream.Length];
			stream.Read(buf, 0, buf.Length);
			stream.Close();
			//*
			FreeType.FontData fontdata = new FreeType.FontData(buf);
			fontdata.SetPainter(new FreeType.FontDrawHandler(Draw));
			fontdata.SetSizeByPixel(0, int.Parse(this.textBox1.Text));
			string text = "The pure cli operating system, CooS.";
			foreach(char ch in text) {
				fontdata.LoadGlyph(ch);
                this.pen_y -= fontdata.BearingSize.Height;
				fontdata.DrawGlyph();
                this.pen_y += fontdata.BearingSize.Height;
                this.pen_x += fontdata.AdvanceSize.Width >> 6;
			}
			//*/
			this.pictureBox1.Image = bmp;
			this.pictureBox1.Refresh();
			this.pen_x = 0;
			this.pen_y += 30;
		}
	}
}
