using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ViDi2;
using ViDi2.Local;
using ViDi2.Training.UI;
using System.Reflection;

namespace TSI_DL_cognexIntegration
{
    public partial class frmMain : Form
    {

        public Bitmap cleanImage { get; set; }

        public string imagePath { get; set; }
        public string redModelPath { get; set; }
        public string greenModelPath { get; set; }

        public string basePath { get; set; }

        public string tempDLPath { get; set; }


        public frmMain()
        {
            InitializeComponent();

            Directory.SetCurrentDirectory(@"..\..\..\");
            basePath = Environment.CurrentDirectory;

            redModelPath = basePath + "\\models\\" + "analyze-oct-14.vrws";
            greenModelPath = basePath + "\\models\\" + "green-classify-256x256-oct-15.vrws";
            imagePath = basePath + "\\images\\" + "220920140046_dents_scratches.bmp";
            tempDLPath = basePath + "\\temp\\";

        }

        public void btnFinalPred_Click(object sender, EventArgs e)
        {

            var watch = new System.Diagnostics.Stopwatch(); // benchmark time
            watch.Start();

            Cursor.Current = Cursors.WaitCursor;

            ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred, new List<int>() { });

            // ------- PRODUCTION, RED FIRST -------------

            using (control)
            {

                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });

                ViDi2.Runtime.IWorkspace red_workspace = control.Workspaces.Add("red workspace", redModelPath);

                IStream stream_r = red_workspace.Streams["default"];

                MarkingOverlayExtensions.SetupOverlayEnvironment();

                int totalImageCount = 0;
                List<string[]> outVals = new List<string[]>();

                List<defect> redDefects = new List<defect>();

                Bitmap inputImage = (Bitmap)Image.FromFile(@imagePath);

                using (IImage image = new ViDi2.Local.LibraryImage(imagePath)) //disposing the image when we do not need it anymore
                {
                    // Allocates a sample with the image
                    using (ISample sample = stream_r.CreateSample(image))
                    {
                        ITool redTool = stream_r.Tools["Analyze"];

                        // Process the image by the tool. All upstream tools are also processed
                        sample.Process(redTool);

                        IRedMarking redMarking = sample.Markings[redTool.Name] as IRedMarking;

                        foreach (IRedView view in redMarking.Views)
                        {
                            foreach (IRegion region in view.Regions)
                            {

                                // each region has Center(x,y) height,width, score and filepath to thumbnail.
                                
                                double s = 256; // size of thumbnail side

                                string outpath = tempDLPath + totalImageCount.ToString() + ".bmp";

                                defect defect = new defect();

                                defect.x = region.Center.X;
                                defect.y = region.Center.Y;
                                defect.width = region.Width;
                                defect.height = region.Height;
                                defect.Redscore = region.Score;
                                defect.filePath = outpath;

                                // create thumbnail, isolate image to pass to classifier

                                var rect = new System.Windows.Rect(
                                    Convert.ToInt32(defect.x),
                                    Convert.ToInt32(defect.y),
                                        (int) s,
                                        (int) s);

                                CreateDefectThumbnail(inputImage, rect, outpath);

                                redDefects.Add(defect);

                                totalImageCount++;

                            }

                            // create and show heatmap
                            var heatmap = view.OverlayImage();
                            var bitmap = heatmap.Bitmap;

                            pictureBoxShow.Image = bitmap;

                            cleanImage = bitmap;
                        }
                    }
                }

                // ------- PRODUCTION, GREEN MODEL NEXT -------------

                List<defect> greenDefects = new List<defect>();

                ViDi2.Runtime.IWorkspace green_workspace = control.Workspaces.Add("green workspace", greenModelPath);

                IStream stream_g = green_workspace.Streams["default"];

                ITool greenTool = stream_g.Tools["Classify"];

                for (int i = 0; i < redDefects.Count; i++)
                {
                    string imgPath = redDefects[i].filePath;

                    // Load an image from file
                    using (IImage image = new LibraryImage(imgPath)) //disposing the image when we do not need it anymore
                    {
                        // Allocates a sample with the image
                        using (ISample sample = stream_g.CreateSample(image))
                        {
                            // Process the image by the tool. All upstream tools are also processed
                            sample.Process(greenTool);

                            IGreenMarking greenMarking = sample.Markings[greenTool.Name] as IGreenMarking;

                            foreach (IGreenView view in greenMarking.Views)
                            {
                                // collect all classification tags and scores 
                                var tags = view.Tags;

                                List<string> outTags = new List<string>();
                                List<double> outScores = new List<double>();

                                foreach (var tag in view.Tags)
                                {
                                    outTags.Add(tag.Name);
                                    outScores.Add(tag.Score);
                                }

                                // store in defect object

                                defect defect = new defect();

                                defect.x = redDefects[i].x;
                                defect.y = redDefects[i].y;
                                defect.height = redDefects[i].height;
                                defect.width = redDefects[i].width;
                                defect.bestTagName = view.BestTag.Name;
                                defect.bestTagScore = view.BestTag.Score;
                                defect.filePath = redDefects[i].filePath;
                                defect.tags = outTags;
                                defect.GreenScores = outScores;
                                defect.Redscore = redDefects[i].Redscore;

                                greenDefects.Add(defect); // add to list of defects
                            }
                        }
                    }
                }

                // view results

                var predictionResults = new frmImage(greenDefects, cleanImage, this);
                predictionResults.Show();

            }

            watch.Stop();
            Cursor.Current = Cursors.Default;

            double totalTime = watch.ElapsedMilliseconds / 1000.0;

            MessageBox.Show("total prediction time in seconds: " + Convert.ToString(totalTime));
        }

        public void btnLoadImage_Click(object sender, EventArgs e)
        {
            // open file dialog
            OpenFileDialog open = new OpenFileDialog();

            // apply image only filter
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                pictureBoxShow.Image = new Bitmap(open.FileName);
                // image file path  
                txbxImagePath.Text = open.FileName;
                imagePath = open.FileName;
                //pictureBoxShow.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        public static void CreateDefectThumbnail(Bitmap bitmap, System.Windows.Rect location, string outpath)
        {
            using (Bitmap thumbnail = new Bitmap((int)location.Width, (int)location.Height, bitmap.PixelFormat))
            {
                thumbnail.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    // Save a little larger area than exact defect for thumbnail.
                    var x = location.X - 10;
                    x = Math.Max(0, x);
                    var y = location.Y - 10;
                    y = Math.Max(0, y);
                    var w = location.Width + 20;
                    w = Math.Min(bitmap.Width, w);
                    var h = location.Height + 20;
                    h = Math.Min(bitmap.Height, h);
                    g.DrawImage(bitmap, 0, 0, new RectangleF((float)location.X, (float)location.Y, (float)location.Width, (float)location.Height), GraphicsUnit.Pixel);
                }
                thumbnail.Save(outpath);
            }
        }

        public void cxbxResize_CheckedChanged(object sender, EventArgs e)
        {
            // use this to rescale image

            if (cxbxResize.Checked == true)
            {
                panel1.AutoScroll = true;
                pictureBoxShow.SizeMode = PictureBoxSizeMode.AutoSize;
            }

            if (cxbxResize.Checked == false)
            {
                panel1.AutoScroll = false;
                pictureBoxShow.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
