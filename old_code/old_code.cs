//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Windows.Forms;
//using ViDi2;
//using ViDi2.Local;
//using ViDi2.Training.UI;
//using System.Reflection;

//namespace TSI_DL_cognexIntegration
//{
//    public partial class frmMain : Form
//    {

//        public Bitmap cleanImage { get; set; }

//        public string imagePath { get; set; }
//        public string redModelPath { get; set; }
//        public string greenModelPath { get; set; }

//        public string basePath { get; set; }

//        public string imagePathPreset { get; set; }
//        public string modelPathPreset { get; set; }
//        public string tempDL { get; set; }


//        public frmMain()
//        {
//            InitializeComponent();
//            Directory.SetCurrentDirectory(@"..\..\..\");
//            basePath = Environment.CurrentDirectory;

//            redModelPath = basePath + "\\models\\" + "analyze-oct-14.vrws";
//            greenModelPath = basePath + "\\models\\" + "green-classify-256x256-oct-15.vrws";
//            imagePath = basePath + "\\images\\" + "220920140046_dents_scratches.bmp";
//            tempDL = basePath + "\\temp\\";

//            imagePathPreset = imagePath;
//            modelPathPreset = redModelPath;

//        }

//        public void btnFinalPred_Click(object sender, EventArgs e)
//        {
//            // ------- PRODUCTION, RED FIRST -------------

//            var watch = new System.Diagnostics.Stopwatch(); // benchmark time


//            ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred, new List<int>() { });

//            using (control)
//            {

//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });

//                ViDi2.Runtime.IWorkspace red_workspace = control.Workspaces.Add("red workspace", redModelPath);

//                IStream stream_r = red_workspace.Streams["default"];

//                MarkingOverlayExtensions.SetupOverlayEnvironment();

//                int totalImageCount = 0;
//                List<string[]> outVals = new List<string[]>();

//                Bitmap inputImage = (Bitmap)Image.FromFile(@imagePath);

//                using (IImage image = new ViDi2.Local.LibraryImage(imagePath)) //disposing the image when we do not need it anymore
//                {
//                    // Allocates a sample with the image
//                    using (ISample sample = stream_r.CreateSample(image))
//                    {
//                        ITool redTool = stream_r.Tools["Analyze"];

//                        // Process the image by the tool. All upstream tools are also processed
//                        sample.Process(redTool);

//                        IRedMarking redMarking = sample.Markings[redTool.Name] as IRedMarking;

//                        foreach (IRedView view in redMarking.Views)
//                        {
//                            //System.Console.WriteLine($"This view has a score of {view.Score}");
//                            foreach (IRegion region in view.Regions)
//                            {
//                                Console.WriteLine($"This region has a score of {region.Score}");

//                                // each region has Center(x,y) and height,width.

//                                double s = 256;

//                                double x = region.Center.X;
//                                double y = region.Center.Y;
//                                double w = region.Width;
//                                double h = region.Height;

//                                double score = region.Score;

//                                // create thumbnail, isolate image to pass to classifier

//                                var rect = new System.Windows.Rect(
//                                    Convert.ToInt32(x),
//                                    Convert.ToInt32(y),
//                                        (int)s,
//                                        (int)s);

//                                string outpath = tempDL + totalImageCount.ToString() + ".bmp";

//                                CreateDefectThumbnail(inputImage, rect, outpath);

//                                string[] sup = { x.ToString(), y.ToString(), w.ToString(), h.ToString(), outpath };

//                                outVals.Add(sup);

//                                totalImageCount++;

//                                // show it

//                                string outMessage = "x: " + x.ToString() + " , " + "y: " + y.ToString() + " , " + "(h,w): " + "(" + h.ToString() + "," + w.ToString() + ")";
//                                string imgPath = outpath;
//                            }

//                            // create heatmap

//                            var overlayImg = view.OverlayImage();
//                            var bitmap = overlayImg.Bitmap;
//                            //heatmapImage = bitmap; //save as instance obj

//                            pictureBoxShow.Image = bitmap;

//                            cleanImage = bitmap;
//                        }
//                    }
//                }

//                // output defect objects

//                List<defect> defects = new List<defect>();

//                // invoke green stream

//                ViDi2.Runtime.IWorkspace green_workspace = control.Workspaces.Add("green workspace", greenModelPath);

//                IStream stream_g = green_workspace.Streams["default"];

//                ITool greenTool = stream_g.Tools["Classify"];

//                for (int i = 0; i < outVals.Count; i++)
//                {
//                    string imgPath = outVals[i][4].ToString();

