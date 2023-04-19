using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataRoutingSample.Models;
using System.Collections.Generic;
using System.Linq;

namespace ODataRoutingSample.Controllers
{
    public class SamsController : ControllerBase
    {
        MyDataContext _context;

        public SamsController(MyDataContext context)
        {
            _context = context;
            if (_context.Sams.Count() == 0)
            {
                IList<SamCelebrity> sams = GetSams();

                foreach (var sam in sams)
                {
                    _context.Sams.Add(sam);
                }

                _context.SaveChanges();
            }
        }

        /*
        [HttpGet]
        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<SamCelebrity> options)
        {
            //var queryableSam = _context.Sams.Where(s=>s.Id>0);

            var queryableSam = _context.Sams;
            var queryableSamOData = options.ApplyTo(queryableSam);

            return Ok(queryableSamOData);
        }
        */

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var sams = _context.Sams
                // fill the dictionary dynamicaly
                .Select(s=>AddAsDynamicProperty(s));

            // Exeception during the execution of the query
            // I understand that the dictionary is not part of the context database but shouldn't the call
            // to the dynamic property be removed from the current filter expression and executed after the SQL query? For example the DiseaseIndex gt 0 filter should not be executed after the database data retrieval foreach?             
            return Ok(sams);
        }

        static int diseaseIndex = 0;

        private SamCelebrity AddAsDynamicProperty(SamCelebrity sam)
        {
            sam.Properties.Add("DiseaseIndex", diseaseIndex++);

            return sam;
        }

        /*
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_context.Sams.AsQueryable());
        }
        */

        private IList<SamCelebrity> GetSams()
        {
            var samList = new List<SamCelebrity>();

            samList.Add(new SamCelebrity() { Id = 1, Name = "L. Jackson" });
            samList.Add(new SamCelebrity() { Id = 2, Name = "Colt" });

            return samList;
        }
    }
}
