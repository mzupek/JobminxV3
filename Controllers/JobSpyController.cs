using AlchemyAPIClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobminxV3.Helpers;
using AlchemyAPIClient.Responses;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JobminxV3.Controllers
{
    public class JobSpyController : Controller
    {
        // GET: JobSpy
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UrlString(string url)
        {
            var client = new AlchemyClient("028a44801d2c3bbb341a5b727046a9866f760d40");
            alchemy keys = new alchemy();
            Uri myuri = new Uri(url);
            AlchemyTextResponse text = await keys.GetCleanText(myuri, client);
            AlchemyKeywordsResponse res = await keys.GetKeywords(text.Text, client);
            AlchemyEntitiesResponse ent = await keys.GetEntities(text.Text, client);
            AlchemyConceptsResponse con = await keys.GetConcepts(text.Text, client);

            ViewBag.Entities = ent.Entities;
            ViewBag.Concepts = con.Concepts;

            return View("Results",res.Keywords);
        }

        [HttpPost]
        public async Task<ActionResult> FullText(string full)
        {
            var client = new AlchemyClient("028a44801d2c3bbb341a5b727046a9866f760d40");
            alchemy keys = new alchemy();
            AlchemyKeywordsResponse res = await keys.GetKeywords(full, client);
            AlchemyEntitiesResponse ent = await keys.GetEntities(full, client);
            AlchemyConceptsResponse con = await keys.GetConcepts(full, client);
            var fieldT = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                          where r.Type == "FieldTerminology"
                          where r.Relevance > 0.2
                         select r.Text).ToList();

            var nonFieldT = (from r in ent.Entities
                            where r.Type != "FieldTerminology"
                            select r.Text).ToList();

            var keywords = (from r in res.Keywords.OrderByDescending(r => r.Relevance)
                            where r.Text != ""
                            where r.Relevance > 0.2
                            select r.Text).ToList();

            var concepts = (from r in con.Concepts.OrderByDescending(r => r.Relevance)
                            where r.Text != ""
                            where r.Relevance > 0.2
                            select r.Text).ToList();
            

            var cleankeywords = keywords.Except(nonFieldT).ToList();
            var cleanconcepts = concepts.Except(nonFieldT).ToList(); 

            ViewBag.Keywords = cleankeywords;
            ViewBag.Entities = fieldT;
            ViewBag.Concepts = cleanconcepts;

            return View("Results");
        }
    }
}