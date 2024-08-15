using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OCR.Models;

namespace OCR.Controllers
{
    public class CameraController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private static List<string> recognizedDigits = new List<string>();
        OCREntities db = new OCREntities();

        public ActionResult StartRecognition()
        { 
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> post_StartRecognition(string jetsonNanoUrl)
        {
            try
            {
                var response = await client.PostAsync($"{jetsonNanoUrl}/start_recognition", null);
                var result = await response.Content.ReadAsStringAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> StopRecognition(string jetsonNanoUrl)
        {
            var response = await client.PostAsync($"{jetsonNanoUrl}/stop_recognition", null);
            var result = await response.Content.ReadAsStringAsync();
            return Json(result);
        }

        [HttpPost]
        public ActionResult ReceiveDigit(List<string> digits, DateTime datetime)
        {
            System.Diagnostics.Debug.WriteLine($"Received digits: {digits}, datetime: {datetime}");
            recognizedDigits.Clear();
            if (digits != null && digits.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Received digit: {digits}");
                recognizedDigits.Insert(0, string.Join(",", digits));
                // Insert db
                Camera data = new Camera();
                data.recognition = string.Join(",", digits);
                data.time = datetime;
                db.Camera.Add(data);
                db.SaveChanges();
                return Json(new { success = "db save" });
            }

            return Json(new { success = "true but not save db" });
        }

        [HttpGet]
        public ActionResult GetRecognizedDigits()
        {
            var latestCameraRecord = db.Camera.OrderByDescending(c => c.id).FirstOrDefault();
            return Json(new { digits = latestCameraRecord?.recognition.ToString()+" "+latestCameraRecord.time }, JsonRequestBehavior.AllowGet);
        }
    }
}