//                    // invoke green model here!

//                    // Load an image from file
//                    using (IImage image = new LibraryImage(imgPath)) //disposing the image when we do not need it anymore
//                    {
//                        // Allocates a sample with the image
//                        using (ISample sample = stream_g.CreateSample(image))
//                        {

//                            // Process the image by the tool. All upstream tools are also processed
//                            sample.Process(greenTool);

//                            IGreenMarking greenMarking = sample.Markings[greenTool.Name] as IGreenMarking;

//                            foreach (IGreenView view in greenMarking.Views)
//                            {
//                                // store in defect object

//                                defect defect = new defect();

//                                defect.x = Convert.ToDouble(outVals[i][0]);
//                                defect.y = Convert.ToDouble(outVals[i][1]);
//                                defect.height = Convert.ToDouble(outVals[i][2]);
//                                defect.width = Convert.ToDouble(outVals[i][3]);
//                                defect.bestTagName = view.BestTag.Name;
//                                defect.bestTagScore = view.BestTag.Score;
//                                defect.filePath = imgPath;

//                                defects.Add(defect); // add to list of defects
//                            }
//                        }
//                    }
//                }

//                // view results

//                var predictionResults = new frmImage(defects, cleanImage, this);
//                predictionResults.Show();

//            }
//        }

//        // -----------------------------------------------------------------
//        // ---------------------------------- other ------------------------
//        // -----------------------------------------------------------------



//        public void btnLoadImage_Click(object sender, EventArgs e)
//        {
//            // open file dialog
//            OpenFileDialog open = new OpenFileDialog();

//            // apply image only filter
//            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
//            if (open.ShowDialog() == DialogResult.OK)
//            {
//                // display image in picture box  
//                pictureBoxShow.Image = new Bitmap(open.FileName);
//                // image file path  
//                txbxImagePath.Text = open.FileName;
//                imagePath = open.FileName;
//                //pictureBoxShow.SizeMode = PictureBoxSizeMode.StretchImage;
//            }
//        }

//        private void btnLoadModel_Click(object sender, EventArgs e)
//        {
//            // open file dialog
//            OpenFileDialog open = new OpenFileDialog();

//            // only look for cognex ML models
//            open.Filter = "Cognex ML models|*.vrws";
//            if (open.ShowDialog() == DialogResult.OK)
//            {
//                //txbxModelPath.Text = open.FileName;
//            }
//        }

//        public void btnLoadPresets_Click(object sender, EventArgs e)
//        {
//            pictureBoxShow.Image = new Bitmap(imagePathPreset);
//            cleanImage = new Bitmap(imagePathPreset);

//            pictureBoxShow.SizeMode = PictureBoxSizeMode.StretchImage;

//            txbxImagePath.Text = imagePathPreset;
//            //txbxModelPath.Text = modelPathPreset;

//        }

//        public void btnRedAnalyzePred_Click(object sender, EventArgs e)
//        {

//            // Initializes the control
//            // This initialization does not allocate any gpu ressources.
//            using (ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred, new List<int>() { }))
//            {
//                // Initializes all CUDA devices
//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });

//                // Open a runtime workspace from file
//                // the path to this file relative to the example root folder
//                // and assumes the resource archive was extracted there.
//                ViDi2.Runtime.IWorkspace workspace = control.Workspaces.Add("workspace", redModelPath);

//                // Store a reference to the stream 'default'
//                IStream stream = workspace.Streams["default"];

//                // setup overlay env
//                MarkingOverlayExtensions.SetupOverlayEnvironment();

//                // Load an image from file
//                using (IImage image = new ViDi2.Local.LibraryImage(imagePath)) //disposing the image when we do not need it anymore
//                {
//                    // Allocates a sample with the image
//                    using (ISample sample = stream.CreateSample(image))
//                    {
//                        ITool redTool = stream.Tools["Analyze"];

//                        // Process the image by the tool. All upstream tools are also processed
//                        sample.Process(redTool);

//                        IRedMarking redMarking = sample.Markings[redTool.Name] as IRedMarking;

//                        // we can remove this later...

//                        foreach (IRedView view in redMarking.Views)
//                        {

//                            System.Console.WriteLine($"This view has a score of {view.Score}");
//                            foreach (IRegion region in view.Regions)
//                            {
//                                Console.WriteLine($"This region has a score of {region.Score}");
//                            }

//                            // create heatmap

//                            var overlayImg = view.OverlayImage();
//                            var bitmap = overlayImg.Bitmap;

//                            pictureBoxShow.Image = bitmap;

