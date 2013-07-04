using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToCss
{
    class Program
    {
        static void Main(string[] args)
        {
            string name;
            string class_name;
            if (args.Length < 1)
            {
                Console.Write("ファイル名:");
                name = Console.ReadLine();
            }
            else
            {
                name = args[0];
            }
            if (args.Length < 2)
            {
                Console.Write("クラス名:");
                class_name = Console.ReadLine();
            }
            else
            {
                class_name = args[1];
            }

            using (var sr = new System.IO.StreamWriter("style.css"))
            {
                using (var img = new System.Drawing.Bitmap(name))
                {
                    sr.Write("."+class_name+"{");
                    sr.Write("width:" + img.Width + "px;");
                    sr.Write("height:" + img.Height + "px;");
                    sr.Write("}");
                    sr.Write("."+class_name+":before{");
                    sr.Write("content: \"\";");
                    sr.Write("display: block;");
                    sr.Write("width:" + 1 + "px;");
                    sr.Write("height:" + 1 + "px;");
                    var check = new bool[img.Height, img.Width];
                    for (int i = 0; i < img.Height; i++)
                    {
                        for (int j = 0; j < img.Width; j++)
                        {
                            check[i, j] = false;
                        }
                    }
                    int max = (img.Width > img.Height) ? img.Width : img.Height;
                    for (int i = 0; i < img.Height; i++)
                    {
                        for (int j = 0; j < img.Width; j++)
                        {
                            if (!check[i, j])
                            {
                                int box_size=1;
                                for (box_size = 1; box_size < max; box_size++)
                                {
                                    if (!blockCheck(img, j, i, box_size)) break;
                                }
                                for (int l = 0; l < ((box_size-1)*2+1); l++)
                                {
                                    for (int m = 0; m < ((box_size-1)*2+1); m++)
                                    {
                                        check[i + l, j + m] = true;
                                    }
                                }
                                var pixel = img.GetPixel(j, i);

                                double alpha = pixel.A / 255.0;
                                
                                if (i == 0 && j == 0)
                                {
                                    sr.Write("background:rgba(" + pixel.R + "," + pixel.G + "," + pixel.B + "," + alpha + ");");
                                    sr.Write("-webkit-box-shadow:");
                                    if (box_size != 1)
                                    {
                                        sr.Write(addPx(box_size - 1) + " " + addPx(box_size - 1) + " 0 " + addPx(box_size - 1) + " rgba(" + pixel.R + "," + pixel.G + "," + pixel.B + "," + alpha + "),");
                                    }
                                }
                                else
                                {
                                    if ((alpha != 0) && (alpha != 1))
                                    {
                                        sr.Write(addPx(box_size - 1 + j) + " " + addPx(box_size - 1 + i) + " 0 " + addPx(box_size - 1) + " rgba(" + pixel.R + "," + pixel.G + "," + pixel.B + "," + toHalfAdjust(alpha,2) + "),");
                                    }
                                    else if (alpha == 1)
                                    {
                                        sr.Write(addPx(box_size - 1 + j) + " " + addPx(box_size - 1 + i) + " 0 " + addPx(box_size - 1) + " rgb(" + pixel.R + "," + pixel.G + "," + pixel.B + "),");
                                    }
                                }
                            }
                        }
                    }
                    sr.Write("0 0;}");
                }
            }
        }

        static public bool blockCheck(System.Drawing.Bitmap img, int x, int y, int size)
        {
            int box_width = 1 + size * 2;
            if ((img.Width > (x + box_width)) && (img.Height > (y + box_width)))
            {
                var pixel = img.GetPixel(x, y);
                for (int i = 0; i < box_width; i++)
                {
                    for (int j = 0; j < box_width; j++)
                    {
                        if (pixel != img.GetPixel(j+x, i+y)) return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static double toHalfAdjust(double dValue, int iDigits)
        {
            double dCoef = System.Math.Pow(10, iDigits);

            return dValue > 0 ? System.Math.Floor((dValue * dCoef) + 0.5) / dCoef :
                                System.Math.Ceiling((dValue * dCoef) - 0.5) / dCoef;
        }

        public static string addPx(int px)
        {
            return (px == 0) ? "0" : px + "px";
        }
    }
}
