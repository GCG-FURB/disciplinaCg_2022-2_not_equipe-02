using System;
using System.Collections.Generic;

namespace CG_Biblioteca
{
  /// <summary>
  /// Classe com funções matemáticas.
  /// </summary>
  public abstract class Matematica
  {
    /// <summary>
    /// Função para calcular um ponto sobre o perímetro de um círculo informando um ângulo e raio.
    /// </summary>
    /// <param name="angulo"></param>
    /// <param name="raio"></param>
    /// <returns></returns>
    public static Ponto4D GerarPtosCirculo(double angulo, double raio)
    {
      Ponto4D pto = new Ponto4D();
      pto.X = (raio * Math.Cos(Math.PI * angulo / 180.0));
      pto.Y = (raio * Math.Sin(Math.PI * angulo / 180.0));
      pto.Z = 0;
      return (pto);
    }

    public static double GerarPtosCirculoSimetrico(double raio)
    {
      return (raio * Math.Cos(Math.PI * 45 / 180.0));
    }

    public static List<Ponto4D> GerarPtosSpline(Ponto4D ptBaixoDir, Ponto4D ptoCimaDir, Ponto4D ptCimaEsq, Ponto4D ptBaixoEsq, int qtdPontos)
        {
            List<Ponto4D> pontos = new List<Ponto4D>();
            double t = 1.0 / qtdPontos;
            double x, y, z;

            for (double i = 0; i <= 1; i += t)  
            {
                x = (Math.Pow((1 - i), 3) * ptBaixoDir.X) + (3 * i * Math.Pow((1 - i), 2) * ptoCimaDir.X) + (3 * Math.Pow(i, 2)* (1 - i) * ptCimaEsq.X) + (Math.Pow(i, 3) * ptBaixoEsq.X);
                y = (Math.Pow((1 - i), 3) * ptBaixoDir.Y) + (3 * i * Math.Pow((1 - i), 2) * ptoCimaDir.Y) + (3 * Math.Pow(i, 2)* (1 - i) * ptCimaEsq.Y) + (Math.Pow(i, 3) * ptBaixoEsq.Y);
                z = (Math.Pow((1 - i), 3) * ptBaixoDir.Z) + (3 * i * Math.Pow((1 - i), 2) * ptoCimaDir.Z) + (3 * Math.Pow(i, 2)* (1 - i) * ptCimaEsq.Z) + (Math.Pow(i, 3) * ptBaixoEsq.Z);
                pontos.Add(new Ponto4D(x, y, z));
            }

            x = ptBaixoEsq.X;
            y = ptBaixoEsq.Y;
            z = ptBaixoEsq.Z;
            pontos.Add(new Ponto4D(x, y, z));
            
            pontos.Reverse();
            return pontos;
        }
    
  }
}