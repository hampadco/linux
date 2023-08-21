// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace ViewModel
// {
//     public class CustomForm
//     {

//     }
//     public class EKG
//     {
//         private static int Flag_LL = 0x01;      // 00000001
//         private static int Flag_RL = 0x02;      // 00000010
//         private static int Flag_LA = 0x04;      // 00000100
//         private static int Flag_RA = 0x08;      // 00001000
//         private static int Flag_Chest = 0x10;   // 00010000
//         private static int Flag_C2 = 0x0100;    // 00000001
//         private static int Flag_C3 = 0x0200;    // 00000010
//         private static int Flag_C4 = 0x0400;    // 00000100
//         private static int Flag_C5 = 0x0800;    // 00001000
//         private static int Flag_C6 = 0x1000;    // 00010000

//         string[] Signal = { "I", "II", "III", "aVR", "aVL", "aVF", "C1",
//                                 "Resp", "C2", "C3", "C4", "C5", "C6" };

//         string[] status ={ "Normal", "Normal + Pacemaker detected", "not used", "not used", "Initializing",
//                                "Searching for electrodes", "not used", "not used", "Simulated Output", "not used",
//                                "Selftest Error", "not used", "not used", "not used", "not used", "not used" };


        

//         StreamWriter Outfile;


//         // Channel class for all parameter
//         public class channel
//         {
//             public string CH_name;
//             public int _hoehe;
//             public int _base;
//             public bool xoffset;
//             public int old_wave;

//             public channel(string n, int h, int b, bool off)
//             {
//                 CH_name = n;
//                 _hoehe = h;
//                 _base = b;
//                 xoffset = off;
//                 old_wave = 0;
//             }
//         }

//         int Outfilectr = -1;
//         int cnt_Channel = 0;
//         int cnt_Channel1 = 0;


//         channel[] Channel = new channel[]
//         {
//                 new channel("I",0, 0,  false),
//                 new channel("II",0, 0, false),
//                 new channel("III",0, 0, false),
//                 new channel("aVR",0, 0,   false),
//                 new channel("aVL",0, 0,  false),
//                 new channel("aVF",0, 0, false),
//                 new channel("C1",0, 0, false),
//                 new channel("Resp",0, 0,false),
//                 new channel("C2",0, 0,  false),
//                 new channel("C3",0, 0, false),
//                 new channel("C4",0, 0,  false),
//                 new channel("C5",0, 0, false),
//                 new channel("C6",0, 0, false)
//         };


//         int column = 1;         // Column of the graph
//         byte rand;
//         int index;
//         byte gr_rand = 20;
//         int max_Wert;


//         int[] Datensatz;        // Array of last Wavevalues
//         int packetsread = 0;

//         bool Flag_eg1200 = false;

//         public EKG()
//         {

//             Datensatz = new int[13];
//             index = rand + 2;


//         }

//         public StreamWriter outfile
//         {
//             get
//             {
//                 return Outfile;
//             }

//             set
//             {
//                 Outfile = value;
//             }
//         }

//         public bool flag_eg1200
//         {
//             get
//             {
//                 return Flag_eg1200;
//             }
//             set
//             {
//                 Flag_eg1200 = value;
//             }

//         }

//         public int Channel_Count
//         {
//             get
//             {

//                 return cnt_Channel;
//             }
//             set
//             {
//                 if (value > 7)
//                     column = 2;
//                 else column = 1;
//                 cnt_Channel = value;
//             }
//         }

//         public int outfileCtr
//         {
//             get
//             {
//                 return Outfilectr;
//             }
//             set
//             {
//                 Outfilectr = value;
//             }
//         }

//         public int Channel_limb_Count
//         {
//             get
//             {
//                 return cnt_Channel1;
//             }
//             set
//             {
//                 cnt_Channel1 = value;
//             }
//         }

//         public int Index
//         {
//             get
//             {
//                 return index;
//             }
//             set
//             {
//                 index = value;
//             }
//         }