//                            //heatmapImage = bitmap;

//                        }
//                    }
//                }
//            }
//        }

//        public void btnLocateClassify_Click(object sender, EventArgs e)
//        {
//            // Use this to test locate classify  

//            string modelPath = "C:\\Users\\Administrator\\Desktop\\locate-classify-oct-7.vrws";
//            string imagePath = imagePathPreset;

//            string outText = "";

//            // use this to locate and classify Objects

//            using (ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred))
//            {
//                // Initializes all CUDA devices
//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });

//                // Open a runtime workspace from file
//                // the path to this file relative to the example root folder
//                // and assumes the resource archive was extracted there.
//                ViDi2.Runtime.IWorkspace workspace = control.Workspaces.Add("workspace", modelPath);

//                // Store a reference to the stream 'default'
//                IStream stream = workspace.Streams["default"];

//                // Load an image from file
//                using (IImage image = new LibraryImage(imagePath)) //disposing the image when we do not need it anymore
//                {
//                    // Allocates a sample with the image
//                    using (ISample sample = stream.CreateSample(image))
//                    {

//                        // ----- blue locate ------------------

//                        ITool blueTool = stream.Tools["Locate"];

//                        // process the image via Locate tool

//                        sample.Process(blueTool);

//                        IBlueMarking blueMarking = sample.Markings[blueTool.Name] as IBlueMarking;

//                        int numBviews = blueMarking.Views.Count;

//                        MessageBox.Show("number of blue views: " + numBviews.ToString());

//                        foreach (IBlueView view in blueMarking.Views)
//                        {
//                            foreach (var feature in view.Features)
//                            {

//                                outText += feature.Name + " , " +
//                                    feature.Position.X + " , " +
//                                    feature.Position.Y + " , " +
//                                    feature.Size.Width + " , " +
//                                    feature.Size.Height + " , " +
//                                    feature.Score;

//                                outText += "\n\n";
//                            }
//                        }

//                        // ----- green classify --------------------

//                        ITool greenTool = stream.Tools["Classify"];

//                        // Process the image by the tool. All upstream tools are also processed
//                        sample.Process(greenTool);

//                        IGreenMarking greenMarking = sample.Markings[greenTool.Name] as IGreenMarking;


//                        int numGviews = greenMarking.Views.Count;

//                        MessageBox.Show("number of green views: " + numGviews.ToString());

//                        foreach (IGreenView view in greenMarking.Views)
//                        {
//                            System.Console.WriteLine($"View: best_tag={view.BestTag.Name}");

//                            foreach (ITag tag in view.Tags)
//                            {
//                                Console.WriteLine($"tag: name={tag.Name}, score={tag.Score}");

//                                outText += tag.Name + " , " + tag.Score;
//                                outText += "\n\n";

//                            }
//                        }
//                    }
//                }

//                txbxOutput.Text = outText;
//            }

//        }

//        private void btnMakeRedGreenModel_Click(object sender, EventArgs e)
//        {
//            // Oct. 14 ----

//            // suggested by Cognex dev -- use Red's regions, then classify

//            // ------------ Use RED ANALYZE TO LOCATE REGION ------------------

//            string outText = "";

//            Bitmap inputImage = (Bitmap)Image.FromFile(@imagePath);

//            List<string> thumbImagePaths = new List<string>();
//            string modelPath = "C:\\Users\\Administrator\\Desktop\\thred_red_analyze_sept30.vrws";
//            using (ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred, new List<int>() { }))
//            {
//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });
//                ViDi2.Runtime.IWorkspace workspace = control.Workspaces.Add("workspace", modelPath);
//                IStream stream = workspace.Streams["default"];

//                MarkingOverlayExtensions.SetupOverlayEnvironment();

//                using (IImage image = new ViDi2.Local.LibraryImage(imagePath)) //disposing the image when we do not need it anymore
//                {
//                    // Allocates a sample with the image
//                    using (ISample sample = stream.CreateSample(image))
//                    {
//                        ITool redTool = stream.Tools["Analyze"];

//                        // Process the image by the tool. All upstream tools are also processed
//                        sample.Process(redTool);

//                        IRedMarking redMarking = sample.Markings[redTool.Name] as IRedMarking;

//                        foreach (IRedView view in redMarking.Views)
//                        {
//                            int i = 0;

//                            System.Console.WriteLine($"This view has a score of {view.Score}");
//                            foreach (IRegion region in view.Regions)
//                            {
//                                Console.WriteLine($"This region has a score of {region.Score}");

