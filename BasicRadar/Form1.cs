using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ArcheBuddy.Bot.Classes;
using System.Threading;
using DefaultNameSpace;


namespace BasicRadar
{
    public partial class Form1 : Form
    {
        Self me;
        DefaultClass main;
        Pen p3 = new Pen(Color.Green, 2f);



        public Form1(DefaultClass main, Self me)
        {
            this.me = me;
            this.main = main;
            InitializeComponent();
        }
        bool Islooted = false;


        /// <summary>
        /// 
        /// Vars For State Machine between summary's
        /// Get_States() first to use our list
        /// 
        /// </summary>
        /// State Vars Below For Auto chest loot 2.0
        //State1 -Swiming
        bool State_is_swim = false;
        //State2 -Swiming_Down 
        bool State_is_swim_down = false;
        //State3 -Swiming_Up 
        bool State_is_swim_up = false;
        //State4 -Found_Chest
        bool State_is_Chest_Found = false;
        //State5  -Breathing Head Above water 
        bool State_is_Breathing = false;
        //State6 -Using Dahuta bubble?
        bool State_is_Undrwtr_Breathing = false;
        //State7 - can we loot chest
        bool State_is_Chest_lootable = false;
        //State8 -
        bool State_is_ = false;









        /// <summary>
        /// 
        /// Vars For State Machine between summary's
        /// Get_States() first to use our list
        /// 
        /// </summary>

        //bool IsSwimfoward = false;
        //Draw Line Pointing North
        public void drawSelf()
        {
            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;

            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(p3, new Point(halfX + 0, halfY + 0), new Point(halfX + 0, halfY - 40));
        }


        //Figure out enemy point relative to center of pictureBox1
        private Tuple<int, int> relativeVector(int aX, int aY, int bX, int bY)
        {
            int cX = (bX - aX);
            int cY = (bY - aY);

            return Tuple.Create(cX, cY);
        }


        //rotate point
        private Tuple<int, int> rotate_point(int px, int py, int cx, int cy, double angle)
        {

            angle = ((450 - angle) * (Math.PI / 180f));

            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;
            int x = (int)(Math.Cos(angle) * (px - cx) - Math.Sin(angle) * (py - cy) + cx);
            int y = (int)(Math.Sin(angle) * (px - cx) + Math.Cos(angle) * (py - cy) + cy);

            return Tuple.Create(x, y);
        }