//         public int Packetsread
//         {
//             get
//             {
//                 return packetsread;
//             }
//             set
//             {
//                 packetsread = value;
//             }
//         }

//         public int Max_Wert
//         {
//             get
//             {
//                 return max_Wert;
//             }
//             set
//             {
//                 max_Wert = value;
//             }
//         }

//         public byte Rand
//         {
//             get
//             {
//                 return rand;
//             }
//             set
//             {
//                 rand = value;
//             }
//         }

//         //parsing identify message
//         public void nextIdentify(string message)
//         {
//             if (message.Contains("EG12000"))
//             {
//                 Flag_eg1200 = true;
//                 ////form1.WaveValue = new int[13];
//             }
//             else
//             {
//                 Flag_eg1200 = false;
//                 ////form1.WaveValue = new int[8];
//             }

//             if (Flag_eg1200 == true)
//             {
//                 ////form1.checkBox_C2.Enabled = true;
//                 ////form1.checkBox_C3.Enabled = true;
//                 ////form1.checkBox_C4.Enabled = true;
//                 ////form1.checkBox_C5.Enabled = true;
//                 ////form1.checkBox_C6.Enabled = true;
//                 ////form1.label_C2.Enabled = true;
//                 ////form1.label_C3.Enabled = true;
//                 ////form1.label_C4.Enabled = true;
//                 ////form1.label_C5.Enabled = true;
//                 ////form1.label_C6.Enabled = true;
//             }
//         }

//         //  81   82   83   84   85   86   87   88   89   90   91   92   93   94   95   96    97    98    99    100
//         int[] pt = { 440, 466, 494, 523, 554, 587, 622, 659, 698, 740, 784, 831, 880, 932, 988, 1046, 1109, 1175, 1244, 1328 };

//         //Funktion for Sound of Piep
//         private void Piep(double Frequenz, int time)
//         {
//             const int MinFreq = 0x25;
//             const int MaxFreq = 0x7FFF;

//             if (Frequenz < MinFreq)
//                 Frequenz = MinFreq;
//             else if (Frequenz > MaxFreq)
//                 Frequenz = MaxFreq;
//             Console.Beep((int)(Frequenz), time);
//         }

//         //parsing Value for pulse and respiration
//         public void nextValue(byte[] buffer)
//         {
//             int lng = 3;
//             int check = (buffer[1] & 0x7f);
//             int temp = 0;
//             for (int i = 2; i < lng; i++)
//             {
//                 temp += buffer[i];
//             }
//             temp += buffer[0];
//             temp = temp % 128;
//             if (temp == check)
//             {
//                 if ((buffer[0] & 0x03) == 1)
//                 {
//                     ////form1.label_Pulse.Text = buffer[2].ToString();
//                     if ( (buffer[2] != 0))     // Have to beep ?
//                     {

//                         int frq = 0;


//                         Piep(pt[frq], 50);
//                     }
//                 }
//                 else if ((buffer[0] & 0x03) == 2)
//                 {
//                     ////form1.label_Resp.Text = buffer[2].ToString();
//                 }
//             }
//         }

//         bool Status_ok = false;

//         // parsing statusmessage and electrodes I - C1
//         public void nextStatus(byte[] buffer)
//         {

//             int lng = 6;
//             int check = (buffer[1] & 0x7f);
//             int temp = 0;

//             for (int i = 2; i < lng; i++)
//             {
//                 temp += buffer[i];
//             }
//             temp += buffer[0];
//             temp = temp % 128;
//             if (temp == check)
//             {
//                 electrode_check((buffer[2] & 0x1F) + 0x80);
//                 ////form1.label_Status.Text = status[(buffer[5] & 0x0f)];
//                 if ((buffer[5] & 0x40) == 0x40)
//                 {
//                     ////form1.toolStripStatusLabel_Modus.Text = "Neonates";
//                     ////form1.radioButton_neo.Checked = true;
//                 }
//                 else
//                 {
//                     ////form1.toolStripStatusLabel_Modus.Text = "Adult";
//                     ////form1.radioButton_adult.Checked = true;
//                 }
//                 if ((buffer[2] & 0x20) == 0x20)
//                 {
//                     //form1.checkBox_50Hz_inter.Checked = true;
//                 }
//                 else
//                 {
//                     //form1.checkBox_50Hz_inter.Checked = false;

