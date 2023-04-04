using DaoModels.DAO.Models;
using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using System.Linq;

namespace ApiMobaileServise.Servise.GoogleApi
{
    public class DetectTrailers : IDetect
    {
        private SqlCommandApiMobile sqlCommandApiMobil = null;

        public void AuchGoole(SqlCommandApiMobile sqlCommandApiMobil)
        {
            this.sqlCommandApiMobil = sqlCommandApiMobil;
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Config.AuchGoogleCloud);
        }

        public string DetectText(params object[] parames)
        {
            string plate = "";
            try
            {
                List<Trailer> trailers = sqlCommandApiMobil.GetTrailers();
                byte[] imager = (byte[])parames[1];
                string idDriver = (string)parames[0];
                var client = ImageAnnotatorClient.Create();
                var image = Google.Cloud.Vision.V1.Image.FromBytes(imager);
                var response = client.DetectText(image);
                var response3 = client.DetectLocalizedObjects(image);
                foreach (var localizedObject in response3)
                {
                    string numPlateTmp = "";
                    Trailer trailer = null;
                    foreach (EntityAnnotation text in response)
                    {
                        if (text.Description != "" && text.Description[text.Description.Length - 1] == '9')
                        {
                            text.Description = text.Description.Remove(text.Description.Length - 1);
                            text.Description += "ST";
                        }
                        if (trailers.FirstOrDefault(t => t.Plate == text.Description) != null)
                        {
                            trailer = trailers.FirstOrDefault(t => t.Plate == text.Description);
                            //sqlCommandApiMobil.SetPlateTrailer(trailer.Id, idDriver);
                            plate = trailer.Plate;
                            break;
                        }
                        else if (trailer != null && trailer.Plate.Contains(text.Description))
                        {
                            numPlateTmp += text.Description;
                        }
                        else if (trailer == null && trailers.FirstOrDefault(t => t.Plate.Contains(text.Description)) != null)
                        {
                            numPlateTmp += text.Description;
                            trailer = trailers.FirstOrDefault(t => t.Plate.Contains(text.Description));
                        }
                        if (numPlateTmp.Length >= 6)
                        {
                            if (trailer != null && trailer.Plate == numPlateTmp)
                            {
                                //sqlCommandApiMobil.SetPlateTrailer(trailer.Id, idDriver);
                                plate = trailer.Plate;
                                numPlateTmp = "";
                                break;
                            }
                            else if (trailer.Plate.Remove(trailer.Plate.Length - 3) == numPlateTmp || trailer.Plate.Remove(trailer.Plate.Length - 2) == numPlateTmp || trailer.Plate.Remove(trailer.Plate.Length - 1) == numPlateTmp)
                            {
                                //sqlCommandApiMobil.SetPlateTrailer(trailer.Id, idDriver);

                                plate = trailer.Plate;
                                break;
                            }
                            else if (numPlateTmp.Length > 7)
                            {
                                trailer = null;
                                numPlateTmp = "";
                            }
                        }
                    }
                    if (plate == "")
                    {
                        plate = numPlateTmp;
                    }

                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            return plate;
        }
    }
}
