using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Data;
using SakilaApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace SakilaApp.Controllers
{
    // Actors debe poder verse con cualquier usuario autenticado.
    // Antes el controlador solo aceptaba roles concretos; si un usuario iniciaba sesion sin esos roles,
    // no podia ingresar a /Actors aunque solo quisiera consultar la lista.
    [Authorize]
    public class ActorsController : Controller
    {
        private readonly SakilaContext _context;

        public ActorsController(SakilaContext context)
        {
            _context = context;
        }

        // Muestra /Actors con paginacion para no cargar todos los actores en una sola pantalla.
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.Actors
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ThenBy(a => a.ActorId);

            return View(await this.PaginateAsync(query, page));
        }
        
        //Mostrar 5 actores siguientes depsues del primero que su nombre empiecen con la letra n o 
        // terminen con la letra n ordenados alfabeticmanete por apellidos
        /*public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors
                .Skip(1)
                .Take(5)
                .Where(f => f.FirstName.StartsWith("n") || f.LastName.EndsWith("n"))
                .OrderBy(f => f.LastName)
                .ToListAsync();
            return View(actors);
        }*/

// =====================================================================
        // EJERCICIOS LINQ CON ENTITY FRAMEWORK CORE – SAKILA
        // CUARTIL II – NIVEL INTERMEDIO (Ejercicios 11 al 14)
        // =====================================================================
        /*Ejercicio 11: Mostrar los 5 actores siguientes después del primero, ordenados por apellido.
        */public async Task<IActionResult> Ejercicio11()
            {
                var actores = await _context.Actors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .ThenBy(a => a.ActorId)
                    .Skip(1)
                    .Take(5)
                    .ToListAsync();
            
                return View("Index", actores);
            }
            /*

        Ejercicio 12: Mostrar los 5 actores cuyos nombres empiecen con N o terminen con N.
        public async Task<IActionResult> Index()
        {
            var actores = await _context.Actors
                .Where(a => a.FirstName.ToUpper().StartsWith("N") || a.FirstName.ToUpper().EndsWith("N"))
                .Take(5)
                .ToListAsync();
            return View(actores);
        }
        /*
        Ejercicio 13: Mostrar los 10 actores cuyos apellidos empiecen con la letra S.
        public async Task<IActionResult> Index()
        {
            var actores = await _context.Actors
                .Where(a => a.LastName.ToUpper().StartsWith("S"))
                .Take(10)
                .ToListAsync();
            return View(actores);
        }

        Ejercicio 14: Mostrar los 5 actores cuyos nombres contengan la cadena JO.
        public async Task<IActionResult> Index()
        {
            var actores = await _context.Actors
                .Where(a => a.FirstName.ToUpper().Contains("JO"))
                .Take(5)
                .ToListAsync();
            return View(actores);
        }*/

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.ActorId == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        [Authorize(Roles = "Administrador,Operador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorId,FirstName,LastName,LastUpdate")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActorId,FirstName,LastName,LastUpdate")] Actor actor)
        {
            if (id != actor.ActorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.ActorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.ActorId == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.ActorId == id);
        }
    }
}