//                 }
//                 switch ((buffer[4] & 0x03))
//                 {
//                     case 0:
//                         {
//                             //form1.radioButton_50.Checked = true;
//                             //form1.toolStripStatusLabel_Speed.Text = "50/sec";
//                             break;
//                         }
//                     case 1:
//                         {
//                             //form1.radioButton_100.Checked = true;
//                             //form1.toolStripStatusLabel_Speed.Text = "100/sec";
//                             break;
//                         }
//                     case 2:
//                         {
//                             //form1.radioButton_150.Checked = true;
//                             //form1.toolStripStatusLabel_Speed.Text = "150/sec";
//                             break;
//                         }
//                     case 3:
//                         {
//                             //form1.radioButton_300.Checked = true;
//                             //form1.toolStripStatusLabel_Speed.Text = "300/sec";
//                             break;
//                         }

//                 }
//                 switch (((buffer[4] & 0x0c) >> 2))
//                 {
//                     case 0:
//                         {
//                             //form1.radioButton_Amp1.Checked = true;
//                             //form1.toolStripStatusLabel_Amplitude.Text = "Amp 1";
//                             break;
//                         }
//                     case 1:
//                         {
//                             //form1.radioButton_Amp2.Checked = true;
//                             //form1.toolStripStatusLabel_Amplitude.Text = "Amp 2";
//                             break;
//                         }
//                     case 2:
//                         {
//                             //form1.radioButton_Amp3.Checked = true;
//                             //form1.toolStripStatusLabel_Amplitude.Text = "Amp 3";
//                             break;
//                         }
//                     case 3:
//                         {
//                             //form1.radioButton_Amp4.Checked = true;
//                             //form1.toolStripStatusLabel_Amplitude.Text = "Amp 4";
//                             break;
//                         }

//                 }


//                 Status_ok = true;
//             }
//         }

//         // parsing for Status of Electrodes C2-C6 (only EG12000)
//         public void nextStatusChest(byte[] buffer)
//         {
//             int lng = 4;
//             int check = (buffer[1] & 0x7f);
//             int temp = 0;
//             for (int i = 2; i < lng; i++)
//             {
//                 temp += buffer[i];
//             }
//             temp += buffer[0];
//             temp = temp % 128;
//             if (temp == check)
//             {
//                 electrode_check(((buffer[2] & 0x1F) << 8) + 0x8000);
//             }

//         }

//         enum cha { chi, chii, chiii, chvr, chvl, chvf, chc1, chresp, chc2, chc3, chc4, chc5, chc6 };

//         //parsing Wavevalue message I - C1
//         public void nextLimb(byte[] buffer)
//         {

//             int lng = (buffer[1] >> 4) + 2;
//             int check = (buffer[1] & 0x0f);
//             int temp = 0;
//             for (int i = 2; i < lng; i++)
//             {
//                 temp += buffer[i];
//             }
//             temp += buffer[0];
//             temp = temp % 16;

//             //if (temp == check)
//             {
//                 for (int i = 2; i < lng; i++)
//                 {
//                     Datensatz[i - 2] = buffer[i];
//                 }

