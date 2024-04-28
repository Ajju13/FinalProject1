using FinalProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject1.Controllers
{
    public class ClassController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();
        // GET: Class


        public ActionResult AddClass()
        {
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name");
            ViewBag.Teacher_ID = new SelectList(db.Facutlies, "Teacher_ID", "Teacher_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClass(Class cls)
        {
            // Check if the provided Class ID already exists
            var existingClass = db.Classes.Find(cls.Class_ID);
            if (existingClass != null)
            {
                ModelState.AddModelError("Class_ID", "Class ID already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Classes.Add(cls);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Class added successfully.";
                return RedirectToAction("AdminMain", "Admin");
            }

            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_Name", cls.Course_ID);
            // Other ViewBag assignments for dropdown lists

            return View(cls);
        }

        public ActionResult ViewClass()
        {
            var classes = db.Classes.ToList(); // Retrieve all students from the database
            return View(classes);
        }

        public ActionResult CourseSelection()
        {
            var classes = db.Classes.ToList();
            var studentList = db.Students.ToList();

            // Convert the list of classes to a list of SelectListItem
            var classListItems = classes.Select(c => new SelectListItem
            {
                Value = c.Class_ID.ToString(), // Assuming 'Class_ID' is the property representing the value
                Text = c.Class_Day // Assuming 'Class_Name' is the property representing the display text
            }).ToList();

            var studentSelectList = studentList.Select(s => new SelectListItem
            {
                Value = s.Student_ID.ToString(), // Assuming Student_ID is the property representing the value
                Text = s.Student_Name // Assuming Student_Name is the property representing the display text
            });

            // Add a default option if needed
            classListItems.Insert(0, new SelectListItem { Value = "", Text = "Select a class" });

            // Add the list of SelectListItem to ViewData with the key 'Class_ID'
            ViewData["Class_ID"] = classListItems;

            // Add the SelectListItem collection to ViewData with the appropriate key
            ViewData["StudentList"] = studentSelectList;

            // Find the highest Class_Enrolment_id and increment by 1
            var highestEnrolmentId = db.Class_Enrolment.Max(c => (int?)c.Class_Enrolment_id) ?? 0;
            var nextEnrolmentId = highestEnrolmentId + 1;

            // Create a new instance of Class_Enrolment model to pass to the view
            var model = new Class_Enrolment
            {
                Class_Enrolment_id = nextEnrolmentId
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CourseSelection(Class_Enrolment c)
        {
            if (ModelState.IsValid)
            {
                // Add the provided Class_Enrolment object 'c' to the database context
                db.Class_Enrolment.Add(c);
                db.SaveChanges();

                // Redirect to a different action or view
                return RedirectToAction("AdminMain", "Admin"); // Example: Redirect to the home page
            }
            // If the model state is not valid, return the same view with validation errors
            return View(c);
        }



    }
}