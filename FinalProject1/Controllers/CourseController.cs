using FinalProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject1.Controllers
{
    public class CourseController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();

        public ActionResult AddCourse()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCourse(Cours c)
        {
            if (ModelState.IsValid)
            {
                Cours c1 = new Cours();

                c1.Course_ID = c.Course_ID;
                c1.Course_Name = c.Course_Name;
                c1.Pre_Requisite_Course_ID = c.Pre_Requisite_Course_ID;
                c1.Credit_Hours = c.Credit_Hours;

                db.Courses.Add(c1);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Course Added Successfully";
                return View(c1);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ViewCourse()
        {
            var courses = db.Courses.ToList(); // Retrieve all students from the database
            return View(courses);
        }

    }
}