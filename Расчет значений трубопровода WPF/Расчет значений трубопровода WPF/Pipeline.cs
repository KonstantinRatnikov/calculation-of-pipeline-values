using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Расчет_значений_трубопровода_WPF
{
    class Pipeline
    {
        double D; //наружный диаметр
        double d;//внутренний диаметр
        double E;//модуль упругости Юнга стали трубопровода
        double ro;//плотность материала трубопровода
        double lC1;//расстояние от левого края ремонтируемого трубопровода до места первого разреза 
        double lC2;//расстояние от левого края ремонтируемого трубопровода до места второго разреза 
        
        public double L { get; }// длина всего участка трубопровода
        double hy;//высота, на которую необходимо центрировать конец трубопровода
        double hz;//высота, на которую необходимо центрировать конец трубопровода        
        double[] coefV;// коэффициенты вертикального полинома координат участка
        double[] coefH;//коэффициенты горизонтальные полинома координат участка
        double permissibleStress;//Допустимое напряжение для стали трубопровода, Па
        double permissibleForce;//Допустимое усилие для стали трубопровода, H

        double I;//Осевой момент инерции трубы (м-4) 
        double q;//Распределенная нагрузка от собственного веса трубопровода (кг/м3)
        double lenghtDamagedARea;//длина поврежденной части трубопровода

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double[] X_H { get; set; }
        
        public double[] X_V { get; set; }

        public double P1 { get; set; }
        public double P2 { get; set; }

        public double H_h { get; set; }
        public double H_v { get; set; }
        public double[] P_H { get; set; }
        public double[] P_V { get; set; }

        public double ResL1 { get; set; }
        public double ResL2 { get; set; }
        public double ResM0 { get; set; }
        public double ResQ0 { get; set; }
        public List<Point> SigmaZlist { get; set; }

        /// <summary>
        /// входные данные
        /// </summary>
        /// <param name="D">наружный диаметр</param>
        /// <param name="d">внутренний диаметр</param>
        /// <param name="E">модуль упругости Юнга стали трубопровода</param>
        /// <param name="ro">плотность материала трубопровода</param>
        /// <param name="lc1">расстояние от левого края ремонтируемого трубопровода до места первого разреза </param>
        /// <param name="lc2">расстояние от левого края ремонтируемого трубопровода до места второго разреза </param>
        /// <param name="L">длина всего участка трубопровода</param>
        /// <param name="hy">высота, на которую необходимо центрировать конец трубопровода</param>
        /// <param name="hz">высота, на которую необходимо центрировать конец трубопровода</param>
        /// <param name="coefV">коэффициенты вертикального полинома координат участка</param>
        /// <param name="coefH">коэффициенты горизонтальные полинома координат участка</param>
        /// <param name="permissibleStress">Допустимое напряжение для стали трубопровода</param>
        /// <param name="permissibleForce">Допустимое усилие для стали трубопровода</param>
        public Pipeline(double D, double d, double E, double ro, double lc1, double lc2,  double L, double hy, double hz, double[] coefV, double[] coefH, double permissibleStress, double permissibleForce)
        {
            X_H = new double[4];
            X_V = new double[4];
            P_H = new double[4];
            P_V= new double[4];


            this.D = D;
            this.d = d;
            this.E = E;
            this.ro = ro;
            this.lC1 = lc1;
            this.lC2 = lc2;
            this.lenghtDamagedARea = Math.Abs(lc1-lc2);
            this.L = L;
            this.coefV = coefV;
            this.coefH = coefH;
            this.hy = hy;
            this.hz = hz;


            this.permissibleStress = permissibleStress;
            this.permissibleForce = permissibleForce;

            //Вызов метода для расчета искомых величин
            Calculation();


        }
        void Calculation()
        {
            //Вызов метода для расчета осевого момента инерции трубы (м^4) и распределенной нагрузки от собственного веса трубопровода (кг/м^3)
            CalculationIntermediateValues();



            double l;// длина части трубопровода

            double step = 0.5;//шаг, с которым происходит перебор вариантов расположения устройств
            double limit;//максимальное расстояние, на которое можно установить устройство 
            if (lC1 > lC2)
            {
                l = lC1;
                limit = lC2;
            }
            else
            {
                l = lC1;
                limit = lC1;
            }


            double Theta0V = coefV[3];//поперечная сила в сечении жесткой заделки по вертикали
            double Theta0H = coefV[3];//поперечная сила в сечении жесткой заделки по горизонтали


            double min = 999999999999;
            for (double l1 = step; l1 < limit - step; l1 += step)
            {
                for (double l2 = l1 + step; l2 < limit; l2 += step)
                {
                    //расчет векторов Xi по вертикали и горизонтали
                    double[] Xv = CalculationOfReactionForcesXi(this.coefV, l1, l2, l, q);
                    double[] Xh = CalculationOfReactionForcesXi(this.coefH, l1, l2, l, 0);//

                    //расчет векторов Pi по вертикали и горизонтали
                    double[] Pv;
                    double[] Ph;
                    if (lC1 > lC2)
                    {
                       Pv = CalculationOfCenteringForcesPi(l1, l2, lC2,hy, Theta0V, q);
                       Ph = CalculationOfCenteringForcesPi(l1, l2, lC2,hz, Theta0H, 0);//
                    }
                    else
                    {
                       Pv = CalculationOfCenteringForcesPi(l1, l2, lC1,hy, Theta0V, q);
                       Ph = CalculationOfCenteringForcesPi(l1, l2, lC1,hz, Theta0H, 0);//
                    }

                    //Расчет результирующих значений Xi на 1 и 2 устройстве
                    double X1t =Math.Sqrt(Xv[0] * Xv[0] + Xh[0] * Xh[0]);
                    double X2t= Math.Sqrt(Xv[1] * Xv[1] + Xh[1] * Xh[1]);

                    //Расчет результирующих значений вектора P
                    double P1t = Math.Sqrt(Pv[0] * Pv[0] + Ph[0] * Ph[0]);
                    double P2t = Math.Sqrt(Pv[1] * Pv[1] + Ph[1] * Ph[1]);
                    double P3t = Math.Sqrt(Pv[2] * Pv[2] + Ph[2] * Ph[2]);
                    double P4t = Math.Sqrt(Pv[3] * Pv[3] + Ph[3] * Ph[3]);

                    //Получение списка значений Mz -  изгибающего момента (Н·м) в сечении с координатой трубопровода z 
                    List<double> Mz = GetMz(l1, l2, limit, step, Pv[0], Pv[1], Pv[2], Pv[3], Ph[0], Ph[1], Ph[2], Ph[3]);

                    //Расчет sigmaZ - оценка величины возникающей в стенке трубопровода в процессе центрирования максимальных напряжений
                    double sigmaZ = Math.Round(GetSigmaZ(Mz.Max(), D, I),3);

                    //Если при текущим положении устройств достигаются минимальные усилия, а также результрующие значения Xi и Pi не превышают допустимое усилие для стали трубопровода и sigmaZ не превышает допустимое напряжение, то запоминаем текущее положения устройств и значение, полученные при расчете
                    if (Math.Max(Math.Max(X1t, X2t), Math.Max(P1t, P2t))< permissibleForce && sigmaZ < permissibleStress && Math.Max(Math.Max(X1t, X2t), Math.Max(P1t, P2t)) < min)
                    {
                        min = Math.Max(Math.Max(X1t, X2t), Math.Max(P1t, P2t));
                        this.X1 = Math.Round(X1t,3);
                        this.X2 = Math.Round(X2t,3);
                        X_H = Array.ConvertAll(Xh, x => Math.Round(x, 3)); ;
                        X_V = Array.ConvertAll(Xv, x => Math.Round(x, 3)); ;
                        this.P1 = Math.Round(P1t,3);
                        this.P2 = Math.Round(P2t,3);
                        P_H = Array.ConvertAll(Ph, x => Math.Round(x, 3)); ;
                        P_V = Array.ConvertAll(Pv, x => Math.Round(x, 3)); ;
                        this.ResL1 = l1;
                        this.ResL2 = l2;

                        this.ResM0 = Math.Round(P3t,3);
                        this.ResQ0 = Math.Round(P4t,3);

                        SigmaZlist = GetListSigmaZ(Mz.ToArray(), limit, step, D, I);
                    }
                }
            }
        }

        /// <summary>
        /// Расчет осевого момента инерции трубы (м^4) и распределенной нагрузки от собственного веса трубопровода (кг/м^3)
        /// </summary>
        void CalculationIntermediateValues()
        {
            I = Math.PI * (Math.Pow(D, 4) - Math.Pow(d, 4)) / 64;
            q = ro * 9.81 * Math.PI * (D * D - d * d) / 4;

        }

        /// <summary>
        /// расчет значений Xi, возникающих в гидроцилиндрах при резком смещении концов трубопровода при его разрезании
        /// </summary>
        /// <param name="polinom">массив коэффициентов полинома</param>
        /// <param name="l1">положение 1 устройства</param>
        /// <param name="l2">положение 2 устройства</param>
        /// <param name="l">длина участка трубопровода до вырезания дефектного участка</param>
        /// <returns></returns>
        double[] CalculationOfReactionForcesXi(double[] polinom, double l1, double l2, double l, double q)
        {
            double theta0 = polinom[3];

            //прогиб трубопровода в сечении с координатой li, вычисляемый путем подстановки координаты li в уравнение полинома, м.
            double y1 = polinom[0] * Math.Pow(l1, 4) + polinom[1] * Math.Pow(l1, 3) + polinom[2] * Math.Pow(l1, 2) + polinom[3] * Math.Pow(l1, 1);
            double y2 = polinom[0] * Math.Pow(l2, 4) + polinom[1] * Math.Pow(l2, 3) + polinom[2] * Math.Pow(l2, 2) + polinom[3] * Math.Pow(l2, 1);

            //Коэффициенты канонических уравнений (м/Н) 
            double[,] sigma = new double[2, 2];
            sigma[0, 0] = l1 * l1 * (l1 - l1 / 3) / (2 * E * I);
            sigma[0, 1] = l1 * l1 * (l2 - l1 / 3) / (2 * E * I);
            sigma[1, 0] = l2 * l2 * (l1 - l2 / 3) / (2 * E * I);
            sigma[1, 1] = l2 * l2 * (l2 - l2 / 3) / (2 * E * I);

            //Грузовые члены (м)
            double delta1P = -q * l1 * l1 / (E * I) * (0.25 * Math.Pow((l - l1), 2) + (l * l1 - l1 * l1 / 2) / 3 - l1 * l1 / 24);
            double delta2P = -q * l2 * l2 / (E * I) * (0.25 * Math.Pow((l - l2), 2) + (l * l2 - l2 * l2 / 2) / 3 - l2 * l2 / 24);

            MathMatrix mathMatrix = new MathMatrix();

            //матрица коэффициентов при неизвестных
            double[,] A = new double[,]
            {
                    { sigma[0, 0], sigma[0, 1] },
                    { sigma[1, 0], sigma[1, 1] }
            };
            //матрица коэффициентов свободных членов
            double[] B = new double[] { y1 - delta1P + theta0 * l1, y2 - delta2P + theta0 * l2 };

            //решение слау методом обратной матрицы
            return mathMatrix.MatrixMultiplication(mathMatrix.InverseMatrix(A), B);
        }

        /// <summary>
        /// расчет значений вектора Pi 
        /// </summary>
        /// <param name="l1">положение 1 устройства</param>
        /// <param name="l2">положение 2 устройства</param>
        /// <param name="l">длина участка трубопровода после вырезания дефектного участка</param>
        /// <param name="h">высота центрирования</param>
        /// <param name="Theta0">Theta0</param>
        /// <returns></returns>
        double[] CalculationOfCenteringForcesPi(double l1, double l2, double l, double h, double Theta0, double q)
        {
            MathMatrix mathMatrix = new MathMatrix();

            //матрица коэффициентов при неизвестных
            double[,] A = new double[,]
            {
                    { Math.Pow((l-l1),2)/2 , Math.Pow((l-l2),2)/2, l, l*l/2},
                    { Math.Pow((l-l1),3)/6 , Math.Pow((l-l2),3)/6, l*l/2, l*l*l/6},
                    { 1, 1, 0, 1},
                    { l1, l2, -1, 0},
            };

            //матрица коэффициентов свободных членов
            double[] B = new double[]
            {
                q*Math.Pow(l,3)/6 - E*I*Theta0,
                E*I*h-E*I*Theta0*l + q*Math.Pow(l,4)/24,
                q*l,
                q*l*l/2
            };

            double[,] inverse = mathMatrix.InverseMatrix(A);
            //решение слау методом обратной матрицы
            return mathMatrix.MatrixMultiplication(inverse, B);
        }

        /// <summary>
        /// расчет списка значений изгибающего момента на трубопроводе
        /// </summary>
        /// <param name="l1">положение 1 устройства</param>
        /// <param name="l2">положение 1 устройства</param>
        /// <param name="length">длина участка трубопровода</param>
        /// <param name="step">шаг итераций</param>
        /// <param name="P1v">величина усилия со стороны захватов на 1 устройстве по вертикали </param>
        /// <param name="P2v">величина усилия со стороны захватов на 2 устройстве по вертикали</param>
        /// <param name="M0v">изгибающий момент в сечении по вертикали</param>
        /// <param name="Q0v">поперечная сила в сечении жесткой заделки по вертикали</param>
        /// <param name="P1h">величина усилия со стороны захватов на 1 устройстве по горизонтали</param>
        /// <param name="P2h">величина усилия со стороны захватов на 2 устройстве по горизонтали</param>
        /// <param name="M0h">изгибающий момент в сечении по горизонтали </param>
        /// <param name="Q0h">поперечная сила в сечении жесткой заделки по горизонтали</param>
        /// <returns></returns>
        List<double> GetMz(double l1, double l2, double length, double step, double P1v, double P2v, double M0v, double Q0v, double P1h, double P2h, double M0h, double Q0h)
        {
            List<double> resMz = new List<double>();

            for (double z = 0; z <= length; z += step)
            {

                double MzH = M0h + Q0h * z - 0 * z * z / 2 + P1h * (z - l1) * He(z, l1) + P2h * (z - l2) * He(z, l2);
                double MzV = M0v + Q0v * z - q * z * z / 2 + P1v * (z - l1) * He(z, l1) + P2v * (z - l2) * He(z, l2);//


                double Mz = Math.Sqrt(Math.Pow(MzH, 2) + Math.Pow(MzV, 2));

                resMz.Add(Mz);
            }
            return resMz;
        }

        /// <summary>
        /// Функция Хевисайда
        /// </summary>
        /// <param name="z">координатой трубопровода </param>
        /// <param name="l">положение устройства</param>
        /// <returns></returns>
        double He(double z, double l)
        {
            return ((z - l) >= 0) ? 1 : 0;
        }
        /// <summary>
        /// получения величины возникающей в стенке трубопровода в процессе центрирования максимальных напряжений 
        /// </summary>
        /// <param name="Mz">Изгибающий момент </param>
        /// <param name="D">наружный диаметр</param>
        /// <param name="I">Осевой момент инерции трубы</param>
        /// <returns></returns>
        double GetSigmaZ(double Mz, double D, double I)
        {
            return Mz * D / (2 * I);
        }
        List<Point> GetListSigmaZ(double[] Mz, double length, double step, double D, double I)
        {
            List<Point> res = new List<Point>();
            int i = 0;
            for (double z = 0; z <= length; z += step)
            {
                res.Add(new Point(z, GetSigmaZ(Mz[i], D, I)));
                i++;
            }
            return res;
        }
    }
}