//                 Display();
//                 //if (index % ((form1.pictureBox_Graph.Image.Width / column - 35) / 10) == 0)
//                 //{
//                 //    Graphics g = Graphics.FromImage(//form1.pictureBox_Graph.Image);
//                 //    //dumint = rand + ((int)(Math.Round((double)((tempxrand - rand) * (j / 10.0)))));
//                 //    g.DrawString(DateTime.Now.ToLongTimeString(), new Font(FontFamily.GenericSerif, 8, FontStyle.Bold), new SolidBrush(Color.Black), new Point((index), //form1.pictureBox_Graph.Height - 20));
//                 //    ////form1.pictureBox_Graph.Refresh();
//                 //    //g.DrawLine(blackPen, new Point(dumint, ch._base - ch._hoehe + gr_rand), new Point(dumint, ch._base));
//                 //}
//                 //if (//form1.pictureBox_Graph.Image != null)
//                 //{
//                 //    if (index < (//form1.pictureBox_Graph.Image.Width / column - 10))
//                 //    {
//                 //        index = index + 1;
//                 //    }
//                 //    else
//                 //    {
//                 //        index = rand + 2;
//                 //        display_diag_exit = false;
//                 //        display_diag();
//                 //    }
//                 //}

//             }
//         }

//         //parsing Wavevalue message C2- C6 (only EG12000)
//         public void nextChest(byte[] buffer)
//         {
//             int lng = (buffer[1] >> 4) + 2;
//             int check = (buffer[1] & 0x0f);
//             int temp = 0;
//             for (int i = 2; i < lng; i++)
//             {
//                 temp += buffer[i];
//             }
//             temp += buffer[0];
//             temp = temp % 16;

//             if (temp == check)
//             {
//                 for (int i = 2; i < lng; i++)
//                 {
//                     Datensatz[i - 2 + Channel_limb_Count] = buffer[i];

//                 }

//             }
//         }

//         public void electrode_check(int electrode)
//         {
//             if ((electrode & 0x00ff) != 0)
//             {
//                 if ((electrode & Flag_LL) == Flag_LL)
//                 {
//                     //form1.label_LL.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_LL.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_LA) == Flag_LA)
//                 {
//                     //form1.label_LA.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_LA.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_RA) == Flag_RA)
//                 {
//                     //form1.label_RA.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_RA.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_RL) == Flag_RL)
//                 {
//                     //form1.label_RL.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_RL.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_Chest) == Flag_Chest)
//                 {
//                     //form1.label_Chest.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 //form1.label_Chest.BackColor = System.Drawing.Color.Red;

//             }
//             else if ((electrode & 0xff00) != 0)
//             {
//                 if ((electrode & Flag_C2) == Flag_C2)
//                 {
//                     //form1.label_C2.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_C2.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_C3) == Flag_C3)
//                 {
//                     //form1.label_C3.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_C3.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_C4) == Flag_C4)
//                 {
//                     //form1.label_C4.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_C4.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_C5) == Flag_C5)
//                 {
//                     //form1.label_C5.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_C5.BackColor = System.Drawing.Color.Red;

//                 if ((electrode & Flag_C6) == Flag_C6)
//                 {
//                     //form1.label_C6.BackColor = System.Drawing.Color.LawnGreen;
//                 }
//                 else //form1.label_C6.BackColor = System.Drawing.Color.Red;
//             }
//         }

//         //Bitmap bmp;

//         // draw waveform to graph
//         private int display_wave(channel ch, int i)
//         {

//             int dumint;
//             double factor = (ch._hoehe - gr_rand) / 256.0;

//             Graphics g = Graphics.FromImage(//form1.pictureBox_Graph.Image);
//             Pen drawPen = new Pen(Color.Black, 1);
//             drawPen.DashStyle = DashStyle.Solid;

//             drawPen.Color = ch.Color;
//             dumint = ch._base - (int)Math.Round((double)(Datensatz[i++] * factor));
//             if ((packetsread > 3) && (display_diag_exit == true)) //(* 1. Datensatz nicht anzeigen	*)
//             {
//                 if (ch.xoffset == true)
//                 {
//                     g.DrawLine(drawPen, new Point(index - 1 + //form1.pictureBox_Graph.Width / 2, ch.old_wave), new Point(index + //form1.pictureBox_Graph.Width / 2, dumint));
//                 }
//                 else
//                 {
//                     g.DrawLine(drawPen, new Point(index - 1, ch.old_wave), new Point(index, dumint));
//                 }
//             }
//             ch.old_wave = dumint;

