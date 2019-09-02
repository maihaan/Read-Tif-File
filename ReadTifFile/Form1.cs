using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OSGeo.GDAL;

namespace ReadTifFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // init gdal -- very importance
            GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
        }

        // Open tif file
        private void btBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Tif files (*.tif) | *.tif";
            if(od.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = od.FileName;
            }
        }

        // Read raster data from tif file
        private void btRead_Click(object sender, EventArgs e)
        {
            // open tif file
            using (Dataset img = Gdal.Open(tbPath.Text, Access.GA_ReadOnly))
            {
                // Get 3 bands of tif file
                Band band1 = img.GetRasterBand(1);
                Band band2 = img.GetRasterBand(2);
                Band band3 = img.GetRasterBand(3);

                // Get with and high of tif file in pixel
                int w = band1.XSize;
                int h = band1.YSize;

                // Get tif transform
                double[] t = new double[6];
                img.GetGeoTransform(t);

                // Get lat lon from interface
                double lat = double.Parse(tbLatLon.Text.Split(' ')[0]);
                double lon = double.Parse(tbLatLon.Text.Split(' ')[1]);

                // Get x, y in pixel from lat, lon
                int x = (int)Math.Round(((lat - t[0]) * (t[1] * t[5] - t[2] * t[4]) - t[2] * ((lon - t[3]) * t[1] - t[4] * (lat - t[0]))) 
                    / (t[1] * (t[1] * t[5] - t[2] * t[4])), 0);
                int y = (int)Math.Round(((lon - t[3]) * t[1] + t[4] * (lat - t[0])) / (t[1] * t[5] - t[2] * t[4]), 0);

                // Read data
                int[] by1 = new int[w];
                int[] by2 = new int[w];
                int[] by3 = new int[w];
                band1.ReadRaster(0, y, w, 1, by1, w, 1, 0, 0);
                band2.ReadRaster(0, y, w, 1, by2, w, 1, 0, 0);
                band3.ReadRaster(0, y, w, 1, by3, w, 1, 0, 0);
                int valueBand1 = by1[x];
                int valueBand2 = by2[x];
                int valueBand3 = by3[x];

                // Show result
                tbResult.Text = valueBand1.ToString() + " "
                    + valueBand2.ToString() + " "
                    + valueBand3.ToString();
            }
        }
    }
}
