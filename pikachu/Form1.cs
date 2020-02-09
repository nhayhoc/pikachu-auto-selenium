﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pikachu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int[,] arr;

        const int SIZE_X = 16;
        const int SIZE_Y = 9;
        int[,] indexEle = new int[SIZE_X + 2, SIZE_Y + 2];

        List<Point> res;
        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(() => { Run(); }).Start();
        }
        private void Run()
        {
            //string[] input = File.ReadAllLines(@"C:\Users\Dell\Desktop\pikachu.txt");
            //arr = new int[input.Length, input.Length];
            //for (int i = 0; i < input.Length; i++)
            //{
            //    string[] temp = input[i].Split(' ');
            //    for (int j = 0; j < temp.Length; j++)
            //    {
            //        int type = int.Parse(temp[j]);
            //        arr[j, i] = type;
            //    }
            //}
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-notifications"); // to disable notification
            var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://gamehaynhat.com.vn/webgl/0/18/?gid=18");
            //MessageBox.Show("hihi");



            arr = new int[SIZE_X + 2, SIZE_Y + 2];
            int x = 1;
            int y = 1;

            //SET TRUC NGANG
            for (int i = 0; i < SIZE_X + 2; i++)
            {               
                arr[i, 0] = 0;
                arr[i, SIZE_Y + 1] = 0;
            }
            for (int i = 0; i < SIZE_Y + 2; i++)
            {
                arr[0, i] = 0;
                arr[SIZE_X + 1, i] = 0;
            }
            dynamic children;
            children = ReadArr(driver);

           

            
            while (true)
            {
                for (int i = 1; i < SIZE_X + 1; i++)
                {
                    for (int j = 1; j < SIZE_Y + 1; j++)
                    {
                        try
                        {

                       
                        res = new List<Point>();

                        CanTry(new Point(i, j), arr[i, j], -1, 0, "");
                        if (res.Count == 0) continue;

                        children[indexEle[i, j]].Click();
                        Thread.Sleep(50);
                        children[indexEle[res[0].X, res[0].Y]].Click();

                        arr[i, j] = 0;
                        arr[res[0].X, res[0].Y] = 0;
                        Thread.Sleep(300);
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message);
                        }
                    }

                }
                try
                {
                    driver.SwitchTo().Alert().Accept();
                    Console.WriteLine("found element");
                    children = ReadArr(driver);
                }
                catch (Exception)
                {
                    Console.WriteLine("not found element");
                }

                ;
            }
            


        }
        dynamic ReadArr(ChromeDriver driver)
        {
            int x = 1;
            int y = 1;
            dynamic children = driver.FindElements(By.XPath("//*[@id=\"board\"]/div/div"));
            for (int i = 3; i < children.Count; i++)
            {
                indexEle[x, y] = i;


                string visibility = children[i].GetCssValue("visibility");
                //string html = children[i].GetAttribute("innerHTML");
                string src = children[i].FindElement(By.TagName("img")).GetAttribute("src");
                string idImg = Regex.Match(src, @"pieces(.*?)\.png").Groups[1].Value;
                arr[x, y] = visibility == "visible" ? int.Parse(idImg) : 0;
                //Console.WriteLine($"{x} x {y} = {arr[x, y]}");


                y++;
                if (y > SIZE_Y)
                {
                    y = 1;
                    x++;
                }
            }
            WriteArr();
            return children;
        }
        void WriteArr()
        {
            for (int i = 0; i < SIZE_Y + 2; i++)
            {
                for (int j = 0; j < SIZE_X + 2; j++)
                {
                    Console.Write(arr[j,i].ToString() + " ");
                }
                Console.WriteLine();
            }
        }
        void CanTry(Point locationNow, int type, int direction, int countDirection, string trace)
        {
            //nếu countDirction >=4 break false;
            //nếu type == type now && countDirec<4 break true

            if (locationNow.X >= SIZE_X+2 || locationNow.Y >= SIZE_Y+2) return;
            if (type == 0) return;

            string[] x = new string[] { "tren", "duoi", "phai", "trai" };

            trace += ", " + ((direction == -1) ? "" : x[direction]) + $" {locationNow}";
            //Console.WriteLine(trace);
            if (trace == ",  {X=1,Y=1}, tren {X=1,Y=0}, phai {X=2,Y=0}, phai {X=3,Y=0}, phai {X=4,Y=0}, duoi {X=4,Y=1}")
            {
                ;
            }
            if (type == GetType(locationNow) && countDirection < 4 && countDirection > 0)
            {
                trace += $" => OK";
                Console.WriteLine(trace);
                res.Add(locationNow);
                return;
            }
            if (countDirection >= 4)
            {
                trace += $"DIE";
                return;
            }
            if (GetType(locationNow) != 0 && direction != -1) return;

            //t - d - p - t
            for (int i = 0; i < 4; i++)
            {
                Point locationDirection = new Point(-1, -1);
                if (i == 0 && direction != 1)
                {
                    if (locationNow.Y - 1 == -1) continue;
                    locationDirection = new Point(locationNow.X, locationNow.Y - 1);
                }
                else if (i == 1 && direction != 0)
                {
                    if (locationNow.Y + 1 == arr.GetLength(0)) continue;
                    locationDirection = new Point(locationNow.X, locationNow.Y + 1);
                }
                else if (i == 2 && direction != 3)
                {
                    if (locationNow.X + 1 == arr.GetLength(0)) continue;
                    locationDirection = new Point(locationNow.X + 1, locationNow.Y);
                }
                else if (i == 3 && direction != 2)
                {
                    if (locationNow.X - 1 == -1) continue;
                    locationDirection = new Point(locationNow.X - 1, locationNow.Y);
                }


                if (locationDirection.X != -1)
                {
                    CanTry(locationDirection, type, i, (direction == i) ? countDirection : countDirection + 1, trace);
                }

            }
        }

        int GetType(Point p)
        {
            return arr[p.X, p.Y];
        }
    }
}
