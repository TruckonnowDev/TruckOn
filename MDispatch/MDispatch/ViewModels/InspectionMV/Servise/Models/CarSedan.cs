﻿using MDispatch.Models.Enum;
using MDispatch.NewElement;
using Plugin.Settings;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV.Servise.Models
{
    public class CarSedan : IVehicle
    {
        public string TypeIndex { get; set; } = "Sedan";
        public int CountCarImg { get; set; } = 33;
        public string TypeVehicle { get; set; } = "car";

        public int GetIndexCar(int countPhoto)
        {
            int indecCar = 0;
            switch (countPhoto)
            {
                case 1:
                    {
                        indecCar = 34;
                        break;
                    }
                case 2:
                    {
                        indecCar = 5;
                        break;
                    }
                case 3:
                    {
                        indecCar = 6;
                        break;
                    }
                case 4:
                    {
                        indecCar = 35;
                        break;
                    }
                case 5:
                    {
                        indecCar = 16;
                        break;
                    }
                case 6:
                    {
                        indecCar = 10;
                        break;
                    }
                case 7:
                    {
                        indecCar = 12;
                        break;
                    }
                case 8:
                    {
                        indecCar = 13;
                        break;
                    }
                case 9:
                    {
                        indecCar = 36;
                        break;
                    }
                case 10:
                    {
                        indecCar = 20;
                        break;
                    }
                case 11:
                    {
                        indecCar = 21;
                        break;
                    }
                case 12:
                    {
                        indecCar = 37;
                        break;
                    }
                default:
                    {
                        indecCar = 0;
                        break;
                    }
            }
            return indecCar;
        }

       
        public string GetNameLayout(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            if (CrossSettings.Current.GetValueOrDefault("Language", (int)LanguageType.English) == (int)LanguageType.English)
            {
                nameLayout = GetNameLayoutEnglish(inderxPhotoInspektion);
            }
            else if (CrossSettings.Current.GetValueOrDefault("Language", (int)LanguageType.English) == (int)LanguageType.Russian)
            {
                nameLayout = GetNameLayoutRussian(inderxPhotoInspektion);
            }
            else if (CrossSettings.Current.GetValueOrDefault("Language", (int)LanguageType.English) == (int)LanguageType.Spanish)
            {
                nameLayout = GetNameLayoutSpanish(inderxPhotoInspektion);
            }
            return nameLayout;
        }

        public string GetNameLayoutEnglish(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            if (inderxPhotoInspektion == 1)
            {
                nameLayout = "Vehicle(Sedan) dashboard";
            }
            else if (inderxPhotoInspektion == 2)
            {
                nameLayout = "Vehicle(Sedan) driver's seat";
            }
            else if (inderxPhotoInspektion == 3)
            {
                nameLayout = "Vehicle(Sedan) interior";
            }
            else if (inderxPhotoInspektion == 4)
            {
                nameLayout = "Driving door from inside the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 5)
            {
                nameLayout = "Front door on the driver’s side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 6)
            {
                nameLayout = "Rear view mirror on the driver’s side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 7)
            {
                nameLayout = "Rear view mirror on the driver’s side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 8)
            {
                nameLayout = "Front of the vehicle(Sedan), driver's side";
            }
            else if (inderxPhotoInspektion == 9)
            {
                nameLayout = "Front wheel of the vehicle(Sedan), driver's side";
            }
            else if (inderxPhotoInspektion == 10)
            {
                nameLayout = "Right side of the front bumper of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 11)
            {
                nameLayout = "Right front headlight of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 12)
            {
                nameLayout = "Center side of the front bumper of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 13)
            {
                nameLayout = "Left side of the front bumper of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 14)
            {
                nameLayout = "Left front headlight of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 15)
            {
                nameLayout = "Vehicle(Sedan) hood";
            }
            else if (inderxPhotoInspektion == 16)
            {
                nameLayout = "Vehicle(Sedan) Windshield";
            }
            else if (inderxPhotoInspektion == 17)
            {
                nameLayout = "----------(Sedan)";
            }
            else if (inderxPhotoInspektion == 18)
            {
                nameLayout = "Front of the vehicle(Sedan), passenger side";
            }
            else if (inderxPhotoInspektion == 19)
            {
                nameLayout = "Front wheel of the vehicle(Sedan), passenger side";
            }
            else if (inderxPhotoInspektion == 20)
            {
                nameLayout = "Front door on the passenger side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 21)
            {
                nameLayout = "Rear view mirror on the passenger side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 22)
            {
                nameLayout = "Rear view mirror on the passenger side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 23)
            {
                nameLayout = "The rear door of the vehicle(Sedan) on the passenger side";
            }
            else if (inderxPhotoInspektion == 24)
            {
                nameLayout = "Rear wheel of the vehicle(Sedan), passenger side";
            }
            else if (inderxPhotoInspektion == 25)
            {
                nameLayout = "Rear of the vehicle(Sedan) on the passenger side";
            }
            else if (inderxPhotoInspektion == 26)
            {
                nameLayout = "All part of the vehicle(Sedan) on the passenger side";
            }
            else if (inderxPhotoInspektion == 27)
            {
                nameLayout = "The right side of the rear bumper of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 28)
            {
                nameLayout = "The center side of the rear bumper of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 29)
            {
                nameLayout = "The left side of the rear bumper of the vehicle";
            }
            else if (inderxPhotoInspektion == 30)
            {
                nameLayout = "All part of the vehicle(Sedan) on the driver's side";
            }
            else if (inderxPhotoInspektion == 31)
            {
                nameLayout = "Rear of the vehicle(Sedan) on the driver's side";
            }
            else if (inderxPhotoInspektion == 32)
            {
                nameLayout = "Rear wheel of the vehicle(Sedan), driver's side";
            }
            else if (inderxPhotoInspektion == 33)
            {
                nameLayout = "The rear door of the vehicle(Sedan) on the driver's side";
            }
            else if (inderxPhotoInspektion == 34)
            {
                nameLayout = "Rear belt mount vehicle on the driver's side";
            }
            else if (inderxPhotoInspektion == 35)
            {
                nameLayout = "Front belt mount vehicle on the driver's side";
            }
            else if (inderxPhotoInspektion == 36)
            {
                nameLayout = "Front belt mount vehicle on the passenger side";
            }
            else if (inderxPhotoInspektion == 37)
            {
                nameLayout = "Rear belt mount vehicle on the passenger side";
            }
            return nameLayout;
        }

        public string GetNameLayoutRussian(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            if (inderxPhotoInspektion == 1)
            {
                nameLayout = "Приборная панель автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 2)
            {
                nameLayout = "Сиденье водителя автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 3)
            {
                nameLayout = "Автомобиль(Седан) салон";
            }
            else if (inderxPhotoInspektion == 4)
            {
                nameLayout = "Дверь из салона автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 5)
            {
                nameLayout = "Передняя дверь с водительской стороны автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 6)
            {
                nameLayout = "Зеркало заднего вида на стороне водителя автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 7)
            {
                nameLayout = "Зеркало заднего вида на стороне водителя автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 8)
            {
                nameLayout = "Передняя часть автомобиля(Седан) со стороны водителя";
            }
            else if (inderxPhotoInspektion == 9)
            {
                nameLayout = "Переднее колесо автомобиля(Седан) со стороны водителя";
            }
            else if (inderxPhotoInspektion == 10)
            {
                nameLayout = "Правая сторона переднего бампера автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 11)
            {
                nameLayout = "Правая передняя фара автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 12)
            {
                nameLayout = "Центральная сторона переднего бампера автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 13)
            {
                nameLayout = "Левая сторона переднего бампера автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 14)
            {
                nameLayout = "Левая передняя фара автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 15)
            {
                nameLayout = "Капот";
            }
            else if (inderxPhotoInspektion == 16)
            {
                nameLayout = "Лобовое стекло";
            }
            else if (inderxPhotoInspektion == 17)
            {
                nameLayout = "----------(Седан)";
            }
            else if (inderxPhotoInspektion == 18)
            {
                nameLayout = "Передняя часть автомобиля(Седан) со стороны пассажира";
            }
            else if (inderxPhotoInspektion == 19)
            {
                nameLayout = "Переднее колесо автомобиля(Седан) со стороны пассажира";
            }
            else if (inderxPhotoInspektion == 20)
            {
                nameLayout = "Передняя дверь со стороны пассажира автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 21)
            {
                nameLayout = "Зеркало заднего вида на стороне пассажира автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 22)
            {
                nameLayout = "Rear view mirror on the passenger side of the vehicle(Sedan)";
            }
            else if (inderxPhotoInspektion == 23)
            {
                nameLayout = "Задняя дверь автомобиля(Седан) со стороны пассажира";
            }
            else if (inderxPhotoInspektion == 24)
            {
                nameLayout = "Заднее колесо автомобиля(Седан) со стороны пассажира";
            }
            else if (inderxPhotoInspektion == 25)
            {
                nameLayout = "Задняя часть автомобиля(Седан) со стороны пассажира";
            }
            else if (inderxPhotoInspektion == 26)
            {
                nameLayout = "Вся часть автомобиля(Седан) на стороне пассажира";
            }
            else if (inderxPhotoInspektion == 27)
            {
                nameLayout = "Правая сторона заднего бампера автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 28)
            {
                nameLayout = "Центральная сторона заднего бампера автомобиля(Седан)";
            }
            else if (inderxPhotoInspektion == 29)
            {
                nameLayout = "Левая сторона заднего бампера автомобиля";
            }
            else if (inderxPhotoInspektion == 30)
            {
                nameLayout = "Вся часть автомобиля(Седан) на стороне водителя";
            }
            else if (inderxPhotoInspektion == 31)
            {
                nameLayout = "Задняя часть автомобиля(Седан) со стороны водителя";
            }
            else if (inderxPhotoInspektion == 32)
            {
                nameLayout = "Заднее колесо автомобиля(Седан) со стороны водителя";
            }
            else if (inderxPhotoInspektion == 33)
            {
                nameLayout = "Задняя дверь автомобиля(Седан) со стороны водителя";
            }
            else if (inderxPhotoInspektion == 34)
            {
                nameLayout = "Задний ремень крепления автомобиля на стороне водителя";
            }
            else if (inderxPhotoInspektion == 35)
            {
                nameLayout = "Передний ремень крепления автомобиля на стороне водителя";
            }
            else if (inderxPhotoInspektion == 36)
            {
                nameLayout = "Автомобиль с передним ремнем безопасности на стороне пассажира";
            }
            else if (inderxPhotoInspektion == 37)
            {
                nameLayout = "Автомобиль с креплением на ремне сзади на стороне пассажира";
            }
            return nameLayout;
        }

        public string GetNameLayoutSpanish(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            if (inderxPhotoInspektion == 1)
            {
                nameLayout = "Tablero de instrumentos del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 2)
            {
                nameLayout = "Asiento del conductor del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 3)
            {
                nameLayout = "Interior del vehículo(Sedán)";
            }
            else if (inderxPhotoInspektion == 4)
            {
                nameLayout = "Puerta del auto(Sedán)";
            }
            else if (inderxPhotoInspektion == 5)
            {
                nameLayout = "Puerta de entrada en el lado del conductor del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 6)
            {
                nameLayout = "Espejo retrovisor en el lado del conductor del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 7)
            {
                nameLayout = "Espejo retrovisor en el lado del conductor del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 8)
            {
                nameLayout = "Delante del coche(Sedán) desde el lado del conductor";
            }
            else if (inderxPhotoInspektion == 9)
            {
                nameLayout = "Rueda delantera del coche(Sedán) desde el lado del conductor";
            }
            else if (inderxPhotoInspektion == 10)
            {
                nameLayout = "Lado derecho del parachoques delantero del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 11)
            {
                nameLayout = "Faro delantero derecho del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 12)
            {
                nameLayout = "El lado central del parachoques delantero del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 13)
            {
                nameLayout = "Lado izquierdo del parachoques delantero del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 14)
            {
                nameLayout = "Faro delantero izquierdo del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 15)
            {
                nameLayout = "Сapucha";
            }
            else if (inderxPhotoInspektion == 16)
            {
                nameLayout = "Parabrisas";
            }
            else if (inderxPhotoInspektion == 17)
            {
                nameLayout = "----------(Sedán)";
            }
            else if (inderxPhotoInspektion == 18)
            {
                nameLayout = "Delante del coche(Sedán) desde el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 19)
            {
                nameLayout = "Rueda delantera del coche(Sedán) desde el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 20)
            {
                nameLayout = "Puerta delantera en el lado del pasajero del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 21)
            {
                nameLayout = "Espejo retrovisor en el lado del pasajero del automóvil(Sedán)";
            }
            else if (inderxPhotoInspektion == 22)
            {
                nameLayout = "Espejo retrovisor en el lado del pasajero del vehículo(Sedán)";
            }
            else if (inderxPhotoInspektion == 23)
            {
                nameLayout = "Puerta trasera del coche(Sedán) desde el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 24)
            {
                nameLayout = "Rueda trasera de un coche(Sedán) desde el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 25)
            {
                nameLayout = "Parte trasera del coche(Sedán) desde el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 26)
            {
                nameLayout = "Toda la parte del coche(Sedán) en el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 27)
            {
                nameLayout = "Lado derecho del parachoques trasero del coche(Sedán)";
            }
            else if (inderxPhotoInspektion == 28)
            {
                nameLayout = "Центральная сторона заднего бампера автомобиля(Sedán)";
            }
            else if (inderxPhotoInspektion == 29)
            {
                nameLayout = "Lado izquierdo del parachoques trasero del coche";
            }
            else if (inderxPhotoInspektion == 30)
            {
                nameLayout = "Toda la parte del coche(Sedán) en el lado del conductor";
            }
            else if (inderxPhotoInspektion == 31)
            {
                nameLayout = "Parte trasera del coche(Sedán) desde el lado del conductor";
            }
            else if (inderxPhotoInspektion == 32)
            {
                nameLayout = "Rueda trasera de un coche(Sedán) desde el lado del conductor";
            }
            else if (inderxPhotoInspektion == 33)
            {
                nameLayout = "Puerta trasera del coche(Sedán) desde el lado del conductor";
            }
            else if (inderxPhotoInspektion == 34)
            {
                nameLayout = "Correa de sujeción trasera del coche en el lado del conductor";
            }
            else if (inderxPhotoInspektion == 35)
            {
                nameLayout = "Correa de sujeción delantera del coche en el lado del conductor";
            }
            else if (inderxPhotoInspektion == 36)
            {
                nameLayout = "Coche con cinturón de seguridad delantero en el lado del pasajero";
            }
            else if (inderxPhotoInspektion == 37)
            {
                nameLayout = "Coche con clip para cinturón en la parte trasera del lado del pasajero";
            }
            return nameLayout;
        }

        public async Task OrintableScreen(int inderxPhotoInspektion)
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            //if (inderxPhotoInspektion == 2 || inderxPhotoInspektion == 3 || inderxPhotoInspektion == 4 || inderxPhotoInspektion == 5 || inderxPhotoInspektion == 6 || inderxPhotoInspektion == 7  || inderxPhotoInspektion == 25 
            //    || inderxPhotoInspektion == 26 || inderxPhotoInspektion == 27 || inderxPhotoInspektion == 32 || inderxPhotoInspektion == 20 || inderxPhotoInspektion == 22)
            //{
            //    DependencyService.Get<IOrientationHandler>().ForcePortrait();
            //}
            //else if (inderxPhotoInspektion == 1 || inderxPhotoInspektion == 8 || inderxPhotoInspektion == 9 || inderxPhotoInspektion == 10 || inderxPhotoInspektion == 11 || inderxPhotoInspektion == 12 || inderxPhotoInspektion == 13 || inderxPhotoInspektion == 14
            //    || inderxPhotoInspektion == 15 || inderxPhotoInspektion == 16 || inderxPhotoInspektion == 17 || inderxPhotoInspektion == 18 || inderxPhotoInspektion == 19  || inderxPhotoInspektion == 23
            //    || inderxPhotoInspektion == 24 || inderxPhotoInspektion == 28 || inderxPhotoInspektion == 29 || inderxPhotoInspektion == 30 || inderxPhotoInspektion == 31 || inderxPhotoInspektion == 33 || inderxPhotoInspektion == 21)
            //{
            //    DependencyService.Get<IOrientationHandler>().ForceLandscape();
            //}
        }
    }
}