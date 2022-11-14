using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HammingCodeSimulator
{
    class BitInfo
    {
        public TextBlock[] blocks;
        public TextBlock DebugBox;
        public TextBlock DebugBox2;

        public int[] bitValue;
        public int[] parityValue;

        public static int[] bitIndex = new int[] { 3, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15 };
        public static int[] parityIndex = new int[] { 0, 1, 2, 4, 8 }; 

        public BitInfo()
        {
            bitValue = new int[11];
            parityValue = new int[5];
        }

        public static int getBitIndex(int bitIdx)
        {
            for(int i=0; i<bitIndex.Length; i++)
            {
                if (bitIdx == bitIndex[i]) return i;
            }
            return -1;
        }
        public void calcParity()
        {
            parityValue = new int[]{ 0, 0, 0, 0, 0};
            for (int i = 0; i < bitValue.Length; i++)
            {
                if (bitValue[i] == 1) { parityValue[0] = 1 - parityValue[0]; }
                if ((bitIndex[i] + 1) % 2 == 0 && bitValue[i] == 1) { parityValue[1] = 1 - parityValue[1]; }
                if ((bitIndex[i] % 4) > 1 && bitValue[i] == 1) { parityValue[2] = 1 - parityValue[2]; }
                if ((bitIndex[i] % 8) / 2 >= 2 && bitValue[i] == 1) { parityValue[3] = 1 - parityValue[3]; }
                if (bitIndex[i] >= 8 && bitValue[i] == 1) { parityValue[4] = 1 - parityValue[4]; }
            }
            for (int i = 1; i < parityValue.Length; i++)
            {
                if (parityValue[i] == 1) { parityValue[0] = 1 - parityValue[0]; }
            }
            updateParityTextBlock();
        }

        private void updateBitTextBlock()
        {
            int i = 0;
            for (i = 0; i < bitIndex.Length; i++)
            {
                blocks[bitIndex[i]].Text = bitValue[i].ToString();
            }
        }

        private void updateParityTextBlock()
        {
            int i = 0;
            for(i=0; i<parityIndex.Length; i++)
            {
                blocks[parityIndex[i]].Text = parityValue[i].ToString();
            }
        }

        public void updateTextBlock()
        {
            updateBitTextBlock();
            updateParityTextBlock();
        }

        public void checkParity()
        {
            int[] tempParityValue = new int[] { 0, 0, 0, 0, 0 };
            int[] checkResult = new int[] { 0, 0, 0, 0, 0 };
            for (int i = 0; i < bitValue.Length; i++)
            {
                if (bitValue[i] == 1) { tempParityValue[0] = 1 - tempParityValue[0]; }
                if ((bitIndex[i] + 1) % 2 == 0 && bitValue[i] == 1) { tempParityValue[1] = 1 - tempParityValue[1]; }
                if ((bitIndex[i] % 4) > 1 && bitValue[i] == 1) { tempParityValue[2] = 1 - tempParityValue[2]; }
                if ((bitIndex[i] % 8) / 2 >= 2 && bitValue[i] == 1) { tempParityValue[3] = 1 - tempParityValue[3]; }
                if (bitIndex[i] >= 8 && bitValue[i] == 1) { tempParityValue[4] = 1 - tempParityValue[4]; }
            }

            int checksum = 0;
            for (int i = 1; i < checkResult.Length; i++)
            {
                if (parityValue[i] == 1) { tempParityValue[0] = 1 - tempParityValue[0]; }
                if (parityValue[i] != tempParityValue[i]) { checkResult[i] = 1; }
                checksum += checkResult[i];
            }
            if(tempParityValue[0] != parityValue[0]) { checkResult[0] = 1; checksum += checkResult[0]; }
            DebugBox2.Text = string.Join(",", checkResult);
            if (checksum == 0)
            {
                foreach(int idx in bitIndex)
                {
                    blocks[idx].Background = Brushes.Green;
                }
            }

            else {
                if (checkResult[0] == 0)
                {
                    //if (checkResult[1] == 1 && checkResult[2] == 1)
                    //{
                    //    int res1 = checkResult[4] * 8 + checkResult[3] * 4 + checkResult[2] * 2 + (1-checkResult[1]);
                    //    int res2 = checkResult[4] * 8 + checkResult[3] * 4 + (1-checkResult[2]) * 2 + checkResult[1];
                    //    blocks[res1].Background = Brushes.Red;
                    //    blocks[res2].Background = Brushes.Red;
                    //    DebugBox.Text = res1.ToString() + ", " + res2.ToString();
                    //}
                    //else if (checkResult[3] == 1 && checkResult[4] == 1)
                    //{
                    //    int res1 = (1-checkResult[4]) * 8 + checkResult[3] * 4 + checkResult[2] * 2 + checkResult[1];
                    //    int res2 = checkResult[4] * 8 + (1-checkResult[3]) * 4 + checkResult[2] * 2 + checkResult[1];
                    //    blocks[res1].Background = Brushes.Red;
                    //    blocks[res2].Background = Brushes.Red;
                    //    DebugBox.Text = res1.ToString() + ", " + res2.ToString();

                    //}
                    foreach (int idx in bitIndex)
                    {
                        blocks[idx].Background = Brushes.Crimson;
                        DebugBox.Text = "2 bit errors detected";
                    }
                }
                else
                {
                    int res = checkResult[4] * 8 + checkResult[3] * 4 + checkResult[2] * 2 + checkResult[1];
                    blocks[res].Background = Brushes.Red;
                    DebugBox.Text = res.ToString();
                }
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BitInfo sender, receiver;
        double flip_rate_value;
        int flipCnt = 0;

        public MainWindow()
        {
            InitializeComponent();
            sender = new BitInfo();
            receiver = new BitInfo();

            sender.blocks = new TextBlock[] { o1, o2, o3, o4, o5, o6, o7, o8, o9, o10, o11, o12, o13, o14, o15 , o16};
            receiver.blocks = new TextBlock[] { r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r14, r15, r16 };
            receiver.DebugBox = DebugBox;
            receiver.DebugBox2 = DebugBox2;

            this.flip_rate_value = Double.Parse(this.flip_rate.Text);
        }

        private void flip_rate_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.flip_rate_value = Double.Parse(this.flip_rate.Text);
        }

        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            foreach(int idx in BitInfo.bitIndex)
            {
                receiver.blocks[idx].Background = Brushes.White;
            }
            foreach(int idx in BitInfo.parityIndex)
            {
                receiver.blocks[idx].Background = Brushes.LightGray;
            }
            flipCnt = 0;
            receiver.bitValue = Flip_Bit(this.sender.bitValue);
            receiver.parityValue = Flip_Bit(this.sender.parityValue);
            receiver.updateTextBlock();

            receiver.checkParity();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            foreach(int idx in BitInfo.bitIndex)
            {
                int value = rand.Next(0, 2);
                this.sender.blocks[idx].Text = value.ToString();
                this.sender.bitValue[BitInfo.getBitIndex(idx)] = (value);
            }
            this.sender.calcParity();
        }

        private int[] Flip_Bit(int[] value)
        {
            Random rand = new Random();
            int prob = (int)(flip_rate_value * 100);
            int[] retVal = new int[value.Length];
            for(int i=0; i<value.Length; i++)
            {
                if (flipCnt > 1)
                {
                    retVal[i] = value[i];
                }
                else
                {
                    int flipRes = rand.Next(0, 100);
                    if (flipRes < prob)
                    {
                        retVal[i] = 1 - value[i];
                        flipCnt++;
                    }
                    else retVal[i] = value[i];
                }
            }
            return retVal;
        }
    }
}
