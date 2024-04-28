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
    public class StudentController : Controller
    {
        FINALPROJECTEntities1 db = new FINALPROJECTEntities1();
        // GET: Student
        [HttpGet]
        public ActionResult AddStudent()
        {
            var newStudent = new Student();

            // Get the maximum Student_ID from the database
            var maxStudentId = db.Students.Max(u => (int?)u.Student_ID) ?? 0;

            // Set the Student_ID for the new student as the maximum ID + 1
            newStudent.Student_ID = maxStudentId + 1;

            return View(newStudent);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(Student s1, HttpPostedFileBase imgfile)

        {
            if (ModelState.IsValid)
            {

                // Check if the user already exists
                var existingUser = db.Students.FirstOrDefault(u => u.Student_Email == s1.Student_Email);

                if (existingUser != null)
                {
                    // Display error message if user already exists
                    TempData["ErrorMessage"] = "User with this email already exists.";
                    return View(s1);
                }

                // Get the maximum Student_ID from the database
                var maxStudentId = db.Students.Max(u => (int?)u.Student_ID) ?? 0;

                // Increment the maximum Student_ID by 1
                s1.Student_ID = maxStudentId + 1;

                Student sl = new Student();
                string path = UploadImage(imgfile);
                if (path.Equals("-1"))
                {

                }
                else
                {
                    try
                    {
                        sl.Student_ID = s1.Student_ID;
                        sl.Student_Name = s1.Student_Name;
                        sl.Student_Email = s1.Student_Email;
                        sl.Student_Address = s1.Student_Address;
                        sl.Student_Phone = s1.Student_Phone;
                        sl.Student_Image = path;

                        db.Students.Add(sl);
                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Student added successfully.";
                        return RedirectToAction("AddStudent", "Student"); // Redirect to a different action
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

        public ActionResult ViewStudents()
        {
            var students = db.Students.ToList(); // Retrieve all students from the database
            return View(students);
        }


        public ActionResult EditStudent(int id)
        {
            var student = db.Students.FirstOrDefault(s => s.Student_ID == id);

            if (student == null)
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Student record not found.";
                return RedirectToAction("Index");
            }

            // Pass the student model to the view for editing
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(Student editedStudent)
        {
            if (ModelState.IsValid)
            {
                // Update student information in the database
                var existingStudent = db.Students.FirstOrDefault(s => s.Student_ID == editedStudent.Student_ID);

                if (existingStudent != null)
                {
                    // Update student information with edited values
                    existingStudent.Student_Name = editedStudent.Student_Name;
                    existingStudent.Student_Email = editedStudent.Student_Email;
                    existingStudent.Student_Address = editedStudent.Student_Address;
                    existingStudent.Student_Phone = editedStudent.Student_Phone;


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
                return View(editedStudent);
            }

            // Redirect back to the StudentMain page
            return RedirectToAction("StudentMain", new { id = editedStudent.Student_ID });
        }

        [HttpPost]
        public ActionResult DeleteStudent(int id)
        {
            var student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound(); // or handle appropriately if student is not found
            }

            // Delete all class enrollments associated with the student
            var enrollments = db.Class_Enrolment.Where(e => e.Student_ID == id);
            db.Class_Enrolment.RemoveRange(enrollments);

            // Now delete the student record
            db.Students.Remove(student);
            db.SaveChanges();

            return RedirectToAction("ViewStudents"); // Redirect back to the same page
        }

        public ActionResult StudentMain(int id)
        {
            // Retrieve the student record using the provided ID
            var student = db.Students.FirstOrDefault(s => s.Student_ID == id);

            if (student != null)
            {
                // Pass the student model to the view
                return View(student);
            }
            else
            {
                // Handle the case where student is not found
                TempData["ErrorMessage"] = "Student record not found.";
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