        //Get enemies and draw points after rotating
        private void getEnemy()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.Player)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);


                            if (main.isEnemy(obj) == true && main.isStealth(pName) != true)
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";

                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Red);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.RosyBrown);

                            }

                            if (main.isStealth(pName) == true && main.isEnemy(obj) == true)
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Purple);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.MediumPurple);
                            }


                        }
                    }

                }

        }

        //get friendly players, rotate points, draw points
        private void getFriendly()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true) //check if safe for radar
                {
                    if (obj.type == BotTypes.Player)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);


                            if (main.isEnemy(obj) == false && main.isStealth(pName) != true)
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Green);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DarkGreen);

                            }
                        }
                    }
                }
        }


        //very horrible way to see if a creature is a world boss
        private void getBosses()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true) //check if safe for radar
                {
                    if (obj.type == BotTypes.Creature)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (obj.hp <= 600000)
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Blue);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DarkBlue);
                            }



                        }
                    }

                }

        }

        //get vehicles, rotate points, draw points
        private void getVehicles()
        {
            foreach (var obj in main.getCreatures()) //check if safe for radar
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.Slave)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);


                            if (main.isEnemy(obj) == false)
                            {

                                int foundS1 = pName.IndexOf(" ");
                                int foundS2 = pName.IndexOf(" ", foundS1 + 1);
                                pName = pName.Remove(foundS1 + 1, foundS2 - foundS1);
                                //  if (pName.
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Teal);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.SkyBlue);



                                textBox2.Text = pName;
                            }

                            if (main.isEnemy(obj) == true)
                            {

                                int foundS1 = pName.IndexOf(" ");
                                int foundS2 = pName.IndexOf(" ", foundS1 + 1);
                                pName = pName.Remove(foundS1 + 1, foundS2 - foundS1);
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.OrangeRed);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.OrangeRed);


                                textBox2.Text = pName;
                            }


                        }
                    }

                }

        }

        private void getPlants()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (pName == "Wild Ginseng")
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DeepPink);
                            }



                        }
                    }

                }

        }
        ////////////////////////////////////////////////////////////////
        private void AutoFarm()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {
                        if (obj.name != me.name)
                        {

                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;

                            string strbX = bX.ToString();
                            string strbY = bY.ToString();
                            string straY = aY.ToString();
                            string straX = aX.ToString();
                            int ppdist = Int32.Parse(pDist);


                            if (pName == "Recovered Treasure Chest")
                            {
                                if (me.Z == obj.Z)
                                {
                                    main.SwimUp(false);

                                }
                                main.TurnDirectly(obj);
                                obj.getUseSkills();
                                //main.SwimUp(false);
                                main.UseDoodadSkill("Pick up an item.", obj, true);
                                Thread.Sleep(1000);

                            }

                            if (pName == "Sunken Treasure Chest")
                            {
                                textBox2.Text = pName + " Was Found " + pDist + "m";
                                //textBox2.Text = strbX + " Obj X | " + strbY + " obj Y | " + straX + " ME X | " + straY + " me Y";
                                // drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                //drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.Blue);



                                if (ppdist <= 140)
                                {


                                    main.TurnDirectly(obj);
                                    //    main.SwimDown(true);
                                    //  main.MoveForward(true);

                                    if (Islooted == true)
                                    {
                                        main.SwimDown(false);
                                        main.MoveForward(false);
                                    }
                                    if (Islooted == false)
                                    {
                                        main.SwimDown(true);
                                        main.MoveForward(true);
                                        textBox2.Text = "Moving to " + pName;

                                    }

                                    if (pDist == "0")
                                    {
                                        main.SwimDown(false);

                                        main.MoveForward(false);
                                        Islooted = true;
                                        //IsSwimfoward = true;
                                    }
                                    if (me.Z >= 95)
                                    {
                                        main.SwimUp(false);
                                    }
                                    if (me.Z == obj.Z)
                                    {
                                        main.SwimDown(false);

                                    }
                                    if (Islooted == true)
                                    {
                                        // we loot here
                                        textBox2.Text = "Looting " + pName;
                                        obj.getUseSkills();
                                        main.UseDoodadSkill("Spend up to 25 Labor to salvage the debris of the wrecked ship.", obj, true);
                                        Thread.Sleep(1000);
                                        main.SwimUp(true);

                                        if (me.Z == 99)
                                        {

                                            //link to recoverd chest function

                                            Islooted = false;
                                        }
                                        if (pName == "Recoverd Treasure Chest")
                                        {
                                            if (me.Z == obj.Z)
                                            {
                                                main.SwimUp(false);

                                            }
                                            obj.getUseSkills();
                                            textBox2.Text = "Moving to " + pName + " And Looting";
                                            main.UseDoodadSkill("Pick up an item.", obj, true);
                                            Thread.Sleep(1000);
                                            main.SwimUp(false);
                                            Islooted = false;
                                        }


                                    }



                                }



                            }
                        }

                    }
                }
        }
        //Expermental
        private void getChest()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {

                        if (obj.name != me.name)
                        {
                            if (obj.phaseId == 8356)
                            {
                                int halfX = pictureBox1.ClientRectangle.Width / 2;
                                int halfY = pictureBox1.ClientRectangle.Height / 2;
                                string pName = obj.name;
                                string pDist = ((int)main.dist(obj)).ToString();
                                int aX = (int)me.X;
                                int aY = (int)me.Y;
                                int bX = (int)obj.X;
                                int bY = (int)obj.Y;
                                double angle = findAngle(me.turnAngle);
                                Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                                Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                                //set name to event chest name
                                if (pName == "Sunken Treasure Chest")
                                {
                                    if (checkBox11.Checked == false)
                                    {
                                        textBox2.Text = pName + " Was Found " + pDist + "m";
                                    }

                                    drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                    drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.Blue);
                                }



                            }
                        }

                    }
                }
        }

        private void getTSTree()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            //set name to event chest name
                            if (pName == "Thunderstruck Tree")
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.Blue);
                            }



                        }
                    }

                }
        }
        private void getbubs()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (pName == "Buried Ancient Cargo Ship")
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.Blue);
                            }



                        }
                    }

                }
        }

        private void getGoldenChest()
        {



        }



        ////////////////////////////////////////////////////////////////
        private double findAngle(int angle)
        {

            //COUNTERCLOCKWISE
            //TEMPangle -= 43;
            double newAngle = angle * 2.8125;
            /*angle = (int)(angle * 2.8125);

            if (angle > 360)
                angle = 360;

            angle = angle - 127;

            if (angle > 360)
            {
                int theDiff = angle - 360;
                angle = 0 + theDiff;
            }
            if (angle < 0)
                angle = 360 + angle;

            return angle;*/
            //newAngle -= 120.9375;
            newAngle -= 30;
            if (newAngle < 0.0)
                return newAngle + 360.0;
            else
                return newAngle;
        }

        public void drawPoint(int x, int y, Color c)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            SolidBrush brush = new SolidBrush(c);
            Point dPoint = new Point(x, (pictureBox1.Height - y));
            dPoint.X = dPoint.X - 2;
            dPoint.Y = dPoint.Y - 2;
            Rectangle rect = new Rectangle(dPoint, new Size(4, 4));
            g.FillRectangle(brush, rect);
            g.Dispose();
        }

        private void drawString(string y, int a, int b, Brush c)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);

            using (Font myFont = new Font("Arial", 12))
            {
                Point dPoint = new Point(a, (pictureBox1.Height - b));
                dPoint.X = dPoint.X - 2;
                dPoint.Y = dPoint.Y - 2;
                g.DrawString(y, myFont, c, dPoint);
            }
        }

        /*
        private bool figFaction()
        {
            if (me.factionId == 109 || me.factionId == 113) //east
            {

            }

            if (me.factionId == 101 || me.factionId == 103)
            {

            }

        }
        */

        //Allows borderless windows to be moved
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            FarmTimer.Enabled = false;

        }



        private void timer1_Tick_1(object sender, EventArgs e)
        {
            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;
            pictureBox1.Refresh();
            drawPoint(halfX + 0, halfY + 0, Color.Green);
            drawSelf();

            if (checkBox1.Checked)
            {
                getEnemy();
            }
            if (checkBox2.Checked)
            {
                getFriendly();
            }
            if (checkBox3.Checked)
            {
                getbubs();
            }
            if (checkBox4.Checked)
            {
                getPlants();
            }
            if (checkBox5.Checked)
            {
                getBosses();
            }
            if (checkBox6.Checked)
            {
                getChest();
            }
            if (checkBox7.Checked)
            {
                //  getGoldenChest();
            }
            if (checkBox8.Checked)
            {
                getTSTree();
            }

            //Thread.Sleep(10);


        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // pictureBox1.Refresh();
            //pictureBox1.Invalidate();

            //int halfX = pictureBox1.ClientRectangle.Width / 2;
            // int halfY = pictureBox1.ClientRectangle.Height / 2;



            drawSelf();
            // drawPoint(halfX + 0, halfY + 0, Color.White);
            int tmp = me.laborPoints;
            string str = Convert.ToString(tmp);
            label1.Text = "Name: " + me.name;
            label2.Text = "Player Pos: " + "X " + me.X + "   |   " + "Y " + me.Y + "   |   " + "Z " + me.Z;
            label16.Text = "Labor: " + str;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true)
            {

                this.Opacity = 100;
            }
            if (checkBox7.Checked == false)
            {
                this.Opacity = .69;
            }

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox9.Checked == true)
                {
                    checkBox10.Checked = false;
                    //fast refresh
                    timer1.Interval = 100;
                    timer2.Interval = 100;
                }
                if (checkBox9.Checked == false)
                {
                    //enable slow
                    checkBox10.Checked = true;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked == true)
            {
                checkBox9.Checked = false;

                //slow refresh
                timer1.Interval = 550;
                timer2.Interval = 550;
            }
            if (checkBox10.Checked == false)
            {
                //enable fast
                checkBox9.Checked = true;

            }

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == false)
            {
                groupBox2.Visible = false;
            }
            if (checkBox6.Checked == true)
            {
                groupBox2.Visible = true;
            }
        }

        private void FarmTimer_Tick(object sender, EventArgs e)
        {
            AutoFarm();
            //FarmTimer.Enabled = false;
            // if (IsSwimfoward == true)
            //  { main.MoveForward(false); }
            //  if (IsSwimfoward == true)
            // { main.MoveForward(false); }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox11.Checked == true)
            {
                FarmTimer.Enabled = true;

            }
            if (checkBox11.Checked == false)
            {
                main.MoveForward(false);
                main.SwimDown(false);
                main.SwimUp(false);
                FarmTimer.Enabled = false;

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer3_Tick(object sender, EventArgs e)
        {

            if (label17.ForeColor == Color.Red)
            {
                label17.ForeColor = Color.LimeGreen;
            }

            if (label17.ForeColor == Color.LimeGreen)
            {
                label17.ForeColor = Color.Blue;
            }

            if (label17.ForeColor == Color.Blue)
            {
                label17.ForeColor = Color.Red;
            }
        }

        public void Get_States()
        {
            //grab data and apply to variables



        }

    }
}

