/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
        private bool mouseMoverPto = false;
        private SegReta obj_seg_reta;
        private Ponto4D pto1;
        private Ponto4D pto2;
        private int raio;
        private int angulo;
        private double deslocamento;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            deslocamento = 0;
            camera.xmin = -300; camera.xmax = 300; camera.ymin = -300; camera.ymax = 300;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            objetoId = Utilitario.charProximo(objetoId);

            raio = 100;
            angulo = 45;
            pto1 = new Ponto4D(0, 0, 0);
            pto2 = Matematica.GerarPtosCirculo(angulo, raio);
            obj_seg_reta = new SegReta(objetoId, null, pto1, pto2);
            obj_seg_reta.ObjetoCor.CorR = 0; obj_seg_reta.ObjetoCor.CorG = 0; obj_seg_reta.ObjetoCor.CorB = 0;
            obj_seg_reta.PrimitivaTipo = PrimitiveType.Lines;
            obj_seg_reta.PrimitivaTamanho = 5;
            objetosLista.Add(obj_seg_reta);
            objetoSelecionado = obj_seg_reta;


#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
#if CG_OpenGL
            GL.ClearColor(0.55f, 0.55f, 0.55f, 1f);
#endif
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
#if CG_OpenGL
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
#if CG_OpenGL
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#endif
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
#if CG_Gizmo
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();
#endif
            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.Q)
            {
                deslocamento -= 5;
                pto1.X = deslocamento;
                pto1.Y = 0;
                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }

            else if (e.Key == Key.W)
            {
                deslocamento += 5;

                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }

            else if (e.Key == Key.Z)
            {
                angulo = angulo + 5;

                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }

            else if (e.Key == Key.X)
            {
                angulo = angulo - 5;

                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }

            else if (e.Key == Key.A && raio > 10)
            {
                raio = raio - 10;

                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }

            else if (e.Key == Key.S)
            {
                raio = raio + 10;
                pto1.X = deslocamento;
                pto1.Y = 0;
                pto2 = Matematica.GerarPtosCirculo(angulo, raio);
                pto2.X += deslocamento;

                objetoSelecionado.PontosAlterar(pto1, 0);
                objetoSelecionado.PontosAlterar(pto2, 1);
            }
#if CG_Gizmo


#endif
            else if (e.Key == Key.V)
                mouseMoverPto = !mouseMoverPto;   //TODO: falta atualizar a BBox do objeto            

            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (mouseMoverPto && (objetoSelecionado != null))
            {
                objetoSelecionado.PontosUltimo().X = mouseX;
                objetoSelecionado.PontosUltimo().Y = mouseY;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
#endif
        }
#endif
    }
    class Program
    {
        static void Main(string[] args)
        {
            ToolkitOptions.Default.EnableHighResolution = false;
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N2";
            window.Run(1.0 / 60.0);
        }
    }
}
