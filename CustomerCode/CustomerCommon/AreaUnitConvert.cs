using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.BaseLib
{
    public class AreaUnitConvert
    {
        public const double _MuToSquaremeter = 10000 / 15;

        public const double _SquaremeterToMu = 15 / 10000;

        public const double _MuToHectare = 1 / 15;

        public const double _HectareToMu = 15 / 1;

        public const double _HectareToSquaremeter = 10000;

        public const double _SquaremeterToHectare = 1 / 10000;

        public const double _KilometerToHectare = 100;

        public const double _HectareToKilometer = 1 / 100;

        public const double _KilometerToSquaremeter = 1000000;

        public const double _SquaremeterToKilometer = 1 / 1000000;

        public const double _MuToKilometer = 1 / 1500;

        public const double _KilometerToMu = 1500;

        public static double MuToSquaremeter(double mu) { return mu * _MuToSquaremeter; }

        public static double SquaremeterToMu(double squaremeter) { return squaremeter * _SquaremeterToMu; }

        public static double MuToHectare(double mu) { return mu * _MuToHectare; }

        public static double HectareToMu(double hectare) { return hectare * _HectareToMu; }

        public static double HectareToSquaremeter(double hectare) { return hectare * _HectareToSquaremeter; }

        public static double SquaremeterToHectare(double squaremeter) { return squaremeter * _SquaremeterToHectare; }

        public static double KilometerToHectare(double kilometer) { return kilometer * _KilometerToHectare; }

        public static double HectareToKilometer(double hectare) { return hectare * _HectareToKilometer; }

        public static double KilometerToSquaremeter(double kilometer) { return kilometer * _KilometerToSquaremeter; }

        public static double SquaremeterToKilometer(double squaremeter) { return squaremeter * _SquaremeterToKilometer; }

        public static double MuToKilometer(double mu) { return mu * _MuToKilometer; }

        public static double KilometerToMu(double kilometer) { return kilometer * _KilometerToMu; }

    }
}
