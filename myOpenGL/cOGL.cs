using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;
        GLUquadric obj;
      public  float[] ar = new float[4];
        bool shadow = false;
        bool reflect = false;

        public bool llight=false;
        
        public double heli_ground_angle = -86.0f;
        public float heli_spin_angle = 0.0f;
        
        public double heli_z_pos = 1f;
        public double heli_x_pos = 2.0f;
        public double heli_y_pos = -0.5f;

        
        
        //public int intOptionC = 0;

        
        
        // Code from mapping example
        // Planes for texture-coordinate generation.       

        public float rotangle; // To rotate the rotor
        float rot1 = 0; //To rotate/open door
        float rot2 = 0; //To rotate/open door
        //GLUquadric *quad;

        public float[] ScrollValue = new float[20];
        public float zShift = 0.0f; // smalls arrows
        public float yShift = 0.0f;
        public float xShift = 0.0f;
        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;
        double[] AccumulatedRotationsTraslations = new double[16];

        //--------------------------------------------------
        //shadow
        float[] planeCoeff = { 1, 1, 1, 1 };
        public float[] pos = new float[4];
        float[,] ground = new float[3, 3];//{ { 1, 1, -0.5 }, { 0, 1, -0.5 }, { 1, 0, -0.5 } };
        float[,] wall = new float[3, 3];//{ { -15, 3, 0 }, { 15, 3, 0 }, { 15, 3, 15 } };

        public void SetLightSourcePosition(double x, double y, double z)
        {
            
            if(llight) // light button
            {
           
            ar[0] = (float)x - 10f;
            ar[1] = (float)y - 10f;
            ar[2] = (float)z - 10f;

            ar[3] = 1f;
            GL.glEnable(GL.GL_LIGHTING);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, ar);
            }
        }

        public cOGL(Control pb)
        {
            p = pb;
            Width = p.Width;
            Height = p.Height;

            /*******************/

            ground[0, 0] = 1;
            ground[0, 1] = 1;
            ground[0, 2] = -0.5f;

            ground[1, 0] = 0;
            ground[1, 1] = 1;
            ground[1, 2] = -0.5f;

            ground[2, 0] = 1;
            ground[2, 1] = 0;
            ground[2, 2] = -0.5f;

            wall[0, 0] = -15;
            wall[0, 1] = 3;
            wall[0, 2] = 0;

            wall[1, 0] = 15;
            wall[1, 1] = 3;
            wall[1, 2] = 0;

            wall[2, 0] = 15;
            wall[2, 1] = 3;
            wall[2, 2] = 15;
            /**********************/

            InitializeGL();
            obj = GLU.gluNewQuadric(); 
        }

        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj); //   A MUST in case of previous call to gluNewQuadric
            WGL.wglDeleteContext(m_uint_RC);
        }

        uint m_uint_HWND = 0;

        public uint HWND
        {
            get { return m_uint_HWND; }
        }

        uint m_uint_DC = 0;

        public uint DC
        {
            get { return m_uint_DC; }
        }
        uint m_uint_RC = 0;

        public uint RC
        {
            get { return m_uint_RC; }
        }

        public void check_shadow()
        {
            if (shadow)
                shadow = false;
            else
                shadow = true;
        }
        public void check_reflection()
        {
            if (reflect)
                reflect = false;
            else
                reflect = true;
        }
     
       
        void DrawFloor()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            //!!! for blended REFLECTION 
            GL.glColor4d(1, 1, 1, 0.5);
            GL.glTexCoord2d(0, 0);
            GL. glVertex3d(-100, 1, -100);
            GL.glTexCoord2d(1, 0);
            GL. glVertex3d(-100, 1,100);
            GL.glTexCoord2d(1, 1);
            GL. glVertex3d(100, 1, 100);
            GL.glTexCoord2d(0, 1);
            GL. glVertex3d(100, 1, -100);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
        }
    
        //waves
        void DrawFloor_large()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            //!!! for blended REFLECTION 
            GL.glColor3d(1, 1, 1);
            GL.glTexCoord2d(0, 0);
            GL. glVertex3d(-100, 1, -100);
            GL.glTexCoord2d(1, 0);
            GL. glVertex3d(-100, 1, 200);
            GL.glTexCoord2d(1, 1);
            GL. glVertex3d(100, 1, 200);
            GL.glTexCoord2d(0, 1);
            GL. glVertex3d(100, 1, -100);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
        } 
                        
        // Reduces a normal vector specified as a set of three coordinates,
        // to a unit normal vector of length one.
        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from working hard for close to zero values
            if (length == 0.0f)
                length = 1.0f;

            // accuiring unit vector
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;

        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = v1[y] * v2[z] - v1[z] * v2[y];
            outp[y] = v1[z] * v2[x] - v1[x] * v2[z];
            outp[z] = v1[x] * v2[y] - v1[y] * v2[x];

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        float[] cubeXform = new float[16];

        // Creates a shadow projection matrix out of the plane equation
        // coefficients and the position of the light. The return value is stored
        // in cubeXform[,]
        void MakeShadowMatrix(float[,] points)
        {
            float[] planeCoeff = new float[4];
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }
        //Shadows

        //--------------------------------------------------

        void Draw_shadow_source_red()
        {

            //shadow source red
            GL.glPushMatrix();
            ////Draw Light Source
            GL.glDisable(GL.GL_LIGHTING);
            GL.glTranslatef(pos[0] - 2, pos[1] + 7, pos[2] + 3);
            //RED Light source
            GL.glColor3f(1, 0, 0);
            GLUT.glutSolidSphere(0.5f, 8, 8);
            GL.glTranslatef(-pos[0], pos[1], -pos[2]);
            GL.glPopMatrix();


            
        }
        // world cube
        void DrawTexturedCube()
        {
            //front
            GL.glDisable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);

            //
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            GL.glColor3f(1.0f,  1.0f, 1.0f);

            GL.glTexCoord2f(0.0f, 0.0f);
            GL.glVertex3f(-40.0f, 0.0f, -50.0f);

            GL.glTexCoord2f(1.0f, 0.0f);
            GL.glVertex3f(-40.0f, 50.0f, -50.0f);

            GL.glTexCoord2f(1.0f, 1.0f);
            GL.glVertex3f(40.0f, 50.0f, -50.0f);

            GL.glTexCoord2f(0.0f, 1.0f);
            GL.glVertex3f(40.0f, 0.0f, -50.0f);

            

            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);



            GL.glDisable(GL.GL_BLEND);
            //
            GL.glEnable(GL.GL_TEXTURE_2D);

            //back

            GL.glDisable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[8]);


            //
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            GL.glColor3f(1.0f, 1.0f, 1.0f);

            GL.glTexCoord2f(0.0f, 0.0f);
            GL.glVertex3f(-40.0f, 0.0f, 30.0f);

            GL.glTexCoord2f(1.0f, 0.0f);
            GL.glVertex3f(-40.0f, 50.0f, 30.0f);

            GL.glTexCoord2f(1.0f, 1.0f);
            GL.glVertex3f(40.0f, 50.0f, 30.0f);

            GL.glTexCoord2f(0.0f, 1.0f);
            GL.glVertex3f(40.0f, 0.0f, 30.0f);



            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);



            GL.glDisable(GL.GL_BLEND);
            //
            GL.glEnable(GL.GL_TEXTURE_2D);

            //top and side

            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);

            GL.glColor3f(1.0f, 1.0f, 1.0f);
            GL.glTexCoord2f(0.0f, 0.0f);
            GL.glVertex3f(-40.0f, 0.0f, -50.0f);

            GL.glTexCoord2f(1.0f, 0.0f);
            GL.glVertex3f(-40.0f, 50.0f, -50.0f);

            GL.glTexCoord2f(1.0f, 1.0f);
            GL.glVertex3f(-40.0f, 50.0f, 30.0f);

            GL.glTexCoord2f(0.0f, 1.0f);
            GL.glVertex3f(-40.0f, 0.0f, 30.0f);


            //3


            //4 в
            
            GL.glColor3f(1.0f, 1.0f, 1.0f);
          //  GL.glColor3f(0.78f, 0.38f, 0.078f);
          //  GL.glTexCoord2f(0.75f, 1.0f);
            GL.glTexCoord2f(1.0f, 0.0f);
            GL.glVertex3f(40.0f, 50.0f, -50.0f);

          //  GL.glTexCoord2f(0.75f, 0.0f);
            GL.glTexCoord2f(0.0f, 0.0f);
            GL.glVertex3f(40.0f, 0.0f, -50.0f);

           // GL.glTexCoord2f(0.5f, 0.0f);
            GL.glTexCoord2f(0.0f, 1.0f);
            GL.glVertex3f(40.0f, 0.0f, 30.0f);

          //  GL.glTexCoord2f(0.5f, 1.0f);
           
            GL.glTexCoord2f(1.0f, 1.0f);
            GL.glVertex3f(40.0f, 50.0f, 30.0f);
            GL.glDisable(GL.GL_TEXTURE_2D);




            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);

            //other side
            GL.glDisable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
            // GL.glDepthRange(0.8, 2);
            //  GLU.gluPerspective(0, 0, 0.8, 2);
            //GLU.gluLookAt(0.0, 0.0, 5.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);

            GL.glDisable(GL.GL_LIGHTING);
            GL.glBegin(GL.GL_QUADS);
            //GL.glColor3f(1.0f, 1.0f, 1.0f);

            //5 вв


            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
            GL.glColor3f(1.0f, 1.0f, 1.0f);
            GL.glTexCoord2f(0.0f, 0.0f);
            GL.glVertex3f(-40.0f, 50.0f, -50.0f);
            GL.glTexCoord2f(0.0f, 0.1f);
            GL.glVertex3f(40.0f, 50.0f, -50.0f);
            GL.glTexCoord2f(1.0f, 1.0f);
            GL.glVertex3f(40.0f, 50.0f, 30.0f);
            GL.glVertex3f(-40.0f, 50.0f, 30.0f);
            GL.glTexCoord2f(1.0f, 0.0f);
            GL.glVertex3f(40.0f, 50.0f, 30.0f);


            //6 р
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);

        }

        //--------------------------------------------------
        float angle = 0.0f;
        public void Draw()
        {
            //Shadows
            pos[0] = ScrollValue[9];
            pos[1] = ScrollValue[10];
            pos[2] = ScrollValue[11];
            pos[3] = 1.0f;
          
            //Shadows

            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT );


            //FULL and COMPLICATED

            GL.glLoadIdentity();//The identity matrix, in terms of the projection and modelview matrices,
            //essentially resets the matrix back to its default state.

            // not trivial
            double[] ModelVievMatrixBeforeSpecificTransforms = new double[16];
            double[] CurrentRotationTraslation = new double[16];

            GLU.gluLookAt(ScrollValue[0], ScrollValue[1], ScrollValue[2],
                       ScrollValue[3], ScrollValue[4], ScrollValue[5],
                       ScrollValue[6], ScrollValue[7], ScrollValue[8]);
            GL.glTranslatef(0.0f, 0.0f, -4.0f);

           // DrawOldAxes();

            //save current ModelView Matrix values
            //in ModelVievMatrixBeforeSpecificTransforms array
            //ModelView Matrix ========>>>>>> ModelVievMatrixBeforeSpecificTransforms
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, ModelVievMatrixBeforeSpecificTransforms);
            //ModelView Matrix was saved, so
            GL.glLoadIdentity(); // make it identity matrix

            //make transformation in accordance to KeyCode
            float delta;
            if (intOptionC != 0)
            {
                delta = 5.0f * Math.Abs(intOptionC) / intOptionC; // signed 5

                switch (Math.Abs(intOptionC))// for translation & rotation keys
                {
                    case 1:
                        GL.glRotatef(delta, 1, 0, 0);
                        break;
                    case 2:
                        GL.glRotatef(delta, 0, 1, 0);
                        break;
                    case 3:
                        GL.glRotatef(delta, 0, 0, 1);
                        break;
                    case 4:
                        GL.glTranslatef(delta / 20, 0, 0);
                        break;
                    case 5:
                        GL.glTranslatef(0, delta / 20, 0);
                        break;
                    case 6:
                        GL.glTranslatef(0, 0, delta / 20);
                        break;
                }
            }
            //as result - the ModelView Matrix now is pure representation
            //of KeyCode transform and only it !!!

            //save current ModelView Matrix values
            //in CurrentRotationTraslation array
            //ModelView Matrix =======>>>>>>> CurrentRotationTraslation
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, CurrentRotationTraslation);

            //The GL.glLoadMatrix function replaces the current matrix with
            //the one specified in its argument.
            //The current matrix is the
            //projection matrix, modelview matrix, or texture matrix,
            //determined by the current matrix mode (now is ModelView mode)
            GL.glLoadMatrixd(AccumulatedRotationsTraslations); //Global Matrix

            //The GL.glMultMatrix function multiplies the current matrix by
            //the one specified in its argument.
            //That is, if M is the current matrix and T is the matrix passed to
            //GL.glMultMatrix, then M is replaced with M • T
            GL.glMultMatrixd(CurrentRotationTraslation);

            //save the matrix product in AccumulatedRotationsTraslations
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);

            //replace ModelViev Matrix with stored ModelVievMatrixBeforeSpecificTransforms
            GL.glLoadMatrixd(ModelVievMatrixBeforeSpecificTransforms);
            //multiply it by KeyCode defined AccumulatedRotationsTraslations matrix
            GL.glMultMatrixd(AccumulatedRotationsTraslations);


            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_COLOR_MATERIAL);//enables overwriting material properties with vertex colors
            //GL.glEnable(GL.GL_LIGHTING);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, ar);//returns light source parameter values.

            GL.glPushMatrix();
            GL.glTranslated(-26f, 0f, -58f);
           addBuilding();
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslated(26f, 0f, -58f);
            addBuilding();
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslated(0f, 0f, -30f);
            addBuilding();
            GL.glPopMatrix();



            GL.glTranslated(0, -8, -8);


           //air_plain
            GL.glPushMatrix();
            GL.glPushMatrix();
            angle += 5.0f;
            GL.glTranslated(0f, 0f, 4f);

            //spin
             //take this down for keys


            GL.glTranslated(0f, 0f, -4f);
           
            GL.glRotated(heli_ground_angle, 11, 0, 0);
            //**
            GL.glTranslated(heli_x_pos, -heli_z_pos, heli_y_pos);//(,R,Hieght)
            GL.glRotatef(-heli_spin_angle, 0f, 0f, 1f);
           
            
            DRAWHELICOPTER();

            //Clouds
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glDepthRange(0, 100);
            GL.glTranslated(0, 25, -20);

            cloud();
            createCloud();

            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glDepthRange(0, 100);
            GL.glTranslated(-20, 25, -20);

            cloud();
            createCloud();

            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glDepthRange(0, 100);
            GL.glTranslated(-20, 25, 0);

            cloud();
            createCloud();

            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glDepthRange(0, 100);
            GL.glTranslated(0, 25, 0);

            cloud();
            createCloud();



            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(35f, 0f, -30f);
            tree();
         //   ground1();
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-25f, 0f, -20f);
            tree();
            //addBuilding();
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-5f, 0f, -10f);
            tree();

            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(8f, 0f, -10f);
            tree();
 
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(10f, 0f, 3f);
            tree();

            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-8f, 0f, 5f);
            tree();
            GL.glPopMatrix();

            

            GL.glPushMatrix();
            GL.glTranslated(-21f, 0f, -5f);
            tree();
            //addBuilding();
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslated(21f, 0f, -5f);
            tree();
            //addBuilding();
            GL.glPopMatrix();

            //LightSource
            GL.glTranslated(3f, 12f, 10f);//z=3 y=2
            DrawLightSource();
            GL.glPopMatrix();


            //reflection

            GL.glPushMatrix();
            GL.glTranslated(0.5f, -2.8f, 0.5f);
            if (reflect)
            {
               
                         

                GL.glPushMatrix();
                GL.glTranslated(0f, -1f, 0f);
                Drawreflection_Ice();
                 GL.glPopMatrix();
                GL.glTranslated(0f, 0f, 0f);
                GL.glRotatef(90, 10f, 0f, 0f);
                Drawreflection_Hlicopter();
               
            }
            GL.glPopMatrix();


            //shadow
            if (shadow)
            {
                Draw_shadow_source_red();
                GL.glPushMatrix();
                /* Project the shadow. */
                GL.glTranslated(-2, -4.3, -2);
                MakeShadowMatrix(wall);
                GL.glMultMatrixf(cubeXform);

                GL.glPushAttrib(GL.GL_CURRENT_BIT);
                GL.glRotated(50, 1, 0, 0);
                GL.glTranslated(5, 2.5, -1.1);

                GL.glDisable(GL.GL_LIGHTING);
                GL.glDisable(GL.GL_LIGHT0);

                GL.glTranslated(heli_x_pos, -heli_z_pos, heli_y_pos);
                GL.glRotatef(-heli_spin_angle, 0f, 0f, 1f);

                Draw_Shadow_Helicopter();

                GL.glTranslated(-heli_x_pos, heli_z_pos, -heli_y_pos);
                GL.glRotatef(-heli_spin_angle, 0f, 0f, 1f);

                GL.glPopAttrib();       
                GL.glPopMatrix();

                GL.glPushMatrix();

                GL.glPushAttrib(GL.GL_CURRENT_BIT);


                GL.glDisable(GL.GL_LIGHTING);
                GL.glDisable(GL.GL_LIGHT0);


                GL.glPopAttrib();        
                GL.glPopMatrix();
                
            }

           //large floor
            GL.glPushMatrix();
            if (!reflect)
            {
                
                angle += 5.0f;
                GL.glTranslated(0f, 0f, 4f);
                //GL.glRotatef(angle, 0f, 1f, 0f);
                GL.glTranslated(0f, 0f, -4f);
                GL.glTranslated(0f, -2.8f, 0f);
                DrawFloor_large();
                GL.glTranslated(2f, 1.2f, 0.0f);
                GL.glRotated(240, 1, 1, 1);
                landing_field();
 
                
            }
            GL.glPopMatrix();

            //wall


            GL.glTranslated(0f,0, -3f);
            DrawTexturedCube();
       
            GL.glFlush();

            WGL.wglSwapBuffers(m_uint_DC);

        }

        void createCloud(){

                GL.glTranslatef(3, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();

                GL.glTranslatef(1, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();

                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();


                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(144, 0, 1, 0);
                cloud();
                GL.glTranslatef(0, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                GL.glPushMatrix();
                GL.glTranslatef(0, 0, 20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();
                GL.glPopMatrix();

                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(30, 0, 1, 0);
             //000   cloud();
                GL.glTranslatef(8, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();

                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();
                GL.glTranslatef(5, 0, -5);
                GL.glRotatef(200, 0, 1, 0);
                GL.glPushMatrix();
                GL.glTranslatef(5, 0, -30);
                GL.glRotatef(270, 0, 1, 0);
                cloud();
                GL.glPopMatrix();
                GL.glTranslatef(15, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();

                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();

                GL.glTranslatef(7, 0, -20);
                GL.glRotatef(270, 0, 1, 0);
                cloud();


           // }


        }

        //DRAW REFLECTION HELICOPTER
        void Drawreflection_Hlicopter()
        {

            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

            //only floor, draw only to STENCIL buffer
            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            GL.glDisable(GL.GL_DEPTH_TEST);

            // restore regular settings
            GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

            // reflection is drawn only where STENCIL buffer value equal to 1
            GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

            GL.glEnable(GL.GL_STENCIL_TEST);

            // draw reflected scene
            GL.glPushMatrix();
            // angle += 5.0f;
            //   GL.glTranslated(0f, -1f, 0f);
            // GL.glRotatef(0, 0.0f, 0f, 0f);
            //GL.glTranslated(0f, -0.5f, -1.5f);
            GL.glScalef(1, 1, 1); //swap on Z axis
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glCullFace(GL.GL_BACK);
            // DrawBout();

            GL.glCullFace(GL.GL_FRONT);
            GL.glTranslated(heli_x_pos, heli_z_pos, heli_y_pos);
            // GL.glRotatef(10, 0.0f, 20f, 0f);
            GL.glRotatef(heli_spin_angle, 0f, 0f, 1f);
            DRAWHELICOPTER();
            // DrawBout();
            //  DRAWHELICOPTER();
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glPopMatrix();

            // actually drawing floor 

            // GL.glDepthMask((byte)GL.GL_FALSE);

            // GL.glDepthMask((byte)GL.GL_TRUE);

            GL.glDisable(GL.GL_STENCIL_TEST);



        }

        void DRAWHELICOPTER()
        {
            
            //INITIAL axes
            GL.glEnable(GL.GL_LINE_STIPPLE);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColor3f(0.2f, 0.2f, 0.2f);
            // Main body torso
            GL.glPushMatrix();
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();

            //The tail section
            GL.glPushMatrix();
            // Positive y side
            GL.glBegin(GL.GL_POLYGON);

            GL. glVertex3f(-3.0f, 0.05f, -0.3f);
            GL. glVertex3f(-3.0f, 0.05f, -0.1f);
            GL. glVertex3f(-1.25f, 0.2f, -0.1f);
            GL. glVertex3d(-1.25, 0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(-1.25, 0.2, -0.4);
            GL. glVertex3f(-1.25f, 0.2f, -0.1f);
            GL. glVertex3f(-0.5f, 0.5f, 0.5f);
            GL. glVertex3f(-0.5f, 0.5f, -0.5f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-3.0f, 0.05f, -0.3f);
            GL. glVertex3f(-3.25f, 0.05f, -0.3f);
            GL. glVertex3f(-3.5f, 0.05f, 0.2f);
            GL. glVertex3f(-3.25f, 0.05f, 0.2f);
            GL. glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glEnd();
            // Negative y side
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-0.5f, -0.5f, -0.5f);
            GL. glVertex3f(-0.5f, -0.5f, 0.5f);
            GL. glVertex3f(-1.25f, -0.2f, -0.1f);
            GL. glVertex3d(-1.25, -0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-3.0f, -0.05f, -0.1f);
            GL. glVertex3f(-3.0f, -0.05f, -0.3f);
            GL. glVertex3d(-1.25, -0.2, -0.4);
            GL. glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-3.0f, -0.05f, -0.1f);
            GL. glVertex3f(-3.25f, -0.05f, 0.2f);
            GL. glVertex3f(-3.5f, -0.05f, 0.2f);
            GL. glVertex3f(-3.25f, -0.05f, -0.3f);
            GL. glVertex3f(-3.0f, -0.05f, -0.3f);
            GL.glEnd();
            // Top
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glVertex3f(-0.5f, -0.5f, 0.5f);
            GL.glVertex3f(-0.5f, 0.5f, 0.5f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glVertex3f(-3.25f, 0.05f, 0.2f);
            GL.glVertex3f(-3.25f, -0.05f, 0.2f);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.25f, 0.05f, 0.2f);
            GL.glVertex3f(-3.5f, 0.05f, 0.2f);
            GL.glVertex3f(-3.5f, -0.05f, 0.2f);
            GL.glVertex3f(-3.25f, -0.05f, 0.2f);
            GL.glEnd();
            // Bottom
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-0.5f, 0.5f, -0.5f);
            GL. glVertex3f(-0.5f, -0.5f, -0.5f);
            GL. glVertex3d(-1.25, -0.2, -0.4);
            GL. glVertex3d(-1.25, 0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(-1.25, 0.2, -0.4);
            GL. glVertex3d(-1.25, -0.2, -0.4);
            GL. glVertex3f(-3.0f, -0.05f, -0.3f);
            GL. glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-3.0f, -0.05f, -0.3f);
            GL. glVertex3f(-3.25f, -0.05f, -0.3f);
            GL. glVertex3f(-3.25f, 0.05f, -0.3f);
            GL. glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(-3.25f, -0.05f, -0.3f);
            GL. glVertex3d(-3.5, -0.05, 0.2);
            GL. glVertex3f(-3.5f, 0.05f, 0.2f);
            GL. glVertex3f(-3.25f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glPopMatrix();

            /////   rotor

            GL.glColor3d(0.8, 0.8, 1.0);
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.65);
            GL.glRotated(rotangle, 0.0, 0.0, 1.0);
            GL. glScaled(4.0, 0.25, 0.01);
            GLUT. glutSolidCube(1);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.65);
            GL.glRotated(rotangle, 0.0, 0.0, 1.0);
            GL. glScaled(0.25, 4.0, 0.01);
            GLUT. glutSolidCube(1);
            GL.glPopMatrix();
            //Tail rotor
            GL.glPushMatrix();
            GL.glTranslated(-3.3, 0.06, 0.08);
            GL.glRotated(rotangle, 0.0, 1.0, 0.0);
            GL. glScaled(0.65, 0.01, 0.08);
            GLUT. glutSolidCube(1);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-3.3, 0.06, 0.08);
            GL.glRotated(rotangle, 0.0, 1.0, 0.0);
            GL. glScaled(0.08, 0.01, 0.65);
            GLUT. glutSolidCube(1);
            GL.glPopMatrix();
            // Draw - Rotor axle
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.5);
          //  quad = GLU.gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.15, 0.15, 0.15, 10, 10);
            GL.glPopMatrix();

            GL.glColor3f(1.0F, 1.0f, 0.0f);
            // Seat 1 bottom
            GL.glPushMatrix();
           GL. glTranslated(0.75, 0.25, -0.25);
            GL. glScaled(0.85, 0.75, 0.25);
            GLUT. glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 1 back 
            GL.glPushMatrix();
           GL. glTranslated(0.6, 0.25, 0);
            GL. glScaled(0.25, 0.75, 0.8);
            GLUT. glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 2 bottom
           GL.glPushMatrix();
           GL. glTranslatef(0.75f, -0.25f, -0.25f);
            GL. glScaled(0.85, 0.75, 0.25);
            GLUT. glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 2 back 
            GL.glPushMatrix();
           GL. glTranslatef(0.6f, -0.25f, 0f);
            GL. glScaled(0.25, 0.75, 0.8);
            GLUT. glutSolidCube(0.5);
            GL.glPopMatrix();
            // Joystick
            GL.glPushMatrix();
            GL.glColor3f(1, 0, 0);
           GL. glTranslated(1.1, -0.25, -0.5);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.02, 0.02, 0.35, 10, 10);
           GL. glTranslated(0, 0, 0.35);
            GL. glScaled(0.1, 0.1, 0.2);
            GLUT.glutSolidSphere(0.5, 10, 10);
            GL.glPopMatrix();

            GL.glColor3f(0.5f, 0.5f, 1.0f);
            GL.glPushMatrix();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(0.5, 0.5, 0.5);
            GL. glVertex3d(0.5, -0.5, 0.5);
            GL. glVertex3f(0.8f, -0.5f, 0.4f);
            GL. glVertex3f(0.8f, 0.5f, 0.4f);
            GL.glEnd();
            //Windscreen removed
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, 0.5, 0.0);
            GL.glVertex3d(1.2, -0.5, 0.0);
            GL.glVertex3f(1.4f, -0.5f, -0.1f);
            GL.glVertex3f(1.4f, 0.5f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(1.4f, 0.5f, -0.1f);
            GL.glVertex3f(1.4f, -0.5f, -0.1f);
            GL.glVertex3d(1.4, -0.5, -0.4);
            GL.glVertex3d(1.4, 0.5, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(1.4f, 0.5f, -0.4f);
            GL.glVertex3f(1.4f, -0.5f, -0.4f);
            GL.glVertex3d(1.2, -0.5, -0.5);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, -0.5, -0.5);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glVertex3f(0.5f, 0.5f, -0.5f);
            GL.glVertex3f(0.5f, -0.5f, -0.5f);
            GL.glEnd();
            // Positive y side
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3f(1.2f, 0.5f, 0.0f);
            GL. glVertex3f(1.4f, 0.5f, -0.1f);
            GL. glVertex3d(1.4, 0.5, -0.4);
            GL. glVertex3d(1.2, 0.5, -0.5);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(1.0, 0.5, 0.2);
            GL. glVertex3f(1.2f, 0.5f, 0.0f);
            GL. glVertex3d(1.2, 0.5, -0.5);
            GL. glVertex3d(1.0, 0.5, -0.5);
            GL.glEnd();
            // Negative y side
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(1.0, -0.5, -0.5);
            GL. glVertex3d(1.2, -0.5, -0.5);
            GL. glVertex3d(1.4, -0.5, -0.4);
            GL. glVertex3f(1.4f, -0.5f, -0.1f);
            GL. glVertex3f(1.2f, -0.5f, 0.0f);
            GL. glVertex3d(1.0, -0.5, 0.2);
            GL.glEnd();
            // Doors
            // Positive y side
            GL.glColor4d(0.5, 0.5, 1.0, 0.9);
            GL.glPushMatrix();
            GL.glTranslated(0.5, 0.5, 0.0);
            GL.glRotated(rot2, 0, 0, 1);
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(0.0, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, 0.2);
            GL.glVertex3d(0.3, 0.0, 0.4);
            GL.glVertex3d(0.0, 0.0, 0.5);
            GL.glEnd();
            GL.glPopMatrix();
            // Negative y side
            GL.glPushMatrix();
           GL. glTranslated(0.5, -0.5, 0.0);
            GL.glRotated(rot1, 0, 0, 1);
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glBegin(GL.GL_POLYGON);
            GL. glVertex3d(0.0, 0.0, -0.5);
            GL. glVertex3d(0.5, 0.0, -0.5);
            GL. glVertex3d(0.5, 0.0, 0.2);
            GL. glVertex3d(0.3, 0.0, 0.4);
            GL. glVertex3d(0.0, 0.0, 0.5);
            GL.glEnd();
            GL.glPopMatrix();
            GL.glPopMatrix();


            GL.glColor3d(0.8, 0.8, 1.0);
            // Skid legs - front
            GL.glPushMatrix();
           GL. glTranslated(0.8, -0.45, -0.75);
           // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
           GL. glTranslated(0.8, 0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            // Skid legs - back
            GL.glPushMatrix();
           GL. glTranslated(-0.4, -0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
           GL. glTranslated(-0.4, 0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            // Bottom of skids
            GL.glPushMatrix();
            GL.glRotated(90, 0, 1, 0);
           GL. glTranslated(0.75, -0.45, -0.75);
           // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 2.0, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glRotated(90, 0, 1, 0);
           GL. glTranslated(0.75, 0.45, -0.75);
           // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 2.0, 10, 10);
            GL.glPopMatrix();





            GL.glPushMatrix();
           // GL.glDepthMask((byte)GL.GL_FALSE);
           // GL.glDepthMask(GL.GL_FALSE);
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

            // Windscreen
            GL.glColor4d(0.6, 0.6, 1.0, 0.5);
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, -0.5, 0.0);
            GL.glVertex3d(1.2, 0.5, 0.0);
            GL.glVertex3f(0.8f, 0.5f, 0.4f);
            GL.glVertex3f(0.8f, -0.5f, 0.4f);
            GL.glEnd();

            GL.glDisable(GL.GL_BLEND);
           // GL.glDepthMask((byte)GL.GL_TRUE);
           // GL.glDepthMask(GL.GL_TRUE);
            GL.glPopMatrix();
        }

        void Draw_Shadow_Helicopter()
        {

            //  GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, pos);
            // GL.glDisable(GL.GL_LIGHTING);

            //INITIAL axes
            GL.glEnable(GL.GL_LINE_STIPPLE);
            // GL.glEnable(GL.GL_LIGHTING);
            //  GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_COLOR_MATERIAL);
            GL.glColor3d(0.8, 0.8, 0.8);
            // Main body torso
            GL.glPushMatrix();
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();

            //The tail section
            GL.glPushMatrix();
            // Positive y side
            GL.glBegin(GL.GL_POLYGON);

            GL.glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3d(-1.25, 0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(-1.25, 0.2, -0.4);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3f(-0.5f, 0.5f, 0.5f);
            GL.glVertex3f(-0.5f, 0.5f, -0.5f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glVertex3f(-3.25f, 0.05f, -0.3f);
            GL.glVertex3f(-3.5f, 0.05f, 0.2f);
            GL.glVertex3f(-3.25f, 0.05f, 0.2f);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glEnd();
            // Negative y side
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-0.5f, -0.5f, -0.5f);
            GL.glVertex3f(-0.5f, -0.5f, 0.5f);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glVertex3d(-1.25, -0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glVertex3f(-3.0f, -0.05f, -0.3f);
            GL.glVertex3d(-1.25, -0.2, -0.4);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glVertex3f(-3.25f, -0.05f, 0.2f);
            GL.glVertex3f(-3.5f, -0.05f, 0.2f);
            GL.glVertex3f(-3.25f, -0.05f, -0.3f);
            GL.glVertex3f(-3.0f, -0.05f, -0.3f);
            GL.glEnd();
            // Top
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glVertex3f(-0.5f, -0.5f, 0.5f);
            GL.glVertex3f(-0.5f, 0.5f, 0.5f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-1.25f, 0.2f, -0.1f);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glVertex3f(-1.25f, -0.2f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, 0.05f, -0.1f);
            GL.glVertex3f(-3.25f, 0.05f, 0.2f);
            GL.glVertex3f(-3.25f, -0.05f, 0.2f);
            GL.glVertex3f(-3.0f, -0.05f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.25f, 0.05f, 0.2f);
            GL.glVertex3f(-3.5f, 0.05f, 0.2f);
            GL.glVertex3f(-3.5f, -0.05f, 0.2f);
            GL.glVertex3f(-3.25f, -0.05f, 0.2f);
            GL.glEnd();
            // Bottom
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-0.5f, 0.5f, -0.5f);
            GL.glVertex3f(-0.5f, -0.5f, -0.5f);
            GL.glVertex3d(-1.25, -0.2, -0.4);
            GL.glVertex3d(-1.25, 0.2, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(-1.25, 0.2, -0.4);
            GL.glVertex3d(-1.25, -0.2, -0.4);
            GL.glVertex3f(-3.0f, -0.05f, -0.3f);
            GL.glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.0f, -0.05f, -0.3f);
            GL.glVertex3f(-3.25f, -0.05f, -0.3f);
            GL.glVertex3f(-3.25f, 0.05f, -0.3f);
            GL.glVertex3f(-3.0f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(-3.25f, -0.05f, -0.3f);
            GL.glVertex3d(-3.5, -0.05, 0.2);
            GL.glVertex3f(-3.5f, 0.05f, 0.2f);
            GL.glVertex3f(-3.25f, 0.05f, -0.3f);
            GL.glEnd();
            GL.glPopMatrix();

            /////   rotor

            GL.glColor3d(0.8, 0.8, 0.8);
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.65);
            GL.glRotated(rotangle, 0.0, 0.0, 1.0);
            GL.glScaled(4.0, 0.25, 0.01);
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.65);
            GL.glRotated(rotangle, 0.0, 0.0, 1.0);
            GL.glScaled(0.25, 4.0, 0.01);
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();
            //Tail rotor
            GL.glPushMatrix();
            GL.glTranslated(-3.3, 0.06, 0.08);
            GL.glRotated(rotangle, 0.0, 1.0, 0.0);
            GL.glScaled(0.65, 0.01, 0.08);
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-3.3, 0.06, 0.08);
            GL.glRotated(rotangle, 0.0, 1.0, 0.0);
            GL.glScaled(0.08, 0.01, 0.65);
            GLUT.glutSolidCube(1);
            GL.glPopMatrix();
            // Draw - Rotor axle
            GL.glPushMatrix();
            GL.glTranslated(0.0, 0.0, 0.5);
            //  quad = GLU.gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.15, 0.15, 0.15, 10, 10);
            GL.glPopMatrix();

            GL.glColor3d(0.8, 0.8, 0.8);
            // Seat 1 bottom
            GL.glPushMatrix();
            GL.glTranslated(0.75, 0.25, -0.25);
            GL.glScaled(0.85, 0.75, 0.25);
            GLUT.glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 1 back 
            GL.glPushMatrix();
            GL.glTranslated(0.6, 0.25, 0);
            GL.glScaled(0.25, 0.75, 0.8);
            GLUT.glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 2 bottom
            GL.glPushMatrix();
            GL.glTranslatef(0.75f, -0.25f, -0.25f);
            GL.glScaled(0.85, 0.75, 0.25);
            GLUT.glutSolidCube(0.5);
            GL.glPopMatrix();
            // Seat 2 back 
            GL.glPushMatrix();
            GL.glTranslatef(0.6f, -0.25f, 0f);
            GL.glScaled(0.25, 0.75, 0.8);
            GLUT.glutSolidCube(0.5);
            GL.glPopMatrix();
            // Joystick
            GL.glPushMatrix();
            GL.glColor3d(0.8, 0.8, 0.8);
            GL.glTranslated(1.1, -0.25, -0.5);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.02, 0.02, 0.35, 10, 10);
            GL.glTranslated(0, 0, 0.35);
            GL.glScaled(0.1, 0.1, 0.2);
            GLUT.glutSolidSphere(0.5, 10, 10);
            GL.glPopMatrix();

            GL.glColor3d(0.8, 0.8, 0.8);
            GL.glPushMatrix();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(0.5, 0.5, 0.5);
            GL.glVertex3d(0.5, -0.5, 0.5);
            GL.glVertex3f(0.8f, -0.5f, 0.4f);
            GL.glVertex3f(0.8f, 0.5f, 0.4f);
            GL.glEnd();
            //Windscreen removed
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, 0.5, 0.0);
            GL.glVertex3d(1.2, -0.5, 0.0);
            GL.glVertex3f(1.4f, -0.5f, -0.1f);
            GL.glVertex3f(1.4f, 0.5f, -0.1f);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(1.4f, 0.5f, -0.1f);
            GL.glVertex3f(1.4f, -0.5f, -0.1f);
            GL.glVertex3d(1.4, -0.5, -0.4);
            GL.glVertex3d(1.4, 0.5, -0.4);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(1.4f, 0.5f, -0.4f);
            GL.glVertex3f(1.4f, -0.5f, -0.4f);
            GL.glVertex3d(1.2, -0.5, -0.5);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, -0.5, -0.5);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glVertex3f(0.5f, 0.5f, -0.5f);
            GL.glVertex3f(0.5f, -0.5f, -0.5f);
            GL.glEnd();
            // Positive y side
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3f(1.2f, 0.5f, 0.0f);
            GL.glVertex3f(1.4f, 0.5f, -0.1f);
            GL.glVertex3d(1.4, 0.5, -0.4);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glEnd();
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.0, 0.5, 0.2);
            GL.glVertex3f(1.2f, 0.5f, 0.0f);
            GL.glVertex3d(1.2, 0.5, -0.5);
            GL.glVertex3d(1.0, 0.5, -0.5);
            GL.glEnd();
            // Negative y side
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.0, -0.5, -0.5);
            GL.glVertex3d(1.2, -0.5, -0.5);
            GL.glVertex3d(1.4, -0.5, -0.4);
            GL.glVertex3f(1.4f, -0.5f, -0.1f);
            GL.glVertex3f(1.2f, -0.5f, 0.0f);
            GL.glVertex3d(1.0, -0.5, 0.2);
            GL.glEnd();
            // Doors
            // Positive y side
            GL.glColor3d(0.8, 0.8, 0.8);
            GL.glPushMatrix();
            GL.glTranslated(0.5, 0.5, 0.0);
            GL.glRotated(rot2, 0, 0, 1);
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(0.0, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, 0.2);
            GL.glVertex3d(0.3, 0.0, 0.4);
            GL.glVertex3d(0.0, 0.0, 0.5);
            GL.glEnd();
            GL.glPopMatrix();
            // Negative y side
            GL.glPushMatrix();
            GL.glTranslated(0.5, -0.5, 0.0);
            GL.glRotated(rot1, 0, 0, 1);
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(0.0, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, -0.5);
            GL.glVertex3d(0.5, 0.0, 0.2);
            GL.glVertex3d(0.3, 0.0, 0.4);
            GL.glVertex3d(0.0, 0.0, 0.5);
            GL.glEnd();
            GL.glPopMatrix();
            GL.glPopMatrix();


            GL.glColor3d(0.8, 0.8, 0.8);
            // Skid legs - front
            GL.glPushMatrix();
            GL.glTranslated(0.8, -0.45, -0.75);
            // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(0.8, 0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            // Skid legs - back
            GL.glPushMatrix();
            GL.glTranslated(-0.4, -0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(-0.4, 0.45, -0.75);
            //quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 0.25, 10, 10);
            GL.glPopMatrix();
            // Bottom of skids
            GL.glPushMatrix();
            GL.glRotated(90, 0, 1, 0);
            GL.glTranslated(0.75, -0.45, -0.75);
            // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 2.0, 10, 10);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glRotated(90, 0, 1, 0);
            GL.glTranslated(0.75, 0.45, -0.75);
            // quad = gluNewQuadric();
            GLU.gluCylinder(GLU.gluNewQuadric(), 0.03, 0.03, 2.0, 10, 10);
            GL.glPopMatrix();





            GL.glPushMatrix();
            // GL.glDepthMask((byte)GL.GL_FALSE);
            // GL.glDepthMask(GL.GL_FALSE);
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

            // Windscreen
            GL.glColor3d(0.8, 0.8, 0.8);
            GL.glBegin(GL.GL_POLYGON);
            GL.glVertex3d(1.2, -0.5, 0.0);
            GL.glVertex3d(1.2, 0.5, 0.0);
            GL.glVertex3f(0.8f, 0.5f, 0.4f);
            GL.glVertex3f(0.8f, -0.5f, 0.4f);
            GL.glEnd();

            GL.glDisable(GL.GL_BLEND);
            // GL.glDepthMask((byte)GL.GL_TRUE);
            // GL.glDepthMask(GL.GL_TRUE);
            GL.glPopMatrix();

        }

        private static GLUquadric quadObj;
        #region QUAD_OBJ_INIT()
        /// <summary>
        /// Ensures we have a valid quadric.
        /// </summary>
        private static void QUAD_OBJ_INIT()
        {
            InitQuadObj();
        }
        #endregion QUAD_OBJ_INIT()

        #region InitQuadObj()
        /// <summary>
        /// Initializes a new quadric.
        /// </summary>
        private static void InitQuadObj()
        {
            quadObj = GLU.gluNewQuadric();
        }
        #endregion InitQuadObj()

        void tree() 
        {

            //public static void glutSolidCone(double conebase, double height, int slices, int stacks) {
            //QUAD_OBJ_INIT();
            //GLU.gluQuadricDrawStyle(quadObj, GLU.GLU_FILL);
            //GLU.gluQuadricNormals(quadObj, GLU.GLU_SMOOTH);
            //// If we ever changed/used the texture or orientation state
            //// of quadObj, we'd need to change it to the defaults here
            //// with gluQuadricTexture and/or gluQuadricOrientation.
            //GLU.gluCylinder(quadObj, conebase, 0.0, height, slices, stacks);
            //!!!

            //GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
          
            //triangle bottom
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 1.0f, 0.0f);
            GL.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            GL.glTranslatef(0.0f, 0.0f, 0.0f);
            GL.glColor4f(0.0f, 1.0f, 0.5f, 1.0f);
            QUAD_OBJ_INIT();
            GLU.gluQuadricTexture(quadObj, 1); // Cilindro texturizzato
            // GLU.gluQuadricDrawStyle(quadObj, GLU.GLU_FILL);
            GLU.gluQuadricNormals(quadObj, GLU.GLU_SMOOTH);
            GLU.gluCylinder(quadObj, 1.5, 0.0, 3.0, 20, 19);
            //GL.glTexCoord2f(0.0f, 0.0f);
            //GLUT.glutSolidCone(1.5, 3.0, 20, 19);
            GL.glPopMatrix();
            //
              
                      
            //triangle middle
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 3.0f, 0.0f);
            GL.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            GL.glTranslatef(0.0f, 0.0f, 0.0f);
            
            GL.glColor4f(0.0f, 1.0f, 0.5f, 1.0f);
            QUAD_OBJ_INIT();
            GLU.gluQuadricTexture(quadObj, 1); // Cilindro texturizzato
            // GLU.gluQuadricDrawStyle(quadObj, GLU.GLU_FILL);
            GLU.gluQuadricNormals(quadObj, GLU.GLU_SMOOTH);
            GLU.gluCylinder(quadObj, 1.0, 0.0, 2.0, 20, 19);
            
            GL.glPopMatrix();

            //triangle up (wide cylinder)
          
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 4.5f, 0.0f);
            GL.glRotatef(-90.0f, 1.0f, 0.0f, 0.0f);
            GL.glTranslatef(0.0f, 0.0f, 0.0f);
            
            GL.glColor4f(0.0f, 1.0f, 0.5f, 1.0f);
            QUAD_OBJ_INIT();
            GLU.gluQuadricTexture(quadObj, 1); // Cilindro texturizzato
            // GLU.gluQuadricDrawStyle(quadObj, GLU.GLU_FILL);
            GLU.gluQuadricNormals(quadObj, GLU.GLU_SMOOTH);
            GLU.gluCylinder(quadObj, 0.5, 0.0, 1.0, 20, 19);
                        
            GL.glPopMatrix();
            
            //tree stem
            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 1.0f, 0.0f);
            GL.glScalef(0.3f, 3.4f, 0.3f);
            GL.glTranslatef(0.0f, 0.0f, 0.0f);
            GL.glColor3f(0.8f, 0.6f, 0.6f);
            GLUT.glutSolidCube(2.0);
            GL.glPopMatrix();

            GL.glDisable(GL.GL_TEXTURE_2D);
       
        }   

        //public float beta;            
                                  
        void DrawLightSource()
        {
            GL.glPushMatrix();
            
            GL.glTranslatef(ar[0], ar[1], ar[2]);
            GL.glColor3f(1.0f, 1.0f, 0.0f);
            GLU.gluSphere(obj, 0.5, 16, 16);
            GL.glPopMatrix();
        }
                
        void landing_field()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[7]);
            GL.glDisable(GL.GL_LIGHTING);

            GL.glBegin(GL.GL_QUADS);
            GL.glColor3f(1.0f, 1.0f, 1.0f);
            // Front Face
            
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-7.0f, -8.0f, 0.2f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(7.0f, -8.0f, 0.2f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(7.0f, 8.0f, 0.2f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-7.0f, 8.0f, 0.2f);
            // Back Face
           
            GL.glVertex3f(-7.0f, -8.0f, 0.2f);
            
            GL.glVertex3f(-7.0f, 8.0f, 0.2f);
            GL.glVertex3f(7.0f, 8.0f, -0.2f);
            GL.glVertex3f(7.0f, -8.0f, -0.2f);
            // Top Face
            GL.glVertex3f(-7.0f, 8.0f, -0.2f);
            GL.glVertex3f(-7.0f, 8.0f, 0.2f);
            GL.glVertex3f(7.0f, 8.0f, 0.2f);
            GL.glVertex3f(7.0f, 8.0f, -0.2f);
            // Bottom Face
            GL.glVertex3f(-7.0f, -8.0f, -0.2f);
            GL.glVertex3f(7.0f, -8.0f, -0.2f);
            GL.glVertex3f(7.0f, -8.0f, 0.2f);
            GL.glVertex3f(-7.0f, -8.0f, 0.2f);
            // Right face
            GL.glVertex3f(7.0f, -8.0f, -0.2f);
            GL.glVertex3f(7.0f, 8.0f, -0.2f);
            GL.glVertex3f(7.0f, 8.0f, 0.2f);
            GL.glVertex3f(7.0f, -8.0f, 0.2f);
            // Left Face
            GL.glVertex3f(-7.0f, -8.0f, -0.2f);
          
            GL.glVertex3f(-7.0f, -8.0f, 0.2f);
           
            GL.glVertex3f(-7.0f, 8.0f, 0.2f);
            
            GL.glVertex3f(-7.0f, 8.0f, -0.2f);


            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_BLEND);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
 
        }

        void addBuilding()
        {
            GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]);
            GL.glDisable(GL.GL_LIGHTING);

            GL.glBegin(GL.GL_QUADS);
            GL.glColor3f(1.0f, 1.0f, 1.0f);
            // Front Face
         //   GL.glNormal3f(0.0f, 0.0f, 0.5f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-4.0f, -14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(4.0f, -14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(4.0f, 14.0f, 4.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(-4.0f, 14.0f, 4.0f);
            // Back Face
           // GL.glNormal3f(0.0f, 0.0f, -0.5f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(-4.0f, -14.0f, -4.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(-4.0f, 14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(4.0f, 14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(4.0f, -14.0f, -4.0f);
            // Top Face
           // GL.glNormal3f(0.0f, 0.5f, 0.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(-4.0f, 14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-4.0f, 14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(4.0f, 14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(4.0f, 14.0f, -4.0f);
            // Bottom Face
           // GL.glNormal3f(0.0f, -0.5f, 0.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(-4.0f, -14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(4.0f, -14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(4.0f, -14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(-4.0f, -14.0f, 4.0f);
            // Right face
           // GL.glNormal3f(0.5f, 0.0f, 0.0f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(4.0f, -14.0f, -4.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(4.0f, 14.0f, -4.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(4.0f, 14.0f, 4.0f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(4.0f, -14.0f, 4.0f);
            // Left Face
           // GL.glNormal3f(-0.5f, 0.0f, 0.0f);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-4.0f, -14.0f, -4.0f);
            GL.glTexCoord2f(0.5f, 0.0f); GL.glVertex3f(-4.0f, -14.0f, 4.0f);
            GL.glTexCoord2f(0.5f, 0.5f); GL.glVertex3f(-4.0f, 14.0f, 4.0f);
            GL.glTexCoord2f(0.0f, 0.5f); GL.glVertex3f(-4.0f, 14.0f, -4.0f);
            GL.glEnable(GL.GL_LIGHTING);
            GL.glEnable(GL.GL_LIGHT0);
            GL.glEnable(GL.GL_BLEND);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);

          
        }
        
        void cloud()
        {


            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.0f, -2.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.0f, -1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();



            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.0f, 0.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();


            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.0f, 1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 0.0f, 2.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(1.0f, 0.0f, 0.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(1.0f, 0.0f, 1.2f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(1.0f, 0.0f, -1.2f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(-1.0f, 0.0f, 0.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(-1.0f, 0.0f, 1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(-1.0f, 0.0f, -1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.5f, 1.0f, 0.5f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(-0.5f, 1.0f, 0.5f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(-0.5f, 1.0f, -0.5f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.5f, 1.0f, -0.5f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 1.0f, 1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

            GL.glPushMatrix();
            GL.glTranslatef(0.0f, 1.0f, -1.0f);
            GL.glColor3f(1.0f, 1.0f, 0.7f);
            GLUT.glutSolidSphere(1.0, 10, 10);
            GL.glPopMatrix();

        }
        
        void Drawreflection_Ice()
        {

            GL.glEnable(GL.GL_BLEND);////activate blending mode 
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);//defines the operation of blending for all draw buffers when it is enabled
            // GL_SRC_ALPHA--> Draw each of the images with an alpha equal to 0.3333333,
            //where the images don't overlap. 

            //STENCIL buffer--> stencil buffer is an extra buffer, in addition to the color buffer (pixel buffer)
            //and depth buffer (z-buffering) found on modern graphics hardware

            //only floor, draw only to STENCIL buffer
            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF); // draw floor always
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            //glColorMask--> specifies whether the individual color components in the frame buffer can or cannot be written
            GL.glDisable(GL.GL_DEPTH_TEST);
            DrawFloor();
            landing_field();
           //  restore regular settings
            GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

            // reflection is drawn only where STENCIL buffer value equal to 1
            GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

            GL.glEnable(GL.GL_STENCIL_TEST);

            // draw reflected scene
            GL.glPushMatrix();
            angle += 2.0f;
            GL.glTranslated(0f, 0f, 4f);
            GL.glRotatef(angle, 0f, 1f, 0f);
            GL.glTranslated(0f, 0f, -4f);
            GL.glTranslated(-0.5f, 1f, 0f);
            GL.glScalef(1, -1, 1); //swap on Z axis
            GL.glEnable(GL.GL_CULL_FACE);
            GL.glCullFace(GL.GL_BACK);

            GL.glCullFace(GL.GL_FRONT);
            GL.glDisable(GL.GL_CULL_FACE);
            GL.glPopMatrix();

            // actually drawing floor 

            GL.glDepthMask((byte)GL.GL_FALSE);
            GL.glPushMatrix();
            GL.glTranslated(0, 1.3, 0);
            DrawFloor();
            GL.glPopMatrix();
            
            GL.glPushMatrix();

            GL.glTranslated(1.5f, 1.6f, 0.0f);
            GL.glRotated(240, 1, 1, 1);

            landing_field();
            GL.glPopMatrix();

            GL.glDepthMask((byte)GL.GL_TRUE);

            GL.glDisable(GL.GL_STENCIL_TEST);
        }

        //--------------------------------------------------
        public float alpha;
          
        protected virtual void InitializeGL()
        {
            m_uint_HWND = (uint)p.Handle.ToInt32();
            m_uint_DC = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
            // result in a failure to subsequently create the RC.
            WGL.wglSwapBuffers(m_uint_DC);

            WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
            WGL.ZeroPixelDescriptor(ref pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_SUPPORT_OPENGL | WGL.PFD_DOUBLEBUFFER);
            pfd.iPixelType = (byte)(WGL.PFD_TYPE_RGBA);
            pfd.cColorBits = 32;
            pfd.cDepthBits = 32;
            pfd.iLayerType = (byte)(WGL.PFD_MAIN_PLANE);

            int pixelFormatIndex = 0;
            pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
            if (pixelFormatIndex == 0)
            {
                MessageBox.Show("Unable to retrieve pixel format");
                return;
            }

            if (WGL.SetPixelFormat(m_uint_DC, pixelFormatIndex, ref pfd) == 0)
            {
                MessageBox.Show("Unable to set pixel format");
                return;
            }
            //Create rendering context
            m_uint_RC = WGL.wglCreateContext(m_uint_DC);
            if (m_uint_RC == 0)
            {
                MessageBox.Show("Unable to get rendering context");
                return;
            }
            if (WGL.wglMakeCurrent(m_uint_DC, m_uint_RC) == 0)
            {
                MessageBox.Show("Unable to make rendering context current");
                return;
            }


            initRenderingGL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            //glViewport specifies the affine transformation of x and y from normalized device coordinates to window coordinates. 
            Draw();
        }

        protected virtual void initRenderingGL()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;
            if (this.Width == 0 || this.Height == 0)
                return;
            GL.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);
            //glDepthFunc--> specifies the function used to compare each incoming pixel depth value with the depth value present in the depth buffer

            GL.glViewport(0, 0, this.Width, this.Height);
            GL.glMatrixMode(GL.GL_PROJECTION);//specify which matrix is the current matrix 
            GL.glLoadIdentity();

            //nice 3D
            GLU.gluPerspective(90.0, 1.0, 0.4, 100.0);//set up a perspective projection matrix 


            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

            //save the current MODELVIEW Matrix (now it is Identity)
            GL.glGetDoublev(GL.GL_MODELVIEW_MATRIX, AccumulatedRotationsTraslations);
            GenerateTextures();

        }

        //--------------------------------------------------
        // TEXTURES
        public uint[] Textures = new uint[9];
        void GenerateTextures()
        {
            GL.glGenTextures(9, Textures);
            string[] imagesName = { "sea.bmp", "waves.jpg", "sky.jpg", "texture.jpg", "sky.jpg", "sky.jpg", "building.jpg", "H.jpg", "sky.jpg" };
            for (int j = 0; j < 9; j++)
            {
                Bitmap image = new Bitmap(imagesName[j]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[j]);
                //2D for XYZ
                //The level-of-detail number. Level 0 is the base image level
                //The number of color components in the texture. 
                //Must be 1, 2, 3, or 4, or one of the following 
                //    symbolic constants: 
                //                GL_ALPHA, GL_ALPHA4, 
                //                GL_ALPHA8, GL_ALPHA12, GL_ALPHA16, GL_LUMINANCE, GL_LUMINANCE4, 
                //                GL_LUMINANCE8, GL_LUMINANCE12, GL_LUMINANCE16, GL_LUMINANCE_ALPHA, 
                //                GL_LUMINANCE4_ALPHA4, GL_LUMINANCE6_ALPHA2, GL_LUMINANCE8_ALPHA8, 
                //                GL_LUMINANCE12_ALPHA4, GL_LUMINANCE12_ALPHA12, GL_LUMINANCE16_ALPHA16, 
                //                GL_INTENSITY, GL_INTENSITY4, GL_INTENSITY8, GL_INTENSITY12, 
                //                GL_INTENSITY16, GL_R3_G3_B2, GL_RGB, GL_RGB4, GL_RGB5, GL_RGB8, 
                //                GL_RGB10, GL_RGB12, GL_RGB16, GL_RGBA, GL_RGBA2, GL_RGBA4, GL_RGB5_A1, 
                //                GL_RGBA8, GL_RGB10_A2, GL_RGBA12, or GL_RGBA16.


                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                    //The width of the border. Must be either 0 or 1.
                    //The format of the pixel data
                    //The data type of the pixel data
                    //A pointer to the image data in memory
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }
    }

}


