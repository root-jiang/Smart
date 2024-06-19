namespace FaultTreeAnalysis
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
         this.ftaControl1 = new FaultTreeAnalysis.FTAControl();
         this.SuspendLayout();
         // 
         // ftaControl1
         // 
         this.ftaControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.ftaControl1.Location = new System.Drawing.Point(0, 0);
         this.ftaControl1.Name = "ftaControl1";
         this.ftaControl1.Size = new System.Drawing.Size(1350, 729);
         this.ftaControl1.TabIndex = 0;
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1350, 729);
         this.Controls.Add(this.ftaControl1);
         this.KeyPreview = true;
         this.Name = "Form1";
         this.Text = "FTA Tree";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.ResumeLayout(false);
      }

      #endregion

      private FTAControl ftaControl1;
   }
}