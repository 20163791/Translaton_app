using System;
using System.Net.Http;
using System.Web.Mvc;
using translator.Models;

namespace translator.Controllers
{
    public class TranslationController : Controller
    {
        // GET: Translation
        [HttpGet]
        public JsonResult GetTranslation(String input_text)
        {
            Translation translation = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://127.0.0.1:5000/");
                var responseTask = client.GetAsync($"translate/{input_text}");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Translation>();
                    readTask.Wait();
                    translation = readTask.Result;
                    translation.output_text = Cleanup(translation.output_text);
                }
                else
                {
                    translation.output_text = "error";
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return Json(translation, JsonRequestBehavior.AllowGet);
        }

        public String Cleanup(String input)
        {
            return input.Substring(0, input.Length - 7);
        }
    }
}