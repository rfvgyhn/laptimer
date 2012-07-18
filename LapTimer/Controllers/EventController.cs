﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTimer.Services;
using LapTimer.Models;
using LapTimer.Models.ViewModels;
using LapTimer.Infrastructure.Extensions;

namespace LapTimer.Controllers
{
    public class EventController : Controller
    {
        IEventService eventService;

        public EventController(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ByDate(string slug, DateTime? date = null)
        {
            if (date != null)
            {
                var @event = eventService.Single(e => e.Location.Slug == slug && e.Date.Date == date.Value.Date);

                return View("Details", @event);
            }
            
            var model = eventService.Find(e => e.Location.Slug == slug);

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(CreateEventViewModel model)
        {
            Event e = new Event
            {
                Location = new Location { Name = model.LocationName, Slug = model.LocationName.ToSlug() },
                Date = DateTime.UtcNow
            };
            Session session = new Session { Id = 0 };

            foreach (var kvp in model.Participants)
                session.Participants.Add(new Participant { Name = kvp.Value, Number = kvp.Key });

            e.Sessions.Add(session);

            eventService.Save(e);

            return RedirectToRoute("ByDate", new { slug = e.Location.Slug, date = e.Date.ToSlug(), action = "ByDate" });
            
        }

        public ActionResult Timing(string slug, DateTime date)
        {
            var model = eventService.Single(e => e.Location.Slug == slug && e.Date.Date == date.Date);

            return View(model);
        }
        
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult AddLap(SaveLapViewModel model)
        {
            var @event = eventService.Single(model.EventId);
            var session = @event.Sessions.Where(s => s.Id == model.SessionId).Single();
            var participant = session.Participants.Where(p => p.Number == model.Participant).Single();

            participant.Times.Add(TimeSpan.FromMilliseconds(model.Time));

            eventService.Save(@event);

            return Json(new { });
        }
    }
}