//             g.Dispose();
//             drawPen.Dispose();

//             return i;
//         }

//         // Draw Graphs and save Value
//         private void Display()
//         {

//             //bmp = (Bitmap)//form1.pictureBox_Graph.Image;

//             if ((//form1.pictureBox_Graph.Image != null))
//             {
//                 int i = 0;


//                 packetsread += 1;
//                 if (packetsread > 5)
//                 {
//                     packetsread = 5;
//                 }
//                 foreach (channel Member in Channel)
//                 {
//                     if (Member._hoehe > 0)
//                     {
//                         i = display_wave(Member, i);
//                     }

//                 }
//                 if (1==1)
//                 {
//                     if (Outfilectr > 0)
//                     {
//                         int j = 0;
//                         if (Flag_eg1200 == true)
//                         {
//                             foreach (channel Member in Channel)

//                             {
//                                 if (Member._hoehe > 0)
//                                 {
//                                     Outfile.Write(Datensatz[j++]);
//                                     Outfile.Write("\t");
//                                 }
//                                 else
//                                 {
//                                     Outfile.Write(0);
//                                     Outfile.Write("\t");
//                                 }
//                             }
//                         }
//                         else
//                         {
//                             foreach (channel Member in Channel)
//                             {
//                                 //Control[] temp = //form1.splitContainer1.Panel1.Controls.Find(("checkbox_" + Member.CH_name), true);
//                                 if (1 > 0)
//                                 {
//                                     if (Member._hoehe > 0)
//                                     {
//                                         Outfile.Write(Datensatz[j++]);
//                                         Outfile.Write("\t");
//                                     }
//                                     else
//                                     {
//                                         Outfile.Write(0);
//                                         Outfile.Write("\t");
//                                     }
//                                 }
//                             }
//                         }

//                         Outfile.WriteLine(Outfilectr);
//                         Outfilectr++;
//                     }
//                 }

//             }
//         }

//         // Refresh Picturebox of graph
//         public void update()
//         {
//             if ((display_diag_exit == true) && (Status_ok == true))
//             {

//                 //form1.pictureBox_Graph.Invalidate();


//             }
//         }

//         bool display_diag_exit = false;

//         Bitmap bmp1;

//         // Funktion to draw diagrammlegend
//         private void draw_Diagramm(channel ch)
//         {
//             PictureBox PicB = //form1.pictureBox_Graph;

//             Graphics g = Graphics.FromImage(bmp1);

//             Pen blackPen = new Pen(Color.Black, 1);
//             int dumint;
//             int tempxrand = ((PicB.Width / column) - 10);

//             if (ch._hoehe > 0)      //				(* Achsen zeichnen	*)		
//             {
//                 blackPen.DashStyle = DashStyle.Solid;
//                 blackPen.Color = Color.Black;

//                 if (ch.xoffset == true)
//                 {

//                     g.DrawLine(blackPen, new Point(rand + (PicB.Width / column), ch._base), new Point(((PicB.Width) - 10), ch._base));
//                     g.DrawLine(blackPen, new Point(rand + (PicB.Width / column), ch._base - ch._hoehe + gr_rand), new Point(rand + (PicB.Width / column), ch._base));

//                     g.DrawString(ch.CH_name, new Font(FontFamily.GenericSerif, 10, FontStyle.Bold), new SolidBrush(Color.Black), new Point(rand + (PicB.Width / column), ch._base - ch._hoehe + 4));


