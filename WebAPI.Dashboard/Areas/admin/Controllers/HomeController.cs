using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Dashboard.Controllers;

namespace WebAPI.Dashboard.Areas.admin.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        public HomeController(IDocumentStore store)
            : base(store)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!Session.Query<AdminContainer>().Any(x => x.Emails.Any(y => y == Account.Email)))
            {
                ErrorMessage = "Nothing to see there.";

                return RedirectToRoute("default", new
                    {
                        controller = "Home"
                    });
            }

            var stats = Session.Query<StatsPerService.Stats, StatsPerService>()
                               .Select(x => x).ToList();

            var statsModel = Mapper.Map<List<StatsPerService.Stats>, List<ServiceStatsViewModel>>(stats);

            var then = DateTime.Parse("2/24/2013").Ticks;
            var lineChart = Session.Query<RequestsPerDay.Stats, RequestsPerDay>()
                                   .Where(x => x.Date > then)
                                   .OrderByDescending(x=>x.Date)
                                   .ToList();

            var line = TransformToD3JsLine(lineChart);

            var model = new
                {
                    stats = statsModel,
                    lineChart = line
                };

            return View("Index");
        }
    }
}