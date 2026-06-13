using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SakilaApp.Data;
using SakilaApp.Models;


namespace SakilaApp.Controllers


{

    // CORREGIDO: la pagina principal apunta a Films/Index.
    // Con Authorize, si no hay sesion iniciada, ASP.NET redirige al Login.
    [Authorize]
    public class FilmsController : Controller
    {
        private readonly SakilaContext _context;

        public FilmsController(SakilaContext context)
        {
            _context = context;
        }

        // GET: Films
        //MOSTRAR PELICULAS CON DURACION ENTRE 90 Y 120 MINUTOS, ORDENADAS POR DURACION
        /*
        public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films.Include(f => f.Language).Include(f => f.OriginalLanguage);
            var films = await sakilaContext
            .Where(f => f.Length >= 90 && f.Length <= 120)
            .OrderBy(f => f.Length)
            .ToListAsync();
            return View(films);
        }*/

        //MOSTRAR PELICULAS CON DURACION DE AL MENOS 6 DIAS, ORDENADAS POR DURACION DE MANERA DESCENDENTE
        /*public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage);

            var films = await sakilaContext
                .Where(f => f.RentalDuration >= 6)
                .OrderByDescending(f => f.RentalDuration)
                .ToListAsync();

            return View(films);
        }*/

        //PELICULA QUE SEA DE DRAMA Y QUE SU DECRIPCION TENGA LA PALABRA DRAMA
        /*public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films
                .Include(f => f.Language)//diferencia de include y join es que include no hace un join solo busca en la base de datos
                .Include(f => f.OriginalLanguage);

            var films = await sakilaContext
                // ANTES: .Where(f => f.Description.Contains("Drama"))
                // CORREGIDO: valida que Description no sea null y busca drama sin importar mayusculas.
                .Where(f => f.Description != null && EF.Functions.ILike(f.Description, "%drama%"))
                .ToListAsync();
        }*/

        //EJERCICIOS PROPUESTOS------------------------------------------------------------------------------------------
        /*1. Mostrar todas las películas ordenadas alfabéticamente por título
           public async Task<IActionResult> Index()
        {
            var films = await _context.Films
            .Include(f => f.Language)
            .Include(f => f.OriginalLanguage)
           .OrderBy(f => f.Title)
            .ToListAsync();
            return View(films);
        }*/

        /*2. Mostrar las 10 primeras películas registradas
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Take(10)
                .ToListAsync();
            return View(films);
        }*/

        /*3. Mostrar las 20 películas más largas
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .OrderByDescending(f => f.Length)
                .Take(20)
                .ToListAsync();
            return View(films);
        }*/

        /*4. Mostrar las películas cuya duración sea mayor a 120 minutos
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Length > 120)
                .ToListAsync();
            return View(films);
        }*/

        /*5. Mostrar las películas cuya duración esté entre 90 y 120 minutos
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Length >= 90 && f.Length <= 120)
                .ToListAsync();
            return View(films);
        }*/

        /*6. Mostrar las películas cuyo título contenga la palabra "LOVE"
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Title.Contains("LOVE"))
                .ToListAsync();
            return View(films);
        }*/

        /*7. Mostrar las películas cuyo título comience con la letra "A"
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Title.StartsWith("A"))
                .ToListAsync();
            return View(films);
        }*/

        /*8. Mostrar las películas cuyo título termine con la letra "N"
        public async Task<ActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Title.EndsWith("N"))
                .ToListAsync();
            return View(films);
        }*/

        //9. Mostrar las películas con clasificación PG -> no se puede hacer con linq
        
       
        //10. Mostrar las películas con clasificación PG-13 o R -> no se puede hacer con linq

        //EJERCICIOS INTERMEIOS--------------------------------------------------------------------------------------------
        /*1. Mostrar las películas de la categoría Drama
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                // ANTES: .Where(f => f.Description.Contains("Drama"))
                // CORREGIDO: valida que Description no sea null y busca drama sin importar mayusculas.
                .Where(f => f.Description != null && EF.Functions.ILike(f.Description, "%drama%"))
                .ToListAsync();
            return View(films);
        }*/

        /*2. Mostrar las películas de la categoría Comedy
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Description != null && EF.Functions.ILike(f.Description, "%comedy%"))
                .ToListAsync();
            return View(films);
        }*/

        //3. Mostrar las películas que pertenezcan a Drama y duren más de 100 minutos
        //4. Mostrar las películas de Action ordenadas desde la más larga hasta la más corta
        //5. Mostrar las primeras 5 películas de Horror ordenadas por título
        //6. Contar cuántas películas existen en cada categoría
        //7. Mostrar cuántas películas tienen clasificación PG
        //8. Mostrar la cantidad total de películas registradas