//                     blackPen.DashStyle = DashStyle.Dot;
//                     blackPen.Color = Color.Gray;
//                     for (int j = 1; j < 4; j++)
//                     {
//                         dumint = (int)(Math.Round((ch._base - (ch._hoehe - gr_rand) * (0.25 * j)), 0));
//                         g.DrawLine(blackPen, new Point(rand + (PicB.Width / column), dumint), new Point(((PicB.Width) - 10), dumint));

//                     }
//                     for (int j = 1; j < 10; j++)
//                     {
//                         dumint = rand + (PicB.Width / column) + ((int)(Math.Round((double)((tempxrand - rand) * (j / 10.0)))));
//                         g.DrawLine(blackPen, new Point(dumint, ch._base - ch._hoehe + gr_rand), new Point(dumint, ch._base));
//                     }
//                 }
//                 else
//                 {

//                     g.DrawLine(blackPen, new Point(rand, ch._base), new Point(tempxrand, ch._base));
//                     g.DrawLine(blackPen, new Point(rand, ch._base - ch._hoehe + gr_rand), new Point(rand, ch._base));

//                     g.DrawString(ch.CH_name, new Font(FontFamily.GenericSerif, 10, FontStyle.Bold), new SolidBrush(Color.Black), new Point(rand, ch._base - ch._hoehe + 4));


//                     blackPen.DashStyle = DashStyle.Dot;
//                     blackPen.Color = Color.Gray;
//                     for (int j = 1; j < 4; j++)
//                     {
//                         dumint = (int)(Math.Round((ch._base - (ch._hoehe - gr_rand) * (0.25 * j)), 0));
//                         g.DrawLine(blackPen, new Point(rand, dumint), new Point(tempxrand, dumint));

//                     }
//                     for (int j = 1; j < 10; j++)
//                     {
//                         dumint = rand + ((int)(Math.Round((double)((tempxrand - rand) * (j / 10.0)))));
//                         g.DrawLine(blackPen, new Point(dumint, ch._base - ch._hoehe + gr_rand), new Point(dumint, ch._base));
//                     }
//                 }

//             }
//             g.Dispose();
//             //PicB.Dispose();
//             blackPen.Dispose();

//         }

//         // Draw all Diagramms
//         public void display_diag()
//         {


//             PictureBox PicB = //form1.pictureBox_Graph;

//             int Channels = cnt_Channel;

//             int temp_Ch = Channels;

//             display_diag_exit = false;

//             if (temp_Ch > 7)
//             {
//                 temp_Ch = 7;
//             }
//             int Diagram_height = 0;
//             if (temp_Ch > 0)
//             {
//                 Diagram_height = ((PicB.Height - rand) / temp_Ch);
//             }


//             if (Channels > 0)
//             {
//                 if (//form1.checkBox_I.Checked)
//                 {
//                     Channel[(int)cha.chi]._base = Diagram_height;
//                     Channel[(int)cha.chi]._hoehe = Channel[(int)cha.chi]._base;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chi]._base = 0;
//                     Channel[(int)cha.chi]._hoehe = 0;
//                 }

//                 if (//form1.checkBox_II.Checked)
//                 {
//                     Channel[(int)cha.chii]._base = Channel[(int)cha.chi]._base + Diagram_height;
//                     Channel[(int)cha.chii]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chii]._hoehe = 0;
//                     Channel[(int)cha.chii]._base = Channel[(int)cha.chi]._base;
//                 }

//                 if (//form1.checkBox_III.Checked)
//                 {
//                     Channel[(int)cha.chiii]._base = Channel[(int)cha.chii]._base + Diagram_height;
//                     Channel[(int)cha.chiii]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chiii]._hoehe = 0;         //(* nicht darstellen	*)
//                     Channel[(int)cha.chiii]._base = Channel[(int)cha.chii]._base;
//                 }

