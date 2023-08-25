using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcProject.BusinessLayer;
using MvcProject.BusinessLayer.Results;
using MvcProject.Entities;
using MvcProject.WebApp.Filters;

namespace MvcProject.WebApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class MvcProjectUserController : Controller
    {
        // GET: MvcProjectUser
        private MvcProjectUserManager mvcprojectUserManager = new MvcProjectUserManager();
        public ActionResult Index()
        {
            return View(mvcprojectUserManager.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MvcProjectUser mvcprojectUser = mvcprojectUserManager.Find(x => x.Id == id.Value);
            if (mvcprojectUser == null)
            {
                return HttpNotFound();
            }
            return View(mvcprojectUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MvcProjectUser mvcprojectUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<MvcProjectUser> res = mvcprojectUserManager.Insert(mvcprojectUser);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(mvcprojectUser);
                }
                return RedirectToAction("Index");
            }
            return View(mvcprojectUser);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MvcProjectUser mvcprojectUser = mvcprojectUserManager.Find(x => x.Id == id.Value);
            if (mvcprojectUser == null)
            {
                return HttpNotFound();
            }
            return View(mvcprojectUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MvcProjectUser mvcprojectUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<MvcProjectUser> res = mvcprojectUserManager.Update(mvcprojectUser);
                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(mvcprojectUser);
                }
                return RedirectToAction("Index");
            }
            return View(mvcprojectUser);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MvcProjectUser mvcprojectUser = mvcprojectUserManager.Find(x => x.Id == id.Value);
            if (mvcprojectUser == null)
            {
                return HttpNotFound();
            }
            return View(mvcprojectUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MvcProjectUser mvcprojectUser = mvcprojectUserManager.Find(x => x.Id == id);
            mvcprojectUserManager.Delete(mvcprojectUser);
            return RedirectToAction("Index");
        }
    }
}
