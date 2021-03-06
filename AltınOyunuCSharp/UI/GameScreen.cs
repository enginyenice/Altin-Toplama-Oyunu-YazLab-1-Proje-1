﻿using AltınOyunuCSharp.Game.Map.Concrete; //Map sınıfının bulunduğu adres
using AltınOyunuCSharp.Game.Player.Concrete; // Oyuncu sınıfının bulunduğu adres
using AltınOyunuCSharp.Game.Player.Concrete.Players; //Oyuncuların sınıflarının bulunduğu adres
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AltınOyunuCSharp.UI
{
    public partial class GameScreen : Form
    {
        private Bitmap bitmapBoard, bitmap;
        private Graphics graphBoard, graph;
        private readonly Image[] goldImages;
        private readonly Image[] hiddenGoldImages;
        private readonly Image aPlayerImage;
        private readonly Image bPlayerImage;
        private readonly Image cPlayerImage;
        private readonly Image dPlayerImage;
        private int squareEdge = 75; // Oyun alanı kare kenar uzunluğu, default: 75 pixel
        private int cordNumberGuide; // Oyun alanı kılavuz numaralarının yerleşim uzunluğu. squareEdge / 2

        public Form menuForm;
        public Map map;
        public APlayer aPlayer;
        public BPlayer bPlayer;
        public CPlayer cPlayer;
        public DPlayer dPlayer;
        private bool hiddenActive = false;

        public GameScreen(Map gameMap, APlayer a, BPlayer b, CPlayer c, DPlayer d, Form menuForm)
        {
            this.menuForm = menuForm;
            map = gameMap;
            aPlayer = a;
            bPlayer = b;
            cPlayer = c;
            dPlayer = d;
            goldImages = new Image[4];
            hiddenGoldImages = new Image[4];
            cordNumberGuide = (squareEdge * 3) / 4;

            goldImages[0] = global::AltınOyunuCSharp.Properties.Resources.coin5;
            goldImages[1] = global::AltınOyunuCSharp.Properties.Resources.coin10;
            goldImages[2] = global::AltınOyunuCSharp.Properties.Resources.coin15;
            goldImages[3] = global::AltınOyunuCSharp.Properties.Resources.coin20;
            hiddenGoldImages[0] = global::AltınOyunuCSharp.Properties.Resources.hiddenCoin5;
            hiddenGoldImages[1] = global::AltınOyunuCSharp.Properties.Resources.hiddenCoin10;
            hiddenGoldImages[2] = global::AltınOyunuCSharp.Properties.Resources.hiddenCoin15;
            hiddenGoldImages[3] = global::AltınOyunuCSharp.Properties.Resources.hiddenCoin20;
            aPlayerImage = global::AltınOyunuCSharp.Properties.Resources.playerA_front;
            bPlayerImage = global::AltınOyunuCSharp.Properties.Resources.playerB_front;
            cPlayerImage = global::AltınOyunuCSharp.Properties.Resources.playerC_front;
            dPlayerImage = global::AltınOyunuCSharp.Properties.Resources.playerD_front;
            InitializeComponent();
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            //Oyun Formunun ekrana sığdırılması
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Location = new Point(0, 0);
            aPlayerKasa.Text = aPlayer.GetPlayerGold().ToString();
            bPlayerKasa.Text = bPlayer.GetPlayerGold().ToString();
            cPlayerKasa.Text = cPlayer.GetPlayerGold().ToString();
            dPlayerKasa.Text = dPlayer.GetPlayerGold().ToString();
            TimerIntervalNum.Value = tm.Interval;
            GraphicBoardSetup();
        }

        private void GameScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            menuForm.Show();
        }

        public void GraphicBoardSetup()
        {
            int mapy = map.GetMap().GetLength(0);
            int mapx = map.GetMap().GetLength(1);
            //Karelerin kenar boyutlarının, oyun alanının genişlik ve yüksekliğine göre
            //ekrana sığabilecek max boyuta getirilmesi
            while ((squareEdge * mapy) + cordNumberGuide > gamePictureBox.Height)
            {
                --squareEdge;
                cordNumberGuide = squareEdge / 2;
            }
            while ((squareEdge * mapx) + cordNumberGuide > gamePictureBox.Width)
            {
                --squareEdge;
                cordNumberGuide = squareEdge / 2;
            }

            //Picturebox'ın oyun alanı boyutlarına getirilmesi
            gamePictureBox.Height = (squareEdge * mapy) + cordNumberGuide;
            gamePictureBox.Width = (squareEdge * mapx) + cordNumberGuide;

            //Picturebox'ın panel içerisinde ortalanması
            int panelx = (((gamePanel.Width) - gamePictureBox.Width) / 2) + 10;
            int panely = (((gamePanel.Height) - gamePictureBox.Height) / 2) + 10;
            gamePictureBox.Location = new Point(panelx, panely);

            //Oyun alanının oluşturulması ve çizdirilmesi
            bitmapBoard = new Bitmap(gamePictureBox.Width, gamePictureBox.Height);
            graphBoard = Graphics.FromImage(bitmapBoard);
            Color penColor = Color.FromArgb(100, 100, 100);
            Pen pen = new Pen(penColor, 2.0F);
            SolidBrush brush = new SolidBrush(Color.Black);
            Font font = new Font("Calibri", ((cordNumberGuide) / 2) + 1);
            int gap = (squareEdge - cordNumberGuide) / 2;
            for (int i = -1; i < mapy; i++)
            {
                for (int j = -1; j < mapx; j++)
                {
                    if (i == -1)
                    {
                        if (j == -1)
                        {
                        }
                        else if (j == 0)
                            graphBoard.DrawString(Convert.ToString(j), font, brush, cordNumberGuide + gap, 0);
                        else
                            graphBoard.DrawString(Convert.ToString(j), font, brush, (squareEdge * (j + 1)) - gap, 0);
                    }
                    else
                    {
                        if (j == -1 && i == 0)
                            graphBoard.DrawString(Convert.ToString(i), font, brush, 0, cordNumberGuide + gap);
                        else if (j == -1 && i > 0)
                            graphBoard.DrawString(Convert.ToString(i), font, brush, 0, (squareEdge * (i + 1)) - gap);
                        else if (j >= 0 && i >= 0)
                            graphBoard.DrawRectangle(pen, (squareEdge * (j)) + cordNumberGuide, (squareEdge * (i)) + cordNumberGuide, squareEdge, squareEdge);
                    }
                }
            }
            graphBoard.DrawRectangle(pen, cordNumberGuide, cordNumberGuide, (squareEdge * mapx) - 1, (squareEdge * mapy) - 1);
            graphBoard.Dispose();

            // altın, player
            bitmap = new Bitmap(bitmapBoard);
            graph = Graphics.FromImage(bitmap);
            for (int i = 0; i < mapy; i++)
            {
                for (int j = 0; j < mapx; j++)
                {
                    if (map.GetGoldPointValue(i, j) != 0)
                        graph.DrawImage(goldImages[(map.GetGoldPointValue(i, j) / 5) - 1], (squareEdge * j) + cordNumberGuide, (squareEdge * i) + cordNumberGuide, squareEdge, squareEdge);
                }
            }
            graph.DrawImage(aPlayerImage, cordNumberGuide, cordNumberGuide, squareEdge, squareEdge);
            graph.DrawImage(bPlayerImage, squareEdge * (mapx - 1) + cordNumberGuide, cordNumberGuide, squareEdge, squareEdge);
            graph.DrawImage(cPlayerImage, cordNumberGuide, squareEdge * (mapy - 1) + cordNumberGuide, squareEdge, squareEdge);
            graph.DrawImage(dPlayerImage, squareEdge * (mapx - 1) + cordNumberGuide, squareEdge * (mapy - 1) + cordNumberGuide, squareEdge, squareEdge);
            gamePictureBox.Image = bitmap;
            CordGuide();
        }

        public void GraphicDraw()
        {
            graph = Graphics.FromImage(bitmap);
            graph.Clear(Color.Transparent);
            graph.DrawImage(bitmapBoard, 0, 0);
            Pen pen = new Pen(Color.Red, 2);
            for (int i = 0; i < map.GetMap().GetLength(0); i++)
            {
                for (int j = 0; j < map.GetMap().GetLength(1); j++)
                {
                    // Altın resimleri
                    if (map.GetGoldPointValue(i, j) != 0)
                        graph.DrawImage(goldImages[(map.GetGoldPointValue(i, j) / 5) - 1], (squareEdge * j) + cordNumberGuide, (squareEdge * i) + cordNumberGuide, squareEdge, squareEdge);
                    // Gizli altın resimleri (eğer gizli altınlar gösterilmek istenmiş ise)
                    else if (map.GetPrivateGoldPointValue(i, j) != 0 && hiddenActive == true)
                    {
                        graph.DrawImage(hiddenGoldImages[(map.GetPrivateGoldPointValue(i, j) / 5) - 1], (squareEdge * j) + cordNumberGuide, (squareEdge * i) + cordNumberGuide, squareEdge, squareEdge);
                        graph.DrawRectangle(pen, (squareEdge * j) + cordNumberGuide + 1, (squareEdge * i) + cordNumberGuide + 1, squareEdge - 2, squareEdge - 2);
                    }
                    // Oyuncu resimleri
                    if (aPlayer.GetPlayerMatris()[i, j] != 0)
                        graph.DrawImage(aPlayerImage, (j * squareEdge) + cordNumberGuide, (i * squareEdge) + cordNumberGuide, squareEdge, squareEdge);
                    else if (bPlayer.GetPlayerMatris()[i, j] != 0)
                        graph.DrawImage(bPlayerImage, (j * squareEdge) + cordNumberGuide, (i * squareEdge) + cordNumberGuide, squareEdge, squareEdge);
                    else if (cPlayer.GetPlayerMatris()[i, j] != 0)
                        graph.DrawImage(cPlayerImage, (j * squareEdge) + cordNumberGuide, (i * squareEdge) + cordNumberGuide, squareEdge, squareEdge);
                    else if (dPlayer.GetPlayerMatris()[i, j] != 0)
                        graph.DrawImage(dPlayerImage, (j * squareEdge) + cordNumberGuide, (i * squareEdge) + cordNumberGuide, squareEdge, squareEdge);
                }
            }
            gamePictureBox.Image = bitmap;
        }

        public void CordGuide()
        {
            Bitmap btm = new Bitmap(150, 150);
            Graphics g = Graphics.FromImage(btm);
            Pen pen = new Pen(Color.Black, 3);
            Font font = new Font("Calibri", 19);
            SolidBrush brush = new SolidBrush(Color.Black);

            g.DrawLine(pen, (10 + 10), (10 + 10), (75 + 10), (10 + 10));
            g.DrawLine(pen, (10 + 10), (10 + 10), (10 + 10), (75 + 10));
            g.DrawLine(pen, (75 + 10), (10 + 10), (70 + 10), (4 + 10));
            g.DrawLine(pen, (75 + 10), (10 + 10), (70 + 10), (14 + 10));
            g.DrawLine(pen, (10 + 10), (75 + 10), (4 + 10), (70 + 10));
            g.DrawLine(pen, (10 + 10), (75 + 10), (14 + 10), (70 + 10));
            g.DrawString("Y", font, brush, (1 + 9), (76 + 5));
            g.DrawString("X", font, brush, (76 + 9), (1 + 5));
            gamePanel.BackgroundImage = btm;
        }

        public void PlayerDeathPictureDraw(Player player)
        {
            Bitmap bt = new Bitmap(1, 1);
            Pen pen = new Pen(Color.Red, 5);
            switch (player.GetName())
            {
                case "A":
                    bt = new Bitmap(AplayerPicture.BackgroundImage);
                    break;

                case "B":
                    bt = new Bitmap(BplayerPicture.BackgroundImage);
                    break;

                case "C":
                    bt = new Bitmap(CplayerPicture.BackgroundImage);
                    break;

                case "D":
                    bt = new Bitmap(DplayerPicture.BackgroundImage);
                    break;
            }
            graph = Graphics.FromImage(bt);
            graph.DrawLine(pen, 2, 2, bt.Width - 2, bt.Height - 2);
            graph.DrawLine(pen, 2, bt.Height - 2, bt.Width - 2, 2);
            switch (player.GetName())
            {
                case "A":
                    AplayerPicture.BackgroundImage = bt;
                    break;

                case "B":
                    BplayerPicture.BackgroundImage = bt;
                    break;

                case "C":
                    CplayerPicture.BackgroundImage = bt;
                    break;

                case "D":
                    DplayerPicture.BackgroundImage = bt;
                    break;
            }
        }

        private void TimerIntervalNum_ValueChanged(object sender, EventArgs e)
        {
            tm.Interval = Convert.ToInt32(TimerIntervalNum.Value);
        }

        public string GameOrderString(int order)
        {
            switch (order)
            {
                case 1: return "A";
                case 2: return "B";
                case 3: return "C";
                case 4: return "D";
                default:
                    break;
            }
            return "";
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            switch (map.GetGameOrder())
            {
                case 1:
                    if (aPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(aPlayer);
                        aPlayer.SetPlayerMapValue(aPlayer.GetLastCord()[0], aPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        ALog.Items.Clear();
                        for (int i = aPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            ALog.Items.Add(aPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    aPlayer.Move(map);
                    aPlayerKasa.Text = aPlayer.GetPlayerGold().ToString();
                    if (aPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(aPlayer);
                        aPlayer.SetPlayerMapValue(aPlayer.GetLastCord()[0], aPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        ALog.Items.Clear();
                        for (int i = aPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            ALog.Items.Add(aPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    GraphicDraw();
                    ALog.Items.Clear();
                    for (int i = aPlayer.GetLog().Count - 1; i >= 0; i--)
                    {
                        ALog.Items.Add(aPlayer.GetLog()[i]);
                    }
                    map.SetGameOrder();
                    siradaki.Text = GameOrderString(map.GetGameOrder());
                    break;

                case 2:
                    if (bPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(bPlayer);
                        bPlayer.SetPlayerMapValue(bPlayer.GetLastCord()[0], bPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        BLog.Items.Clear();
                        for (int i = bPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            BLog.Items.Add(bPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    bPlayer.Move(map);
                    bPlayerKasa.Text = bPlayer.GetPlayerGold().ToString();
                    if (bPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(bPlayer);
                        bPlayer.SetPlayerMapValue(bPlayer.GetLastCord()[0], bPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        BLog.Items.Clear();
                        for (int i = bPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            BLog.Items.Add(bPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    GraphicDraw();
                    BLog.Items.Clear();
                    for (int i = bPlayer.GetLog().Count - 1; i >= 0; i--)
                    {
                        BLog.Items.Add(bPlayer.GetLog()[i]);
                    }
                    map.SetGameOrder();
                    siradaki.Text = GameOrderString(map.GetGameOrder());
                    break;

                case 3:
                    if (cPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(cPlayer);
                        cPlayer.SetPlayerMapValue(cPlayer.GetLastCord()[0], cPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        CLog.Items.Clear();
                        for (int i = cPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            CLog.Items.Add(cPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    cPlayer.Move(map);
                    cPlayerKasa.Text = cPlayer.GetPlayerGold().ToString();
                    if (cPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(cPlayer);
                        cPlayer.SetPlayerMapValue(cPlayer.GetLastCord()[0], cPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        CLog.Items.Clear();
                        for (int i = cPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            CLog.Items.Add(cPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    GraphicDraw();
                    CLog.Items.Clear();
                    for (int i = cPlayer.GetLog().Count - 1; i >= 0; i--)
                    {
                        CLog.Items.Add(cPlayer.GetLog()[i]);
                    }
                    map.SetGameOrder();
                    siradaki.Text = GameOrderString(map.GetGameOrder());
                    break;

                case 4:
                    if (dPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(dPlayer);
                        dPlayer.SetPlayerMapValue(dPlayer.GetLastCord()[0], dPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        DLog.Items.Clear();
                        for (int i = dPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            DLog.Items.Add(dPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    dPlayer.Move(map);
                    dPlayerKasa.Text = dPlayer.GetPlayerGold().ToString();
                    if (dPlayer.IsDeath() == true)
                    {
                        map.RemovePlayersIsDeath(map.GetGameOrder());
                        map.SetGameOrder();
                        siradaki.Text = GameOrderString(map.GetGameOrder());
                        PlayerDeathPictureDraw(dPlayer);
                        dPlayer.SetPlayerMapValue(dPlayer.GetLastCord()[0], dPlayer.GetLastCord()[1], 0);
                        GraphicDraw();
                        DLog.Items.Clear();
                        for (int i = dPlayer.GetLog().Count - 1; i >= 0; i--)
                        {
                            DLog.Items.Add(dPlayer.GetLog()[i]);
                        }
                        break;
                    }
                    GraphicDraw();
                    DLog.Items.Clear();
                    for (int i = dPlayer.GetLog().Count - 1; i >= 0; i--)
                    {
                        DLog.Items.Add(dPlayer.GetLog()[i]);
                    }
                    map.SetGameOrder();
                    siradaki.Text = GameOrderString(map.GetGameOrder());
                    break;

                default:
                    break;
            }

            if (map.GetgameOver() == true)
            {
                tm.Stop();
                MessageBox.Show(map.GetgameOverReason() + " Oyun bitti.");
                aPlayer.SetLog(map.GetgameOverReason());
                bPlayer.SetLog(map.GetgameOverReason());
                cPlayer.SetLog(map.GetgameOverReason());
                dPlayer.SetLog(map.GetgameOverReason());
                aPlayer.SetLog("Oyun Bitti");
                bPlayer.SetLog("Oyun Bitti");
                cPlayer.SetLog("Oyun Bitti");
                dPlayer.SetLog("Oyun Bitti");
                ScoreBoard scoreBoard = new ScoreBoard(aPlayer, bPlayer, cPlayer, dPlayer, map, this);
                scoreBoard.Show();
            }
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            if (StartBtn.Text == "Oyunu başlat")
            {
                StartBtn.Text = "Oyunu durdur";
                tm.Start();
            }
            else
            {
                StartBtn.Text = "Oyunu başlat";
                tm.Stop();
            }
        }

        private void HiddenGoldBtn_Click(object sender, EventArgs e)
        {
            if (hiddenActive == true)
            {
                hiddenActive = false;
                HiddenGoldBtn.Text = "Gizli altınları göster";
            }
            else
            {
                hiddenActive = true;
                HiddenGoldBtn.Text = "Gizli altınları gizle";
            }
            GraphicDraw();
        }
    }
}