//                 if (//form1.checkBox_aVR.Checked)
//                 {
//                     Channel[(int)cha.chvr]._base = Channel[(int)cha.chiii]._base + Diagram_height;
//                     Channel[(int)cha.chvr]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chvr]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chvr]._base = Channel[(int)cha.chiii]._base;
//                 }
//                 if (//form1.checkBox_aVL.Checked)
//                 {
//                     Channel[(int)cha.chvl]._base = Channel[(int)cha.chvr]._base + Diagram_height;
//                     Channel[(int)cha.chvl]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chvl]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chvl]._base = Channel[(int)cha.chvr]._base;
//                 }

//                 if (//form1.checkBox_aVF.Checked)
//                 {
//                     Channel[(int)cha.chvf]._base = Channel[(int)cha.chvl]._base + Diagram_height;
//                     Channel[(int)cha.chvf]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chvf]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chvf]._base = Channel[(int)cha.chvl]._base;
//                 }
//                 if (//form1.checkBox_C1.Checked)
//                 {
//                     Channel[(int)cha.chc1]._base = Channel[(int)cha.chvf]._base + Diagram_height;
//                     Channel[(int)cha.chc1]._hoehe = Diagram_height;
//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc1]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc1]._base = Channel[(int)cha.chvf]._base;
//                 }

//                 if (//form1.checkBox_C2.Checked)
//                 {
//                     if ((Channel[(int)cha.chc1]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chc2]._base = 0 + Diagram_height;
//                         Channel[(int)cha.chc2].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc2]._base = Channel[(int)cha.chc1]._base + Diagram_height;
//                         Channel[(int)cha.chc2].xoffset = false;

//                     }
//                     Channel[(int)cha.chc2]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc2]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc2]._base = Channel[(int)cha.chc1]._base;
//                     if (Channel[(int)cha.chc1].xoffset == true)
//                     {
//                         Channel[(int)cha.chc2].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc2].xoffset = false;
//                     }
//                 }

//                 if (//form1.checkBox_C3.Checked)
//                 {
//                     if ((Channel[(int)cha.chc2]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chc3]._base = 0 + Diagram_height;

//                         Channel[(int)cha.chc3].xoffset = true;


//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc3]._base = Channel[(int)cha.chc2]._base + Diagram_height;
//                         if (Channel[(int)cha.chc2].xoffset == true)
//                         {
//                             Channel[(int)cha.chc3].xoffset = true;
//                         }
//                         else
//                         {
//                             Channel[(int)cha.chc3].xoffset = false;
//                         }


//                     }
//                     Channel[(int)cha.chc3]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc3]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc3]._base = Channel[(int)cha.chc2]._base;
//                     if (Channel[(int)cha.chc2].xoffset == true)
//                     {
//                         Channel[(int)cha.chc3].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc3].xoffset = false;
//                     }
//                 }
//                 if (//form1.checkBox_C4.Checked)
//                 {
//                     if ((Channel[(int)cha.chc3]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chc4]._base = 0 + Diagram_height;
//                         Channel[(int)cha.chc4].xoffset = true;


//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc4]._base = Channel[(int)cha.chc3]._base + Diagram_height;
//                         if (Channel[(int)cha.chc3].xoffset == true)
//                         {
//                             Channel[(int)cha.chc4].xoffset = true;
//                         }
//                         else
//                         {
//                             Channel[(int)cha.chc4].xoffset = false;
//                         }

//                     }
//                     Channel[(int)cha.chc4]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc4]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc4]._base = Channel[(int)cha.chc3]._base;
//                     if (Channel[(int)cha.chc3].xoffset == true)
//                     {
//                         Channel[(int)cha.chc4].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc4].xoffset = false;
//                     }
//                 }
//                 if (//form1.checkBox_C5.Checked)
//                 {
//                     if ((Channel[(int)cha.chc4]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chc5]._base = 0 + Diagram_height;
//                         Channel[(int)cha.chc5].xoffset = true;


