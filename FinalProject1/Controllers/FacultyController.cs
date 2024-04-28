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
    public class FacultyController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();
        [HttpGet]
        public ActionResult AddFaculty()
        {
            var newFaculty = new Facutly();

            // Get the maximum Student_ID from the database
            var maxFacultyId = db.Facutlies.Max(u => (int?)u.Teacher_ID) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newFaculty.Teacher_ID = maxFacultyId + 1;

            return View(newFaculty);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFaculty(Facutly s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Facutlies.FirstOrDefault(u => u.Teacher_Email == s1.Teacher_Email);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxFacultyId = db.Facutlies.Max(u => (int?)u.Teacher_ID) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.Teacher_ID = maxFacultyId + 1;

                Facutly sl = new Facutly();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.Teacher_ID = s1.Teacher_ID;
                        sl.Teacher_Name = s1.Teacher_Name;
                        sl.Teacher_Email = s1.Teacher_Email;
                        sl.Teacher_Address = s1.Teacher_Address;
                        sl.Teacher_Phone = s1.Teacher_Phone;
                        sl.Teacher_Image = path;

                        db.Facutlies.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Faculty added successfully.";
                        return RedirectToAction("AddFaculty", "Home"); // Redirect to a different action
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


        public ActionResult ViewFaculty()
        {
            var Faculty = db.Facutlies.ToList(); // Retrieve all students from the database
            return View(Faculty);
        }

        public ActionResult EditFaculty(int id)
        {
            var student = db.Facutlies.FirstOrDefault(s => s.Teacher_ID == id);

            if (student == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Faculty record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Facutly editedFaculty)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingFaculty = db.Facutlies.FirstOrDefault(s => s.Teacher_ID == editedFaculty.Teacher_ID);

                if (existingFaculty != null)
                {
                    // Update student information with edited values
                    existingFaculty.Teacher_Name = editedFaculty.Teacher_Name;
                    existingFaculty.Teacher_Email = editedFaculty.Teacher_Email;
                    existingFaculty.Teacher_Address = editedFaculty.Teacher_Address;
                    existingFaculty.Teacher_Phone = editedFaculty.Teacher_Phone;


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
                return View(editedFaculty);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("StudentMain", new { id = editedFaculty.Teacher_ID });
        }

        [HttpPost]
        public ActionResult DeleteFaculty(int id)
        {
            var student = db.Facutlies.Find(id);
            if (student == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }

            // Now delete the student record
            db.Facutlies.Remove(student);
            db.SaveChanges();

            return RedirectToAction("ViewFaculty"); // Redirect back to the same page
        }

        public ActionResult FacultyMain(int id)
        {
            // Retrieve the student record using the provided ID
            var Faculty = db.Facutlies.FirstOrDefault(s => s.Teacher_ID == id);

            if (Faculty != null)
            {
                // Pass the student model to the view
                return View(Faculty);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Faculty record not found.";
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