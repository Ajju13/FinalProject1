using FinalProject1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProject1.Controllers
{
    public class AdminController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();

        public ActionResult AddAdmin()
        {
            var newAdmin = new Admin();

            // Get the maximum Student_ID from the database
            var maxAdminId = db.Admins.Max(u => (int?)u.Admin_ID) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newAdmin.Admin_ID = maxAdminId + 1;

            return View(newAdmin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(Admin s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Admins.FirstOrDefault(u => u.Admin_Email == s1.Admin_Email);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "Admin with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxAdminId = db.Admins.Max(u => (int?)u.Admin_ID) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.Admin_ID = maxAdminId + 1;

                Admin sl = new Admin();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.Admin_ID = s1.Admin_ID;
                        sl.Admin_Name = s1.Admin_Name;
                        sl.Admin_Email = s1.Admin_Email;
                        sl.Admin_Address = s1.Admin_Address;
                        sl.Admin_Phone = s1.Admin_Phone;
                        sl.Admin_Image = path;

                        db.Admins.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Admin added successfully.";
                        return RedirectToAction("AddAdmin", "Admin"); // Redirect to a different action
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Handle validation errors
                        foreach (var entityValidationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in entityValidationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                    }
                }


            }

            // If ModelState is not valid, return the view with the invalid model
            return View(s1);
        }

        public ActionResult ViewAdmin()
        {
            var Admin = db.Admins.ToList(); // Retrieve all students from the database
            return View(Admin);
        }


        public ActionResult EditAdmin(int id)
        {
            var Admin = db.Admins.FirstOrDefault(s => s.Admin_ID == id);

            if (Admin == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Admin record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(Admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin(Admin editedAdmin)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingAdmin = db.Admins.FirstOrDefault(s => s.Admin_ID == editedAdmin.Admin_ID);

                if (existingAdmin != null)
                {
                    // Update student information with edited values
                    existingAdmin.Admin_Name = editedAdmin.Admin_Name;
                    existingAdmin.Admin_Email = editedAdmin.Admin_Email;
                    existingAdmin.Admin_Address = editedAdmin.Admin_Address;
                    existingAdmin.Admin_Phone = editedAdmin.Admin_Phone;


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
                return View(editedAdmin);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("AdminMain", new { id = editedAdmin.Admin_ID });
        }

        [HttpPost]
        public ActionResult DeleteAdmin(int id)
        {
            var Admin = db.Admins.Find(id);
            if (Admin == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }


            // Now delete the student record
            db.Admins.Remove(Admin);
            db.SaveChanges();

            return RedirectToAction("ViewAdmin"); // Redirect back to the same page
        }

        public ActionResult AdminMain(int id)
        {
            // Retrieve the student record using the provided ID
            var Admin = db.Admins.FirstOrDefault(s => s.Admin_ID == id);
            int totalStudents = db.Students.Count();
            int totalUsers = db.Users.Count();
            int totalClasses = db.Classes.Count();

            // Pass the counts to the view using ViewBag
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalClasses = totalClasses;

            if (Admin != null)
            {
                // Pass the student model to the view
                return View(Admin);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Admin record not found.";
                return RedirectToAction("Index");
            }
        }


        public string UploadImage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);

                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                        // Log the exception or handle it appropriately
                    }
                }
                else
                {

                    Response.Write("<script>alert('Only jpg, jpeg, or png formats are acceptable....');</script>");
                }
            }
            else
            {

                Response.Write("<script>alert('Please select a file');</script>");
            }

            return path;
        }

    }
}