//                                // each region has Center(x,y) and height,width. 
//                                // isolate image to pass to classifier

//                                string x = region.Center.X.ToString();
//                                string y = region.Center.Y.ToString();
//                                string w = region.Width.ToString();
//                                string h = region.Height.ToString();

//                                outText += "obj: " + i.ToString() + " --> " + x + " , " + y + " , " + w + " , " + h + "\n\n";

//                                // create thumbnail

//                                var rect = new System.Windows.Rect(
//                                    Convert.ToInt32(region.Center.X),
//                                    Convert.ToInt32(region.Center.Y),
//                                        (int)256,
//                                        (int)256);

//                                string outpath = "C:\\Users\\Administrator\\Desktop\\thumbnails_for_DL\\" + i.ToString() + ".bmp";

//                                CreateDefectThumbnail(inputImage, rect, outpath);

//                                // add to list

//                                thumbImagePaths.Add(outpath);
//                                i++;

//                            }

//                            // create heatmap

//                            var overlayImg = view.OverlayImage();
//                            var bitmap = overlayImg.Bitmap;

//                            pictureBoxShow.Image = bitmap;

//                            // show region results

//                            txbxOutput.Text = outText;

//                        }
//                    }
//                }
//            }

//            MessageBox.Show("number of thumbs:" + thumbImagePaths.Count.ToString());

//            // ---------FEED IMAGES TO GREEN CLASSIFY ----------


//            // Use this to test locate classify  

//            modelPath = "C:\\Users\\Administrator\\Desktop\\locate-classify-oct-7.vrws";

//            // use this to locate and classify Objects

//            using (ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred))
//            {
//                // Initializes all CUDA devices
//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });

//                // Open a runtime workspace from file
//                // the path to this file relative to the example root folder
//                // and assumes the resource archive was extracted there.
//                ViDi2.Runtime.IWorkspace workspace = control.Workspaces.Add("workspace", modelPath);

//                // Store a reference to the stream 'default'
//                IStream stream = workspace.Streams["default"];
//                ITool greenTool = stream.Tools["Classify"];

//                foreach (string imagepath in thumbImagePaths)
//                {
//                    // Load an image from file
//                    using (IImage image = new LibraryImage(imagepath)) //disposing the image when we do not need it anymore
//                    {
//                        // Allocates a sample with the image
//                        using (ISample sample = stream.CreateSample(image))
//                        {

//                            // Process the image by the tool. All upstream tools are also processed
//                            sample.Process(greenTool);

//                            IGreenMarking greenMarking = sample.Markings[greenTool.Name] as IGreenMarking;

//                            foreach (IGreenView view in greenMarking.Views)
//                            {
//                                System.Console.WriteLine($"View: best_tag={view.BestTag.Name}");

//                                foreach (ITag tag in view.Tags)
//                                {
//                                    Console.WriteLine($"tag: name={tag.Name}, score={tag.Score}");

//                                    string outgreentext = tag.Name + " , " + tag.Score;
//                                    MessageBox.Show("you made Green pred: " + imagepath + "\n\n" + outgreentext);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        // resources for passing Red -> Green
//        public static void CreateDefectThumbnail(Bitmap bitmap, System.Windows.Rect location, string outpath)
//        {
//            using (Bitmap thumbnail = new Bitmap((int)location.Width, (int)location.Height, bitmap.PixelFormat))
//            {
//                thumbnail.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
//                using (Graphics g = Graphics.FromImage(thumbnail))
//                {
//                    // Save a little larger area than exact defect for thumbnail.
//                    var x = location.X - 10;
//                    x = Math.Max(0, x);
//                    var y = location.Y - 10;
//                    y = Math.Max(0, y);
//                    var w = location.Width + 20;
//                    w = Math.Min(bitmap.Width, w);
//                    var h = location.Height + 20;
//                    h = Math.Min(bitmap.Height, h);
//                    g.DrawImage(bitmap, 0, 0, new RectangleF((float)location.X, (float)location.Y, (float)location.Width, (float)location.Height), GraphicsUnit.Pixel);
//                }
//                //return thumbnail.CreateWriteableBitmap();
//                thumbnail.Save(outpath);
//                //return outpath;

//            }
//        }

//        private void btnMakethumbs_Click(object sender, EventArgs e)
//        {
//            //use this to cycle through images, use Red Analyze to find defects and make images

//            string imageDir = @"C:\Users\Administrator\Desktop\sept20_scansforDL\thread_scans";

//            //1. add all training testing images to dir

//            string[] imageFiles = Directory.GetFiles(imageDir);

