using FinalProject1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinalProject1.Controllers
{
    public class HomeController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Index(User Log)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == Log.Email && x.password == Log.password);

            if (user != null)
            {
                if (user.user_type == "S")
                {
                    // Retrieve the student ID from the Student table using the email
                    var student = db.Students.FirstOrDefault(s => s.Student_Email == Log.Email);

                    if (student != null)
                    {
                        // Redirect to StudentMain action passing the Student ID as a parameter
                        return RedirectToAction("StudentMain", "Student", new { id = student.Student_ID });
                    }
                    else
                    {
                        // Handle the case where student is not found
                        TempData["ErrorMessage"] = "Student record not found.";
                        return View();
                    }
                }
                else if (user.user_type == "T")
                {
                    var teacher = db.Facutlies.FirstOrDefault(t => t.Teacher_Email == Log.Email);

                    if (teacher != null)
                    {
                        return RedirectToAction("FacultyMain", "Faculty", new { id = teacher.Teacher_ID });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Teacher record not found.";
                        return View();
                    }
                }
                else if (user.user_type == "A")
                {
                    var teacher = db.Admins.FirstOrDefault(a => a.Admin_Email == Log.Email);

                    if (teacher != null)
                    {
                        return RedirectToAction("AdminMain", "Admin", new { id = teacher.Admin_ID });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Admin record not found.";
                        return View();
                    }
                }
            }

            TempData["ErrorMessage"] = "Incorrect email or password. Please try again.";
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(user);
                }

                // Find the highest existing ID in the Authorization table
                long highestId = db.Users.Max(u => (long?)u.Acc_ID) ?? 0;

                // Increment the ID for the new user
                user.Acc_ID = highestId + 1;

                db.Users.Add(user);
                db.SaveChanges();

                // Display success message
                TempData["SuccessMessage"] = "User added successfully.";
                return View(user);
            }

            return View(user);
        }

        public ActionResult ViewUser()
        {
            var User = db.Users.ToList(); // Retrieve all students from the database
            return View(User);
        }

        public ActionResult EditUser(int id)
        {
            var User = db.Users.FirstOrDefault(s => s.Acc_ID == id);

            if (User == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "User record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User editedUser)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingUser = db.Users.FirstOrDefault(s => s.Acc_ID == editedUser.Acc_ID);

                if (existingUser != null)
                {
                    // Update student information with edited values
                    existingUser.Email = editedUser.Email;
                    existingUser.password = editedUser.password;
                    existingUser.user_type = editedUser.user_type;


                    // Save changes to the database
                    db.SaveChanges();

                    // Display success message
                    TempData["SuccessMessage"] = "Student information updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Student record not found.";
                }
            }
            else
            {
                // If ModelState is not valid, return the view with the invalid model
                return View(editedUser);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("Index","Home" ,new { id = editedUser.Acc_ID });
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound(); // or handle appropriately if user is not found
            }

            if (user.user_type == "S")
            {
                // If the user is a student, delete all records associated with the student
                var student = db.Students.FirstOrDefault(s => s.Student_Email == user.Email);
                if (student != null)
                {
                    // Delete all class enrollments associated with the student
                    var enrollments = db.Class_Enrolment.Where(e => e.Student_ID == student.Student_ID);
                    db.Class_Enrolment.RemoveRange(enrollments);
                    // Now delete the student record
                    db.Students.Remove(student);
                }
            }
            else if (user.user_type == "T" || user.user_type == "A")
            {
                // If the user is a faculty or admin, delete their record directly
                if (user.user_type == "T")
                {
                    // Delete faculty record
                    var faculty = db.Facutlies.FirstOrDefault(f => f.Teacher_Email == user.Email);
                    if (faculty != null)
                    {
                        // Find all classes associated with the faculty
                        var classesToDelete = db.Classes.Where(c => c.Teacher_ID == faculty.Teacher_ID);

                        // Delete each class
                        foreach (var cls in classesToDelete)
                        {
                            db.Classes.Remove(cls);
                        }

                        // Delete the faculty record
                        db.Facutlies.Remove(faculty);
                    }
                }
                else if (user.user_type == "A")
                {
                    // Delete admin record
                    var admin = db.Admins.FirstOrDefault(a => a.Admin_Email == user.Email);
                    if (admin != null)
                    {
                        db.Admins.Remove(admin);
                    }
                }
            }

            // Now delete the user record
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("ViewUser"); // Redirect back to the same page
        }

    }
}