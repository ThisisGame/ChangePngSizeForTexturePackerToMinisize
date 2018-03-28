using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace MultiSizeIcon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        


        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //首先读取配置文件
            if(File.Exists("./ChangePngSizeForTexturePackerToMinisize.txt")==false)
            {
                MessageBox.Show("Not Found ChangePngSizeForTexturePackerToMinisize.txt");
                return;
            }

            StreamReader tmpStreamReaderConfig = new StreamReader("./ChangePngSizeForTexturePackerToMinisize.txt");
            List<string> tmpListImageDataStr = new List<string>();
            while (tmpStreamReaderConfig.EndOfStream==false)
            {
                tmpListImageDataStr.Add(tmpStreamReaderConfig.ReadLine());
            }


            List<string> tmpListPngFileJump = new List<string>();
            for (int i = 0; i < tmpListImageDataStr.Count; i++)
            {
                string tmpImageDataStr = tmpListImageDataStr[i];
                tmpImageDataStr = tmpImageDataStr.Trim();
                if (tmpImageDataStr.StartsWith("#"))
                {
                    continue;
                }
                string[] tmpImageDataStrArr = tmpImageDataStr.Split(',');
                if(tmpImageDataStrArr.Length!=4)
                {
                    continue;
                }

                string tmpImageName = tmpImageDataStrArr[0];
                string tmpImageWidthStr = tmpImageDataStrArr[1];
                string tmpImageHeightStr = tmpImageDataStrArr[2];
                string tmpImageScaleStr = tmpImageDataStrArr[3];
                int tmpImageWidth = int.Parse(tmpImageWidthStr);
                int tmpImageHeight = int.Parse(tmpImageHeightStr);
                float tmpImageScale = float.Parse(tmpImageScaleStr);

                //处理图片
                FileStream stream = File.OpenRead("./"+ tmpImageName);
                int fileLength = 0;
                fileLength = (int)stream.Length;
                Byte[] image = new Byte[fileLength];
                stream.Read(image, 0, fileLength);
                System.Drawing.Image result = System.Drawing.Image.FromStream(stream);
                stream.Close();

                //判断图片是否已经处理过
                if(result.Width!=tmpImageWidth || result.Height!=tmpImageHeight)
                {
                    tmpListPngFileJump.Add(tmpImageName);
                    continue;
                }


                Size s = new Size((int)(tmpImageScale*result.Width), (int)(tmpImageScale*result.Height));
                Bitmap newBit = new Bitmap(result, s);

                File.Move("./" + tmpImageName, "./" + tmpImageName + ".bak");

                newBit.Save("./" + tmpImageName);

                File.Delete("./" + tmpImageName + ".bak");

                newBit.Dispose();
            }


            string tmpMsg = string.Empty;
            if(tmpListPngFileJump.Count>0)
            {
                tmpMsg += "处理完毕，跳过的文件有" + tmpListPngFileJump.Count + "个,";
                for (int i = 0; i < tmpListPngFileJump.Count; i++)
                {
                    tmpMsg += Environment.NewLine+tmpListPngFileJump[i];
                }
            }
            else
            {
                tmpMsg += "处理完毕，跳过的文件有0个,";
            }
            MessageBox.Show(tmpMsg);
        }
    }
}
