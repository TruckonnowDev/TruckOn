﻿using MDispatch.Models.Enum;
using MDispatch.NewElement;
using Plugin.Settings;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV.Servise.Models
{
    public class MotorcycleTricycle : IVehicle
    {
        public string TypeIndex { get; set; } = "Tricycle";
        public string TypeVehicle { get; set; } = "motorcycle";
        public int CountCarImg { get; set; } = 23;

        public int GetIndexCar(int countPhoto)
        {
            int indecCar = 0;
            switch (countPhoto)
            {
                case 1:
                    {
                        indecCar = 24;
                        break;
                    }
                case 2:
                    {
                        indecCar = 4;
                        break;
                    }
                case 3:
                    {
                        indecCar = 5;
                        break;
                    }
                case 4:
                    {
                        indecCar = 25;
                        break;
                    }
                case 5:
                    {
                        indecCar = 10;
                        break;
                    }
                case 6:
                    {
                        indecCar = 8;
                        break;
                    }
                case 7:
                    {
                        indecCar = 9;
                        break;
                    }
                case 8:
                    {
                        indecCar = 11;
                        break;
                    }
                case 9:
                    {
                        indecCar = 12;
                        break;
                    }
                case 10:
                    {
                        indecCar = 26;
                        break;
                    }
                case 11:
                    {
                        indecCar = 16;
                        break;
                    }
                case 12:
                    {
                        indecCar = 15;
                        break;
                    }
                case 13:
                    {
                        indecCar = 27;
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
            switch (inderxPhotoInspektion)
            {
                case 1: nameLayout = "Vehicle(Tricycle) dashboard"; break;
                case 2: nameLayout = "Vehicle(Tricycle) driver seat"; break;
                case 3: nameLayout = "Vehicle(Tricycle) interior"; break;
                case 4: nameLayout = "The central part of the vehicle(Tricycle) on the driver’s side"; break;
                case 5: nameLayout = "Rear-view mirror of the vehicle(Tricycle) on the driver side"; break;
                case 6: nameLayout = "Front of the vehicle(Tricycle), driver's side"; break;
                case 7: nameLayout = "The front wheel of the vehicle(Tricycle) the driver's side"; break;
                case 8: nameLayout = "The right side of the front bumper of the vehicle(Tricycle)"; break;
                case 9: nameLayout = "The central side of the front bumper of the vehicle(Tricycle)"; break;
                case 10: nameLayout = "The central side of the front bumper of the vehicle(Tricycle)"; break;
                case 11: nameLayout = "The left side of the front bumper of the vehicle(Tricycle)"; break;
                case 12: nameLayout = "The entire lower part of the front bumper of the vehicle(Tricycle)"; break;
                case 13: nameLayout = "Front of the vehicle(Tricycle), passenger side"; break;
                case 14: nameLayout = "The front wheel of the vehicle(Tricycle) the passenger side"; break;
                case 15: nameLayout = "The front wheel of the vehicle(Tricycle) the passenger side"; break;
                case 16: nameLayout = "The central part of the vehicle(Tricycle) on the passenger side"; break;
                case 17: nameLayout = "Rear of the vehicle(Tricycle) on the passenger side"; break;
                case 18: nameLayout = "The rear wheel of the vehicle(Tricycle) from the passenger side"; break;
                case 19: nameLayout = "Vehicle(Tricycle)"; break;
                case 20: nameLayout = "Vehicle(Tricycle) Rear Wheel Hitch Place"; break;
                case 21: nameLayout = "Vehicle(Tricycle) Rear Wheel Hitch Place"; break;
                case 22: nameLayout = "The rear wheel of the vehicle(Tricycle) from the driver’s side"; break;
                case 23: nameLayout = "Rear of the vehicle(Tricycle) on the driver’s side"; break;

                case 24: nameLayout = "Rear belt mount vehicle on the driver's side"; break;
                case 25: nameLayout = "Front belt mount vehicle on the driver's side"; break;
                case 26: nameLayout = "Front belt mount vehicle on the passenger side"; break;
                case 27: nameLayout = "Rear belt mount vehicle on the passenger side"; break;
            }
            return nameLayout;
        }

        public string GetNameLayoutRussian(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            switch (inderxPhotoInspektion)
            {
                case 1: nameLayout = "Приборная панель мотоцикла(Motocicleta triciclo)"; break;
                case 2: nameLayout = "Сиденье водителя мотоцикла(Motocicleta triciclo)"; break;
                case 3: nameLayout = "Салон"; break;
                case 4: nameLayout = "Центральная часть мотоцикла(Motocicleta triciclo) со стороны водителя"; break;
                case 5: nameLayout = "Зеркало заднего вида мотоцикла(Motocicleta triciclo) со стороны водителя"; break;
                case 6: nameLayout = "Передняя часть мотоцикла(Motocicleta triciclo) со стороны водителя"; break;
                case 7: nameLayout = "Переднее колесо мотоцикла(Motocicleta triciclo) со стороны водителя"; break;
                case 8: nameLayout = "Правая сторона переднего бампера мотоцикла(Motocicleta triciclo)"; break;
                case 9: nameLayout = "Центральная сторона переднего бампера мотоцикла(Motocicleta triciclo)"; break;
                case 10: nameLayout = "Центральная сторона переднего бампера мотоцикла(Motocicleta triciclo)"; break;
                case 11: nameLayout = "Левая сторона переднего бампера мотоцикла(Motocicleta triciclo)"; break;
                case 12: nameLayout = "Вся нижняя часть переднего бампера мотоцикла(Motocicleta triciclo)"; break;
                case 13: nameLayout = "Передняя часть мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 14: nameLayout = "Переднее колесо мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 15: nameLayout = "Переднее колесо мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 16: nameLayout = "Центральная часть мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 17: nameLayout = "адняя часть мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 18: nameLayout = "Заднее колесо мотоцикла(Motocicleta triciclo) со стороны пассажира"; break;
                case 19: nameLayout = "---------------(Tricycle)"; break;
                case 20: nameLayout = "Место сцепки заднего колеса"; break;
                case 21: nameLayout = "Место сцепки заднего колеса"; break;
                case 22: nameLayout = "Заднее колесо мотоцикла(Motocicleta triciclo) со стороны водителя"; break;
                case 23: nameLayout = "Задняя часть мотоцикла(Трехколесный велосипед) со стороны водителя"; break;

                case 24: nameLayout = "Задний ремень крепления мотоцикла на стороне водителя"; break;
                case 25: nameLayout = "Передний ремень крепления мотоцикла на стороне водителя"; break;
                case 26: nameLayout = "Передниый ремень крепления мотоцикла на стороне пасажира"; break;
                case 27: nameLayout = "Задний ремень крепления мотоцикла на стороне пасажира"; break;
            }
            return nameLayout;
        }

        public string GetNameLayoutSpanish(int inderxPhotoInspektion)
        {
            string nameLayout = "";
            switch (inderxPhotoInspektion)
            {
                case 1: nameLayout = "Tablero de la motocicleta(Motocicleta triciclo)"; break;
                case 2: nameLayout = "Asiento del conductor de la motocicleta(Motocicleta triciclo)"; break;
                case 3: nameLayout = "Salón"; break;
                case 4: nameLayout = "El centro de la motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;
                case 5: nameLayout = "Espejo retrovisor de motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;
                case 6: nameLayout = "Frente de la motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;
                case 7: nameLayout = "Rueda delantera de motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;
                case 8: nameLayout = "Parachoques delantero de motocicleta lado derecho(Motocicleta triciclo)"; break;
                case 9: nameLayout = "Lado central del parachoques delantero de la motocicleta(Motocicleta triciclo)"; break;
                case 10: nameLayout = "Lado central del parachoques delantero de la motocicleta(Motocicleta triciclo)"; break;
                case 11: nameLayout = "Lado izquierdo del parachoques delantero de la motocicleta(Motocicleta triciclo)"; break;
                case 12: nameLayout = "Toda la parte inferior del parachoques delantero de la motocicleta.(Motocicleta triciclo)"; break;
                case 13: nameLayout = "Frente de la motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 14: nameLayout = "Rueda delantera de motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 15: nameLayout = "Rueda delantera de motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 16: nameLayout = "El centro de la motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 17: nameLayout = "Parte trasera de la motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 18: nameLayout = "Rueda trasera de motocicleta(Motocicleta triciclo) desde el lado del pasajero"; break;
                case 19: nameLayout = "---------------(Motocicleta triciclo)"; break;
                case 20: nameLayout = "Punto de enganche de la rueda trasera"; break;
                case 21: nameLayout = "Punto de enganche de la rueda trasera"; break;
                case 22: nameLayout = "Rueda trasera de motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;
                case 23: nameLayout = "Parte trasera de la motocicleta(Motocicleta triciclo) desde el lado del conductor"; break;

                case 24: nameLayout = "Correa trasera de motocicleta en el lado del conductor"; break;
                case 25: nameLayout = "Correa de sujeción delantera de la motocicleta en el lado del conductor."; break;
                case 26: nameLayout = "Correa de motocicleta delantera en el lado del pasajero"; break;
                case 27: nameLayout = "Correa trasera de motocicleta en el lado del pasajero"; break;
            }
            return nameLayout;
        }

        public async Task OrintableScreen(int inderxPhotoInspektion)
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
        }
    }
}