using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices;
using myOpenGL.Properties;

namespace myOpenGL
{
    public partial class Form1 : Form
    {
        cOGL cGL;       
        static float up_down = 0;
        static float left_right = 1;
        static float back_forward = 1;
        static float spin = 1;
        static int accel = 0;

        public Form1()
        {
            InitializeComponent();
            cGL = new cOGL(panel1);
            //apply the bars values as cGL.ScrollValue[..] properties 
                                         //!!!
            hScrollBarScroll(hScrollBar1, null);
            hScrollBarScroll(hScrollBar2, null);
            hScrollBarScroll(hScrollBar3, null);
            hScrollBarScroll(hScrollBar4, null);
            hScrollBarScroll(hScrollBar5, null);
            hScrollBarScroll(hScrollBar6, null);
            hScrollBarScroll(hScrollBar7, null);
            hScrollBarScroll(hScrollBar8, null);
            hScrollBarScroll(hScrollBar9, null);
            hScrollBarScroll(hScrollBar10, null);

            this.KeyDown += new KeyEventHandler(Main_KeyDown);
            this.KeyUp += new KeyEventHandler(Main_KeyUp);
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
            this.panel1.Size = new System.Drawing.Size(700, 600);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            cGL.OnResize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            L_X.Enabled = false;
            L_Y.Enabled = false;
            L_Z.Enabled = false;

            hScrollBar10.Enabled = false;
            hScrollBar11.Enabled = false;
            hScrollBar12.Enabled = false;
        }
       //         
        private void hScrollBarScroll(object sender, ScrollEventArgs e)
        {
            cGL.intOptionC = 0;
            HScrollBar hb = (HScrollBar)sender;
            int n = int.Parse(hb.Name.Substring(10));
            cGL.ScrollValue[n - 1] = (hb.Value - 100) / 10.0f;
            if (e != null)
                cGL.Draw();
        }
        
        public float[] oldPos = new float[7];