//            string modelPath = "C:\\Users\\Administrator\\Desktop\\analyze-oct-14.vrws";
//            using (ViDi2.Runtime.Local.Control control = new ViDi2.Runtime.Local.Control(GpuMode.Deferred, new List<int>() { }))
//            {
//                control.InitializeComputeDevices(GpuMode.SingleDevicePerTool, new List<int>() { });
//                ViDi2.Runtime.IWorkspace workspace = control.Workspaces.Add("workspace", modelPath);
//                IStream stream = workspace.Streams["default"];

//                MarkingOverlayExtensions.SetupOverlayEnvironment();

//                //2. Use red analyze for each image

//                int totalImageCount = 0;

//                foreach (string imagePath in imageFiles)
//                {
//                    Bitmap inputImage = (Bitmap)Image.FromFile(@imagePath);

//                    using (IImage image = new ViDi2.Local.LibraryImage(imagePath)) //disposing the image when we do not need it anymore
//                    {
//                        // Allocates a sample with the image
//                        using (ISample sample = stream.CreateSample(image))
//                        {
//                            ITool redTool = stream.Tools["Analyze"];

//                            // Process the image by the tool. All upstream tools are also processed
//                            sample.Process(redTool);

//                            IRedMarking redMarking = sample.Markings[redTool.Name] as IRedMarking;

//                            foreach (IRedView view in redMarking.Views)
//                            {
//                                List<string[]> outVals = new List<string[]>();

//                                System.Console.WriteLine($"This view has a score of {view.Score}");
//                                foreach (IRegion region in view.Regions)
//                                {
//                                    Console.WriteLine($"This region has a score of {region.Score}");

//                                    // each region has Center(x,y) and height,width. 
//                                    // isolate image to pass to classifier

//                                    string x = region.Center.X.ToString();
//                                    string y = region.Center.Y.ToString();
//                                    string w = region.Width.ToString();
//                                    string h = region.Height.ToString();

//                                    //outText += "obj: " + i.ToString() + " --> " + x + " , " + y + " , " + w + " , " + h + "\n\n";

//                                    // create thumbnail

//                                    var rect = new System.Windows.Rect(
//                                        Convert.ToInt32(region.Center.X),
//                                        Convert.ToInt32(region.Center.Y),
//                                            (int)256,
//                                            (int)256);

//                                    string outpath = "C:\\Users\\Administrator\\Desktop\\thumbnails_for_DL\\" + totalImageCount.ToString() + ".bmp";

//                                    CreateDefectThumbnail(inputImage, rect, outpath);

//                                    // add to list

//                                    string[] sup = { x, y, w, h, outpath };

//                                    outVals.Add(sup);

//                                    //thumbImagePaths.Add(outpath);
//                                    totalImageCount++;

//                                }

//                                // create heatmap overlay

//                                var overlayImg = view.OverlayImage();
//                                var bitmap = overlayImg.Bitmap;

//                                pictureBoxShow.Image = bitmap;

//                            }
//                        }
//                    }
//                }



//            }

//        }


//        public void btnShowThumbnail_Click(object sender, EventArgs e)
//        {
//            // show the form

//            string imgPath = "C:\\Users\\Administrator\\Desktop\\thumbnails_for_DL_oct15\\1375.bmp";

//            List<defect> defects = new List<defect>();

//            defect sampledefect = new defect();

//            sampledefect.x = 999;
//            sampledefect.y = 999;
//            sampledefect.height = 999;
//            sampledefect.width = 999;
//            sampledefect.bestTagName = "unknown";
//            sampledefect.bestTagScore = 999;
//            sampledefect.filePath = imgPath;

//            defects.Add(sampledefect);

//            // now open sample form

//            var myForm = new frmImage(defects, cleanImage, this);

//            myForm.pictureBox1.Image = new Bitmap(imgPath);
//            myForm.txbxModelText.Text = "hai!";
//            myForm.lblImgPath.Text = imgPath;
//            myForm.Show();

//        }

//        public void cxbxResize_CheckedChanged(object sender, EventArgs e)
//        {
//            // use this to rescale image

//            if (cxbxResize.Checked == true)
//            {
//                panel1.AutoScroll = true;
//                pictureBoxShow.SizeMode = PictureBoxSizeMode.AutoSize;
//            }

//            if (cxbxResize.Checked == false)
//            {
//                panel1.AutoScroll = false;
//                pictureBoxShow.SizeMode = PictureBoxSizeMode.StretchImage;
//            }
//        }
//    }
//}
