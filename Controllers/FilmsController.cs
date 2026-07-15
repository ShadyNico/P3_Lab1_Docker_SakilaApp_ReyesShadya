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
        
        public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films.Include(f => f.Language).Include(f => f.OriginalLanguage);
            return View(await sakilaContext.ToListAsync());
        }
        

        //Films/HorrorActoresS
        /*
        public async Task<IActionResult> HorrorActoresS()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)t
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Horror") &&
                            f.FilmActors.Any(fa => fa.Actor.LastName.ToUpper().EndsWith("S")))
                .Take(10)
                .ToListAsync();

            return View("Index", peliculas);
        }*/

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
        // =====================================================================
        // EJERCICIOS LINQ CON ENTITY FRAMEWORK CORE – SAKILA
        // CUARTIL I – NIVEL BÁSICO (Ejercicios 1 al 10)
        // =====================================================================
        /*Ejercicio 1: Mostrar las 10 primeras películas ordenadas alfabéticamente por título.
        public async Task<IActionResult> Ejercicio1()
            {
                
                var peliculas = await _context.Films
                    .Include(f => f.Language)
                    .Include(f => f.OriginalLanguage)
                    .OrderBy(f => f.Title)
                    .Take(10)
                    .ToListAsync();
            
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            
                return View("Index", peliculas);
            }

        Ejercicio 2: Mostrar las 5 películas más largas registradas.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .OrderByDescending(f => f.Length)
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 3: Mostrar las 10 películas cuyo título contenga la palabra LOVE.
        public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films.Include(f => f.Language).Include(f => f.OriginalLanguage);
            var peliculas = await _context.Films
                .Where(f => f.Title.ToUpper().Contains("LOVE"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 4: Mostrar las 10 películas cuyo título empiece con la letra A.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.Title.ToUpper().StartsWith("A"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 5: Mostrar las 10 películas cuyo título termine con la letra N.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.Title.ToUpper().EndsWith("N"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 6: Mostrar las 10 películas cuya duración sea mayor a 120 minutos.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.Length > 120)
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 7: Mostrar las 10 películas cuyo costo de reemplazo sea menor a 20 dólares.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.ReplacementCost < 20m)
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 8: Mostrar las 10 películas cuya duración sea mayor a 100 minutos y cuyo costo de reemplazo sea menor a 20 dólares.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.Length > 100 && f.ReplacementCost < 20m)
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }
        
        Ejercicio 9: Mostrar las 10 películas cuyo título contenga LOVE o cuya tarifa de alquiler sea 4.99.
        public async Task<IActionResult> Ejercicio9()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.Title.ToUpper().Contains("LOVE") ||
                            f.RentalRate == 4.99m)
                .OrderBy(f => f.FilmId)
                .Take(10)
                .ToListAsync();
        
            ViewBag.PaginaActual = 1;
            ViewBag.TotalPaginas = 1;
        
            return View("Index", peliculas);
        }
        
        //Ejercicio 10: Mostrar las 10 películas cuyo título empiece con A o termine con N.
        /*public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films.Include(f => f.Language).Include(f => f.OriginalLanguage);
            var peliculas = await _context.Films
                .Where(f => f.Title.ToUpper().StartsWith("A") || f.Title.ToUpper().EndsWith("N"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }
        /*
        // =====================================================================
        // CUARTIL II – NIVEL INTERMEDIO (Ejercicios 15 al 20)
        // =====================================================================

        Ejercicio 15: Mostrar las 10 películas junto con el nombre de su idioma.
        public async Task<IActionResult> Ejercicio15()
            {
                var peliculas = await _context.Films
                    .Include(f => f.Language)
                    .Include(f => f.OriginalLanguage)
                    .OrderBy(f => f.FilmId)
                    .Take(10)
                    .ToListAsync();
            
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            
                return View("Index", peliculas);
            }

        Ejercicio 16: Mostrar las 10 primeras películas cuyo idioma sea diferente a English.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Language.Name != "English")
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 17: Mostrar las 10 primeras películas cuyo idioma sea diferente de English y cuyo título empiece con A.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Language.Name != "English" && f.Title.ToUpper().StartsWith("A"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 18: Mostrar las 5 películas cuyo idioma sea diferente a English o cuyo título contenga LOVE.
        public async Task<IActionResult> Index()
        {
            var sakilaContext = _context.Films.Include(f => f.Language).Include(f => f.OriginalLanguage);
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Language.Name != "English" || f.Title.ToUpper().Contains("LOVE"))
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 19: Mostrar las 5 películas más largas cuyo idioma sea diferente a English.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Language.Name != "English")
                .OrderByDescending(f => f.Length)
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 20: Mostrar las 10 películas cuyo idioma sea English, ordenadas por duración descendente y omitiendo la primera.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Language.Name == "English")
                .OrderByDescending(f => f.Length)
                .Skip(1)
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        // =====================================================================
        // CUARTIL III – NIVEL AVANZADO (Ejercicios 21 al 30)
        // =====================================================================

        Ejercicio 21: Mostrar las 10 películas pertenecientes a la categoría Action.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Action"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 22: Mostrar las 5 películas más largas de la categoría Drama.
        public async Task<IActionResult> Ejercicio22()
        {
            var peliculas = await _context.Films
                .Join(
                    _context.FilmCategories,
                    film => film.FilmId,
                    filmCategory => filmCategory.FilmId,
                    (film, filmCategory) => new
                    {
                        Film = film,
                        FilmCategory = filmCategory
                    })
                .Join(
                    _context.Categories,
                    item => item.FilmCategory.CategoryId,
                    category => category.CategoryId,
                    (item, category) => new
                    {
                        item.Film,
                        Category = category
                    })
                .Where(item => item.Category.Name == "Drama")
                .OrderByDescending(item => item.Film.Length)
                .ThenBy(item => item.Film.FilmId)
                .Select(item => item.Film)
                .Take(5)
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .ToListAsync();
        
            ViewBag.PaginaActual = 1;
            ViewBag.TotalPaginas = 1;
        
            return View("Index", peliculas);
        }
        /*

        Ejercicio 23: Mostrar las 10 películas de categoría Comedy cuyo título contenga la letra A.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Comedy") && f.Title.ToUpper().Contains("A"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 24: Mostrar las 5 películas de categoría Horror omitiendo la primera.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Horror"))
                .OrderBy(f => f.Title)
                .Skip(1)
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }
        
        //Ejercicio 25: Mostrar las 10 películas de categoría Family ordenadas por título.
        public async Task<IActionResult> Ejercicio25()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.FilmCategories
                    .Any(fc => fc.Category.Name == "Family"))
                .OrderBy(f => f.Title)
                .Take(10)
                .ToListAsync();
        
            ViewBag.PaginaActual = 1;
            ViewBag.TotalPaginas = 1;
        
            return View("Index", peliculas);
        }

        /*
        Ejercicio 26: Mostrar las 10 películas de categoría Animation cuya duración sea mayor a 100 minutos.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Animation") && f.Length > 100)
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 27: Mostrar las 10 películas de categoría Action cuyo costo de reemplazo sea menor a 20 dólares.
        public async Task<IActionResult> Ejercicio27()
            {
                var peliculas = await _context.Films
                    .Include(f => f.Language)
                    .Include(f => f.OriginalLanguage)
                    .Where(f => f.FilmCategories
                                    .Any(fc => fc.Category.Name == "Action") &&
                                f.ReplacementCost < 20m)
                    .OrderBy(f => f.FilmId)
                    .Take(10)
                    .ToListAsync();
            
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            
                return View("Index", peliculas);
            }

        Ejercicio 28: Mostrar las 5 películas de categoría Comedy cuya duración sea mayor a 120 minutos.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Comedy") && f.Length > 120)
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 29: Mostrar las 10 películas de categoría Drama cuyo título empiece con la letra M.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Drama") && f.Title.ToUpper().StartsWith("M"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 30: Mostrar las 5 películas de categoría Family ordenadas por duración descendente.
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Family"))
                .OrderByDescending(f => f.Length)
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        // =====================================================================
        // CUARTIL IV – NIVEL DESAFÍO (Ejercicios 31 al 40)
        // =====================================================================

        Ejercicio 31: Mostrar las 10 películas en las que participe un actor cuyo apellido empiece con S.
        public async Task<IActionResult> Index31()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmActors.Any(fa => fa.Actor.LastName.ToUpper().StartsWith("S")))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 32: Mostrar las 5 películas en las que participe un actor cuyo nombre contenga JO.
        public async Task<IActionResult> Index32()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmActors.Any(fa => fa.Actor.FirstName.ToUpper().Contains("JO")))
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 33: Mostrar las 5 películas en las que participe un actor cuyo apellido termine con N.
        public async Task<IActionResult> Ejercicio33()
        {
            var peliculas = await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Where(f => f.FilmActors
                    .Any(fa => fa.Actor.LastName.ToUpper().EndsWith("N")))
                .OrderBy(f => f.FilmId)
                .Take(5)
                .ToListAsync();
        
            ViewBag.PaginaActual = 1;
            ViewBag.TotalPaginas = 1;
        
            return View("Index", peliculas);
        }

        /*
        Ejercicio 34: Mostrar las 10 películas en las que participe un actor cuyo nombre empiece con M y cuyo título contenga la letra A.
        public async Task<IActionResult> Index34()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmActors.Any(fa => fa.Actor.FirstName.ToUpper().StartsWith("M")) && f.Title.ToUpper().Contains("A"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 35: Mostrar las 5 películas de categoría Comedy en las que participe un actor cuyo apellido empiece con B.
        */public async Task<IActionResult> Ejercicio35()
            {
                var peliculas = await _context.Films
                    .Include(f => f.Language)
                    .Include(f => f.OriginalLanguage)
                    .Where(f => f.FilmCategories
                                    .Any(fc => fc.Category.Name == "Comedy") &&
                                f.FilmActors
                                    .Any(fa => fa.Actor.LastName.ToUpper()
                                        .StartsWith("B")))
                    .OrderBy(f => f.FilmId)
                    .Take(5)
                    .ToListAsync();
            
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            
                return View("Index", peliculas);
            }

        /*
        Ejercicio 36: Mostrar las 10 películas en las que participe un actor cuyo apellido empiece con C y cuya categoría sea Action.
        public async Task<IActionResult> Index36()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmActors.Any(fa => fa.Actor.LastName.ToUpper().StartsWith("C")) && f.FilmCategories.Any(fc => fc.Category.Name == "Action"))
                .Take(10)
                .ToListAsync();
            return View(peliculas);
        }

        Ejercicio 37: Mostrar las 5 películas en las que participe un actor cuyo nombre contenga AN y cuya categoría sea Drama.
        public async Task<IActionResult> Index37()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmActors.Any(fa => fa.Actor.FirstName.ToUpper().Contains("AN")) && f.FilmCategories.Any(fc => fc.Category.Name == "Drama"))
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }
        */
        

        /*Ejercicio 39: Mostrar las 5 películas de categoría Family en las que participe un actor cuyo nombre empiece con J.
        public async Task<IActionResult> Index39()
        {
            var peliculas = await _context.Films
                .Where(f => f.FilmCategories.Any(fc => fc.Category.Name == "Family") && f.FilmActors.Any(fa => fa.Actor.FirstName.ToUpper().StartsWith("J")))
                .Take(5)
                .ToListAsync();
            return View(peliculas);
        }
        /*
        Ejercicio 40: Mostrar las 10 películas de categoría Comedy en las que participe un actor cuyo apellido contenga la letra R y cuya duración sea mayor a 100 minutos.
        */public async Task<IActionResult> Ejercicio40()
            {
                var peliculas = await _context.Films
                    .Include(f => f.Language)
                    .Include(f => f.OriginalLanguage)
                    .Where(f => f.FilmCategories
                                    .Any(fc => fc.Category.Name == "Comedy") &&
                                f.FilmActors
                                    .Any(fa => fa.Actor.LastName.ToUpper()
                                        .Contains("R")) &&
                                f.Length > 100)
                    .OrderBy(f => f.FilmId)
                    .Take(10)
                    .ToListAsync();
            
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 1;
            
                return View("Index", peliculas);
            }


        /*public async Task<IActionResult> Index(
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
        */
        

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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador")]
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
     [Authorize(Roles = "Administrador")]
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