        //9. Mostrar las películas cuyo costo de reemplazo sea mayor a 20 dólares
        /*9. Mostrar las películas cuyo costo de reemplazo sea mayor a 25.5 dólares
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.ReplacementCost > 25.5m)
                .ToListAsync();
            return View(films);
        }*/

        //10. Mostrar las películas cuya duración sea mayor a 100 minutos y cuyo costo de reemplazo sea menor a 20 dólares

        //RETOS------------------------------------------------------------------------------------------------------------
        //1. Mostrar los 10 actores con más películas registradas
        //2. Mostrar todas las películas donde participe un actor específico
        //3. Mostrar los clientes cuyo apellido contenga una cadena determinada
        //4. Mostrar las ciudades que pertenezcan a un país específico
        //5. Mostrar los clientes que hayan realizado más de 20 alquileres
        //6. Mostrar las películas nunca alquiladas
        //7. Mostrar las 10 películas más alquiladas
        //8. Mostrar el promedio de duración de las películas por categoría
        //9. Mostrar la categoría con mayor cantidad de películas
        //10. Mostrar el actor que aparece en más películas

        //>100minutos tarifa alquiler >= 3.5
        /*public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Length > 100 && f.RentalRate >= 3.5m)
                .ToListAsync();
            return View(films);
        }*/

        /*Titulo = love o tarifa alquiler 4.99 y ademas duracion >100minutos ordenado por tarifa
        public async Task<IActionResult> Index()
        {
            var films = await _context.Films
                .Where(f => (f.Title.Contains("love") || f.RentalRate == 4.99m) && f.Length > 100)
                .OrderBy(f => f.RentalRate)
                .ToListAsync();
            return View(films);
        }*/

        
        /*public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films//context es la llave a la base de datos
                .Join(_context.Languages,
                    film => film.LanguageId,
                    language => language.LanguageId,
                    (film, language) => film)
                .OrderBy(f => f.Title)
                .ToListAsync();

            return View(peliculas);
        }*/

        //con include
        /*public async Task<IActionResult> Index()
        {
            var  peliculas = await _context.Films 
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .OrderBy(f => f.Title)
                .ToListAsync();
            return View(peliculas);
        }*/

        //Mas estricto
        /*public async Task<IActionResult> Index()
        {
            var peliculasConIdioma = await _context.Films
                .Join(_context.Languages,
                    film => film.LanguageId,
                    language => language.LanguageId,
                    (film, language) => new { film, language })
                .GroupJoin(_context.Languages,
                    x => x.film.OriginalLanguageId,
                    originalLanguage => (int?)originalLanguage.LanguageId,
                    (x, originalLanguages) => new { x.film, x.language, originalLanguages })
                .SelectMany(
                    x => x.originalLanguages.DefaultIfEmpty(),
                    (x, originalLanguage) => new { x.film, x.language, originalLanguage })
                // Language.Name viene de language.name, que en Sakila esta definido como CHAR(20).
                // PostgreSQL rellena CHAR(20) con espacios a la derecha, por ejemplo "English   ".
                // Trim() normaliza el valor antes de compararlo.
                .Where(x => x.language.Name.Trim() == "English")
                .OrderBy(x => x.film.Title)
                .ToListAsync();
            // El Join trae Language, pero al devolver solo Film la navegacion no queda cargada para la vista.
            // Por eso se asignan los idiomas obtenidos por los joins antes de enviar el modelo.
            foreach (var item in peliculasConIdioma)
            {
                item.film.Language = item.language;
                item.film.OriginalLanguage = item.originalLanguage;
            }
            var peliculas = peliculasConIdioma.Select(x => x.film).ToList();
            return View(peliculas);
        }*/

        /*public async Task<IActionResult> Index()
        {
            var peliculasConIdioma = await _context.Films
                .Join(_context.FilmCategories,
                    film => film.FilmId,
                    filmCategory => filmCategory.FilmId,
                    (film, filmCategory) => new { film, filmCategory })
                .Join(_context.Categories,
                    temp => temp.filmCategory.CategoryId,
                    category => category.CategoryId,
                    (temp, category) => new { temp.film, category })
                .Where(x => x.category.Name == "Drama")
                .OrderByDescending(x => x.film.Length)
                .Take(10)
                .Select(x => x.film)
                .ToListAsync();
             foreach (var item in peliculasConIdioma)
            {
                item.Language = await _context.Languages.FindAsync(item.LanguageId);
                item.OriginalLanguage = await _context.Languages.FindAsync(item.OriginalLanguageId);
            }
            var peliculas = peliculasConIdioma;
            return View(peliculas);
        }*/