//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc5]._base = Channel[(int)cha.chc4]._base + Diagram_height;
//                         if (Channel[(int)cha.chc4].xoffset == true)
//                         {
//                             Channel[(int)cha.chc5].xoffset = true;
//                         }
//                         else
//                         {
//                             Channel[(int)cha.chc5].xoffset = false;
//                         }
//                     }
//                     Channel[(int)cha.chc5]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc5]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc5]._base = Channel[(int)cha.chc4]._base;
//                     if (Channel[(int)cha.chc4].xoffset == true)
//                     {
//                         Channel[(int)cha.chc5].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc5].xoffset = false;
//                     }
//                 }
//                 if (//form1.checkBox_C6.Checked)
//                 {
//                     if ((Channel[(int)cha.chc5]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chc6]._base = 0 + Diagram_height;
//                         Channel[(int)cha.chc6].xoffset = true;


//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc6]._base = Channel[(int)cha.chc5]._base + Diagram_height;
//                         if (Channel[(int)cha.chc5].xoffset == true)
//                         {
//                             Channel[(int)cha.chc6].xoffset = true;
//                         }
//                         else
//                         {
//                             Channel[(int)cha.chc6].xoffset = false;
//                         }
//                     }

//                     Channel[(int)cha.chc6]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chc6]._hoehe = 0;          //(* nicht darstellen	*)
//                     Channel[(int)cha.chc6]._base = Channel[(int)cha.chc5]._base;
//                     if (Channel[(int)cha.chc5].xoffset == true)
//                     {
//                         Channel[(int)cha.chc6].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chc6].xoffset = false;
//                     }
//                 }
//                 if (//form1.checkBox_Resp.Checked)
//                 {
//                     if ((Channel[(int)cha.chc6]._base + Diagram_height) > PicB.Height)
//                     {
//                         Channel[(int)cha.chresp]._base = 0 + Diagram_height;
//                         Channel[(int)cha.chresp].xoffset = true;


//                     }
//                     else
//                     {
//                         Channel[(int)cha.chresp]._base = Channel[(int)cha.chc6]._base + Diagram_height;
//                         if (Channel[(int)cha.chc6].xoffset == true)
//                         {
//                             Channel[(int)cha.chresp].xoffset = true;
//                         }
//                         else
//                         {
//                             Channel[(int)cha.chresp].xoffset = false;
//                         }
//                     }
//                     Channel[(int)cha.chresp]._hoehe = Diagram_height;

//                 }
//                 else
//                 {
//                     Channel[(int)cha.chresp]._hoehe = 0;            //(* nicht darstellen	*)
//                     Channel[(int)cha.chresp]._base = Channel[(int)cha.chc6]._base;
//                     if (Channel[(int)cha.chc6].xoffset == true)
//                     {
//                         Channel[(int)cha.chresp].xoffset = true;
//                     }
//                     else
//                     {
//                         Channel[(int)cha.chresp].xoffset = false;
//                     }

//                 }



//                 if (PicB.Height > 0)
//                 {
//                     bmp1 = new Bitmap(PicB.Width, PicB.Height);

//                     Graphics g = Graphics.FromImage(bmp1);

//                     Pen blackPen = new Pen(Color.Black, 1);
//                     g.Clear(Color.White);
//                     string tempstr = Max_Wert.ToString() + " mv";
//                     //g.DrawLine(blackPen, new Point(rand-10,  gr_rand), new Point(rand-10,Diagram_height));
//                     g.DrawString("0 mv", new Font(FontFamily.GenericSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(3, Diagram_height - 5));
//                     g.DrawString(tempstr, new Font(FontFamily.GenericSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new Point(3, gr_rand - 5));

//                     foreach (channel Member in Channel)
//                     {
//                         draw_Diagramm(Member);
//                     }


//                     //form1.pictureBox_Graph.Image = bmp1;
//                     //form1.pictureBox_Graph.Refresh();
//                     display_diag_exit = true;
//                     g.Dispose();
//                     //PicB.Dispose();
//                     blackPen.Dispose();
//                 }
//             }
//         }
//     }
// }
