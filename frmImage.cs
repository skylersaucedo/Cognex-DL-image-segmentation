using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TSI_DL_cognexIntegration
{
    
    public partial class frmImage : Form
    {
        public List<defect> _defects;

        public frmMain _frmMain;

        public Bitmap _cleanImage;

        public frmImage(List<defect> defects, Bitmap cleanImage, frmMain form)
        {
            InitializeComponent();

            _defects = defects;
            _frmMain = form;
            _cleanImage = cleanImage;

            if (_defects != null)
            {
                pictureBox1.Image = new Bitmap(_defects[0].filePath);
                txbxModelText.Text = makeOutputText(0);
                lblImgPath.Text = _defects[0].filePath;
                tBarResults.Maximum = _defects.Count;
                tBarResults.Minimum = 0;
            }

            else
            {
                MessageBox.Show("no defects detected.");
            }
            
        }

        public string makeOutputText(int i)
        {
            string outMessage = "";

            if (_defects != null)
            {
                // unpack and populate textbox

                outMessage = "------------Prediction Properties ------------" + System.Environment.NewLine;

                outMessage += string.Format("x: {0, -10}", _defects[i].x.ToString()) + System.Environment.NewLine;
                outMessage += string.Format("y: {0, -10}", _defects[i].y.ToString()) + System.Environment.NewLine;
                outMessage += string.Format("height: {0, -10}", _defects[i].height.ToString()) + System.Environment.NewLine;
                outMessage += string.Format("width: {0, -10}", _defects[i].width.ToString()) + System.Environment.NewLine;
                outMessage += string.Format("red score: {0, -10}", _defects[i].Redscore.ToString()) + System.Environment.NewLine;

                outMessage += "------------Best Prediction --------------" + System.Environment.NewLine;

                outMessage += string.Format("   Tag: {0, -10}", _defects[i].bestTagName) + System.Environment.NewLine;
                outMessage += string.Format("   score: {0, -10}", (_defects[i].bestTagScore * 100.0).ToString()) + System.Environment.NewLine;

                outMessage += "------------Tags and Scores --------------" + System.Environment.NewLine;

                for (int j = 0; j < _defects[i].tags.Count; j++)
                {
                    outMessage += string.Format(_defects[i].tags[j] + " , " + " " + (_defects[i].GreenScores[j] * 100.0).ToString()) + System.Environment.NewLine;
                }
            }

            return outMessage;
        }

        public Color assingColorToTag(string tag)
        {
            Color outColor = Color.White;

            switch(tag)
            {
                case "clean":
                    outColor = Color.White;
                    break;
                case "cracks":
                    outColor = Color.Blue;
                    break;

                case "dents":
                    outColor = Color.Orange;
                    break;
                case "edge":
                    outColor = Color.Purple;
                    break;

                case "pits":
                    outColor = Color.Gold;
                    break;

                case "scratch":
                    outColor = Color.Pink;
                    break;
            }

            return outColor;

        }

        public void tBarResults_Scroll(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(tBarResults.Value);

            if (i < _defects.Count)
            {
                lblNumber.Text = i.ToString();
                txbxModelText.Text = makeOutputText(i);
                lblImgPath.Text = _defects[i].filePath;

                Rectangle rect = new Rectangle();

                rect.X = Convert.ToInt32(_defects[i].x);
                rect.Y = Convert.ToInt32(_defects[i].y);
                rect.Width = 255;
                rect.Height = 255;

                Bitmap dummyImage = _cleanImage;

                Color colorID = assingColorToTag(_defects[i].bestTagName);

                // assign color bounding box to main form

                using (Graphics g = Graphics.FromImage(dummyImage))
                {
                    using (Pen pen = new Pen(new SolidBrush(colorID), 5))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }

                // change picture box outline

                Bitmap thumbnailImage = new Bitmap(_defects[i].filePath);


                using (Graphics g = Graphics.FromImage(thumbnailImage))
                {
                    using (Pen pen = new Pen(new SolidBrush(colorID), 5))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }

                pictureBox1.Image = thumbnailImage;
                _frmMain.pictureBoxShow.Image = dummyImage;

                _frmMain.pictureBoxShow.Refresh();
                pictureBox1.Refresh();
                txbxModelText.Refresh();

            }

            else
            {
                MessageBox.Show("value outside count of defects");
            }
        }
    }
}