        /*tres joins
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Join(_context.Languages,
                    film => film.LanguageId,
                    language => language.LanguageId,
                    (film, language) => new { film, language })
                .Join(_context.FilmCategories,
                    temp => temp.film.FilmId,
                    filmCategory => filmCategory.FilmId,
                    (temp, filmCategory) => new { temp.film, temp.language, filmCategory })
                .Join(_context.Categories,
                    temp => temp.filmCategory.CategoryId,
                    category => category.CategoryId,
                    (temp, category) => new { temp.film, temp.language, category })
                .Where(x => x.language.Name == "English" && x.category.Name == "Comedy")
                .OrderBy(x => x.film.Title)
                .Select(x => x.film)
                .ToListAsync();
                
            return View(peliculas);
        }*/

        //Mostrar las películas cuyo idioma sea English y cuyo título empiece con la letra A, ordenadas alfabéticamente por título con join
       /* public async Task<IActionResult> Index()
        {
             var peliculasConIdioma = await _context.Films
                .Join(_context.Languages,
                    film => film.LanguageId,
                    language => language.LanguageId,
                    (film, language) => new { film, language })
                .Where(x => x.language.Name == "English" && x.film.Title.StartsWith("A"))
                .OrderBy(x => x.film.Title)
                .Take(5)
                .Select(x => x.film)
                .ToListAsync();
                foreach (var item in peliculasConIdioma)
            {
                item.Language = await _context.Languages.FindAsync(item.LanguageId);
                item.OriginalLanguage = await _context.Languages.FindAsync(item.OriginalLanguageId);
            }
            var peliculas = peliculasConIdioma;
            return View(peliculas);
        }*/

        //Mostrar las primeras 5 peliculas y el precio ordenado de mayor a menor cuyo idioma sea English y cuyo título empiece con la letra A, ordenadas alfabéticamente por título con join
        /*public async Task<IActionResult> Index()
        {
            var peliculasConIdioma = await _context.Films
                .Join(_context.Languages,
                    film => film.LanguageId,
                    language => language.LanguageId,
                    (film, language) => new { film, language })
                .Where(x => x.language.Name == "English" && x.film.Title.StartsWith("A"))
                .OrderByDescending(x => x.film.RentalRate)
                .OrderBy(x => x.film.Title)
                .Take(5)
                .Select(x => x.film)
                .ToListAsync();
            foreach (var item in peliculasConIdioma)
            {
                item.Language = await _context.Languages.FindAsync(item.LanguageId);
                item.OriginalLanguage = await _context.Languages.FindAsync(item.OriginalLanguageId);
            }
            var peliculas = peliculasConIdioma;
            return View(peliculas);
        }*/
       /* public async Task<IActionResult> Index(string? buscar, int? duracionMinima)
        {
            var consulta = _context.Films.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(f => f.Title.Contains(buscar));
            }

            if (duracionMinima.HasValue)
            {
                consulta = consulta.Where(f => f.Length >= duracionMinima.Value);
            }

            var peliculas = await consulta
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .OrderBy(f => f.Title)
                .ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.DuracionMinima = duracionMinima;

            return View(peliculas);
        }*/

        public async Task<IActionResult> Index(
            string? buscar,
            int? duracionMinima,
            int pagina = 1)
        {
            int tamanioPagina = 10;

            var consulta = _context.Films.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                consulta = consulta.Where(f => f.Title.Contains(buscar));
            }

            if (duracionMinima.HasValue)
            {
                consulta = consulta.Where(f => f.Length >= duracionMinima.Value);
            }

            int totalRegistros = await consulta.CountAsync();

            var peliculas = await consulta
                .OrderBy(f => f.Title)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.DuracionMinima = duracionMinima;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanioPagina);

            return View(peliculas);
        }

        

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["LanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId");
            ViewData["OriginalLanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmId,Title,Description,ReleaseYear,LanguageId,OriginalLanguageId,RentalDuration,RentalRate,Length,ReplacementCost,LastUpdate,SpecialFeatures,Fulltext")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.OriginalLanguageId);
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["LanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.OriginalLanguageId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmId,Title,Description,ReleaseYear,LanguageId,OriginalLanguageId,RentalDuration,RentalRate,Length,ReplacementCost,LastUpdate,SpecialFeatures,Fulltext")] Film film)
        {
            if (id != film.FilmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmId))
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
            ViewData["LanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Languages, "LanguageId", "LanguageId", film.OriginalLanguageId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .FirstOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                _context.Films.Remove(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.FilmId == id);
        }
    }
}
