using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using APILibrary.Test.Mock;
using WebApplication.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using APILibrary.Test.Mock.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace APILibrary.Test
{
    public class Tests
    {

        private MockDbContext _db;
        private CustomersController _controller;

        [SetUp]
        public void Setup()
        {
            _db = MockDbContext.GetDbContext();

          
            var httpContext = new DefaultHttpContext();
            _controller = new CustomersController(_db)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };


        }

        [Test]
        public async Task TestGetAll()
        {
            var dictionary = new Dictionary<string, StringValues>();
            dictionary.Add("lastname", new StringValues("Charles"));
            _controller.Request.Query = new QueryCollection(dictionary);



            var actionResult = await _controller.GetAllAsync("", "", "", "");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            //_db.Customers.Count().Should().Be(values.Count());

        }

        [Test]
        public async Task TestGetBYId()
        {

            var actionResult = await _controller.GetById(2, "");
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        }

        //[Test]
        //public async Task TestGetNotFoundtById()
        //{

        //    var actionResult = await _controller.GetById(100, "");
        //    var result = actionResult.Result as ObjectResult;

        //    var okResult = result.Should().BeOfType<NotFoundResult>().Subject;

        //}

        [Test]
        public async Task TestPut()
        {
            CustomerMock customer = new CustomerMock
            {
                Email = "AliAhmadr@yahoo.fr",
                Phone = "65421895154",
                Lastname = "Fouret",
                Firstname = "Jeanne",
                Genre = "Autres",
                Address = null,
                ZipCode = "6854",
                City = "Limoges",
                ID = 2
            };

            var actionResult = await _controller.UpdateItem(2, customer);
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        }

        [Test]
        public async Task TestCreate()
        {
            CustomerMock customer = new CustomerMock
            {
                Email = "AliAhmadr@yahoo.fr",
                Phone = "65421895154",
                Lastname = "Maria",
                Firstname = "Julia",
                Genre = "Autres",
                Address = null,
                ZipCode = "6854",
                City = "Limoges"

            };

            var actionResult = await _controller.CreateItem(customer);
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<CreatedResult>().Subject;

        }


        [Test]
        public async Task TestDelete()
        {

            // Penser à Changer la valeur retounée de la DeleteItem
            var actionResult = await _controller.RemoveItem(1);
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        }


        [Test]
        public async Task TestGetAllWithParams()
        {
            var dictionary = new Dictionary<string, StringValues>();
            dictionary.Add("lastname", new StringValues("Charles"));
            _controller.Request.Query = new QueryCollection(dictionary);

            var actionResult = await _controller.GetAllAsync("lastname", "", "", "");
            var result = actionResult.Result as ObjectResult;
            var values = ((IEnumerable<object>)(result).Value);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

           // _db.Customers.Count().Should().Be(values.Count());

        }
        [Test]
        public async Task TestSearch()
        {
            var dictionary = new Dictionary<string, StringValues>();
            dictionary.Add("lastname", new StringValues("Charles"));
            _controller.Request.Query = new QueryCollection(dictionary);


            var actionResult = await _controller.SearchAsync("lastname,firstname", "1-3", "", "lastname");
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

          

        }
        /*

         public async Task TestNotFoundSearch()
            {


                var actionResult = await _controller.Search("Charles", "Homme", "");
                var result = actionResult.Result as ObjectResult;

                var okResult = result.Should().BeOfType<NotFoundResult>().Subject;

            }

            


        [Test]
        public async Task TestSort()
        {


            var actionResult = await _controller.Sort("", "lastname", "");
            var result = actionResult.Result as ObjectResult;

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        }
        */
        //[Test]
        //public async Task TestSearch()
        //{

        //    // Penser à Changer la valeur retounée de la DeleteItem
        //    var actionResult = await _controller.Search( "Charles", "Homme", "");
        //    var result = actionResult.Result as ObjectResult;

        //    var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        //}

    }
}