/*
 Admin controller class
 Author: Evgeny Chudaev
 Purpose: Implements controller class responsible for handling requests and business logic for the admins area
 Admins are is where analysts, manager and superuser roles are set up. Each role has different privileges.
 Admin area will be accessible by only 1 individual who can set up other superusers, who in turn will be able to
 set up analysts/managers.
 General users who want to simply submit a request do not need to be added to the admin list
 */

using LoginTest.Data;
using LoginTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace LoginTest.Controllers
{
    public class AdminsController : Controller
    {

        private readonly CrudContext _contextAdmin;

        // Controller constructor
        public AdminsController(CrudContext context)
        {
            _contextAdmin = context;
        }

        // Index method - responsible for displaying a list of admins
        public IActionResult Index()
        {
            return View(_contextAdmin.Admins.ToList());
        }

        //Method responsible for rounting to ticket edit page
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Admins admin = _contextAdmin.Admins.SingleOrDefault(x => x.ID == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        //Method responsible editing of admin information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Admins admin)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Admins oAdmin = new Admins { ID = admin.ID, login = admin.login, name = admin.name, role = admin.role };               
                _contextAdmin.Entry(oAdmin).Property("login").IsModified = true;
                _contextAdmin.Entry(oAdmin).Property("name").IsModified = true;
                _contextAdmin.Entry(oAdmin).Property("role").IsModified = true;      

                _contextAdmin.SaveChanges();
                TempData["message"] = "Admin edited!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "There have been errors");
            return View(admin);
        }

        //Method responsible for deletion of the admin
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Admins admin = _contextAdmin.Admins.SingleOrDefault(x => x.ID == id);

            if (admin == null)
            {
                return NotFound();
            }

            _contextAdmin.Remove(admin);
            _contextAdmin.SaveChanges();

            TempData["message"] = "Person Deleted!";
            return RedirectToAction("Index");

        }

        //Method responsible for creating new admin
        public IActionResult Create(int? id)
        {
            return View();
        }

        //Method responsible for creating new admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Admins admin)
        {
            if (ModelState.IsValid)
            {
                admin.CreatedDate = System.DateTime.Now;
                _contextAdmin.Add(admin);
                _contextAdmin.SaveChanges();

                TempData["message"] = "Admin Created!";

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "There has been errors");
            return View(admin);

        }



    }
}