        private void numericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nUD = (NumericUpDown)sender;
            int i = int.Parse(nUD.Name.Substring(nUD.Name.Length - 1));
            int pos = (int)nUD.Value; 
            switch(i)
            {
                case 1:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xShift += 0.25f;
                        cGL.intOptionC = 4;  //RIGHT X
                    }
                    else
                    {
                        cGL.xShift -= 0.25f;
                        cGL.intOptionC = -4; //LEFT X
                    }
                    break;
                case 2:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yShift += 0.25f;
                        cGL.intOptionC = 5; //UP Y
                    }
                    else
                    {
                        cGL.yShift -= 0.25f;
                        cGL.intOptionC = -5; //DOWN Y
                    }
                    break;
                case 3:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.zShift += 0.25f;
                        cGL.intOptionC = 6;  //forward Z
                    }
                    else
                    {
                        cGL.zShift -= 0.25f;
                        cGL.intOptionC = -6; //backward Z
                    }
                    break;
                case 4:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xAngle += 5;
                        cGL.intOptionC = 1;
                    }
                    else
                    {
                        cGL.xAngle -= 5;
                        cGL.intOptionC = -1;
                    }
                    break;
                case 5:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yAngle += 5;
                        cGL.intOptionC = 2;
                    }
                    else
                    {
                        cGL.yAngle -= 5;
                        cGL.intOptionC = -2;
                    }
                    break;
                case 6: 
	                if (pos>oldPos[i-1]) 
	                {
		                cGL.zAngle+=5;
		                cGL.intOptionC=3;
	                }
	                else
	                {
                        cGL.zAngle -= 5;
                        cGL.intOptionC = -3;
                    }
                    break;
            }
            cGL.Draw();
            oldPos[i - 1] = pos;
            cGL.intOptionC = 0;

        }
        //1
        private void L_X_Scroll(object sender, ScrollEventArgs e)
        {
            cGL.SetLightSourcePosition(L_X.Value / 10.0f,L_Y.Value / 10.0f, L_Z.Value / 10.0f);
            cGL.Draw();
        }

        private void L_X_ValueChanged(object sender, EventArgs e)
        {
            cGL.SetLightSourcePosition(L_X.Value / 10.0f, L_Y.Value / 10.0f, L_Z.Value / 10.0f);
            cGL.Draw();
        }

        private void timerRUN_Tick(object sender, EventArgs e)
        {
            cGL.alpha -= 5;
            cGL.rotangle += 50; //Rotate rotter
            cGL.Draw();
        }

        private void chk_shadow_CheckedChanged(object sender, EventArgs e)
        {
            if (CK_SHDOW.Checked)
            {
                GB_SHADOW.Enabled = true;
                cGL.check_shadow();

               //// cGL.intOptionC = 0;
               // HScrollBar hb = hScrollBar11;
                int n = int.Parse(hScrollBar11.Name.Substring(10));
                int n1 = int.Parse(hScrollBar12.Name.Substring(10));
                cGL.ScrollValue[n - 1] = (hScrollBar11.Value - 100) / 10.0f;
                cGL.ScrollValue[n1 - 1] = (hScrollBar12.Value - 100) / 10.0f;

                hScrollBar10.Enabled = true;
                hScrollBar11.Enabled = true;
                hScrollBar12.Enabled = true;

                cGL.Draw();
            }
            else
            {
                hScrollBar10.Enabled = false;
                hScrollBar11.Enabled = false;
                hScrollBar12.Enabled = false;

                
                GB_SHADOW.Enabled = false;
                cGL.check_shadow();
                cGL.Draw();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cGL.alpha -= 2;
            cGL.Draw();
        }

        private void chk_reflection_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_reflection.Checked)
            {
                cGL.check_reflection();
                cGL.Draw();
            }
            else
            {
                cGL.check_reflection();
                cGL.Draw();
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        up_down = up_down + 1;
                        cGL.heli_y_pos = up_down * 0.1;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    up_down = up_down + 1;
                    cGL.heli_y_pos = up_down * 0.1;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.S)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        if(up_down < 1)
                            cGL.Draw();
                        else
                        up_down = up_down - 1;
                        cGL.heli_y_pos = up_down * 0.1;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    up_down = up_down - 1;
                    cGL.heli_y_pos = up_down * 0.1;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
            }

            if (e.KeyCode == Keys.O)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        left_right = left_right - 0.2f;
                        cGL.heli_z_pos = left_right;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    left_right = left_right - 0.2f;
                    cGL.heli_z_pos = left_right;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
                
                //cGL.intOptionC = 6;
            }
            if (e.KeyCode == Keys.L)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        left_right = left_right + 0.2f;
                        cGL.heli_z_pos = left_right;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    left_right = left_right + 0.2f;
                    cGL.heli_z_pos = left_right;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
                
              //  cGL.intOptionC = 0;
            }

            if (e.KeyCode == Keys.A)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        back_forward = back_forward - 0.2f;
                        cGL.heli_x_pos = back_forward;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    back_forward = back_forward - 0.2f;
                    cGL.heli_x_pos = back_forward;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
                
            }
            if (e.KeyCode == Keys.D)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        back_forward = back_forward + 0.2f;
                        cGL.heli_x_pos = back_forward;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    back_forward = back_forward + 0.2f;
                    cGL.heli_x_pos = back_forward;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
                
            }
             if (e.KeyCode == Keys.OemSemicolon)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        spin = spin + 1f;
                        cGL.heli_spin_angle = spin;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    spin = spin + 1f;
                    cGL.heli_spin_angle = spin;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
               
            }
            if (e.KeyCode == Keys.K)
            {
                if (accel == 0)
                    accel += 1;
                else if (accel == 5)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        spin = spin - 1f;
                        cGL.heli_spin_angle = spin;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                }
                else
                {
                    spin = spin - 1f;
                    cGL.heli_spin_angle = spin;
                    accel += 1;
                    cGL.alpha -= 5;
                    cGL.rotangle += 50;
                    cGL.Draw();
                }
            }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        up_down = up_down + (float)(0.2*accel);
                        cGL.heli_y_pos = up_down * 0.1;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.S)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        if (up_down < 1)
                            cGL.Draw();
                        else
                        up_down = up_down - (0.2f * accel);
                        cGL.heli_y_pos = up_down * 0.1;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.O)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        left_right = left_right - (0.04f*accel);
                        cGL.heli_z_pos = left_right;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.L)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        left_right = left_right + (0.04f * accel);
                        cGL.heli_z_pos = left_right;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.A)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        back_forward = back_forward - (0.04f * accel);
                        cGL.heli_x_pos = back_forward;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.D)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        back_forward = back_forward + (0.04f * accel);
                        cGL.heli_x_pos = back_forward;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.OemSemicolon)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        spin = spin + (0.2f*accel);
                        cGL.heli_spin_angle = spin;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
            if (e.KeyCode == Keys.K)
            {

                while (accel > 0)
                {
                    for (int i = 0; i < accel; i++)
                    {
                        spin = spin - (0.2f * accel);
                        cGL.heli_spin_angle = spin;
                        cGL.alpha -= 5;
                        cGL.rotangle += 50;
                        cGL.Draw();
                    }
                    accel -= 1;
                    cGL.Draw();
                }
            }
        }

        private void chk_light_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chk_light.Checked)
            {
                cGL.llight = true;
                L_X.Enabled = true;
                L_Y.Enabled = true;
                L_Z.Enabled = true;

            }
            else
            {

                cGL.llight = false;
                //GL.glDisable(GL.GL_LIGHTING);
                cGL.ar[0] = 0;
                cGL.ar[1] = 0;
                cGL.ar[2] = 0;
                cGL.ar[3] = 0;
                GL.glEnable(GL.GL_LIGHTING);
                GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, cGL.ar);

                //L_X.Value = 100;
                //L_Y.Value = 100;
                //L_Z.Value = 100;

                L_X.Enabled = false;
                L_Y.Enabled = false;
                L_Z.Enabled = false;


                cGL.Draw();
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            timerRUN.Enabled = !timerRUN.Enabled;
            if (timerRUN.Enabled)
                Start.Text = "Started";
            else
                Start.Text = "Stopped";
        }

        private void Axes_TextChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }


       

    }
}