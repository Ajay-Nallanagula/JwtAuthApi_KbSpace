using System.Collections.Generic;
using JWTAuthenticationApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthenticationApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Fruit")]
    public class FruitController : Controller
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IEnumerable<Fruit> Get()
        {
            var currentUser = HttpContext.User;
            var fruitList = new Fruit[]
            {
                new Fruit {Item = "Grapes", Description = "Citrus Fruit"},
                new Fruit {Item = "Apple", Description = "Round Red Apple"},
                new Fruit {Item = "Orange", Description = "Citrus Orange Rich in C Vitamin"},
                new Fruit {Item = "Banana", Description = "Yellow Fruit rich in fiber"},
            };
            return fruitList;
        }
    }
}